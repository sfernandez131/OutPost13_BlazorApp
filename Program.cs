using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace OutPost13
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).UseContentRoot(Directory.GetCurrentDirectory()).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseStartup<Startup>()
                    .UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
                });
    }
}
