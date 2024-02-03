
namespace HIOF.GamingSocial.ProfileInformation.Data;

/// <summary>
/// Represents an object with a groupId and all of its members.
/// </summary>
public class GroupMembership
{
    /// <summary>
    /// Gets or sets the group ID.
    /// </summary>
    public Guid ProfileGuid { get; set; }

    /// <summary>
    /// Gets or sets the members of the group.
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// Gets or sets the member type. Must be either "Member", "Admin" or "Owner".
    /// </summary>
    /// <remarks>Defaults to "Member".</remarks>
    public string MemberType { get; set; }
}
