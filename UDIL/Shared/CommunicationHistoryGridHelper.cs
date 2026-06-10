using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace UDIL.Shared
{
    /// <summary>
    /// Sorts communication history rows (newest first) and formats collapsed/expandable message logs.
    /// </summary>
    public static class CommunicationHistoryGridHelper
    {
        public const int PreviewMaxLength = 120;

        private static readonly string[] SortColumnPriority = { "meter_datetime", "db_datetime", "id" };

        public static DataTable SortDescending(DataTable table)
        {
            if (table == null || table.Rows.Count <= 1)
            {
                return table;
            }

            string sortColumn = SortColumnPriority.FirstOrDefault(table.Columns.Contains);
            if (string.IsNullOrEmpty(sortColumn))
            {
                return table;
            }

            DataView view = table.DefaultView;
            view.Sort = sortColumn + " DESC";
            return view.ToTable();
        }

        public static void Bind(GridView grid, DataTable table)
        {
            if (grid == null)
            {
                return;
            }

            if (table != null && table.Rows.Count > 0)
            {
                grid.DataSource = SortDescending(table);
            }
            else
            {
                grid.DataSource = null;
            }

            grid.DataBind();
        }

        public static string GetPreview(object messageLog)
        {
            string text = messageLog?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(text))
            {
                return "(empty)";
            }
            string oneLine = text.Replace("\r\n", " ").Replace("\n", " ").Trim();
            if (oneLine.Length <= PreviewMaxLength)
            {
                return HttpUtility.HtmlEncode(oneLine);
            }

            return HttpUtility.HtmlEncode(oneLine.Substring(0, PreviewMaxLength)) + "…";
        }

        public static string GetFull(object messageLog)
            {
            string text = messageLog?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(text))
            {
                return "(empty)";
            }

            return HttpUtility.HtmlEncode(text);
        }
    }
}
