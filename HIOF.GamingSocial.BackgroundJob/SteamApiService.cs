namespace BackgroundJob;

public class SteamApiService
{
    private readonly HttpClient _httpClient;

    public SteamApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> TriggerGameListUpdateAsync()
    {
        // Added delay to avoid issues when running upon start or restart. 
        // This is because the API is not ready to receive requests when the background job starts on slower machines.
        await Task.Delay(40000);
        var response = await _httpClient.PostAsync("/V3/VideoGameInformation/UpdateGameList", null);
        return response;
    }
}
