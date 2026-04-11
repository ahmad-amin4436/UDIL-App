using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;

namespace UDIL.Common
{
    public static class ConfigurationManager
    {
        private const string CONFIG_FILE_PATH = "~/App_Data/configurations.json";
        private const string SESSION_CURRENT_CONFIG = "CurrentConfiguration";
        
        private static Dictionary<string, Configuration> _configurations;
        private static Configuration _currentConfig;

        static ConfigurationManager()
        {
            LoadConfigurationsFromFile();
            LoadCurrentConfiguration();
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
        /// Gets all saved configurations
        /// </summary>
        public static Dictionary<string, Configuration> GetAllConfigurations()
        {
            if (_configurations == null)
            {
                LoadConfigurationsFromFile();
            }
            return _configurations ?? new Dictionary<string, Configuration>();
        }

        /// <summary>
        /// Saves a configuration with the given name
        /// </summary>
        public static void SaveConfiguration(string name, Configuration config)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Configuration name cannot be empty", nameof(name));
                }

                if (_configurations == null)
                {
                    _configurations = new Dictionary<string, Configuration>();
                }

                config.Name = name;
                config.CreatedDate = DateTime.Now;
                _configurations[name] = config;

                SaveConfigurationsToFile();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving configuration '{name}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads a configuration by name
        /// </summary>
        public static Configuration LoadConfiguration(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Configuration name cannot be empty", nameof(name));
            }

            if (_configurations == null)
            {
                LoadConfigurationsFromFile();
            }

            if (_configurations != null && _configurations.ContainsKey(name))
            {
                return _configurations[name];
            }

            throw new Exception($"Configuration '{name}' not found");
        }

        /// <summary>
        /// Deletes a configuration by name
        /// </summary>
        public static void DeleteConfiguration(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Configuration name cannot be empty", nameof(name));
                }

                if (_configurations != null && _configurations.ContainsKey(name))
                {
                    _configurations.Remove(name);
                    SaveConfigurationsToFile();
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
        /// Refreshes configurations from file
        /// </summary>
        public static void RefreshConfigurations()
        {
            LoadConfigurationsFromFile();
            LoadCurrentConfiguration();
        }

        #endregion

        #region Private Methods

        private static void LoadConfigurationsFromFile()
        {
            try
            {
                if (HttpContext.Current?.Server != null)
                {
                    string configPath = HttpContext.Current.Server.MapPath(CONFIG_FILE_PATH);
                    
                    if (File.Exists(configPath))
                    {
                        string json = File.ReadAllText(configPath);
                        var serializer = new JavaScriptSerializer();
                        _configurations = serializer.Deserialize<Dictionary<string, Configuration>>(json);
                    }
                    else
                    {
                        _configurations = new Dictionary<string, Configuration>();
                        // Create the file with empty configurations
                        SaveConfigurationsToFile();
                    }
                }
                else
                {
                    _configurations = new Dictionary<string, Configuration>();
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                System.Diagnostics.Debug.WriteLine($"Error loading configurations: {ex.Message}");
                _configurations = new Dictionary<string, Configuration>();
            }
        }

        private static void SaveConfigurationsToFile()
        {
            try
            {
                if (HttpContext.Current?.Server != null)
                {
                    string configPath = HttpContext.Current.Server.MapPath(CONFIG_FILE_PATH);
                    
                    // Ensure directory exists
                    string directory = Path.GetDirectoryName(configPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    string json = new JavaScriptSerializer().Serialize(_configurations);
                    File.WriteAllText(configPath, json);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving configurations to file: {ex.Message}", ex);
            }
        }

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

                // Load first available configuration from saved configurations
                if (_configurations != null && _configurations.Count > 0)
                {
                    foreach (var config in _configurations.Values)
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

        #endregion
    }
}
