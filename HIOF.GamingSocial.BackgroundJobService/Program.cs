using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace BackgroundJobService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient();
                    services.AddSingleton<IHostedService, BackgroundJobWorker>();
                });
    }

    public class BackgroundJobWorker : IHostedService, IDisposable
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private Timer _timer;

        public BackgroundJobWorker(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ExecuteJobAsync, null, TimeSpan.Zero, TimeSpan.FromHours(3));
            return Task.CompletedTask;
        }

        private async void ExecuteJobAsync(object state)
        {
            var client = _httpClientFactory.CreateClient();
            var endpointUrl = "https://localhost:7296/V3/VideoGameInformation/UpdateGameList";

            try
            {
                var requestBody = new { }; // Replace with your request body
                var response = await client.PostAsJsonAsync(endpointUrl, requestBody);
                response.EnsureSuccessStatusCode(); // Throw an exception if the response status code is not successful
                Console.WriteLine("Background job executed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing background job: {ex.Message}");
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
