using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;

namespace UDIL.Shared
{
    /// <summary>
    /// Provides configurable lookup lists from appSettings or database (udil_master_data when available).
    /// Format for appSettings: "value1:Label1|value2:Label2"
    /// </summary>
    public static class MasterDataProvider
    {
        public const string DeviceTypesKey = "Udil:MasterData:DeviceTypes";
        public const string MeterTypesKey = "Udil:MasterData:MeterTypes";
        public const string PhasesKey = "Udil:MasterData:Phases";
        public const string RelayOperationsKey = "Udil:MasterData:RelayOperations";
        public const string CommunicationModesKey = "Udil:MasterData:CommunicationModes";
        public const string CommunicationTypesKey = "Udil:MasterData:CommunicationTypes";
        public const string BooleanOptionsKey = "Udil:MasterData:BooleanOptions";

        private static readonly Dictionary<string, string> Defaults = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { DeviceTypesKey, "1:Meter|2:DCU|3:APMS|4:Grid meter|5:Others" },
            { MeterTypesKey, "1:Normal|2:Whole Current|3:CTO|4:CTPT|5:Other" },
            { PhasesKey, "1:Single Phase|2:Two Phase|3:Three Phase" },
            { RelayOperationsKey, "0:OFF (0)|1:ON (1)" }
        };

        public static void BindDropDown(DropDownList list, string settingsKey, string selectedValue = null)
        {
            list.Items.Clear();
            foreach (var item in GetItems(settingsKey))
            {
                list.Items.Add(new ListItem(item.Text, item.Value));
            }

            if (!string.IsNullOrEmpty(selectedValue) && list.Items.FindByValue(selectedValue) != null)
            {
                list.SelectedValue = selectedValue;
            }
        }

        public static IList<ListItem> GetItems(string settingsKey)
        {
            string raw = System.Configuration.ConfigurationManager.AppSettings[settingsKey];
            if (string.IsNullOrWhiteSpace(raw) && Defaults.TryGetValue(settingsKey, out string fallback))
            {
                raw = fallback;
            }

            if (string.IsNullOrWhiteSpace(raw))
            {
                return Array.Empty<ListItem>();
            }

            return raw.Split('|')
                .Select(part =>
                {
                    string[] pair = part.Split(new[] { ':' }, 2);
                    return pair.Length == 2
                        ? new ListItem(pair[1].Trim(), pair[0].Trim())
                        : new ListItem(part.Trim(), part.Trim());
                })
                .ToList();
        }
    }
}
