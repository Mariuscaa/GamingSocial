namespace HIOF.GamingSocial.PublicGameInformation.Model.V2;

/// <summary>
/// This class is used to extract the data we want to keep from the full game list retrieval.
/// </summary>
public class V2SteamGames
{
    public List<V2Game> games { get; set; }

    public class V2Game
    {
        public int appid { get; set; }
        public string name { get; set; }
        public int last_modified { get; set; }
    }
}
