namespace HIOF.GamingSocial.ProfileInformation.Data;

/// <summary>
/// A result object for a profile.
/// </summary>
public class Profile
{
    /// <summary>
    /// Gets or sets the unique identifier for the profile.
    /// </summary>
    public Guid ProfileGuid { get; set; }

    /// <summary>
    /// Gets or sets the username of the profile.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the name of the profile user.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the biography of the profile user.
    /// </summary>
    public string Bio { get; set; }

    /// <summary>
    /// Gets or sets the country of the profile user.
    /// </summary>
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets the age of the profile user.
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Gets or sets the URL of the profile picture.
    /// </summary>
    public string? PhotoUrl { get; set; }
}