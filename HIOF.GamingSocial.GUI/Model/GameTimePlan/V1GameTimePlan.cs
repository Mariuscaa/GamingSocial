namespace HIOF.GamingSocial.GUI.Model;

public class V1GameTimePlan
{
    public int GameTimePlanId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int GameId { get; set; }
    public int GroupId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public V1GameTimePlan() { }
}
