using System;

namespace HIOF.GamingSocial.GUI.Model.GameInformation
{
    public class ProfileModel
    {
        public Guid ProfileGuid { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Country { get; set; }
        public int Age { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
