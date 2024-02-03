using Catel.Data;
using System.ComponentModel.DataAnnotations;

namespace HIOF.GamingSocial.ProfileInformation.Model
{
    
    public class V1PostProfile
    {
        [Required]
        public string? Name { get; set; } = null;
        [Required]
        public string? Bio { get; set; }
        [Required]
        public string? Country { get; set; }
        [Required]
        public int? Age { get; set; }
        [Required]
        public string? PhotoUrl { get; set; }
    }
}
