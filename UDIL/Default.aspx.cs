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
using UDIL.DAL;
using UDIL.Shared;

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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/authorization_service");
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
                var configs = UDIL.Shared.ConfigurationManager.GetAllConfigurations();
                
                ddlSavedConfigs.Items.Clear();
                ddlSavedConfigs.Items.Add(new ListItem("-- Select Configuration --", ""));
                
                foreach (var config in configs)
                {
                    ddlSavedConfigs.Items.Add(new ListItem(config.Key, config.Key));
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
                    DbPort = txtDbPort.Text.Trim(),
                    DbName = txtDbName.Text.Trim(),
                    DbUid = txtDbUid.Text.Trim(),
                    DbPwd = txtDbPwd.Text.Trim(),
                    DbProvider = txtDbProvider.Text.Trim()
                };

                UDIL.Shared.ConfigurationManager.SaveConfiguration(configName, config);

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

                var config = UDIL.Shared.ConfigurationManager.LoadConfiguration(configName);
                
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

                UDIL.Shared.ConfigurationManager.DeleteConfiguration(configName);

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

    }
}
