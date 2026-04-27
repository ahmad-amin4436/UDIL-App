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
    public partial class WakeUpSimNumber : System.Web.UI.Page
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
                    wsPrivateKey.Text = Session["PrivateKey"].ToString();
                }

                // Generate unique transaction ID
                wsTransactionId.Text = AppCommon.GenerateTransactionId();
            }
        }

        protected void btnWakeSimNumber_Click(object sender, EventArgs e)
        {
            lblWakeSimNumberMessage.Text = string.Empty;
            // Store Wake SIM Number request time
            Session["WakeSimNumberRequestTime"] = DateTime.Now;
            // Generate new transaction ID for this request
            string transactionId = wsTransactionId.Text.Trim();
            string globalDeviceId = wsGlobalDeviceId.Text.Trim();
            string globalDeviceIdArray = $"[\"{globalDeviceId}\"]";
            string requestDateTime = wsRequestDateTime.Text.Trim();
            string wakeupNumber1 = wsWakeupNumber1.Text.Trim();
            string wakeupNumber2 = wsWakeupNumber2.Text.Trim();
            string wakeupNumber3 = wsWakeupNumber3.Text.Trim();

            string privateKey = Session["PrivateKey"] as string;

            if (string.IsNullOrEmpty(privateKey))
            {
                lblWakeSimNumberMessage.Text = "Please authorize first to obtain a private key.";
                lblWakeSimNumberMessage.CssClass = "text-danger";
                return;
            }

            wsPrivateKey.Text = privateKey;

            if (string.IsNullOrEmpty(transactionId) || string.IsNullOrEmpty(globalDeviceId) || string.IsNullOrEmpty(requestDateTime) ||
                string.IsNullOrEmpty(wakeupNumber1) || string.IsNullOrEmpty(wakeupNumber2) || string.IsNullOrEmpty(wakeupNumber3))
            {
                lblWakeSimNumberMessage.Text = "All fields are required: Transaction ID, Global Device ID, Request DateTime, Wake Up Number 1, Wake Up Number 2, and Wake Up Number 3.";
                lblWakeSimNumberMessage.CssClass = "text-danger";
                return;
            }

            string postData = $"global_device_id={HttpUtility.UrlEncode(globalDeviceIdArray)}&request_datetime={HttpUtility.UrlEncode(requestDateTime)}&wakeup_number_1={HttpUtility.UrlEncode(wakeupNumber1)}&wakeup_number_2={HttpUtility.UrlEncode(wakeupNumber2)}&wakeup_number_3={HttpUtility.UrlEncode(wakeupNumber3)}";

            try
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Starting Wake SIM Number for transaction: {transactionId}");

                WakeSimNumberResponse wakeSimResponse = PostWakeSimNumber(transactionId, privateKey, postData);

                System.Diagnostics.Debug.WriteLine($"[DEBUG] PostWakeSimNumber returned: {(wakeSimResponse == null ? "NULL" : $"status={wakeSimResponse.status}, message={wakeSimResponse.message}")}");

                if (wakeSimResponse == null)
                {
                    lblWakeSimNumberMessage.Text = "Failed to get a response from the wake up SIM number endpoint.";
                    lblWakeSimNumberMessage.CssClass = "text-danger";
                    return;
                }

                if (wakeSimResponse.status != "1")
                {
                    lblWakeSimNumberMessage.Text = $"Wake SIM Number failed: {wakeSimResponse.message}";
                    lblWakeSimNumberMessage.CssClass = "text-danger";
                    return;
                }

                TransactionStatusResponse statusResponse = AppCommon.GetTransactionStatus(transactionId, privateKey);

                if (statusResponse == null)
                {
                    lblWakeSimNumberMessage.Text = "Wake SIM Number accepted, but transaction status could not be retrieved.";
                    lblWakeSimNumberMessage.CssClass = "text-warning";
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
                    lblWakeSimNumberMessage.Text = $"Wake SIM Number failed. Command was cancelled by the meter (indv_status: 2, status_level: 4). Status: {stageText}.";
                    lblWakeSimNumberMessage.CssClass = "text-danger";

                    // Show cancelled tracker briefly
                    pnlTracker.Visible = true;
                    lblTrackerTransactionId.Text = transactionId;
                    UpdateTrackerUI(-1); // Show cancelled stage
                    lblStageDescription.Text = "Command was cancelled by the meter (indv_status: 2, status_level: 4).";
                    lblStageDescription.CssClass = "text-danger";

                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "disableWakeSimNumberBtn", "disableWakeSimNumberButton();", true);
                }
                else
                {
                    string stageText = AppCommon.GetStageDescription(maxStatusLevel);
                    lblWakeSimNumberMessage.Text = $"Wake SIM Number succeeded. Transaction status: {statusResponse.status}. Latest status level: {maxStatusLevel} ({stageText}).";
                    lblWakeSimNumberMessage.CssClass = "text-success";

                    // After success
                    Session["CurrentTransactionId"] = transactionId;
                    Session["CurrentPrivateKey"] = privateKey;

                    pnlTracker.Visible = true;
                    lblTrackerTransactionId.Text = transactionId;

                    timerTracker.Enabled = true;
                    // Hide loader after work
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "disableWakeSimNumberBtn", "disableWakeSimNumberButton();", true);
                }
            }
            catch (WebException ex)
            {
                lblWakeSimNumberMessage.Text = "HTTP Error: " + ex.Message;
                lblWakeSimNumberMessage.CssClass = "text-danger";
            }
            catch (Exception ex)
            {
                lblWakeSimNumberMessage.Text = "Error: " + ex.Message;
                lblWakeSimNumberMessage.CssClass = "text-danger";
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
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "resetWakeSimNumberLoading", "resetWakeSimNumberLoading();", true);
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
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "resetWakeSimNumberLoading", "resetWakeSimNumberLoading();", true);

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

        private WakeSimNumberResponse PostWakeSimNumber(string transactionId, string privateKey, string postData)
        {
            string baseUrl = GetBaseUrl();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/update_wake_up_sim_number");
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
                    return serializer.Deserialize<WakeSimNumberResponse>(responseContent);
                }
            }
            catch (WebException ex)
            {
                string body = AppCommon.ReadWebExceptionResponse(ex);
                throw new WebException($"Wake SIM Number request failed: {ex.Message}. Response body: {body}", ex);
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

        public class WakeSimNumberResponse
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
