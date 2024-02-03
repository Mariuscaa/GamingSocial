namespace HIOF.GamingSocial.Chat.Data;

/// <summary>
/// Represents the return object for a message.
/// </summary>
public class ChatMessage
{
    public int ChatId { get; set; }

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
    public int groupid { get; set; }

    /// <summary>
    /// Gets or sets the content of the message
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets when the message was sent
    /// </summary>
    public string MessageSent { get; set; }
}
