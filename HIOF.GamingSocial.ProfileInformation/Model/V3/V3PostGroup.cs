namespace HIOF.GamingSocial.ProfileInformation.Model.V3;

/// <summary>
/// Represents a POST group object which is used for the creation of groups.
/// </summary>
public class V3PostGroup
{
    /// <summary>
    /// Gets or sets the group name.
    /// </summary>
    public string GroupName { get; set; }

    /// <summary>
    /// Gets or sets the group description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets whether the group is hidden from search.
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Gets or sets whether the group is private (members are hidden).
    /// </summary>
    public bool IsPrivate { get; set; }

    /// <summary>
    /// Gets or sets the URL of the group photo.
    /// </summary>
    public string? PhotoUrl { get; set; }
}
