namespace HIOF.GamingSocial.PublicProfileInformation.Models.V1;

/// <summary>
/// Represents the full response from the Steam achievements API for a steam profile.
/// </summary>
public class V1SteamGetPlayerAchievementsData
{
    public playerstats Playerstats { get; set; }

    public class playerstats
    {
        public string SteamId { get; set; }
        public string GameName { get; set; }

        public achievements[] Achievements { get; set; }
    }

    public class achievements
    {
        public string ApiName { get; set; }
        public int Achieved { get; set; }
        public int UnlockTime { get; set; }
    }
}