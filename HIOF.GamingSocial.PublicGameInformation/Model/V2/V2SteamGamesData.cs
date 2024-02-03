namespace HIOF.GamingSocial.PublicGameInformation.Model.V2;

/// <summary>
/// This class is used to deserialize a list of games from steam.
/// </summary>
public class V2SteamGamesData
{
    public Response response { get; set; }

    public class Response
    {
        public App[] apps { get; set; }
        public bool have_more_results { get; set; }
        public int last_appid { get; set; }
    }

    public class App
    {
        public int appid { get; set; }
        public string name { get; set; }
        public int last_modified { get; set; }
        public int price_change_number { get; set; }
    }

}
