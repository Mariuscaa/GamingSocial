namespace HIOF.GamingSocial.GameInformation.Models.V3;

/// <summary>
/// Represents a POST object for a collection of games to a profile.
/// </summary>
public class V3PostGameCollection
{
    /// <summary>
    /// Gets or sets the unique identifier of the profile.
    /// </summary>
    public Guid ProfileId { get; set; }

    /// <summary>
    /// Gets or sets the list of game ids in the collection.
    /// </summary>
    public List<int> GameIds { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="V3PostGameCollection"/> class.
    /// </summary>
    public V3PostGameCollection()
    {

    }
}
