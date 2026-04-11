using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace UDIL.Common
{
    public static class AppCommon
    {
        public static string GenerateTransactionId()
        {
            return "TXN" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public static string HandleTransactionStatusRequest(string transactionId, string privateKey)
        {
            try
            {
                string baseUrl = GetBaseUrl();

                if (string.IsNullOrEmpty(transactionId) || string.IsNullOrEmpty(privateKey))
                {
                    return "{\"status\":\"0\",\"message\":\"Missing transaction ID or private key\"}";
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/UIP/transaction_status");
                request.Method = "POST";
                request.Headers.Add("transactionid", transactionId);
                request.Headers.Add("privatekey", privateKey);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseContent = reader.ReadToEnd();
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        TransactionStatusResponse statusResponse = serializer.Deserialize<TransactionStatusResponse>(responseContent);

                        // Enhance the response with stage descriptions
                        if (statusResponse.data != null)
                        {
                            foreach (var item in statusResponse.data)
                            {
                                if (!string.IsNullOrEmpty(item.status_level))
                                {
                                    int statusLevel;
                                    if (int.TryParse(item.status_level, out statusLevel))
                                    {
                                        // Add stage description to the response
                                        var enhancedItem = new
                                        {
                                            global_device_id = item.global_device_id,
                                            msn = item.msn,
                                            type = item.type,
                                            transactionid = item.transactionid,
                                            request_cancel_reason = item.request_cancel_reason,
                                            status_4_datetime = item.status_4_datetime,
                                            response_data = item.response_data,
                                            status_1_datetime = item.status_1_datetime,
                                            command_receiving_datetime = item.command_receiving_datetime,
                                            indv_status = item.indv_status,
                                            request_cancel_datetime = item.request_cancel_datetime,
                                            status_5_datetime = item.status_5_datetime,
                                            status_2_datetime = item.status_2_datetime,
                                            status_level = item.status_level,
                                            request_cancelled = item.request_cancelled,
                                            status_3_datetime = item.status_3_datetime,
                                            stage_description = GetStageDescription(statusLevel)
                                        };
                                    }
                                }
                            }
                        }

                        return responseContent;
                    }
                }
            }
            catch (Exception ex)
            {
                return "{\"status\":\"0\",\"message\":\"Error: " + ex.Message.Replace("\"", "\\\"") + "\"}";
            }
        }

        public static string GetStageDescription(int statusLevel)
        {
            switch (statusLevel)
            {
                case -1: return "Command cancelled by meter";
                case 0: return "Waiting for processing";
                case 1: return "Commencing command processing";
                case 2: return "Communication request sent to meter";
                case 3: return "Communication established with meter";
                case 4: return "Command sent to meter";
                case 5: return "Command executed by meter";
                case 6: return "Device Communication History Log validated";
                case 7: return "Meter visual data validated";
                default: return "Unknown stage";
            }
        }

        public static string GetBaseUrl()
        {
            return UDIL.Common.ConfigurationManager.GetBaseUrl();
        }

        public static int GetTimeout()
        {
            return UDIL.Common.ConfigurationManager.GetTimeout();
        }

        public static string GetConnectionString()
        {
            return UDIL.Common.ConfigurationManager.GetConnectionString();
        }
    }

    public class TransactionStatusResponse
    {
        public string status { get; set; }
        public string transactionid { get; set; }
        public string message { get; set; }
        public List<TransactionStatusData> data { get; set; }
    }

    public class TransactionStatusData
    {
        public string global_device_id { get; set; }
        public string msn { get; set; }
        public string type { get; set; }
        public string transactionid { get; set; }
        public string request_cancel_reason { get; set; }
        public string status_4_datetime { get; set; }
        public string response_data { get; set; }
        public string status_1_datetime { get; set; }
        public string command_receiving_datetime { get; set; }
        public string indv_status { get; set; }
        public string request_cancel_datetime { get; set; }
        public string status_5_datetime { get; set; }
        public string status_2_datetime { get; set; }
        public string status_level { get; set; }
        public string request_cancelled { get; set; }
        public string status_3_datetime { get; set; }
    }
}
