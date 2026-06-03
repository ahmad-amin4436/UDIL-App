using System.Web.UI;
using System.Web.UI.WebControls;
using UDIL.Shared;

namespace UDIL.Shared.Web
{
    public static class TrackerUiHelper
    {
        public static void UpdateTrackerUI(Panel pnlTracker, Label lblStage, Label lblStageDescription, Panel progressBar, int level)
        {
            ResetSteps(pnlTracker);

            if (level == -1)
            {
                for (int i = 0; i <= UdilConstants.MaxTrackerStage; i++)
                {
                    var step = pnlTracker.FindControl("step" + i) as Label;
                    if (step != null)
                    {
                        step.CssClass = "tracker-step cancelled";
                    }
                }

                lblStage.Text = "Cancelled";
                lblStage.CssClass = "badge bg-dark-danger px-3 py-2";
                lblStageDescription.Text = AppCommon.GetStageDescription(level);
                lblStageDescription.CssClass = "text-danger";
                progressBar.Style["width"] = "0%";
                return;
            }

            for (int i = 0; i <= UdilConstants.MaxTrackerStage; i++)
            {
                var step = pnlTracker.FindControl("step" + i) as Label;
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
                else
                {
                    step.CssClass = "tracker-step";
                }
            }

            lblStage.Text = $"Stage {level}";
            lblStage.CssClass = "badge bg-primary px-3 py-2";
            lblStageDescription.Text = AppCommon.GetStageDescription(level);
            lblStageDescription.CssClass = "text-muted";

            int percent = (level * 100) / UdilConstants.FinalTrackerStage;
            progressBar.Style["width"] = percent + "%";
        }

        public static void UpdateTrackerUIWithTimeout(Panel pnlTracker, Label lblStage, Panel progressBar, int failedStage, int currentStage)
        {
            ResetSteps(pnlTracker);

            for (int i = 0; i < failedStage; i++)
            {
                var step = pnlTracker.FindControl("step" + i) as Label;
                if (step != null)
                {
                    step.CssClass = "tracker-step completed";
                }
            }

            var failedStep = pnlTracker.FindControl("step" + failedStage) as Label;
            if (failedStep != null)
            {
                failedStep.CssClass = "tracker-step failed";
            }

            var activeStep = pnlTracker.FindControl("step" + currentStage) as Label;
            if (activeStep != null)
            {
                activeStep.CssClass = "tracker-step active";
            }

            lblStage.Text = $"Stage {currentStage}";
            lblStage.CssClass = "badge bg-warning px-3 py-2";

            int percent = (currentStage * 100) / UdilConstants.FinalTrackerStage;
            progressBar.Style["width"] = percent + "%";
        }

        public static void ResetSteps(Panel pnlTracker)
        {
            for (int i = 0; i <= UdilConstants.MaxTrackerStage; i++)
            {
                var step = pnlTracker.FindControl("step" + i) as Label;
                if (step != null)
                {
                    step.CssClass = "tracker-step";
                }
            }
        }
    }
}
