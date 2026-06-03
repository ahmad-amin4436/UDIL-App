using System;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using UDIL.Shared;

namespace UDIL.Shared.Web
{
    public sealed class TrackerPageContext
    {
        public HttpSessionState Session { get; set; }
        public Timer TimerTracker { get; set; }
        public Timer TimerTables { get; set; }
        public Panel PnlTracker { get; set; }
        public Panel PnlDataTables { get; set; }
        public Label LblStage { get; set; }
        public Label LblStageDescription { get; set; }
        public Panel ProgressBar { get; set; }
        public GridView GvMeterVisuals { get; set; }
        public GridView GvCommunicationHistory { get; set; }
        public GridView GvEvents { get; set; }
        public Func<string> GetConnectionString { get; set; }
        public Func<string> GetGlobalDeviceId { get; set; }
        public Action<string, string, string> SaveApiTestResult { get; set; }
        public Action ReenableSubmitButton { get; set; }
    }

    public static class OperationPageSupport
    {
        public static void ConfigureTimers(Timer tracker, Timer tables)
        {
            if (tracker != null)
            {
                tracker.Interval = UdilConstants.TrackerTimerIntervalMs;
            }

            if (tables != null)
            {
                tables.Interval = UdilConstants.TablesTimerIntervalMs;
            }
        }

        public static void InitializeCommandPage(bool isPostBack, Action initSessionFields, Action initDefaults)
        {
            if (!isPostBack)
            {
                initSessionFields?.Invoke();
                initDefaults?.Invoke();
            }
        }

        public static void RunTrackerTick(TrackerPageContext ctx)
        {
            if (ctx == null)
            {
                return;
            }

            string transactionId = ctx.Session[UdilConstants.SessionCurrentTransactionId] as string;
            string privateKey = ctx.Session[UdilConstants.SessionCurrentPrivateKey] as string;

            TrackerTickResult tick = TransactionTrackerService.ProcessTick(ctx.Session, transactionId, privateKey);
            if (tick.SkippedPoll)
            {
                return;
            }

            if (tick.IsCancelled)
            {
                ctx.TimerTracker.Enabled = false;
                TrackerUiHelper.UpdateTrackerUI(ctx.PnlTracker, ctx.LblStage, ctx.LblStageDescription, ctx.ProgressBar, -1);
                ctx.LblStageDescription.Text = tick.CancelReason;
                ctx.LblStageDescription.CssClass = "text-danger";
                ctx.ReenableSubmitButton?.Invoke();
                return;
            }

            if (tick.StageTimedOut)
            {
                TrackerUiHelper.UpdateTrackerUIWithTimeout(ctx.PnlTracker, ctx.LblStage, ctx.ProgressBar, tick.TimedOutStage, tick.NextStageAfterTimeout);
                ctx.LblStageDescription.Text = tick.TimeoutMessage;
                ctx.LblStageDescription.CssClass = "text-warning";
            }

            TrackerUiHelper.UpdateTrackerUI(ctx.PnlTracker, ctx.LblStage, ctx.LblStageDescription, ctx.ProgressBar, tick.MaxLevel);

            if (tick.IsCompleted)
            {
                ctx.TimerTracker.Enabled = false;
                ctx.LblStage.Text = "Completed";
                ctx.LblStage.CssClass = "badge bg-success px-3 py-2";
                ctx.SaveApiTestResult?.Invoke(transactionId, "Pass", "API test completed successfully via tracker");
                RunTablesRefresh(ctx, forceRefresh: true);
                if (ctx.TimerTables != null)
                {
                    ctx.TimerTables.Enabled = true;
                }
            }
        }

        public static void RunTablesTick(TrackerPageContext ctx)
        {
            RunTablesRefresh(ctx, forceRefresh: false);
        }

        public static void RunTablesRefresh(TrackerPageContext ctx, bool forceRefresh)
        {
            if (ctx == null)
            {
                return;
            }

            string globalDeviceId = ctx.GetGlobalDeviceId?.Invoke();
            if (string.IsNullOrWhiteSpace(globalDeviceId))
            {
                return;
            }

            string conn = ctx.GetConnectionString?.Invoke();
            if (string.IsNullOrEmpty(conn))
            {
                return;
            }

            MeterTablesRefreshResult refresh = MeterTablesRefreshService.Refresh(ctx.Session, conn, globalDeviceId, forceRefresh);
            MeterValidationGridBinder.BindIfChanged(refresh, ctx.GvMeterVisuals, ctx.GvCommunicationHistory, ctx.GvEvents);

            if (ctx.PnlDataTables != null)
            {
                ctx.PnlDataTables.Visible = true;
            }
        }
    }
}
