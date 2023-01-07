using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Zip.InstallmentsService.Service
{
    /// <summary>
    /// Class which defines a main methods and start-up setup details
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args);
            
            builder.Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
