namespace HIOF.GamingSocial.ProfileInformation.Model.V2;

public class V2GroupMembership
{
    public int GroupId { get; set; }
    public List<Guid> ProfileGuids { get; set; }

    public V2GroupMembership()
    {

    }
}
