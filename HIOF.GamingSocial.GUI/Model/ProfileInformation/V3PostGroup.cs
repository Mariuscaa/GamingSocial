using System.ComponentModel.DataAnnotations;

namespace HIOF.GamingSocial.GUI.Model;

public class V3PostGroup
{
    [Required(ErrorMessage = "Please enter a group name.")]
    public string GroupName { get; set; }
    [Required(ErrorMessage = "Please enter a description.")]
    public string Description { get; set; }
    public bool IsHidden { get; set; }
    public bool IsPrivate { get; set; }
    public string? PhotoUrl { get; set; }
}
