using System;
using System.Globalization;
using System.Web.UI.WebControls;

namespace UDIL.Shared
{
    /// <summary>
    /// Centralized DateTime parsing, formatting, and validation for Web Forms pages.
    /// </summary>
    public static class DateTimeHelper
    {
        public static string FormatApiDateTime(DateTime value)
        {
            return value.ToString(UdilConstants.ApiDateTimeFormat, CultureInfo.InvariantCulture);
        }

        public static string FormatApiTime(TimeSpan value)
        {
            return value.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
        }

        public static string FormatApiMonthDay(DateTime value)
        {
            return value.ToString(UdilConstants.ApiMonthDayFormat, CultureInfo.InvariantCulture);
        }

        public static string NormalizeTimeWithSeconds(string timeValue)
        {
            if (string.IsNullOrWhiteSpace(timeValue))
            {
                return timeValue;
            }

            timeValue = timeValue.Trim();
            string[] parts = timeValue.Split(':');
            if (parts.Length == 2)
            {
                string hour = parts[0].PadLeft(2, '0');
                string minute = parts[1].PadLeft(2, '0');
                return $"{hour}:{minute}:00";
            }

            if (parts.Length == 3)
            {
                return $"{parts[0].PadLeft(2, '0')}:{parts[1].PadLeft(2, '0')}:{parts[2].PadLeft(2, '0')}";
            }

            return timeValue;
        }

        public static bool TryParseApiDateTime(string input, out DateTime result, out string errorMessage)
        {
            result = DateTime.MinValue;
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                errorMessage = "Date and time are required.";
                return false;
            }

            if (DateTime.TryParseExact(
                input.Trim(),
                UdilConstants.ApiDateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result))
            {
                return true;
            }

            if (DateTime.TryParse(input.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return true;
            }

            errorMessage = $"Invalid date/time. Use format {UdilConstants.ApiDateTimeFormat}.";
            return false;
        }

        public static bool TryParseApiTime(string input, out string normalizedTime, out string errorMessage)
        {
            normalizedTime = null;
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                errorMessage = "Time is required.";
                return false;
            }

            normalizedTime = NormalizeTimeWithSeconds(input.Trim());

            if (TimeSpan.TryParseExact(normalizedTime, @"hh\:mm\:ss", CultureInfo.InvariantCulture, out _))
            {
                return true;
            }

            errorMessage = $"Invalid time. Use format {UdilConstants.ApiTimeFormat}.";
            return false;
        }

        public static void PopulateHourMinuteSecondDropDowns(DropDownList hour, DropDownList minute, DropDownList second, DateTime? selected = null)
        {
            if (hour.Items.Count == 0)
            {
                for (int h = 0; h < 24; h++)
                {
                    hour.Items.Add(new ListItem(h.ToString("00"), h.ToString("00")));
                }
            }

            if (minute.Items.Count == 0)
            {
                for (int m = 0; m < 60; m++)
                {
                    minute.Items.Add(new ListItem(m.ToString("00"), m.ToString("00")));
                }
            }

            if (second.Items.Count == 0)
            {
                for (int s = 0; s < 60; s++)
                {
                    second.Items.Add(new ListItem(s.ToString("00"), s.ToString("00")));
                }
            }

            if (selected.HasValue)
            {
                hour.SelectedValue = selected.Value.Hour.ToString("00");
                minute.SelectedValue = selected.Value.Minute.ToString("00");
                second.SelectedValue = selected.Value.Second.ToString("00");
            }
        }

        public static DateTime CombineCalendarAndTime(System.Web.UI.WebControls.Calendar calendar, DropDownList hour, DropDownList minute, DropDownList second)
        {
            DateTime date = calendar.SelectedDate == DateTime.MinValue ? DateTime.Today : calendar.SelectedDate;
            int h = int.Parse(hour.SelectedValue);
            int m = int.Parse(minute.SelectedValue);
            int s = int.Parse(second.SelectedValue);
            return new DateTime(date.Year, date.Month, date.Day, h, m, s, DateTimeKind.Local);
        }
    }
}
