using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UDIL.DAL;

namespace UDIL.EventsDataTables
{
    public partial class billing_data : System.Web.UI.Page
    {
        private string SearchTerm
        {
            get { return ViewState["SearchTerm"] as string ?? string.Empty; }
            set { ViewState["SearchTerm"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBillingData();
            }
        }

        private void LoadBillingData()
        {
            try
            {
                Tables tables = new Tables(GetConnectionString());
                string searchTerm = SearchTerm;
                DataSet ds = tables.GetBillingData(searchTerm);

                if (ds.Tables.Contains("BillingData") && ds.Tables["BillingData"].Rows.Count > 0)
                {
                    gvBillingData.DataSource = ds.Tables["BillingData"];
                    gvBillingData.DataBind();
                }
                else
                {
                    gvBillingData.DataSource = null;
                    gvBillingData.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading billing data: {ex.Message}");
            }
        }

        protected void gvBillingData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBillingData.PageIndex = e.NewPageIndex;
            LoadBillingData();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SearchTerm = txtSearch.Text.Trim();
            gvBillingData.PageIndex = 0;
            LoadBillingData();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            SearchTerm = string.Empty;
            gvBillingData.PageIndex = 0;
            LoadBillingData();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageSize = int.Parse(ddlPageSize.SelectedValue);
            gvBillingData.PageSize = pageSize;
            gvBillingData.PageIndex = 0;
            LoadBillingData();
        }

        protected void gvBillingData_DataBound(object sender, EventArgs e)
        {
            BindPagerControls();
            UpdateRecordCount();
        }

        private void UpdateRecordCount()
        {
            if (gvBillingData.DataSource is DataTable dt)
            {
                int totalRecords = dt.Rows.Count;
                int currentPage = gvBillingData.PageIndex + 1;
                int totalPages = gvBillingData.PageCount;
                int pageSize = gvBillingData.PageSize;
                int startRecord = (currentPage - 1) * pageSize + 1;
                int endRecord = Math.Min(currentPage * pageSize, totalRecords);

                lblRecordCount.Text = $"Showing {startRecord:N0} - {endRecord:N0} of {totalRecords:N0} records";
            }
            else
            {
                lblRecordCount.Text = "No records found";
            }
        }

        private void BindPagerControls()
        {
            // Update pager info label
            Label lblPagerInfo = (Label)gvBillingData.BottomPagerRow?.FindControl("lblPagerInfo");
            if (lblPagerInfo != null)
            {
                int currentPage = gvBillingData.PageIndex + 1;
                int totalPages = gvBillingData.PageCount;
                lblPagerInfo.Text = $"Page {currentPage:N0} of {totalPages:N0}";
            }

            // Bind page number repeater
            Repeater rptPageNumbers = (Repeater)gvBillingData.BottomPagerRow?.FindControl("rptPageNumbers");
            if (rptPageNumbers != null)
            {
                List<int> pageNumbers = GetPageNumbers();
                rptPageNumbers.DataSource = pageNumbers;
                rptPageNumbers.DataBind();
            }

            // Enable/disable navigation buttons
            LinkButton lnkFirst = (LinkButton)gvBillingData.BottomPagerRow?.FindControl("lnkFirst");
            LinkButton lnkPrev = (LinkButton)gvBillingData.BottomPagerRow?.FindControl("lnkPrev");
            LinkButton lnkNext = (LinkButton)gvBillingData.BottomPagerRow?.FindControl("lnkNext");
            LinkButton lnkLast = (LinkButton)gvBillingData.BottomPagerRow?.FindControl("lnkLast");

            if (lnkFirst != null) lnkFirst.Enabled = gvBillingData.PageIndex > 0;
            if (lnkPrev != null) lnkPrev.Enabled = gvBillingData.PageIndex > 0;
            if (lnkNext != null) lnkNext.Enabled = gvBillingData.PageIndex < gvBillingData.PageCount - 1;
            if (lnkLast != null) lnkLast.Enabled = gvBillingData.PageIndex < gvBillingData.PageCount - 1;
        }

        private List<int> GetPageNumbers()
        {
            List<int> pageNumbers = new List<int>();
            int currentPage = gvBillingData.PageIndex;
            int totalPages = gvBillingData.PageCount;
            int pageRange = 5;

            int startPage = Math.Max(0, currentPage - pageRange);
            int endPage = Math.Min(totalPages - 1, currentPage + pageRange);

            for (int i = startPage; i <= endPage; i++)
            {
                pageNumbers.Add(i);
            }

            return pageNumbers;
        }

        protected void PageButton_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string commandArgument = btn.CommandArgument;

            switch (commandArgument)
            {
                case "First":
                    gvBillingData.PageIndex = 0;
                    break;
                case "Prev":
                    gvBillingData.PageIndex = Math.Max(0, gvBillingData.PageIndex - 1);
                    break;
                case "Next":
                    gvBillingData.PageIndex = Math.Min(gvBillingData.PageCount - 1, gvBillingData.PageIndex + 1);
                    break;
                case "Last":
                    gvBillingData.PageIndex = gvBillingData.PageCount - 1;
                    break;
            }

            LoadBillingData();
        }

