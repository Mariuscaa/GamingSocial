

using System.ComponentModel.DataAnnotations;

namespace HIOF.GamingSocial.GUI.Model.ProfileInformation;


public class V3PostProfile
{
    [Required(ErrorMessage = "Please enter a UserName.")]
    public string UserName { get; set; } 
    [Required(ErrorMessage = "Please enter a Name.")]
    public string Name { get; set; } 
    [Required(ErrorMessage = "Please enter a Bio.")]
    public string Bio { get; set; }
    [Required(ErrorMessage = "Please enter a Country.")]
    public string Country { get; set; } 
    [Required(ErrorMessage = "Please enter a Age.")]
    public int? Age { get; set; }
    public string? PhotoUrl { get; set; }
}
