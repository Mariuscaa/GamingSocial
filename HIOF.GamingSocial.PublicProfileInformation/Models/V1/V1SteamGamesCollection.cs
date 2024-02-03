namespace HIOF.GamingSocial.PublicProfileInformation.Models.V1;

/// <summary>
/// Represents a collection of Steam games for a given user.
/// </summary>
public class V1SteamGamesCollection
{
    /// <summary>
    /// The user's Steam ID
    /// </summary>
    public string steamId { get; set; }

    /// <summary>
    /// The number of games in the collection
    /// </summary>
    public int game_count { get; set; }

    /// <summary>
    /// The list of games in the collection
    /// </summary>
    public List<Game> games { get; set; }

    /// <summary>
    /// Represents a game in the Steam games collection.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// The game's app ID
        /// </summary>
        public int appid { get; set; }

        /// <summary>
        /// The game's name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The amount of time the user has played the game
        /// </summary>
        public int playtime_forever { get; set; }

        /// <summary>
        /// The URL for the game's icon image
        /// Example: https://media.steampowered.com/steamcommunity/public/images/apps/578080/609f27278aa70697c13bf99f32c5a0248c381f9d.jpg
        /// </summary>
        public string img_icon_url { get; set; }
    }
}
