using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDIL.DAL
{
    public class NavigationItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string Group { get; set; }  // e.g., "Write Requests", "Read Requests", "Events Data Table"
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public string RequiredRole { get; set; }  // For role-based access control
        public bool IsExternal { get; set; }  // For external links
        
        public NavigationItem()
        {
            IsActive = true;
            SortOrder = 0;
            IsExternal = false;
        }
    }
}