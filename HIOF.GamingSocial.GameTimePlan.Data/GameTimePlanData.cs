namespace HIOF.GamingSocial.GameTimePlan.Data;

/// <summary>
/// Represents the database object for a game time plan.
/// </summary>
public class GameTimePlanData
{
    /// <summary>
    /// Gets or sets the unique identifier of the game time plan.
    /// </summary>
    public int GameTimePlanId { get; set; }

    /// <summary>
    /// Gets or sets the name of the game time plan.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the game time plan.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the game associated to the time plan.
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the group associated to the time plan.
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// Gets or sets the start time of the game time plan.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end time of the game time plan.
    /// </summary>
    public DateTime EndTime { get; set; }
}
