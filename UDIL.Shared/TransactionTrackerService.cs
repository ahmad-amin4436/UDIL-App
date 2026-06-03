using System;
using System.Linq;
using System.Web.SessionState;

namespace UDIL.Shared
{
    public sealed class TrackerTickResult
    {
        public bool SkippedPoll { get; set; }
        public bool ShouldStopTracker { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsCompleted { get; set; }
        public int MaxLevel { get; set; }
        public string CancelReason { get; set; }
        public bool StageTimedOut { get; set; }
        public int TimedOutStage { get; set; }
        public int NextStageAfterTimeout { get; set; }
        public string TimeoutMessage { get; set; }
        public TransactionStatusResponse StatusResponse { get; set; }
    }

    /// <summary>
    /// Server-side transaction tracker logic shared across command/read/cancellation pages.
    /// Throttles HTTP polling to reduce postback blocking.
    /// </summary>
    public static class TransactionTrackerService
    {
        public static TrackerTickResult ProcessTick(HttpSessionState session, string transactionId, string privateKey)
        {
            var result = new TrackerTickResult();

            if (string.IsNullOrEmpty(transactionId) || string.IsNullOrEmpty(privateKey))
            {
                result.ShouldStopTracker = true;
                return result;
            }

            DateTime? lastPoll = session[UdilConstants.SessionLastTrackerPollUtc] as DateTime?;
            if (lastPoll.HasValue)
            {
                double elapsedMs = (DateTime.UtcNow - lastPoll.Value).TotalMilliseconds;
                if (elapsedMs < UdilConstants.TrackerMinPollIntervalMs)
                {
                    result.SkippedPoll = true;
                    result.MaxLevel = session[UdilConstants.SessionCurrentStage] != null
                        ? Convert.ToInt32(session[UdilConstants.SessionCurrentStage])
                        : 0;
                    return result;
                }
            }

            TransactionStatusResponse statusResponse = AppCommon.GetTransactionStatus(transactionId, privateKey);
            session[UdilConstants.SessionLastTrackerPollUtc] = DateTime.UtcNow;
            result.StatusResponse = statusResponse;

            if (statusResponse?.data == null)
            {
                return result;
            }

            int maxLevel = 0;
            foreach (var item in statusResponse.data)
            {
                if (int.TryParse(item.status_level, out int lvl) && lvl > maxLevel)
                {
                    maxLevel = lvl;
                }

                if (IsCancelled(item))
                {
                    result.IsCancelled = true;
                    result.ShouldStopTracker = true;
                    result.MaxLevel = -1;
                    result.CancelReason = item.request_cancelled == UdilConstants.UserCancelRequestFlag
                        ? "Request was cancelled by the user (request_cancelled: 1)."
                        : "Command was cancelled by the meter (indv_status: 2, status_level: 4).";
                    ClearStageSession(session);
                    return result;
                }
            }

            int currentStage = session[UdilConstants.SessionCurrentStage] != null
                ? Convert.ToInt32(session[UdilConstants.SessionCurrentStage])
                : 0;
            DateTime stageStartTime = session[UdilConstants.SessionStageStartTime] != null
                ? (DateTime)session[UdilConstants.SessionStageStartTime]
                : DateTime.Now;

            if (maxLevel != currentStage)
            {
                session[UdilConstants.SessionCurrentStage] = maxLevel;
                session[UdilConstants.SessionStageStartTime] = DateTime.Now;
            }
            else
            {
                TimeSpan timeInStage = DateTime.Now - stageStartTime;
                if (timeInStage.TotalMinutes >= UdilConstants.StageTimeoutMinutes)
                {
                    int nextStage = currentStage + 1;
                    if (nextStage <= UdilConstants.MaxTrackerStage)
                    {
                        result.StageTimedOut = true;
                        result.TimedOutStage = currentStage;
                        result.NextStageAfterTimeout = nextStage;
                        result.TimeoutMessage =
                            $"Stage {currentStage} failed due to timeout ({UdilConstants.StageTimeoutMinutes} minutes). Moving to stage {nextStage}.";
                        session[UdilConstants.SessionCurrentStage] = nextStage;
                        session[UdilConstants.SessionStageStartTime] = DateTime.Now;
                        maxLevel = nextStage;
                    }
                }
            }

            result.MaxLevel = maxLevel;

            if (maxLevel >= UdilConstants.FinalTrackerStage)
            {
                result.IsCompleted = true;
                result.ShouldStopTracker = true;
                ClearStageSession(session);
            }

            return result;
        }

        public static bool IsCancelled(TransactionStatusData item)
        {
            return (item.indv_status == UdilConstants.MeterCancelIndvStatus &&
                    item.status_level == UdilConstants.MeterCancelStatusLevel) ||
                   item.request_cancelled == UdilConstants.UserCancelRequestFlag;
        }

        public static bool IsApiSuccess(string status)
        {
            return status == UdilConstants.ApiStatusSuccess;
        }

        public static void ResetTrackerSession(HttpSessionState session)
        {
            session[UdilConstants.SessionLastTrackerPollUtc] = null;
            ClearStageSession(session);
        }

        private static void ClearStageSession(HttpSessionState session)
        {
            session[UdilConstants.SessionCurrentStage] = null;
            session[UdilConstants.SessionStageStartTime] = null;
        }
    }
}
