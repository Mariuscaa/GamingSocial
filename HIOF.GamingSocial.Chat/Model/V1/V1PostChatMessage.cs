namespace HIOF.GamingSocial.Chat.Model.V1;

/// <summary>
/// Represents the post object for a message.
/// </summary>
public class V1PostChatMessage
{
    /// <summary>
    /// Gets or sets the Guid of the Sender of the chatmessage
    /// </summary>
    public Guid Sender { get; set; }

    /// <summary>
    /// Gets or sets the Guid of the Reciever of the chatmessage
    /// </summary>
    public Guid? Reciever { get; set; }

    /// <summary>
    /// Gets or sets the int that marks the message with a groupid if any
    /// </summary>
    public int Groupid { get; set; }

    /// <summary>
    /// Gets or sets the content of the message
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets when the message was sent
    /// </summary>
    public string? MessageSent { get; set; }
}
