namespace HIOF.GamingSocial.GameInformation.Data;

/// <summary>
/// Represents a database object for videogame information.
/// </summary>
public class VideoGameInformation
{
    /// <summary>
    /// Gets or sets the game ID.
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// Gets or sets the steam app ID.
    /// </summary>
    public int? SteamAppId { get; set; }

    /// <summary>
    /// Gets or sets the game title.
    /// </summary>
    public string GameTitle { get; set; }

    /// <summary>
    /// This was used by old setup. Left it to allow for the implementation of games outside 
    /// of Steam at a later date. Gets or sets the giantbomb GUID.
    /// </summary>
    public string? GiantbombGuid { get; set; }

    /// <summary>
    /// Gets or sets the game description. This is not used at the moment, because 
    /// we get descriptions live from steam. Might be useful in the future.
    /// </summary>
    public string? GameDescription { get; set; }
}
