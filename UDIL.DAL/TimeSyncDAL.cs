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
    public class TimeSyncDAL
    {
        private string connectionString;
        public TimeSyncDAL()
        {
            // Default constructor - use Web.config connection string
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
        }

        public TimeSyncDAL(string connectionString)
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
                        SELECT 
                            msn, global_device_id, last_command, last_command_datetime, 
                            last_command_resp, dvtm_datetime, dvtm_meter_clock
                        FROM meter_visuals
                        WHERE global_device_id = @global_device_id;

                        SELECT 
                            message_log
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
    }
}
