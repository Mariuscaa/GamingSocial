using HIOF.GamingSocial.PublicGameInformation.Model.V1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HIOF.GamingSocial.PublicGameInformation.Model.V2;
using Microsoft.Extensions.Options;
using HIOF.GamingSocial.PublicGameInformation.Configuration;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace HIOF.GamingSocial.PublicGameInformation.Controllers.V2;

/// <summary>
/// Handles communication with the steam web api.
/// </summary>
[ApiController]
[Route("V2/PublicGameInformation")]
public class V2SteamGamesController : ControllerBase
{
    private readonly ILogger<V2SteamGamesController> _logger;
    private readonly IOptions<KeyServiceSettings> _KeyServiceSettings;
    private readonly IOptions<UrlServiceSettings> _UrlServiceSettings;

    /// <summary>
    /// Initializes a new instance of the V2SteamGamesController class.
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="keyServiceSettings">IOptions implementation of KeyServiceSettings file</param>
    /// <param name="urlServiceSettings">IOptions implementation of UrlServiceSettings file</param>
    public V2SteamGamesController(ILogger<V2SteamGamesController> logger,
        IOptions<KeyServiceSettings> keyServiceSettings,
        IOptions<UrlServiceSettings> urlServiceSettings)
    {
        _logger = logger;
        _KeyServiceSettings = keyServiceSettings;
        _UrlServiceSettings = urlServiceSettings;
    }

