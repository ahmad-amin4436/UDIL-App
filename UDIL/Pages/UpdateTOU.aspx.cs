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
    public partial class UpdateTOU : System.Web.UI.Page
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
                    tsPrivateKey.Text = Session["PrivateKey"].ToString();
                }

                // Generate unique transaction ID
                tsTransactionId.Text = AppCommon.GenerateTransactionId();
            }
        }




        protected void btnUpdateTOU_Click(object sender, EventArgs e)
        {
            lblUpdateTOUMessage.Text = string.Empty;
            // Store Update TOU request time
            Session["UpdateTOURequestTime"] = DateTime.Now;
            // Generate new transaction ID for this request
            string transactionId = tsTransactionId.Text.Trim();
            string globalDeviceId = tsGlobalDeviceId.Text.Trim();
            string globalDeviceIdArray = $"[\"{globalDeviceId}\"]";
            string requestDateTime = tsRequestDateTime.Text.Trim();
            string activationDateTime = tsActivationDateTime.Text.Trim();

            string privateKey = Session["PrivateKey"] as string;

            if (string.IsNullOrEmpty(privateKey))
            {
                lblUpdateTOUMessage.Text = "Please authorize first to obtain a private key.";
                lblUpdateTOUMessage.CssClass = "text-danger";
                return;
            }

            tsPrivateKey.Text = privateKey;

            if (string.IsNullOrEmpty(transactionId) || string.IsNullOrEmpty(globalDeviceId) || string.IsNullOrEmpty(requestDateTime) || 
                string.IsNullOrEmpty(activationDateTime))
            {
                lblUpdateTOUMessage.Text = "Transaction ID, Global Device ID, Request DateTime, and Activation DateTime are required.";
                lblUpdateTOUMessage.CssClass = "text-danger";
                return;
            }

            // Build JSON from individual input fields
            string dayProfile, weekProfile, seasonProfile, holidayProfile;
            
            try
            {
                // Build Day Profile JSON
                var dayProfileArray = new[]
                {
                    new { name = tsDay1Name.Text.Trim(), tariff_slabs = tsDay1Tariff.Text.Trim().Split(',').Select(s => s.Trim()).ToArray() },
                    new { name = tsDay2Name.Text.Trim(), tariff_slabs = tsDay2Tariff.Text.Trim().Split(',').Select(s => s.Trim()).ToArray() },
                    new { name = tsDay3Name.Text.Trim(), tariff_slabs = tsDay3Tariff.Text.Trim().Split(',').Select(s => s.Trim()).ToArray() },
                    new { name = tsDay4Name.Text.Trim(), tariff_slabs = tsDay4Tariff.Text.Trim().Split(',').Select(s => s.Trim()).ToArray() }
                };
                dayProfile = new JavaScriptSerializer().Serialize(dayProfileArray);

                // Build Week Profile JSON
                var weekProfileArray = new[]
                {
                    new { name = tsWeek1Name.Text.Trim(), weekly_day_profile = tsWeek1Days.Text.Trim().Split(',').Select(s => s.Trim()).ToArray() },
                    new { name = tsWeek2Name.Text.Trim(), weekly_day_profile = tsWeek2Days.Text.Trim().Split(',').Select(s => s.Trim()).ToArray() },
                    new { name = tsWeek3Name.Text.Trim(), weekly_day_profile = tsWeek3Days.Text.Trim().Split(',').Select(s => s.Trim()).ToArray() },
                    new { name = tsWeek4Name.Text.Trim(), weekly_day_profile = tsWeek4Days.Text.Trim().Split(',').Select(s => s.Trim()).ToArray() }
                };
                weekProfile = new JavaScriptSerializer().Serialize(weekProfileArray);

                // Build Season Profile JSON
                var seasonProfileArray = new[]
                {
                    new { name = tsSeason1Name.Text.Trim(), week_profile_name = tsSeason1Week.Text.Trim(), start_date = tsSeason1Date.Text.Trim() },
                    new { name = tsSeason2Name.Text.Trim(), week_profile_name = tsSeason2Week.Text.Trim(), start_date = tsSeason2Date.Text.Trim() },
                    new { name = tsSeason3Name.Text.Trim(), week_profile_name = tsSeason3Week.Text.Trim(), start_date = tsSeason3Date.Text.Trim() },
                    new { name = tsSeason4Name.Text.Trim(), week_profile_name = tsSeason4Week.Text.Trim(), start_date = tsSeason4Date.Text.Trim() }
                };
                seasonProfile = new JavaScriptSerializer().Serialize(seasonProfileArray);

                // Build Holiday Profile JSON
                var holidayProfileArray = new[]
                {
                    new { name = tsHoliday1Name.Text.Trim(), date = tsHoliday1Date.Text.Trim(), day_profile_name = tsHoliday1Day.Text.Trim() },
                    new { name = tsHoliday2Name.Text.Trim(), date = tsHoliday2Date.Text.Trim(), day_profile_name = tsHoliday2Day.Text.Trim() },
                    new { name = tsHoliday3Name.Text.Trim(), date = tsHoliday3Date.Text.Trim(), day_profile_name = tsHoliday3Day.Text.Trim() }
                };
                holidayProfile = new JavaScriptSerializer().Serialize(holidayProfileArray);
            }
            catch (Exception ex)
            {
                lblUpdateTOUMessage.Text = "Error building JSON data: " + ex.Message;
                lblUpdateTOUMessage.CssClass = "text-danger";
                return;
            }

            string postData = $"global_device_id={HttpUtility.UrlEncode(globalDeviceIdArray)}&request_datetime={HttpUtility.UrlEncode(requestDateTime)}&activation_datetime={HttpUtility.UrlEncode(activationDateTime)}&day_profile={HttpUtility.UrlEncode(dayProfile)}&week_profile={HttpUtility.UrlEncode(weekProfile)}&season_profile={HttpUtility.UrlEncode(seasonProfile)}&holiday_profile={HttpUtility.UrlEncode(holidayProfile)}";

            try
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Starting Update TOU for transaction: {transactionId}");

                UpdateTOUResponse updateTouResponse = PostUpdateTOU(transactionId, privateKey, postData);

                System.Diagnostics.Debug.WriteLine($"[DEBUG] PostUpdateTOU returned: {(updateTouResponse == null ? "NULL" : $"status={updateTouResponse.status}, message={updateTouResponse.message}")}");

                if (updateTouResponse == null)
                {
                    lblUpdateTOUMessage.Text = "Failed to get a response from the update TOU endpoint.";
                    lblUpdateTOUMessage.CssClass = "text-danger";
                    return;
                }

                if (updateTouResponse.status != "1")
                {
                    lblUpdateTOUMessage.Text = $"Update TOU failed: {updateTouResponse.message}";
                    lblUpdateTOUMessage.CssClass = "text-danger";
                    return;
                }

                TransactionStatusResponse statusResponse = AppCommon.PollTransactionStatus(transactionId, privateKey);

                if (statusResponse == null)
                {
                    lblUpdateTOUMessage.Text = "Update TOU accepted, but transaction status could not be retrieved.";
                    lblUpdateTOUMessage.CssClass = "text-warning";
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
                    lblUpdateTOUMessage.Text = $"Update TOU failed. Command was cancelled by the meter (indv_status: 2, status_level: 4). Status: {stageText}.";
                    lblUpdateTOUMessage.CssClass = "text-danger";

                    // Show cancelled tracker briefly
                    pnlTracker.Visible = true;
                    lblTrackerTransactionId.Text = transactionId;
                    UpdateTrackerUI(-1); // Show cancelled stage
                    lblStageDescription.Text = "Command was cancelled by the meter (indv_status: 2, status_level: 4).";
                    lblStageDescription.CssClass = "text-danger";

                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "disableUpdateTOUBtn", "disableUpdateTOUButton();", true);
                }
                else
                {
                    string stageText = AppCommon.GetStageDescription(maxStatusLevel);
                    lblUpdateTOUMessage.Text = $"Update TOU succeeded. Transaction status: {statusResponse.status}. Latest status level: {maxStatusLevel} ({stageText}).";
                    lblUpdateTOUMessage.CssClass = "text-success";

                    // After success
                    Session["CurrentTransactionId"] = transactionId;
                    Session["CurrentPrivateKey"] = privateKey;

                    pnlTracker.Visible = true;
                    lblTrackerTransactionId.Text = transactionId;

                    timerTracker.Enabled = true;
                    // Hide loader after work
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "disableUpdateTOUBtn", "disableUpdateTOUButton();", true);
                }
            }
            catch (WebException ex)
            {
                lblUpdateTOUMessage.Text = "HTTP Error: " + ex.Message;
                lblUpdateTOUMessage.CssClass = "text-danger";
            }
            catch (Exception ex)
            {
                lblUpdateTOUMessage.Text = "Error: " + ex.Message;
                lblUpdateTOUMessage.CssClass = "text-danger";
            }
        }

        private string ValidateAndFormatJson(string jsonText, string fieldName)
        {
            if (string.IsNullOrEmpty(jsonText))
            {
                throw new ArgumentException($"{fieldName} cannot be empty.");
            }

            try
            {
                // Try to parse JSON to validate it
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                object parsedJson = serializer.Deserialize<object>(jsonText);
                
                // Return the original text if it's valid JSON
                return jsonText;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"{fieldName} contains invalid JSON format: {ex.Message}");
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

            var statusResponse = AppCommon.GetTransactionStatus(transactionId, privateKey);

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
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "resetUpdateTOULoading", "resetUpdateTOULoading();", true);
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
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "resetUpdateTOULoading", "resetUpdateTOULoading();", true);

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

        private IPPortUpdateResponse PostIPPortUpdate(string transactionId, string privateKey, string postData)
        {
            string baseUrl = GetBaseUrl();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/update_ip_port");
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
                    return serializer.Deserialize<IPPortUpdateResponse>(responseContent);
                }
            }
            catch (WebException ex)
            {
                string body = AppCommon.ReadWebExceptionResponse(ex);
                throw new WebException($"IP port update request failed: {ex.Message}. Response body: {body}", ex);
            }
        }

        private AuxRelayOperationsResponse PostAuxRelayOperations(string transactionId, string privateKey, string postData)
        {
            string baseUrl = GetBaseUrl();
            string url = $"{baseUrl}/aux_relay_operations";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
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
                    return serializer.Deserialize<AuxRelayOperationsResponse>(responseContent);
                }
            }
            catch (WebException ex)
            {
                string body = AppCommon.ReadWebExceptionResponse(ex);
                throw new WebException($"Aux relay operations request failed: {ex.Message}. Response body: {body}", ex);
            }
        }

        private UpdateTOUResponse PostUpdateTOU(string transactionId, string privateKey, string postData)
        {
            string baseUrl = GetBaseUrl();
            string url = $"{baseUrl}/update_time_of_use";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
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
                    return serializer.Deserialize<UpdateTOUResponse>(responseContent);
                }
            }
            catch (WebException ex)
            {
                string body = AppCommon.ReadWebExceptionResponse(ex);
                throw new WebException($"Update TOU request failed: {ex.Message}. Response body: {body}", ex);
            }
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

            int percent = (level * 100) / 8;
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

        public class TimeSyncResponse
        {
            public string status { get; set; }
            public string transactionid { get; set; }
            public string message { get; set; }
            public object data { get; set; }
        }

        public class IPPortUpdateResponse
        {
            public string status { get; set; }
            public string transactionid { get; set; }
            public string message { get; set; }
            public object data { get; set; }
        }

        public class AuxRelayOperationsResponse
        {
            public string status { get; set; }
            public string transactionid { get; set; }
            public string message { get; set; }
            public object data { get; set; }
        }

        public class UpdateTOUResponse
        {
            public string status { get; set; }
            public string transactionid { get; set; }
            public string message { get; set; }
            public object data { get; set; }
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