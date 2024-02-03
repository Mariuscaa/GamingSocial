using HIOF.GamingSocial.ProfileInformation.Model.V3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HIOF.GamingSocial.ProfileInformation.Data;

namespace HIOF.GamingSocial.ProfileInformation.Controllers.V3;

/// <summary>
/// Handles all requests related to friends.
/// </summary>
[ApiController]
[Route("V3/Friend")]
public class V3FriendController : ControllerBase
{
    private readonly ILogger<V3ProfileController> _logger;
    private readonly ProfileInformationDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="V3FriendController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="db">The database context.</param>
    public V3FriendController(ILogger<V3ProfileController> logger, ProfileInformationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Endpoint for creating new friends / friendships in the database. 
    /// A friendship can only be added to the database once. This means that the friendship always goes both ways.
    /// </summary>
    /// <param name="profileGuid1">profileGuid1 shall become friends with profileGuid2. Order is irrelevant.</param>
    /// <param name="profileGuid2">profileGuid2 shall become friends with profileGuid1. Order is irrelevant.</param>
    /// <returns>Returns a friend object with both of the profileGuids.</returns>
    [HttpPost("")]
    [ProducesResponseType(typeof(V3Result<V3Friend>), 200)]
    [ProducesResponseType(typeof(V3Result<V3Friend>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3Friend>>> AddFriends(Guid profileGuid1, Guid profileGuid2)
    {
        if (profileGuid1 == profileGuid2)
        {
            _logger.LogWarning("ProfileGuid1 cannot be the same as profileGuid2.");
            return BadRequest(new V3Result<V3Friend>("ProfileGuid1 cannot be the same as profileGuid2."));
        }

        // Uses comparison to make it so the same friendship cannot be added twice. 
        // Saves space by only having one friendship in the database instead of two.
        var comparisonResult = profileGuid1.CompareTo(profileGuid2);

        var sortedGuid1 = comparisonResult < 0 ? profileGuid1 : profileGuid2;
        var sortedGuid2 = comparisonResult < 0 ? profileGuid2 : profileGuid1;
        var existingFriendship = _db.Friend
            .FirstOrDefault(f => f.ProfileGuid1 == sortedGuid1 && f.ProfileGuid2 == sortedGuid2);

        if (existingFriendship != null)
        {
            _logger.LogWarning("Friendship already exists.");
            return BadRequest(new V3Result<V3Friend>("Friendship already exists."));
        }

        var friend = new Friend
        {
            ProfileGuid1 = comparisonResult < 0 ? profileGuid1 : profileGuid2,
            ProfileGuid2 = comparisonResult < 0 ? profileGuid2 : profileGuid1,
        };

        _db.Friend.Add(friend);
        await _db.SaveChangesAsync();

        var result = new V3Result<V3Friend>(new V3Friend
        {
            ProfileGuid1 = friend.ProfileGuid1,
            ProfileGuid2 = friend.ProfileGuid2
        });
        _logger.LogInformation($"Friendship created between {profileGuid1} and {profileGuid2}.");
        return result;
    }

    /// <summary>
    /// Gets friends for a specific profileGuid.
    /// </summary>
    /// <param name="profileGuid">The profileGuids which shall be searched for in the database.</param>
    /// <returns>A list of all the profileGuids that the provided profile is friends with.</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(V3Result<IEnumerable<Guid>>), 200)]
    [ProducesResponseType(typeof(V3Result<IEnumerable<Guid>>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<IEnumerable<Guid>>>> GetFriends(Guid profileGuid)
    {
        var friends = await _db.Friend
            .Where(f => f.ProfileGuid1 == profileGuid || f.ProfileGuid2 == profileGuid)
            .Select(f => f.ProfileGuid1 == profileGuid ? f.ProfileGuid2 : f.ProfileGuid1)
            .ToListAsync();

        if (friends.Count == 0)
        {
            _logger.LogWarning($"No friends found for user with Guid {profileGuid}.");
            return NotFound(new V3Result<IEnumerable<Guid>>($"No friends found for user with Guid {profileGuid}."));
        }
        else
        {
            return new V3Result<IEnumerable<Guid>>(friends);
        }
    }

    /// <summary>
    /// Checks if two profiles are friends.
    /// </summary>
    /// <param name="profileGuid1">Profile 1</param>
    /// <param name="profileGuid2">Profile 2</param>
    /// <returns>True or false, depending on whether they are friends or not.</returns>
    [HttpGet("FriendCheck")]
    [ProducesResponseType(typeof(V3Result<bool>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<bool>>> CheckIfFriends(Guid profileGuid1, Guid profileGuid2)
    {
        var friend = await _db.Friend
            .Where(f => (f.ProfileGuid1 == profileGuid1 && f.ProfileGuid2 == profileGuid2) ||
            (f.ProfileGuid1 == profileGuid2 && f.ProfileGuid2 == profileGuid1))
            .Select(f => new V3Friend()
            {
                ProfileGuid1 = profileGuid1,
                ProfileGuid2 = profileGuid2,
            })
            .SingleOrDefaultAsync();
        if (friend == null)
        {
            return new V3Result<bool>(false);
        }
        else
        {
            return new V3Result<bool>(true);
        }
    }

    /// <summary>
    /// Deletes a friend connection / friendship from the database.
    /// </summary>
    /// <param name="profileGuid1">profileGuid1 shall no longer be friends with profileGuid2. Order is irrelevant.</param>
    /// <param name="profileGuid2">profileGuid2 shall no longer be friends with profileGuid1. Order is irrelevant.</param>
    /// <returns>A friend object with both of the profileGuids which have now been deleted.</returns>
    [HttpDelete("")]
    [ProducesResponseType(typeof(V3Result<V3Friend>), 200)]
    [ProducesResponseType(typeof(V3Result<V3Friend>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3Friend>>> DeleteFriend(Guid profileGuid1, Guid profileGuid2)
    {
        var friend = await _db.Friend
            .Where(f => (f.ProfileGuid1 == profileGuid1 && f.ProfileGuid2 == profileGuid2) ||
                (f.ProfileGuid1 == profileGuid2 && f.ProfileGuid2 == profileGuid1))
            .SingleOrDefaultAsync();


        if (friend == null)
        {
            _logger.LogWarning($"Profiles {profileGuid1} and {profileGuid2} were not found as friends in the database.");
            return NotFound(new V3Result<V3Friend>($"Profiles {profileGuid1} and {profileGuid2} were not found as friends in the database."));
        }
        else
        {
            _db.Friend.Remove(friend);
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Profiles {profileGuid1} and {profileGuid2} were successfully removed as friends from the database.");
            return Ok(new V3Result<V3Friend>(new V3Friend()
            {
                ProfileGuid1 = friend.ProfileGuid1,
                ProfileGuid2 = friend.ProfileGuid2,
            }));
        }
    }
}
