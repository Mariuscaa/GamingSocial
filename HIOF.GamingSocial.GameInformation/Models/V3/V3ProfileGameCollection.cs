namespace HIOF.GamingSocial.GameInformation.Models.V3;

/// <summary>
/// Represents a single game associated with a profile.
/// </summary>
public class V3ProfileGameCollection
{
    /// <summary>
    /// Gets or sets the profile id.
    /// </summary>
    public Guid ProfileId { get; set; }

    /// <summary>
    /// Gets or sets the game id.
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="V3ProfileGameCollection"/> class.
    /// </summary>
    public V3ProfileGameCollection()
    {
    }
}
