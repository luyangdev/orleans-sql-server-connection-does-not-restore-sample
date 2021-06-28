using Orleans;
using Orleans.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TestGrains;
using System.Net.Sockets;
using Orleans.Configuration;

namespace TestSilo
{

    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                var host = await StartSiloSqlServer();
                Console.WriteLine("\n\n Press Enter to terminate...\n\n");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSiloSqlServer()
        {
            var builder = new SiloHostBuilder()
                .UseTransactions()
                .UseAdoNetReminderService(options =>
                {
                    options.Invariant = Constants.SqlServerInvariant;
                    options.ConnectionString = Constants.SqlServerConnectionString;
                })
                .AddAdoNetGrainStorage(Constants.StorageName, options =>
                {
                    options.Invariant = Constants.SqlServerInvariant;
                    options.ConnectionString = Constants.SqlServerConnectionString;
                })
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
                .ConfigureEndpoints(Constants.SiloPort, Constants.SiloGatewayPort, AddressFamily.InterNetwork, true)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(TaskGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }

        private static async Task<ISiloHost> StartSiloInMemory()
        {
            var builder = new SiloHostBuilder()
                .UseTransactions()
                .UseInMemoryReminderService()
                .AddMemoryGrainStorageAsDefault()
                .UseLocalhostClustering()
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(TaskGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
