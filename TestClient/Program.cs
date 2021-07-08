using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestGrains;

namespace TestClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            using var client = BuildClientSqlServer();

            await client.Connect();

            Console.WriteLine("\n== Step 1. Should OK ==");
            Console.WriteLine("Make sure sql server is running now, and press any key to continue");
            Console.ReadKey();
            Console.WriteLine("Key pressed");
            var result = await client.GetGrain<ITaskGrain>("key").Execute();
            Console.WriteLine(result.Count);

            Console.WriteLine("\n== Step 2. Should Exception ==");
            Console.WriteLine("Stop sql server now, and press any key to continue");
            Console.ReadKey();
            Console.WriteLine("Key pressed");

            try
            {
                result = await client.GetGrain<ITaskGrain>("key").Execute();
                Console.WriteLine(result.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("\n== Step 3. Should OK ==");
            Console.WriteLine("Restart sql server now, and press any key to continue");
            Console.ReadKey();
            Console.WriteLine("Key pressed");

            result = await client.GetGrain<ITaskGrain>("key").Execute();
            Console.WriteLine(result.Count);

        }

        private static IClusterClient BuildClientSqlServer()
        {
            return new ClientBuilder()
                .UseAdoNetClustering(options =>
                {
                    options.Invariant = Constants.SqlServerInvariant;
                    options.ConnectionString = Constants.SqlServerConnectionString;
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ServiceId = Constants.ServiceId;
                    options.ClusterId = Constants.ClusterId;
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();
        }

        private static IClusterClient BuildClientInMemory()
        {
            return new ClientBuilder()
                .UseLocalhostClustering()
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();
        }
    }
}