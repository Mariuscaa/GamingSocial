namespace HIOF.GamingSocial.GUI.Model;

public class V2SteamGameDetails
{
    public string Title { get; set; }
    public int SteamAppId { get; set; }
    public string AboutTheGame { get; set; }
    public string ShortDescription { get; set; }
    public string HeaderImageUrl { get; set; }
    public List<string> Genres { get; set; }        
    public DateTime? ReleaseDate { get; set; }
    public string BackgroundImageUrl { get; set; }
}
