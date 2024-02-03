namespace HIOF.GamingSocial.PublicGameInformation.Model.V2;

/// <summary>
/// Represents details of a game on Steam API version 2. 
/// This is used to extract the desired data from the steam response.
/// </summary>
public class V2SteamGameDetails
{
    /// <summary>
    /// Gets or sets the title of the game.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the Steam app ID of the game.
    /// </summary>
    public int SteamAppId { get; set; }

    /// <summary>
    /// Gets or sets a brief summary of the game.
    /// </summary>
    public string ShortDescription { get; set; }

    /// <summary>
    /// Gets or sets the header image URL of the game.
    /// </summary>
    public string HeaderImageUrl { get; set; }

    /// <summary>
    /// Gets or sets a list of genres of the game.
    /// </summary>
    public List<string> Genres { get; set; }

    /// <summary>
    /// Gets or sets the release date of the game.
    /// </summary>
    public DateTime? ReleaseDate { get; set; }

    /// <summary>
    /// Gets or sets the background image URL of the game.
    /// </summary>
    public string BackgroundImageUrl { get; set; }

    /// <summary>
    /// Gets or sets a detailed description of the game.
    /// </summary>
    public string AboutTheGame { get; set; }
}
