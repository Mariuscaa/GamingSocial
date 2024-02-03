using HIOF.GamingSocial.GameTimePlan.Data;
using HIOF.GamingSocial.GameTimePlan.Model;
using HIOF.GamingSocial.GameTimePlan.Model.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using HIOF.GamingSocial.GameTimePlan.Model.External;

namespace HIOF.GamingSocial.GameTimePlan.Controllers.V1;

/// <summary>
/// Handles the plans for gametime events. This is like a calendar for which a group can save planned game sessions.
/// </summary>
[ApiController]
[Route("[controller]")]
public class V1GameTimePlanController : ControllerBase
{
    private readonly ILogger<V1GameTimePlanController> _logger;

    private readonly GameTimePlanDbContext _db;

    /// <summary>
    /// Creates an instance of <see cref="V1GameTimePlanController"/> with the provided logger and database context.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="db">The database context instance.</param>
    public V1GameTimePlanController(ILogger<V1GameTimePlanController> logger, GameTimePlanDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Creates a single gametimeplan in the database.
    /// </summary>
    /// <param name="postGameTimePlan">An object which contains all the required information about a gametimeplan.</param>
    /// <returns>An object for the newly created gametimeplan.</returns>
    [HttpPost("")]
    [ProducesResponseType(typeof(V1Result<V1GameTimePlan>), 200)]
    [ProducesResponseType(typeof(V1Result<V1GameTimePlan>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V1Result<V1GameTimePlan>>> CreateGameTimePlan(V1PostGameTimePlan postGameTimePlan)
    {
        var gameTimePlan = new GameTimePlanData()
        {
            Name = postGameTimePlan.Name,
            Description = postGameTimePlan.Description,
            GameId = postGameTimePlan.GameId,
            GroupId = postGameTimePlan.GroupId,
            StartTime = postGameTimePlan.StartTime,
            EndTime = postGameTimePlan.EndTime,
        };

        if (gameTimePlan.Name.Length >= 50)
        {
            _logger.LogWarning("POST '/V1GameTimePlan' was called with a bad request. Name is too long. Max is 50 characters.");
            return BadRequest(new V1Result<V1GameTimePlan>("Name is too long. Max is 50 characters."));
        }

        if (gameTimePlan.Description.Length >= 200)
        {
            _logger.LogWarning("POST '/V1GameTimePlan' was called with a bad request. Description is too long. Max is 200 characters.");

            return BadRequest(new V1Result<V1GameTimePlan>("Description is too long. Max is 200 characters."));
        }

        if (postGameTimePlan.GameId < 1 || postGameTimePlan.GameId > 10000000)
        {
            _logger.LogWarning("POST '/V1GameTimePlan' was called with a bad request. GameId is invalid. Needs to be between 1 and 10000000.");
            return BadRequest(new V1Result<V1GameTimePlan>("GameId is invalid. Needs to be between 1 and 10000000" +
                "."));
        }

        if (gameTimePlan.GroupId < 0)
        {
            _logger.LogWarning("POST '/V1GameTimePlan' was called with a bad request. GroupId needs to be greater than 0.");
            return BadRequest(new V1Result<V1GameTimePlan>("GroupId needs to be greater than 0."));
        }

        if (gameTimePlan.StartTime < DateTime.Now)
        {
            _logger.LogWarning("POST '/V1GameTimePlan' was called with a bad request. StartTime cannot be in the past.");
            return BadRequest(new V1Result<V1GameTimePlan>("StartTime cannot be in the past."));
        }

        if (gameTimePlan.EndTime <= gameTimePlan.StartTime)
        {
            _logger.LogWarning("POST '/V1GameTimePlan' was called with a bad request. EndTime cannot be before (or the same as) start time.");
            return BadRequest(new V1Result<V1GameTimePlan>("EndTime cannot be before (or the same as) start time."));
        }

        _db.GameTimePlan.Add(gameTimePlan);
        await _db.SaveChangesAsync();

        var result = new V1Result<V1GameTimePlan>(new V1GameTimePlan()
        {
            GameTimePlanId = gameTimePlan.GameTimePlanId,
            Name = gameTimePlan.Name,
            Description = gameTimePlan.Description,
            GameId = gameTimePlan.GameId,
            GroupId = gameTimePlan.GroupId,
            StartTime = gameTimePlan.StartTime,
            EndTime = gameTimePlan.EndTime
        });
        _logger.LogInformation($"Created GameTimePlan with id: {result.Value.GameId}.");
        return Ok(result);
    }

    /// <summary>
    /// Gets gametimeplans from the database. By default it gets everything, but can be filtered with parameters.
    /// </summary>
    /// <param name="groupId">Optional parameter to filter by group.</param>
    /// <param name="gameId">Optional parameter to filter by game.</param>
    /// <returns>A list of GameTimePlans.</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(V1Result<IEnumerable<V1GameTimePlan>>), 200)]
    [ProducesResponseType(typeof(V1Result<IEnumerable<V1GameTimePlan>>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V1Result<IEnumerable<V1GameTimePlan>>>> Get(int? groupId = null, int? gameId = null)
    {
        if (groupId == null && gameId == null)
        {
            var responsePlans = await _db.GameTimePlan
                .Select(gameTimePlan => new V1GameTimePlan
                {
                    GameTimePlanId = gameTimePlan.GameTimePlanId,
                    Name = gameTimePlan.Name,
                    Description = gameTimePlan.Description,
                    GameId = gameTimePlan.GameId,
                    GroupId = gameTimePlan.GroupId,
                    StartTime = gameTimePlan.StartTime,
                    EndTime = gameTimePlan.EndTime,
                }).ToListAsync();
            if (responsePlans.Count == 0)
            {
                _logger.LogWarning("Get call was unsuccessful. No GameTimePlans were found.");
                return NotFound(new V1Result<IEnumerable<V1GameTimePlan>>("No GameTimePlans were found."));
            }
            return Ok(new V1Result<IEnumerable<V1GameTimePlan>>(responsePlans));
        }
        else if (groupId.HasValue && gameId.HasValue)
        {
            var responsePlans = await _db.GameTimePlan
            .Where(plan => plan.GroupId == groupId && plan.GameId == gameId)
            .Select(gameTimePlan => new V1GameTimePlan
            {
                GameTimePlanId = gameTimePlan.GameTimePlanId,
                Name = gameTimePlan.Name,
                Description = gameTimePlan.Description,
                GameId = gameTimePlan.GameId,
                GroupId = gameTimePlan.GroupId,
                StartTime = gameTimePlan.StartTime,
                EndTime = gameTimePlan.EndTime,
            }).ToListAsync();
            if (responsePlans.Count == 0)
            {
                _logger.LogWarning("Get call was unsuccessful. No GameTimePlans were found.");
                return NotFound(new V1Result<IEnumerable<V1GameTimePlan>>("No GameTimePlans were found."));
            }
            return Ok(new V1Result<IEnumerable<V1GameTimePlan>>(responsePlans));
        }
        else if (groupId.HasValue)
        {
            var responsePlans = await _db.GameTimePlan
            .Where(plan => plan.GroupId == groupId)
            .Select(gameTimePlan => new V1GameTimePlan
            {
                GameTimePlanId = gameTimePlan.GameTimePlanId,
                Name = gameTimePlan.Name,
                Description = gameTimePlan.Description,
                GameId = gameTimePlan.GameId,
                GroupId = gameTimePlan.GroupId,
                StartTime = gameTimePlan.StartTime,
                EndTime = gameTimePlan.EndTime,
            }).ToListAsync();
            if (responsePlans.Count == 0)
            {
                _logger.LogWarning("Get call was unsuccessful. No GameTimePlans were found.");
                return NotFound(new V1Result<IEnumerable<V1GameTimePlan>>("No GameTimePlans were found."));
            }
            return Ok(new V1Result<IEnumerable<V1GameTimePlan>>(responsePlans));
        }
        else
        {
            var responsePlans = await _db.GameTimePlan
            .Where(plan => plan.GameId == gameId)
            .Select(gameTimePlan => new V1GameTimePlan
            {
                GameTimePlanId = gameTimePlan.GameTimePlanId,
                Name = gameTimePlan.Name,
                Description = gameTimePlan.Description,
                GameId = gameTimePlan.GameId,
                GroupId = gameTimePlan.GroupId,
                StartTime = gameTimePlan.StartTime,
                EndTime = gameTimePlan.EndTime
            }).ToListAsync();
            if (responsePlans.Count == 0)
            {
                _logger.LogWarning("Get call was unsuccessful. No GameTimePlans were found.");
                return NotFound(new V1Result<IEnumerable<V1GameTimePlan>>("No GameTimePlans were found."));
            }
            return Ok(new V1Result<IEnumerable<V1GameTimePlan>>(responsePlans));
        }
    }

    /// <summary>
    /// Gets all information about a single game.
    /// </summary>
    /// <param name="gameTimePlanId">A unique id for a specific gametimeplan.</param>
    /// <returns>A single gametimeplan object.</returns>
    [HttpGet("{gameTimePlanId}")]
    [ProducesResponseType(typeof(V1Result<IEnumerable<V1GameTimePlan>>), 200)]
    [ProducesResponseType(typeof(V1Result<IEnumerable<V1GameTimePlan>>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V1Result<V1GameTimePlan>>> GetSingle(int gameTimePlanId)
    {
        try
        {
            var responseGame = await _db.GameTimePlan
                .Where(plan => plan.GameTimePlanId == gameTimePlanId)
                .Select(gameTimePlan => new V1GameTimePlan
                {
                    GameTimePlanId = gameTimePlan.GameTimePlanId,
                    Name = gameTimePlan.Name,
                    Description = gameTimePlan.Description,
                    GameId = gameTimePlan.GameId,
                    GroupId = gameTimePlan.GroupId,
                    StartTime = gameTimePlan.StartTime,
                    EndTime = gameTimePlan.EndTime
                }).SingleAsync();
            return Ok(new V1Result<V1GameTimePlan>(responseGame));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"GET was not called successfully with id: {gameTimePlanId}.");
            return NotFound(new V1Result<V1GameTimePlan>("Could not find gametimeplan with id " + gameTimePlanId));
        }
    }

    /// <summary>
    /// Generates a game suggestion for a group based on games in their collection and their rating. 
    /// Returns multiple if they have the same average rating.
    /// </summary>
    /// <param name="groupId">The id for the group which shall be given a game suggestion.</param>
    /// <returns>A list with the gameId(s) with the highest average rating for the group. 
    /// If no rating is given for any of the common games, it will return all of them.</returns>
    [HttpGet("GameSuggestion")]
    [ProducesResponseType(typeof(V1Result<IEnumerable<int>>), 200)]
    [ProducesResponseType(typeof(V1Result<IEnumerable<int>>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V1Result<IEnumerable<int>>>> GameSuggestion(int groupId)
    {
        string url = $"https://localhost:7087/V2/GroupMembership/{groupId}";

        using var client = new HttpClient();
        var response = await client.GetAsync(url);
        var responseString = await response.Content.ReadAsStringAsync();

        var usersInGroup = JsonConvert.DeserializeObject<V1Result<V2GroupMembership>>(responseString);
        if (usersInGroup == null)
        {
            _logger.LogWarning($"GET '/V1GameTimePlan/GameSuggestion' was not called successfully. Failed to deserialize response.");
            return StatusCode(StatusCodes.Status500InternalServerError, new V1Result<IEnumerable<int>>("Failed to get users for the group"));
        }
        var personalGameCollections = new List<V3ProfileWithGames>();

        foreach (var user in usersInGroup.Value.ProfileGuids)
        {
            url = $"https://localhost:7296/V3/GameCollection/Profile?profileGuid={user}";
            response = await client.GetAsync(url);
            responseString = await response.Content.ReadAsStringAsync();
            var singleUserRatings = JsonConvert.DeserializeObject<V1Result<V3ProfileWithGames>>(responseString);
            if (singleUserRatings != null && singleUserRatings.Value != null)
            {
                personalGameCollections.Add(singleUserRatings.Value);
            }
            else
            {
                _logger.LogWarning($"Failed to deserialize response for {user}");
            }
        }
        Dictionary<int, double> gameAverages = new Dictionary<int, double>();

        foreach (var userGames in personalGameCollections)
        {
            if (userGames == null)
            {
                continue;
            }
            foreach (var game in userGames.GamesCollection)
            {
                if (game != null)
                {
                    if (!gameAverages.ContainsKey(game.GameId) && IsGameInAllCollections(game.GameId, personalGameCollections))
                    {
                        gameAverages[game.GameId] = 0.0;
                    }
                    if (game.GameRating.HasValue && gameAverages.ContainsKey(game.GameId))
                    {
                        gameAverages[game.GameId] += game.GameRating.Value;
                    }
                }
            }
        }

        foreach (var gameId in gameAverages.Keys.ToList())
        {
            int count = personalGameCollections.Sum(userGames => userGames.GamesCollection.Count(game => game != null && game.GameId == gameId));
            gameAverages[gameId] /= count;
        }

        if (gameAverages.Count == 0)
        {
            _logger.LogWarning($"GameSuggestion found no games in common.");
            return new V1Result<IEnumerable<int>>(new List<int>() {0});

        }
        double maxRating = gameAverages.Values.Max();

        List<int> topRatedGames = gameAverages.Where(kvp => kvp.Value == maxRating)
                                       .Select(kvp => kvp.Key)
                                       .ToList();
        return new V1Result<IEnumerable<int>>(topRatedGames);
    }
    private bool IsGameInAllCollections(int gameId, List<V3ProfileWithGames> personalGameCollections)
    {
        foreach (var userGames in personalGameCollections)
        {
            if (userGames == null)
            {
                return false;
            }
            if (!userGames.GamesCollection.Any(game => game != null && game.GameId == gameId))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Deletes a GameTimePlanId and the data related to it.
    /// </summary>
    /// <param name="gameTimePlanId">A unique GameTimePlanId</param>
    /// <returns> A object where its value is set as 0, because it has succesfully been deleted from the database.</returns>
    [ProducesResponseType(typeof(V1Result<GameTimePlanData>), 200)]
    [ProducesResponseType(typeof(V1Result<GameTimePlanData>), 404)]
    [ProducesResponseType(500)]
    [HttpDelete("")]
    public async Task<ActionResult<V1Result<GameTimePlanData>>> DeleteGameTimePlan(int gameTimePlanId)
    {
        var responseCollection = await _db.GameTimePlan
                .Where(gameTimePlan => gameTimePlan.GameTimePlanId == gameTimePlanId)
                .Select(gameTimePlan => new GameTimePlanData
                {
                    GameTimePlanId = gameTimePlan.GameTimePlanId,
                    Name = gameTimePlan.Name,
                    Description = gameTimePlan.Description,
                    GameId = gameTimePlan.GameId,
                    GroupId = gameTimePlan.GroupId,
                    StartTime = gameTimePlan.StartTime,
                    EndTime = gameTimePlan.EndTime
                })
                .ToListAsync();

        if (responseCollection.IsNullOrEmpty())
        {
            _logger.LogWarning($"Could not find GameTimePlan with id: {gameTimePlanId}.");
            return NotFound(new V1Result<GameTimePlanData>($"Could not find GameTimePlan with id: {gameTimePlanId}."));
        }

        var gameTimePlanner = new GameTimePlanData()
        {
            GameTimePlanId = gameTimePlanId,
        };

        _db.GameTimePlan.Remove(gameTimePlanner);
        await _db.SaveChangesAsync();
        _logger.LogInformation($"Deleted GameTimePlan with id: {gameTimePlanId}.");
        return Ok(new V1Result<GameTimePlanData>(gameTimePlanner));
    }
}