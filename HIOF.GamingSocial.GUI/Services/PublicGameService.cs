using HIOF.GamingSocial.GUI.Model;
using System.Text.Json;

namespace HIOF.GamingSocial.GUI.Services;

public class PublicGameService
{
    // https://localhost:7250/V2/PublicGameInformation/10
    private readonly ILogger<PublicGameService> _logger;
    private readonly HttpClient _httpClient;
    private string PublicGameInformationApiBase = "https://localhost:7250";
    private string GameInformationApiBase = "https://localhost:7296";
    public PublicGameService(ILogger<PublicGameService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }
    public async Task<V3Result<V2SteamGameDetails>> GetGameAsync(int gameId)
    {
        var response = await _httpClient.GetAsync($"{GameInformationApiBase}/V3/VideoGameInformation/{gameId}");
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        if (response.IsSuccessStatusCode)
        {
            var gameInformationResult = JsonSerializer.Deserialize<V3Result<V3VideoGameInformation>>(responseString, options);

            response = await _httpClient.GetAsync($"{PublicGameInformationApiBase}/V2/PublicGameInformation/{gameInformationResult.Value.SteamAppId}");
            responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var okResult = JsonSerializer.Deserialize<V3Result<V2SteamGameDetails>>(responseString, options);
                return okResult;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var notFoundResult = JsonSerializer.Deserialize<V3Result<V2SteamGameDetails>>(responseString, options);
                return notFoundResult;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var unauthorizedErrorResult = JsonSerializer.Deserialize<V3Result<V2SteamGameDetails>>(responseString, options);
                return unauthorizedErrorResult;
            }
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return new V3Result<V2SteamGameDetails>($"Game with id {gameId} was not found."); ;
        }

        return new V3Result<V2SteamGameDetails>("Unknown error occurred");
    }
}
