using HIOF.GamingSocial.GUI.Model.ProfileInformation;
using HIOF.GamingSocial.GUI.Model;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;


namespace HIOF.GamingSocial.GUI.Services;
public class UserService
{
    private readonly ILogger<UserService> _logger;
    private readonly HttpClient _httpClient;
    private string ProfileInformationApiBase = "https://localhost:7087";

    public UserService(ILogger<UserService> logger, HttpClient httpClient, GroupService groupService)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<V3Result<V3PostProfile>> CreateProfileAsync(V3PostProfile postProfile)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ProfileInformationApiBase}/V3/Profile", postProfile);

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3PostProfile>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            _logger.LogError("Failed to deserialize the response body into a V3Result<V3PostProfile> object.");
            return new V3Result<V3PostProfile>("Ran into an unknown issue when creating a profile.");
        }

        return await response.Content.ReadFromJsonAsync<V3Result<V3PostProfile>>();
    }

    public async Task<V3Result<V3Profile>> LoginAsync(string username)
    {
        var response = await _httpClient.GetAsync($"{ProfileInformationApiBase}/V3/Profile/UserName?UserName={username}");
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3Profile>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3Profile> object.");
                return new V3Result<V3Profile>("Ran into an unknown issue when getting user info.");
            }
        }
        return await response.Content.ReadFromJsonAsync<V3Result<V3Profile>>();
    }


    public async Task<V3Result<V3Profile>> CheckUserAsync(Guid profileGuid)
    {
        var response = await _httpClient.GetAsync($"{ProfileInformationApiBase}/V3/Profile/{profileGuid}");

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3Profile>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3Profile> object.");
                return new V3Result<V3Profile>("Ran into an unknown issue when checking single user.");
            }
        }
        return await response.Content.ReadFromJsonAsync<V3Result<V3Profile>>();

    }

    public async Task<V3Result<V3Profile>> UpdateProfileAsync(V3PatchProfile profile, Guid profileGuid)
    {
        var json = JsonSerializer.Serialize(profile);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync($"{ProfileInformationApiBase}/V3/Profile/{profileGuid}", content);
        if (response.IsSuccessStatusCode)
        {
            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<V3Result<V3Profile>>(resultJson);
            if (result != null)
            {
                return result;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3Profile> object.");
                return new V3Result<V3Profile>("Ran into an unknown issue when editing a profile.");
            }
        }
        else
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3Profile>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3Profile> object.");
                return new V3Result<V3Profile>("Ran into an unknown issue when editing a profile.");
            }
        }
    }

    public async Task<V3Result<List<V3Profile>>> GetProfilesAsync(string searchQuery)
    {
        var response = await _httpClient.GetAsync($"{ProfileInformationApiBase}/V3/Profile?userName={searchQuery}");

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<List<V3Profile>>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<List<V3Profile>> object.");
                return new V3Result<List<V3Profile>>("Ran into an unknown issue when getting profiles.");
            }
        }
        return await response.Content.ReadFromJsonAsync<V3Result<List<V3Profile>>>();
    }
    
    
    public async Task<V3Result<V3Profile>> DiscoverNewProfileAsync(Guid profileGuid, int? specificGame, bool? gamesInCommon)
    {
        string url;
        if (specificGame == 0)
        {
            url = $"{ProfileInformationApiBase}/V3/Profile/ProfileDiscovery?profileGuid={profileGuid}&" +
            $"gamesInCommon={gamesInCommon}";
        }
        else
        {
            url = $"{ProfileInformationApiBase}/V3/Profile/ProfileDiscovery?profileGuid={profileGuid}&" +
            $"gamesInCommon={gamesInCommon}&specificGame={specificGame}";
        }
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3Profile>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<Guid> object.");
                return new V3Result<V3Profile>("Ran into an unknown issue peforming profile discovery.");
            }
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<V3Result<V3Profile>>();
        }
    }
}
