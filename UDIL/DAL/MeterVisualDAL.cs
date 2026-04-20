using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Collections.Generic;

namespace UDIL.DAL
{
    public class MeterVisualDAL
    {
        private string connectionString;

        public MeterVisualDAL()
        {
            // Default constructor - use Web.config connection string
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
        }

        public MeterVisualDAL(string connectionString)
        {
            // Constructor with custom connection string
            this.connectionString = connectionString;
        }

        public bool ValidateMeterVisuals(string globalDeviceId, string deviceType, string mdiResetDate, string mdiResetTime, 
                                       string bidirectionalDevice, string simNumber, string simId, string phase, 
                                       string meterType, string communicationMode, string communicationType, 
                                       string communicationInterval, string initialCommunicationTime)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // Check if meter exists in meter_visuals table
                    string checkExistsQuery = "SELECT COUNT(*) FROM meter_visuals WHERE global_device_id = @global_device_id";
                    
                    using (MySqlCommand checkCommand = new MySqlCommand(checkExistsQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@global_device_id", globalDeviceId);
                        
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                        
                        if (count == 0)
                        {
                            return false; // Meter not found
                        }
                    }
                    
                    // Validate meter parameters against meter_visuals table using inline SQL
                    string validationQuery = @"
                        SELECT 
                            CASE 
                                WHEN dmdt_meter_type IS NOT NULL AND dmdt_meter_type != @meter_type THEN 0
                                WHEN dmdt_bidirectional_device IS NOT NULL AND dmdt_bidirectional_device != @bidirectional_device THEN 0
                                WHEN dmdt_communication_mode IS NOT NULL AND dmdt_communication_mode != @communication_mode THEN 0
                                WHEN dmdt_communication_type IS NOT NULL AND dmdt_communication_type != @communication_type THEN 0
                                WHEN dmdt_communication_interval IS NOT NULL AND dmdt_communication_interval != @communication_interval THEN 0
                                WHEN dmdt_initial_communication_time IS NOT NULL AND dmdt_initial_communication_time != @initial_communication_time THEN 0
                                WHEN dmdt_phase IS NOT NULL AND dmdt_phase != @phase THEN 0
                                WHEN mdi_reset_date IS NOT NULL AND mdi_reset_date != @mdi_reset_date THEN 0
                                WHEN mdi_reset_time IS NOT NULL AND mdi_reset_time != @mdi_reset_time THEN 0
                                WHEN msim_id IS NOT NULL AND msim_id != @simId THEN 0
                                ELSE 1
                            END AS validation_result
                        FROM meter_visuals 
                        WHERE global_device_id = @global_device_id";
                    
                    using (MySqlCommand validationCommand = new MySqlCommand(validationQuery, connection))
                    {
                        // Add parameters
                        validationCommand.Parameters.AddWithValue("@global_device_id", globalDeviceId);
                        validationCommand.Parameters.AddWithValue("@meter_type", meterType);
                        validationCommand.Parameters.AddWithValue("@bidirectional_device", bidirectionalDevice);
                        validationCommand.Parameters.AddWithValue("@communication_mode", communicationMode);
                        validationCommand.Parameters.AddWithValue("@communication_type", communicationType);
                        validationCommand.Parameters.AddWithValue("@communication_interval", communicationInterval);
                        validationCommand.Parameters.AddWithValue("@initial_communication_time", initialCommunicationTime);
                        validationCommand.Parameters.AddWithValue("@phase", phase);
                        validationCommand.Parameters.AddWithValue("@mdi_reset_date", mdiResetDate);
                        validationCommand.Parameters.AddWithValue("@mdi_reset_time", mdiResetTime);
                        validationCommand.Parameters.AddWithValue("@simId", simId);

                        // First validate device communication history
                        if (!ValidateDeviceCommunicationHistory(globalDeviceId))
                        {
                            return false; // Device communication history validation failed
                        }
                        
                        // Then validate meter visual data
                        int result = Convert.ToInt32(validationCommand.ExecuteScalar());
                        return result == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                System.Diagnostics.Debug.WriteLine($"Error validating meter visuals: {ex.Message}");
                return false;
            }
        }

