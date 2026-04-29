using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using UDIL.DAL;

namespace UDIL.Shared
{
    public static class ConfigurationManager
    {
        private const string SESSION_CURRENT_CONFIG = "CurrentConfiguration";
        private const string SESSION_CURRENT_SESSION = "CurrentSession";
        
        private static Configuration _currentConfig;
        private static TestSession _currentSession;

        static ConfigurationManager()
        {
            LoadCurrentConfiguration();
            LoadCurrentSession();
        }

        #region Public Methods

        /// <summary>
        /// Gets the current active configuration
        /// </summary>
        public static Configuration Current
        {
            get
            {
                if (_currentConfig == null)
                {
                    LoadCurrentConfiguration();
                }
                return _currentConfig;
            }
        }

        #region Configuration Management

        /// <summary>
        /// Gets all saved configurations from database
        /// </summary>
        public static Dictionary<string, Configuration> GetAllConfigurations()
        {
            try
            {
                var dbLayer = new DatabaseLayer();
                var configDs = dbLayer.GetAllConfigurations();
                var configurations = new Dictionary<string, Configuration>();
                
                if (configDs.Tables.Count > 0)
                {
                    foreach (DataRow row in configDs.Tables[0].Rows)
                    {
                        var config = new Configuration
                        {
                            Name = row["name"].ToString(),
                            BaseUrl = row["base_url"].ToString(),
                            Timeout = Convert.ToInt32(row["timeout"]),
                            DbServer = row["db_server"].ToString(),
                            DbPort = row["db_port"].ToString(),
                            DbName = row["db_name"].ToString(),
                            DbUid = row["db_uid"].ToString(),
                            DbPwd = row["db_pwd"].ToString(),
                            DbProvider = row["db_provider"].ToString(),
                            CreatedDate = Convert.ToDateTime(row["created_date"])
                        };
                        configurations[config.Name] = config;
                    }
                }
                
                return configurations;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading configurations from database: {ex.Message}");
                return new Dictionary<string, Configuration>();
            }
        }

        /// <summary>
        /// Saves a configuration to database
        /// </summary>
        public static void SaveConfiguration(string name, Configuration config)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Configuration name cannot be empty", nameof(name));
                }

                config.Name = name;
                config.CreatedDate = DateTime.Now;

                var dbLayer = new DatabaseLayer();
                bool success = dbLayer.SaveConfiguration(config);
                
                if (!success)
                {
                    throw new Exception($"Failed to save configuration '{name}' to database");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving configuration '{name}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads a configuration by name from database
        /// </summary>
        public static Configuration LoadConfiguration(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Configuration name cannot be empty", nameof(name));
            }

            var dbLayer = new DatabaseLayer();
            var configDs = dbLayer.GetConfiguration(name);
            
            if (configDs.Tables.Count > 0 && configDs.Tables[0].Rows.Count > 0)
            {
                DataRow row = configDs.Tables[0].Rows[0];
                return new Configuration
                {
                    Name = row["name"].ToString(),
                    BaseUrl = row["base_url"].ToString(),
                    Timeout = Convert.ToInt32(row["timeout"]),
                    DbServer = row["db_server"].ToString(),
                    DbPort = row["db_port"].ToString(),
                    DbName = row["db_name"].ToString(),
                    DbUid = row["db_uid"].ToString(),
                    DbPwd = row["db_pwd"].ToString(),
                    DbProvider = row["db_provider"].ToString(),
                    CreatedDate = Convert.ToDateTime(row["created_date"])
                };
            }

            throw new Exception($"Configuration '{name}' not found");
        }

        /// <summary>
        /// Deletes a configuration by name from database
        /// </summary>
        public static void DeleteConfiguration(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Configuration name cannot be empty", nameof(name));
                }

                var dbLayer = new DatabaseLayer();
                bool success = dbLayer.DeleteConfiguration(name);
                
