using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using UDIL.DAL;
using UDIL.Pages;
using UDIL.Shared;
using UDIL.Shared.Web;

namespace UDIL.Cancellations
{
    public partial class SANC : UDIL.AuthenticatedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            OperationPageSupport.ConfigureTimers(timerTracker, timerTables);

            if (Request["GetTransactionStatus"] == "true" && !string.IsNullOrEmpty(Request["transactionId"]))
            {
                string privateKey = SessionManager.PrivateKey;
                AppCommon.HandleTransactionStatusRequest(Request["transactionId"].ToString(), privateKey);
                return;
            }

            if (!IsPostBack)
            {
                ScriptManager.RegisterClientScriptInclude(this.Page, this.Page.GetType(), "udil-tester", ResolveUrl("~/udil-tester.js"));
                if (SessionManager.HasPrivateKey)
                {
                    scPrivateKey.Text = SessionManager.PrivateKey;
                }

                // Populate GlobalDeviceId and MSN from session if available
                if (SessionManager.HasGlobalDeviceId)
                {
                    scGlobalDeviceId.Text = SessionManager.GlobalDeviceId;
                }

                // Generate unique transaction ID
                scTransactionId.Text = AppCommon.GenerateTransactionId();
            }
        }



        protected void btnSANCCancellation_Click(object sender, EventArgs e)
        {
            lblSANCCancellationMessage.Text = string.Empty;
            // Store SANC Cancellation request time
            Session["SANCCancellationRequestTime"] = DateTime.Now;
            // Generate new transaction ID for this request
            string transactionId = scTransactionId.Text.Trim();
            string globalDeviceId = scGlobalDeviceId.Text.Trim();

            // Store GlobalDeviceId in session (overwrites existing value)
            if (!string.IsNullOrEmpty(globalDeviceId))
            {
                SessionManager.GlobalDeviceId = globalDeviceId;
            }


            string privateKey = SessionManager.PrivateKey;

            if (string.IsNullOrEmpty(privateKey))
            {
                lblSANCCancellationMessage.Text = "Please authorize first to obtain a private key.";
                lblSANCCancellationMessage.CssClass = "text-danger";
                return;
            }

            scPrivateKey.Text = privateKey;

            if (string.IsNullOrEmpty(transactionId) || string.IsNullOrEmpty(globalDeviceId))
            {
                lblSANCCancellationMessage.Text = "Transaction ID and Global Device ID are required.";
                lblSANCCancellationMessage.CssClass = "text-danger";
                return;
            }


            TransactionLogger.SavePendingTransaction(transactionId, globalDeviceId);

            string postData = $"global_device_id={HttpUtility.UrlEncode(globalDeviceId)}&type=sanc";

            try
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Starting SANC Cancellation for transaction: {transactionId}");

                SANCCancellationResponse sancResponse = PostSANCCancellation(transactionId, privateKey, postData);

                System.Diagnostics.Debug.WriteLine($"[DEBUG] PostSANCCancellation returned: {(sancResponse == null ? "NULL" : $"status={sancResponse.status}, message={sancResponse.message}")}");

                if (sancResponse == null)
                {
                    lblSANCCancellationMessage.Text = "Failed to get a response from the SANC cancellation endpoint.";
                    lblSANCCancellationMessage.CssClass = "text-danger";
                    return;
                }

                if (sancResponse.status != "1")
                {
                    lblSANCCancellationMessage.Text = $"SANC cancellation failed: {sancResponse.message}";
                    lblSANCCancellationMessage.CssClass = "text-danger";
                    return;
                }

                TransactionStatusResponse statusResponse = AppCommon.PollTransactionStatus(transactionId, privateKey);

                if (statusResponse == null)
                {
                    lblSANCCancellationMessage.Text = "SANC cancellation accepted, but transaction status could not be retrieved.";
                    lblSANCCancellationMessage.CssClass = "text-warning";
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
                    lblSANCCancellationMessage.Text = $"SANC cancellation failed. Command was cancelled by the meter (indv_status: 2, status_level: 4). Status: {stageText}.";
                    lblSANCCancellationMessage.CssClass = "text-danger";

                    // Show cancelled tracker briefly
                    pnlTracker.Visible = true;
                    lblTrackerTransactionId.Text = transactionId;
                    UpdateTrackerUI(-1); // Show cancelled stage
                    lblStageDescription.Text = "Command was cancelled by the meter (indv_status: 2, status_level: 4).";
                    lblStageDescription.CssClass = "text-danger";

                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "disableSANCCancellationBtn", "disableSANCCancellationButton();", true);
                }
                else
                {
                    string stageText = AppCommon.GetStageDescription(maxStatusLevel);
                    lblSANCCancellationMessage.Text = $"SANC cancellation succeeded. Transaction status: {statusResponse.status}. Latest status level: {maxStatusLevel} ({stageText}).";
                    lblSANCCancellationMessage.CssClass = "text-success";

                    // After success
                    Session[UdilConstants.SessionCurrentTransactionId] = transactionId;
                    Session[UdilConstants.SessionCurrentPrivateKey] = privateKey;

                    pnlTracker.Visible = true;
                    lblTrackerTransactionId.Text = transactionId;

                    timerTracker.Enabled = true;
                    // Hide loader after work
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "disableSANCCancellationBtn", "disableSANCCancellationButton();", true);
                }
            }
            catch (WebException ex)
            {
                lblSANCCancellationMessage.Text = "HTTP Error: " + ex.Message;
                lblSANCCancellationMessage.CssClass = "text-danger";
            }
            catch (Exception ex)
            {
                lblSANCCancellationMessage.Text = "Error: " + ex.Message;
                lblSANCCancellationMessage.CssClass = "text-danger";
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
            OperationPageSupport.RunTrackerTick(BuildTrackerContext());
        }

        private TrackerPageContext BuildTrackerContext()
        {
            return new TrackerPageContext
            {
                Session = Session,
                TimerTracker = timerTracker,
                TimerTables = timerTables,
                PnlTracker = pnlTracker,
                PnlDataTables = pnlDataTables,
                LblStage = lblStage,
                LblStageDescription = lblStageDescription,
                ProgressBar = progressBar,
                GvMeterVisuals = gvMeterVisuals,
                GvCommunicationHistory = gvCommunicationHistory,
                GvEvents = gvEvents,
                GetConnectionString = () => UDIL.Shared.ConfigurationManager.GetConnectionString(),
                GetGlobalDeviceId = () => scGlobalDeviceId.Text.Trim(),
                SaveApiTestResult = SaveApiTestResult
            };
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

        private SANCCancellationResponse PostSANCCancellation(string transactionId, string privateKey, string postData)
        {
            SessionHelper.Unlock();
            string baseUrl = GetBaseUrl();
            string url = $"{baseUrl}/parameterization_cancellation";
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
                    return serializer.Deserialize<SANCCancellationResponse>(responseContent);
                }
            }
            catch (WebException ex)
            {
                string body = AppCommon.ReadWebExceptionResponse(ex);
                throw new WebException($"SANC cancellation request failed: {ex.Message}. Response body: {body}", ex);
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
                        step.CssClass = "tracker-step";
                }

                lblStage.Text = "Cancelled";
                lblStage.CssClass = "badge bg-dark-danger px-3 py-2";
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

            int percent = (level * 100) / 5;
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

        public class SANCCancellationResponse
        {
            public string status { get; set; }
            public string transactionid { get; set; }
            public string message { get; set; }
            public object data { get; set; }
        }

                        private void ShowDataTables(bool forceRefresh = false)
        {
            try
            {
                var ctx = BuildTrackerContext();
                if (string.IsNullOrWhiteSpace(ctx.GetGlobalDeviceId()))
                {
                    return;
                }
                OperationPageSupport.RunTablesRefresh(ctx, forceRefresh);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing data tables: {ex.Message}");
            }
        }

