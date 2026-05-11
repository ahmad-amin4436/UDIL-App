using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDIL.DAL
{
    public class Tables
    {
        private string connectionString;
        public Tables()
        {
            // Default constructor - use Web.config connection string
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
        }

        public Tables(string connectionString)
        {
            // Constructor with custom connection string
            this.connectionString = connectionString;
        }
        public DataSet GetUDILTables(string globalDeviceId)
        {
            DataSet ds = new DataSet();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT *
                        FROM meter_visuals
                        WHERE global_device_id = @global_device_id;

                        SELECT *
                        FROM device_communication_history
                        WHERE global_device_id = @global_device_id
                        ORDER BY meter_datetime DESC
                        LIMIT 2;

                        SELECT *
                        FROM events
                        WHERE global_device_id = @global_device_id
                        ORDER BY event_datetime DESC
                        LIMIT 5;
                    ";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@global_device_id", globalDeviceId);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }
                    }
                }

                // Optional: Rename tables for easier access
                if (ds.Tables.Count > 0) ds.Tables[0].TableName = "MeterVisuals";
                if (ds.Tables.Count > 1) ds.Tables[1].TableName = "CommunicationHistory";
                if (ds.Tables.Count > 2) ds.Tables[2].TableName = "Events";

                return ds;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching UDIL tables: {ex.Message}");
                return new DataSet();
            }
        }

        public DataSet GetLoadProfileData(string searchTerm = null)
        {
            DataSet ds = new DataSet();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM load_profile_data";

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query += @" WHERE 
                            `id` LIKE @searchTerm OR
                            `msn` LIKE @searchTerm OR
                            `global_device_id` LIKE @searchTerm OR
                            `meter_datetime` LIKE @searchTerm OR
                            `interval` LIKE @searchTerm OR
                            `channel_id` LIKE @searchTerm OR
                            `frequency` LIKE @searchTerm OR
                            `current_phase_a` LIKE @searchTerm OR
                            `current_phase_b` LIKE @searchTerm OR
                            `current_phase_c` LIKE @searchTerm OR
                            `voltage_phase_a` LIKE @searchTerm OR
                            `voltage_phase_b` LIKE @searchTerm OR
                            `voltage_phase_c` LIKE @searchTerm OR
                            `average_pf` LIKE @searchTerm OR
                            `mdc_read_datetime` LIKE @searchTerm OR
                            `db_datetime` LIKE @searchTerm OR
                            `is_synced` LIKE @searchTerm";
                    }

                    query += " LIMIT 1000;";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            cmd.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                        }

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }
                    }
                }

                if (ds.Tables.Count > 0) ds.Tables[0].TableName = "LoadProfileData";

                return ds;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching load profile data: {ex.Message}");
                return new DataSet();
            }
        }

        public DataSet GetInstantaneousData(string searchTerm = null)
        {
            DataSet ds = new DataSet();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM instantaneous_data";

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query += @" WHERE 
                            `id` LIKE @searchTerm OR
                            `msn` LIKE @searchTerm OR
                            `global_device_id` LIKE @searchTerm OR
                            `signal_strength` LIKE @searchTerm OR
                            `frequency` LIKE @searchTerm OR
                            `current_phase_a` LIKE @searchTerm OR
                            `meter_datetime` LIKE @searchTerm OR
                            `current_tariff_register` LIKE @searchTerm OR
                            `current_phase_b` LIKE @searchTerm OR
                            `current_phase_c` LIKE @searchTerm OR
                            `voltage_phase_a` LIKE @searchTerm OR
                            `voltage_phase_b` LIKE @searchTerm OR
                            `voltage_phase_c` LIKE @searchTerm OR
                            `aggregate_active_pwr_pos` LIKE @searchTerm OR
                            `aggregate_active_pwr_neg` LIKE @searchTerm OR
                            `aggregate_active_pwr_abs` LIKE @searchTerm OR
                            `aggregate_reactive_pwr_pos` LIKE @searchTerm OR
                            `aggregate_reactive_pwr_neg` LIKE @searchTerm OR
                            `aggregate_reactive_pwr_abs` LIKE @searchTerm OR
                            `average_pf` LIKE @searchTerm OR
                            `mdc_read_datetime` LIKE @searchTerm OR
                            `db_datetime` LIKE @searchTerm OR
                            `is_synced` LIKE @searchTerm";
                    }

                    query += " LIMIT 1000;";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            cmd.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                        }

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }
                    }
                }

                if (ds.Tables.Count > 0) ds.Tables[0].TableName = "InstantaneousData";

                return ds;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching instantaneous data: {ex.Message}");
                return new DataSet();
            }
        }

        public DataSet GetBillingData(string searchTerm = null)
        {
            DataSet ds = new DataSet();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM billing_data";

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query += @" WHERE 
                            `id` LIKE @searchTerm OR
                            `msn` LIKE @searchTerm OR
                            `global_device_id` LIKE @searchTerm OR
                            `meter_datetime` LIKE @searchTerm OR
                            `active_energy_pos_t1` LIKE @searchTerm OR
                            `active_energy_pos_t2` LIKE @searchTerm OR
                            `active_energy_pos_t3` LIKE @searchTerm OR
                            `active_energy_pos_t4` LIKE @searchTerm OR
                            `active_energy_pos_tl` LIKE @searchTerm OR
                            `active_energy_neg_t1` LIKE @searchTerm OR
                            `active_energy_neg_t2` LIKE @searchTerm OR
                            `active_energy_neg_t3` LIKE @searchTerm OR
                            `active_energy_neg_t4` LIKE @searchTerm OR
                            `active_energy_neg_tl` LIKE @searchTerm OR
                            `active_energy_abs_t1` LIKE @searchTerm OR
                            `active_energy_abs_t2` LIKE @searchTerm OR
                            `active_energy_abs_t3` LIKE @searchTerm OR
                            `active_energy_abs_t4` LIKE @searchTerm OR
                            `active_energy_abs_tl` LIKE @searchTerm OR
                            `reactive_energy_pos_t1` LIKE @searchTerm OR
                            `reactive_energy_pos_t2` LIKE @searchTerm OR
                            `reactive_energy_pos_t3` LIKE @searchTerm OR
                            `reactive_energy_pos_t4` LIKE @searchTerm OR
                            `reactive_energy_pos_tl` LIKE @searchTerm OR
                            `reactive_energy_neg_t1` LIKE @searchTerm OR
                            `reactive_energy_neg_t2` LIKE @searchTerm OR
                            `reactive_energy_neg_t3` LIKE @searchTerm OR
                            `reactive_energy_neg_t4` LIKE @searchTerm OR
                            `reactive_energy_neg_tl` LIKE @searchTerm OR
                            `reactive_energy_abs_t1` LIKE @searchTerm OR
                            `reactive_energy_abs_t2` LIKE @searchTerm OR
                            `reactive_energy_abs_t3` LIKE @searchTerm OR
                            `reactive_energy_abs_t4` LIKE @searchTerm OR
                            `reactive_energy_abs_tl` LIKE @searchTerm OR
                            `active_mdi_pos_t1` LIKE @searchTerm OR
                            `active_mdi_pos_t2` LIKE @searchTerm OR
                            `active_mdi_pos_t3` LIKE @searchTerm OR
                            `active_mdi_pos_t4` LIKE @searchTerm OR
                            `active_mdi_pos_tl` LIKE @searchTerm OR
                            `active_mdi_neg_t1` LIKE @searchTerm OR
                            `active_mdi_neg_t2` LIKE @searchTerm OR
                            `active_mdi_neg_t3` LIKE @searchTerm OR
                            `active_mdi_neg_t4` LIKE @searchTerm OR
                            `active_mdi_neg_tl` LIKE @searchTerm OR
                            `active_mdi_abs_t1` LIKE @searchTerm OR
                            `active_mdi_abs_t2` LIKE @searchTerm OR
                            `active_mdi_abs_t3` LIKE @searchTerm OR
                            `active_mdi_abs_t4` LIKE @searchTerm OR
                            `active_mdi_abs_tl` LIKE @searchTerm OR
                            `cumulative_mdi_pos_t1` LIKE @searchTerm OR
                            `cumulative_mdi_pos_t2` LIKE @searchTerm OR
                            `cumulative_mdi_pos_t3` LIKE @searchTerm OR
                            `cumulative_mdi_pos_t4` LIKE @searchTerm OR
                            `cumulative_mdi_pos_tl` LIKE @searchTerm OR
                            `cumulative_mdi_neg_t1` LIKE @searchTerm OR
                            `cumulative_mdi_neg_t2` LIKE @searchTerm OR
                            `cumulative_mdi_neg_t3` LIKE @searchTerm OR
                            `cumulative_mdi_neg_t4` LIKE @searchTerm OR
                            `cumulative_mdi_neg_tl` LIKE @searchTerm OR
                            `cumulative_mdi_abs_t1` LIKE @searchTerm OR
                            `cumulative_mdi_abs_t2` LIKE @searchTerm OR
                            `cumulative_mdi_abs_t3` LIKE @searchTerm OR
                            `cumulative_mdi_abs_t4` LIKE @searchTerm OR
                            `cumulative_mdi_abs_tl` LIKE @searchTerm OR
                            `mdc_read_datetime` LIKE @searchTerm OR
                            `db_datetime` LIKE @searchTerm OR
                            `is_synced` LIKE @searchTerm";
                    }

                    query += " LIMIT 1000;";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            cmd.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                        }

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }
                    }
                }

                if (ds.Tables.Count > 0) ds.Tables[0].TableName = "BillingData";

                return ds;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching billing data: {ex.Message}");
                return new DataSet();
            }
        }

        public DataSet GetMonthlyBillingData(string searchTerm = null)
        {
            DataSet ds = new DataSet();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM monthly_billing_data";

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query += @" WHERE 
                            `id` LIKE @searchTerm OR
                            `msn` LIKE @searchTerm OR
                            `global_device_id` LIKE @searchTerm OR
                            `meter_datetime` LIKE @searchTerm OR
                            `active_energy_pos_t1` LIKE @searchTerm OR
                            `active_energy_pos_t2` LIKE @searchTerm OR
                            `active_energy_pos_t3` LIKE @searchTerm OR
                            `active_energy_pos_t4` LIKE @searchTerm OR
                            `active_energy_pos_tl` LIKE @searchTerm OR
                            `active_energy_neg_t1` LIKE @searchTerm OR
                            `active_energy_neg_t2` LIKE @searchTerm OR
                            `active_energy_neg_t3` LIKE @searchTerm OR
                            `active_energy_neg_t4` LIKE @searchTerm OR
                            `active_energy_neg_tl` LIKE @searchTerm OR
                            `active_energy_abs_t1` LIKE @searchTerm OR
                            `active_energy_abs_t2` LIKE @searchTerm OR
                            `active_energy_abs_t3` LIKE @searchTerm OR
                            `active_energy_abs_t4` LIKE @searchTerm OR
                            `active_energy_abs_tl` LIKE @searchTerm OR
                            `reactive_energy_pos_t1` LIKE @searchTerm OR
                            `reactive_energy_pos_t2` LIKE @searchTerm OR
                            `reactive_energy_pos_t3` LIKE @searchTerm OR
                            `reactive_energy_pos_t4` LIKE @searchTerm OR
                            `reactive_energy_pos_tl` LIKE @searchTerm OR
                            `reactive_energy_neg_t1` LIKE @searchTerm OR
                            `reactive_energy_neg_t2` LIKE @searchTerm OR
                            `reactive_energy_neg_t3` LIKE @searchTerm OR
                            `reactive_energy_neg_t4` LIKE @searchTerm OR
                            `reactive_energy_neg_tl` LIKE @searchTerm OR
                            `reactive_energy_abs_t1` LIKE @searchTerm OR
                            `reactive_energy_abs_t2` LIKE @searchTerm OR
                            `reactive_energy_abs_t3` LIKE @searchTerm OR
                            `reactive_energy_abs_t4` LIKE @searchTerm OR
                            `reactive_energy_abs_tl` LIKE @searchTerm OR
                            `active_mdi_pos_t1` LIKE @searchTerm OR
                            `active_mdi_pos_t2` LIKE @searchTerm OR
                            `active_mdi_pos_t3` LIKE @searchTerm OR
                            `active_mdi_pos_t4` LIKE @searchTerm OR
                            `active_mdi_pos_tl` LIKE @searchTerm OR
                            `active_mdi_neg_t1` LIKE @searchTerm OR
                            `active_mdi_neg_t2` LIKE @searchTerm OR
                            `active_mdi_neg_t3` LIKE @searchTerm OR
                            `active_mdi_neg_t4` LIKE @searchTerm OR
                            `active_mdi_neg_tl` LIKE @searchTerm OR
                            `active_mdi_abs_t1` LIKE @searchTerm OR
                            `active_mdi_abs_t2` LIKE @searchTerm OR
                            `active_mdi_abs_t3` LIKE @searchTerm OR
                            `active_mdi_abs_t4` LIKE @searchTerm OR
                            `active_mdi_abs_tl` LIKE @searchTerm OR
                            `cumulative_mdi_pos_t1` LIKE @searchTerm OR
                            `cumulative_mdi_pos_t2` LIKE @searchTerm OR
                            `cumulative_mdi_pos_t3` LIKE @searchTerm OR
                            `cumulative_mdi_pos_t4` LIKE @searchTerm OR
                            `cumulative_mdi_pos_tl` LIKE @searchTerm OR
                            `cumulative_mdi_neg_t1` LIKE @searchTerm OR
                            `cumulative_mdi_neg_t2` LIKE @searchTerm OR
                            `cumulative_mdi_neg_t3` LIKE @searchTerm OR
                            `cumulative_mdi_neg_t4` LIKE @searchTerm OR
                            `cumulative_mdi_neg_tl` LIKE @searchTerm OR
                            `cumulative_mdi_abs_t1` LIKE @searchTerm OR
                            `cumulative_mdi_abs_t2` LIKE @searchTerm OR
                            `cumulative_mdi_abs_t3` LIKE @searchTerm OR
                            `cumulative_mdi_abs_t4` LIKE @searchTerm OR
                            `cumulative_mdi_abs_tl` LIKE @searchTerm OR
                            `longitude` LIKE @searchTerm OR
                            `latitude` LIKE @searchTerm OR
                            `picture_1` LIKE @searchTerm OR
                            `picture_2` LIKE @searchTerm OR
                            `reading_mode` LIKE @searchTerm OR
                            `mdi_reset_datetime` LIKE @searchTerm OR
                            `reset_count` LIKE @searchTerm OR
                            `mdc_read_datetime` LIKE @searchTerm OR
                            `db_datetime` LIKE @searchTerm OR
                            `is_synced` LIKE @searchTerm";
                    }

                    query += " LIMIT 1000;";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            cmd.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                        }

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }
                    }
                }

                if (ds.Tables.Count > 0) ds.Tables[0].TableName = "MonthlyBillingData";

                return ds;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching monthly billing data: {ex.Message}");
                return new DataSet();
            }
        }
    }
}