    /// <summary>
    /// BEWARE THIS ENDPOINT CRASHES IN SWAGGER.
    /// Gets information about all the videogames currently available at Steam.
    /// </summary>
    /// <returns>A list of a videogame objects with an app id, game title and last modified date.</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(V2Result<V2SteamGames>), 200)]
    [ProducesResponseType(typeof(V2Result<V2SteamGames>), 401)]
    [ProducesResponseType(typeof(V2Result<V2SteamGames>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V2Result<V2SteamGames>>> AllSteamGames()
    {
        int maxResultsPerRequest = 50000;
        using var client = new HttpClient();
        var key = _KeyServiceSettings.Value.KeySetting1;
        var apiUrlFormat1 = _UrlServiceSettings.Value.UrlSetting1;
        var apiUrlFormat2 = _UrlServiceSettings.Value.UrlSetting2;

        var steamApiUrl = string.Format(apiUrlFormat1, key, maxResultsPerRequest);

        try
        {
            var responseTest = await client.GetAsync(steamApiUrl);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, $"Server could not connect to Steam API.");
            return StatusCode(StatusCodes.Status500InternalServerError, new V2Result<V2SteamGames>("Server could not connect to Steam API."));
        }

        var response = await client.GetAsync(steamApiUrl);

        switch (response.StatusCode)
        {
            case System.Net.HttpStatusCode.OK:
                var responseString = await response.Content.ReadAsStringAsync();

                V2SteamGamesData? steamGamesData =
                    JsonConvert.DeserializeObject<V2SteamGamesData>(responseString);

                V2SteamGames steamGames = new V2SteamGames()
                {
                    games = new List<V2SteamGames.V2Game>()
                };

                if (steamGamesData != null)
                {
                    foreach (var game in steamGamesData.response.apps)
                    {
                        steamGames.games.Add(new V2SteamGames.V2Game()
                        {
                            appid = game.appid,
                            last_modified = game.last_modified,
                            name = game.name
                        });
                    }
                    if (steamGamesData.response.have_more_results)
                    {
                        steamApiUrl = string.Format(apiUrlFormat2, key, steamGamesData.response.last_appid, maxResultsPerRequest);

                        response = await client.GetAsync(steamApiUrl);
                        responseString = await response.Content.ReadAsStringAsync();

                        steamGamesData =
                            JsonConvert.DeserializeObject<V2SteamGamesData>(responseString);

                        if (steamGamesData != null)
                        {
                            foreach (var game in steamGamesData.response.apps)
                            {
                                steamGames.games.Add(new V2SteamGames.V2Game()
                                {
                                    appid = game.appid,
                                    last_modified = game.last_modified,
                                    name = game.name
                                });
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Failed to get more games even if previous response said it had more games.");
                        }
                    }
                }
                else
                {
                    _logger.LogError("Steam response code was ok, but response was not as expected.");
                }

                _logger.LogInformation($"Steam was called successfully. Returned {steamGames.games.Count} games.");
                return Ok(new V2Result<V2SteamGames>(steamGames));

            case System.Net.HttpStatusCode.NotFound:
                _logger.LogError($"Page was not found. Something wrong with URL.");
                return NotFound(new V2Result<V2SteamGames>("Page was not found. Something wrong with URL. Contact API administrator."));

            case System.Net.HttpStatusCode.Unauthorized:
                _logger.LogError($"Steam did not authorize the request.");
                return Unauthorized(new V2Result<V2SteamGames>("Steam did not authorize the request. Contact API administrator."));
        }
        _logger.LogError($"Unexpected error. Reason: unhandled http status code in response from steam.");
        return new V2Result<V2SteamGames>("Unknown error.");
    }

    /// <summary>
    /// Requests information about a specific game from the Steam API.
    /// </summary>
    /// <param name="steamAppId">The Steam app id for the game.</param>
    /// <returns>An object with some properties found on steam like description, genres, etc.</returns>
    [HttpGet("{steamAppId}")]
    [ProducesResponseType(typeof(V2Result<V2SteamGameDetails>), 200)]
    [ProducesResponseType(typeof(V2Result<V2SteamGameDetails>), 401)]
    [ProducesResponseType(typeof(V2Result<V2SteamGameDetails>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V2Result<V2SteamGameDetails>>> GetGameDetails(int steamAppId)
    {
        var steamAppDetailsUrlFormat = _UrlServiceSettings.Value.UrlSetting3;

        var url = string.Format(steamAppDetailsUrlFormat, steamAppId);

        using var client = new HttpClient();
        var response = await client.GetAsync(url);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning($"Page was not found. Something wrong with URL.");
            return NotFound(new V2Result<V2SteamGameDetails>("Page was not found. Something wrong with URL. Contact API administrator."));
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogError($"Steam did not authorize the request.");
            return Unauthorized(new V2Result<V2SteamGameDetails>("Steam did not authorize the request. Contact API administrator."));
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            _logger.LogError($"Server could not connect to Steam API.");
            return StatusCode(StatusCodes.Status500InternalServerError, new V2Result<V2SteamGameDetails>("Server could not connect to Steam API."));
        }

        var gameDetailsData = await response.Content.ReadFromJsonAsync<Dictionary<string, GameDetails>>();

        if (gameDetailsData == null)
        {
            _logger.LogError($"Unexpected error. Reason: gameDetailsData is null.");
            return StatusCode(StatusCodes.Status500InternalServerError, new V2Result<V2SteamGameDetails>("Unexpected error. Reason: Steam response was not as expected."));
        }
        if (gameDetailsData.Values.First().success == false)
        {
            _logger.LogWarning($"Game with steam app id '{steamAppId}' was not found.");
            return NotFound(new V2Result<V2SteamGameDetails>($"Game with steam app id '{steamAppId}' was not found."));
        }

        // Release date can be something other than a date (like "coming soon"). 
        DateTime releaseDate;
        if (DateTime.TryParse(gameDetailsData.Values.First().data.release_date.date, out releaseDate))
        {
            var gameDetails = new V2SteamGameDetails()
            {
                Title = gameDetailsData.Values.First().data.name,
                SteamAppId = gameDetailsData.Values.First().data.steam_appid,
                AboutTheGame = gameDetailsData.Values.First().data.about_the_game,
                ShortDescription = gameDetailsData.Values.First().data.short_description,
                HeaderImageUrl = gameDetailsData.Values.First().data.header_image,
                BackgroundImageUrl = gameDetailsData.Values.First().data.background_raw,
                ReleaseDate = releaseDate,
                Genres = new List<string>()
            };
            foreach (var genre in gameDetailsData.Values.First().data.genres)
            {
                gameDetails.Genres.Add(genre.description);
            }
            return Ok(new V2Result<V2SteamGameDetails>(gameDetails));
        }
        else
        {
            var gameDetails = new V2SteamGameDetails()
            {
                Title = gameDetailsData.Values.First().data.name,
                SteamAppId = gameDetailsData.Values.First().data.steam_appid,
                AboutTheGame = gameDetailsData.Values.First().data.about_the_game,
                ShortDescription = gameDetailsData.Values.First().data.short_description,
                HeaderImageUrl = gameDetailsData.Values.First().data.header_image,
                BackgroundImageUrl = gameDetailsData.Values.First().data.background_raw,
                ReleaseDate = null,
                Genres = new List<string>()
            };
            foreach (var genre in gameDetailsData.Values.First().data.genres)
            {
                gameDetails.Genres.Add(genre.description);
            }
            return Ok(new V2Result<V2SteamGameDetails>(gameDetails));
        }
    }
}