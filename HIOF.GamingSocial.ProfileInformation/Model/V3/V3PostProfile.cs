namespace HIOF.GamingSocial.ProfileInformation.Model.V3;

/// <summary>
/// Represents a POST object for the creation of profiles.
/// </summary>
public class V3PostProfile
{
    /// <summary>
    /// The unique username associated with the profile.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// The name of the user associated with the profile.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The bio section of the profile.
    /// </summary>
    public string Bio { get; set; }

    /// <summary>
    /// The country of the user associated with the profile.
    /// </summary>
    public string Country { get; set; }

    /// <summary>
    /// The age of the user associated with the profile.
    /// </summary>
    public int Age { get; set; }


    /// <summary>
    /// The URL for the user's photo.
    /// </summary>
    public string? PhotoUrl { get; set; }
}
