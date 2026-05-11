using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using UDIL.DAL;
using UDIL.Shared;

namespace UDIL.Read
{
    public partial class WSIM : UDIL.AuthenticatedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Populate private key from session
                if (SessionManager.HasPrivateKey)
                {
                    wsimPrivateKey.Text = SessionManager.PrivateKey;
                }

                // Populate global device ID from session if available
                if (SessionManager.HasGlobalDeviceId)
                {
                    wsimGlobalDeviceId.Text = SessionManager.GlobalDeviceId;
                }

                // Generate unique transaction ID
                wsimTransactionId.Text = AppCommon.GenerateTransactionId();
            }
        }

        protected void btnWsimRead_Click(object sender, EventArgs e)
        {
            lblWsimMessage.Text = string.Empty;
            pnlResponse.Visible = false;

            string transactionId = wsimTransactionId.Text.Trim();
            string globalDeviceId = wsimGlobalDeviceId.Text.Trim();
            string type = wsimType.SelectedValue;

            string privateKey = SessionManager.PrivateKey;

            if (string.IsNullOrEmpty(privateKey))
            {
                lblWsimMessage.Text = "Please authorize first to obtain a private key.";
                lblWsimMessage.CssClass = "text-danger";
                return;
            }

            wsimPrivateKey.Text = privateKey;

            if (string.IsNullOrEmpty(transactionId) || string.IsNullOrEmpty(globalDeviceId) || string.IsNullOrEmpty(type))
            {
                lblWsimMessage.Text = "All fields are required: Transaction ID, Global Device ID, and Type.";
                lblWsimMessage.CssClass = "text-danger";
                return;
            }

            // Store global device ID in session for use across the app
            SessionManager.GlobalDeviceId = globalDeviceId;

            string postData = $"global_device_id={HttpUtility.UrlEncode(globalDeviceId)}&type={HttpUtility.UrlEncode(type)}";

            try
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Starting WSIM read for transaction: {transactionId}");

                WsimReadResponse response = GetWsimData(transactionId, privateKey, postData);

                System.Diagnostics.Debug.WriteLine($"[DEBUG] GetWsimData returned: {(response == null ? "NULL" : $"status={response.status}, message={response.message}")}");

                if (response == null)
                {
                    lblWsimMessage.Text = "Failed to get a response from the WSIM read endpoint.";
                    lblWsimMessage.CssClass = "text-danger";
                    return;
                }

                if (response.status != "1")
                {
                    lblWsimMessage.Text = $"WSIM read failed: {response.message}";
                    lblWsimMessage.CssClass = "text-danger";

                    lblResponseStatus.Text = "Failed";
                    lblResponseStatus.CssClass = "badge bg-danger px-3 py-2";
                    lblResponseTransactionId.Text = response.transactionid;
                    lblResponseMessage.Text = response.message;
                    pnlResponse.Visible = true;
                    return;
                }

                // Success
                lblWsimMessage.Text = $"WSIM read succeeded. {response.message}";
                lblWsimMessage.CssClass = "text-success";

                lblResponseStatus.Text = "Success";
                lblResponseStatus.CssClass = "badge bg-success px-3 py-2";
                lblResponseTransactionId.Text = response.transactionid;
                lblResponseMessage.Text = response.message;

                // Display data if available
                if (response.data != null && response.data.Count > 0)
                {
                    WsimData data = response.data[0];
                    // Store global device ID and MSN in session for use across the app
                    if (!string.IsNullOrEmpty(data.global_device_id))
                    {
                        SessionManager.GlobalDeviceId = data.global_device_id;
                        lblRespGlobalDeviceId.Text = data.global_device_id;
                    }
                    else
                    {
                        lblRespGlobalDeviceId.Text = "N/A";
                    }

                    if (!string.IsNullOrEmpty(data.msn))
                    {
                        SessionManager.MSN = data.msn;
                        lblRespMsn.Text = data.msn;
                    }
                    else
                    {
                        lblRespMsn.Text = "N/A";
                    }

                    lblRespWsimDateTime.Text = !string.IsNullOrEmpty(data.wsim_datetime) ? data.wsim_datetime : "N/A";
                    lblRespWsimWakeupNumber1.Text = !string.IsNullOrEmpty(data.wsim_wakeup_number_1) ? data.wsim_wakeup_number_1 : "N/A";
                    lblRespWsimWakeupNumber2.Text = !string.IsNullOrEmpty(data.wsim_wakeup_number_2) ? data.wsim_wakeup_number_2 : "N/A";
                    lblRespWsimWakeupNumber3.Text = !string.IsNullOrEmpty(data.wsim_wakeup_number_3) ? data.wsim_wakeup_number_3 : "N/A";
                }
                else
                {
                    lblRespGlobalDeviceId.Text = "N/A";
                    lblRespMsn.Text = "N/A";
                    lblRespWsimDateTime.Text = "N/A";
                    lblRespWsimWakeupNumber1.Text = "N/A";
                    lblRespWsimWakeupNumber2.Text = "N/A";
                    lblRespWsimWakeupNumber3.Text = "N/A";
                }

                pnlResponse.Visible = true;

                // After success, show data tables
                SessionManager.CurrentTransactionId = transactionId;
                ShowDataTables();
                pnlDataTables.Visible = true;
                timerTables.Enabled = true;

                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "disableWsimBtn", "disableWsimButton();", true);
            }
            catch (WebException ex)
            {
                lblWsimMessage.Text = "HTTP Error: " + ex.Message;
                lblWsimMessage.CssClass = "text-danger";
            }
            catch (Exception ex)
            {
                lblWsimMessage.Text = "Error: " + ex.Message;
                lblWsimMessage.CssClass = "text-danger";
            }
        }

        private WsimReadResponse GetWsimData(string transactionId, string privateKey, string postData)
        {
            string baseUrl = GetBaseUrl();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/on_demand_parameter_read");
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
                    return serializer.Deserialize<WsimReadResponse>(responseContent);
                }
            }
            catch (WebException ex)
            {
                string body = AppCommon.ReadWebExceptionResponse(ex);
                throw new WebException($"WSIM read request failed: {ex.Message}. Response body: {body}", ex);
            }
        }

        private string GetBaseUrl()
        {
            return UDIL.Shared.ConfigurationManager.GetBaseUrl();
        }

        public class WsimReadResponse
        {
            public string status { get; set; }
            public string transactionid { get; set; }
            public List<WsimData> data { get; set; }
            public string message { get; set; }
        }

        public class WsimData
        {
            public string global_device_id { get; set; }
            public string msn { get; set; }
            public string wsim_datetime { get; set; }
            public string wsim_wakeup_number_1 { get; set; }
            public string wsim_wakeup_number_2 { get; set; }
            public string wsim_wakeup_number_3 { get; set; }
        }

        private string ReadWebExceptionResponse(WebException ex)
        {
            if (ex.Response == null)
                return string.Empty;

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

        private void ShowDataTables()
        {
            try
            {
                string globalDeviceId = SessionManager.GlobalDeviceId;
                if (!string.IsNullOrEmpty(globalDeviceId))
                {
                    LoadTableData(globalDeviceId);
                    pnlDataTables.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing data tables: {ex.Message}");
            }
        }

        private void LoadTableData(string globalDeviceId)
        {
            try
            {
                Tables dal = new Tables(UDIL.Shared.ConfigurationManager.GetConnectionString());
                DataSet ds = dal.GetUDILTables(globalDeviceId);

                if (ds.Tables.Contains("MeterVisuals") && ds.Tables["MeterVisuals"].Rows.Count > 0)
                {
                    gvMeterVisuals.DataSource = ds.Tables["MeterVisuals"];
                    gvMeterVisuals.DataBind();
                }
                else
                {
                    gvMeterVisuals.DataSource = null;
                    gvMeterVisuals.DataBind();
                }

                if (ds.Tables.Contains("CommunicationHistory") && ds.Tables["CommunicationHistory"].Rows.Count > 0)
                {
                    gvCommunicationHistory.DataSource = ds.Tables["CommunicationHistory"];
                    gvCommunicationHistory.DataBind();
                }
                else
                {
                    gvCommunicationHistory.DataSource = null;
                    gvCommunicationHistory.DataBind();
                }

                if (ds.Tables.Contains("Events") && ds.Tables["Events"].Rows.Count > 0)
                {
                    gvEvents.DataSource = ds.Tables["Events"];
                    gvEvents.DataBind();
                }
                else
                {
                    gvEvents.DataSource = null;
                    gvEvents.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading table data: {ex.Message}");
            }
        }

        protected void timerTables_Tick(object sender, EventArgs e)
        {
            ShowDataTables();
        }

        protected void TableButton_Command(object sender, CommandEventArgs e)
        {
            string tableName = e.CommandArgument.ToString();
            string action = e.CommandName;

            if (action == "Fail")
            {
                timerTables.Enabled = false;
                ShowFailRemarksSection(tableName);
            }
            else if (action == "Pass")
            {
                HandleTableAction(tableName, "Pass", null);
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
                        btnMeterVisualsFail.Visible = false;
                        btnMeterVisualsPass.CssClass = "btn btn-success btn-sm disabled";
                        btnMeterVisualsPass.Enabled = false;
                        break;
                    case "CommunicationHistory":
                        btnCommunicationHistoryFail.Visible = false;
                        btnCommunicationHistoryPass.CssClass = "btn btn-success btn-sm disabled";
                        btnCommunicationHistoryPass.Enabled = false;
                        break;
                    case "Events":
                        btnEventsFail.Visible = false;
                        btnEventsPass.CssClass = "btn btn-success btn-sm disabled";
                        btnEventsPass.Enabled = false;
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
                        btnMeterVisualsPass.Visible = false;
                        btnMeterVisualsFail.CssClass = "btn btn-danger btn-sm disabled";
                        btnMeterVisualsFail.Enabled = false;
                        break;
                    case "CommunicationHistory":
                        btnCommunicationHistoryPass.Visible = false;
                        btnCommunicationHistoryFail.CssClass = "btn btn-danger btn-sm disabled";
                        btnCommunicationHistoryFail.Enabled = false;
                        break;
                    case "Events":
                        btnEventsPass.Visible = false;
                        btnEventsFail.CssClass = "btn btn-danger btn-sm disabled";
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
                        ((System.Web.UI.HtmlControls.HtmlControl)meterVisualsRemarks).Style.Add("display", "block");
                        txtMeterVisualsRemarks.Text = "";
                        break;
                    case "CommunicationHistory":
                        ((System.Web.UI.HtmlControls.HtmlControl)commHistoryRemarks).Style.Add("display", "block");
                        txtCommHistoryRemarks.Text = "";
                        break;
                    case "Events":
                        ((System.Web.UI.HtmlControls.HtmlControl)eventsRemarks).Style.Add("display", "block");
                        txtEventsRemarks.Text = "";
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
                        ((System.Web.UI.HtmlControls.HtmlControl)meterVisualsRemarks).Style.Add("display", "block");
                        txtMeterVisualsRemarks.Text = reason;
                        break;
                    case "CommunicationHistory":
                        ((System.Web.UI.HtmlControls.HtmlControl)commHistoryRemarks).Style.Add("display", "block");
                        txtCommHistoryRemarks.Text = reason;
                        break;
                    case "Events":
                        ((System.Web.UI.HtmlControls.HtmlControl)eventsRemarks).Style.Add("display", "block");
                        txtEventsRemarks.Text = reason;
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
                        remarks = txtMeterVisualsRemarks.Text.Trim();
                        break;
                    case "CommunicationHistory":
                        remarks = txtCommHistoryRemarks.Text.Trim();
                        break;
                    case "Events":
                        remarks = txtEventsRemarks.Text.Trim();
                        break;
                }

                if (!string.IsNullOrEmpty(remarks))
                {
                    HandleTableAction(tableName + "Table - AUXR", "Fail", remarks);
                    System.Diagnostics.Debug.WriteLine($"Remarks saved for {tableName}: {remarks}");

                    HideRemarksSection(tableName);
                    MakeTableCardFailed(tableName);

                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "showFailAlert",
                        $"alert('{tableName} marked as Fail with remarks: " + remarks.Replace("'", "\\'") + "');", true);

                    timerTables.Enabled = true;
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
                        ((System.Web.UI.HtmlControls.HtmlControl)meterVisualsRemarks).Style.Add("display", "none");
                        txtMeterVisualsRemarks.Text = "";
                        break;
                    case "CommunicationHistory":
                        ((System.Web.UI.HtmlControls.HtmlControl)commHistoryRemarks).Style.Add("display", "none");
                        txtCommHistoryRemarks.Text = "";
                        break;
                    case "Events":
                        ((System.Web.UI.HtmlControls.HtmlControl)eventsRemarks).Style.Add("display", "none");
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
                        ((System.Web.UI.HtmlControls.HtmlControl)meterVisualsRemarks).Style.Add("display", "none");
                        txtMeterVisualsRemarks.Text = "";
                        break;
                    case "CommunicationHistory":
                        ((System.Web.UI.HtmlControls.HtmlControl)commHistoryRemarks).Style.Add("display", "none");
                        txtCommHistoryRemarks.Text = "";
                        break;
                    case "Events":
                        ((System.Web.UI.HtmlControls.HtmlControl)eventsRemarks).Style.Add("display", "none");
                        txtEventsRemarks.Text = "";
                        break;
                }

                timerTables.Enabled = true;

                System.Diagnostics.Debug.WriteLine($"Remarks section hidden for {tableName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error canceling remarks: {ex.Message}");
            }
        }

        protected void btnWsimResponsePass_Click(object sender, EventArgs e)
        {
            try
            {
                string transactionId = SessionManager.CurrentTransactionId;
                SaveApiTestResult(transactionId, "Pass", "WSIM Read Response marked as Pass");

                btnWsimResponsePass.CssClass = "btn btn-success btn-sm disabled";
                btnWsimResponsePass.Enabled = false;
                btnWsimResponseFail.Visible = false;

                lblWsimMessage.Text = "WSIM Response marked as Pass.";
                lblWsimMessage.CssClass = "text-success";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error marking response as pass: {ex.Message}");
            }
        }

        protected void btnWsimResponseFail_Click(object sender, EventArgs e)
        {
            try
            {
                ((System.Web.UI.HtmlControls.HtmlControl)wsimResponseRemarks).Style.Add("display", "block");
                txtWsimResponseRemarks.Text = "";
                txtWsimResponseRemarks.Focus();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing response remarks: {ex.Message}");
            }
        }

        protected void btnSaveWsimResponseRemarks_Click(object sender, EventArgs e)
        {
            try
            {
                string remarks = txtWsimResponseRemarks.Text.Trim();
                if (!string.IsNullOrEmpty(remarks))
                {
                    string transactionId = SessionManager.CurrentTransactionId;
                    string globalDeviceId = SessionManager.GlobalDeviceId;

                    SaveTestResult("WSIMReadResponse", "Fail", remarks, transactionId, globalDeviceId);

                    ((System.Web.UI.HtmlControls.HtmlControl)wsimResponseRemarks).Style.Add("display", "none");

                    btnWsimResponsePass.Visible = false;
                    btnWsimResponseFail.CssClass = "btn btn-danger btn-sm disabled";
                    btnWsimResponseFail.Enabled = false;

                    lblWsimMessage.Text = $"WSIM Response marked as Fail. Reason: {remarks}";
                    lblWsimMessage.CssClass = "text-danger";

                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "showResponseFailAlert",
                        $"alert('WSIM Response marked as Fail with remarks: " + remarks.Replace("'", "\\'") + "');", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving response remarks: {ex.Message}");
            }
        }

        protected void btnCancelWsimResponseRemarks_Click(object sender, EventArgs e)
        {
            try
            {
                ((System.Web.UI.HtmlControls.HtmlControl)wsimResponseRemarks).Style.Add("display", "none");
                txtWsimResponseRemarks.Text = "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error canceling response remarks: {ex.Message}");
            }
        }

        private void HandleTableAction(string tableName, string action, string reason)
        {
            try
            {
                string transactionId = SessionManager.CurrentTransactionId;
                string globalDeviceId = SessionManager.GlobalDeviceId;

                SaveTestResult(tableName, action, reason, transactionId, globalDeviceId);

                string message = action == "Pass"
                    ? tableName + " marked as Pass"
                    : tableName + " marked as Fail. Reason: " + reason;

                System.Diagnostics.Debug.WriteLine("Table Action: " + message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling table action: {ex.Message}");
            }
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

                string globalDeviceId = SessionManager.GlobalDeviceId;
                UDIL.DAL.TestResult testResult = new UDIL.DAL.TestResult
                {
                    SessionId = currentSession.SessionId,
                    TestName = "WSIMReadAPI",
                    TestType = "API",
                    Status = status,
                    Remarks = remarks,
                    TestDate = DateTime.Now,
                    TransactionId = transactionId,
                    GlobalDeviceId = globalDeviceId
                };

                UDIL.DAL.DatabaseLayer dbLayer = new UDIL.DAL.DatabaseLayer(System.Configuration.ConfigurationManager.ConnectionStrings["TestSuitConnenction"]?.ConnectionString);
                dbLayer.SaveTestResult(testResult);

                System.Diagnostics.Debug.WriteLine("API test result saved: " + status);
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
                    TestType = "Database",
                    Status = status,
                    Remarks = remarks,
                    TestDate = DateTime.Now,
                    TransactionId = transactionId,
                    GlobalDeviceId = globalDeviceId
                };

                UDIL.DAL.DatabaseLayer dbLayer = new UDIL.DAL.DatabaseLayer(System.Configuration.ConfigurationManager.ConnectionStrings["TestSuitConnenction"]?.ConnectionString);
                dbLayer.SaveTestResult(testResult);

                System.Diagnostics.Debug.WriteLine("Test result saved for " + testName + ": " + status);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving test result: {ex.Message}");
            }
        }

    }
}