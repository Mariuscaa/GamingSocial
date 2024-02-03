using HIOF.GamingSocial.GUI.Model.ProfileInformation;

namespace HIOF.GamingSocial.GUI.Model;

public class InviteWithProfileAndGroup
{
    public V3Invite Invite { get; set; }
    public V3Profile SenderProfile { get; set; }
    public V3Group Group { get; set;}

    public InviteWithProfileAndGroup()
    {
    }
}
