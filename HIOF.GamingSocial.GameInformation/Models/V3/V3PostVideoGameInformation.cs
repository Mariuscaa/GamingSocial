namespace HIOF.GamingSocial.GameInformation.Models.V3;

/// <summary>
/// Represents a videogame POST object.
/// </summary>
public class V3PostVideoGameInformation
{
    /// <summary>
    /// Gets or sets the game title.
    /// </summary>
    public string GameTitle { get; set; }

    /// <summary>
    /// Gets or sets the Steam App ID.
    /// </summary>
    public int? SteamAppId { get; set; }

    /// <summary>
    /// OLD SETUP. We kept it to allow for expansion into games outside of Steam.
    /// Gets or sets the Giant Bomb Guid.
    /// </summary>
    public string? GiantbombGuid { get; set; }

    /// <summary>
    /// Unused for now. We get information live from Steam Web API.
    /// Gets or sets the game description.
    /// </summary>
    public string? GameDescription { get; set; }
}
