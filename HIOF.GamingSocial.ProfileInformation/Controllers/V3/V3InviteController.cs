using HIOF.GamingSocial.ProfileInformation.Data;
using HIOF.GamingSocial.ProfileInformation.Model.V3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HIOF.GamingSocial.ProfileInformation.Data;

namespace HIOF.GamingSocial.ProfileInformation.Controllers.V3;

/// <summary>
/// Handles all invite related requests. This is only used for Friend and Group invites at the moment.
/// </summary>
[ApiController]
[Route("V3/Invite")]
public class V3InviteController : Controller
{
    private readonly ILogger<V3InviteController> _logger;
    private readonly ProfileInformationDbContext _db;
    private readonly string[] _validInviteTypes = { "Friend", "Group" };

    /// <summary>
    /// Initializes a new instance of the <see cref="V3InviteController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="db">The database context.</param>
    public V3InviteController(ILogger<V3InviteController> logger, ProfileInformationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Posts a new invite to the database.
    /// </summary>
    /// <param name="postInvite">An object with all the necessary properties to identify an invite. 
    /// RelatedId is used for other ID's such as group id (can be null for friend invite).</param>
    /// <returns>An object with the newly created database object.</returns>
    [HttpPost("")]
    [ProducesResponseType(typeof(V3Result<V3Invite>), 200)]
    [ProducesResponseType(typeof(V3Result<V3Invite>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3Invite>>> CreateInvite(V3PostInvite postInvite)
    {
        if (postInvite.SenderGuid == postInvite.ReceiverGuid)
        {
            _logger.LogWarning("Sender cannot be the same as reciever.");
            return BadRequest(new V3Result<V3Invite>("Sender cannot be the same as Reciever."));
        }

        if (!_validInviteTypes.Contains(postInvite.InviteType))
        {
            _logger.LogWarning($"Invalid invite type: {postInvite.InviteType}");
            return BadRequest(new V3Result<V3Invite>("Invalid invite type. Must be either 'Friend' or 'Group'."));
        }

        if (postInvite.Message != null && postInvite.Message.Length > 200)
        {
            _logger.LogWarning($"Invalid message length: {postInvite.Message.Length}");
            return BadRequest(new V3Result<V3Invite>("Invalid message length. Must be less than 200 characters."));
        }

        var existingInvite = await _db.Invite
            .Where(invite => invite.SenderGuid == postInvite.SenderGuid 
                && invite.ReceiverGuid == postInvite.ReceiverGuid && invite.InviteType == postInvite.InviteType && invite.RelatedId == postInvite.RelatedId)
            .FirstOrDefaultAsync();
        if (existingInvite != null)
        {
            _logger.LogWarning($"Invite already exists: {existingInvite.InviteId}");
            return BadRequest(new V3Result<V3Invite>("Invite already exists."));
        }

        if (postInvite.InviteType == "Group")
        {
            if (postInvite.RelatedId == null || postInvite.RelatedId <= 0)
            {
                _logger.LogWarning($"Invalid related id: {postInvite.RelatedId}");
                return BadRequest(new V3Result<V3Invite>("Must include related Id if invitetype is 'Group'. Must also be greater than 0."));
            }
        }

        var invite = new Invite
        {
            SenderGuid = postInvite.SenderGuid,
            ReceiverGuid = postInvite.ReceiverGuid,
            InviteType = postInvite.InviteType,
            Message = postInvite.Message,
            RelatedId = postInvite.RelatedId
        };

        _db.Invite.Add(invite);
        await _db.SaveChangesAsync();

        var result = new V3Result<V3Invite>(new V3Invite
        {
            InviteId = invite.InviteId,
            SenderGuid = invite.SenderGuid,
            ReceiverGuid = invite.ReceiverGuid,
            InviteType = invite.InviteType,
            Message = invite.Message,
            RelatedId = invite.RelatedId
        });

        _logger.LogInformation($"{invite.InviteType} invite from {invite.SenderGuid} to {invite.ReceiverGuid} was created successfully.");
        return Ok(result);
    }

    /// <summary>
    /// Gets invites from the database. Filter by senderGuid or/and receiverGuid.
    /// </summary>
    /// <param name="senderGuid">Guid to look for as sender.</param>
    /// <param name="receiverGuid">Guid to look for as receiver.</param>
    /// <returns>A list of invites based on the parameters.</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(V3Result<IEnumerable<V3Invite>>), 200)]
    [ProducesResponseType(typeof(V3Result<IEnumerable<V3Invite>>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<IEnumerable<V3Invite>>>> GetInvites(Guid? senderGuid = null, Guid? receiverGuid = null)
    {
        if (senderGuid != null && receiverGuid != null)
        {
            var invites = await _db.Invite
                .Where(invite => invite.SenderGuid == senderGuid && invite.ReceiverGuid == receiverGuid)
                .Select(invite => new V3Invite
                {
                    InviteId = invite.InviteId,
                    SenderGuid = (Guid)senderGuid,
                    ReceiverGuid = (Guid)receiverGuid,
                    InviteType = invite.InviteType,
                    Message = invite.Message,
                    RelatedId = invite.RelatedId
                })
                .ToListAsync();

            return Ok(new V3Result<IEnumerable<V3Invite>>(invites));
        }

        else if (receiverGuid == null && senderGuid != null)
        {
            var invites = await _db.Invite
                .Where(invite => invite.SenderGuid == senderGuid)
                .Select(invite => new V3Invite
                {
                    InviteId = invite.InviteId,
                    SenderGuid = (Guid)senderGuid,
                    ReceiverGuid = invite.ReceiverGuid,
                    InviteType = invite.InviteType,
                    Message = invite.Message,
                    RelatedId = invite.RelatedId
                })
                .ToListAsync();

            return Ok(new V3Result<IEnumerable<V3Invite>>(invites));
        }
        else if (receiverGuid != null && senderGuid == null)
        {
            var invites = await _db.Invite
                .Where(invite => invite.ReceiverGuid == receiverGuid)
                .Select(invite => new V3Invite
                {
                    InviteId = invite.InviteId,
                    SenderGuid = invite.SenderGuid,
                    ReceiverGuid = (Guid)receiverGuid,
                    InviteType = invite.InviteType,
                    Message = invite.Message,
                    RelatedId = invite.RelatedId
                })
                .ToListAsync();

            return Ok(new V3Result<IEnumerable<V3Invite>>(invites));
        }
        else
        {
            _logger.LogWarning("Did not provide senderGuid or receiverGuid.");
            return BadRequest(new V3Result<IEnumerable<V3Invite>>("Must provide either a senderGuid or receiverGuid (or both)."));
        }
    }

    /// <summary>
    /// Deletes an invite from the database.
    /// </summary>
    /// <param name="inviteId">The id for the invite.</param>
    /// <returns>An object to represent the deleted data.</returns>
    [HttpDelete("")]
    [ProducesResponseType(typeof(V3Result<V3Invite>), 200)]
    [ProducesResponseType(typeof(V3Result<V3Invite>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3Invite>>> Delete(int inviteId)
    {
        var invite = await _db.Invite
                .Where(inv => inv.InviteId == inviteId)
                .Select(inv => new Invite
                {
                    InviteId = inv.InviteId,
                    SenderGuid = inv.SenderGuid,
                    ReceiverGuid = inv.ReceiverGuid,
                    InviteType = inv.InviteType,
                    Message = inv.Message,
                    RelatedId = inv.RelatedId
                })
                .SingleOrDefaultAsync();

        if (invite == null)
        {
            _logger.LogWarning($"Could not find any invite with inviteId: {inviteId}");
            return NotFound(new V3Result<V3Invite>($"Could not find any invite with inviteId: {inviteId}"));
        }

        _db.Invite.Remove(invite);
        await _db.SaveChangesAsync();

        var removedInvite = new V3Invite()
        {
            InviteId = invite.InviteId,
            SenderGuid = invite.SenderGuid,
            ReceiverGuid = invite.ReceiverGuid,
            InviteType = invite.InviteType,
            Message = invite.Message,
            RelatedId = invite.RelatedId
        };
        return Ok(new V3Result<V3Invite>(removedInvite));
    }
}
