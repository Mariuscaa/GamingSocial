namespace HIOF.GamingSocial.ProfileInformation.Model.V3;

/// <summary>
/// Represents a POST invite object which is used for the creation of invites.
/// Can be used for either friends or groups.
/// </summary>
public class V3PostInvite
{
    /// <summary>
    /// Gets or sets the GUID of the invite sender.
    /// </summary>
    public Guid SenderGuid { get; set; }

    /// <summary>
    /// Gets or sets the GUID of the invite receiver.
    /// </summary>
    public Guid ReceiverGuid { get; set; }

    /// <summary>
    /// Gets or sets the type of the invite. Must be either "Friend" or "Group" atm.
    /// </summary>
    public string InviteType { get; set; }

    /// <summary>
    /// Gets or sets the message of the invite.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the ID related to the invite. For example, if the invite is for a group, this would be the group ID.
    /// Is null for friends.
    /// </summary>
    public int? RelatedId { get; set; }
}
