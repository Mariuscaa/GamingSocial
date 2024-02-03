using BackgroundJob;
using HIOF.GamingSocial.BackgroundJob.Models;
using System.Net.Http.Json;

namespace HIOF.GamingSocial.BackgroundJob;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly SteamApiService _steamApiService;

    public Worker(ILogger<Worker> logger, SteamApiService steamApiService)
    {
        _logger = logger;
        _steamApiService = steamApiService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            _logger.LogInformation("\u001b[36mStarted getting games from Steam now. Might take a little while..\u001b[0m");

            var response = await _steamApiService.TriggerGameListUpdateAsync();
            var content = await response.Content.ReadAsStringAsync();

            if (content.StartsWith("Completed ok, games have been added to database."))
            {
                _logger.LogInformation($"\u001b[36m{content}\u001b[0m");
                _logger.LogInformation("\u001b[36mThe app might be a bit slow while everything is being set up for the first time.\u001b[0m");

            }
            else
            {
                if (response.IsSuccessStatusCode)
                {
                    var gameListResult = await response.Content.ReadFromJsonAsync<Result<List<V3VideoGameInformation>>>();


                    if (gameListResult != null && !gameListResult.HasErrors)
                    {
                        _logger.LogInformation($"\u001b[0mGame list updated successfully. Added {gameListResult.Value.Count} games.\u001b[36m");
                    }
                    else
                    {
                        _logger.LogInformation(content);
                    }
                }
                else
                {
                    _logger.LogError("Failed to update game list: {statusCode}", response.StatusCode);
                }
            }

            // Calculate the time span between now and the next desired execution time (22.00).
            var currentTime = DateTimeOffset.Now;

            var targetTime = currentTime.Date.AddHours(22); 

            if (targetTime <= currentTime)
            {
                // If today's target time has already passed, schedule for tomorrow
                targetTime = targetTime.AddDays(1); 
            }

            TimeSpan delay = targetTime - currentTime;

            await Task.Delay(delay, stoppingToken);
        }
    }
}
