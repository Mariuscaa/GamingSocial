using HIOF.GamingSocial.GameInformation.Data;
using HIOF.GamingSocial.GameInformation.Models.V3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace HIOF.GamingSocial.GameInformation.Controllers.V3;

/// <summary>
/// Handles the data for video game information. 
/// </summary>
[ApiController]
[Route("V3/VideoGameInformation")]
public class V3VideoGameInformationController : ControllerBase
{
    private readonly ILogger<V3VideoGameInformationController> _logger;
    private readonly VideoGameDbContext _db;

    /// <summary>
    /// Creates a new instance of the <see cref="V3VideoGameInformationController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="db">The video game database.</param>
    public V3VideoGameInformationController(ILogger<V3VideoGameInformationController> logger, VideoGameDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Adds a videogame to the database.
    /// </summary>
    /// <param name="gamePost">An object with the following fields: GameTitle, SteamAppId.</param>
    /// <returns>An object with the result of the POST attempt.</returns>
    [HttpPost("")]
    [ProducesResponseType(typeof(V3Result<V3VideoGameInformation>), 200)]
    [ProducesResponseType(typeof(V3Result<V3VideoGameInformation>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3VideoGameInformation>>> CreateGame(V3PostVideoGameInformation gamePost)
    {
        var videoGameInformation = new VideoGameInformation()
        {
            GameTitle = gamePost.GameTitle,
            SteamAppId = gamePost.SteamAppId,
            GameDescription = gamePost.GameDescription,
            GiantbombGuid = gamePost.GiantbombGuid,
        };

        if (gamePost.GameTitle.Length >= 200)
        {
            _logger.LogWarning("Game title is too long. Max is 200 characters.");
            return BadRequest(new V3Result<V3VideoGameInformation>("Game title is too long. Max is 200 characters."));
        }

        if (gamePost.SteamAppId >= 10000000)
        {
            _logger.LogWarning("SteamId is invalid. Max is 8 digits.");
            return BadRequest(new V3Result<V3VideoGameInformation>("SteamId is invalid. Max is 8 digits."));
        }

        if (gamePost.SteamAppId <= 0)
        {
            _logger.LogWarning("SteamId is invalid. Needs to be greater than 0.");
            return BadRequest(new V3Result<V3VideoGameInformation>("SteamId is invalid. Needs to be greater than 0."));
        }

        if (gamePost.GiantbombGuid != null)
        {
            if (gamePost.GiantbombGuid.Length >= 11)
            {
                _logger.LogWarning("Giantbomb guid is too long. Max is 11 characters. It usually looks like this: 3030-123");
                return BadRequest(new V3Result<V3VideoGameInformation>("Giantbomb guid is too long. Max is 11 characters. It usually looks like this: 3030-123"));
            }
        }

        if (gamePost.GameDescription != null)
        {
            if (gamePost.GameDescription.Length >= 450)
            {
                _logger.LogWarning("Game description is too long. Max is 450 characters.");
                return BadRequest(new V3Result<V3VideoGameInformation>("Game description is too long. Max is 450 characters."));
            }
        }

        _db.VideoGameInformation.Add(videoGameInformation);
        await _db.SaveChangesAsync();

        var result = new V3Result<V3VideoGameInformation>(new V3VideoGameInformation()
        {
            Id = videoGameInformation.GameId,
            GameTitle = videoGameInformation.GameTitle,
            SteamAppId = videoGameInformation.SteamAppId,
            GiantbombGuid = videoGameInformation.GiantbombGuid,
            GameDescription = videoGameInformation.GameDescription
        });

        _logger.LogInformation($"Game with id '{videoGameInformation.GameId}' added to database.");
        return Ok(result);
    }

    /// <summary>
    /// Gets all the videogames currently available on the Steam web API and saves them in a local database. 
    /// The result contains over 80 000 games so it may take 10-30 sec to run.
    /// Will find and add differences if games are already in the database.
    /// </summary>
    /// <returns>A list of updated videogames</returns>
    [HttpPost("UpdateGameList")]
    [ProducesResponseType(typeof(V3Result<IEnumerable<V3VideoGameInformation>>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<IEnumerable<V3VideoGameInformation>>>> PostAllSteamGamesToDatabase()
    {
        string url = $"https://localhost:7250/V2/PublicGameInformation";
        using var client = new HttpClient();
        var response = await client.GetAsync(url);
        var responseString = await response.Content.ReadAsStringAsync();

        var steamGames = JsonConvert.DeserializeObject<V3Result<V3SteamGames>>(responseString);
        if (steamGames == null)
        { 
            _logger.LogError($"Could not deserialize response from publicGameInformation.");
            return StatusCode(StatusCodes.Status500InternalServerError, new V3Result<IEnumerable<V3VideoGameInformation>>("Failed to get games."));
        }
        if (steamGames.Value == null)
        {
            _logger.LogError($"Error from PublicGameInformation: {steamGames.Errors[0]}");
            return StatusCode(StatusCodes.Status500InternalServerError, new V3Result<IEnumerable<V3VideoGameInformation>>(steamGames.Errors[0].ToString()));
        }

        // Creates a dictionary by sending in a function (g => g.SteamAppId) that extracts a key from each VideoGameInformation object,
        // where SteamAppId is the key that is used to uniquely identify each game.
        var existingGames = await _db.VideoGameInformation
        .Where(g => g.SteamAppId != null)
        .ToDictionaryAsync(g => g.SteamAppId ?? 0);

        var newGames = new List<VideoGameInformation>();
        foreach (var game in steamGames.Value.games)
        {
            // Tries to get the object with the corresponding app id to check if the game is already in the database.
            // If it is not in the database, it will get added.
            if (!existingGames.TryGetValue(game.appid, out var existingGame))
            {
                existingGame = new VideoGameInformation();
                existingGame.SteamAppId = game.appid;
                _db.VideoGameInformation.Add(existingGame);
                newGames.Add(existingGame);
            }

            // Truncate the game name if it's longer than 200 characters to avoid potential errors when saving to database.
            game.name = game.name.Length > 200 ? game.name.Substring(0, 200) : game.name;
            existingGame.GameTitle = game.name;
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation($"Added {newGames.Count} games to database.");

        if (newGames.Count < 1000)
        {
            var resultGames = newGames.Select(g => new V3VideoGameInformation()
            {
                Id = g.GameId,
                GameTitle = g.GameTitle,
                SteamAppId = g.SteamAppId
            }).ToList();

            return Ok(new V3Result<IEnumerable<V3VideoGameInformation>>(resultGames));
        }
        else
        {
            // To avoid crashing swagger or a client.
            return Ok("Completed ok, games have been added to database. Too many results to return list. Number of games added: " + newGames.Count);
        }
    }

    /// <summary>
    /// Gets information about multiple videogames from the local database. 
    /// Does not load well with many games in swagger, therefore it just returns the total 
    /// amount of games if no search query is provided.
    /// </summary>
    /// <param name="steamAppId">Optional parameter to search for a unique Id from the online steam web API.</param>
    /// <param name="searchString">Optional parameter to search for a specific game title.</param>
    /// <param name="includeSimilar">Optional parameter to allow for similar results. 
    /// For example counter strike would return results like Counter-Strike.</param>
    /// <returns>A list of videogame objects.</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(V3Result<IEnumerable<V3VideoGameInformation>>), 200)]
    [ProducesResponseType(typeof(V3Result<IEnumerable<V3VideoGameInformation>>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<IEnumerable<V3VideoGameInformation>>>> Get(int? steamAppId,
        string? searchString = null, bool includeSimilar = false)
    {
        if (steamAppId != null)
        {
            try
            {
                var responseGame = await _db.VideoGameInformation
                .Where(game => game.SteamAppId == steamAppId)
                .Select(game => new V3VideoGameInformation
                {
                    Id = game.GameId,
                    SteamAppId = game.SteamAppId,
                    GameDescription = game.GameDescription,
                    GameTitle = game.GameTitle,
                    GiantbombGuid = game.GiantbombGuid
                }).SingleOrDefaultAsync();

                if (responseGame == null)
                {
                    _logger.LogWarning($"No game found with steamAppId {steamAppId}");
                    return NotFound(new V3Result<IEnumerable<V3VideoGameInformation>>($"No game found with steamAppId {steamAppId}"));
                }

                return Ok(new V3Result<V3VideoGameInformation>(responseGame));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, $"Returned multiple games with steamAppId {steamAppId}.");
                return NotFound(new V3Result<IEnumerable<V3VideoGameInformation>>($"Returned multiple games with steamAppId {steamAppId}. Needs to be checked out by db administrator."));
            }
        }

        if (searchString == null)
        {
            var responseGames = await _db.VideoGameInformation
            .Select(game => new V3VideoGameInformation
            {
                Id = game.GameId,
                SteamAppId = game.SteamAppId,
                GameDescription = game.GameDescription,
                GameTitle = game.GameTitle,
                GiantbombGuid = game.GiantbombGuid
            }).ToListAsync();
            return Ok(new V3Result<IEnumerable<V3VideoGameInformation>>("Too many results to return list. Number of games: " + responseGames.Count));
        }
        else if (!includeSimilar)
        {
            var responseGames = await _db.VideoGameInformation
                .Where(game => game.GameTitle.Contains(searchString))
                .Select(game => new V3VideoGameInformation
                {
                    Id = game.GameId,
                    SteamAppId = game.SteamAppId,
                    GameDescription = game.GameDescription,
                    GameTitle = game.GameTitle,
                    GiantbombGuid = game.GiantbombGuid
                }).ToListAsync();

            if (!responseGames.Any())
            {
                _logger.LogWarning($"No games found with the search `{searchString}`.");
                return NotFound(new V3Result<IEnumerable<V3VideoGameInformation>>($"No games found with the search `{searchString}`."));
            }
            return new V3Result<IEnumerable<V3VideoGameInformation>>(responseGames);
        }
        else
        {
            string searchStringAlphaNumeric = new string(searchString.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToLower();
            var responseGames = await _db.VideoGameInformation
                .Select(game => new V3VideoGameInformation
                {
                    Id = game.GameId,
                    SteamAppId = game.SteamAppId,
                    GameDescription = game.GameDescription,
                    GameTitle = game.GameTitle,
                    GiantbombGuid = game.GiantbombGuid
                })
                .ToListAsync();

            responseGames = responseGames.Where(game =>
                new string(game.GameTitle.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToLower()
                    .Contains(searchStringAlphaNumeric))
                .ToList();

            if (!responseGames.Any())
            {
                _logger.LogWarning($"No games found with the search `{searchString}`.");
                return NotFound(new V3Result<IEnumerable<V3VideoGameInformation>>($"No games found with the search `{searchString}`"));
            }
            return new V3Result<IEnumerable<V3VideoGameInformation>>(responseGames);
        }

    }

    /// <summary>
    /// Gets information about a single game in the local database.
    /// </summary>
    /// <param name="gameId">A unique game id.</param>
    /// <returns>Information about a single game.</returns>
    [HttpGet("{gameId}")]
    [ProducesResponseType(typeof(V3Result<V3VideoGameInformation>), 200)]
    [ProducesResponseType(typeof(V3Result<V3VideoGameInformation>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3VideoGameInformation>>> GetSingle(int gameId)
    {
        try
        {
            var responseGame = await _db.VideoGameInformation
                .Where(game => game.GameId == gameId)
                .Select(game => new V3VideoGameInformation
                {
                    Id = game.GameId,
                    GameTitle = game.GameTitle,
                    SteamAppId = game.SteamAppId,
                    GiantbombGuid = game.GiantbombGuid,
                    GameDescription = game.GameDescription,
                }).SingleAsync();
            return Ok(new V3Result<V3VideoGameInformation>(responseGame));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"No game found with id {gameId}");
            return NotFound(new V3Result<V3VideoGameInformation>("Game with id '" + gameId + "' was not found."));
        }
    }

    /// <summary>
    /// Deletes a gameId and the data related to it.
    /// </summary>
    /// <param name="GameId"> A unique GameId</param>
    /// <returns> A object where its value is set as 0, because it has succesfully been deleted from the database.</returns>
    [HttpDelete("")]
    [ProducesResponseType(typeof(V3Result<V3VideoGameInformation>), 200)]
    [ProducesResponseType(typeof(V3Result<V3VideoGameInformation>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3VideoGameInformation>>> DeleteGameId(int GameId)
    {
        var responseCollection = await _db.VideoGameInformation
                .Where(game => game.GameId == GameId)
                .Select(game => new VideoGameInformation
                {

                    GameId = game.GameId,
                    SteamAppId = game.SteamAppId,
                    GameTitle = game.GameTitle,
                    GiantbombGuid = game.GiantbombGuid,
                    GameDescription = game.GameDescription
                })
                .ToListAsync();

        if (responseCollection.IsNullOrEmpty())
        {
            _logger.LogWarning($"Failed to delete video game {GameId}. Game was not found.");
            return NotFound(new V3Result<VideoGameInformation>("Cant't find what you are searching for in the database. Please make sure you have filled in the correct information."));
        }

        var gameInformation = new VideoGameInformation()
        {
            GameId = GameId,
        };

        _db.VideoGameInformation.Remove(gameInformation);
        await _db.SaveChangesAsync();
        _logger.LogInformation($"Deleted video game {GameId}.");
        return Ok(new V3Result<VideoGameInformation>(gameInformation));
    }
}