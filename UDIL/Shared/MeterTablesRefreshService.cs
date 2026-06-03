using System;
using System.Data;
using System.Text;
using System.Web.SessionState;
using UDIL.DAL;
using UDIL.Shared;

namespace UDIL.Shared.Web
{
    public sealed class MeterTablesRefreshResult
    {
        public DataSet DataSet { get; set; }
        public bool DataChanged { get; set; }
        public bool SkippedQuery { get; set; }
        public string Fingerprint { get; set; }
    }

    /// <summary>
    /// Loads validation tables with fingerprint-based change detection to avoid redundant GridView rebinds.
    /// </summary>
    public static class MeterTablesRefreshService
    {
        public static MeterTablesRefreshResult Refresh(HttpSessionState session, string connectionString, string globalDeviceId, bool forceRefresh = false)
        {
            var result = new MeterTablesRefreshResult();

            if (string.IsNullOrWhiteSpace(globalDeviceId))
            {
                result.SkippedQuery = true;
                return result;
            }

            DateTime? lastRefresh = session[UdilConstants.SessionLastTablesRefreshUtc] as DateTime?;
            if (!forceRefresh && lastRefresh.HasValue)
            {
                double elapsedMs = (DateTime.UtcNow - lastRefresh.Value).TotalMilliseconds;
                if (elapsedMs < UdilConstants.TablesTimerIntervalMs)
                {
                    result.SkippedQuery = true;
                    result.Fingerprint = session[UdilConstants.SessionLastTablesFingerprint] as string;
                    return result;
                }
            }

            Tables dal = new Tables(connectionString);
            DataSet ds = dal.GetUDILTables(globalDeviceId);
            string fingerprint = ComputeFingerprint(ds);

            string previous = session[UdilConstants.SessionLastTablesFingerprint] as string;
            result.DataSet = ds;
            result.Fingerprint = fingerprint;
            result.DataChanged = forceRefresh || !string.Equals(previous, fingerprint, StringComparison.Ordinal);

            session[UdilConstants.SessionLastTablesFingerprint] = fingerprint;
            session[UdilConstants.SessionLastTablesRefreshUtc] = DateTime.UtcNow;

            return result;
        }

        public static void ClearCache(HttpSessionState session)
        {
            session[UdilConstants.SessionLastTablesFingerprint] = null;
            session[UdilConstants.SessionLastTablesRefreshUtc] = null;
        }

        public static string ComputeFingerprint(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0)
            {
                return "empty";
            }

            var sb = new StringBuilder();
            foreach (DataTable table in ds.Tables)
            {
                sb.Append(table.TableName).Append(':').Append(table.Rows.Count);

                if (table.Rows.Count > 0)
                {
                    DataRow row = table.Rows[0];
                    foreach (DataColumn col in table.Columns)
                    {
                        if (col.ColumnName.IndexOf("datetime", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            col.ColumnName.IndexOf("_dt", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            sb.Append('|').Append(col.ColumnName).Append('=').Append(row[col]?.ToString());
                        }
                    }
                }

                sb.Append(';');
            }

            return sb.ToString();
        }
    }
}
