using System.Text.Json.Serialization;

namespace HIOF.GamingSocial.GUI.Model
{
    public class V3VideoGameInformation
    {
        public int Id { get; set; }
        public int? SteamAppId { get; set; }
        public string GameTitle { get; set; }
        public string? GiantbombGuid { get; set; }
        public string? GameDescription { get; set; }
    }
}
