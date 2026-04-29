using System;
using System.Web;
using System.Web.UI;

namespace UDIL
{
    public static class AuthHelper
    {
        // Check if user is authenticated
        public static bool IsAuthenticated()
        {
            return HttpContext.Current.Session["UserId"] != null && 
                   !string.IsNullOrEmpty(HttpContext.Current.Session["UserId"].ToString());
        }

        // Get current user information
        public static string GetCurrentUserId()
        {
            return HttpContext.Current.Session["UserId"]?.ToString() ?? string.Empty;
        }

        public static string GetCurrentUsername()
        {
            return HttpContext.Current.Session["Username"]?.ToString() ?? string.Empty;
        }

        public static string GetCurrentFullName()
        {
            return HttpContext.Current.Session["FullName"]?.ToString() ?? string.Empty;
        }

        public static string GetCurrentRole()
        {
            return HttpContext.Current.Session["Role"]?.ToString() ?? "Guest";
        }

        // Redirect to login page if not authenticated
        public static void RequireAuthentication()
        {
            if (!IsAuthenticated())
            {
                HttpContext.Current.Response.Redirect("~/Login.aspx?ReturnUrl=" + 
                    HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.Url.AbsoluteUri));
                HttpContext.Current.Response.End();
            }
        }

        // Logout user
        public static void Logout()
        {
            // Clear session
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            
            // Clear remember me cookie
            HttpCookie rememberCookie = HttpContext.Current.Request.Cookies["UDIL_Remember"];
            if (rememberCookie != null)
            {
                rememberCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(rememberCookie);
            }
            
            // Redirect to login page
            HttpContext.Current.Response.Redirect("~/Login.aspx");
        }

        // Check if user has specific role
        public static bool HasRole(string requiredRole)
        {
            if (!IsAuthenticated())
                return false;
                
            string userRole = GetCurrentRole();
            return string.Equals(userRole, requiredRole, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase);
        }

        // Require specific role
        public static void RequireRole(string requiredRole)
        {
            RequireAuthentication();
            
            if (!HasRole(requiredRole))
            {
                HttpContext.Current.Response.Redirect("~/AccessDenied.aspx");
                HttpContext.Current.Response.End();
            }
        }
    }

    // Base page class with authentication
    public class AuthenticatedPage : Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            // Check authentication on every page load
            if (!IsLoginPage())
            {
                AuthHelper.RequireAuthentication();
            }
        }

        private bool IsLoginPage()
        {
            string currentUrl = Request.Url.AbsolutePath.ToLower();
            return currentUrl.Contains("login.aspx") || currentUrl.Contains("accessdenied.aspx");
        }

        protected void LogoutUser()
        {
            AuthHelper.Logout();
        }
    }
}
