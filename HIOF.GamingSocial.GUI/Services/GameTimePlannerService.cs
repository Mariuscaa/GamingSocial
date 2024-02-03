using HIOF.GamingSocial.GUI.Model;

namespace HIOF.GamingSocial.GUI.Services;

public class GameTimePlannerService
{
    private readonly ILogger<GameTimePlannerService> _logger;
    private readonly HttpClient _httpClient;
    string GameTimePlannerApiBase = "https://localhost:7287";

    public GameTimePlannerService(ILogger<GameTimePlannerService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<V3Result<V1GameTimePlan>> CreateGameTimePlanAsync(V1PostGameTimePlan postGameTimePlan)
    {
        if (postGameTimePlan.GroupId == 0)
        {
            return new V3Result<V1GameTimePlan>("Group is required.");
        }
        if (postGameTimePlan.GameId == 0)
        {
            return new V3Result<V1GameTimePlan>("Game is required.");
        }
        var response = await _httpClient.PostAsJsonAsync($"{GameTimePlannerApiBase}/V1GameTimePlan", postGameTimePlan);

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V1GameTimePlan>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V1GameTimePlan> object.");
                return new V3Result<V1GameTimePlan>("Ran into an unknown issue when creating gametimeplan.");
            }
        }
        return await response.Content.ReadFromJsonAsync<V3Result<V1GameTimePlan>>();
    }

    public async Task<V3Result<List<V1GameTimePlan>>> GetGameTimePlansAsync(int groupId)
    {
        var response = await _httpClient.GetAsync($"{GameTimePlannerApiBase}/V1GameTimePlan?groupId={groupId}");
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<List<V1GameTimePlan>>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<List<V1GameTimePlan>> object.");
                return new V3Result<List<V1GameTimePlan>>("Ran into an unknown issue when getting gametimeplans.");
            }
        }

        return await response.Content.ReadFromJsonAsync<V3Result<List<V1GameTimePlan>>>();

    }
    public async Task<V3Result<V1GameTimePlan>> GetSingleGameTimePlanAsync(int gameTimePlanId)
    {
        var response = await _httpClient.GetAsync($"{GameTimePlannerApiBase}/V1GameTimePlan/{gameTimePlanId}");
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadAsStringAsync();
            if (errorResult != null)
            {
                return new V3Result<V1GameTimePlan>(errorResult);
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V1GameTimePlan> object.");
                return new V3Result<V1GameTimePlan>("Ran into an unknown issue when getting gametimeplan.");
            }
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<V3Result<V1GameTimePlan>>();
        }
    }
    public async Task<V3Result<V1GameTimePlan>> DeleteGameTimePlanAsync(int gameTimePlanId)
    {
        var response = await _httpClient.DeleteAsync($"{GameTimePlannerApiBase}/V1GameTimePlan?gameTimePlanId={gameTimePlanId}");
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadAsStringAsync();
            if (errorResult != null)
            {
                return new V3Result<V1GameTimePlan>(errorResult);
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V1GameTimePlan> object.");
                return new V3Result<V1GameTimePlan>("Ran into an unknown issue when deleting gametimeplan.");
            }
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<V3Result<V1GameTimePlan>>();
        }
    }

    public async Task<V3Result<List<int>>> GetGameSuggestionAsync(int groupId)
    {
        var response = _httpClient.GetAsync($"{GameTimePlannerApiBase}/V1GameTimePlan/GameSuggestion?groupId={groupId}");
        if (!response.Result.IsSuccessStatusCode)
        {
            var errorResult = await response.Result.Content.ReadAsStringAsync();
            if (errorResult != null)
            {
                return new V3Result<List<int>>(errorResult);
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<List<int>> object.");
                return new V3Result<List<int>>("Ran into an unknown issue when getting game suggestions.");
            }
        }
        else
        {
            return await response.Result.Content.ReadFromJsonAsync<V3Result<List<int>>>();
        }
    }
}
