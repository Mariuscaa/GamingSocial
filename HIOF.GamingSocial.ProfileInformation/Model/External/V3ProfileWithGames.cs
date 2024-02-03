namespace HIOF.GamingSocial.ProfileInformation.Model.External;


public class V3ProfileWithGames
{
    public Guid ProfileGuid { get; set; }
    public List<V3GameRatings> GamesCollection { get; set; }

    public V3ProfileWithGames()
    {

    }

}

