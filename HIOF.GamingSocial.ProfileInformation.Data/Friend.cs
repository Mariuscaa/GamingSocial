

namespace HIOF.GamingSocial.ProfileInformation.Data;


/// <summary>
/// Represents a friend association between two profiles. Order is irrelevant.
/// </summary>
public class Friend
{

    /// <summary>
    /// Gets or sets the GUID of the first profile in the friend association.
    /// </summary>
    public Guid ProfileGuid1 { get; set; }

    /// <summary>
    /// Gets or sets the GUID of the second profile in the friend association.
    /// </summary>
    public Guid ProfileGuid2 { get; set;}

    public Friend()
    {
    }
}
