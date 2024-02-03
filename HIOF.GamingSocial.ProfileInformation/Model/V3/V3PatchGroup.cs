namespace HIOF.GamingSocial.ProfileInformation.Model.V3;

/// <summary>
/// Represents a PATCH group object which is used to change existing groups.
/// </summary>
public class V3PatchGroup
{
    /// <summary>
    /// Gets or sets the id of the group.
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    public string? GroupName { get; set; }

    /// <summary>
    /// Gets or sets the description of the group.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the group is hidden or not.
    /// Used to hide the group from search.
    /// </summary>
    public bool? IsHidden { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the group is private or not.
    /// Used to hide group members.
    /// </summary>
    public bool? IsPrivate { get; set; }

    /// <summary>
    /// Gets or sets the photo url of the group.
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="V3PatchGroup"/> class.
    /// </summary>
    public V3PatchGroup() { }
}

