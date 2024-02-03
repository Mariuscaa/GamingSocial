using HIOF.GamingSocial.GameInformation.Models.V3;
using HIOF.GamingSocial.GUI.Model;
using HIOF.GamingSocial.GUI.Model.GameInformation;
using HIOF.GamingSocial.GameInformation.Protos;
using Grpc.Net.Client;

namespace HIOF.GamingSocial.GUI.Services;

public class GameService
{
    private readonly ILogger<GameService> _logger;
    private readonly HttpClient _httpClient;
    private string GameInformationApiBase = "https://localhost:7296";

    public GameService(ILogger<GameService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<V3Result<List<GameRatingWithName>>> PostSteamGamesForProfileAsync(Guid profileGuid, string steamId)
    {
        var response = await _httpClient.PostAsync($"{GameInformationApiBase}/V3/GameCollection/GameCollectionUpdate?profileGuid={profileGuid}&steamId={steamId}", null);

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<ProfileWithGames>>();
            if (errorResult != null)
            {
                _logger.LogWarning($"Error adding games to collection " + string.Join(", ", errorResult.Errors));
                return new V3Result<List<GameRatingWithName>>($"Error while adding games to collection: {errorResult.Errors[0]}");
            }
            else
            {
                _logger.LogError($"Failed to deserialize response.");
                return new V3Result<List<GameRatingWithName>>($"Unexpected error while adding games to collection.");
            }
        }
        var profileWithSteamGamesResult = await response.Content.ReadFromJsonAsync<V3Result<ProfileWithGames>>();

        var games = new List<GameRatingWithName>();
        foreach (var game in profileWithSteamGamesResult.Value.GamesCollection)
        {
            var gameResponse = await _httpClient.GetAsync($"{GameInformationApiBase}/V3/VideoGameInformation/{game.GameId}");
            if (!gameResponse.IsSuccessStatusCode)
            {
                var errorResult = await gameResponse.Content.ReadFromJsonAsync<V3Result<V3VideoGameInformation>>();
                if (errorResult != null)
                {
                    _logger.LogWarning($"Error adding game to collection " + string.Join(", ", errorResult.Errors));
                }
                else
                {
                    _logger.LogError($"Failed to deserialize response: {gameResponse.Content.ToString}");
                }
            }
            var gameResult = await gameResponse.Content.ReadFromJsonAsync<V3Result<V3VideoGameInformation>>();
            games.Add(new GameRatingWithName()
            {
                GameId = gameResult.Value.Id,
                GameRating = game.GameRating,
                GameTitle = gameResult.Value.GameTitle
            });
        }

        return new V3Result<List<GameRatingWithName>>(games);
    }

