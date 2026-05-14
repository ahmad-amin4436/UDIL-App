using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using UDIL.DAL;
using UDIL.Shared;

namespace UDIL.Pages
{
    public partial class TransactionStatus : UDIL.AuthenticatedPage
    {
        private const int FinalTrackerStage = 5;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["GetTransactionStatus"] == "true" && !string.IsNullOrEmpty(Request["transactionId"]))
            {
                string privateKey = GetPrivateKey();
                AppCommon.HandleTransactionStatusRequest(Request["transactionId"], privateKey);
                return;
            }

            tsPrivateKey.Text = GetPrivateKey() ?? string.Empty;

            if (!IsPostBack)
            {
                ScriptManager.RegisterClientScriptInclude(Page, Page.GetType(), "udil-tester", ResolveUrl("~/udil-tester.js"));
                LoadTransactionIds();
            }
        }

        protected void tsTransactionId_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowTrackerForSelectedTransaction();
        }

        protected void timerTracker_Tick(object sender, EventArgs e)
        {
            ShowTrackerForSelectedTransaction();
        }

        private void LoadTransactionIds()
        {
            tsTransactionId.Items.Clear();
            tsTransactionId.Items.Add(new ListItem("-- Select transaction --", string.Empty));

            try
            {
                DatabaseLayer dbLayer = new DatabaseLayer(System.Configuration.ConfigurationManager.ConnectionStrings["TestSuitConnenction"]?.ConnectionString);
                DataTable transactions = dbLayer.GetTransactions();

                foreach (DataRow row in transactions.Rows)
                {
                    string transactionId = row["transaction_id"]?.ToString();
                    if (string.IsNullOrWhiteSpace(transactionId))
                    {
                        continue;
                    }

                    string pageName = row["page_name"] == DBNull.Value ? string.Empty : row["page_name"].ToString();
                    string createdDate = row["created_date"] == DBNull.Value
                        ? string.Empty
                        : Convert.ToDateTime(row["created_date"]).ToString("yyyy-MM-dd HH:mm:ss");

                    string text = string.IsNullOrWhiteSpace(pageName)
                        ? $"{transactionId} - {createdDate}"
                        : $"{transactionId} - {pageName} - {createdDate}";

                    tsTransactionId.Items.Add(new ListItem(text, transactionId));
                }

                lblTransactionStatusMessage.Text = tsTransactionId.Items.Count > 1
                    ? string.Empty
                    : "No saved transaction IDs found.";
                lblTransactionStatusMessage.CssClass = tsTransactionId.Items.Count > 1 ? "mt-2 d-block" : "mt-2 d-block text-muted";
            }
            catch (Exception ex)
            {
                lblTransactionStatusMessage.Text = "Unable to load transaction IDs: " + ex.Message;
                lblTransactionStatusMessage.CssClass = "mt-2 d-block text-danger";
            }
        }

        private void ShowTrackerForSelectedTransaction()
        {
            string transactionId = tsTransactionId.SelectedValue;
            string privateKey = GetPrivateKey();

            if (string.IsNullOrWhiteSpace(transactionId))
            {
                pnlTracker.Visible = false;
                timerTracker.Enabled = false;
                lblTransactionStatusMessage.Text = "Select a transaction ID to show its tracker.";
                lblTransactionStatusMessage.CssClass = "mt-2 d-block text-muted";
                return;
            }

            if (string.IsNullOrWhiteSpace(privateKey))
            {
                pnlTracker.Visible = false;
                timerTracker.Enabled = false;
                lblTransactionStatusMessage.Text = "Please authorize first to obtain a private key.";
                lblTransactionStatusMessage.CssClass = "mt-2 d-block text-danger";
                return;
            }

            tsPrivateKey.Text = privateKey;
            SessionManager.CurrentTransactionId = transactionId;
            Session["CurrentPrivateKey"] = privateKey;

            pnlTracker.Visible = true;
            lblTrackerTransactionId.Text = transactionId;

            try
            {
                TransactionStatusResponse statusResponse = AppCommon.GetTransactionStatus(transactionId, privateKey);
                int trackerLevel = GetTrackerLevel(statusResponse);
                UpdateTrackerUI(trackerLevel);

                lblTransactionStatusMessage.Text = "Tracker loaded for selected transaction.";
                lblTransactionStatusMessage.CssClass = "mt-2 d-block text-success";
                timerTracker.Enabled = trackerLevel >= 0 && trackerLevel < FinalTrackerStage;
            }
            catch (WebException ex)
            {
                UpdateTrackerUI(0);
                lblTransactionStatusMessage.Text = "Unable to retrieve transaction status: " + ex.Message;
                lblTransactionStatusMessage.CssClass = "mt-2 d-block text-warning";
                timerTracker.Enabled = false;
            }
            catch (Exception ex)
            {
                UpdateTrackerUI(0);
                lblTransactionStatusMessage.Text = "Error loading tracker: " + ex.Message;
                lblTransactionStatusMessage.CssClass = "mt-2 d-block text-danger";
                timerTracker.Enabled = false;
            }
        }

        private int GetTrackerLevel(TransactionStatusResponse statusResponse)
        {
            if (statusResponse?.data == null || statusResponse.data.Count == 0)
            {
                return 0;
            }

            if (statusResponse.data.Any(item => item.indv_status == "2" && item.status_level == "4"))
            {
                return -1;
            }

            int maxLevel = 0;
            foreach (var item in statusResponse.data)
            {
                if (int.TryParse(item.status_level, out int level) && level > maxLevel)
                {
                    maxLevel = level;
                }
            }

            return Math.Min(maxLevel, FinalTrackerStage);
        }

        private void UpdateTrackerUI(int level)
        {
            ResetSteps();

            if (level == -1)
            {
                for (int i = 0; i <= FinalTrackerStage; i++)
                {
                    Label step = pnlTracker.FindControl("step" + i) as Label;
                    if (step != null)
                    {
                        step.CssClass = "tracker-step cancelled";
                    }
                }

                lblStage.Text = "Cancelled";
                lblStage.CssClass = "badge bg-danger px-3 py-2";
                lblStageDescription.Text = AppCommon.GetStageDescription(level);
                lblStageDescription.CssClass = "text-danger";
                progressBar.Style["width"] = "0%";
                return;
            }

            for (int i = 0; i <= FinalTrackerStage; i++)
            {
                Label step = pnlTracker.FindControl("step" + i) as Label;
                if (step == null)
                {
                    continue;
                }

                if (i < level)
                {
                    step.CssClass = "tracker-step completed";
                }
                else if (i == level)
                {
                    step.CssClass = "tracker-step active";
                }
            }

            bool completed = level >= FinalTrackerStage;
            lblStage.Text = completed ? "Completed" : $"Stage {level}";
            lblStage.CssClass = completed ? "badge bg-success px-3 py-2" : "badge bg-primary px-3 py-2";
            lblStageDescription.Text = AppCommon.GetStageDescription(level);
            lblStageDescription.CssClass = "text-muted";

            int percent = (level * 100) / FinalTrackerStage;
            progressBar.Style["width"] = percent + "%";
        }

        private void ResetSteps()
        {
            for (int i = 0; i <= FinalTrackerStage; i++)
            {
                Label step = pnlTracker.FindControl("step" + i) as Label;
                if (step != null)
                {
                    step.CssClass = "tracker-step";
                }
            }
        }

        private string GetPrivateKey()
        {
            string privateKey = SessionManager.PrivateKey;
            if (string.IsNullOrWhiteSpace(privateKey))
            {
                privateKey = Session["CurrentPrivateKey"] as string;
            }

            return privateKey;
        }
    }
}
