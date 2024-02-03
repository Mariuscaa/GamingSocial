using HIOF.GamingSocial.PublicProfileInformation.Configuration;
using HIOF.GamingSocial.PublicProfileInformation.Models.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HIOF.GamingSocial.PublicProfileInformation.Controllers.V1;

/// <summary>
/// Not in use (yet). Handles the retrieval of achievements for the steam profiles. 
/// </summary>
[ApiController]
[Route("V1/SteamPlayerAchievements")]
public class SteamPlayerAchievementsController : ControllerBase
{
    private readonly ILogger<SteamPlayerAchievementsController> _logger;
    private readonly IOptions<KeyServiceSettings> _KeyServiceSettings;

    ///<summary>
    /// Constructor for SteamPlayerAchievementsController.
    ///</summary>
    ///<param name="logger">Logger object.</param>  
    ///<param name="keyServiceSettings">Options object containing the API key for the Steam Web API service.</param>
    public SteamPlayerAchievementsController(ILogger<SteamPlayerAchievementsController> logger, 
        IOptions<KeyServiceSettings> keyServiceSettings)
    {
        _logger = logger;
        _KeyServiceSettings = keyServiceSettings;
    }

    /// <summary>
    /// Gets a game the user owns and all the achievements for it.
    /// </summary>
    /// <param name="steamId"> A unique SteamId</param>
    /// <param name="appId"> A unique AppId</param>
    /// <returns> A list of all the achievements for a game that the user owns and a list over the completed achievements.</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(V1Result<V1SteamGetPlayerAchievementsData>), 200)]
    [ProducesResponseType(typeof(V1Result<V1SteamGetPlayerAchievementsData>), 400)]
    [ProducesResponseType(500)] 
    public async Task<ActionResult<V1Result<V1SteamGetPlayerAchievementsData>>> GetSteamPlayerAchievements(string? steamId, string? appId) 
    {
        using var client = new HttpClient();
        var key = _KeyServiceSettings.Value.KeySetting1;
        var steamProfileAchievementsUrl = $"https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v0001/?appid={appId}&key={key}&steamid={steamId}=json";
        var response = await client.GetAsync(steamProfileAchievementsUrl);

        string responseString = response.Content.ReadAsStringAsync().Result;


        if (steamId == null)
        {
            var steamIdMissing =
                    new V1Result<V1SteamGetPlayerAchievementsData>($"Please fill out the SteamId section.");
            _logger.LogWarning("Unsuccessfull call to GetSteamPlayerAchievements. No Steam ID provided.");
            return BadRequest(steamIdMissing);

        }
        if (steamId.Length != 17)
        {
            var steamIdLength =
                    new V1Result<V1SteamGetPlayerAchievementsData>($"Invalid character length for the SteamId.");
            _logger.LogWarning("Unsuccessfull call to GetSteamPlayerAchievements. Wrong lenght for provided Steam ID.");
            return BadRequest(steamIdLength);

        }
         
        if (appId == null)
        {
            var appIdMissing =
                    new V1Result<V1SteamGetPlayerAchievementsData>($"Please fill out the AppId section.");
            _logger.LogWarning("Unsuccessfull call to GetSteamPlayerAchievements. AppID was not provided.");
            return BadRequest(appIdMissing);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage =
                new V1Result<V1SteamGetPlayerAchievementsData>($"Unable to find the following SteamId: {steamId} or AppId: {appId}, please try again.");
            _logger.LogWarning("Unsuccessfull call to GetSteamPlayerAchievements. Provided IDs does not exist.");
            return BadRequest(errorMessage);

        }

        V1SteamGetPlayerAchievementsData responseJson = JsonConvert.DeserializeObject<V1SteamGetPlayerAchievementsData>(responseString);

        V1SteamAchievements steamAchievements = new V1SteamAchievements()
        {
            Achievements = new List<V1SteamAchievements.achievements>(),
            Allachievements = new List<V1SteamAchievements.allAchievements>(),

            GameName = responseJson.Playerstats.GameName,
            SteamId = steamId
        };

        foreach (var achievement in responseJson.Playerstats.Achievements)
        {
            if (achievement.Achieved == 1)
            {
                steamAchievements.Achievements.Add(new V1SteamAchievements.achievements()
                {

                    Achieved = achievement.Achieved,
                    UnlockTime = achievement.UnlockTime,
                    ApiName = achievement.ApiName
                });
            }              
        }

        foreach (var Everyachievement in responseJson.Playerstats.Achievements)
             if (Everyachievement.Achieved >= 0)
                steamAchievements.Allachievements.Add(new V1SteamAchievements.allAchievements()
                {
                    Achieved = Everyachievement.Achieved,
                    UnlockTime = Everyachievement.UnlockTime,
                    ApiName = Everyachievement.ApiName
                });

        return Ok(new V1Result<V1SteamAchievements>(steamAchievements));
    }
}

