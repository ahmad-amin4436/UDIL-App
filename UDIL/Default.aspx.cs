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
using UDIL.Shared;
using UDIL.DAL;
using System.Data;
using System.Text;

namespace UDIL
{
    public partial class _Default : UDIL.AuthenticatedPage
    {
        protected TextBox txtUsername;
        protected TextBox txtPassword;
        protected TextBox txtCode;
        protected Label lblAuthMessage;
        protected Button btnAuthorize;

        // Configuration controls
        protected TextBox txtBaseUrl;
        protected TextBox txtConfigName;
        protected TextBox txtTimeout;
        protected DropDownList ddlSavedConfigs;
        protected Button btnSaveConfig;
        protected Button btnLoadConfig;
        protected Button btnDeleteConfig;
        protected Button btnApplyConfig;
        protected Label lblConfigMessage;

        // Database configuration controls
        protected TextBox txtDbServer;
        protected TextBox txtDbPort;
        protected TextBox txtDbName;
        protected TextBox txtDbUid;
        protected TextBox txtDbPwd;
        protected TextBox txtDbProvider;

        // Session Management controls
        protected TextBox txtSessionName;
        protected TextBox txtSessionDescription;
        protected DropDownList ddlSavedSessions;
        protected TextBox txtSessionDate;
        protected TextBox txtTestEnvironment;
        protected TextBox txtDeviceCount;
        protected TextBox txtTestType;
        protected Button btnCreateSession;
        protected Button btnLoadSession;
        protected Button btnDeleteSession;
        protected Button btnExportSession;
        protected Label lblSessionId;
        protected Label lblSessionStatus;
        protected Label lblSessionCreated;
        protected Label lblSessionModified;
        protected Label lblTestsCompleted;
        protected Label lblTotalTests;
        protected Label lblSessionMessage;

        protected TextBox dcTransactionId;
        protected TextBox dcPrivateKey;
        protected TextBox dcDsn;
        protected TextBox dcGlobalDeviceId;
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
            

            // Initialize configuration dropdown if not postback
            if (!IsPostBack)
            {
                LoadConfigurations();
                LoadCurrentConfiguration();
                LoadSessions();
                LoadCurrentSession();
            }

            // Register export button for full postback (required for file downloads in UpdatePanel)
            ScriptManager.GetCurrent(this).RegisterPostBackControl(btnExportSession);

            if (!IsPostBack)
            {
                ScriptManager.RegisterClientScriptInclude(this.Page, this.Page.GetType(), "udil-tester", ResolveUrl("~/udil-tester.js"));
                if (Session["PrivateKey"] != null)
                {
                    //dcPrivateKey.Text = Session["PrivateKey"].ToString();
                }

                // Generate unique transaction ID
                AppCommon.GenerateTransactionId();
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

            string baseUrl = GetBaseUrl();
            string fullUrl = baseUrl + "/authorization_service";
            
            // Debug logging to help troubleshoot connection issues
            System.Diagnostics.Debug.WriteLine($"Attempting to connect to: {fullUrl}");
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("username", username);
            request.Headers.Add("password", password);
            request.Headers.Add("code", code);
            
            // Add timeout settings
            request.Timeout = 30000; // 30 seconds
            request.ReadWriteTimeout = 30000;
            
            // Set user agent
            request.UserAgent = "UDILTester/1.0";

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
                string responseBody = AppCommon.ReadWebExceptionResponse(ex);
                System.Diagnostics.Debug.WriteLine($"WebException: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Status: {ex.Status}");
                System.Diagnostics.Debug.WriteLine($"Response: {responseBody}");
                
                if (ex.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    lblAuthMessage.Text = "Cannot resolve server hostname. Check your internet connection and server address.";
                }
                else if (ex.Status == WebExceptionStatus.ConnectFailure)
                {
                    lblAuthMessage.Text = "Cannot connect to server. Server may be down or firewall blocking connection.";
                }
                else if (ex.Status == WebExceptionStatus.Timeout)
                {
                    lblAuthMessage.Text = "Connection timeout. Server is not responding within 30 seconds.";
                }
                else if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    lblAuthMessage.Text = "Protocol error. Server returned an error response.";
                }
                else
                {
                    lblAuthMessage.Text = "Network Error: " + ex.Message;
                }
                lblAuthMessage.CssClass = "text-danger";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General Exception: {ex.Message}");
                lblAuthMessage.Text = "Error: " + ex.Message;
                lblAuthMessage.CssClass = "text-danger";
            }
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

                        
        #region Configuration Management

