using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
namespace UDIL.Shared
{
    [Serializable]
    public class TouDayProfileRow
    {
        public string DayName { get; set; }
        public string TariffTimesCsv { get; set; }
    }

    public static class TouProfileBuilder
    {
        public static bool TryBuildDayProfileJson(IList<TouDayProfileRow> rows, out string json, out string error)
        {
            json = null;
            error = null;
            var list = new List<object>();

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row.DayName))
                {
                    continue;
                }

                var slabs = new List<string>();
                if (!string.IsNullOrWhiteSpace(row.TariffTimesCsv))
                {
                    foreach (string part in row.TariffTimesCsv.Split(','))
                    {
                        string time = part.Trim();
                        if (string.IsNullOrEmpty(time))
                        {
                            continue;
                        }

                        string normalized;
                        if (!DateTimeHelper.TryParseApiTime(time, out normalized, out error))
                        {
                            return false;
                        }

                        slabs.Add(normalized);
                    }
                }

                list.Add(new { name = row.DayName.Trim(), tariff_slabs = slabs.ToArray() });
            }

            if (list.Count == 0)
            {
                error = "At least one day profile is required.";
                return false;
            }

            json = new JavaScriptSerializer().Serialize(list);
            return true;
        }

        public static string BuildWeekProfileJson(params (string name, string daysCsv)[] weeks)
        {
            var list = weeks
                .Where(w => !string.IsNullOrWhiteSpace(w.name))
                .Select(w => new
                {
                    name = w.name.Trim(),
                    weekly_day_profile = w.daysCsv.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).ToArray()
                });
            return new JavaScriptSerializer().Serialize(list);
        }

        public static string BuildSeasonProfileJson(params (string name, string week, string startDate)[] seasons)
        {
            var list = seasons
                .Where(s => !string.IsNullOrWhiteSpace(s.name))
                .Select(s => new { name = s.name.Trim(), week_profile_name = s.week.Trim(), start_date = s.startDate.Trim() });
            return new JavaScriptSerializer().Serialize(list);
        }

        public static string BuildHolidayProfileJson(params (string name, string date, string dayProfile)[] holidays)
        {
            var list = holidays
                .Where(h => !string.IsNullOrWhiteSpace(h.name))
                .Select(h => new { name = h.name.Trim(), date = h.date.Trim(), day_profile_name = h.dayProfile.Trim() });
            return new JavaScriptSerializer().Serialize(list);
        }
    }
}
