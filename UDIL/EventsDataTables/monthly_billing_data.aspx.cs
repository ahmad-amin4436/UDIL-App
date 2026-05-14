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
    public partial class monthly_billing_data : System.Web.UI.Page
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
                // If global device ID exists in session, use it for filtering
                if (UDIL.Shared.SessionManager.HasGlobalDeviceId)
                {
                    string deviceId = UDIL.Shared.SessionManager.GlobalDeviceId;
                    txtSearch.Text = deviceId;
                    SearchTerm = deviceId;
                }

                LoadMonthlyBillingData();
            }
        }

        private void LoadMonthlyBillingData()
        {
            try
            {
                Tables tables = new Tables(GetConnectionString());
                string searchTerm = SearchTerm;
                DataSet ds = tables.GetMonthlyBillingData(searchTerm);

                if (ds.Tables.Contains("MonthlyBillingData") && ds.Tables["MonthlyBillingData"].Rows.Count > 0)
                {
                    gvMonthlyBillingData.DataSource = ds.Tables["MonthlyBillingData"];
                    gvMonthlyBillingData.DataBind();
                }
                else
                {
                    gvMonthlyBillingData.DataSource = null;
                    gvMonthlyBillingData.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading monthly billing data: {ex.Message}");
            }
        }

        protected void gvMonthlyBillingData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMonthlyBillingData.PageIndex = e.NewPageIndex;
            LoadMonthlyBillingData();
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchTerm = txtSearch.Text.Trim();
            gvMonthlyBillingData.PageIndex = 0;
            LoadMonthlyBillingData();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            SearchTerm = string.Empty;
            gvMonthlyBillingData.PageIndex = 0;
            LoadMonthlyBillingData();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageSize = int.Parse(ddlPageSize.SelectedValue);
            gvMonthlyBillingData.PageSize = pageSize;
            gvMonthlyBillingData.PageIndex = 0;
            LoadMonthlyBillingData();
        }

        protected void gvMonthlyBillingData_DataBound(object sender, EventArgs e)
        {
            BindPagerControls();
            UpdateRecordCount();
        }

        private void UpdateRecordCount()
        {
            if (gvMonthlyBillingData.DataSource is DataTable dt)
            {
                int totalRecords = dt.Rows.Count;
                int currentPage = gvMonthlyBillingData.PageIndex + 1;
                int totalPages = gvMonthlyBillingData.PageCount;
                int pageSize = gvMonthlyBillingData.PageSize;
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
            Label lblPagerInfo = (Label)gvMonthlyBillingData.BottomPagerRow?.FindControl("lblPagerInfo");
            if (lblPagerInfo != null)
            {
                int currentPage = gvMonthlyBillingData.PageIndex + 1;
                int totalPages = gvMonthlyBillingData.PageCount;
                lblPagerInfo.Text = $"Page {currentPage:N0} of {totalPages:N0}";
            }

            // Bind page number repeater
            Repeater rptPageNumbers = (Repeater)gvMonthlyBillingData.BottomPagerRow?.FindControl("rptPageNumbers");
            if (rptPageNumbers != null)
            {
                List<int> pageNumbers = GetPageNumbers();
                rptPageNumbers.DataSource = pageNumbers;
                rptPageNumbers.DataBind();
            }

            // Enable/disable navigation buttons
            LinkButton lnkFirst = (LinkButton)gvMonthlyBillingData.BottomPagerRow?.FindControl("lnkFirst");
            LinkButton lnkPrev = (LinkButton)gvMonthlyBillingData.BottomPagerRow?.FindControl("lnkPrev");
            LinkButton lnkNext = (LinkButton)gvMonthlyBillingData.BottomPagerRow?.FindControl("lnkNext");
            LinkButton lnkLast = (LinkButton)gvMonthlyBillingData.BottomPagerRow?.FindControl("lnkLast");

            if (lnkFirst != null) lnkFirst.Enabled = gvMonthlyBillingData.PageIndex > 0;
            if (lnkPrev != null) lnkPrev.Enabled = gvMonthlyBillingData.PageIndex > 0;
            if (lnkNext != null) lnkNext.Enabled = gvMonthlyBillingData.PageIndex < gvMonthlyBillingData.PageCount - 1;
            if (lnkLast != null) lnkLast.Enabled = gvMonthlyBillingData.PageIndex < gvMonthlyBillingData.PageCount - 1;
        }

        private List<int> GetPageNumbers()
        {
            List<int> pageNumbers = new List<int>();
            int currentPage = gvMonthlyBillingData.PageIndex;
            int totalPages = gvMonthlyBillingData.PageCount;
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
                    gvMonthlyBillingData.PageIndex = 0;
                    break;
                case "Prev":
                    gvMonthlyBillingData.PageIndex = Math.Max(0, gvMonthlyBillingData.PageIndex - 1);
                    break;
                case "Next":
                    gvMonthlyBillingData.PageIndex = Math.Min(gvMonthlyBillingData.PageCount - 1, gvMonthlyBillingData.PageIndex + 1);
                    break;
                case "Last":
                    gvMonthlyBillingData.PageIndex = gvMonthlyBillingData.PageCount - 1;
                    break;
            }

            LoadMonthlyBillingData();
        }

        protected void rptPageNumbers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                int pageIndex = Convert.ToInt32(e.CommandArgument);
                gvMonthlyBillingData.PageIndex = pageIndex;
                LoadMonthlyBillingData();
            }
        }

        protected void btnGoToPage_Click(object sender, EventArgs e)
        {
            TextBox txtGoToPage = (TextBox)gvMonthlyBillingData.BottomPagerRow?.FindControl("txtGoToPage");
            if (txtGoToPage != null)
            {
                int pageNumber;
                if (int.TryParse(txtGoToPage.Text, out pageNumber))
                {
                    pageNumber = Math.Max(1, Math.Min(pageNumber, gvMonthlyBillingData.PageCount));
                    gvMonthlyBillingData.PageIndex = pageNumber - 1;
                    LoadMonthlyBillingData();
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
                btnMonthlyBillingDataFail.Visible = false;
                btnMonthlyBillingDataPass.CssClass = "btn btn-success btn-sm disabled";
                btnMonthlyBillingDataPass.Enabled = false;
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
                btnMonthlyBillingDataPass.Visible = false;
                btnMonthlyBillingDataFail.CssClass = "btn btn-danger btn-sm disabled";
                btnMonthlyBillingDataFail.Enabled = false;
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
                ((System.Web.UI.HtmlControls.HtmlControl)monthlyBillingDataRemarks).Style.Add("display", "block");
                txtMonthlyBillingDataRemarks.Text = "";
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
                ((System.Web.UI.HtmlControls.HtmlControl)monthlyBillingDataRemarks).Style.Add("display", "none");
                txtMonthlyBillingDataRemarks.Text = "";
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
                remarks = txtMonthlyBillingDataRemarks.Text.Trim();

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
                ((System.Web.UI.HtmlControls.HtmlControl)monthlyBillingDataRemarks).Style.Add("display", "none");
                txtMonthlyBillingDataRemarks.Text = "";
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