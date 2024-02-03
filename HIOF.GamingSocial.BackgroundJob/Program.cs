using HIOF.GamingSocial.BackgroundJob;
using System.Diagnostics;

namespace BackgroundJob;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                // This requires administrative permissions.
                if (!EventLog.SourceExists("GS/BackgroundJobSource"))
                {
                    EventLog.CreateEventSource("GS/BackgroundJobSource", "GS/BackgroundJobLog");
                }

                logging.AddEventLog(settings =>
                {
                    settings.SourceName = "GS/BackgroundJobSource";
                    settings.LogName = "GS/BackgroundJobLog";
                });
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHttpClient<SteamApiService>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7296");
                });

                services.AddHostedService<Worker>();
            });
}