        public bool ValidateDeviceCommunicationHistory(string globalDeviceId)
        {
            try
            {
                // Get device creation request time from session
                DateTime deviceCreationRequestTime = GetDeviceCreationRequestTime();
                
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    string query = @"
                        SELECT message_log 
                        FROM device_communication_history 
                        WHERE global_device_id = @global_device_id 
                        ORDER BY meter_datetime DESC 
                        LIMIT 1";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@global_device_id", globalDeviceId);
                        
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            bool hasValidDMDT = false;
                            
                            while (reader.Read())
                            {
                                string messageLog = reader["message_log"]?.ToString() ?? string.Empty;
                                
                                // Split the message_log into individual log entries
                                string[] logEntries = messageLog.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                                
                                // Parse all entries with their datetimes
                                List<(DateTime time, string entry)> parsedEntries = new List<(DateTime, string)>();
                                
                                foreach (string entry in logEntries)
                                {
                                    DateTime logDateTime = ExtractDateTimeFromMessageLog(entry);
                                    
                                    // Skip entries that couldn't parse datetime
                                    if (logDateTime != DateTime.MinValue)
                                    {
                                        parsedEntries.Add((logDateTime, entry));
                                    }
                                }
                                
                                // Sort entries by datetime to maintain order
                                parsedEntries.Sort((x, y) => x.time.CompareTo(y.time));
                                
                                // Find DMDT commands with their subsequent OK responses
                                for (int i = 0; i < parsedEntries.Count; i++)
                                {
                                    var currentEntry = parsedEntries[i];
                                    
                                    // Check if this is a DMDT command
                                    if (currentEntry.entry.Contains("DMDT?"))
                                    {
                                        // Look for OK, 1 response in subsequent entries (within reasonable time range)
                                        for (int j = i + 1; j < parsedEntries.Count && j < i + 5; j++) // Check next few entries
                                        {
                                            var nextEntry = parsedEntries[j];
                                            
                                            // Check if this is the OK response for the DMDT command
                                            if (nextEntry.entry.Contains("OK, 1"))
                                            {
                                                // Use the OK response time for validation
                                                DateTime okResponseTime = nextEntry.time;
                                                DateTime okDateWithoutSeconds = new DateTime(okResponseTime.Year, okResponseTime.Month, okResponseTime.Day, okResponseTime.Hour, okResponseTime.Minute, 0);
                                                DateTime requestDateWithoutSeconds = new DateTime(deviceCreationRequestTime.Year, deviceCreationRequestTime.Month, deviceCreationRequestTime.Day, deviceCreationRequestTime.Hour, deviceCreationRequestTime.Minute, 0);
                                                
                                                if (okDateWithoutSeconds >= requestDateWithoutSeconds)
                                                {
                                                    hasValidDMDT = true;
                                                    break; // Found valid DMDT, no need to check further
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            
                            return hasValidDMDT;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating device communication history: {ex.Message}");
                return false;
            }
        }

        private DateTime GetDeviceCreationRequestTime()
        {
            try
            {
                if (System.Web.HttpContext.Current?.Session != null)
                {
                    var sessionTime = System.Web.HttpContext.Current.Session["DeviceCreationRequestTime"];
                    if (sessionTime != null)
                    {
                        return Convert.ToDateTime(sessionTime);
                    }
                }
                return DateTime.MinValue;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public bool CheckMeterVisualExists(string globalDeviceId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM meter_visuals WHERE global_device_id = @global_device_id";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@global_device_id", globalDeviceId);
                        
                        connection.Open();
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking meter visual exists: {ex.Message}");
                return false;
            }
        }

        public DataTable GetMeterVisualData(string globalDeviceId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = @"
                        SELECT msn, global_device_id, last_command, last_command_datetime, 
                               last_command_resp, last_command_resp_datetime, meter_datetime, 
                               mdc_read_datetime, db_datetime, active_energy_pos_tl, 
                               active_energy_pos_tl_datetime, active_energy_neg_tl, 
                               active_energy_neg_tl_datetime, reactive_energy_pos_tl, 
                               reactive_energy_pos_tl_datetime, reactive_energy_neg_tl, 
                               reactive_energy_neg_tl_datetime, aggregate_active_energy_abs_tl, 
                               aggregate_active_energy_abs_tl_datetime, aggregate_active_pwr_pos, 
                               aggregate_active_pwr_pos_datetime, aggregate_active_pwr_neg, 
                               aggregate_active_pwr_neg_datetime, aggregate_reactive_pwr_pos, 
                               aggregate_reactive_pwr_pos_datetime, aggregate_reactive_pwr_neg, 
                               aggregate_reactive_pwr_neg_datetime, aggregate_reactive_energy_abs_tl, 
                               aggregate_reactive_energy_abs_tl_datetime, current_phase_a, 
                               current_phase_b, current_phase_c, voltage_phase_a, voltage_phase_b, 
                               voltage_phase_c, frequency, pf_phase_a, pf_phase_b, pf_phase_c, 
                               average_pf, last_contactor_on_datetime, last_contactor_off_datetime, 
                               last_communication_datetime, last_signal_strength, power_status, 
                               power_status_datetime, auxr_datetime, auxr_status, dvtm_datetime, 
                               dvtm_meter_clock, sanc_datetime, sanc_load_limit, sanc_maximum_retries, 
                               sanc_retry_interval, sanc_threshold_duration, sanc_retry_clear_interval, 
                               lsch_datetime, lsch_start_datetime, lsch_end_datetime, 
                               lsch_load_shedding_slabs, tiou_datetime, tiou_day_profile, 
                               tiou_week_profile, tiou_season_profile, tiou_holiday_profile, 
                               tiou_activation_datetime, ippo_datetime, ippo_primary_ip_address, 
                               ippo_secondary_ip_address, ippo_primary_port, ippo_secondary_port, 
                               mdsm_datetime, mdsm_activation_datetime, mdsm_data_type, 
                               mdsm_sampling_interval, mdsm_sampling_initial_time, oppo_datetime, 
                               oppo_optical_port_on_datetime, oppo_optical_port_off_datetime, 
                               wsim_datetime, wsim_wakeup_number_1, wsim_wakeup_number_2, 
                               wsim_wakeup_number_3, mtst_datetime, mtst_meter_activation_status, 
                               dmdt_datetime, dmdt_communication_mode, dmdt_bidirectional_device, 
                               dmdt_communication_type, dmdt_communication_interval, 
                               dmdt_initial_communication_time, dmdt_phase, dmdt_meter_type, 
                               mdi_reset_date, mdi_reset_time, msim_id
                        FROM meter_visuals 
                        WHERE global_device_id = @global_device_id";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@global_device_id", globalDeviceId);
                        
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting meter visual data: {ex.Message}");
                return null;
            }
        }

        public bool ValidateEventsTable(string globalDeviceId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // Check if events table exists and has records for the device
                    string query = @"
                        SELECT COUNT(*) 
                        FROM information_schema.tables 
                        WHERE table_schema = DATABASE() 
                        AND table_name = 'events'";
                    
                    using (MySqlCommand tableCheckCommand = new MySqlCommand(query, connection))
                    {
                        int tableExists = Convert.ToInt32(tableCheckCommand.ExecuteScalar());
                        
                        if (tableExists == 0)
                        {
                            return false; // Events table doesn't exist
                        }
                    }
                    
                    // Check if there are events for the specific device
                    string eventsQuery = @"
                        SELECT COUNT(*) 
                        FROM events 
                        WHERE global_device_id = @global_device_id 
                        LIMIT 1";
                    
                    using (MySqlCommand eventsCommand = new MySqlCommand(eventsQuery, connection))
                    {
                        eventsCommand.Parameters.AddWithValue("@global_device_id", globalDeviceId);
                        
                        int eventCount = Convert.ToInt32(eventsCommand.ExecuteScalar());
                        
                        return eventCount > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating events table: {ex.Message}");
                return false;
            }
        }

        private DateTime ExtractDateTimeFromMessageLog(string messageLog)
        {
            try
            {
                // Extract datetime from beginning of message log
                // Format examples:
                // "2026-04-11 10:01:06 OK, 22 upto 2026-04-11 10:00:00"
                // "4/11/2026 3:02:44 AM DMDT?"
                
                if (string.IsNullOrEmpty(messageLog))
                {
                    return DateTime.MinValue;
                }

                // Try to find datetime pattern at beginning
                string[] parts = messageLog.Split(new[] { ' ' }, 4);
                if (parts.Length < 3)
                {
                    return DateTime.MinValue;
                }

                // Try different datetime formats
                string[] formats = {
                    "yyyy-MM-dd HH:mm:ss",    // "2026-04-11 10:01:06"
                    "M/d/yyyy h:mm:ss tt",    // "4/11/2026 3:02:44 AM"
                    "MM/dd/yyyy h:mm:ss tt",  // "04/11/2026 3:02:44 AM"
                    "M/d/yyyy H:mm:ss",       // "4/11/2026 15:02:44"
                    "MM/dd/yyyy H:mm:ss"      // "04/11/2026 15:02:44"
                };

                // Try to parse first 3 parts as datetime
                string datePart = parts[0];
                string timePart = parts[1];
                string ampmPart = parts.Length > 2 ? parts[2] : "";
                
                string dateTimeStr = datePart + " " + timePart;
                if (!string.IsNullOrEmpty(ampmPart) && (ampmPart == "AM" || ampmPart == "PM"))
                {
                    dateTimeStr += " " + ampmPart;
                }

                foreach (string format in formats)
                {
                    if (DateTime.TryParseExact(dateTimeStr, format, null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        return result;
                    }
                }

                // Fallback: try to extract first 19 characters for standard format
                if (messageLog.Length >= 19)
                {
                    string fallback = messageLog.Substring(0, 19);
                    if (DateTime.TryParse(fallback, out DateTime fallbackResult))
                    {
                        return fallbackResult;
                    }
                }

                return DateTime.MinValue;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }
}
