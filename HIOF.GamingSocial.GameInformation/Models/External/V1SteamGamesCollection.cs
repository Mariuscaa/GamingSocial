namespace HIOF.GamingSocial.GameInformation.Models.External;

public class V1SteamGamesCollection
{
    public string steamId { get; set; }
    public int game_count { get; set; }
    public List<Game> games { get; set; }

    public class Game
    {
        public int appid { get; set; }
        public string name { get; set; }
        public int playtime_forever { get; set; }

        // Example: https://media.steampowered.com/steamcommunity/public/images/apps/578080/609f27278aa70697c13bf99f32c5a0248c381f9d.jpg
        public string img_icon_url { get; set; }
    }
}