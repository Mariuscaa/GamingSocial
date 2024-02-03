namespace HIOF.GamingSocial.ProfileInformation.Model.V3;

/// <summary>
/// Represents a group member. This is used in the group membership class.
/// </summary>
public class V3Member
{
    /// <summary>
    /// Gets or sets the profile GUID for the member.
    /// </summary>
    public Guid ProfileGuid { get; set; }

    /// <summary>
    /// Gets or sets the member type. Must be either "Member", "Admin" or "Owner".
    /// </summary>
    /// <remarks>Defaults to "Member".</remarks>
    public string? MemberType { get; set; } = "Member";
}
