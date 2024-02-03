namespace HIOF.GamingSocial.GameInformation.Models.V3;

/// <summary>
/// Class representing a videogame information return object.
/// </summary>
public class V3VideoGameInformation
{
    /// <summary>
    /// The id of the video game.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The Steam app id of the video game.
    /// </summary>
    public int? SteamAppId { get; set; }

    /// <summary>
    /// The title of the video game.
    /// </summary>
    public string GameTitle { get; set; }

    /// <summary>
    /// OLD SETUP. Kept it to allow for the use of games outside of Steam in the future.
    /// A unique Guid from the giant bomb web api.
    /// </summary>
    public string? GiantbombGuid { get; set; }

    /// <summary>
    /// Currently unused as we get information live from steam web api.
    /// The description of the video game.
    /// </summary>
    public string? GameDescription { get; set; }
}
