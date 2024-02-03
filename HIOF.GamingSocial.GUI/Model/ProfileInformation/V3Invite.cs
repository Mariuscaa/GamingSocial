namespace HIOF.GamingSocial.GUI.Model;

public class V3Invite
{
    public int InviteId { get; set; }
    public Guid SenderGuid { get; set; }
    public Guid ReceiverGuid { get; set; }
    public string InviteType { get; set; }
    public string? Message { get; set; }
    public int? RelatedId { get; set; }
}
