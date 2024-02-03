using HIOF.GamingSocial.PublicProfileInformation.Configuration;
using HIOF.GamingSocial.PublicProfileInformation.Models.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;

namespace HIOF.GamingSocial.PublicProfileInformation.Controllers.V1;

/// <summary>
/// This controller will retrieve a list of games owned by an individual Steam user.
/// The controller will call the GetOwnedGames method of the Steam Web API.
/// Needed input is a valid Steam Key and a valid Steam ID. 
/// Output is a list of games represented by title, id, time spent playing and a hash for a game icon-file.
/// The controller will also output descriptive error messages where applicable. 
/// </summary>
[ApiController]
[Route("V1/SteamGameCollection")]
public class SteamGameCollectionController : ControllerBase
{
    private readonly ILogger<SteamGameCollectionController> _logger;
    private readonly IOptions<KeyServiceSettings> _KeyServiceSettings;

    /// <summary>
    /// Constructor for Steam game collection controller.
    /// </summary>
    /// <param name="logger">Logger instance for the controller.</param>
    /// <param name="keyServiceSettings">Settings to be used for Key service.</param>
    public SteamGameCollectionController(ILogger<SteamGameCollectionController> logger,
        IOptions<KeyServiceSettings> keyServiceSettings)
    {
        _logger = logger;
        _KeyServiceSettings = keyServiceSettings;
    }

    /// <summary>
    /// Gets all the games a user has stored on a specific Steam ID (SID).
    /// Games are represented as objects that hold the properties: appid, name, playtime_forever and img_icon_url. 
    /// </summary>
    /// <param name="steamId">steamId (SID) represents an ID for one individual Steam user account. </param>
    /// <returns>Returns a result holding error codes if applicable, the input SID, a count of installed games and 
    /// a list of installed games with their respective properties.</returns>
    [ProducesResponseType(typeof(V1Result<V1SteamGamesCollection>), 200)]
    [ProducesResponseType(typeof(V1Result<V1SteamGamesCollection>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{steamId}")]
    public async Task<ActionResult<V1Result<V1SteamGamesCollection>>> GetSteamGameCollection(string steamId)
    {
        using var client = new HttpClient();
        var key = _KeyServiceSettings.Value.KeySetting1;
        var steamGameCollectionUrl = $"https://api.Steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={key}&steamid={steamId}&include_appinfo=true";
        

        if (string.IsNullOrWhiteSpace(steamId))
        {
            _logger.LogWarning("Unsuccessfull call to GetSteamGameCollection. Invalid Steam ID provided.");
            return BadRequest(new V1Result<V1SteamGamesCollection>("Invalid Steam ID. You only entered whitespace."));
        }

        if (steamId.Any(char.IsWhiteSpace))
        {
            _logger.LogWarning("Unsuccessfull call to GetSteamGameCollection. Invalid Steam ID provided.");
            return BadRequest(new V1Result<V1SteamGamesCollection>("Invalid Steam ID. Valid IDs does not contain whitespace."));
        }

        bool containsOnlyDigits = steamId.All(char.IsDigit);
        if(containsOnlyDigits == false)
        {
            _logger.LogWarning("Unsuccessfull call to GetSteamGameCollection. Invalid Steam ID provided.");
            return BadRequest(new V1Result<V1SteamGamesCollection>("Invalid Steam ID. Valid Steam IDs only contain digits."));
        }
        if (steamId.Length != 17)
        {
            _logger.LogWarning("Unsuccessfull call to GetSteamGameCollection. Invalid Steam ID provided.");
            return BadRequest(new V1Result<V1SteamGamesCollection>("Invalid Steam ID. Valid Steam IDs are 17 digits long. You entered: " + steamId.Length));
        }

        var response = await client.GetAsync(steamGameCollectionUrl);

        string responseString = response.Content.ReadAsStringAsync().Result;
        if (responseString == "{\"response\":{}}")
        {
            _logger.LogWarning("Unsuccessfull call steam. Empty result. May be because profile has hidden games.");
            return BadRequest(new V1Result<V1SteamGamesCollection>("Could not get games for this profile. Might be private or games are hidden."));
        }
        

        switch (response.StatusCode)
        {
            case (System.Net.HttpStatusCode)200:
                V1SteamGetOwnedGamesData? responseJson = JsonConvert
                    .DeserializeObject<V1SteamGetOwnedGamesData>(responseString) ?? throw new ArgumentNullException(nameof(responseJson));
                V1SteamGamesCollection steamGames = new V1SteamGamesCollection()
                {
                    games = new List<V1SteamGamesCollection.Game>(),
                    game_count = responseJson.response.game_count,
                    steamId = steamId
                };

                foreach (var game in responseJson.response.games)
                {
                    steamGames.games.Add(new V1SteamGamesCollection.Game()
                    {
                        appid = game.appid,
                        name = game.name,
                        playtime_forever = game.playtime_forever,
                        img_icon_url = game.img_icon_url,
                    });
                }
                return Ok(new V1Result<V1SteamGamesCollection>(steamGames));
                
            case (System.Net.HttpStatusCode)400:
                _logger.LogWarning("GetSteamGamesCollection called Steam Web API unsuccesfully. Check sent parameters. Error code 400.");
                return BadRequest(new V1Result<V1SteamGamesCollection>("Please verify that all required parameters are being sent."));
            
            case (System.Net.HttpStatusCode)401:

                _logger.LogWarning("GetSteamGamesCollection called Steam Web API unsuccesfully. Check KEY parameters. Error code 401.");
                return BadRequest(new V1Result<V1SteamGamesCollection>("Access is denied."));
            case (System.Net.HttpStatusCode)403:

            
                _logger.LogWarning("GetSteamGamesCollection called Steam Web API unsuccesfully. Check KEY parameter. Error code 403.");
                return BadRequest(new V1Result<V1SteamGamesCollection>("Access is denied."));

            case (System.Net.HttpStatusCode)404:


                _logger.LogWarning("GetSteamGamesCollection called Steam Web API unsuccesfully. The API does not exist. Error code 404.");
                return BadRequest(new V1Result<V1SteamGamesCollection>("The API requested does not exist."));

            case (System.Net.HttpStatusCode)405:
                _logger.LogWarning("GetSteamGamesCollection called Steam Web API unsuccesfully. Incorrect HTTP method. Error code 405.");
                return BadRequest(new V1Result<V1SteamGamesCollection>("Error in call to steam."));

            case (System.Net.HttpStatusCode)429:
                _logger.LogWarning("GetSteamGamesCollection called Steam Web API unsuccesfully. Rate limitations has been issued. Error code 429.");
                return BadRequest(new V1Result<V1SteamGamesCollection>("You are being rate limited."));
        }

        _logger.LogWarning("GetSteamGamesCollection called Steam Web API unsuccesfully. Unknown error. Error code 500.");
        return new ObjectResult(new V1Result<V1SteamGamesCollection>("An unknown error has occured or the server is temporaraliy not avaialable. Contact the Steamworks Dev Team if this error persists."))
        {
            StatusCode = 500
        };
    }
}