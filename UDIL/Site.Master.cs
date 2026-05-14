using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UDIL.DAL;

namespace UDIL
{
    public partial class SiteMaster : MasterPage
    {
        private DatabaseLayer _dbLayer;

        protected void Page_Load(object sender, EventArgs e)
        {
            _dbLayer = new DatabaseLayer();
        }

        protected string GetActiveClass(string pageName, string queryString = null)
        {
            string currentPath = Request.AppRelativeCurrentExecutionFilePath.ToLower();
            string currentQuery = Request.QueryString["page"];
            
            if (!string.IsNullOrEmpty(queryString))
            {
                return currentPath.Contains(pageName) && currentQuery == queryString ? "active" : "";
            }
            else
            {
                return currentPath.Contains(pageName) && currentQuery == null ? "active" : "";
            }
        }

        /// <summary>
        /// Get navigation items grouped by their category
        /// </summary>
        protected Dictionary<string, List<NavigationItem>> GetGroupedNavigationItems()
        {
            try
            {
                if (_dbLayer == null)
                {
                    _dbLayer = new DatabaseLayer();
                }
                
                return _dbLayer.GetGroupedNavigationItems();
            }
            catch (Exception ex)
            {
                // Log error and return empty dictionary
                System.Diagnostics.Debug.WriteLine($"Error loading navigation: {ex.Message}");
                return new Dictionary<string, List<NavigationItem>>();
            }
        }

        /// <summary>
        /// Check if user has permission to view a navigation item based on role
        /// </summary>
        protected bool HasNavigationPermission(string requiredRole)
        {
            // If no role is required, everyone can see it
            if (string.IsNullOrEmpty(requiredRole))
            {
                return true;
            }

            // Get user role from session
            string userRole = Session["Role"] as string;
            
            if (string.IsNullOrEmpty(userRole))
            {
                return false;
            }

            // Check if user role matches required role
            // You can implement more complex role checking logic here
            return userRole.Equals(requiredRole, StringComparison.OrdinalIgnoreCase) || 
                   userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Get the appropriate icon class for a navigation item
        /// </summary>
        protected string GetIconClass(string icon)
        {
            if (string.IsNullOrEmpty(icon))
            {
                return "bi bi-circle"; // Default icon
            }

            // If icon already starts with "bi-", use it as is
            if (icon.StartsWith("bi-"))
            {
                return icon;
            }

            // Otherwise, prepend "bi bi-"
            return $"bi bi-{icon}";
        }

        /// <summary>
        /// Check if a navigation item is for an external link
        /// </summary>
        protected string GetTargetAttribute(bool isExternal)
        {
            return isExternal ? "_blank" : "_self";
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
