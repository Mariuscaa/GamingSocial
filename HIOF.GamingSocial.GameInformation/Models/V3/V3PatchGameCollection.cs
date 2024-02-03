namespace HIOF.GamingSocial.GameInformation.Models.V3;

/// <summary>
/// Represents a PATCH object for a game collection.
/// </summary>
public class V3PatchGameCollection
{
    /// <summary>
    /// Gets or sets the profile ID associated with the game collection.
    /// </summary>
    public Guid ProfileId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the game associated with the game collection.
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// Gets or sets the rating of the game associated with the game collection.
    /// </summary>
    public int? GameRating { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="V3PatchGameCollection"/> class.
    /// </summary>
    public V3PatchGameCollection()
    {
    }
}
