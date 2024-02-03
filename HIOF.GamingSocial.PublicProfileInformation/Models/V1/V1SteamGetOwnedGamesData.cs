namespace HIOF.GamingSocial.PublicProfileInformation.Models.V1;

/// <summary>
/// Represents the full response from the Steam API for a steam profile.
/// </summary>
public class V1SteamGetOwnedGamesData
{
    public Response response { get; set; }

    public class Response
    {
        public int game_count { get; set; }
        public Game[] games { get; set; }
    }

    public class Game
    {
        public int appid { get; set; }
        public string name { get; set; }
        public int playtime_forever { get; set; }
        public string img_icon_url { get; set; }
        public int playtime_windows_forever { get; set; }
        public int playtime_mac_forever { get; set; }
        public int playtime_linux_forever { get; set; }
        public int rtime_last_played { get; set; }
        public int[] content_descriptorids { get; set; }
        public bool has_community_visible_stats { get; set; }
        public bool has_leaderboards { get; set; }
        public int playtime_2weeks { get; set; }
    }
}
