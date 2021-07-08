using System;
using System.Collections.Generic;
using System.Text;

namespace TestGrains
{
    public static class Constants
    {
        public const string ClusterId = "TestOrleansCluster";
        public const string ServiceId = "TestOrleansService";
        public const string SqlServerConnectionString = "Data Source=localhost;Initial Catalog=TestOrleansDb;uid=testorleans;pwd=testorleans;ConnectRetryCount=10;ConnectRetryInterval=3;";
        public const string SqlServerInvariant = "System.Data.SqlClient";
        public const string StorageName = "dev";
        public const int SiloPort = 30000;
        public const int SiloGatewayPort = 40000;
    }
}
