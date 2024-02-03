namespace HIOF.GamingSocial.GameTimePlan.Model.V1;

/// <summary>
/// Represents the return object for a game time plan.
/// </summary>
public class V1GameTimePlan
{
    /// <summary>
    /// Gets or sets the ID of the game time plan.
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
    /// Gets or sets the ID of the game.
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the group.
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

    /// <summary>
    /// Default constructor.
    /// </summary>
    public V1GameTimePlan() { }
}
