using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using UDIL.Shared;

namespace UDIL.Pages
{
    public partial class UpdateDeviceMetadata : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["GetTransactionStatus"] == "true" && !string.IsNullOrEmpty(Request["transactionId"]))
            {
                string privateKey = Session["PrivateKey"] as string;
                AppCommon.HandleTransactionStatusRequest(Request["transactionId"].ToString(), privateKey);
                return;
            }

            if (!IsPostBack)
            {
                ScriptManager.RegisterClientScriptInclude(this.Page, this.Page.GetType(), "udil-tester", ResolveUrl("~/udil-tester.js"));
                if (Session["PrivateKey"] != null)
                {
                    dmPrivateKey.Text = Session["PrivateKey"].ToString();
                }

                // Generate unique transaction ID
                dmTransactionId.Text = AppCommon.GenerateTransactionId();
            }
        }


        protected void btnUpdateDeviceMetadata_Click(object sender, EventArgs e)
        {
            lblUpdateDeviceMetadataMessage.Text = string.Empty;
            // Store Update Device Metadata request time
            Session["UpdateDeviceMetadataRequestTime"] = DateTime.Now;
            // Generate new transaction ID for this request
            string transactionId = dmTransactionId.Text.Trim();
            string globalDeviceId = dmGlobalDeviceId.Text.Trim();
            string globalDeviceIdArray = $"[\"{globalDeviceId}\"]";
            string requestDateTime = dmRequestDateTime.Text.Trim();
            string communicationMode = dmCommunicationMode.SelectedValue;
            string bidirectionalDevice = dmBidirectionalDevice.SelectedValue;
            string communicationType = dmCommunicationType.SelectedValue;
            string initialCommunicationTime = dmInitialCommunicationTime.Text.Trim();
            string communicationInterval = dmCommunicationInterval.Text.Trim();
            string meterType = dmMeterType.SelectedValue;
            string phase = dmPhase.SelectedValue;

            string privateKey = Session["PrivateKey"] as string;

            if (string.IsNullOrEmpty(privateKey))
            {
                lblUpdateDeviceMetadataMessage.Text = "Please authorize first to obtain a private key.";
                lblUpdateDeviceMetadataMessage.CssClass = "text-danger";
                return;
            }

            dmPrivateKey.Text = privateKey;

            if (string.IsNullOrEmpty(transactionId) || string.IsNullOrEmpty(globalDeviceId) || string.IsNullOrEmpty(requestDateTime) ||
                string.IsNullOrEmpty(communicationMode) || string.IsNullOrEmpty(bidirectionalDevice) || string.IsNullOrEmpty(communicationType) ||
                string.IsNullOrEmpty(initialCommunicationTime) || string.IsNullOrEmpty(communicationInterval) || string.IsNullOrEmpty(meterType) || string.IsNullOrEmpty(phase))
            {
                lblUpdateDeviceMetadataMessage.Text = "All fields are required: Transaction ID, Global Device ID, Request DateTime, Communication Mode, Bidirectional Device, Communication Type, Initial Communication Time, Communication Interval, Meter Type, and Phase.";
                lblUpdateDeviceMetadataMessage.CssClass = "text-danger";
                return;
            }

            string postData = $"global_device_id={HttpUtility.UrlEncode(globalDeviceIdArray)}&request_datetime={HttpUtility.UrlEncode(requestDateTime)}&communication_mode={HttpUtility.UrlEncode(communicationMode)}&bidirectional_device={HttpUtility.UrlEncode(bidirectionalDevice)}&communication_type={HttpUtility.UrlEncode(communicationType)}&initial_communication_time={HttpUtility.UrlEncode(initialCommunicationTime)}&communication_interval={HttpUtility.UrlEncode(communicationInterval)}&meter_type={HttpUtility.UrlEncode(meterType)}&phase={HttpUtility.UrlEncode(phase)}";

            try
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Starting Update Device Metadata for transaction: {transactionId}");

                UpdateDeviceMetadataResponse updateResponse = PostUpdateDeviceMetadata(transactionId, privateKey, postData);

                System.Diagnostics.Debug.WriteLine($"[DEBUG] PostUpdateDeviceMetadata returned: {(updateResponse == null ? "NULL" : $"status={updateResponse.status}, message={updateResponse.message}")}");

                if (updateResponse == null)
                {
                    lblUpdateDeviceMetadataMessage.Text = "Failed to get a response from the update device metadata endpoint.";
                    lblUpdateDeviceMetadataMessage.CssClass = "text-danger";
                    return;
                }

                if (updateResponse.status != "1")
                {
                    lblUpdateDeviceMetadataMessage.Text = $"Update device metadata failed: {updateResponse.message}";
                    lblUpdateDeviceMetadataMessage.CssClass = "text-danger";
                    return;
                }

                TransactionStatusResponse statusResponse = GetTransactionStatus(transactionId, privateKey);

                if (statusResponse == null)
                {
                    lblUpdateDeviceMetadataMessage.Text = "Update device metadata accepted, but transaction status could not be retrieved.";
                    lblUpdateDeviceMetadataMessage.CssClass = "text-warning";
                    return;
                }

                int maxStatusLevel = 0;
                bool commandCancelled = false;
                if (statusResponse.data != null)
                {
                    foreach (var item in statusResponse.data)
                    {
                        if (int.TryParse(item.status_level, out int level) && level > maxStatusLevel)
                        {
                            maxStatusLevel = level;
                        }

                        // Check if command should be cancelled by meter
                        if (item.indv_status == "2" && item.status_level == "4")
                        {
                            commandCancelled = true;
                            break; // Exit loop since we found cancellation
                        }
                    }
                }

                if (commandCancelled)
                {
                    // Handle command cancellation
                    string stageText = AppCommon.GetStageDescription(-1); // Use cancelled stage
                    lblUpdateDeviceMetadataMessage.Text = $"Update device metadata failed. Command was cancelled by the meter (indv_status: 2, status_level: 4). Status: {stageText}.";
                    lblUpdateDeviceMetadataMessage.CssClass = "text-danger";

                    // Show cancelled tracker briefly
                    pnlTracker.Visible = true;
                    lblTrackerTransactionId.Text = transactionId;
                    UpdateTrackerUI(-1); // Show cancelled stage
                    lblStageDescription.Text = "Command was cancelled by the meter (indv_status: 2, status_level: 4).";
                    lblStageDescription.CssClass = "text-danger";

                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "disableUpdateDeviceMetadataBtn", "disableUpdateDeviceMetadataButton();", true);
                }
                else
                {
                    string stageText = AppCommon.GetStageDescription(maxStatusLevel);
                    lblUpdateDeviceMetadataMessage.Text = $"Update device metadata succeeded. Transaction status: {statusResponse.status}. Latest status level: {maxStatusLevel} ({stageText}).";
                    lblUpdateDeviceMetadataMessage.CssClass = "text-success";

                    // After success
                    Session["CurrentTransactionId"] = transactionId;
                    Session["CurrentPrivateKey"] = privateKey;

                    pnlTracker.Visible = true;
                    lblTrackerTransactionId.Text = transactionId;

                    timerTracker.Enabled = true;
                    // Hide loader after work
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "disableUpdateDeviceMetadataBtn", "disableUpdateDeviceMetadataButton();", true);
                }
            }
            catch (WebException ex)
            {
                lblUpdateDeviceMetadataMessage.Text = "HTTP Error: " + ex.Message;
                lblUpdateDeviceMetadataMessage.CssClass = "text-danger";
            }
            catch (Exception ex)
            {
                lblUpdateDeviceMetadataMessage.Text = "Error: " + ex.Message;
                lblUpdateDeviceMetadataMessage.CssClass = "text-danger";
            }
        }
        private string NormalizeTimeWithSeconds(string timeValue)
        {
            if (string.IsNullOrEmpty(timeValue))
            {
                return timeValue;
            }

            string[] parts = timeValue.Split(':');
            if (parts.Length == 2)
            {
                string hour = parts[0].PadLeft(2, '0');
                string minute = parts[1].PadLeft(2, '0');
                return $"{hour}:{minute}:00";
            }

            return timeValue;
        }
        protected void timerTracker_Tick(object sender, EventArgs e)
        {
            string transactionId = Session["CurrentTransactionId"] as string;
            string privateKey = Session["CurrentPrivateKey"] as string;

            var statusResponse = GetTransactionStatus(transactionId, privateKey);

            if (statusResponse?.data == null)
                return;

            int maxLevel = 0;

            foreach (var item in statusResponse.data)
            {
                if (int.TryParse(item.status_level, out int lvl) && lvl > maxLevel)
                    maxLevel = lvl;

                // Check if command should be cancelled by meter
                if (item.indv_status == "2" && item.status_level == "4")
                {
                    // Command cancelled by meter - stop tracking and show cancellation message
                    timerTracker.Enabled = false;
                    UpdateTrackerUI(-1); // Use cancelled stage
                    lblStageDescription.Text = "Command was cancelled by the meter (indv_status: 2, status_level: 4).";
                    lblStageDescription.CssClass = "text-danger";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "resetUpdateDeviceMetadataLoading", "resetUpdateDeviceMetadataLoading();", true);
                    return; // Exit early since command was cancelled
                }
            }

            // Get current stage from session or initialize
            int currentStage = Session["CurrentStage"] != null ? (int)Session["CurrentStage"] : 0;
            DateTime stageStartTime = Session["StageStartTime"] != null ? (DateTime)Session["StageStartTime"] : DateTime.Now;

            // Check if stage has changed
            if (maxLevel != currentStage)
            {
                // Stage has changed, update session variables
                Session["CurrentStage"] = maxLevel;
                Session["StageStartTime"] = DateTime.Now;
            }
            else
            {
                // Check for timeout (5 minutes = 300 seconds)
                TimeSpan timeInStage = DateTime.Now - stageStartTime;
                if (timeInStage.TotalMinutes >= 5)
                {
                    // Stage timeout reached, mark as failed and move to next stage
                    int nextStage = currentStage + 1;
                    if (nextStage <= 8)
                    {
                        Session["CurrentStage"] = nextStage;
                        Session["StageStartTime"] = DateTime.Now;
                        maxLevel = nextStage;

                        // Update UI to show failed stage
                        UpdateTrackerUIWithTimeout(currentStage, nextStage);
                        lblStageDescription.Text = $"Stage {currentStage} failed due to timeout (5 minutes). Moving to stage {nextStage}.";
                        lblStageDescription.CssClass = "text-warning";
                    }
                }
            }
            
            UpdateTrackerUI(maxLevel);

            // Save current state to ViewState

            if (maxLevel >= 5)
            {
                timerTracker.Enabled = false;
                lblStage.Text = "Completed";
                lblStage.CssClass = "badge bg-success px-3 py-2";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "resetUpdateDeviceMetadataLoading", "resetUpdateDeviceMetadataLoading();", true);

                // Hide tracker only when level 8 is reached
                //pnlTracker.Visible = false;

                // Clear ViewState when completed
                ViewState["TrackerLevel"] = null;
                ViewState["TrackerTransactionId"] = null;

                // Clear session variables
                Session["CurrentStage"] = null;
                Session["StageStartTime"] = null;
            }
        }
        private TransactionStatusResponse GetTransactionStatus(string transactionId, string privateKey)
        {
            int maxRetries = 3;
            int timeoutMs = GetTimeout() * 1000; // Convert seconds to milliseconds

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    string baseUrl = GetBaseUrl();
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/transaction_status");
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.Accept = "*/*";
                    request.Headers.Add("transactionid", transactionId);
                    request.Headers.Add("privatekey", privateKey);
                    request.ContentLength = 0;
                    request.UserAgent = "UDILTester/1.0";

                    // Set timeout values (in milliseconds)
                    request.Timeout = timeoutMs;
                    request.ReadWriteTimeout = timeoutMs;

                    // Enable keep-alive
                    request.KeepAlive = true;
                    request.ServicePoint.Expect100Continue = false;
                    request.ServicePoint.UseNagleAlgorithm = false;

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseContent = reader.ReadToEnd();
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        return serializer.Deserialize<TransactionStatusResponse>(responseContent);
                    }
                }
                catch (WebException ex)
                {
                    string body = ReadWebExceptionResponse(ex);

                    // Check if it's a 401 Unauthorized with private key timeout
                    if (ex.Response is HttpWebResponse httpResponse && httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        // Try to re-authorize with fixed credentials
                        if (ReauthorizeWithFixedCredentials())
                        {
                            // Re-authorization successful, get the new private key and retry immediately
                            privateKey = Session["PrivateKey"] as string;
                            continue; // Skip the sleep and retry immediately
                        }//
                    }

                    // If this is the last attempt, throw the exception
                    if (attempt == maxRetries)
                    {
                        //throw new WebException($"Transaction status request failed after {maxRetries} attempts. Last error: {ex.Message}. Response body: {body}", ex);
                    }

                    // Log the retry attempt (you can replace this with proper logging)
                    System.Threading.Thread.Sleep(2000 * attempt); // Wait before retry: 2s, 4s, 6s
                }
            }

            throw new WebException("Transaction status request failed: Maximum retry attempts exceeded.");
        }
        private bool ReauthorizeWithFixedCredentials()
        {
            try
            {
                string baseUrl = GetBaseUrl();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/authorization_service");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("username", "Hello@123");
                request.Headers.Add("password", "123");
                request.Headers.Add("code", "235");
                request.Timeout = 30000;
                request.ReadWriteTimeout = 30000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseContent = reader.ReadToEnd();
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    AuthResponse authResponse = serializer.Deserialize<AuthResponse>(responseContent);

                    if (authResponse.status == "1")
                    {
                        Session["PrivateKey"] = authResponse.privatekey;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log re-authorization failure if needed
                System.Diagnostics.Debug.WriteLine($"Re-authorization failed: {ex.Message}");
            }

            return false;
        }

        private void UpdateTrackerUI(int level)
        {
            ResetSteps();

            // Handle cancelled stage specially
            if (level == -1)
            {
                // For cancelled stage, mark all steps as incomplete and show cancellation
                for (int i = 0; i <= 8; i++)
                {
                    var step = pnlTracker.FindControl("step" + i) as Label;
                    if (step != null)
                        step.CssClass = "tracker-step cancelled";
                }

                lblStage.Text = "Cancelled";
                lblStage.CssClass = "badge bg-danger px-3 py-2";
                lblStageDescription.Text = AppCommon.GetStageDescription(level);
                lblStageDescription.CssClass = "text-danger";
                progressBar.Style["width"] = "0%";
                return;
            }

            for (int i = 0; i <= 8; i++)
            {
                var step = pnlTracker.FindControl("step" + i) as Label;

                if (step == null) continue;

                if (i < level)
                    step.CssClass = "tracker-step completed";
                else if (i == level)
                    step.CssClass = "tracker-step active";
                else
                    step.CssClass = "tracker-step";
            }

            lblStage.Text = $"Stage {level}";
            lblStage.CssClass = "badge bg-primary px-3 py-2";
            lblStageDescription.Text = AppCommon.GetStageDescription(level);
            lblStageDescription.CssClass = "text-muted";

            int percent = (level * 100) / 7;
            progressBar.Style["width"] = percent + "%";
        }
        private void ResetSteps()
        {
            for (int i = 0; i <= 8; i++)
            {
                var step = pnlTracker.FindControl("step" + i) as Label;
                if (step != null)
                    step.CssClass = "tracker-step";
            }
        }

        private void UpdateTrackerUIWithTimeout(int failedStage, int currentStage)
        {
            ResetSteps();

            // Mark completed stages
            for (int i = 0; i < failedStage; i++)
            {
                var step = pnlTracker.FindControl("step" + i) as Label;
                if (step != null)
                    step.CssClass = "tracker-step completed";
            }

            // Mark failed stage
            var failedStep = pnlTracker.FindControl("step" + failedStage) as Label;
            if (failedStep != null)
                failedStep.CssClass = "tracker-step failed";

            // Mark current active stage
            var activeStep = pnlTracker.FindControl("step" + currentStage) as Label;
            if (activeStep != null)
                activeStep.CssClass = "tracker-step active";

            lblStage.Text = $"Stage {currentStage}";
            lblStage.CssClass = "badge bg-warning px-3 py-2";

            int percent = (currentStage * 100) / 5;
            progressBar.Style["width"] = percent + "%";
        }

        private string ExtractGlobalDeviceId(string deviceIdentityJson)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceIdentityJson))
                    return string.Empty;

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var deviceIdentity = serializer.Deserialize<List<Dictionary<string, object>>>(deviceIdentityJson);

                if (deviceIdentity != null && deviceIdentity.Count > 0)
                {
                    if (deviceIdentity[0].ContainsKey("global_device_id"))
                    {
                        return deviceIdentity[0]["global_device_id"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error extracting global_device_id: {ex.Message}");
            }

            return string.Empty;
        }
        private string ReadWebExceptionResponse(WebException ex)
        {
            if (ex.Response == null)
            {
                return string.Empty;
            }

            try
            {
                using (var responseStream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(responseStream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                return string.Empty;
            }
        }
        private UpdateDeviceMetadataResponse PostUpdateDeviceMetadata(string transactionId, string privateKey, string postData)
        {
            string baseUrl = GetBaseUrl();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/update_device_metadata");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Headers.Add("transactionid", transactionId);
            request.Headers.Add("privatekey", privateKey);
            request.UserAgent = "UDILTester/1.0";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(postData);
            request.ContentLength = requestBytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(requestBytes, 0, requestBytes.Length);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseContent = reader.ReadToEnd();
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    return serializer.Deserialize<UpdateDeviceMetadataResponse>(responseContent);
                }
            }
            catch (WebException ex)
            {
                string body = ReadWebExceptionResponse(ex);
                throw new WebException($"Update device metadata request failed: {ex.Message}. Response body: {body}", ex);
            }
        }
        private string GetBaseUrl()
        {
            return UDIL.Shared.ConfigurationManager.GetBaseUrl();
        }

        private int GetTimeout()
        {
            return UDIL.Shared.ConfigurationManager.GetTimeout();
        }

        private string GetConnectionString()
        {
            return UDIL.Shared.ConfigurationManager.GetConnectionString();
        }

        public class AuthResponse
        {
            public string status { get; set; }
            public string privatekey { get; set; }
            public string message { get; set; }
        }
        public class UpdateDeviceMetadataResponse
        {
            public string status { get; set; }
            public string transactionid { get; set; }
            public string message { get; set; }
            public object data { get; set; }
        }

        public class TransactionStatusResponse
        {
            public string status { get; set; }
            public string transactionid { get; set; }
            public string message { get; set; }
            public List<TransactionStatusData> data { get; set; }
        }

        public class TransactionStatusData
        {
            public string global_device_id { get; set; }
            public string msn { get; set; }
            public string type { get; set; }
            public string transactionid { get; set; }
            public string request_cancel_reason { get; set; }
            public string status_4_datetime { get; set; }
            public string response_data { get; set; }
            public string status_1_datetime { get; set; }
            public string command_receiving_datetime { get; set; }
            public string indv_status { get; set; }
            public string request_cancel_datetime { get; set; }
            public string status_5_datetime { get; set; }
            public string status_2_datetime { get; set; }
            public string status_level { get; set; }
            public string request_cancelled { get; set; }
            public string status_3_datetime { get; set; }
        }
    }
}