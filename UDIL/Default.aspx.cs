using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace UDIL
{
    public partial class _Default : Page
    {
        protected TextBox txtUsername;
        protected TextBox txtPassword;
        protected TextBox txtCode;
        protected Label lblAuthMessage;
        protected Button btnAuthorize;

        protected TextBox dcTransactionId;
        protected TextBox dcPrivateKey;
        protected TextBox dcDeviceIdentity;
        protected TextBox dcRequestDateTime;
        protected DropDownList dcDeviceType;
        protected TextBox dcMdiResetDate;
        protected TextBox dcMdiResetTime;
        protected DropDownList dcBidirectionalDevice;
        protected TextBox dcSimNumber;
        protected TextBox dcSimId;
        protected DropDownList dcPhase;
        protected DropDownList dcMeterType;
        protected DropDownList dcCommunicationMode;
        protected DropDownList dcCommunicationType;
        protected TextBox dcCommunicationInterval;
        protected TextBox dcInitialCommunicationTime;
        protected Button btnDeviceCreation;
        protected Label lblDeviceCreationMessage;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Handle AJAX requests for transaction status
            if (Request["GetTransactionStatus"] == "true" && !string.IsNullOrEmpty(Request["transactionId"]))
            {
                HandleTransactionStatusRequest();
                return;
            }

            if (!IsPostBack)
            {
                ScriptManager.RegisterClientScriptInclude(this.Page, this.Page.GetType(), "udil-tester", ResolveUrl("~/udil-tester.js"));
                if (Session["PrivateKey"] != null)
                {
                    dcPrivateKey.Text = Session["PrivateKey"].ToString();
                }
            }
        }

        private void HandleTransactionStatusRequest()
        {
            try
            {
                string transactionId = Request["transactionId"];
                string privateKey = Session["PrivateKey"] as string;

                if (string.IsNullOrEmpty(privateKey))
                {
                    Response.Clear();
                    Response.ContentType = "application/json";
                    Response.Write("{\"status\":\"error\",\"message\":\"Not authenticated\"}");
                    Response.End();
                    return;
                }

                TransactionStatusResponse statusResponse = GetTransactionStatus(transactionId, privateKey);
                
                Response.Clear();
                Response.ContentType = "application/json";
                
                if (statusResponse != null)
                {
                    var jsonResponse = new
                    {
                        status = "success",
                        data = statusResponse.data,
                        message = statusResponse.message
                    };
                    Response.Write(new JavaScriptSerializer().Serialize(jsonResponse));
                }
                else
                {
                    Response.Write("{\"status\":\"error\",\"message\":\"Failed to get transaction status\"}");
                }
                
                Response.End();
            }
            catch (Exception ex)
            {
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write($"{{\"status\":\"error\",\"message\":\"{ex.Message.Replace("\"", "\\\"")}\"}}");
                Response.End();
            }
        }

        protected void btnAuthorize_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string code = txtCode.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(code))
            {
                lblAuthMessage.Text = "Please fill in all fields.";
                lblAuthMessage.CssClass = "text-danger";
                return;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://116.58.46.245:4050/UIP/authorization_service");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("username", username);
            request.Headers.Add("password", password);
            request.Headers.Add("code", code);

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseContent = reader.ReadToEnd();

                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        AuthResponse authResponse = serializer.Deserialize<AuthResponse>(responseContent);

                        if (authResponse.status == "1")
                        {
                            Session["PrivateKey"] = authResponse.privatekey;
                            lblAuthMessage.Text = authResponse.message;
                            lblAuthMessage.CssClass = "text-success";
                        }
                        else
                        {
                            lblAuthMessage.Text = "Authorization failed: " + authResponse.message;
                            lblAuthMessage.CssClass = "text-danger";
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                lblAuthMessage.Text = "HTTP Error: " + ex.Message;
                lblAuthMessage.CssClass = "text-danger";
            }
            catch (Exception ex)
            {
                lblAuthMessage.Text = "Error: " + ex.Message;
                lblAuthMessage.CssClass = "text-danger";
            }
        }

        protected void btnDeviceCreation_Click(object sender, EventArgs e)
        {
            lblDeviceCreationMessage.Text = string.Empty;
            string transactionId = dcTransactionId.Text.Trim();
            string deviceIdentity = dcDeviceIdentity.Text.Trim();
            string requestDateTime = dcRequestDateTime.Text.Trim();
            string deviceType = dcDeviceType.SelectedValue;
            string mdiResetDate = dcMdiResetDate.Text.Trim();
            string mdiResetTime = NormalizeTimeWithSeconds(dcMdiResetTime.Text.Trim());
            string bidirectionalDevice = dcBidirectionalDevice.SelectedValue;
            string simNumber = dcSimNumber.Text.Trim();
            string simId = dcSimId.Text.Trim();
            string phase = dcPhase.SelectedValue;
            string meterType = dcMeterType.SelectedValue;
            string communicationMode = dcCommunicationMode.SelectedValue;
            string communicationType = dcCommunicationType.SelectedValue;
            string communicationInterval = dcCommunicationInterval.Text.Trim();
            string initialCommunicationTime = NormalizeTimeWithSeconds(dcInitialCommunicationTime.Text.Trim());

            string privateKey = Session["PrivateKey"] as string;
            if (string.IsNullOrEmpty(privateKey))
            {
                lblDeviceCreationMessage.Text = "Please authorize first to obtain a private key.";
                lblDeviceCreationMessage.CssClass = "text-danger";
                return;
            }

            dcPrivateKey.Text = privateKey;

            if (string.IsNullOrEmpty(transactionId) || string.IsNullOrEmpty(deviceIdentity) || string.IsNullOrEmpty(requestDateTime))
            {
                lblDeviceCreationMessage.Text = "Transaction ID, Device Identity JSON, and Request DateTime are required.";
                lblDeviceCreationMessage.CssClass = "text-danger";
                return;
            }

            string postData = $"device_identity={HttpUtility.UrlEncode(deviceIdentity)}&request_datetime={HttpUtility.UrlEncode(requestDateTime)}&device_type={HttpUtility.UrlEncode(deviceType)}&mdi_reset_date={HttpUtility.UrlEncode(mdiResetDate)}&mdi_reset_time={HttpUtility.UrlEncode(mdiResetTime)}&sim_number={HttpUtility.UrlEncode(simNumber)}&sim_id={HttpUtility.UrlEncode(simId)}&phase={HttpUtility.UrlEncode(phase)}&meter_type={HttpUtility.UrlEncode(meterType)}&communication_mode={HttpUtility.UrlEncode(communicationMode)}&communication_type={HttpUtility.UrlEncode(communicationType)}&initial_communication_time={HttpUtility.UrlEncode(initialCommunicationTime)}&communication_interval={HttpUtility.UrlEncode(communicationInterval)}&bidirectional_device={HttpUtility.UrlEncode(bidirectionalDevice)}";

            try
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Starting device creation for transaction: {transactionId}");
                
                DeviceCreationResponse creationResponse = PostDeviceCreation(transactionId, privateKey, postData);

                System.Diagnostics.Debug.WriteLine($"[DEBUG] PostDeviceCreation returned: {(creationResponse == null ? "NULL" : $"status={creationResponse.status}, message={creationResponse.message}")}");

                if (creationResponse == null)
                {
                    lblDeviceCreationMessage.Text = "Failed to get a response from the device creation endpoint.";
                    lblDeviceCreationMessage.CssClass = "text-danger";
                    return;
                }

                if (creationResponse.status != "1")
                {
                    lblDeviceCreationMessage.Text = $"Device creation failed: {creationResponse.message}";
                    lblDeviceCreationMessage.CssClass = "text-danger";
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[DEBUG] Device creation succeeded, polling transaction status...");
                TransactionStatusResponse statusResponse = PollTransactionStatus(transactionId, privateKey);

                System.Diagnostics.Debug.WriteLine($"[DEBUG] PollTransactionStatus returned: {(statusResponse == null ? "NULL" : $"status={statusResponse.status}, datacount={statusResponse.data?.Count ?? 0}")}");

                if (statusResponse == null)
                {
                    lblDeviceCreationMessage.Text = "Device creation accepted, but transaction status could not be retrieved.";
                    lblDeviceCreationMessage.CssClass = "text-warning";
                    return;
                }

                int maxStatusLevel = 0;
                if (statusResponse.data != null)
                {
                    foreach (var item in statusResponse.data)
                    {
                        if (int.TryParse(item.status_level, out int level) && level > maxStatusLevel)
                        {
                            maxStatusLevel = level;
                        }
                    }
                }

                string stageText = GetStageDescription(maxStatusLevel);
                lblDeviceCreationMessage.Text = $"Device creation succeeded. Transaction status: {statusResponse.status}. Latest status level: {maxStatusLevel} ({stageText}).";
                lblDeviceCreationMessage.CssClass = "text-success";

                // After success
                Session["CurrentTransactionId"] = transactionId;
                Session["CurrentPrivateKey"] = privateKey;
                pnlTracker.Visible = true;
                lblTrackerTransactionId.Text = transactionId;

                timerTracker.Enabled = true;
                // Hide loader after work
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "disableDeviceCreationBtn", "disableDeviceCreationButton();", true);
                
                // Save initial tracker state to ViewState
            }
            catch (WebException ex)
            {
                lblDeviceCreationMessage.Text = "HTTP Error: " + ex.Message;
                lblDeviceCreationMessage.CssClass = "text-danger";
            }
            catch (Exception ex)
            {
                lblDeviceCreationMessage.Text = "Error: " + ex.Message;
                lblDeviceCreationMessage.CssClass = "text-danger";
            }
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
            }

            UpdateTrackerUI(maxLevel);

            // Save current state to ViewState

            if (maxLevel >= 5)
            {
                timerTracker.Enabled = false;
                lblStage.Text = "Completed";
                lblStage.CssClass = "badge bg-success px-3 py-2";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "resetDeviceCreationLoading", "resetDeviceCreationLoading();", true);

                // Hide tracker only when level 5 is reached
                //pnlTracker.Visible = false;

                // Clear ViewState when completed
                ViewState["TrackerLevel"] = null;
                ViewState["TrackerTransactionId"] = null;
            }
        }
        private void UpdateTrackerUI(int level)
        {
            ResetSteps();

            for (int i = 0; i <= 5; i++)
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
            lblStageDescription.Text = GetStageDescription(level);

            int percent = (level * 100) / 5;
            progressBar.Style["width"] = percent + "%";
        }
        private void ResetSteps()
        {
            for (int i = 0; i <= 5; i++)
            {
                var step = FindControl("step" + i) as Label;
                if (step != null)
                    step.CssClass = "tracker-step";
            }
        }
        private DeviceCreationResponse PostDeviceCreation(string transactionId, string privateKey, string postData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://116.58.46.245:4050/UIP/device_creation");
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
                    return serializer.Deserialize<DeviceCreationResponse>(responseContent);
                }
            }
            catch (WebException ex)
            {
                string body = ReadWebExceptionResponse(ex);
                throw new WebException($"Device creation request failed: {ex.Message}. Response body: {body}", ex);
            }
        }

        private TransactionStatusResponse GetTransactionStatus(string transactionId, string privateKey)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://116.58.46.245:4050/UIP/transaction_status");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Headers.Add("transactionid", transactionId);
            request.Headers.Add("privatekey", privateKey);
            request.ContentLength = 0;
            request.UserAgent = "UDILTester/1.0";

            try
            {
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
                throw new WebException($"Transaction status request failed: {ex.Message}. Response body: {body}", ex);
            }
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

        private TransactionStatusResponse PollTransactionStatus(string transactionId, string privateKey)
        {
            TransactionStatusResponse lastResponse = null;
            int maxAttempts = 10;
            int attempt = 0;

            while (attempt < maxAttempts)
            {
                lastResponse = GetTransactionStatus(transactionId, privateKey);
                if (lastResponse?.data != null)
                {
                    bool reachedFinal = lastResponse.data.All(item => int.TryParse(item.status_level, out int statusLevel) && statusLevel >= 5);
                    if (reachedFinal)
                    {
                        break;
                    }
                }

                attempt++;
                Thread.Sleep(2000);
            }

            return lastResponse;
        }

        private string GetStageDescription(int statusLevel)
        {
            switch (statusLevel)
            {
                case 0: return "Waiting for processing";
                case 1: return "Commencing command processing";
                case 2: return "Communication request sent to meter";
                case 3: return "Communication established with meter";
                case 4: return "Command sent to meter";
                case 5: return "Command executed by meter";
                default: return "Unknown stage";
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

        public class DeviceCreationResponse
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

        public class AuthResponse
        {
            public string status { get; set; }
            public string privatekey { get; set; }
            public string message { get; set; }
        }

      
    }
}
