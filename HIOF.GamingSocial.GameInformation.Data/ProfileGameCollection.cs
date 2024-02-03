namespace HIOF.GamingSocial.GameInformation.Data;

/// <summary>
/// Represents a collection of games associated with a user profile.
/// </summary>
public class ProfileGameCollection
{
    /// <summary>
    /// Gets or sets the unique identifier for the profile.
    /// </summary>
    public Guid ProfileId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the game.
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// Gets or sets the optional rating for the game.
    /// </summary>
    public int? GameRating { get; set; }
}
    
