namespace HIOF.GamingSocial.GUI.Model;

public class V3PostInvite
{

    public Guid SenderGuid { get; set; }
    public Guid ReceiverGuid { get; set; }
    public string InviteType { get; set; }
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the ID related to the invite. For example, if the invite is for a group, this would be the group ID.
    /// </summary>
    public int? RelatedId { get; set; }
}
