using HIOF.GamingSocial.GUI.Model;
using HIOF.GamingSocial.GUI.Model.Chat;

namespace HIOF.GamingSocial.GUI.Services;

public class ChatAPIService
{
    private readonly ILogger<ChatAPIService> _logger;
    private readonly HttpClient _httpClient;
    private string ChatApiBase = "https://localhost:7192";
    public ChatAPIService(ILogger<ChatAPIService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<V3Result<PostChatMessage>> CreateMessageAsync(PostChatMessage? postChat)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ChatApiBase}/V1/Chat", postChat);
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<PostChatMessage>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            _logger.LogError("Failed to deserialize the response body into a V3Result<PostChatMessage> object.");
            return new V3Result<PostChatMessage>("Ran into an unknown issue when creating a profile.");
        }

        return await response.Content.ReadFromJsonAsync<V3Result<PostChatMessage>>();

    }



    public async Task<V3Result<List<GetChatMessage>>> UserLoadMessageAsync(Guid ProfileGuid1, Guid ProfileGuid2)
    {
        var response = await _httpClient.GetAsync($"{ChatApiBase}/V1/Chat?profileGuid1={ProfileGuid1}&profileGuid2={ProfileGuid2}");

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<List<GetChatMessage>>>();
            if (errorResult != null)
            {

                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<List<GetChatMessage>> object.");
                return new V3Result<List<GetChatMessage>>("Ran into an unknown issue when getting profiles.");
            }
        }
        return await response.Content.ReadFromJsonAsync<V3Result<List<GetChatMessage>>>();
    }

    public async Task<V3Result<List<GetChatMessage>>> LoadGroupSenderMessageAsync(int room)
    {
        var response = await _httpClient.GetAsync($"{ChatApiBase}/V1/Chat?room={room}");
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<List<GetChatMessage>>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<List<GetChatMessage>> object.");
                return new V3Result<List<GetChatMessage>>("Ran into an unknown issue when getting profiles.");
            }
        }
        return await response.Content.ReadFromJsonAsync<V3Result<List<GetChatMessage>>>();

    }

}