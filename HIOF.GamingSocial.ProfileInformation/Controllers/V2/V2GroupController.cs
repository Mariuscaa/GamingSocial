using HIOF.GamingSocial.ProfileInformation.Data;
using HIOF.GamingSocial.ProfileInformation.Model;
using HIOF.GamingSocial.ProfileInformation.Model.V2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HIOF.GamingSocial.ProfileInformation.Data;

namespace HIOF.GamingSocial.ProfileInformation.Controllers.V2
{
    /// <summary>
    /// Handles groups.
    /// </summary>
    [Route("V2/Group")]
    [ApiController]
    public class V2GroupController : ControllerBase
    {
        private readonly ILogger<V2GroupController> _logger;
        private readonly ProfileInformationDbContext _db;
        public V2GroupController(ILogger<V2GroupController> logger, ProfileInformationDbContext db)
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
        [ProducesResponseType(typeof(V2Result<V2Group>), 200)]
        [ProducesResponseType(typeof(V2Result<V2Group>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<V2Result<V2Group>>> CreateGroup(V2PostGroup postGroup)
        {
            var group = new Group
            {
                GroupName = postGroup.GroupName
            };

            _db.Group.Add(group);
            await _db.SaveChangesAsync();

            var result = new V2Result<V2Group>(new V2Group
            {
                GroupId = group.GroupId,
                GroupName = postGroup.GroupName
            });
            return Ok(result);
        }

        /// <summary>
        /// Gets groups from the database. Can either get all groups or search for a group by name.
        /// </summary>
        /// <param name="searchString">Optional search string to look for the name of a group.</param>
        /// <returns>A list of groups.</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(V2Result<V2Group>), 200)]
        [ProducesResponseType(typeof(V2Result<V2Group>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<V2Result<IEnumerable<V2Group>>>> GetGroups(string? searchString = null)
        {
            if (searchString == null)
            {
                var responseGroups = await _db.Group
                    .Select(group => new V2Group
                    {
                        GroupId = group.GroupId,
                        GroupName = group.GroupName,
                    }).ToListAsync();

                return Ok(new V2Result<IEnumerable<V2Group>>(responseGroups));
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
                    })
                    .ToListAsync();

                responseGroups = responseGroups.Where(game =>
                    new string(game.GroupName.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToLower()
                        .Contains(searchStringAlphaNumeric))
                    .ToList();

                if (!responseGroups.Any())
                {
                    return BadRequest(new V2Result<IEnumerable<V2Group>>($"No groups found with the search `{searchString}`"));
                }

                List<V2Group> groups = new List<V2Group>();
                foreach ( var group in responseGroups )
                {
                    groups.Add(new V2Group()
                    {
                        GroupName = group.GroupName,
                        GroupId = group.GroupId
                    });
                }
                return new V2Result<IEnumerable<V2Group>>(groups);
            }
        }

        /// <summary>
        /// Gets the information saved about a single group.
        /// </summary>
        /// <param name="groupId">An unique id for a group.</param>
        /// <returns>An object for the found group.</returns>
        [HttpGet("{groupId}")]
        [ProducesResponseType(typeof(V2Result<V2Group>), 200)]
        [ProducesResponseType(typeof(V2Result<V2Group>), 404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<V2Result<V2Group>>> GetSingleGroup(int groupId)
        {
            var responseGroup = await _db.Group
                .Where(group => group.GroupId == groupId)
                .Select(group => new V2Group
                {
                    GroupId = group.GroupId,
                    GroupName = group.GroupName,
                })
                .SingleOrDefaultAsync();

            if (responseGroup == null)
            {
                return NotFound(new V2Result<V2Group>($"No groups found with the search`{groupId}`"));
            }

            return new V2Result<V2Group>(responseGroup);
        }

        /// <summary>
        /// Deletes groups from the database.
        /// </summary>
        /// <param name="groupId">The ID of the group which shall be deleted.</param>
        /// <returns>The deleted group.</returns>
        [HttpDelete("")]
        [ProducesResponseType(typeof(V2Result<V2Group>), 200)]
        [ProducesResponseType(typeof(V2Result<V2Group>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<V2Result<V2Group>>> DeleteGroup(int groupId)
        {
            try
            {
                var response = await _db.Group
                    .Where(group => group.GroupId == groupId)
                    .Select(group => new Group
                    {
                        GroupId = group.GroupId,
                        GroupName = group.GroupName
                    })
                    .SingleAsync();

                _db.Group.Remove(response);
                await _db.SaveChangesAsync();

                var deletedGroup = new V2Group()
                {

                    GroupId = response.GroupId,
                    GroupName = response.GroupName,
                };

                return Ok(new V2Result<V2Group>(deletedGroup));
            }
            catch (Exception ex)
            {
                return BadRequest(new V2Result<V2Group>("Could not find any group with ID equal to " + groupId));
            }
        }
    }
}
