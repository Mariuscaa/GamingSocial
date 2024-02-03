namespace HIOF.GamingSocial.ProfileInformation.Model.V3;

/// <summary>
/// Class representing a collection of groups
/// </summary>
public class V3ProfileGroups
{
    /// <summary>
    /// The ProfileGuid of the user.
    /// </summary>
    public Guid ProfileGuid { get; set; }

    /// <summary>
    /// Gets or sets the list of GroupIds in the collection.
    /// </summary>
    public List<int> GroupIds { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="V3ProfileGroups"/> class.
    /// </summary>
    public V3ProfileGroups() { }

}
