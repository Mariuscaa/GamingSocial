using System.ComponentModel.DataAnnotations;

namespace HIOF.GamingSocial.GUI.Model.ProfileInformation;

public class V3Profile
{
    public Guid ProfileGuid { get; set; }
    [Required(ErrorMessage = "Please enter a UserName.")]
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Bio { get; set; }
    public string Country { get; set; }
    public int Age { get; set; }
    public string? PhotoUrl { get; set; }
    
    public V3Profile() { }

}