    public async Task<V3Result<V3VideoGameInformation>> GetSingleGameAsync(int gameId)
    {
        var response = await _httpClient.GetAsync($"{GameInformationApiBase}/V3/VideoGameInformation/{gameId}");
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3VideoGameInformation>>();
            if (errorResult != null)
            {
                _logger.LogWarning($"Error getting game {gameId}: " + string.Join(", ", errorResult.Errors));
                return errorResult;
            }
            else
            {
                _logger.LogWarning($"Failed to deserialize response for game id: {gameId}");
                return new V3Result<V3VideoGameInformation>("Unknown error occurred while getting game.");
            }
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<V3Result<V3VideoGameInformation>>();
        }
    }

    public async Task<List<GameRatingWithName>> GetGamesForProfileAsync(Guid profileGuid)
    {
        // Replace with actual API base addresses.
        string gameCollectionApiBase = "https://localhost:7296";

        // Call the Profile endpoint to get the logged in user's games.           
        var profileWithGamesResponse = await _httpClient.GetAsync($"{gameCollectionApiBase}/V3/GameCollection/Profile?profileGuid={profileGuid}&onlyRatedGames=false");

        if (!profileWithGamesResponse.IsSuccessStatusCode)
        {
            if (profileWithGamesResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var errorResult = await profileWithGamesResponse.Content.ReadFromJsonAsync<V3Result<ProfileWithGames>>();
                if (errorResult == null)
                {
                    _logger.LogWarning($"Failed to deserialize response for guid: {profileGuid}");
                    return new List<GameRatingWithName>();

                }
                _logger.LogWarning($"Error getting games for profile {profileGuid}: " + string.Join(", ", errorResult.Errors));
                return new List<GameRatingWithName>();
            }
        }

        var profileWithGamesResult = await profileWithGamesResponse.Content.ReadFromJsonAsync<V3Result<ProfileWithGames>>();

        var games = new List<GameRatingWithName>();
        foreach (var game in profileWithGamesResult.Value.GamesCollection)
        {
            var gameResponse = await _httpClient.GetAsync($"{gameCollectionApiBase}/V3/VideoGameInformation/{game.GameId}");

            if (!gameResponse.IsSuccessStatusCode)
            {
                var errorResult = await gameResponse.Content.ReadFromJsonAsync<V3Result<V3VideoGameInformation>>();
                if (errorResult != null)
                {
                    _logger.LogWarning("Ran into an issue while getting game: " + errorResult.Errors[0]);
                }
                else
                {
                    _logger.LogWarning("Ran into an unexpected issue while deserializing game. Id: " + game.GameId);
                }
            }
            var gameResult = await gameResponse.Content.ReadFromJsonAsync<V3Result<V3VideoGameInformation>>();

            games.Add(new GameRatingWithName()
            {
                GameId = gameResult.Value.Id,
                GameRating = game.GameRating,
                GameTitle = gameResult.Value.GameTitle
            });
        }

        return new List<GameRatingWithName>(games);
    }

    public async Task<V3Result<V3PostGameCollection>> AddGameToCollection(Guid profileGuid, int gameId)
    {
        var postGameCollection = new V3PostGameCollection()
        {
            GameIds = new List<int> { gameId },
            ProfileId = profileGuid
        };

        var response = await _httpClient.PostAsJsonAsync($"{GameInformationApiBase}/V3/GameCollection", postGameCollection);

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3PostGameCollection>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize response from adding game to collection.");
                return new V3Result<V3PostGameCollection>("Ran into an unexpected issue while adding game.");
            }
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<V3Result<V3PostGameCollection>>();
        }
    }

    public async Task<V3Result<V3ProfileGameCollection>> RemoveGameFromCollection(Guid profileGuid, int gameId)
    {
        var response = await _httpClient.DeleteAsync(
            $"{GameInformationApiBase}/V3/GameCollection?profileGuid={profileGuid}&gameId={gameId}");

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3ProfileGameCollection>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize response from game removal from collection.");
                return new V3Result<V3ProfileGameCollection>("Ran into an unexpected issue removing game.");
            }
        }
        return await response.Content.ReadFromJsonAsync<V3Result<V3ProfileGameCollection>>();
    }
    public async Task<V3Result<bool>> CheckIfGameIsInCollection(Guid profileGuid, int gameId)
    {
        var response = await _httpClient.GetAsync(
                       $"{GameInformationApiBase}/V3/GameCollection/CollectionCheck?profileGuid={profileGuid}&gameId={gameId}");

        if(!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<bool>>();
            if (errorResult != null)
            {
                return errorResult;
            }

            else
            {
                _logger.LogError("Failed to deserialize response from game collection check.");
                return new V3Result<bool>("Ran into an unexpected issue checking game.");
            }
        }
        return await response.Content.ReadFromJsonAsync<V3Result<bool>>();
    }


    public async Task<V3Result<List<V3VideoGameInformation>>> Search(string searchQuery)
    {
        var response = await _httpClient.GetAsync(
                       $"{GameInformationApiBase}/V3/VideoGameInformation?searchString={searchQuery}&includeSimilar=true");
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<List<V3VideoGameInformation>>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize response from game collection check.");
                return new V3Result<List<V3VideoGameInformation>>("Ran into an unexpected issue checking game.");
            }
        }
        return await response.Content.ReadFromJsonAsync<V3Result<List<V3VideoGameInformation>>>();
    }

    public async Task<List<V3VideoGameInformation>> UpdateGameListAsync()
    {
        var channel = GrpcChannel.ForAddress("https://localhost:7296");
        var client = new VideoGameService.VideoGameServiceClient(channel);
        var returnList = new List<V3VideoGameInformation>();

        try
        {
            var response = await client.UpdateGameDatabaseAsync(new Sharedprotos.EmptyRequest());

            if (response.ResultCase == Sharedprotos.ProtoResult.ResultOneofCase.Value)
            {
                if (response.Value.Value.Is(Sharedprotos.OkMessage.Descriptor))
                {
                    var okMessage = response.Value.Value.Unpack<Sharedprotos.OkMessage>();
                    _logger.LogInformation(okMessage.TextMessage);
                    return returnList;
                }

                else
                {
                    var gameInformation = response.Value.Value.Unpack<ProtoVideoGameInformationList>();

                    foreach (var game in gameInformation.VideoGameInformation)
                    {
                        Console.WriteLine($"Game {game.Id}: {game.GameTitle}");
                        returnList.Add(new V3VideoGameInformation()
                        {
                            Id = game.Id,
                            GameTitle = game.GameTitle,
                            GameDescription = game.GameDescription,
                            SteamAppId = game.SteamAppId,
                            GiantbombGuid = game.GiantbombGuid
                        });
                    }
                    return returnList;
                }
            }
            else if (response.ResultCase == Sharedprotos.ProtoResult.ResultOneofCase.Error)
            {
                _logger.LogError($"Error: {response.Error}");
                return returnList;
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return returnList;
        }
        return returnList;

    }
}
