namespace HIOF.GamingSocial.ProfileInformation.Model.V3;

/// <summary>
/// A result object for a profile.
/// </summary>
public class V3Profile
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

    /// <summary>
    /// Initializes a new instance of the <see cref="V3Profile"/> class.
    /// </summary>
    public V3Profile() { }
}
