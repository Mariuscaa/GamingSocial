namespace HIOF.GamingSocial.GUI.Model;

public class V3PatchGroup
{
    public int GroupId { get; set; }
    public string? GroupName { get; set; }
    public string? Description { get; set; }
    public bool IsHidden { get; set; }
    public bool IsPrivate { get; set; }
    public string? PhotoUrl { get; set; }
    public V3PatchGroup() { }
}

