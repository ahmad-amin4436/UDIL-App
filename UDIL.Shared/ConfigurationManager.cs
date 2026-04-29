using System;
using System.Web;

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
    }
}
