using HIOF.GamingSocial.ProfileInformation.Data;
using HIOF.GamingSocial.ProfileInformation.Model.V3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HIOF.GamingSocial.ProfileInformation.Data;

namespace HIOF.GamingSocial.ProfileInformation.Controllers.V3;

/// <summary>
/// Controller that handles creation, editing, deleting and retrieval of groups
/// </summary>
[Route("V3/Group")]
[ApiController]
public class V3GroupController : ControllerBase
{
    private readonly ILogger<V3GroupController> _logger;
    private readonly ProfileInformationDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="V3GroupController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="db">The database context.</param>
    public V3GroupController(ILogger<V3GroupController> logger, ProfileInformationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Creates a single group with a name.
    /// </summary>
    /// <param name="postGroup">An object with the name of a group.</param>
    /// <returns>An object for the newly created group.</returns>
    [HttpPost("")]
    [ProducesResponseType(typeof(V3Result<V3Group>), 200)]
    [ProducesResponseType(typeof(V3Result<V3Group>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3Group>>> CreateGroup(V3PostGroup postGroup)
    {
        if (postGroup.GroupName.Length > 50)
        {
            _logger.LogWarning($"Invalid group name length: {postGroup.GroupName.Length}");
            return BadRequest(new V3Result<V3Group>("Invalid group name length. Must be less than 50 characters."));
        }

        if (postGroup.Description.Length > 500)
        {
            _logger.LogWarning($"Invalid description length: {postGroup.Description.Length}");
            return BadRequest(new V3Result<V3Group>("Invalid description length. Must be less than 500 characters."));
        }

        if (postGroup.PhotoUrl != null && postGroup.PhotoUrl.Length > 2048)
        {
            _logger.LogWarning($"Invalid picture url length: {postGroup.PhotoUrl.Length}");
            return BadRequest(new V3Result<V3Group>("Invalid picture url length. Must be less than 2048 characters."));
        }

        var existingGroup = await _db.Group
            .Where(group => group.GroupName == postGroup.GroupName)
            .FirstOrDefaultAsync();

        if (existingGroup != null)
        {
            _logger.LogWarning($"Group already exists: {existingGroup.GroupName}");
            return BadRequest(new V3Result<V3Group>("Group already exists."));
        }

        var group = new Group
        {
            GroupName = postGroup.GroupName,
            Description= postGroup.Description,
            IsHidden= postGroup.IsHidden,
            IsPrivate= postGroup.IsPrivate,
            PhotoUrl= postGroup.PhotoUrl,
        };

        _db.Group.Add(group);
        await _db.SaveChangesAsync();

        var result = new V3Result<V3Group>(new V3Group
        {
            GroupId= group.GroupId,
            GroupName = group.GroupName,
            Description = group.Description,
            IsHidden = group.IsHidden,
            IsPrivate = group.IsPrivate,
            PhotoUrl = group.PhotoUrl
        });

        _logger.LogInformation($"Group with id {group.GroupId} was created successfully.");
        return Ok(result);
    }

    /// <summary>
    /// Gets groups from the database. Can either get all groups or search for a group by name.
    /// </summary>
    /// <param name="searchString">Optional search string to look for the name of a group.</param>
    /// <param name="includeHidden">Optional search string to check if a group is hidden. It is true by default</param>
    /// <returns>A list of groups.</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(V3Result<V3Group>), 200)]
    [ProducesResponseType(typeof(V3Result<V3Group>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<IEnumerable<V3Group>>>> GetGroups(string? searchString = null, bool? includeHidden = true)
    {
        
        if (searchString == null && includeHidden == true)
        {
            var responseGroups = await _db.Group
                .Select(group => new V3Group
                {
                    GroupId = group.GroupId,
                    GroupName = group.GroupName,
                    Description = group.Description,
                    IsHidden = group.IsHidden,
                    IsPrivate = group.IsPrivate,
                    PhotoUrl = group.PhotoUrl
                }).ToListAsync();

            return Ok(new V3Result<IEnumerable<V3Group>>(responseGroups));
        }

        else if (searchString != null && includeHidden == false)
        {
            // Changes the search string to make it less strict.
            string searchStringAlphaNumeric = new string(searchString.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToLower();
            var responseGroups = await _db.Group
                .Where(group => group.IsHidden == false)
                .Select(group => new Group
                {
                    GroupId = group.GroupId,
                    GroupName = group.GroupName,
                    Description = group.Description,
                    IsHidden = group.IsHidden,
                    IsPrivate = group.IsPrivate,
                    PhotoUrl = group.PhotoUrl
                })
                .ToListAsync();

            responseGroups = responseGroups.Where(game =>
                new string(game.GroupName.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToLower()
                    .Contains(searchStringAlphaNumeric))
                .ToList();

            if (!responseGroups.Any())
            {
                _logger.LogWarning($"No groups found with the search `{searchString}`.");
                return NotFound(new V3Result<IEnumerable<V3Group>>($"No groups found with the search `{searchString}`."));
            }

            List<V3Group> groups = new List<V3Group>();
            foreach (var group in responseGroups)
            {
                groups.Add(new V3Group()
                {
                    GroupName = group.GroupName,
                    GroupId = group.GroupId,
                    Description = group.Description,
                    IsHidden = group.IsHidden,
                    IsPrivate = group.IsPrivate,
                    PhotoUrl = group.PhotoUrl,
                });
            }
            return new V3Result<IEnumerable<V3Group>>(groups);
        }
        else if (searchString == null && includeHidden == false)
        {
            var responseGroups = await _db.Group
                .Where(group => group.IsHidden == false)
                .Select(group => new V3Group
                {
                    GroupId = group.GroupId,
                    GroupName = group.GroupName,
                    Description = group.Description,
                    IsHidden = group.IsHidden,
                    IsPrivate = group.IsPrivate,
                    PhotoUrl = group.PhotoUrl
                }).ToListAsync();

            return Ok(new V3Result<IEnumerable<V3Group>>(responseGroups));
        }

        else
        {
            // Changes the search string to make it less strict.
            string searchStringAlphaNumeric = new string(searchString.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToLower();
            var responseGroups = await _db.Group
                .Select(group => new Group
                {
                    GroupId = group.GroupId,
                    GroupName = group.GroupName,
                    Description = group.Description,
                    IsHidden = group.IsHidden,
                    IsPrivate = group.IsPrivate,
                    PhotoUrl = group.PhotoUrl
                })
                .ToListAsync();

            responseGroups = responseGroups.Where(game =>
                new string(game.GroupName.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToLower()
                    .Contains(searchStringAlphaNumeric))
                .ToList();

            if (!responseGroups.Any())
            {
                _logger.LogWarning($"No groups found with the search `{searchString}`.");
                return NotFound(new V3Result<IEnumerable<V3Group>>($"No groups found with the search `{searchString}`."));
            }

            List<V3Group> groups = new List<V3Group>();
            foreach ( var group in responseGroups )
            {
                groups.Add(new V3Group()
                {
                    GroupName = group.GroupName,
                    GroupId = group.GroupId,
                    Description = group.Description,
                    IsHidden = group.IsHidden,
                    IsPrivate = group.IsPrivate,
                    PhotoUrl = group.PhotoUrl,
                });
            }
            return new V3Result<IEnumerable<V3Group>>(groups);
        }
    }

    /// <summary>
    /// Gets the information saved about a single group.
    /// </summary>
    /// <param name="groupId">An unique id for a group.</param>
    /// <returns>An object for the found group.</returns>
    [HttpGet("{groupId}")]
    [ProducesResponseType(typeof(V3Result<V3Group>), 200)]
    [ProducesResponseType(typeof(V3Result<V3Group>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3Group>>> GetSingleGroup(int groupId)
    {
        var responseGroup = await _db.Group
            .Where(group => group.GroupId == groupId)
            .Select(group => new V3Group
            {
                GroupId = group.GroupId,
                GroupName = group.GroupName,
                Description = group.Description,
                IsHidden = group.IsHidden,
                IsPrivate = group.IsPrivate,
                PhotoUrl = group.PhotoUrl,
            })
            .SingleOrDefaultAsync();

        if (responseGroup == null)
        {
            _logger.LogInformation($"No group found with the search `{groupId}`.");
            return NotFound(new V3Result<V3Group>($"No group found with the search `{groupId}`."));
        }

        return new V3Result<V3Group>(responseGroup);
    }

    /// <summary>
    /// Handles updates to existing groups
    /// </summary>
    /// <param name="groupId">The id of the group which shall be edited.</param>
    /// <param name="patchGroup">An object with the parameters which shall be changed. 
    /// Leave out or as null if you want to leave properties unchanged.</param>
    /// <returns>An object with the newly changed data.</returns>
    [HttpPatch("{groupId}")]
    [ProducesResponseType(typeof(V3Result<V3Group>), 200)]
    [ProducesResponseType(typeof(V3Result<V3Group>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3Group>>> Patch(int groupId, V3PatchGroup patchGroup)
    {
        var existingGroup = await _db.Group
            .SingleOrDefaultAsync(group => group.GroupId == groupId);

        if (existingGroup == null)
        {
            _logger.LogWarning($"No group found with the search for id`{groupId}`");
            return BadRequest(new V3Result<V3Profile>($"No group found with the search for id `{groupId}`"));
        }

        if (patchGroup.GroupName != null)
        {
            existingGroup.GroupName = patchGroup.GroupName;
        }

        if (patchGroup.Description != null)
        {
            existingGroup.Description = patchGroup.Description;
        }

        if (patchGroup.IsHidden != null)
        {
            existingGroup.IsHidden = (bool)patchGroup.IsHidden;
        }

        if (patchGroup.IsPrivate != null)
        {
            existingGroup.IsPrivate = (bool)patchGroup.IsPrivate;
        }

        if (patchGroup.PhotoUrl != null)
        {
            existingGroup.PhotoUrl = patchGroup.PhotoUrl;
        }

        _db.Group.Update(existingGroup);
        await _db.SaveChangesAsync();

        var updatedGroup = new V3Group()
        {
            GroupId = groupId,
            GroupName = existingGroup.GroupName,
            Description = existingGroup.Description,
            IsHidden = existingGroup.IsHidden,
            IsPrivate = existingGroup.IsPrivate,
            PhotoUrl = existingGroup.PhotoUrl,
        };

        _logger.LogInformation($"Updated group with id `{groupId}`");
        return Ok(new V3Result<V3Group>(updatedGroup));
    }

    /// <summary>
    /// Deletes groups from the database.
    /// </summary>
    /// <param name="groupId">The ID of the group which shall be deleted.</param>
    /// <returns>The deleted group.</returns>
    [HttpDelete("")]
    [ProducesResponseType(typeof(V3Result<V3Group>), 200)]
    [ProducesResponseType(typeof(V3Result<V3Group>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3Group>>> DeleteGroup(int groupId)
    {
        try
        {
            var response = await _db.Group
                .Where(group => group.GroupId == groupId)
                .Select(group => new Group
                {
                    GroupId = group.GroupId,
                    GroupName = group.GroupName,
                    Description = group.Description,
                    IsHidden = group.IsHidden,
                    IsPrivate = group.IsPrivate,
                    PhotoUrl = group.PhotoUrl,
                })
                .SingleAsync();

            _db.Group.Remove(response);
            await _db.SaveChangesAsync();

            var deletedGroup = new V3Group()
            {
                GroupId = response.GroupId,
                GroupName = response.GroupName,
                PhotoUrl= response.PhotoUrl,
                Description= response.Description,
                IsHidden= response.IsHidden,
                IsPrivate= response.IsPrivate
            };
            _logger.LogInformation($"Group {deletedGroup.GroupId} was deleted successfully.");

            return Ok(new V3Result<V3Group>(deletedGroup));
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Could not find any group with ID equal to {groupId}.");
            return NotFound(new V3Result<V3Group>("Could not find any group with ID equal to " + groupId));
        }
    }
}
