namespace HIOF.GamingSocial.ProfileInformation.Model.V3;

/// <summary>
/// Represents a response object for a group.
/// </summary>
public class V3Group
{
    /// <summary>
    /// Gets or sets the unique identifier of the group.
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    public string GroupName { get; set; }

    /// <summary>
    /// Gets or sets the description of the group.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the group is hidden.
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the group is private.
    /// </summary>
    public bool IsPrivate { get; set; }

    /// <summary>
    /// Gets or sets the photo URL of the group.
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="V3Group"/> class.
    /// </summary>
    public V3Group()
    {
    }
}
