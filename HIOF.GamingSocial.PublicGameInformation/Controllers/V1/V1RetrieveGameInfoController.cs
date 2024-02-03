using HIOF.GamingSocial.PublicGameInformation.Model.V1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace HIOF.GamingSocial.RetrieveGameInfo.Controllers.V1;

/// <summary>
/// OLD SETUP, CHECK V2. Decided to keep this to show possibilites to add games outside of steam.
/// Handels the endpoints for getting game information online.
/// </summary>
[ApiController]
[Route("V1/PublicGameInformation")]
public class V1RetrieveGameInfoController : ControllerBase
{
    private readonly ILogger<V1RetrieveGameInfoController> _logger;

    public V1RetrieveGameInfoController(ILogger<V1RetrieveGameInfoController> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Gets all the information about a single game from the Giantbomb web api.
    /// </summary>
    /// <param name="guid">A unique id for each item in the Giant Bomb web API.</param>
    /// <returns>An object with all info about a game.</returns>
    [HttpGet("GiantbombGame/{guid}")]
    [ProducesResponseType(typeof(V1Result<V1GiantbombGameData>), 200)]
    [ProducesResponseType(typeof(V1Result<V1GiantbombGameData>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V1Result<V1GiantbombGameData>>> GiantbombGame(string guid)
    {
        using var client = new HttpClient();
        var newHeader = new ProductInfoHeaderValue("GamingSocialNET", "1.0");
        client.DefaultRequestHeaders.UserAgent.Add(newHeader);

        var key = "7347944b9f9422a3454f0122086539c2d314d9da";
        // API documentation: https://www.giantbomb.com/api/documentation/.
        // Example guid: 3030-1.
        // $"https://www.giantbomb.com/api/game/3030-1/?api_key=7347944b9f9422a3454f0122086539c2d314d9da&format=json".
        var jsonFileUrl = $"https://www.giantbomb.com/api/game/{guid}/?api_key={key}&format=json";
        var response = await client.GetAsync(jsonFileUrl);
        if (response.StatusCode == (HttpStatusCode)404)
        {
            return NotFound(new V1Result<V1GiantbombGameData>($"Guid {guid} returned error 404 Not Found. " +
                $"Guid can only be numbers with a dash. For example 3030-1."));
        }

        var responseString = response.Content.ReadAsStringAsync().Result;
        responseString = responseString.Replace("[]", "null");

        V1GiantbombGameData? gameData =
            JsonConvert.DeserializeObject<V1GiantbombGameData>(responseString);

        switch (gameData.status_code)
        {
            case 1:
                return Ok(new V1Result<V1GiantbombGameData>(gameData));
            case 100:
                return BadRequest(new V1Result<V1GiantbombGameData>("Problem with API key on the server. Contact administrator."));
            case 101:
                return BadRequest(new V1Result<V1GiantbombGameData>($"No results for the guid {guid}."));
            case 102:
                return BadRequest(new V1Result<V1GiantbombGameData>($"Input {guid} caused URL formatting error."));
            case 104:
                return BadRequest(new V1Result<V1GiantbombGameData>($"Filter error. Serverside problem, contact administator."));
        }
        return new ObjectResult(new V1Result<V1GiantbombGameData>("Unknown error."))
        {
            StatusCode = 500
        };
    }



    /// <summary>
    /// Gets the description of a game from the Giantbomb API.
    /// </summary>
    /// <param name="guid">A unique id for each item in the Giant Bomb web API.</param>
    /// <returns>A string in HTML format with the description of a game.</returns>
    [HttpGet("GiantbombGameDescription/{guid}")]
    [ProducesResponseType(typeof(V1Result<V1Description>), 200)]
    [ProducesResponseType(typeof(V1Result<V1Description>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V1Result<V1Description>>> GiantbombGameDescription(string guid)
    {
        using var client = new HttpClient();
        var newHeader = new ProductInfoHeaderValue("GamingSocialNET", "1.0");
        client.DefaultRequestHeaders.UserAgent.Add(newHeader);
        var key = "7347944b9f9422a3454f0122086539c2d314d9da";

        // API documentation: https://www.giantbomb.com/api/documentation/.
        // Example guid: 3030-1.
        var jsonFileUrl = $"https://www.giantbomb.com/api/game/{guid}/?api_key={key}&format=json";

        var response = await client.GetAsync(jsonFileUrl);
        if (response.StatusCode == (HttpStatusCode)404)
        {
            return NotFound(new V1Result<V1Description>($"Guid {guid} returned error 404 Not Found. " +
                $"Guid can only be numbers with a dash. For example 3030-1."));
        }
        var responseString = response.Content.ReadAsStringAsync().Result;

        // Replace "[]" to prevent parsing error due to the web api returning
        // an empty list instead of an object when game is not found.
        responseString = responseString.Replace("[]", "null");

        V1GiantbombGameData? gameData =
            JsonConvert.DeserializeObject<V1GiantbombGameData>(responseString);

        switch (gameData.status_code)
        {
            case 1:
                V1Description? description = new V1Description()
                {
                    Description = Convert.ToString(gameData.results.description)
                };
                return Ok(new V1Result<V1Description>(description));
            case 100:
                return BadRequest(new V1Result<V1Description>("Problem with API key on the server. Contact administrator."));
            case 101:
                return BadRequest(new V1Result<V1Description>($"No results for the guid {guid}."));
            case 102:
                return BadRequest(new V1Result<V1Description>($"Input {guid} caused URL formatting error."));
            case 104:
                return BadRequest(new V1Result<V1Description>($"Filter error. Serverside problem, contact administator."));
        }
        return BadRequest(new V1Result<V1Description>($"Unknown error."));
    }
}