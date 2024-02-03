using HIOF.GamingSocial.GUI.Model;
using HIOF.GamingSocial.GUI.Model.ProfileInformation;

namespace HIOF.GamingSocial.GUI.Services;

public class FriendService
{
    private readonly ILogger<FriendService> _logger;
    private readonly HttpClient _httpClient;
    private string ProfileInformationApiBase = "https://localhost:7087";
    public FriendService(ILogger<FriendService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<V3Result<List<Guid>>> CheckFriendsAsync(Guid profileGuid)
    {
        var response = await _httpClient.GetAsync($"{ProfileInformationApiBase}/V3/Friend?profileGuid={profileGuid}");
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<List<Guid>>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize response from friends check.");
                return new V3Result<List<Guid>>("Failed to get friends.");
            }
        }
        return await response.Content.ReadFromJsonAsync<V3Result<List<Guid>>>();
    }

    public async Task<V3Result<bool>> CheckIfFriendsAsync(Guid profileGuid1, Guid profileGuid2)
    {
        var response = await _httpClient.GetAsync(
            $"{ProfileInformationApiBase}/V3/Friend/FriendCheck?profileGuid1={profileGuid1}&profileGuid2={profileGuid2}");

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<bool>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize response from friend check.");
                return new V3Result<bool>("Failed to check if friends.");
            }
        }
        return await response.Content.ReadFromJsonAsync<V3Result<bool>>();
    }
    public async Task<V3Result<V3Friend>> RemoveFriendAsync(Guid profileGuid1, Guid profileGuid2)
    {
        var response = await _httpClient.DeleteAsync($"{ProfileInformationApiBase}/V3/Friend?profileGuid1={profileGuid1}&profileGuid2={profileGuid2}");
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3Friend>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3Friend> object.");
                return new V3Result<V3Friend>("Ran into an unknown issue while deleting friendship.");
            }
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<V3Result<V3Friend>>();
        }
    }

    public async Task<V3Result<V3Friend>> AddToFriendsList(Guid profileGuid1, Guid profileGuid2)
    {
        var response = await _httpClient.PostAsync(
            $"{ProfileInformationApiBase}/V3/Friend?profileGuid1={profileGuid1}&profileGuid2={profileGuid2}", null);
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3Friend>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3Friend> object.");
                return new V3Result<V3Friend>("Ran into an unknown issue when adding friend.");
            }
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<V3Result<V3Friend>>();
        }
    }
}
