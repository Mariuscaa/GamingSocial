using HIOF.GamingSocial.ProfileInformation.Data;
using HIOF.GamingSocial.ProfileInformation.Model.V3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HIOF.GamingSocial.ProfileInformation.Data;


namespace HIOF.GamingSocial.ProfileInformation.Controllers.V3;

/// <summary>
/// Handles group memberships. This allows for profiles to be a part of groups.
/// </summary>
[Route("V3/GroupMembership")]
[ApiController]
public class V3GroupMembershipController : ControllerBase
{
    private readonly ILogger<V3GroupMembershipController> _logger;
    private readonly ProfileInformationDbContext _db;
    private readonly string[] _validMemberTypes = { "Member", "Admin", "Owner" };


    /// <summary>
    /// Initializes a new instance of the <see cref="V3GroupMembershipController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="db">The database context.</param>
    public V3GroupMembershipController(ILogger<V3GroupMembershipController> logger, ProfileInformationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Adds a user or multiple users to a group.
    /// </summary>
    /// <param name="postGroupMembership">An object with the users and the specified group they shall be a part of.</param>
    /// <returns>An object which contains the group id and a list of the newly added users.</returns>
    [HttpPost("")]
    [ProducesResponseType(typeof(V3Result<V3GroupMemberships>), 200)]
    [ProducesResponseType(typeof(V3Result<V3GroupMemberships>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3GroupMemberships>>> CreateGroupMembership(V3PostGroupMembership postGroupMembership)
    {
        // Check if group membership already exists.
        var groupMembershipExists = await _db.GroupMembership
            .AnyAsync(gm => gm.GroupId == postGroupMembership.GroupId && gm.ProfileGuid == postGroupMembership.Member.ProfileGuid);
        if (groupMembershipExists)
        {
            _logger.LogWarning($"{postGroupMembership.Member.ProfileGuid} already exists in group {postGroupMembership.GroupId}");
            return BadRequest(new V3Result<V3GroupMemberships>("User is already a member of this group."));

        }

        if (postGroupMembership.GroupId < 1)
        {
            _logger.LogWarning($"Group id needs to be greater than 0");
            return BadRequest(new V3Result<V3GroupMemberships>("Group id needs to be greater than 0"));
        }


        if (!_validMemberTypes.Contains(postGroupMembership.Member.MemberType))
        {
            _logger.LogWarning("Membertype was not 'Owner', 'Admin' or 'Member'.");
            return BadRequest(new V3Result<V3GroupMemberships>("Membertype must be 'Owner', 'Admin' or 'Member'."));
        }

        var groupMembership = new GroupMembership
        {
            GroupId = postGroupMembership.GroupId,
            ProfileGuid = postGroupMembership.Member.ProfileGuid,
            MemberType = postGroupMembership.Member.MemberType
        };
        _db.GroupMembership.Add(groupMembership);

        await _db.SaveChangesAsync();

        var result = new V3Result<V3GroupMemberships>(new V3GroupMemberships
        {
            GroupId = postGroupMembership.GroupId,
            Members = new List<V3Member>()
            {
                postGroupMembership.Member
            }
        });
        return Ok(result);
    }

    /// <summary>
    /// Gets all the group memberships currently in the database.
    /// </summary>
    /// <returns>A list of groups with members.</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(V3Result<V3GroupMemberships>), 200)]
    [ProducesResponseType(typeof(V3Result<V3GroupMemberships>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<IEnumerable<V3GroupMemberships>>>> GetGroupMemberships()
    {
        var responseGroupMemberships = await _db.GroupMembership
            .Select(responseGroupMemberships => new GroupMembership
            {
                GroupId = responseGroupMemberships.GroupId,
                ProfileGuid = responseGroupMemberships.ProfileGuid,
                MemberType = responseGroupMemberships.MemberType,
            }).ToListAsync();

        // The database has each member in every group saved as one line.
        // This part of the method puts the members in the correct group before returning as a list of V3GroupMemberships.
        List<V3GroupMemberships> convertedList = new List<V3GroupMemberships>();
        foreach (var groupMembership in responseGroupMemberships)
        {
            if (!convertedList.Any(gm => gm.GroupId == groupMembership.GroupId))
            {
                convertedList.Add(new V3GroupMemberships { GroupId = groupMembership.GroupId });
            }

            var objToUpdate = convertedList.Find(gm => gm.GroupId.Equals(groupMembership.GroupId));

            if (objToUpdate != null)
            {
                // Append the new ProfileGuid to the ProfileGuids array
                // If ProfileGuids is null, the entire expression will evaluate to null.
                // We then use the null-coalescing operator ?? to provide an empty List<Guid> in case ProfileGuids is null

                var newProfileMembers = objToUpdate.Members?.ToList() ?? new List<V3Member>();
                newProfileMembers.Add(new V3Member()
                {
                    ProfileGuid = groupMembership.ProfileGuid,
                    MemberType = groupMembership.MemberType
                });
                objToUpdate.Members = newProfileMembers.ToList();

                // Update the object in the list
                var indexToUpdate = convertedList.IndexOf(objToUpdate);
                if (indexToUpdate != -1)
                {

                    convertedList[indexToUpdate] = objToUpdate;
                }
            }
        }

        return Ok(new V3Result<IEnumerable<V3GroupMemberships>>(convertedList));
    }

    /// <summary>
    /// Gets all the members for a specified group.
    /// </summary>
    /// <param name="groupId">An unique Id for a group.</param>
    /// <param name="profileGuid">An optional unique Guid for a profile.</param>
    /// <returns>An object with the group id and a list of users.</returns>
    [HttpGet("{groupId}")]
    [ProducesResponseType(typeof(V3Result<V3GroupMemberships>), 200)]
    [ProducesResponseType(typeof(V3Result<V3GroupMemberships>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3GroupMemberships>>> GetSingleGroupMembership(int groupId, Guid? profileGuid = null)
    {
        if (profileGuid == null)
        {
            var responseGroupMemberships = await _db.GroupMembership
            .Where(g => g.GroupId == groupId)
            .Select(responseGroupMemberships => new GroupMembership
            {
                GroupId = responseGroupMemberships.GroupId,
                ProfileGuid = responseGroupMemberships.ProfileGuid,
                MemberType = responseGroupMemberships.MemberType,
            }).ToListAsync();

            if (responseGroupMemberships.Count == 0)
            {
                _logger.LogWarning($"There are no group memberships with the group ID {groupId} in the database.");
                return NotFound(new V3Result<V3GroupMemberships>($"There are no group memberships with the group ID {groupId} in the database."));
            }
            V3GroupMemberships convertedGroupMembership = new V3GroupMemberships();

            convertedGroupMembership.GroupId = groupId;
            convertedGroupMembership.Members = new List<V3Member>();

            foreach (var groupMembership in responseGroupMemberships)
            {
                convertedGroupMembership.Members.Add(new V3Member()
                {
                    ProfileGuid = groupMembership.ProfileGuid,
                    MemberType = groupMembership.MemberType,
                });
            }

            return Ok(new V3Result<V3GroupMemberships>(convertedGroupMembership));
        }
        else
        {
            var responseGroupMemberships = await _db.GroupMembership
            .Where(g => g.GroupId == groupId && g.ProfileGuid == profileGuid)
            .Select(responseGroupMemberships => new GroupMembership
            {
                GroupId = responseGroupMemberships.GroupId,
                ProfileGuid = responseGroupMemberships.ProfileGuid,
                MemberType = responseGroupMemberships.MemberType,
            }).ToListAsync();

            if (responseGroupMemberships.Count == 0)
            {
                _logger.LogWarning($"Profile {profileGuid} is not a member of {groupId}.");
                return NotFound(new V3Result<V3GroupMemberships>($"Profile {profileGuid} is not a member of {groupId}."));
            }
            V3GroupMemberships convertedGroupMembership = new V3GroupMemberships();

            convertedGroupMembership.GroupId = groupId;
            convertedGroupMembership.Members = new List<V3Member>();

            foreach (var groupMembership in responseGroupMemberships)
            {
                convertedGroupMembership.Members.Add(new V3Member()
                {
                    ProfileGuid = groupMembership.ProfileGuid,
                    MemberType = groupMembership.MemberType,
                });
            }
            return Ok(new V3Result<V3GroupMemberships>(convertedGroupMembership));
        }
    }

    /// <summary>
    /// Gets all the groups that a specified profile is a part of.
    /// </summary>
    /// <param name="profileGuid">A unique GUID for a profile.</param>
    /// <returns>An object with the user and a list with the groups they are a part of.</returns>
    [HttpGet("profile/{profileGuid}")]
    [ProducesResponseType(typeof(V3Result<V3ProfileGroups>), 200)]
    [ProducesResponseType(typeof(V3Result<V3ProfileGroups>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3ProfileGroups>>> GetProfileGroups(Guid profileGuid)
    {
        var responseGroupMemberships = await _db.GroupMembership
            .Where(p => p.ProfileGuid == profileGuid)
            .Select(responseGroupMemberships => new GroupMembership
            {
                GroupId = responseGroupMemberships.GroupId,
                ProfileGuid = responseGroupMemberships.ProfileGuid
            }).ToListAsync();

        if (responseGroupMemberships.Count == 0)
        {
            _logger.LogWarning($"There are no groups for the profile ID {profileGuid} in the database.");
            return NotFound(new V3Result<V3ProfileGroups>($"There are no groups for the profile ID {profileGuid} in the database."));
        }

        V3ProfileGroups convertedProfileGroups = new V3ProfileGroups();

        convertedProfileGroups.ProfileGuid = profileGuid;
        convertedProfileGroups.GroupIds = new List<int>();

        foreach (var groupMembership in responseGroupMemberships)
        {
            convertedProfileGroups.GroupIds.Add(groupMembership.GroupId);
        }

        return Ok(new V3Result<V3ProfileGroups>(convertedProfileGroups));
    }

    /// <summary>
    /// Handles updates of existing group memberships. Meaning changes to the member type.
    /// </summary>
    /// <param name="groupId">The id of the group which shall be updated.</param>
    /// <param name="profileGuid">The guid of the member which shall have their member type updated.</param>
    /// <param name="memberType">A string for the desired member type for the member. Either "Owner", "Admin" or "Member".</param>
    /// <returns>Returns an object with the changed data.</returns>
    [HttpPatch("")]
    [ProducesResponseType(typeof(V3Result<V3GroupMemberships>), 200)]
    [ProducesResponseType(typeof(V3Result<V3GroupMemberships>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3GroupMemberships>>> PatchMemberType(int groupId, Guid profileGuid, string memberType)
    {
        var existingGroupMembership = await _db.GroupMembership
            .SingleOrDefaultAsync(gm => gm.ProfileGuid == profileGuid && gm.GroupId == groupId);

        if (existingGroupMembership == null)
        {
            _logger.LogWarning($"No groupmembership found with the search for `{profileGuid}` in group {groupId}");
            return BadRequest(new V3Result<V3GroupMemberships>($"No groupmembership found with the search for `{profileGuid}` in group {groupId}"));
        }
        else
        {
            if (!_validMemberTypes.Contains(memberType))
            {
                _logger.LogWarning("Membertype was not 'Owner', 'Admin' or 'Member'.");
                return BadRequest(new V3Result<V3GroupMemberships>("Membertype must be 'Owner', 'Admin' or 'Member'."));
            }
            else
            {
                existingGroupMembership.MemberType = memberType;
            }
        }

        _db.GroupMembership.Update(existingGroupMembership);
        await _db.SaveChangesAsync();

        var updatedGroupMembership = new V3GroupMemberships()
        {
            GroupId = groupId,
            Members = new List<V3Member>()
            {
                new V3Member()
                {
                    ProfileGuid = profileGuid,
                    MemberType = memberType
                }
            }
        };

        _logger.LogInformation($"Updated groupmembership to {memberType} for profile with id `{profileGuid}` in group {groupId}");
        return Ok(new V3Result<V3GroupMemberships>(updatedGroupMembership));
    }

    /// <summary>
    /// Deletes group membership(s) from the database. 
    /// Can either delete a single profile from a group or delete all members of a group.
    /// </summary>
    /// <param name="profileGuid">Optional parameter to specify which profile to remove from a group.</param>
    /// <param name="groupId">Required parameter to specify which group to delete something from.</param>
    /// <returns>An object with the group id and a list of the deleted profiles.</returns>
    [HttpDelete("")]
    [ProducesResponseType(typeof(V3Result<V3GroupMemberships>), 200)]
    [ProducesResponseType(typeof(V3Result<V3GroupMemberships>), 400)]
    [ProducesResponseType(typeof(V3Result<V3GroupMemberships>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3GroupMemberships>>> DeleteGroupMembership(Guid? profileGuid = null, int? groupId = null)
    {
        if (profileGuid != null && groupId != null)
        {
            try
            {
                var groupMembership = await _db.GroupMembership
                        .Where(gm => gm.GroupId == groupId && gm.ProfileGuid == profileGuid)
                        .Select(gm => new GroupMembership
                        {
                            ProfileGuid = gm.ProfileGuid,
                            GroupId = gm.GroupId,
                            MemberType = gm.MemberType
                        })
                        .SingleAsync();
                _db.GroupMembership.Remove(groupMembership);
                await _db.SaveChangesAsync();

                var deletedGroupMembership = new V3GroupMemberships()
                {
                    GroupId = (int)groupId,
                    Members = new List<V3Member>() {
                        new V3Member() {
                            ProfileGuid = groupMembership.ProfileGuid,
                            MemberType = groupMembership.MemberType
                            }
                    }
                };
                _logger.LogInformation($"{profileGuid} was successfully removed from {groupId}.");
                return Ok(new V3Result<V3GroupMemberships>(deletedGroupMembership));
            }

            catch (Exception)
            {
                _logger.LogWarning($"Could not find the profile {profileGuid} in group {groupId}.");
                return NotFound(new V3Result<V3GroupMemberships>($"Could not find the profile " + profileGuid + " in group " + groupId + "."));
            }
        }

        // Deletes all members in a group.
        else if (groupId != null && profileGuid == null)
        {
            var groupMemberships = await _db.GroupMembership
                        .Where(gm => gm.GroupId == groupId)
                        .Select(gm => new GroupMembership
                        {
                            ProfileGuid = gm.ProfileGuid,
                            GroupId = gm.GroupId,
                            MemberType = gm.MemberType
                        })
                        .ToListAsync();

            if (groupMemberships.IsNullOrEmpty())
            {
                _logger.LogWarning($"Could not find any group with the ID {groupId} with any members.");
                return NotFound(new V3Result<V3GroupMemberships>("Could not find any group with the ID " + groupId + " with any members."));
            }

            var groupMembers = new List<V3Member>();
            foreach (var groupMembership in groupMemberships)
            {
                _db.GroupMembership.Remove(groupMembership);
                groupMembers.Add(new V3Member()
                {
                    ProfileGuid = groupMembership.ProfileGuid,
                    MemberType = groupMembership.MemberType,
                });
            }
            await _db.SaveChangesAsync();

            var deletedGroupMemberships = new V3GroupMemberships()
            {
                GroupId = (int)groupId,
                Members = groupMembers
            };
            _logger.LogInformation($"Deleted all members in group {groupId}.");
            return Ok(new V3Result<V3GroupMemberships>(deletedGroupMemberships));
        }
        else
        {
            _logger.LogWarning("Method called without groupId.");
            return BadRequest(new V3Result<V3GroupMemberships>("You have to include groupId. profileId is optional."));
        }
    }
}
