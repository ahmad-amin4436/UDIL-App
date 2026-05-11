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
    public partial class load_profile_data : System.Web.UI.Page
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
                LoadLoadProfileData();
            }
        }

        private void LoadLoadProfileData()
        {
            try
            {
                Tables tables = new Tables(GetConnectionString());
                string searchTerm = SearchTerm;
                DataSet ds = tables.GetLoadProfileData(searchTerm);

                if (ds.Tables.Contains("LoadProfileData") && ds.Tables["LoadProfileData"].Rows.Count > 0)
                {
                    gvLoadProfileData.DataSource = ds.Tables["LoadProfileData"];
                    gvLoadProfileData.DataBind();
                }
                else
                {
                    gvLoadProfileData.DataSource = null;
                    gvLoadProfileData.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading load profile data: {ex.Message}");
            }
        }

        protected void gvLoadProfileData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLoadProfileData.PageIndex = e.NewPageIndex;
            LoadLoadProfileData();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SearchTerm = txtSearch.Text.Trim();
            gvLoadProfileData.PageIndex = 0;
            LoadLoadProfileData();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            SearchTerm = string.Empty;
            gvLoadProfileData.PageIndex = 0;
            LoadLoadProfileData();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageSize = int.Parse(ddlPageSize.SelectedValue);
            gvLoadProfileData.PageSize = pageSize;
            gvLoadProfileData.PageIndex = 0;
            LoadLoadProfileData();
        }

        protected void gvLoadProfileData_DataBound(object sender, EventArgs e)
        {
            BindPagerControls();
            UpdateRecordCount();
        }

        private void UpdateRecordCount()
        {
            if (gvLoadProfileData.DataSource is DataTable dt)
            {
                int totalRecords = dt.Rows.Count;
                int currentPage = gvLoadProfileData.PageIndex + 1;
                int totalPages = gvLoadProfileData.PageCount;
                int pageSize = gvLoadProfileData.PageSize;
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
            Label lblPagerInfo = (Label)gvLoadProfileData.BottomPagerRow?.FindControl("lblPagerInfo");
            if (lblPagerInfo != null)
            {
                int currentPage = gvLoadProfileData.PageIndex + 1;
                int totalPages = gvLoadProfileData.PageCount;
                lblPagerInfo.Text = $"Page {currentPage:N0} of {totalPages:N0}";
            }

            // Bind page number repeater
            Repeater rptPageNumbers = (Repeater)gvLoadProfileData.BottomPagerRow?.FindControl("rptPageNumbers");
            if (rptPageNumbers != null)
            {
                List<int> pageNumbers = GetPageNumbers();
                rptPageNumbers.DataSource = pageNumbers;
                rptPageNumbers.DataBind();
            }

            // Enable/disable navigation buttons
            LinkButton lnkFirst = (LinkButton)gvLoadProfileData.BottomPagerRow?.FindControl("lnkFirst");
            LinkButton lnkPrev = (LinkButton)gvLoadProfileData.BottomPagerRow?.FindControl("lnkPrev");
            LinkButton lnkNext = (LinkButton)gvLoadProfileData.BottomPagerRow?.FindControl("lnkNext");
            LinkButton lnkLast = (LinkButton)gvLoadProfileData.BottomPagerRow?.FindControl("lnkLast");

            if (lnkFirst != null) lnkFirst.Enabled = gvLoadProfileData.PageIndex > 0;
            if (lnkPrev != null) lnkPrev.Enabled = gvLoadProfileData.PageIndex > 0;
            if (lnkNext != null) lnkNext.Enabled = gvLoadProfileData.PageIndex < gvLoadProfileData.PageCount - 1;
            if (lnkLast != null) lnkLast.Enabled = gvLoadProfileData.PageIndex < gvLoadProfileData.PageCount - 1;
        }

        private List<int> GetPageNumbers()
        {
            List<int> pageNumbers = new List<int>();
            int currentPage = gvLoadProfileData.PageIndex;
            int totalPages = gvLoadProfileData.PageCount;
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
                    gvLoadProfileData.PageIndex = 0;
                    break;
                case "Prev":
                    gvLoadProfileData.PageIndex = Math.Max(0, gvLoadProfileData.PageIndex - 1);
                    break;
                case "Next":
                    gvLoadProfileData.PageIndex = Math.Min(gvLoadProfileData.PageCount - 1, gvLoadProfileData.PageIndex + 1);
                    break;
                case "Last":
                    gvLoadProfileData.PageIndex = gvLoadProfileData.PageCount - 1;
                    break;
            }

            LoadLoadProfileData();
        }

        protected void rptPageNumbers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                int pageIndex = Convert.ToInt32(e.CommandArgument);
                gvLoadProfileData.PageIndex = pageIndex;
                LoadLoadProfileData();
            }
        }

        protected void btnGoToPage_Click(object sender, EventArgs e)
        {
            TextBox txtGoToPage = (TextBox)gvLoadProfileData.BottomPagerRow?.FindControl("txtGoToPage");
            if (txtGoToPage != null)
            {
                int pageNumber;
                if (int.TryParse(txtGoToPage.Text, out pageNumber))
                {
                    pageNumber = Math.Max(1, Math.Min(pageNumber, gvLoadProfileData.PageCount));
                    gvLoadProfileData.PageIndex = pageNumber - 1;
                    LoadLoadProfileData();
                }
            }
        }

        private string GetConnectionString()
        {
            return UDIL.Shared.ConfigurationManager.GetConnectionString();
        }
    }
}