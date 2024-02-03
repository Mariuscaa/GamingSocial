namespace HIOF.GamingSocial.ProfileInformation.Model.V3;

/// <summary>
/// Represents an object with a groupId and all of its members.
/// </summary>
public class V3GroupMemberships
{
    /// <summary>
    /// Gets or sets the group ID.
    /// </summary>
    public int GroupId { get; set; }
    /// <summary>
    /// Gets or sets the members of the group.
    /// </summary>
    public List<V3Member> Members { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="V3GroupMemberships"/> class.
    /// </summary>
    public V3GroupMemberships()
    {

    }
}
