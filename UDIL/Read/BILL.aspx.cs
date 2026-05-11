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
    public partial class BILL : UDIL.AuthenticatedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Populate private key from session
                if (SessionManager.HasPrivateKey)
                {
                    billPrivateKey.Text = SessionManager.PrivateKey;
                }

                // Populate global device ID from session if available
                if (SessionManager.HasGlobalDeviceId)
                {
                    billGlobalDeviceId.Text = SessionManager.GlobalDeviceId;
                }

                // Generate unique transaction ID
                billTransactionId.Text = AppCommon.GenerateTransactionId();
            }
        }

        protected void btnBillRead_Click(object sender, EventArgs e)
        {
            lblBillMessage.Text = string.Empty;
            pnlResponse.Visible = false;

            string transactionId = billTransactionId.Text.Trim();
            string globalDeviceId = billGlobalDeviceId.Text.Trim();
            string type = billType.SelectedValue;
            string startDateTime = billStartDateTime.Text.Trim();
            string endDateTime = billEndDateTime.Text.Trim();

            string privateKey = SessionManager.PrivateKey;

            if (string.IsNullOrEmpty(privateKey))
            {
                lblBillMessage.Text = "Please authorize first to obtain a private key.";
                lblBillMessage.CssClass = "text-danger";
                return;
            }

            billPrivateKey.Text = privateKey;

            if (string.IsNullOrEmpty(transactionId) || string.IsNullOrEmpty(globalDeviceId) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(startDateTime) || string.IsNullOrEmpty(endDateTime))
            {
                lblBillMessage.Text = "All fields are required: Transaction ID, Global Device ID, Type, Start DateTime, and End DateTime.";
                lblBillMessage.CssClass = "text-danger";
                return;
            }

            // Store global device ID in session for use across the app
            SessionManager.GlobalDeviceId = globalDeviceId;

            string postData = $"global_device_id={HttpUtility.UrlEncode(globalDeviceId)}&type={HttpUtility.UrlEncode(type)}&start_datetime={HttpUtility.UrlEncode(startDateTime)}&end_datetime={HttpUtility.UrlEncode(endDateTime)}";

            try
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Starting BILL read for transaction: {transactionId}");

                BillReadResponse response = GetBillData(transactionId, privateKey, postData);

                System.Diagnostics.Debug.WriteLine($"[DEBUG] GetBillData returned: {(response == null ? "NULL" : $"status={response.status}, message={response.message}")}");

                if (response == null)
                {
                    lblBillMessage.Text = "Failed to get a response from the BILL read endpoint.";
                    lblBillMessage.CssClass = "text-danger";
                    return;
                }

                if (response.status != "1")
                {
                    lblBillMessage.Text = $"BILL read failed: {response.message}";
                    lblBillMessage.CssClass = "text-danger";

                    lblResponseStatus.Text = "Failed";
                    lblResponseStatus.CssClass = "badge bg-danger px-3 py-2";
                    lblResponseTransactionId.Text = response.transactionid;
                    lblResponseMessage.Text = response.message;
                    pnlResponse.Visible = true;
                    return;
                }

                // Success
                lblBillMessage.Text = $"BILL read succeeded. {response.message}";
                lblBillMessage.CssClass = "text-success";

                lblResponseStatus.Text = "Success";
                lblResponseStatus.CssClass = "badge bg-success px-3 py-2";
                lblResponseTransactionId.Text = response.transactionid;
                lblResponseMessage.Text = response.message;

                // Display data if available
                if (response.data != null && response.data.Count > 0)
                {
                    // Bind data to GridView
                    gvBillData.DataSource = response.data;
                    gvBillData.DataBind();

                    // Store global device ID and MSN in session for use across the app
                    if (!string.IsNullOrEmpty(response.data[0].global_device_id))
                    {
                        SessionManager.GlobalDeviceId = response.data[0].global_device_id;
                    }

                    if (!string.IsNullOrEmpty(response.data[0].msn))
                    {
                        SessionManager.MSN = response.data[0].msn;
                    }
                }
                else
                {
                    gvBillData.DataSource = null;
                    gvBillData.DataBind();
                }

                pnlResponse.Visible = true;
                pnlBillData.Visible = true;

                // After success, show data tables
                SessionManager.CurrentTransactionId = transactionId;
                ShowDataTables();
                pnlDataTables.Visible = true;
                timerTables.Enabled = true;

                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "disableBillBtn", "disableBillButton();", true);
            }
            catch (WebException ex)
            {
                lblBillMessage.Text = "HTTP Error: " + ex.Message;
                lblBillMessage.CssClass = "text-danger";
            }
            catch (Exception ex)
            {
                lblBillMessage.Text = "Error: " + ex.Message;
                lblBillMessage.CssClass = "text-danger";
            }
        }

        private BillReadResponse GetBillData(string transactionId, string privateKey, string postData)
        {
            string baseUrl = GetBaseUrl();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/on_demand_data_read");
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
                    return serializer.Deserialize<BillReadResponse>(responseContent);
                }
            }
            catch (WebException ex)
            {
                string body = AppCommon.ReadWebExceptionResponse(ex);
                throw new WebException($"BILL read request failed: {ex.Message}. Response body: {body}", ex);
            }
        }

        private string GetBaseUrl()
        {
            return UDIL.Shared.ConfigurationManager.GetBaseUrl();
        }

        public class BillReadResponse
        {
            public string status { get; set; }
            public string transactionid { get; set; }
            public List<BillData> data { get; set; }
            public string message { get; set; }
        }

        public class BillData
        {
            public string meter_datetime { get; set; }
            public string global_device_id { get; set; }
            public string msn { get; set; }
            public string active_energy_abs_t1 { get; set; }
            public string active_energy_abs_t2 { get; set; }
            public string active_energy_abs_t3 { get; set; }
            public string active_energy_abs_t4 { get; set; }
            public string active_energy_abs_tl { get; set; }
            public string active_energy_pos_t1 { get; set; }
            public string active_energy_pos_t2 { get; set; }
            public string active_energy_pos_t3 { get; set; }
            public string active_energy_pos_t4 { get; set; }
            public string active_energy_pos_tl { get; set; }
            public string active_energy_neg_t1 { get; set; }
            public string active_energy_neg_t2 { get; set; }
            public string active_energy_neg_t3 { get; set; }
            public string active_energy_neg_t4 { get; set; }
            public string active_energy_neg_tl { get; set; }
            public string reactive_energy_abs_t1 { get; set; }
            public string reactive_energy_abs_t2 { get; set; }
            public string reactive_energy_abs_t3 { get; set; }
            public string reactive_energy_abs_t4 { get; set; }
            public string reactive_energy_abs_tl { get; set; }
            public string reactive_energy_pos_t1 { get; set; }
            public string reactive_energy_pos_t2 { get; set; }
            public string reactive_energy_pos_t3 { get; set; }
            public string reactive_energy_pos_t4 { get; set; }
            public string reactive_energy_pos_tl { get; set; }
            public string reactive_energy_neg_t1 { get; set; }
            public string reactive_energy_neg_t2 { get; set; }
            public string reactive_energy_neg_t3 { get; set; }
            public string reactive_energy_neg_t4 { get; set; }
            public string reactive_energy_neg_tl { get; set; }
            public string active_mdi_pos_t1 { get; set; }
            public string active_mdi_pos_t2 { get; set; }
            public string active_mdi_pos_t3 { get; set; }
            public string active_mdi_pos_t4 { get; set; }
            public string active_mdi_pos_tl { get; set; }
            public string active_mdi_neg_t1 { get; set; }
            public string active_mdi_neg_t2 { get; set; }
            public string active_mdi_neg_t3 { get; set; }
            public string active_mdi_neg_t4 { get; set; }
            public string active_mdi_neg_tl { get; set; }
            public string active_mdi_abs_t1 { get; set; }
            public string active_mdi_abs_t2 { get; set; }
            public string active_mdi_abs_t3 { get; set; }
            public string active_mdi_abs_t4 { get; set; }
            public string active_mdi_abs_tl { get; set; }
            public string cumulative_mdi_pos_t1 { get; set; }
            public string cumulative_mdi_pos_t2 { get; set; }
            public string cumulative_mdi_pos_t3 { get; set; }
            public string cumulative_mdi_pos_t4 { get; set; }
            public string cumulative_mdi_pos_tl { get; set; }
            public string cumulative_mdi_neg_t1 { get; set; }
            public string cumulative_mdi_neg_t2 { get; set; }
            public string cumulative_mdi_neg_t3 { get; set; }
            public string cumulative_mdi_neg_t4 { get; set; }
            public string cumulative_mdi_neg_tl { get; set; }
            public string cumulative_mdi_abs_t1 { get; set; }
            public string cumulative_mdi_abs_t2 { get; set; }
            public string cumulative_mdi_abs_t3 { get; set; }
            public string cumulative_mdi_abs_t4 { get; set; }
            public string cumulative_mdi_abs_tl { get; set; }
            public string mdc_read_datetime { get; set; }
            public string db_datetime { get; set; }
            public string is_synced { get; set; }
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

                // Don't rebind LPRO data table here - it should only be bound once
                // when the API call is successful to prevent scroller position reset
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
                        $"alert('{tableName} marked as Fail with remarks: {remarks.Replace("'", "\\'")}');", true);

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

        protected void btnBillResponsePass_Click(object sender, EventArgs e)
        {
            try
            {
                string transactionId = SessionManager.CurrentTransactionId;
                SaveApiTestResult(transactionId, "Pass", "BILL Read Response marked as Pass");

                btnBillResponsePass.CssClass = "btn btn-success btn-sm disabled";
                btnBillResponsePass.Enabled = false;
                btnBillResponseFail.Visible = false;

                lblBillMessage.Text = "BILL Response marked as Pass.";
                lblBillMessage.CssClass = "text-success";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error marking response as pass: {ex.Message}");
            }
        }

        protected void btnBillResponseFail_Click(object sender, EventArgs e)
        {
            try
            {
                ((System.Web.UI.HtmlControls.HtmlControl)billResponseRemarks).Style.Add("display", "block");
                txtBillResponseRemarks.Text = "";
                txtBillResponseRemarks.Focus();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing response remarks: {ex.Message}");
            }
        }

        protected void btnSaveBillResponseRemarks_Click(object sender, EventArgs e)
        {
            try
            {
                string remarks = txtBillResponseRemarks.Text.Trim();
                if (!string.IsNullOrEmpty(remarks))
                {
                    string transactionId = SessionManager.CurrentTransactionId;
                    string globalDeviceId = SessionManager.GlobalDeviceId;

                    SaveTestResult("BILLReadResponse", "Fail", remarks, transactionId, globalDeviceId);

                    ((System.Web.UI.HtmlControls.HtmlControl)billResponseRemarks).Style.Add("display", "none");

                    btnBillResponsePass.Visible = false;
                    btnBillResponseFail.CssClass = "btn btn-danger btn-sm disabled";
                    btnBillResponseFail.Enabled = false;

                    lblBillMessage.Text = $"BILL Response marked as Fail. Reason: {remarks}";
                    lblBillMessage.CssClass = "text-danger";

                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "showResponseFailAlert",
                        $"alert('BILL Response marked as Fail with remarks: {remarks.Replace("'", "\\'")}');", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving response remarks: {ex.Message}");
            }
        }

        protected void btnCancelBillResponseRemarks_Click(object sender, EventArgs e)
        {
            try
            {
                ((System.Web.UI.HtmlControls.HtmlControl)billResponseRemarks).Style.Add("display", "none");
                txtBillResponseRemarks.Text = "";
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
                    ? $"{tableName} marked as Pass"
                    : $"{tableName} marked as Fail. Reason: {reason}";

                System.Diagnostics.Debug.WriteLine($"Table Action: {message}");
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
                    TestName = "BILLReadAPI",
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
                    TestType = "Database",
                    Status = status,
                    Remarks = remarks,
                    TestDate = DateTime.Now,
                    TransactionId = transactionId,
                    GlobalDeviceId = globalDeviceId
                };

                UDIL.DAL.DatabaseLayer dbLayer = new UDIL.DAL.DatabaseLayer(System.Configuration.ConfigurationManager.ConnectionStrings["TestSuitConnenction"]?.ConnectionString);
                dbLayer.SaveTestResult(testResult);

                System.Diagnostics.Debug.WriteLine($"Test result saved for {testName}: {status}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving test result: {ex.Message}");
            }
        }

    }
}