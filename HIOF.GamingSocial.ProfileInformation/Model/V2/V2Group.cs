using System.Text.Json.Serialization;

namespace HIOF.GamingSocial.ProfileInformation.Model.V2;

public class V2Group
{
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public V2Group() {}
}
