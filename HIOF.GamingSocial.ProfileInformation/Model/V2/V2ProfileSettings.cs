namespace HIOF.GamingSocial.ProfileInformation.Model.V2
{
    public class V2ProfileSettings
    {
        public Guid ProfileGuid { get; set; }
        public string Language { get; set; }
        public bool DarkMode { get; set; }
        public bool PublicProfile { get; set; }
        public string OnlineStatus { get; set; }
    }
}
