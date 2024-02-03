namespace HIOF.GamingSocial.GameInformation.Models.V3;

/// <summary>
/// Class representing a rating of game.
/// </summary>
public class V3GameRatings
{
    /// <summary>
    /// The ID of the game.
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// The rating of the game. Can be null.
    /// </summary>
    public int? GameRating { get; set; }

    /// <summary>
    /// Constructor for the V3GameRatings class.
    /// </summary>
    public V3GameRatings()
    {

    }
}
