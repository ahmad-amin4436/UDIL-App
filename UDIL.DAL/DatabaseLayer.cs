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
    public class TestResult
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string TestName { get; set; }
        public string TestType { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public DateTime TestDate { get; set; }
        public string TransactionId { get; set; }
        public string GlobalDeviceId { get; set; }

        public TestResult()
        {
            // Default values
            TestDate = DateTime.Now;
            Status = "Pending";
        }
    }
    public class DatabaseLayer
    {
        private string connectionString;
        public DatabaseLayer()
        {
            // Default constructor - use Web.config connection string
            connectionString = ConfigurationManager.ConnectionStrings["TestSuitConnenction"]?.ConnectionString;
        }

        public DatabaseLayer(string connectionString)
        {
            // Constructor with custom connection string
            this.connectionString = connectionString;
        }
       
        // Authentication Methods
        public bool ValidateUser(string username, string password)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password AND is_active = 1";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password); // In production, use hashed passwords

                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error validating user: " + ex.Message);
            }
        }

        public DataSet GetUserDetails(string username)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT user_id, username, full_name, email, role, last_login FROM users WHERE username = @username AND is_active = 1";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            return ds;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting user details: " + ex.Message);
            }
        }

        public void UpdateLastLogin(string username)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE users SET last_login = @last_login WHERE username = @username";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@last_login", DateTime.Now);
                        command.Parameters.AddWithValue("@username", username);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating last login: " + ex.Message);
            }
        }

        public bool CreateUser(string username, string password, string fullName, string email, string role = "User")
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO users (username, password, full_name, email, role, is_active, created_date) 
                                   VALUES (@username, @password, @full_name, @email, @role, 1, @created_date)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password); // In production, use hashed passwords
                        command.Parameters.AddWithValue("@full_name", fullName);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@role", role);
                        command.Parameters.AddWithValue("@created_date", DateTime.Now);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating user: " + ex.Message);
            }
        }

        #region Configuration Management

        public DataSet GetAllConfigurations()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM configurations ORDER BY created_date DESC";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            return ds;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all configurations: " + ex.Message);
            }
        }

        public bool SaveConfiguration(UDIL.Shared.Configuration config)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO configurations (name, base_url, timeout, db_server, db_port, db_name, db_uid, db_pwd, db_provider, created_date) 
                                   VALUES (@name, @base_url, @timeout, @db_server, @db_port, @db_name, @db_uid, @db_pwd, @db_provider, @created_date)
                                   ON DUPLICATE KEY UPDATE 
                                   base_url = @base_url, timeout = @timeout, db_server = @db_server, db_port = @db_port, 
                                   db_name = @db_name, db_uid = @db_uid, db_pwd = @db_pwd, db_provider = @db_provider";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", config.Name);
                        command.Parameters.AddWithValue("@base_url", config.BaseUrl);
                        command.Parameters.AddWithValue("@timeout", config.Timeout);
                        command.Parameters.AddWithValue("@db_server", config.DbServer);
                        command.Parameters.AddWithValue("@db_port", config.DbPort);
                        command.Parameters.AddWithValue("@db_name", config.DbName);
                        command.Parameters.AddWithValue("@db_uid", config.DbUid);
                        command.Parameters.AddWithValue("@db_pwd", config.DbPwd);
                        command.Parameters.AddWithValue("@db_provider", config.DbProvider);
                        command.Parameters.AddWithValue("@created_date", DateTime.Now);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving configuration: " + ex.Message);
            }
        }

        public DataSet GetConfiguration(string name)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM configurations WHERE name = @name";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            return ds;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting configuration: " + ex.Message);
            }
        }

        public bool DeleteConfiguration(string name)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM configurations WHERE name = @name";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting configuration: " + ex.Message);
            }
        }

        #endregion

        #region Session Management

        public DataSet GetAllSessions()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM test_sessions ORDER BY created_date DESC";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            return ds;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all sessions: " + ex.Message);
            }
        }

        public bool SaveSession(UDIL.Shared.TestSession session)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO test_sessions (session_id, name, description, created_date, modified_date, status, test_environment, device_count, test_type, tests_completed, total_tests) 
                                   VALUES (@session_id, @name, @description, @created_date, @modified_date, @status, @test_environment, @device_count, @test_type, @tests_completed, @total_tests)
                                   ON DUPLICATE KEY UPDATE 
                                   description = @description, modified_date = @modified_date, status = @status, test_environment = @test_environment, 
                                   device_count = @device_count, test_type = @test_type, tests_completed = @tests_completed, total_tests = @total_tests";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@session_id", session.SessionId);
                        command.Parameters.AddWithValue("@name", session.Name);
                        command.Parameters.AddWithValue("@description", session.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@created_date", session.CreatedDate);
                        command.Parameters.AddWithValue("@modified_date", session.ModifiedDate);
                        command.Parameters.AddWithValue("@status", session.Status);
                        command.Parameters.AddWithValue("@test_environment", session.TestEnvironment);
                        command.Parameters.AddWithValue("@device_count", session.DeviceCount);
                        command.Parameters.AddWithValue("@test_type", session.TestType);
                        command.Parameters.AddWithValue("@tests_completed", session.TestsCompleted);
                        command.Parameters.AddWithValue("@total_tests", session.TotalTests);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving session: " + ex.Message);
            }
        }

        public DataSet GetSession(string name)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM test_sessions WHERE name = @name";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            return ds;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting session: " + ex.Message);
            }
        }

        public bool DeleteSession(string name)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM test_sessions WHERE name = @name";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting session: " + ex.Message);
            }
        }

        public bool UpdateSessionProgress(string sessionName, int testsCompleted, int totalTests, string status = null)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"UPDATE test_sessions SET tests_completed = @tests_completed, total_tests = @total_tests, modified_date = @modified_date";

                    if (!string.IsNullOrEmpty(status))
                    {
                        query += ", status = @status";
                    }

                    query += " WHERE name = @session_name";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@tests_completed", testsCompleted);
                        command.Parameters.AddWithValue("@total_tests", totalTests);
                        command.Parameters.AddWithValue("@modified_date", DateTime.Now);
                        command.Parameters.AddWithValue("@session_name", sessionName);

                        if (!string.IsNullOrEmpty(status))
                        {
                            command.Parameters.AddWithValue("@status", status);
                        }

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating session progress: " + ex.Message);
            }
        }

        #endregion

        #region Test Results Management

        public bool SaveTestResult(TestResult testResult)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO test_results (session_id, test_name, test_type, status, remarks, test_date, transaction_id, global_device_id)
                                   VALUES (@session_id, @test_name, @test_type, @status, @remarks, @test_date, @transaction_id, @global_device_id)
                                   ON DUPLICATE KEY UPDATE
                                   status = @status, remarks = @remarks, test_date = @test_date";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@session_id", testResult.SessionId);
                        command.Parameters.AddWithValue("@test_name", testResult.TestName);
                        command.Parameters.AddWithValue("@test_type", testResult.TestType);
                        command.Parameters.AddWithValue("@status", testResult.Status);
                        command.Parameters.AddWithValue("@remarks", testResult.Remarks ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@test_date", testResult.TestDate);
                        command.Parameters.AddWithValue("@transaction_id", testResult.TransactionId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@global_device_id", testResult.GlobalDeviceId ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving test result: " + ex.Message);
            }
        }

        public DataSet GetTestResultsBySession(string sessionId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM test_results WHERE session_id = @session_id ORDER BY test_date DESC";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@session_id", sessionId);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            return ds;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting test results: " + ex.Message);
            }
        }

        public TestResult GetTestResult(string sessionId, string testName)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM test_results WHERE session_id = @session_id AND test_name = @test_name";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@session_id", sessionId);
                        command.Parameters.AddWithValue("@test_name", testName);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new TestResult
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    SessionId = reader["session_id"].ToString(),
                                    TestName = reader["test_name"].ToString(),
                                    TestType = reader["test_type"].ToString(),
                                    Status = reader["status"].ToString(),
                                    Remarks = reader["remarks"] != DBNull.Value ? reader["remarks"].ToString() : null,
                                    TestDate = Convert.ToDateTime(reader["test_date"]),
                                    TransactionId = reader["transaction_id"] != DBNull.Value ? reader["transaction_id"].ToString() : null,
                                    GlobalDeviceId = reader["global_device_id"] != DBNull.Value ? reader["global_device_id"].ToString() : null
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting test result: " + ex.Message);
            }

            return null;
        }

        #endregion
    }
}
