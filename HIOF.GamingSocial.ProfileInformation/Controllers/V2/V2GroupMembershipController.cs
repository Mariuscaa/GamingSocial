using HIOF.GamingSocial.ProfileInformation.Data;
using HIOF.GamingSocial.ProfileInformation.Model;
using HIOF.GamingSocial.ProfileInformation.Model.V2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HIOF.GamingSocial.ProfileInformation.Data;


namespace HIOF.GamingSocial.ProfileInformation.Controllers.V2
{
    /// <summary>
    /// Handles group memberships. This allows for profiles to be a part of groups.
    /// </summary>
    [Route("V2/GroupMembership")]
    [ApiController]
    public class V2GroupMembershipController : ControllerBase
    {
        private readonly ILogger<V2GroupMembershipController> _logger;
        private readonly ProfileInformationDbContext _db;
        public V2GroupMembershipController(ILogger<V2GroupMembershipController> logger, ProfileInformationDbContext db)
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
        [ProducesResponseType(typeof(V2Result<V2GroupMembership>), 200)]
        [ProducesResponseType(typeof(V2Result<V2GroupMembership>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<V2Result<V2GroupMembership>>> CreateGroupmembership(V2PostGroupMembership postGroupMembership)
        {
            if (postGroupMembership.GroupId < 1)
            {
                return BadRequest(new V2Result<V2GroupMembership>("Group id needs to be greater than 0"));
            }

            for (int i = 0; i < postGroupMembership.ProfileGuids.Count; i++)
            {
                var groupMembership = new GroupMembership
                {
                    GroupId = postGroupMembership.GroupId,
                    ProfileGuid = postGroupMembership.ProfileGuids[i]
                };
                _db.GroupMembership.Add(groupMembership);
                await _db.SaveChangesAsync();
            }


            var result = new V2Result<V2GroupMembership>(new V2GroupMembership
            {
                GroupId = postGroupMembership.GroupId,
                ProfileGuids = postGroupMembership.ProfileGuids
            });
            return Ok(result);
        }

        /// <summary>
        /// Gets all the group memberships currently in the database.
        /// </summary>
        /// <returns>A list of groups with members.</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(V2Result<V2GroupMembership>), 200)]
        [ProducesResponseType(typeof(V2Result<V2GroupMembership>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<V2Result<IEnumerable<V2GroupMembership>>>> GetGroupMemberships()
        {

            var responseGroupMemberships = await _db.GroupMembership
                .Select(responseGroupMemberships => new GroupMembership
                {
                    GroupId = responseGroupMemberships.GroupId,
                    ProfileGuid = responseGroupMemberships.ProfileGuid
                }).ToListAsync();

            List<V2GroupMembership> convertedList = new List<V2GroupMembership>();


            foreach (var groupMembership in responseGroupMemberships)
            {
                if (!convertedList.Any(gm => gm.GroupId == groupMembership.GroupId))
                {
                    convertedList.Add(new V2GroupMembership { GroupId = groupMembership.GroupId });
                }

                var objToUpdate = convertedList.Find(gm => gm.GroupId.Equals(groupMembership.GroupId));

                if (objToUpdate != null)
                {
                    // Append the new ProfileGuid to the ProfileGuids array
                    // If ProfileGuids is null, the entire expression will evaluate to null.
                    // We then use the null-coalescing operator ?? to provide an empty List<Guid> in case ProfileGuids is null
                    var newProfileGuids = objToUpdate.ProfileGuids?.ToList() ?? new List<Guid>();
                    newProfileGuids.Add(groupMembership.ProfileGuid);
                    objToUpdate.ProfileGuids = newProfileGuids.ToList();

                    // Update the object in the list
                    var indexToUpdate = convertedList.IndexOf(objToUpdate);
                    if (indexToUpdate != -1)
                    {
                        convertedList[indexToUpdate] = objToUpdate;
                    }
                }
            }

            return Ok(new V2Result<IEnumerable<V2GroupMembership>>(convertedList));
        }

        /// <summary>
        /// Gets all the members for a specified group.
        /// </summary>
        /// <param name="groupId">An unique Id for a group.</param>
        /// <returns>An object with the group id and a list of users.</returns>
        [HttpGet("{groupId}")]
        [ProducesResponseType(typeof(V2Result<V2GroupMembership>), 200)]
        [ProducesResponseType(typeof(V2Result<V2GroupMembership>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<V2Result<V2GroupMembership>>> GetSingleGroupMembership(int groupId)
        {

            var responseGroupMemberships = await _db.GroupMembership
                .Where(g => g.GroupId == groupId)
                .Select(responseGroupMemberships => new GroupMembership
                {
                    GroupId = responseGroupMemberships.GroupId,
                    ProfileGuid = responseGroupMemberships.ProfileGuid
                }).ToListAsync();

            if (responseGroupMemberships.Count == 0)
            {
                return BadRequest(new V2Result<V2GroupMembership>($"There are no groups with the ID {groupId} in the database."));
            }

            V2GroupMembership convertedGroupMembership = new V2GroupMembership();

            convertedGroupMembership.GroupId = groupId;
            convertedGroupMembership.ProfileGuids = new List<Guid>();

            foreach (var groupMembership in responseGroupMemberships)
            {
                convertedGroupMembership.ProfileGuids.Add(groupMembership.ProfileGuid);
            }

            return Ok(new V2Result<V2GroupMembership>(convertedGroupMembership));
        }

        /// <summary>
        /// Gets all the groups that a specified profile is a part of.
        /// </summary>
        /// <param name="profileGuid">A unique GUID for a profile.</param>
        /// <returns>An object with the user and a list with the groups they are a part of.</returns>
        [HttpGet("profile/{profileGuid}")]
        [ProducesResponseType(typeof(V2Result<V2ProfileGroups>), 200)]
        [ProducesResponseType(typeof(V2Result<V2ProfileGroups>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<V2Result<V2ProfileGroups>>> GetProfileGroups(Guid profileGuid)
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
                return BadRequest(new V2Result<V2ProfileGroups>($"There are no groups for the profile ID {profileGuid} in the database."));
            }

            V2ProfileGroups convertedProfileGroups = new V2ProfileGroups();

            convertedProfileGroups.ProfileGuid = profileGuid;
            convertedProfileGroups.GroupIds = new List<int>();

            foreach (var groupMembership in responseGroupMemberships)
            {
                convertedProfileGroups.GroupIds.Add(groupMembership.GroupId);
            }

            return Ok(new V2Result<V2ProfileGroups>(convertedProfileGroups));
        }


        /// <summary>
        /// Deletes group membership(s) from the database. 
        /// Can either delete a single profile from a group or delete all members of a group.
        /// </summary>
        /// <param name="profileGuid">Optional parameter to specify which profile to remove from a group.</param>
        /// <param name="groupId">Required parameter to specify which group to delete something from.</param>
        /// <returns>An object with the group id and a list of the deleted profiles.</returns>
        [HttpDelete("")]
        [ProducesResponseType(typeof(V2Result<V2GroupMembership>), 200)]
        [ProducesResponseType(typeof(V2Result<V2GroupMembership>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<V2Result<V2GroupMembership>>> DeleteGroupMembership(Guid? profileGuid = null, int? groupId = null)
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
                            })
                            .SingleAsync();
                    _db.GroupMembership.Remove(groupMembership);
                    await _db.SaveChangesAsync();

                    var deletedGroupMembership = new V2GroupMembership()
                    {
                        GroupId = (int)groupId,
                        ProfileGuids = new List<Guid> { groupMembership.ProfileGuid }
                    };

                    return Ok(new V2Result<V2GroupMembership>(deletedGroupMembership));
                }

                catch (Exception)
                {
                    return BadRequest(new V2Result<V2GroupMembership>($"Could not find the profile " + profileGuid + " in group " + groupId + "."));
                }
            }

            // Deletes all members in a group.
            else if(groupId != null && profileGuid == null)
            {
                var groupMemberships = await _db.GroupMembership
                            .Where(gm => gm.GroupId == groupId)
                            .Select(gm => new GroupMembership
                            {
                                ProfileGuid = gm.ProfileGuid,
                                GroupId = gm.GroupId,
                            })
                            .ToListAsync();

                if (groupMemberships.IsNullOrEmpty())
                {
                    return BadRequest(new V2Result<V2GroupMembership>("Could not find any group with the ID " + groupId));
                }

                var profileGuids = new List<Guid>();
                foreach (var groupMembership in groupMemberships)
                {
                    _db.GroupMembership.Remove(groupMembership);
                    profileGuids.Add(groupMembership.ProfileGuid);
                }
                await _db.SaveChangesAsync();

                var deletedGroupMemberships = new V2GroupMembership()
                {
                    GroupId = (int)groupId,
                    ProfileGuids = profileGuids
                };

                return Ok(new V2Result<V2GroupMembership>(deletedGroupMemberships));
            }
            else {
                return BadRequest(new V2Result<V2GroupMembership>("You have to include groupId. profileId is optional."));
            }
        }
    }
}
