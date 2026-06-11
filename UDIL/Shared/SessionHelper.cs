using System;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

namespace UDIL.Shared
{
    public static class SessionHelper
    {
        public static void Unlock()
        {
            try
            {
                HttpContext context = HttpContext.Current;
                if (context?.Session == null) return;

                var containerField = typeof(HttpSessionState).GetField("_container", BindingFlags.NonPublic | BindingFlags.Instance);
                if (containerField == null) return;

                var container = containerField.GetValue(context.Session);
                if (container == null) return;

                var unlockMethod = container.GetType().GetMethod("Unlock", BindingFlags.NonPublic | BindingFlags.Instance);
                if (unlockMethod == null) return;

                unlockMethod.Invoke(container, null);
            }
            catch
            {
            }
        }
    }
}
