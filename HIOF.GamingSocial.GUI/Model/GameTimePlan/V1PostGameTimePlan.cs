using System.ComponentModel.DataAnnotations;

namespace HIOF.GamingSocial.GUI.Model;
public class V1PostGameTimePlan
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public int GameId { get; set; }
    [Required]
    public int GroupId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public V1PostGameTimePlan() { }

}
