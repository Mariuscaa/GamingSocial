using System.ComponentModel.DataAnnotations;

namespace HIOF.GamingSocial.ProfileInformation.Model.V2
{
    public class V2Profile
    {
        public Guid ProfileGuid { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Country { get; set; }
        public int Age { get; set; }
        public string? PhotoUrl { get; set; }

        public V2Profile() { }

    }
}
