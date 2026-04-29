using System;
using System.Web;

namespace UDIL.Shared
{
    public class Configuration
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public int Timeout { get; set; }
        public DateTime CreatedDate { get; set; }
        
        // Database configuration
        public string DbServer { get; set; }
        public string DbPort { get; set; }
        public string DbName { get; set; }
        public string DbUid { get; set; }
        public string DbPwd { get; set; }
        public string DbProvider { get; set; }

        public Configuration()
        {
            // Default values
            BaseUrl = "http://116.58.46.245:4050/UIP";
            Timeout = 60;
            CreatedDate = DateTime.Now;
            
            // Default database configuration
            DbServer = "116.58.46.245";
            DbPort = "4000";
            DbName = "udil33";
            DbUid = "accurate";
            DbPwd = "Accurate@123";
            DbProvider = "MySql.Data.MySqlClient";
        }

        public string GetConnectionString()
        {
            return $"Server={DbServer};Port={DbPort};Database={DbName};Uid={DbUid};Pwd={DbPwd}";
        }

        public string GetApiUrl(string endpoint)
        {
            return $"{BaseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
        }
    }

    public class TestSession
    {
        public string SessionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Status { get; set; }
        public string TestEnvironment { get; set; }
        public int DeviceCount { get; set; }
        public string TestType { get; set; }
        public int TestsCompleted { get; set; }
        public int TotalTests { get; set; }

        public TestSession()
        {
            // Default values
            SessionId = Guid.NewGuid().ToString("N");
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            Status = "Not Started";
            TestEnvironment = "Production";
            DeviceCount = 0;
            TestType = "Compliance";
            TestsCompleted = 0;
            TotalTests = 0;
        }

        public double GetProgressPercentage()
        {
            if (TotalTests == 0) return 0;
            return (double)TestsCompleted / TotalTests * 100;
        }

        public TimeSpan GetDuration()
        {
            return ModifiedDate - CreatedDate;
        }
    }
}
