namespace HIOF.GamingSocial.GUI.Model.Chat;


public class PostChatMessage
{
    public Guid Sender { get; set; }
    public Guid Reciever { get; set; }
    public int? groupid { get; set; }
    public string? Message { get; set; }
    public string MessageSent { get; set; }
}
