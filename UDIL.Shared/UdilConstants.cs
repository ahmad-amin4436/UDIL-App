using System;
using System.Configuration;

namespace UDIL.Shared
{
    /// <summary>
    /// Centralized application constants. Values can be overridden via Web.config appSettings (Udil:* keys).
    /// </summary>
    public static class UdilConstants
    {
        public const string ApiStatusSuccess = "1";
        public const string MeterCancelIndvStatus = "2";
        public const string MeterCancelStatusLevel = "4";
        public const string UserCancelRequestFlag = "1";

        public const int FinalTrackerStage = 5;
        public const int MaxTrackerStage = 8;

        public const string TransactionIdPrefix = "TXN";
        public const string ApiDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public const string ApiTimeFormat = "HH:mm:ss";
        public const string ApiMonthDayFormat = "dd-MM";

        public static int TrackerTimerIntervalMs => GetInt("Udil:TrackerTimerIntervalMs", 3000);
        public static int TablesTimerIntervalMs => GetInt("Udil:TablesTimerIntervalMs", 5000);
        public static int TrackerMinPollIntervalMs => GetInt("Udil:TrackerMinPollIntervalMs", 2500);
        public static int StageTimeoutMinutes => GetInt("Udil:StageTimeoutMinutes", 5);
        public static int StatusPollMaxAttempts => GetInt("Udil:StatusPollMaxAttempts", 10);
        public static int StatusPollSleepMs => GetInt("Udil:StatusPollSleepMs", 2000);
        public static int StatusHttpMaxRetries => GetInt("Udil:StatusHttpMaxRetries", 5);

        public const string SessionCurrentTransactionId = "CurrentTransactionId";
        public const string SessionCurrentPrivateKey = "CurrentPrivateKey";
        public const string SessionCurrentStage = "CurrentStage";
        public const string SessionStageStartTime = "StageStartTime";
        public const string SessionLastTrackerPollUtc = "LastTrackerPollUtc";
        public const string SessionLastTablesFingerprint = "LastTablesFingerprint";
        public const string SessionLastTablesRefreshUtc = "LastTablesRefreshUtc";

        private static int GetInt(string key, int defaultValue)
        {
            string raw = System.Configuration.ConfigurationManager.AppSettings[key];
            return int.TryParse(raw, out int value) ? value : defaultValue;
        }
    }
}
