using System.Web;

namespace UDIL.Shared
{
    /// <summary>
    /// Centralized session management for device information
    /// </summary>
    public static class SessionManager
    {
        private const string GLOBAL_DEVICE_ID_KEY = "GlobalDeviceId";
        private const string MSN_KEY = "MSN";
        private const string PRIVATE_KEY_KEY = "PrivateKey";
        private const string CURRENT_TRANSACTION_ID_KEY = "CurrentTransactionId";

        /// <summary>
        /// Gets or sets the Global Device ID in session
        /// </summary>
        public static string GlobalDeviceId
        {
            get
            {
                var context = System.Web.HttpContext.Current;
                return context?.Session?[GLOBAL_DEVICE_ID_KEY]?.ToString();
            }
            set
            {
                var context = System.Web.HttpContext.Current;
                if (context?.Session != null)
                {
                    context.Session[GLOBAL_DEVICE_ID_KEY] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the MSN in session
        /// </summary>
        public static string MSN
        {
            get
            {
                var context = System.Web.HttpContext.Current;
                return context?.Session?[MSN_KEY]?.ToString();
            }
            set
            {
                var context = System.Web.HttpContext.Current;
                if (context?.Session != null)
                {
                    context.Session[MSN_KEY] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Private Key in session
        /// </summary>
        public static string PrivateKey
        {
            get
            {
                var context = System.Web.HttpContext.Current;
                return context?.Session?[PRIVATE_KEY_KEY]?.ToString();
            }
            set
            {
                var context = System.Web.HttpContext.Current;
                if (context?.Session != null)
                {
                    context.Session[PRIVATE_KEY_KEY] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Current Transaction ID in session
        /// </summary>
        public static string CurrentTransactionId
        {
            get
            {
                var context = System.Web.HttpContext.Current;
                return context?.Session?[CURRENT_TRANSACTION_ID_KEY]?.ToString();
            }
            set
            {
                var context = System.Web.HttpContext.Current;
                if (context?.Session != null)
                {
                    context.Session[CURRENT_TRANSACTION_ID_KEY] = value;
                }
            }
        }

        /// <summary>
        /// Clears all device-related session values
        /// </summary>
        public static void ClearDeviceSession()
        {
            var context = System.Web.HttpContext.Current;
            if (context?.Session != null)
            {
                context.Session.Remove(GLOBAL_DEVICE_ID_KEY);
                context.Session.Remove(MSN_KEY);
                context.Session.Remove(CURRENT_TRANSACTION_ID_KEY);
            }
        }

        /// <summary>
        /// Checks if Global Device ID is available in session
        /// </summary>
        public static bool HasGlobalDeviceId
        {
            get
            {
                return !string.IsNullOrEmpty(GlobalDeviceId);
            }
        }

        /// <summary>
        /// Checks if MSN is available in session
        /// </summary>
        public static bool HasMSN
        {
            get
            {
                return !string.IsNullOrEmpty(MSN);
            }
        }

        /// <summary>
        /// Checks if Private Key is available in session
        /// </summary>
        public static bool HasPrivateKey
        {
            get
            {
                return !string.IsNullOrEmpty(PrivateKey);
            }
        }
    }
}
