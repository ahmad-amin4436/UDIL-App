using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.UI;
using UDIL.DAL;
using UDIL.Shared;

namespace UDIL.Pages
{
    public static class TransactionLogger
    {
        public static void SavePendingTransaction(string transactionId, string globalDeviceId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
            {
                return;
            }

            try
            {
                SessionManager.CurrentTransactionId = transactionId;
                var currentSession = UDIL.Shared.ConfigurationManager.GetCurrentSession();

                var transaction = new TransactionRecord
                {
                    TransactionId = transactionId,
                    SessionId = currentSession?.SessionId,
                    PageName = GetCurrentPageTestName(),
                    TestType = currentSession?.TestType,
                    Status = "Pending",
                    CreatedDate = DateTime.Now,
                    GlobalDeviceId = string.IsNullOrWhiteSpace(globalDeviceId) ? null : globalDeviceId
                };

                var dbLayer = new DatabaseLayer(System.Configuration.ConfigurationManager.ConnectionStrings["TestSuitConnenction"]?.ConnectionString);
                dbLayer.SaveTransaction(transaction);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[TransactionLogger] Error saving transaction ID: " + ex.Message);
            }
        }

        private static string GetCurrentPageTestName()
        {
            var page = HttpContext.Current?.CurrentHandler as Page;
            string path = page?.AppRelativeVirtualPath ?? HttpContext.Current?.Request?.AppRelativeCurrentExecutionFilePath;
            string pageName = Path.GetFileNameWithoutExtension(path);

            return string.IsNullOrWhiteSpace(pageName)
                ? "Page"
                : pageName;
        }
    }
}
