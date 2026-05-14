using System;
using System.Configuration;
using System.Data;
using System.Security.Policy;
using System.Web;
using System.Web.UI;
using UDIL.DAL;

namespace UDIL
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is already logged in
            if (Session["UserId"] != null && !string.IsNullOrEmpty(Session["UserId"].ToString()))
            {
                // Redirect to main application
                Response.Redirect("Default.aspx");
                return;
            }

            
            
            if (!IsPostBack)
            {
                // Clear any existing session
                Session.Clear();
                Session.Abandon();
                // Hide alert panel on initial load
                pnlAlert.Visible = false;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Validate input
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowAlert("Please enter both username and password.", "danger");
                return;
            }

            try
            {
                // Database authentication
                DatabaseLayer dbLayer = new DatabaseLayer();
                
                if (dbLayer.ValidateUser(username, password))
                {
                    // Get user details
                    DataSet userDs = dbLayer.GetUserDetails(username);
                    
                    if (userDs.Tables.Count > 0 && userDs.Tables[0].Rows.Count > 0)
                    {
                        DataRow userRow = userDs.Tables[0].Rows[0];
                        
                        // Create session
                        Session["UserId"] = userRow["user_id"].ToString();
                        Session["Username"] = userRow["username"].ToString();
                        Session["FullName"] = userRow["full_name"].ToString();
                        Session["Email"] = userRow["email"].ToString();
                        Session["Role"] = userRow["role"].ToString();
                        Session["LoginTime"] = DateTime.Now.ToString();
                        
                        // Update last login in database
                        dbLayer.UpdateLastLogin(username);
                        
                        // Handle remember me
                        if (chkRememberMe.Checked)
                        {
                            // Create persistent cookie
                            HttpCookie rememberCookie = new HttpCookie("UDIL_Remember", username);
                            rememberCookie.Expires = DateTime.Now.AddDays(30);
                            Response.Cookies.Add(rememberCookie);
                        }
                        
                        // Redirect to intended page or default
                        string returnUrl = Request.QueryString["ReturnUrl"];
                        if (!string.IsNullOrEmpty(returnUrl) && IsLocalUrl(returnUrl))
                        {
                            Response.Redirect(returnUrl);
                        }
                        else
                        {
                            Response.Redirect("Dashboard.aspx");
                        }
                    }
                    else
                    {
                        ShowAlert("Unable to retrieve user details. Please contact administrator.", "danger");
                    }
                }
                else
                {
                    ShowAlert("Invalid username or password. Please try again.", "danger");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Login error: " + ex.Message, "danger");
                // Log error for debugging
                System.Diagnostics.Debug.WriteLine("Login Error: " + ex.ToString());
            }
        }

        private void ShowAlert(string message, string alertType)
        {
            pnlAlert.Visible = true;
            lblMessage.Text = message;
            
            // Set alert CSS class based on type
            switch (alertType.ToLower())
            {
                case "success":
                    pnlAlert.CssClass = "alert alert-success alert-dismissible fade show";
                    break;
                case "warning":
                    pnlAlert.CssClass = "alert alert-warning alert-dismissible fade show";
                    break;
                case "info":
                    pnlAlert.CssClass = "alert alert-info alert-dismissible fade show";
                    break;
                case "danger":
                default:
                    pnlAlert.CssClass = "alert alert-danger alert-dismissible fade show";
                    break;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            // Check for remember me cookie
            if (!IsPostBack)
            {
                HttpCookie rememberCookie = Request.Cookies["UDIL_Remember"];
                if (rememberCookie != null && !string.IsNullOrEmpty(rememberCookie.Value))
                {
                    txtUsername.Text = rememberCookie.Value;
                    chkRememberMe.Checked = true;
                    txtPassword.Focus();
                }
            }
        }

        private bool IsLocalUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                return false;

            return uri.Host == Request.Url.Host && 
                   uri.Port == Request.Url.Port &&
                   uri.Scheme == Request.Url.Scheme;
        }
    }
}
