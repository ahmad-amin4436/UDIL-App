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
    public partial class instantaneous_data : System.Web.UI.Page
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
                LoadInstantaneousData();
            }
        }

        private void LoadInstantaneousData()
        {
            try
            {
                Tables tables = new Tables(GetConnectionString());
                string searchTerm = SearchTerm;
                DataSet ds = tables.GetInstantaneousData(searchTerm);

                if (ds.Tables.Contains("InstantaneousData") && ds.Tables["InstantaneousData"].Rows.Count > 0)
                {
                    gvInstantaneousData.DataSource = ds.Tables["InstantaneousData"];
                    gvInstantaneousData.DataBind();
                }
                else
                {
                    gvInstantaneousData.DataSource = null;
                    gvInstantaneousData.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading instantaneous data: {ex.Message}");
            }
        }

        protected void gvInstantaneousData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvInstantaneousData.PageIndex = e.NewPageIndex;
            LoadInstantaneousData();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SearchTerm = txtSearch.Text.Trim();
            gvInstantaneousData.PageIndex = 0;
            LoadInstantaneousData();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            SearchTerm = string.Empty;
            gvInstantaneousData.PageIndex = 0;
            LoadInstantaneousData();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageSize = int.Parse(ddlPageSize.SelectedValue);
            gvInstantaneousData.PageSize = pageSize;
            gvInstantaneousData.PageIndex = 0;
            LoadInstantaneousData();
        }

        protected void gvInstantaneousData_DataBound(object sender, EventArgs e)
        {
            BindPagerControls();
            UpdateRecordCount();
        }

        private void UpdateRecordCount()
        {
            if (gvInstantaneousData.DataSource is DataTable dt)
            {
                int totalRecords = dt.Rows.Count;
                int currentPage = gvInstantaneousData.PageIndex + 1;
                int totalPages = gvInstantaneousData.PageCount;
                int pageSize = gvInstantaneousData.PageSize;
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
            Label lblPagerInfo = (Label)gvInstantaneousData.BottomPagerRow?.FindControl("lblPagerInfo");
            if (lblPagerInfo != null)
            {
                int currentPage = gvInstantaneousData.PageIndex + 1;
                int totalPages = gvInstantaneousData.PageCount;
                lblPagerInfo.Text = $"Page {currentPage:N0} of {totalPages:N0}";
            }

            // Bind page number repeater
            Repeater rptPageNumbers = (Repeater)gvInstantaneousData.BottomPagerRow?.FindControl("rptPageNumbers");
            if (rptPageNumbers != null)
            {
                List<int> pageNumbers = GetPageNumbers();
                rptPageNumbers.DataSource = pageNumbers;
                rptPageNumbers.DataBind();
            }

            // Enable/disable navigation buttons
            LinkButton lnkFirst = (LinkButton)gvInstantaneousData.BottomPagerRow?.FindControl("lnkFirst");
            LinkButton lnkPrev = (LinkButton)gvInstantaneousData.BottomPagerRow?.FindControl("lnkPrev");
            LinkButton lnkNext = (LinkButton)gvInstantaneousData.BottomPagerRow?.FindControl("lnkNext");
            LinkButton lnkLast = (LinkButton)gvInstantaneousData.BottomPagerRow?.FindControl("lnkLast");

            if (lnkFirst != null) lnkFirst.Enabled = gvInstantaneousData.PageIndex > 0;
            if (lnkPrev != null) lnkPrev.Enabled = gvInstantaneousData.PageIndex > 0;
            if (lnkNext != null) lnkNext.Enabled = gvInstantaneousData.PageIndex < gvInstantaneousData.PageCount - 1;
            if (lnkLast != null) lnkLast.Enabled = gvInstantaneousData.PageIndex < gvInstantaneousData.PageCount - 1;
        }

        private List<int> GetPageNumbers()
        {
            List<int> pageNumbers = new List<int>();
            int currentPage = gvInstantaneousData.PageIndex;
            int totalPages = gvInstantaneousData.PageCount;
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
                    gvInstantaneousData.PageIndex = 0;
                    break;
                case "Prev":
                    gvInstantaneousData.PageIndex = Math.Max(0, gvInstantaneousData.PageIndex - 1);
                    break;
                case "Next":
                    gvInstantaneousData.PageIndex = Math.Min(gvInstantaneousData.PageCount - 1, gvInstantaneousData.PageIndex + 1);
                    break;
                case "Last":
                    gvInstantaneousData.PageIndex = gvInstantaneousData.PageCount - 1;
                    break;
            }

            LoadInstantaneousData();
        }

        protected void rptPageNumbers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                int pageIndex = Convert.ToInt32(e.CommandArgument);
                gvInstantaneousData.PageIndex = pageIndex;
                LoadInstantaneousData();
            }
        }

        protected void btnGoToPage_Click(object sender, EventArgs e)
        {
            TextBox txtGoToPage = (TextBox)gvInstantaneousData.BottomPagerRow?.FindControl("txtGoToPage");
            if (txtGoToPage != null)
            {
                int pageNumber;
                if (int.TryParse(txtGoToPage.Text, out pageNumber))
                {
                    pageNumber = Math.Max(1, Math.Min(pageNumber, gvInstantaneousData.PageCount));
                    gvInstantaneousData.PageIndex = pageNumber - 1;
                    LoadInstantaneousData();
                }
            }
        }

        private string GetConnectionString()
        {
            return UDIL.Shared.ConfigurationManager.GetConnectionString();
        }
    }
}