                if (!success)
                {
                    throw new Exception($"Failed to delete configuration '{name}' from database");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting configuration '{name}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Applies a configuration as the current active configuration
        /// </summary>
        public static void ApplyConfiguration(Configuration config)
        {
            try
            {
                if (config == null)
                {
                    throw new ArgumentException("Configuration cannot be null", nameof(config));
                }

                _currentConfig = config;
                
                // Store in session for web application
                if (HttpContext.Current?.Session != null)
                {
                    HttpContext.Current.Session[SESSION_CURRENT_CONFIG] = config;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error applying configuration: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the base URL from current configuration
        /// </summary>
        public static string GetBaseUrl()
        {
            return Current?.BaseUrl ?? "http://116.58.46.245:4050/UIP";
        }

        /// <summary>
        /// Gets the timeout in seconds from current configuration
        /// </summary>
        public static int GetTimeout()
        {
            return Current?.Timeout ?? 60;
        }

        /// <summary>
        /// Gets the database connection string from current configuration
        /// </summary>
        public static string GetConnectionString()
        {
            return Current?.GetConnectionString() ?? "Server=116.58.46.245;Port=4000;Database=udil33;Uid=accurate;Pwd=Accurate@123;providerName=MySql.Data.MySqlClient";
        }

        /// <summary>
        /// Gets the API URL for a specific endpoint
        /// </summary>
        public static string GetApiUrl(string endpoint)
        {
            return Current?.GetApiUrl(endpoint) ?? $"http://116.58.46.245:4050/UIP/{endpoint}";
        }

        /// <summary>
        /// Refreshes configurations from database
        /// </summary>
        public static void RefreshConfigurations()
        {
            LoadCurrentConfiguration();
            LoadCurrentSession();
        }

        #endregion

        #region Session Management

        /// <summary>
        /// Gets the current active session
        /// </summary>
        public static TestSession GetCurrentSession()
        {
            if (_currentSession == null)
            {
                LoadCurrentSession();
            }
            return _currentSession;
        }

        /// <summary>
        /// Gets all saved sessions from database
        /// </summary>
        public static Dictionary<string, TestSession> GetAllSessions()
        {
            try
            {
                var dbLayer = new DatabaseLayer();
                var sessionDs = dbLayer.GetAllSessions();
                var sessions = new Dictionary<string, TestSession>();
                
                if (sessionDs.Tables.Count > 0)
                {
                    foreach (DataRow row in sessionDs.Tables[0].Rows)
                    {
                        var session = new TestSession
                        {
                            SessionId = row["session_id"].ToString(),
                            Name = row["name"].ToString(),
                            Description = row["description"] != DBNull.Value ? row["description"].ToString() : null,
                            CreatedDate = Convert.ToDateTime(row["created_date"]),
                            ModifiedDate = Convert.ToDateTime(row["modified_date"]),
                            Status = row["status"].ToString(),
                            TestEnvironment = row["test_environment"].ToString(),
                            DeviceCount = Convert.ToInt32(row["device_count"]),
                            TestType = row["test_type"].ToString(),
                            TestsCompleted = Convert.ToInt32(row["tests_completed"]),
                            TotalTests = Convert.ToInt32(row["total_tests"])
                        };
                        sessions[session.Name] = session;
                    }
                }
                
                return sessions;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading sessions from database: {ex.Message}");
                return new Dictionary<string, TestSession>();
            }
        }

        /// <summary>
        /// Saves a session to database
        /// </summary>
        public static void SaveSession(TestSession session)
        {
            try
            {
                if (session == null)
                {
                    throw new ArgumentException("Session cannot be null", nameof(session));
                }

                if (string.IsNullOrEmpty(session.Name))
                {
                    throw new ArgumentException("Session name cannot be empty", nameof(session.Name));
                }

                session.SessionId = string.IsNullOrEmpty(session.SessionId) ? Guid.NewGuid().ToString("N") : session.SessionId;
                session.CreatedDate = session.CreatedDate == DateTime.MinValue ? DateTime.Now : session.CreatedDate;
                session.ModifiedDate = DateTime.Now;

                var dbLayer = new DatabaseLayer();
                bool success = dbLayer.SaveSession(session);
                
                if (!success)
                {
                    throw new Exception($"Failed to save session '{session.Name}' to database");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving session '{session.Name}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads a session by name from database
        /// </summary>
        public static TestSession LoadSession(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Session name cannot be empty", nameof(name));
            }

            var dbLayer = new DatabaseLayer();
            var sessionDs = dbLayer.GetSession(name);
            
            if (sessionDs.Tables.Count > 0 && sessionDs.Tables[0].Rows.Count > 0)
            {
                DataRow row = sessionDs.Tables[0].Rows[0];
                return new TestSession
                {
                    SessionId = row["session_id"].ToString(),
                    Name = row["name"].ToString(),
                    Description = row["description"] != DBNull.Value ? row["description"].ToString() : null,
                    CreatedDate = Convert.ToDateTime(row["created_date"]),
                    ModifiedDate = Convert.ToDateTime(row["modified_date"]),
                    Status = row["status"].ToString(),
                    TestEnvironment = row["test_environment"].ToString(),
                    DeviceCount = Convert.ToInt32(row["device_count"]),
                    TestType = row["test_type"].ToString(),
                    TestsCompleted = Convert.ToInt32(row["tests_completed"]),
                    TotalTests = Convert.ToInt32(row["total_tests"])
                };
            }

            throw new Exception($"Session '{name}' not found");
        }

        /// <summary>
        /// Deletes a session by name from database
        /// </summary>
        public static void DeleteSession(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Session name cannot be empty", nameof(name));
                }

                var dbLayer = new DatabaseLayer();
                bool success = dbLayer.DeleteSession(name);
                
                if (!success)
                {
                    throw new Exception($"Failed to delete session '{name}' from database");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting session '{name}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sets the current active session
        /// </summary>
        public static void SetCurrentSession(TestSession session)
        {
            try
            {
                if (session == null)
                {
                    throw new ArgumentException("Session cannot be null", nameof(session));
                }

                _currentSession = session;
                
                // Store in session for web application
                if (HttpContext.Current?.Session != null)
                {
                    HttpContext.Current.Session[SESSION_CURRENT_SESSION] = session;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error setting current session: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Clears the current session
        /// </summary>
        public static void ClearCurrentSession()
        {
            _currentSession = null;
            
            if (HttpContext.Current?.Session != null)
            {
                HttpContext.Current.Session[SESSION_CURRENT_SESSION] = null;
            }
        }

        /// <summary>
        /// Updates session progress in database
        /// </summary>
        public static void UpdateSessionProgress(string sessionName, int testsCompleted, int totalTests, string status = null)
        {
            try
            {
                var dbLayer = new DatabaseLayer();
                bool success = dbLayer.UpdateSessionProgress(sessionName, testsCompleted, totalTests, status);
                
                if (!success)
                {
                    throw new Exception($"Failed to update session progress for '{sessionName}' in database");
                }
                
                // Update current session if it's the same
                var currentSession = GetCurrentSession();
                if (currentSession != null && currentSession.Name == sessionName)
                {
                    currentSession.TestsCompleted = testsCompleted;
                    currentSession.TotalTests = totalTests;
                    currentSession.ModifiedDate = DateTime.Now;
                    
                    if (!string.IsNullOrEmpty(status))
                    {
                        currentSession.Status = status;
                    }
                    
                    SetCurrentSession(currentSession);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating session progress: {ex.Message}", ex);
            }
        }

        #endregion

        #region Private Methods

        private static void LoadCurrentConfiguration()
        {
            try
            {
                // Try to load from session first (web application)
                if (HttpContext.Current?.Session != null)
                {
                    var sessionConfig = HttpContext.Current.Session[SESSION_CURRENT_CONFIG] as Configuration;
                    if (sessionConfig != null)
                    {
                        _currentConfig = sessionConfig;
                        return;
                    }
                }

                // Load first available configuration from database
                var configurations = GetAllConfigurations();
                if (configurations.Count > 0)
                {
                    foreach (var config in configurations.Values)
                    {
                        _currentConfig = config;
                        return;
                    }
                }

                // Use default configuration if no saved configurations exist
                _currentConfig = new Configuration();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading current configuration: {ex.Message}");
                _currentConfig = new Configuration();
            }
        }

        private static void LoadCurrentSession()
        {
            try
            {
                // Try to load from session first (web application)
                if (HttpContext.Current?.Session != null)
                {
                    var sessionSession = HttpContext.Current.Session[SESSION_CURRENT_SESSION] as TestSession;
                    if (sessionSession != null)
                    {
                        _currentSession = sessionSession;
                        return;
                    }
                }

                // Load first available session from database
                var sessions = GetAllSessions();
                if (sessions.Count > 0)
                {
                    foreach (var session in sessions.Values)
                    {
                        _currentSession = session;
                        return;
                    }
                }

                // No current session
                _currentSession = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading current session: {ex.Message}");
                _currentSession = null;
            }
        }

        #endregion

        #endregion
    }
}
