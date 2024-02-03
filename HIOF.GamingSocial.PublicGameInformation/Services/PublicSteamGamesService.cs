using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using HIOF.GamingSocial.PublicGameInformation.Configuration;
using Microsoft.Extensions.Options;
using HIOF.GamingSocial.PublicGameInformation.Protos;
using static HIOF.GamingSocial.PublicGameInformation.Protos.PublicSteamGamesService;
using Newtonsoft.Json;
using HIOF.GamingSocial.PublicGameInformation.Model.V2;

namespace HIOF.GamingSocial.PublicGameInformation.Services;

/// <summary>
/// Handles the retrieval of videogames from the public steam API.
/// </summary>
public class PublicSteamGamesService : PublicSteamGamesServiceBase
{
    private readonly ILogger<PublicSteamGamesService> _logger;
    private readonly IOptions<KeyServiceSettings> _KeyServiceSettings;
    private readonly IOptions<UrlServiceSettings> _UrlServiceSettings;

    /// <summary>
    /// Constructor for PublicSteamGamesService.
    /// </summary>
    /// <param name="logger">Default logger.</param>
    /// <param name="keyServiceSettings">Gets keys from app settings.</param>
    /// <param name="urlServiceSettings">Gets urls from app settings.</param>
    public PublicSteamGamesService(ILogger<PublicSteamGamesService> logger,
        IOptions<KeyServiceSettings> keyServiceSettings,
        IOptions<UrlServiceSettings> urlServiceSettings)
    {
        _logger = logger;
        _KeyServiceSettings = keyServiceSettings;
        _UrlServiceSettings = urlServiceSettings;
    }

    /// <summary>
    /// Gets all public games from the steam API.
    /// </summary>
    /// <param name="request">Empty request.</param>
    /// <param name="context">An object that holds all the contextual information about the ongoing call.</param>
    /// <returns>A list of all the games in a ProtoResult object.</returns>
    public override async Task<Sharedprotos.ProtoResult> GetPublicGames(Sharedprotos.EmptyRequest request, ServerCallContext context)
    {
        int maxResultsPerRequest = 50000;
        using var client = new HttpClient();
        var key = _KeyServiceSettings.Value.KeySetting1;
        var apiUrlFormat1 = _UrlServiceSettings.Value.UrlSetting1;
        var apiUrlFormat2 = _UrlServiceSettings.Value.UrlSetting2;

        var SteamApiUrl = string.Format(apiUrlFormat1, key, maxResultsPerRequest);

        try
        {
            var responseTest = await client.GetAsync(SteamApiUrl);
        }

        catch (Exception ex)
        {
            Sharedprotos.ProtoResult resultWithError = new Sharedprotos.ProtoResult
            {
                Errors = { "Error in getting async response." }
            };
            _logger.LogError(ex, $"Server could not connect to Steam API.");
            return resultWithError;
        }

        var response = await client.GetAsync(SteamApiUrl);

        switch (response.StatusCode)
        {
            case System.Net.HttpStatusCode.OK:
                var responseString = await response.Content.ReadAsStringAsync();

                V2SteamGamesData? steamGamesData =
                    JsonConvert.DeserializeObject<V2SteamGamesData>(responseString);

                AllSteamGamesResponse steamGames = new AllSteamGamesResponse()
                {
                };

                foreach (var game in steamGamesData.response.apps)
                {
                    steamGames.Games.Add(new Game()
                    {
                        Appid = game.appid,
                        LastModified = game.last_modified,
                        Name = game.name
                    });
                }

                if (steamGamesData.response.have_more_results)
                {
                    SteamApiUrl = string.Format(apiUrlFormat2, key, steamGamesData.response.last_appid, maxResultsPerRequest);

                    response = await client.GetAsync(SteamApiUrl);
                    responseString = await response.Content.ReadAsStringAsync();

                    steamGamesData =
                        JsonConvert.DeserializeObject<V2SteamGamesData>(responseString);

                    foreach (var game in steamGamesData.response.apps)
                    {
                        steamGames.Games.Add(new Game()
                        {
                            Appid = game.appid,
                            LastModified = game.last_modified,
                            Name = game.name
                        });
                    }
                }
                _logger.LogInformation($"Steam was called successfully. Returned {steamGames.Games.Count} games.");
                return new Sharedprotos.ProtoResult
                {
                    Value = new Sharedprotos.ProtoResult.Types.ResultValue
                    {
                        Value = Any.Pack(steamGames)
                    }
                };

            case System.Net.HttpStatusCode.NotFound:
                Sharedprotos.ProtoResult resultWithError = new Sharedprotos.ProtoResult
                {
                    Errors = { "Not Found" }
                };
                _logger.LogError($"Page was not found. Something wrong with URL.");
                return resultWithError;

            case System.Net.HttpStatusCode.Unauthorized:
                Sharedprotos.ProtoResult resultWithError1 = new Sharedprotos.ProtoResult
                {
                    Errors = { "Unauthorized" }
                };
                _logger.LogError($"Steam did not authorize the request.");
                return resultWithError1;
        }
        Sharedprotos.ProtoResult resultWithError2 = new Sharedprotos.ProtoResult
        {
            Errors = { "Unknown error" }
        };
        _logger.LogError($"Unexpected error. Reason: unhandled http status code in response from steam.");
        return resultWithError2;
    }
}
