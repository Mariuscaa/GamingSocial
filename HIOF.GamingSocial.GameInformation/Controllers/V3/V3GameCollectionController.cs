using HIOF.GamingSocial.GameInformation.Data;
using HIOF.GamingSocial.GameInformation.Models.External;
using HIOF.GamingSocial.GameInformation.Models.V3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace HIOF.GamingSocial.GameInformation.Controllers.V3;

/// <summary>
/// Controller that handles creation, editing, deleting and retrieval of users personal game collections with their ratings.
/// </summary>
[ApiController]
[Route("V3/GameCollection")]
public class V3GameCollectionController : ControllerBase
{
    private readonly ILogger<V3GameCollectionController> _logger;
    private readonly VideoGameDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="V3GameCollectionController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="db">The database context.</param>
    public V3GameCollectionController(ILogger<V3GameCollectionController> logger, VideoGameDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Adds a collection of games for one user to the database
    /// </summary>
    /// <param name="postGameCollection"> an object that contains one ProfileGuid and a list of GameIds</param>
    /// <returns>Returns the result of the post, if successfull it returns what was saved to the database</returns>
    [HttpPost("")]
    [ProducesResponseType(typeof(V3Result<V3PostGameCollection>), 200)]
    [ProducesResponseType(typeof(V3Result<V3PostGameCollection>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3PostGameCollection>>> CreateCollection(V3PostGameCollection postGameCollection)
    {
        if (postGameCollection.GameIds.Count == 0)
        {
            _logger.LogWarning($"The list for GameIds contains no elements.");
            return BadRequest(new V3Result<V3PostGameCollection>($"The list for GameIds contains no elements! Remember to input GameIds!"));
        }

        for (int i = 0; i < postGameCollection.GameIds.Count; i++)
        {
            var gameCollection = new ProfileGameCollection()
            {
                ProfileId = postGameCollection.ProfileId,
                GameId = postGameCollection.GameIds[i],
            };
            _db.ProfileGameCollection.Add(gameCollection);
        }
        await _db.SaveChangesAsync();

        var result = new V3Result<V3PostGameCollection>(new V3PostGameCollection()
        {
            ProfileId = postGameCollection.ProfileId,
            GameIds = postGameCollection.GameIds,
        });
        _logger.LogInformation($"Successfully added {postGameCollection.GameIds.Count} games to the collection for Profile: {postGameCollection.ProfileId}");
        return Ok(result);
    }

    /// <summary>
    /// Dynamically updates the GameCollection database by searching for games connected to the steam account. 
    /// </summary>
    /// <param name="profileGuid">Unique Guid for a profile in our system.</param>
    /// <param name="steamId">A unique id for a Steam account. For example like this: 76561198035571936</param>
    /// <returns>An object with the updated information in the GameCollection database.</returns>
    [HttpPost("GameCollectionUpdate")]
    [ProducesResponseType(typeof(V3Result<V3ProfileWithGames>), 200)]
    [ProducesResponseType(typeof(V3Result<V3ProfileWithGames>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3ProfileWithGames>>> RetrieveAndUpdateGameCollection(Guid profileGuid, string steamId)
    {
        var resultProfileWithGames = new V3ProfileWithGames()
        {
            ProfileGuid = profileGuid,
            GamesCollection = new List<V3GameRatings>()
        };
        string url = $"https://localhost:7095/V1/SteamGameCollection/{steamId}";
        using var client = new HttpClient();
        var response = await client.GetAsync(url);
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var steamGameCollectionResult = JsonSerializer.Deserialize<V3Result<V1SteamGamesCollection>>(responseString, options);

        if (steamGameCollectionResult == null)
        {
            _logger.LogError("Could not deserialize response from public game information.");
            return StatusCode(StatusCodes.Status500InternalServerError, new V3Result<V3ProfileWithGames>("Unexpected error: Could not get games."));
        }

        var gameIds = new List<int>();

        if (steamGameCollectionResult.Errors.IsNullOrEmpty())
        {
            if (steamGameCollectionResult.Value.games.Any())
            {
                foreach (var game in steamGameCollectionResult.Value.games)
                {
                    url = $"https://localhost:7296/V3/VideoGameInformation?steamAppId={game.appid}";
                    response = await client.GetAsync(url);
                    responseString = await response.Content.ReadAsStringAsync();
                    var gameInfoResult = JsonSerializer.Deserialize<V3Result<V3VideoGameInformation>>(responseString, options);

                    if (gameInfoResult == null)
                    {
                        _logger.LogError("Could not deserialize response from gameinformation.");
                    }
                    else
                    {
                        if (gameInfoResult.Errors.IsNullOrEmpty())
                        {
                            gameIds.Add(gameInfoResult.Value.Id);
                        }
                        else
                        {
                            _logger.LogWarning(gameInfoResult.Errors[0] + " || steamAppId: " + game.appid);
                        }
                    }
                }

                // Gets existing gameids for the user to avoid errors when adding to db later.
                var existingGames = await _db.ProfileGameCollection
                .Where(x => x.ProfileId == profileGuid)
                .Select(x => x.GameId)
                .ToListAsync();

                for (int i = 0; i < gameIds.Count; i++)
                {
                    var gameId = gameIds[i];
                    if (!existingGames.Contains(gameId))
                    {
                        var gameCollection = new ProfileGameCollection()
                        {
                            ProfileId = profileGuid,
                            GameId = gameId,
                        };
                        resultProfileWithGames.GamesCollection.Add(new V3GameRatings()
                        {
                            GameId = gameId
                        });
                        _db.ProfileGameCollection.Add(gameCollection);
                    }
                }
                await _db.SaveChangesAsync();
                _logger.LogInformation($"Successfully added {gameIds.Count} games to the collection for Profile: {profileGuid}");
                return Ok(new V3Result<V3ProfileWithGames>(resultProfileWithGames));
            }
            else
            {
                _logger.LogWarning($"The profile {steamId} has no games.");
                return BadRequest(new V3Result<V3ProfileWithGames>($"The profile {steamId} has no games."));
            }
        }
        else
        {
            _logger.LogWarning("Error from PublicProfileInformation: " + steamGameCollectionResult.Errors[0]);
            return BadRequest(new V3Result<V3ProfileWithGames>(steamGameCollectionResult.Errors[0]));
        }
    }

    /// <summary>
    /// Gets all the users that have a specific game and their ratings. 
    /// If no game is specified, it will get all the game collections for all profiles.
    /// </summary>
    /// <param name="gameId">An int that refers to a unique game in the database.</param>
    /// <returns>Returns a list of users and their game collection with ratings.</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(V3Result<V3ProfileWithGames>), 200)]
    [ProducesResponseType(typeof(V3Result<V3ProfileWithGames>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<IEnumerable<V3ProfileWithGames>>>> Get(int? gameId = null)
    {
        if (gameId == null)
        {
            var responseCollection = await _db.ProfileGameCollection
            .ToListAsync();

            var responseUsers = new List<V3ProfileWithGames>();

            foreach (var group in responseCollection.GroupBy(c => c.ProfileId))
            {
                var responseUser = new V3ProfileWithGames
                {
                    ProfileGuid = group.Key,
                    GamesCollection = new List<V3GameRatings>()
                };

                foreach (var collection in group)
                {
                    var gameWithRating = new V3GameRatings
                    {
                        GameId = collection.GameId,
                        GameRating = collection.GameRating
                    };
                    responseUser.GamesCollection.Add(gameWithRating);
                }
                responseUsers.Add(responseUser);
            }

            return Ok(new V3Result<List<V3ProfileWithGames>>(responseUsers));
        }
        else
        {
            var responseCollection = await _db.ProfileGameCollection
            .Where(profile => profile.GameId == gameId)
            .Select(profile => new ProfileGameCollection
            {
                ProfileId = profile.ProfileId,
                GameId = profile.GameId,
                GameRating = profile.GameRating,
            })
            .ToListAsync();

            if (responseCollection.Count == 0)
            {
                _logger.LogInformation($"Could not find any users with the game {gameId}.");
                return NotFound(new V3Result<IEnumerable<V3ProfileWithGames>>($"Could not find any users with the game {gameId}."));
            }

            var responseUsers = new List<V3ProfileWithGames>();

            foreach (var group in responseCollection.GroupBy(c => c.ProfileId))
            {
                var responseUser = new V3ProfileWithGames
                {
                    ProfileGuid = group.Key,
                    GamesCollection = new List<V3GameRatings>()
                };

                foreach (var collection in group)
                {
                    var gameWithRating = new V3GameRatings
                    {
                        GameId = collection.GameId,
                        GameRating = collection.GameRating
                    };
                    responseUser.GamesCollection.Add(gameWithRating);
                }
                responseUsers.Add(responseUser);
            }

            return Ok(new V3Result<List<V3ProfileWithGames>>(responseUsers));
        }

    }

    /// <summary>
    /// Gets all the games one user have and their ratings.
    /// </summary>
    /// <param name="profileGuid">a unique Guid</param>
    /// <param name="onlyRatedGames">a bool value to choose to include/exclude non rated games.</param>
    /// <returns>Returns a list of games one user have and their ratings.</returns>
    [HttpGet("Profile")]
    [ProducesResponseType(typeof(V3Result<V3ProfileWithGames>), 200)]
    [ProducesResponseType(typeof(V3Result<V3ProfileWithGames>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3ProfileWithGames>>> GetSingleGameCollection(Guid profileGuid, bool onlyRatedGames = false)
    {
        var responseCollection = await _db.ProfileGameCollection
            .Where(profile => profile.ProfileId == profileGuid)
            .Select(profile => new ProfileGameCollection
            {
                ProfileId = profile.ProfileId,
                GameId = profile.GameId,
                GameRating = profile.GameRating,
            })
            .ToListAsync();

        if (responseCollection.Count == 0)
        {
            _logger.LogWarning($"Search for the profileGuid {profileGuid} returned no results.");
            return NotFound(new V3Result<V3ProfileWithGames>($"Search for the profileGuid {profileGuid} returned no results. Its collection may be empty or the user does not exist."));
        }
        List<V3GameRatings> gamesWithRatings;

        if (onlyRatedGames)
        {
            gamesWithRatings = responseCollection
                .Where(collection => collection.GameRating.HasValue)
                .Select(collection => new V3GameRatings
                {
                    GameId = collection.GameId,
                    GameRating = collection.GameRating.HasValue ? collection.GameRating.Value : null
                })
                .ToList();
        }
        else
        {
            gamesWithRatings = responseCollection.Select(collection => new V3GameRatings
            {
                GameId = collection.GameId,
                GameRating = collection.GameRating
            }).ToList();
        }

        var response = new V3ProfileWithGames
        {
            ProfileGuid = profileGuid,
            GamesCollection = gamesWithRatings
        };

        return Ok(new V3Result<V3ProfileWithGames>(response));
    }

    /// <summary>
    /// Checks if a single profile has a single game.
    /// </summary>
    /// <param name="profileGuid">The Guid for the profile</param>
    /// <param name="gameId">The id for the game.</param>
    /// <returns>True or false depending on whether the user has the game or not.</returns>
    [HttpGet("CollectionCheck")]
    [ProducesResponseType(typeof(V3Result<bool>), 200)]
    [ProducesResponseType(typeof(V3Result<bool>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<bool>>> CheckIfProfileHasGame(Guid profileGuid, int gameId)
    {
        var responseCollection = await _db.ProfileGameCollection
            .Where(profile => profile.ProfileId == profileGuid)
            .Where(profile => profile.GameId == gameId)
            .Select(profile => new ProfileGameCollection
            {
                ProfileId = profile.ProfileId,
                GameId = profile.GameId
            }).SingleOrDefaultAsync();

        if (responseCollection == null)
        {
            return Ok(new V3Result<bool>(false));
        }
        else
        {
            return Ok(new V3Result<bool>(true));
        }
        
    }

    /// <summary>
    /// Edits the rating of a specific game for a user. 
    /// If the game does not exist in collection, it will be created.
    /// </summary>
    /// <param name="patchPersonalGameRating">An object with the following fields: ProfileId, GameIds and GameRating</param>
    /// <returns> A object with the result of the patch attempt.</returns> 
    [HttpPatch("")]
    [ProducesResponseType(typeof(V3Result<V3PatchGameCollection>), 200)]
    [ProducesResponseType(typeof(V3Result<V3PatchGameCollection>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3PatchGameCollection>>> CreateUserRatingAGame(V3PatchGameCollection patchPersonalGameRating)
    {

        if (patchPersonalGameRating.GameRating > 100 || patchPersonalGameRating.GameRating <= 0)
        {
            _logger.LogWarning($"The game rating score must be between 1 and 100.");
            return BadRequest(new V3Result<V3PatchGameCollection>($"The game rating score must be between 1 and 100."));
        }

        if (patchPersonalGameRating.GameId <= 0)
        {
            _logger.LogWarning($"The gameId can not be a negative number or null.");
            return BadRequest(new V3Result<V3PatchGameCollection>($"The gameId can not be a negative number or null."));
        }

        var gameRating = await _db.ProfileGameCollection.FindAsync(patchPersonalGameRating.ProfileId, patchPersonalGameRating.GameId);

        if (gameRating == null)
        {
            gameRating = new ProfileGameCollection
            {
                ProfileId = patchPersonalGameRating.ProfileId,
                GameId = patchPersonalGameRating.GameId,
                GameRating = patchPersonalGameRating.GameRating
            };

            _db.ProfileGameCollection.Add(gameRating);
        }
        else
        {
            gameRating.ProfileId = patchPersonalGameRating.ProfileId;
            gameRating.GameId = patchPersonalGameRating.GameId;
            gameRating.GameRating = patchPersonalGameRating.GameRating;
        }
        await _db.SaveChangesAsync();

        var result = new V3Result<V3PatchGameCollection>(new V3PatchGameCollection()
        {
            ProfileId = gameRating.ProfileId,
            GameId = gameRating.GameId,
            GameRating = gameRating.GameRating,
        });
        _logger.LogInformation($"The game rating for the game {patchPersonalGameRating.GameId} " +
            $"was updated to {patchPersonalGameRating.GameRating} for the user {patchPersonalGameRating.ProfileId}.");
        return Ok(result);
    }

    /// <summary>
    /// Deletes a game from a profile's game collection.
    /// </summary>
    /// <param name="profileGuid">a unique Guid referring to a specific user</param>
    /// <param name="gameId">a unique int referring to a specific game</param>
    /// <returns>Returns what was deleted from the database</returns>
    [HttpDelete("")]
    [ProducesResponseType(typeof(V3Result<V3ProfileGameCollection>), 200)]
    [ProducesResponseType(typeof(V3Result<V3ProfileGameCollection>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3ProfileGameCollection>>> DeleteGameFromUser(Guid profileGuid, int gameId)
    {
        var responseCollections = await _db.ProfileGameCollection
            .Where(profile => profile.ProfileId == profileGuid)
            .Where(profile => profile.GameId == gameId)
            .Select(profile => new ProfileGameCollection
            {
                ProfileId = profile.ProfileId,
                GameId = profile.GameId
            }).ToListAsync();

        if (responseCollections.IsNullOrEmpty())
        {
            _logger.LogWarning($"Can't find any game collection with the Guid {profileGuid} and gameId {gameId}");
            return NotFound(new V3Result<V3ProfileGameCollection>($"Can't find any game collection with the Guid {profileGuid} and gameId {gameId}"));
        }

        var gameCollection = new ProfileGameCollection()
        {
            ProfileId = profileGuid,
            GameId = gameId,
        };

        _db.ProfileGameCollection.Remove(gameCollection);
        await _db.SaveChangesAsync();

        var returnGameCollection = new V3ProfileGameCollection()
        {
            ProfileId = gameCollection.ProfileId,
            GameId = gameCollection.GameId,
        };

        _logger.LogInformation($"The game with the gameId {gameId} was deleted from the profile collection for {profileGuid}.");
        return Ok(new V3Result<V3ProfileGameCollection>(returnGameCollection));
    }
}

