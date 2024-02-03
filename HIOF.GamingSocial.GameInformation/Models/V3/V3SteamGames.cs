namespace HIOF.GamingSocial.GameInformation.Models.V3;

/// <summary>
/// Represents the V3 Steam Games model.
/// </summary>
public class V3SteamGames
{
    /// <summary>
    /// The list of steam games.
    /// </summary>
    public List<Game> games { get; set; }

    /// <summary>
    /// Represents a single steam game.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// The steam app ID of the game.
        /// </summary>
        public int appid { get; set; }

        /// <summary>
        /// The name of the game.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The last modified date of the game. Can possibly be used in the future.
        /// </summary>
        public int last_modified { get; set; }
    }
}