        protected void rptPageNumbers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                int pageIndex = Convert.ToInt32(e.CommandArgument);
                gvBillingData.PageIndex = pageIndex;
                LoadBillingData();
            }
        }

        protected void btnGoToPage_Click(object sender, EventArgs e)
        {
            TextBox txtGoToPage = (TextBox)gvBillingData.BottomPagerRow?.FindControl("txtGoToPage");
            if (txtGoToPage != null)
            {
                int pageNumber;
                if (int.TryParse(txtGoToPage.Text, out pageNumber))
                {
                    pageNumber = Math.Max(1, Math.Min(pageNumber, gvBillingData.PageCount));
                    gvBillingData.PageIndex = pageNumber - 1;
                    LoadBillingData();
                }
            }
        }

        private string GetConnectionString()
        {
            return UDIL.Shared.ConfigurationManager.GetConnectionString();
        }

        protected void TableButton_Command(object sender, CommandEventArgs e)
        {
            string tableName = e.CommandArgument.ToString();
            string action = e.CommandName;

            if (action == "Fail")
            {
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
                btnBillingDataFail.Visible = false;
                btnBillingDataPass.CssClass = "btn btn-success btn-sm disabled";
                btnBillingDataPass.Enabled = false;
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
                btnBillingDataPass.Visible = false;
                btnBillingDataFail.CssClass = "btn btn-danger btn-sm disabled";
                btnBillingDataFail.Enabled = false;
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
                ((System.Web.UI.HtmlControls.HtmlControl)billingDataRemarks).Style.Add("display", "block");
                txtBillingDataRemarks.Text = "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing fail remarks section: {ex.Message}");
            }
        }

        private void HideRemarksSection(string tableName)
        {
            try
            {
                ((System.Web.UI.HtmlControls.HtmlControl)billingDataRemarks).Style.Add("display", "none");
                txtBillingDataRemarks.Text = "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error hiding remarks section: {ex.Message}");
            }
        }

        protected void SaveRemarks_Command(object sender, CommandEventArgs e)
        {
            string tableName = e.CommandArgument.ToString();
            string remarks = "";

            try
            {
                remarks = txtBillingDataRemarks.Text.Trim();

                if (!string.IsNullOrEmpty(remarks))
                {
                    HandleTableAction(tableName, "Fail", remarks);
                    System.Diagnostics.Debug.WriteLine($"Remarks saved for {tableName}: {remarks}");
                    HideRemarksSection(tableName);
                    MakeTableCardFailed(tableName);
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "showFailAlert",
                        $"alert('{tableName} marked as Fail with remarks: {remarks.Replace("'", "\\'")}');", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving remarks: {ex.Message}");
            }
        }

        protected void CancelRemarks_Command(object sender, CommandEventArgs e)
        {
            string tableName = e.CommandArgument.ToString();

            try
            {
                ((System.Web.UI.HtmlControls.HtmlControl)billingDataRemarks).Style.Add("display", "none");
                txtBillingDataRemarks.Text = "";
                System.Diagnostics.Debug.WriteLine($"Remarks section hidden for {tableName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error canceling remarks: {ex.Message}");
            }
        }

        private void HandleTableAction(string tableName, string action, string reason)
        {
            try
            {
                SaveTestResult(tableName, action, reason);
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

        private void SaveTestResult(string testName, string status, string remarks)
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
                    TransactionId = null,
                    GlobalDeviceId = null
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