using System;
using System.Web;
using System.Web.UI;

namespace UDIL
{
    public partial class AccessDenied : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // No authentication check needed for this page
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Clear session
            Session.Clear();
            Session.Abandon();
            
            // Clear remember me cookie
            HttpCookie rememberCookie = Request.Cookies["UDIL_Remember"];
            if (rememberCookie != null)
            {
                rememberCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(rememberCookie);
            }
            
            // Redirect to login page
            Response.Redirect("~/Login.aspx");
        }
    }
}
