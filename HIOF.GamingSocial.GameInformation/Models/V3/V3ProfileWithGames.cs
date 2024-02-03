namespace HIOF.GamingSocial.GameInformation.Models.V3;

/// <summary>
/// Class representing a user profile with a collection of games.
/// </summary>
public class V3ProfileWithGames
{
    /// <summary>
    /// Gets or sets the unique identifier for the user profile.
    /// </summary>
    public Guid ProfileGuid { get; set; }

    /// <summary>
    /// Gets or sets the collection of game ratings associated with the user profile.
    /// </summary>
    public List<V3GameRatings> GamesCollection { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="V3ProfileWithGames"/> class.
    /// </summary>
    public V3ProfileWithGames()
    {

    }

}

