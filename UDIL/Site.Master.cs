using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UDIL
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ScriptManager.RegisterClientScriptInclude(this.Page, this.Page.GetType(), "udil-app", ResolveUrl("~/udil-app.js"));
            }
        }
    }
}