protected void timerTables_Tick(object sender, EventArgs e)
        {
            OperationPageSupport.RunTablesTick(BuildTrackerContext());
        }

        protected void TableButton_Command(object sender, CommandEventArgs e)
        {
            string tableName = e.CommandArgument.ToString();
            string action = e.CommandName;

            if (action == "Fail")
            {
                // Pause table refresh timer when fail button is clicked
                timerTables.Enabled = false;

                // Show remarks section for fail reason
                ShowFailRemarksSection(tableName);
            }
            else if (action == "Pass")
            {
                // Handle pass action
                HandleTableAction(tableName, "Pass", null);

                // Make table card green and hide fail button
                MakeTableCardGreen(tableName);
            }
        }

        private void MakeTableCardGreen(string tableName)
        {
            try
            {
                switch (tableName)
                {
                    case "MeterVisuals":
                        btnMeterVisualsPass.Enabled = false;
                        btnMeterVisualsFail.Enabled = false;
                        break;

                    case "CommunicationHistory":
                        btnCommunicationHistoryPass.Enabled = false;
                        btnCommunicationHistoryFail.Enabled = false;
                        break;

                    case "Events":
                        btnEventsPass.Enabled = false;
                        btnEventsFail.Enabled = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error making table card green: {ex.Message}");
            }
        }

        private void MakeTableCardFailed(string tableName)
        {
            try
            {
                switch (tableName)
                {
                    case "MeterVisuals":
                        btnMeterVisualsFail.Enabled = false;
                        break;
                    case "CommunicationHistory":
                        btnCommunicationHistoryFail.Enabled = false;
                        break;
                    case "Events":
                        btnEventsFail.Enabled = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error making table card failed: {ex.Message}");
            }
        }

        private void ShowFailRemarksSection(string tableName)
        {
            try
            {
                switch (tableName)
                {
                    case "MeterVisuals":
                        meterVisualsRemarks.Style["display"] = "block";
                        break;

                    case "CommunicationHistory":
                        commHistoryRemarks.Style["display"] = "block";
                        break;

                    case "Events":
                        eventsRemarks.Style["display"] = "block";
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing fail remarks section: {ex.Message}");
            }
        }

        private void ShowRemarksSection(string tableName, string reason)
        {
            try
            {
                switch (tableName)
                {
                    case "MeterVisuals":
                        txtMeterVisualsRemarks.Text = reason;
                        meterVisualsRemarks.Style["display"] = "block";
                        break;

                    case "CommunicationHistory":
                        txtCommHistoryRemarks.Text = reason;
                        commHistoryRemarks.Style["display"] = "block";
                        break;

                    case "Events":
                        txtEventsRemarks.Text = reason;
                        eventsRemarks.Style["display"] = "block";
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing remarks section: {ex.Message}");
            }
        }

        protected void SaveRemarks_Command(object sender, CommandEventArgs e)
        {
            string tableName = e.CommandArgument.ToString();
            string remarks = "";

            try
            {
                switch (tableName)
                {
                    case "MeterVisuals":
                        remarks = txtMeterVisualsRemarks.Text;
                        break;
                    case "CommunicationHistory":
                        remarks = txtCommHistoryRemarks.Text;
                        break;
                    case "Events":
                        remarks = txtEventsRemarks.Text;
                        break;
                }

                if (!string.IsNullOrEmpty(remarks))
                {
                    SaveTestResult(tableName, "Fail", remarks, scTransactionId.Text.Trim(), scGlobalDeviceId.Text.Trim());
                    MakeTableCardFailed(tableName);
                    HideRemarksSection(tableName);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving remarks: {ex.Message}");
            }
        }

        private void HideRemarksSection(string tableName)
        {
            try
            {
                switch (tableName)
                {
                    case "MeterVisuals":
                        meterVisualsRemarks.Style["display"] = "none";
                        txtMeterVisualsRemarks.Text = "";
                        break;
                    case "CommunicationHistory":
                        commHistoryRemarks.Style["display"] = "none";
                        txtCommHistoryRemarks.Text = "";
                        break;
                    case "Events":
                        eventsRemarks.Style["display"] = "none";
                        txtEventsRemarks.Text = "";
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error hiding remarks section: {ex.Message}");
            }
        }

        protected void CancelRemarks_Command(object sender, CommandEventArgs e)
        {
            string tableName = e.CommandArgument.ToString();

            try
            {
                switch (tableName)
                {
                    case "MeterVisuals":
                        meterVisualsRemarks.Style["display"] = "none";
                        txtMeterVisualsRemarks.Text = "";
                        break;
                    case "CommunicationHistory":
                        commHistoryRemarks.Style["display"] = "none";
                        txtCommHistoryRemarks.Text = "";
                        break;
                    case "Events":
                        eventsRemarks.Style["display"] = "none";
                        txtEventsRemarks.Text = "";
                        break;
                }

                // Resume timer if all remarks are hidden
                if (meterVisualsRemarks.Style["display"] != "block" &&
                    commHistoryRemarks.Style["display"] != "block" &&
                    eventsRemarks.Style["display"] != "block")
                {
                    timerTables.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cancelling remarks: {ex.Message}");
            }
        }

        private void HandleTableAction(string tableName, string action, string reason)
        {
            SaveTestResult(tableName, action, reason ?? string.Empty, scTransactionId.Text.Trim(), scGlobalDeviceId.Text.Trim());
        }

        private void SaveApiTestResult(string transactionId, string status, string remarks)
        {
            try
            {
                var currentSession = UDIL.Shared.ConfigurationManager.GetCurrentSession();
                if (currentSession == null)
                {
                    System.Diagnostics.Debug.WriteLine("No current session found, skipping API test result save");
                    return;
                }

                string globalDeviceId = scGlobalDeviceId.Text.Trim();
                UDIL.DAL.TestResult testResult = new UDIL.DAL.TestResult
                {
                    SessionId = currentSession.SessionId,
                    TestName = "SANCAPI",
                    TestType = "API",
                    Status = status,
                    Remarks = remarks,
                    TestDate = DateTime.Now,
                    TransactionId = transactionId,
                    GlobalDeviceId = globalDeviceId
                };

                UDIL.DAL.DatabaseLayer dbLayer = new UDIL.DAL.DatabaseLayer(System.Configuration.ConfigurationManager.ConnectionStrings["TestSuitConnenction"]?.ConnectionString);
                dbLayer.SaveTestResult(testResult);

                System.Diagnostics.Debug.WriteLine($"API test result saved: {status}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving API test result: {ex.Message}");
            }
        }

        private void SaveTestResult(string testName, string status, string remarks, string transactionId, string globalDeviceId)
        {
            try
            {
                var currentSession = UDIL.Shared.ConfigurationManager.GetCurrentSession();
                if (currentSession == null)
                {
                    System.Diagnostics.Debug.WriteLine("No current session found, skipping test result save");
                    return;
                }

                UDIL.DAL.TestResult testResult = new UDIL.DAL.TestResult
                {
                    SessionId = currentSession.SessionId,
                    TestName = testName,
                    TestType = "Validation",
                    Status = status,
                    Remarks = remarks,
                    TestDate = DateTime.Now,
                    TransactionId = transactionId,
                    GlobalDeviceId = globalDeviceId
                };

                UDIL.DAL.DatabaseLayer dbLayer = new UDIL.DAL.DatabaseLayer(System.Configuration.ConfigurationManager.ConnectionStrings["TestSuitConnenction"]?.ConnectionString);
                dbLayer.SaveTestResult(testResult);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving test result: {ex.Message}");
            }
        }

        private System.Web.UI.Control FindParentControl(System.Web.UI.Control control, string cssClass)
        {
            System.Web.UI.Control parent = control.Parent;
            while (parent != null)
            {
                if (parent is System.Web.UI.WebControls.WebControl webControl &&
                    webControl.CssClass != null &&
                    webControl.CssClass.Contains(cssClass))
                {
                    return parent;
                }
                parent = parent.Parent;
            }
            return null;
        }
    }
}
