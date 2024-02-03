namespace HIOF.GamingSocial.ProfileInformation.Model.V3;

/// <summary>
/// A class representing a PATCH object for making changes to an existing profile.
/// All properties are nullable, and only the provided properties will be changed.
/// </summary>
public class V3PatchProfile
{
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the bio.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the age.
    /// </summary>
    public int? Age { get; set; }

    /// <summary>
    /// Gets or sets the photo URL.
    /// </summary>
    public string? PhotoUrl { get; set; }
}