        private void LoadConfigurations()
        {
            try
            {
                var dbLayer = new DatabaseLayer();
                var configDs = dbLayer.GetAllConfigurations();
                
                ddlSavedConfigs.Items.Clear();
                ddlSavedConfigs.Items.Add(new ListItem("-- Select Configuration --", ""));
                
                if (configDs.Tables.Count > 0)
                {
                    foreach (System.Data.DataRow row in configDs.Tables[0].Rows)
                    {
                        ddlSavedConfigs.Items.Add(new ListItem(row["name"].ToString(), row["name"].ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                lblConfigMessage.Text = "Error loading configurations: " + ex.Message;
                lblConfigMessage.CssClass = "text-danger";
            }
        }

        private void LoadCurrentConfiguration()
        {
            // Load current configuration from ConfigurationManager
            var config = UDIL.Shared.ConfigurationManager.Current;
            
            txtBaseUrl.Text = config.BaseUrl;
            txtTimeout.Text = config.Timeout.ToString();
            
            // Load database configuration
            txtDbServer.Text = config.DbServer;
            txtDbPort.Text = config.DbPort;
            txtDbName.Text = config.DbName;
            txtDbUid.Text = config.DbUid;
            txtDbPwd.Text = config.DbPwd;
            txtDbProvider.Text = config.DbProvider;
        }

        protected void btnSaveConfig_Click(object sender, EventArgs e)
        {
            try
            {
                string configName = txtConfigName.Text.Trim();
                if (string.IsNullOrEmpty(configName))
                {
                    lblConfigMessage.Text = "Please enter a configuration name.";
                    lblConfigMessage.CssClass = "text-danger";
                    return;
                }

                var config = new UDIL.Shared.Configuration
                {
                    BaseUrl = txtBaseUrl.Text.Trim(),
                    Timeout = Convert.ToInt32(txtTimeout.Text.Trim()),
                    
                    // Database configuration
                    DbServer = txtDbServer.Text.Trim(),
                    Name = configName,
                    DbPort = txtDbPort.Text.Trim(),
                    DbName = txtDbName.Text.Trim(),
                    DbUid = txtDbUid.Text.Trim(),
                    DbPwd = txtDbPwd.Text.Trim(),
                    DbProvider = txtDbProvider.Text.Trim()
                };

                var dbLayer = new DatabaseLayer();
                dbLayer.SaveConfiguration(config);

                lblConfigMessage.Text = "Configuration saved successfully!";
                lblConfigMessage.CssClass = "text-success";
                
                // Refresh dropdown
                LoadConfigurations();
                ddlSavedConfigs.SelectedValue = configName;
            }
            catch (Exception ex)
            {
                lblConfigMessage.Text = "Error saving configuration: " + ex.Message;
                lblConfigMessage.CssClass = "text-danger";
            }
        }

        protected void btnLoadConfig_Click(object sender, EventArgs e)
        {
            try
            {
                string configName = ddlSavedConfigs.SelectedValue;
                if (string.IsNullOrEmpty(configName))
                {
                    lblConfigMessage.Text = "Please select a configuration to load.";
                    lblConfigMessage.CssClass = "text-danger";
                    return;
                }

                var dbLayer = new DatabaseLayer();
                var configDs = dbLayer.GetConfiguration(configName);
                
                if (configDs.Tables.Count == 0 || configDs.Tables[0].Rows.Count == 0)
                {
                    throw new Exception($"Configuration '{configName}' not found");
                }
                
                System.Data.DataRow row = configDs.Tables[0].Rows[0];
                var config = new UDIL.Shared.Configuration
                {
                    Name = row["name"].ToString(),
                    BaseUrl = row["base_url"].ToString(),
                    Timeout = Convert.ToInt32(row["timeout"]),
                    DbServer = row["db_server"].ToString(),
                    DbPort = row["db_port"].ToString(),
                    DbName = row["db_name"].ToString(),
                    DbUid = row["db_uid"].ToString(),
                    DbPwd = row["db_pwd"].ToString(),
                    DbProvider = row["db_provider"].ToString(),
                    CreatedDate = Convert.ToDateTime(row["created_date"])
                };
                
                txtBaseUrl.Text = config.BaseUrl;
                txtTimeout.Text = config.Timeout.ToString();
                txtConfigName.Text = config.Name;

                // Load database configuration
                txtDbServer.Text = config.DbServer;
                txtDbPort.Text = config.DbPort;
                txtDbName.Text = config.DbName;
                txtDbUid.Text = config.DbUid;
                txtDbPwd.Text = config.DbPwd;
                txtDbProvider.Text = config.DbProvider;

                lblConfigMessage.Text = "Configuration loaded successfully!";
                lblConfigMessage.CssClass = "text-success";
            }
            catch (Exception ex)
            {
                lblConfigMessage.Text = "Error loading configuration: " + ex.Message;
                lblConfigMessage.CssClass = "text-danger";
            }
        }

        protected void btnDeleteConfig_Click(object sender, EventArgs e)
        {
            try
            {
                string configName = ddlSavedConfigs.SelectedValue;
                if (string.IsNullOrEmpty(configName))
                {
                    lblConfigMessage.Text = "Please select a configuration to delete.";
                    lblConfigMessage.CssClass = "text-danger";
                    return;
                }

                var dbLayer = new DatabaseLayer();
                dbLayer.DeleteConfiguration(configName);

                lblConfigMessage.Text = "Configuration deleted successfully!";
                lblConfigMessage.CssClass = "text-success";
                
                // Clear form and refresh dropdown
                txtConfigName.Text = "";
                LoadConfigurations();
            }
            catch (Exception ex)
            {
                lblConfigMessage.Text = "Error deleting configuration: " + ex.Message;
                lblConfigMessage.CssClass = "text-danger";
            }
        }

        protected void btnApplyConfig_Click(object sender, EventArgs e)
        {
            try
            {
                var config = new UDIL.Shared.Configuration
                {
                    BaseUrl = txtBaseUrl.Text.Trim(),
                    Timeout = Convert.ToInt32(txtTimeout.Text.Trim()),
                    
                    // Database configuration
                    DbServer = txtDbServer.Text.Trim(),
                    DbPort = txtDbPort.Text.Trim(),
                    DbName = txtDbName.Text.Trim(),
                    DbUid = txtDbUid.Text.Trim(),
                    DbPwd = txtDbPwd.Text.Trim(),
                    DbProvider = txtDbProvider.Text.Trim()
                };

                // Apply configuration using ConfigurationManager
                UDIL.Shared.ConfigurationManager.ApplyConfiguration(config);

                lblConfigMessage.Text = "Configuration applied successfully!";
                lblConfigMessage.CssClass = "text-success";
            }
            catch (Exception ex)
            {
                lblConfigMessage.Text = "Error applying configuration: " + ex.Message;
                lblConfigMessage.CssClass = "text-danger";
            }
        }

        protected void ddlSavedConfigs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlSavedConfigs.SelectedValue))
            {
                btnLoadConfig_Click(sender, e);
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

        #endregion

        #region Session Management

        private void LoadSessions()
        {
            try
            {
                var dbLayer = new DatabaseLayer();
                var sessionDs = dbLayer.GetAllSessions();
                
                ddlSavedSessions.Items.Clear();
                ddlSavedSessions.Items.Add(new ListItem("-- Select Session --", ""));
                
                if (sessionDs.Tables.Count > 0)
                {
                    foreach (System.Data.DataRow row in sessionDs.Tables[0].Rows)
                    {
                        ddlSavedSessions.Items.Add(new ListItem(row["name"].ToString(), row["name"].ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                lblSessionMessage.Text = "Error loading sessions: " + ex.Message;
                lblSessionMessage.CssClass = "text-danger";
            }
        }

        private void LoadCurrentSession()
        {
            try
            {
                var currentSession = UDIL.Shared.ConfigurationManager.GetCurrentSession();
                
                if (currentSession != null)
                {
                    lblSessionId.Text = currentSession.SessionId;
                    lblSessionStatus.Text = currentSession.Status;
                    lblSessionStatus.CssClass = GetSessionStatusBadgeClass(currentSession.Status);
                    lblSessionCreated.Text = currentSession.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
                    lblSessionModified.Text = currentSession.ModifiedDate.ToString("yyyy-MM-dd HH:mm:ss");
                    lblTestsCompleted.Text = currentSession.TestsCompleted.ToString();
                    lblTotalTests.Text = currentSession.TotalTests.ToString();
                    
                    txtSessionName.Text = currentSession.Name;
                    txtSessionDescription.Text = currentSession.Description;
                    txtSessionDate.Text = currentSession.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
                    txtTestEnvironment.Text = currentSession.TestEnvironment;
                    txtDeviceCount.Text = currentSession.DeviceCount.ToString();
                    txtTestType.Text = currentSession.TestType;
                }
                else
                {
                    ResetSessionDisplay();
                }
            }
            catch (Exception ex)
            {
                lblSessionMessage.Text = "Error loading current session: " + ex.Message;
                lblSessionMessage.CssClass = "text-danger";
            }
        }

        private void ResetSessionDisplay()
        {
            lblSessionId.Text = "N/A";
            lblSessionStatus.Text = "Not Started";
            lblSessionStatus.CssClass = "badge bg-secondary";
            lblSessionCreated.Text = "N/A";
            lblSessionModified.Text = "N/A";
            lblTestsCompleted.Text = "0";
            lblTotalTests.Text = "0";
        }

        private string GetSessionStatusBadgeClass(string status)
        {
            switch (status.ToLower())
            {
                case "active":
                case "in progress":
                    return "badge bg-warning";
                case "completed":
                    return "badge bg-success";
                case "failed":
                case "error":
                    return "badge bg-danger";
                case "paused":
                    return "badge bg-info";
                default:
                    return "badge bg-secondary";
            }
        }

        protected void btnCreateSession_Click(object sender, EventArgs e)
        {
            try
            {
                string sessionName = txtSessionName.Text.Trim();
                if (string.IsNullOrEmpty(sessionName))
                {
                    lblSessionMessage.Text = "Please enter a session name.";
                    lblSessionMessage.CssClass = "text-danger";
                    return;
                }

                var session = new UDIL.Shared.TestSession
                {
                    Name = sessionName,
                    Description = txtSessionDescription.Text.Trim(),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Status = "Active",
                    TestEnvironment = txtTestEnvironment.Text.Trim(),
                    DeviceCount = Convert.ToInt32(txtDeviceCount.Text.Trim() == "" ? "0" : txtDeviceCount.Text.Trim()),
                    TestType = txtTestType.Text.Trim(),
                    TestsCompleted = 0,
                    TotalTests = 0
                };

                var dbLayer = new DatabaseLayer();
                dbLayer.SaveSession(session);

                lblSessionMessage.Text = "Session created successfully!";
                lblSessionMessage.CssClass = "text-success";
                
                // Refresh displays
                LoadSessions();
                LoadCurrentSession();
            }
            catch (Exception ex)
            {
                lblSessionMessage.Text = "Error creating session: " + ex.Message;
                lblSessionMessage.CssClass = "text-danger";
            }
        }

        protected void btnLoadSession_Click(object sender, EventArgs e)
        {
            try
            {
                string sessionName = ddlSavedSessions.SelectedValue;
                if (string.IsNullOrEmpty(sessionName))
                {
                    lblSessionMessage.Text = "Please select a session to load.";
                    lblSessionMessage.CssClass = "text-danger";
                    return;
                }

                var dbLayer = new DatabaseLayer();
                var sessionDs = dbLayer.GetSession(sessionName);
                
                if (sessionDs.Tables.Count == 0 || sessionDs.Tables[0].Rows.Count == 0)
                {
                    throw new Exception($"Session '{sessionName}' not found");
                }
                
                System.Data.DataRow row = sessionDs.Tables[0].Rows[0];
                var session = new UDIL.Shared.TestSession
                {
                    SessionId = row["session_id"].ToString(),
                    Name = row["name"].ToString(),
                    Description = row["description"] != System.DBNull.Value ? row["description"].ToString() : null,
                    CreatedDate = Convert.ToDateTime(row["created_date"]),
                    ModifiedDate = Convert.ToDateTime(row["modified_date"]),
                    Status = row["status"].ToString(),
                    TestEnvironment = row["test_environment"].ToString(),
                    DeviceCount = Convert.ToInt32(row["device_count"]),
                    TestType = row["test_type"].ToString(),
                    TestsCompleted = Convert.ToInt32(row["tests_completed"]),
                    TotalTests = Convert.ToInt32(row["total_tests"])
                };
                UDIL.Shared.ConfigurationManager.SetCurrentSession(session);

                lblSessionMessage.Text = "Session loaded successfully!";
                lblSessionMessage.CssClass = "text-success";
                
                // Refresh displays
                LoadCurrentSession();
            }
            catch (Exception ex)
            {
                lblSessionMessage.Text = "Error loading session: " + ex.Message;
                lblSessionMessage.CssClass = "text-danger";
            }
        }

        protected void btnDeleteSession_Click(object sender, EventArgs e)
        {
            try
            {
                string sessionName = ddlSavedSessions.SelectedValue;
                if (string.IsNullOrEmpty(sessionName))
                {
                    lblSessionMessage.Text = "Please select a session to delete.";
                    lblSessionMessage.CssClass = "text-danger";
                    return;
                }

                var dbLayer = new DatabaseLayer();
                dbLayer.DeleteSession(sessionName);

                lblSessionMessage.Text = "Session deleted successfully!";
                lblSessionMessage.CssClass = "text-success";
                
                // Clear form and refresh displays
                txtSessionName.Text = "";
                txtSessionDescription.Text = "";
                LoadSessions();
                
                // Reset current session display if it was the deleted session
                var currentSession = UDIL.Shared.ConfigurationManager.GetCurrentSession();
                if (currentSession != null && currentSession.Name == sessionName)
                {
                    UDIL.Shared.ConfigurationManager.ClearCurrentSession();
                    ResetSessionDisplay();
                }
            }
            catch (Exception ex)
            {
                lblSessionMessage.Text = "Error deleting session: " + ex.Message;
                lblSessionMessage.CssClass = "text-danger";
            }
        }

        protected void btnExportSession_Click(object sender, EventArgs e)
        {
            try
            {
                var currentSession = UDIL.Shared.ConfigurationManager.GetCurrentSession();
                if (currentSession == null)
                {
                    lblSessionMessage.Text = "No active session to export.";
                    lblSessionMessage.CssClass = "text-danger";
                    return;
                }

                // Get failed test results for current session
                var dbLayer = new DatabaseLayer();
                DataSet failedTestsDs = dbLayer.GetFailedTestResultsBySession(currentSession.SessionId);

                if (failedTestsDs.Tables.Count == 0 || failedTestsDs.Tables[0].Rows.Count == 0)
                {
                    lblSessionMessage.Text = "No failed test results found for current session.";
                    lblSessionMessage.CssClass = "text-warning";
                    return;
                }

                DataTable failedTests = failedTestsDs.Tables[0];

                // Build professional HTML Excel content
                StringBuilder excelContent = new StringBuilder();
                excelContent.AppendLine("<html xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\" xmlns=\"http://www.w3.org/TR/REC-html40\">");
                excelContent.AppendLine("<head>");
                excelContent.AppendLine("<meta charset=\"utf-8\">");
                excelContent.AppendLine("<style>");
                excelContent.AppendLine("body { font-family: Calibri, Arial, sans-serif; }");
                excelContent.AppendLine(".title { font-size: 18pt; font-weight: bold; color: #FFFFFF; background-color: #DC3545; padding: 10px; text-align: center; }");
                excelContent.AppendLine(".subtitle { font-size: 12pt; font-weight: bold; color: #333333; background-color: #F8F9FA; padding: 8px; }");
                excelContent.AppendLine(".info { font-size: 10pt; padding: 5px; background-color: #E9ECEF; }");
                excelContent.AppendLine("table { border-collapse: collapse; width: 100%; }");
                excelContent.AppendLine("th { background-color: #495057; color: #FFFFFF; font-weight: bold; padding: 10px; border: 1px solid #333333; text-align: left; }");
                excelContent.AppendLine("td { padding: 8px; border: 1px solid #DEE2E6; font-size: 10pt; }");
                excelContent.AppendLine(".row-even { background-color: #F8F9FA; }");
                excelContent.AppendLine(".row-odd { background-color: #FFFFFF; }");
                excelContent.AppendLine(".status-fail { color: #DC3545; font-weight: bold; }");
                excelContent.AppendLine(".remarks { color: #6C757D; font-style: italic; }");
                excelContent.AppendLine("</style>");
                excelContent.AppendLine("</head>");
                excelContent.AppendLine("<body>");

                // Report Title
                excelContent.AppendLine("<table>");
                excelContent.AppendLine("<tr><td class=\"title\" colspan=\"9\">FAILED TEST RESULTS REPORT</td></tr>");
                excelContent.AppendLine("</table>");
                excelContent.AppendLine("<br>");

                // Session Information Section
                excelContent.AppendLine("<table>");
                excelContent.AppendLine("<tr><td class=\"subtitle\" colspan=\"2\">Session Information</td></tr>");
                excelContent.AppendLine($"<tr><td class=\"info\" style=\"width: 150px; font-weight: bold;\">Session Name:</td><td class=\"info\">{EscapeHtml(currentSession.Name)}</td></tr>");
                excelContent.AppendLine($"<tr><td class=\"info\" style=\"font-weight: bold;\">Session ID:</td><td class=\"info\">{currentSession.SessionId}</td></tr>");
                excelContent.AppendLine($"<tr><td class=\"info\" style=\"font-weight: bold;\">Export Date:</td><td class=\"info\">{DateTime.Now:yyyy-MM-dd HH:mm:ss}</td></tr>");
                excelContent.AppendLine($"<tr><td class=\"info\" style=\"font-weight: bold;\">Total Failed Tests:</td><td class=\"info\" style=\"color: #DC3545; font-weight: bold;\">{failedTests.Rows.Count}</td></tr>");
                excelContent.AppendLine("</table>");
                excelContent.AppendLine("<br>");

                // Data Table
                excelContent.AppendLine("<table>");
                // Header row
                excelContent.AppendLine("<tr>");
                //excelContent.AppendLine("<th style=\"width: 50px;\">ID</th>");
                excelContent.AppendLine("<th style=\"width: 200px;\">Test Name</th>");
                excelContent.AppendLine("<th style=\"width: 100px;\">Test Type</th>");
                excelContent.AppendLine("<th style=\"width: 80px;\">Status</th>");
                excelContent.AppendLine("<th style=\"width: 300px;\">Remarks</th>");
                excelContent.AppendLine("<th style=\"width: 140px;\">Test Date</th>");
                excelContent.AppendLine("<th style=\"width: 120px;\">Transaction ID</th>");
                excelContent.AppendLine("<th style=\"width: 120px;\">Global Device ID</th>");
                excelContent.AppendLine("</tr>");

                // Data rows with alternating colors
                int rowIndex = 0;
                foreach (DataRow row in failedTests.Rows)
                {
                    string rowClass = (rowIndex % 2 == 0) ? "row-even" : "row-odd";
                    //string id = row["id"].ToString();
                    string testName = EscapeHtml(row["test_name"].ToString());
                    string testType = EscapeHtml(row["test_type"].ToString());
                    string status = "FAIL";
                    string remarks = EscapeHtml(row["remarks"] != DBNull.Value ? row["remarks"].ToString() : "N/A");
                    string testDate = Convert.ToDateTime(row["test_date"]).ToString("yyyy-MM-dd HH:mm");
                    string transactionId = row["transaction_id"] != DBNull.Value ? row["transaction_id"].ToString() : "-";
                    string globalDeviceId = row["global_device_id"] != DBNull.Value ? row["global_device_id"].ToString() : "-";

                    excelContent.AppendLine($"<tr class=\"{rowClass}\">");
                    //excelContent.AppendLine($"<td>{id}</td>");
                    excelContent.AppendLine($"<td><b>{testName}</b></td>");
                    excelContent.AppendLine($"<td>{testType}</td>");
                    excelContent.AppendLine($"<td class=\"status-fail\">{status}</td>");
                    excelContent.AppendLine($"<td class=\"remarks\">{remarks}</td>");
                    excelContent.AppendLine($"<td>{testDate}</td>");
                    excelContent.AppendLine($"<td>{transactionId}</td>");
                    excelContent.AppendLine($"<td>{globalDeviceId}</td>");
                    excelContent.AppendLine("</tr>");

                    rowIndex++;
                }

                excelContent.AppendLine("</table>");
                excelContent.AppendLine("<br>");

                // Footer
                excelContent.AppendLine("<table>");
                excelContent.AppendLine($"<tr><td style=\"font-size: 9pt; color: #6C757D; text-align: center; padding: 10px;\">Generated by UDIL Tester | {DateTime.Now:yyyy-MM-dd HH:mm:ss}</td></tr>");
                excelContent.AppendLine("</table>");

                excelContent.AppendLine("</body>");
                excelContent.AppendLine("</html>");

                // Generate file name with timestamp
                string fileName = $"FailedTests_{currentSession.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.xls";

                // Set up response for file download
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Write(excelContent.ToString());
                Response.Flush();
                Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return;
            }
            catch (Exception ex)
            {
                lblSessionMessage.Text = "Error exporting failed tests: " + ex.Message;
                lblSessionMessage.CssClass = "text-danger";
            }
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            // If field contains comma, quote, or newline, wrap in quotes and escape quotes
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                field = field.Replace("\"", "\"\""); // Escape quotes
                return $"\"{field}\""; // Wrap in quotes
            }

            return field;
        }

        private string EscapeHtml(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
        }

        protected void ddlSavedSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlSavedSessions.SelectedValue))
            {
                btnLoadSession_Click(sender, e);
            }
        }

        #endregion

    }
}
