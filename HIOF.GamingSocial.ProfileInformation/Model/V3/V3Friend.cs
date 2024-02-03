namespace HIOF.GamingSocial.ProfileInformation.Model.V3;

/// <summary>
/// Represents a friend association between two profiles. Order is irrelevant.
/// </summary>
public class V3Friend
{
    /// <summary>
    /// Gets or sets the GUID of the first profile in the friend association.
    /// </summary>
    public Guid ProfileGuid1 { get; set; }

    /// <summary>
    /// Gets or sets the GUID of the second profile in the friend association.
    /// </summary>
    public Guid ProfileGuid2 { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="V3Friend"/> class.
    /// </summary>
    public V3Friend()
    {
    }
}
