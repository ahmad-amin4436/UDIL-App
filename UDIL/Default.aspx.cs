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

namespace UDIL
{
    public partial class _Default : Page
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

                // Generate export data (JSON format)
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string exportData = serializer.Serialize(currentSession);
                
                // Generate file name with timestamp
                string fileName = $"Session_{currentSession.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                
                // Set up response for file download
                Response.Clear();
                Response.ContentType = "application/json";
                Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");
                Response.Write(exportData);
                Response.End();

                lblSessionMessage.Text = "Session exported successfully!";
                lblSessionMessage.CssClass = "text-success";
            }
            catch (Exception ex)
            {
                lblSessionMessage.Text = "Error exporting session: " + ex.Message;
                lblSessionMessage.CssClass = "text-danger";
            }
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
