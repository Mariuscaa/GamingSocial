using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using HIOF.GamingSocial.GameInformation.Data;
using HIOF.GamingSocial.GameInformation.Protos;
using static HIOF.GamingSocial.GameInformation.Protos.VideoGameService;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.Collections;
using Grpc.Net.Client;
using HIOF.GamingSocial.PublicGameInformation.Protos;
using Microsoft.IdentityModel.Tokens;

namespace HIOF.GamingSocial.GameInformation.Services;

/// <summary>
/// Service for handling calling publicGameInformation and saving the result to the database.
/// </summary>
public class VideoGameService : VideoGameServiceBase
{
    private readonly ILogger<VideoGameService> _logger;
    private readonly VideoGameDbContext _db;

    /// <summary>
    /// Constructor for VideoGameService.
    /// </summary>
    /// <param name="logger">Default logger parameter</param>
    /// <param name="dbContext">Default dbContext parameter</param>
    public VideoGameService(ILogger<VideoGameService> logger, VideoGameDbContext dbContext)
    {
        _logger = logger;
        _db = dbContext;
    }

    /// <summary>
    /// Method for updating the database with the latest games from the publicGameInformation service.
    /// </summary>
    /// <param name="request">Requires an empty request object by using the EmptyRequest class.</param>
    /// <param name="context">Default ServerCallContext which is handled automaticly.</param>
    /// <returns>A ProtoResult object with either values, error or an okMessage.</returns>
    public async override Task<Sharedprotos.ProtoResult> UpdateGameDatabase(Sharedprotos.EmptyRequest request, ServerCallContext context)
    {
        var channel = GrpcChannel.ForAddress("https://localhost:7250");
        var client = new PublicSteamGamesService.PublicSteamGamesServiceClient(channel);

        try
        {
            var response = await client.GetPublicGamesAsync(new Sharedprotos.EmptyRequest());
            if (response.ResultCase == Sharedprotos.ProtoResult.ResultOneofCase.Value)
            {
                if (response.Value.Value.Is(Sharedprotos.OkMessage.Descriptor))
                {
                    var okMessage = response.Value.Value.Unpack<Sharedprotos.OkMessage>();
                    Console.WriteLine(okMessage.TextMessage);

                    return new Sharedprotos.ProtoResult
                    {
                        Value = new Sharedprotos.ProtoResult.Types.ResultValue
                        {
                            Value = Any.Pack(okMessage)
                        }
                    };
                }
                else
                {
                    var steamGames = response.Value.Value.Unpack<AllSteamGamesResponse>();
                    if (steamGames.Games.IsNullOrEmpty())
                    {
                        Sharedprotos.ProtoResult resultWithError = new Sharedprotos.ProtoResult
                        {
                            Errors = { "An error occurred. Returned 0 games." }
                        };
                        _logger.LogWarning("Result with error: " + resultWithError);

                        return resultWithError;
                    }

                    var existingGames = await _db.VideoGameInformation
                        .Where(g => g.SteamAppId != null)
                        .ToDictionaryAsync(g => g.SteamAppId ?? 0);

                    var newGames = new List<VideoGameInformation>();
                    foreach (var game in steamGames.Games)
                    {
                        // Tries to get the object with the corresponding app id to check if the game is already in the database.
                        // If it is not in the database, it will get added.
                        if (!existingGames.TryGetValue(game.Appid, out var existingGame))
                        {
                            existingGame = new VideoGameInformation();
                            existingGame.SteamAppId = game.Appid;
                            _db.VideoGameInformation.Add(existingGame);
                            newGames.Add(existingGame);
                        }
                        game.Name = game.Name.Length > 200 ? game.Name.Substring(0, 200) : game.Name;
                        existingGame.GameTitle = game.Name;
                    }
                    await _db.SaveChangesAsync();
                    _logger.LogInformation("GRPC: Number of games added: " + newGames.Count);

                    if (newGames.Count < 1000)
                    {
                        RepeatedField<ProtoVideoGameInformation> videoGames = new RepeatedField<ProtoVideoGameInformation>();

                        foreach (var game in newGames)
                        {
                            videoGames.Add(new ProtoVideoGameInformation
                            {
                                Id = game.GameId,
                                GameTitle = game.GameTitle,
                                SteamAppId = (int)game.SteamAppId
                            });
                        }

                        // Add the list of video game information to ProtoVideoGameInformationList
                        ProtoVideoGameInformationList gameList = new ProtoVideoGameInformationList();
                        gameList.VideoGameInformation.AddRange(videoGames);

                        return new Sharedprotos.ProtoResult
                        {
                            Value = new Sharedprotos.ProtoResult.Types.ResultValue
                            {
                                Value = Any.Pack(gameList)
                            }
                        };
                    }
                    else
                    {
                        return new Sharedprotos.ProtoResult
                        {
                            Value = new Sharedprotos.ProtoResult.Types.ResultValue
                            {
                                Value = Any.Pack(new Sharedprotos.OkMessage { TextMessage = $"Completed ok, games have been added to database. Too many results to return list. Number of games added: " + newGames.Count })
                            }
                        };
                    }
                }
            }
            else if (response.ResultCase == Sharedprotos.ProtoResult.ResultOneofCase.Error)
            {
                _logger.LogWarning("Error: " + response.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex + ex.StackTrace);
            Sharedprotos.ProtoResult resultWithError = new Sharedprotos.ProtoResult
            {
                Errors = { $"Error: {ex.Message}" }
            };

            return resultWithError;
        }


        Sharedprotos.ProtoResult resultWithError1 = new Sharedprotos.ProtoResult
        {
            Errors = { "Unknown error" }
        };
        _logger.LogWarning("Result with error: " + resultWithError1);

        return resultWithError1;
    }
}
