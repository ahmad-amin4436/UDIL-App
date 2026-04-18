using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using System.Web;
using System.Threading;

namespace UDIL.Shared
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
            return UDIL.Shared.ConfigurationManager.GetBaseUrl();
        }

        public static int GetTimeout()
        {
            return UDIL.Shared.ConfigurationManager.GetTimeout();
        }

        public static string GetConnectionString()
        {
            return UDIL.Shared.ConfigurationManager.GetConnectionString();
        }
        public static TransactionStatusResponse PollTransactionStatus(string transactionId, string privateKey)
        {
            TransactionStatusResponse lastResponse = null;
            int maxAttempts = 10;
            int attempt = 0;

            while (attempt < maxAttempts)
            {
                lastResponse = GetTransactionStatus(transactionId, privateKey);
                if (lastResponse?.data != null)
                {
                    // Check if command was cancelled by meter
                    bool commandCancelled = lastResponse.data.Any(item => item.indv_status == "2" && item.status_level == "4");
                    if (commandCancelled)
                    {
                        break; // Exit polling if command was cancelled
                    }

                    bool reachedFinal = lastResponse.data.All(item => int.TryParse(item.status_level, out int statusLevel) && statusLevel >= 5);
                    if (reachedFinal)
                    {
                        break;
                    }
                }

                attempt++;
                Thread.Sleep(2000);
            }

            return lastResponse;
        }
        public static TransactionStatusResponse GetTransactionStatus(string transactionId, string privateKey)
        {
            int maxRetries = 3;
            int timeoutMs = GetTimeout() * 1000; // Convert seconds to milliseconds

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    string baseUrl = GetBaseUrl();
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/transaction_status");
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.Accept = "*/*";
                    request.Headers.Add("transactionid", transactionId);
                    request.Headers.Add("privatekey", privateKey);
                    request.ContentLength = 0;
                    request.UserAgent = "UDILTester/1.0";

                    // Set timeout values (in milliseconds)
                    request.Timeout = timeoutMs;
                    request.ReadWriteTimeout = timeoutMs;

                    // Enable keep-alive
                    request.KeepAlive = true;
                    request.ServicePoint.Expect100Continue = false;
                    request.ServicePoint.UseNagleAlgorithm = false;

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseContent = reader.ReadToEnd();
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        return serializer.Deserialize<TransactionStatusResponse>(responseContent);
                    }
                }
                catch (WebException ex)
                {
                    string body = ReadWebExceptionResponse(ex);

                    // Check if it's a 401 Unauthorized with private key timeout
                    if (ex.Response is HttpWebResponse httpResponse && httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        // Try to re-authorize with fixed credentials
                        if (ReauthorizeWithFixedCredentials())
                        {
                            // Re-authorization successful, get the new private key and retry immediately
                            privateKey = HttpContext.Current.Session["PrivateKey"] as string;
                            continue; // Skip the sleep and retry immediately
                        }//
                    }

                    // If this is the last attempt, throw the exception
                    if (attempt == maxRetries)
                    {
                        //throw new WebException($"Transaction status request failed after {maxRetries} attempts. Last error: {ex.Message}. Response body: {body}", ex);
                    }

                    // Log the retry attempt (you can replace this with proper logging)
                    System.Threading.Thread.Sleep(2000 * attempt); // Wait before retry: 2s, 4s, 6s
                }
            }

            throw new WebException("Transaction status request failed: Maximum retry attempts exceeded.");
        }
        public static string ReadWebExceptionResponse(WebException ex)
        {
            if (ex.Response == null)
            {
                return string.Empty;
            }

            try
            {
                using (var responseStream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(responseStream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                return string.Empty;
            }
        }
        public static bool ReauthorizeWithFixedCredentials()
        {
            try
            {
                string baseUrl = GetBaseUrl();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/authorization_service");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("username", "Hello@123");
                request.Headers.Add("password", "123");
                request.Headers.Add("code", "235");
                request.Timeout = 30000;
                request.ReadWriteTimeout = 30000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseContent = reader.ReadToEnd();
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    AuthResponse authResponse = serializer.Deserialize<AuthResponse>(responseContent);

                    if (authResponse.status == "1")
                    {
                        HttpContext.Current.Session["PrivateKey"] = authResponse.privatekey;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log re-authorization failure if needed
                System.Diagnostics.Debug.WriteLine($"Re-authorization failed: {ex.Message}");
            }

            return false;
        }
    }
  
    public class TransactionStatusResponse
    {
        public string status { get; set; }
        public string transactionid { get; set; }
        public string message { get; set; }
        public List<TransactionStatusData> data { get; set; }
    }
    public class AuthResponse
    {
        public string status { get; set; }
        public string privatekey { get; set; }
        public string message { get; set; }
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
