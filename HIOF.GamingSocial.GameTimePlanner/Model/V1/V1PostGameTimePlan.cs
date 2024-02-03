namespace HIOF.GamingSocial.GameTimePlan.Model.V1;

/// <summary>
/// Class containing POST properties game time plans.
/// </summary>
public class V1PostGameTimePlan
{
    /// <summary>
    /// The name of the game time plan.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The description of the game time plan.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The id of the game.
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// The id of the group.
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// The start time of the game time plan.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// The end time of the game time plan.
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public V1PostGameTimePlan() { }
}
