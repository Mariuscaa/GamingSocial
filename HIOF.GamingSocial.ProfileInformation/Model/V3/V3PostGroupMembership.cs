namespace HIOF.GamingSocial.ProfileInformation.Model.V3;

/// <summary>
/// Represents the membership of a member within a group.
/// </summary>
public class V3PostGroupMembership
{
    /// <summary>
    /// The ID of the group.
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// The member who is part of the group.
    /// </summary>
    public V3Member Member { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="V3PostGroupMembership"/> class.
    /// </summary>
    public V3PostGroupMembership()
    {
    }
}
