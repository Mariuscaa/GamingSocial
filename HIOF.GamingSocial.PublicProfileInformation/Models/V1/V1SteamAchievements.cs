namespace HIOF.GamingSocial.PublicProfileInformation.Models.V1;

/// <summary>
/// Represents Steam achievements for a specific game and player.
/// </summary>
public class V1SteamAchievements
{
    /// <summary>
    /// Steam Id of the player for whom the achievements are represented.
    /// </summary>
    public string SteamId { get; set; }

    /// <summary>
    /// Name of the game for which the achievements are represented.
    /// </summary>
    public string GameName { get; set; }

    /// <summary>
    /// List of Steam achievements the player has achieved for the game.
    /// </summary>
    public List<achievements> Achievements { get; set; }

    /// <summary>
    /// List of all Steam achievements available for the game.
    /// </summary>
    public List<allAchievements> Allachievements { get; set; }


    /// <summary>
    /// Represents information for a Steam achievement.
    /// </summary>
    public class achievements
    {
        /// <summary>
        /// API name of the achievement.
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// Whether the player has achieved the achievement or not.
        /// </summary>
        public int Achieved { get; set; }

        /// <summary>
        /// Time for when the player achieved the achievement.
        /// </summary>
        public int UnlockTime { get; set; }
    }

    /// <summary>
    /// Represents information for all Steam achievements available for the game.
    /// </summary>
    public class allAchievements
    {
        /// <summary>
        /// API name of the achievement.
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// Whether the player has achieved the achievement or not.
        /// </summary>
        public int Achieved { get; set; }

        /// <summary>
        /// Time for when the player achieved the achievement.
        /// </summary>
        public int UnlockTime { get; set; }

    }

}
