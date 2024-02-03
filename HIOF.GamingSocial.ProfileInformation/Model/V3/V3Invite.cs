namespace HIOF.GamingSocial.ProfileInformation.Model.V3;

/// <summary>
/// Represents a response object for invite related controllers.
/// </summary>
public class V3Invite
{
    /// <summary>
    /// Gets or sets the unique identifier of the invitation.
    /// </summary>
    public int InviteId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the sender of the invitation.
    /// </summary>
    public Guid SenderGuid { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the receiver of the invitation.
    /// </summary>
    public Guid ReceiverGuid { get; set; }

    /// <summary>
    /// Gets or sets the type of the invitation. Must be either "Friend" or "Group".
    /// </summary>
    public string InviteType { get; set; }

    /// <summary>
    /// Gets or sets the optional message sent along with the invitation.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the optional identifier related to the invitation.
    /// </summary>
    public int? RelatedId { get; set; }
}
