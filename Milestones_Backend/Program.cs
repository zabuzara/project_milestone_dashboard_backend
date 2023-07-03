using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Milestones
{
    /// <summary>
    /// Milestones project as a Web-API in 
    /// ASP.NET C#, used MongoDB-Atlas in Cloud
    /// </summary>
    public class Program 
    {
        /// <summary>
        /// Application Entry-Point
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates the Host
        /// </summary>
        /// <param name="args"></param>
        /// <returns>the Host</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}