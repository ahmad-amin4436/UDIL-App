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

    public class TransactionRecord
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string SessionId { get; set; }
        public string PageName { get; set; }
        public string TestType { get; set; }
        public string Status { get; set; }
        public string GlobalDeviceId { get; set; }
        public DateTime CreatedDate { get; set; }

        public TransactionRecord()
        {
            CreatedDate = DateTime.Now;
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

        #region Transaction Management

        public bool SaveTransaction(TransactionRecord transaction)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    EnsureTransactionsTable(connection);

                    string query = @"INSERT INTO `transactions` (transaction_id, session_id, page_name, test_type, status, global_device_id, created_date)
                                   VALUES (@transaction_id, @session_id, @page_name, @test_type, @status, @global_device_id, @created_date)
                                   ON DUPLICATE KEY UPDATE
                                   session_id = @session_id, page_name = @page_name, test_type = @test_type,
                                   status = @status, global_device_id = @global_device_id, created_date = @created_date";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@transaction_id", transaction.TransactionId);
                        command.Parameters.AddWithValue("@session_id", transaction.SessionId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@page_name", transaction.PageName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@test_type", transaction.TestType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@status", transaction.Status);
                        command.Parameters.AddWithValue("@global_device_id", transaction.GlobalDeviceId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@created_date", transaction.CreatedDate);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving transaction: " + ex.Message);
            }
        }

        public DataTable GetTransactions()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    EnsureTransactionsTable(connection);

                    string query = @"SELECT transaction_id, session_id, page_name, test_type, status, global_device_id, created_date
                                   FROM `transactions`
                                   ORDER BY created_date DESC";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting transactions: " + ex.Message);
            }
        }

        private void EnsureTransactionsTable(MySqlConnection connection)
        {
            string query = @"CREATE TABLE IF NOT EXISTS `transactions` (
                                id INT AUTO_INCREMENT PRIMARY KEY,
                                transaction_id VARCHAR(100) NOT NULL,
                                session_id VARCHAR(32) NULL,
                                page_name VARCHAR(255) NULL,
                                test_type VARCHAR(50) NULL,
                                status VARCHAR(50) NOT NULL DEFAULT 'Pending',
                                global_device_id VARCHAR(100) NULL,
                                created_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                UNIQUE KEY unique_transaction_id (transaction_id),
                                INDEX idx_transaction_session_id (session_id),
                                INDEX idx_transaction_id (transaction_id),
                                INDEX idx_transaction_status (status),
                                INDEX idx_transaction_created_date (created_date),
                                CONSTRAINT fk_transactions_session_id FOREIGN KEY (session_id) REFERENCES test_sessions(session_id) ON DELETE SET NULL
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
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

        public DataSet GetFailedTestResultsBySession(string sessionId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT id, session_id, test_name, test_type, status, remarks, test_date, transaction_id, global_device_id 
                                    FROM test_results 
                                    WHERE session_id = @session_id 
                                    AND UPPER(status) = 'FAIL' 
                                    ORDER BY test_date DESC";

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
                throw new Exception("Error getting failed test results: " + ex.Message);
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

        #region Navigation Management

        /// <summary>
        /// Get all active navigation items ordered by group and sort order
        /// </summary>
        public List<NavigationItem> GetAllNavigationItems()
        {
            try
            {
                List<NavigationItem> navigationItems = new List<NavigationItem>();

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT id, title, url, icon, `group`, sort_order, is_active, required_role, is_external 
                                    FROM navigation_items 
                                    WHERE is_active = 1 
                                    ORDER BY `group`, sort_order, id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                navigationItems.Add(new NavigationItem
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Title = reader["title"].ToString(),
                                    Url = reader["url"].ToString(),
                                    Icon = reader["icon"].ToString(),
                                    Group = reader["group"] != DBNull.Value ? reader["group"].ToString() : null,
                                    SortOrder = Convert.ToInt32(reader["sort_order"]),
                                    IsActive = Convert.ToBoolean(reader["is_active"]),
                                    RequiredRole = reader["required_role"] != DBNull.Value ? reader["required_role"].ToString() : null,
                                    IsExternal = reader["is_external"] != DBNull.Value && Convert.ToBoolean(reader["is_external"])
                                });
                            }
                        }
                    }
                }

                return navigationItems;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting navigation items: " + ex.Message);
            }
        }

        /// <summary>
        /// Get navigation items grouped by their group name
        /// </summary>
        public Dictionary<string, List<NavigationItem>> GetGroupedNavigationItems()
        {
            try
            {
                var groupedItems = new Dictionary<string, List<NavigationItem>>();
                var allItems = GetAllNavigationItems();

                foreach (var item in allItems)
                {
                    string groupKey = string.IsNullOrEmpty(item.Group) ? "General" : item.Group;
                    
                    if (!groupedItems.ContainsKey(groupKey))
                    {
                        groupedItems[groupKey] = new List<NavigationItem>();
                    }

                    groupedItems[groupKey].Add(item);
                }

                // Sort each group by sort order
                foreach (var key in groupedItems.Keys.ToList())
                {
                    groupedItems[key] = groupedItems[key].OrderBy(x => x.SortOrder).ToList();
                }

                return groupedItems;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting grouped navigation items: " + ex.Message);
            }
        }

        /// <summary>
        /// Get navigation items for a specific group
        /// </summary>
        public List<NavigationItem> GetNavigationItemsByGroup(string group)
        {
            try
            {
                List<NavigationItem> navigationItems = new List<NavigationItem>();

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT id, title, url, icon, `group`, sort_order, is_active, required_role, is_external 
                                    FROM navigation_items 
                                    WHERE is_active = 1 AND `group` = @group
                                    ORDER BY sort_order, id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@group", group);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                navigationItems.Add(new NavigationItem
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Title = reader["title"].ToString(),
                                    Url = reader["url"].ToString(),
                                    Icon = reader["icon"].ToString(),
                                    Group = reader["group"] != DBNull.Value ? reader["group"].ToString() : null,
                                    SortOrder = Convert.ToInt32(reader["sort_order"]),
                                    IsActive = Convert.ToBoolean(reader["is_active"]),
                                    RequiredRole = reader["required_role"] != DBNull.Value ? reader["required_role"].ToString() : null,
                                    IsExternal = reader["is_external"] != DBNull.Value && Convert.ToBoolean(reader["is_external"])
                                });
                            }
                        }
                    }
                }

                return navigationItems;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting navigation items by group: " + ex.Message);
            }
        }

        /// <summary>
        /// Add a new navigation item
        /// </summary>
        public bool AddNavigationItem(NavigationItem item)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO navigation_items (title, url, icon, `group`, sort_order, is_active, required_role, is_external) 
                                   VALUES (@title, @url, @icon, @group, @sort_order, @is_active, @required_role, @is_external)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@title", item.Title);
                        command.Parameters.AddWithValue("@url", item.Url);
                        command.Parameters.AddWithValue("@icon", (object)item.Icon ?? DBNull.Value);
                        command.Parameters.AddWithValue("@group", (object)item.Group ?? DBNull.Value);
                        command.Parameters.AddWithValue("@sort_order", item.SortOrder);
                        command.Parameters.AddWithValue("@is_active", item.IsActive);
                        command.Parameters.AddWithValue("@required_role", (object)item.RequiredRole ?? DBNull.Value);
                        command.Parameters.AddWithValue("@is_external", item.IsExternal);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding navigation item: " + ex.Message);
            }
        }

        /// <summary>
        /// Update an existing navigation item
        /// </summary>
        public bool UpdateNavigationItem(NavigationItem item)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"UPDATE navigation_items 
                                   SET title = @title, url = @url, icon = @icon, `group` = @group, 
                                       sort_order = @sort_order, is_active = @is_active, 
                                       required_role = @required_role, is_external = @is_external
                                   WHERE id = @id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", item.Id);
                        command.Parameters.AddWithValue("@title", item.Title);
                        command.Parameters.AddWithValue("@url", item.Url);
                        command.Parameters.AddWithValue("@icon", (object)item.Icon ?? DBNull.Value);
                        command.Parameters.AddWithValue("@group", (object)item.Group ?? DBNull.Value);
                        command.Parameters.AddWithValue("@sort_order", item.SortOrder);
                        command.Parameters.AddWithValue("@is_active", item.IsActive);
                        command.Parameters.AddWithValue("@required_role", (object)item.RequiredRole ?? DBNull.Value);
                        command.Parameters.AddWithValue("@is_external", item.IsExternal);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating navigation item: " + ex.Message);
            }
        }

        /// <summary>
        /// Delete a navigation item
        /// </summary>
        public bool DeleteNavigationItem(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM navigation_items WHERE id = @id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting navigation item: " + ex.Message);
            }
        }

        /// <summary>
        /// Get a single navigation item by ID
        /// </summary>
        public NavigationItem GetNavigationItemById(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT id, title, url, icon, `group`, sort_order, is_active, required_role, is_external 
                                    FROM navigation_items 
                                    WHERE id = @id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new NavigationItem
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Title = reader["title"].ToString(),
                                    Url = reader["url"].ToString(),
                                    Icon = reader["icon"].ToString(),
                                    Group = reader["group"] != DBNull.Value ? reader["group"].ToString() : null,
                                    SortOrder = Convert.ToInt32(reader["sort_order"]),
                                    IsActive = Convert.ToBoolean(reader["is_active"]),
                                    RequiredRole = reader["required_role"] != DBNull.Value ? reader["required_role"].ToString() : null,
                                    IsExternal = reader["is_external"] != DBNull.Value && Convert.ToBoolean(reader["is_external"])
                                };
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting navigation item: " + ex.Message);
            }
        }

        #endregion
    }
}
