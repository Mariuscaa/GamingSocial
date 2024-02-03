using Microsoft.AspNetCore.Mvc;
using HIOF.GamingSocial.Chat.Data;
using Microsoft.EntityFrameworkCore;
using HIOF.GamingSocial.Chat.Model.V1;

namespace HIOF.GamingSocial.Chat.Controllers.V1;

/// <summary>
/// Controller that handles creation and retrieval of messages
/// </summary>
[ApiController]
[Route("V1/Chat")]
public class V1ChatController : ControllerBase
{
    private readonly ILogger<V1ChatController> _logger;
    private readonly ChatDbContext _db;


    /// <summary>
    /// Initializes a new instance of the <see cref="V1ChatController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="db">The database context.</param>
    public V1ChatController(ILogger<V1ChatController> logger, ChatDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Receives chat message data, creates a new chat message in the database.
    /// </summary>
    /// <param name="postChat">An object with the following properties: Sender, Reciever, Groupid, Message, MessageSent.</param>
    /// <returns>Returns the result of the post, if successful it returns what was saved to the database</returns>
    [HttpPost]
    [ProducesResponseType(typeof(V1Result<V1PostChatMessage>), 200)]
    [ProducesResponseType(typeof(V1Result<V1PostChatMessage>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V1Result<V1PostChatMessage>>> CreateChat(V1PostChatMessage? postChat)
    {

        var message = new ChatMessage
        {
            Sender = postChat.Sender,
            Reciever = postChat.Reciever,
            groupid = postChat.Groupid,
            Message = postChat.Message,
            MessageSent = postChat.MessageSent,
        };
        _db.ChatMessage.Add(message);
        await _db.SaveChangesAsync();

        var result = new V1Result<V1PostChatMessage>(new V1PostChatMessage
        {
            Sender = message.Sender,
            Reciever = message.Reciever,
            Groupid = postChat.Groupid = 0,
            Message = message.Message,
            MessageSent = postChat.MessageSent,
        });
        _logger.LogInformation($"Created chat message with id: {message.ChatId}.");
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a list of chat messages either based on a group room or between two users specified by profileGuid1 and profileGuid2.
    /// </summary>
    /// <param name="profileGuid1">Identifier of the first user in a chat</param>
    /// <param name="profileGuid2">Identifier of the second user in a chat</param>
    /// <param name="room">Identifier of the group chat room</param>
    /// <returns>Returns a list of chat messages related to either the group room or the two specified users. If no related messages are found, returns a Not Found response.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(V1Result<V1GetChatMessage>), 200)]
    [ProducesResponseType(typeof(V1Result<V1GetChatMessage>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V1Result<List<V1GetChatMessage>>>> Getchat(Guid? profileGuid1, Guid? profileGuid2, int? room = 0)
    {
        var profile1 = await _db.ChatMessage.FirstOrDefaultAsync(p => p.Sender == profileGuid1);
        var profile2 = await _db.ChatMessage.FirstOrDefaultAsync(p => p.Reciever == profileGuid2);

    

        if (room != 0)
        {
            var responseProfiles = await _db.ChatMessage
                            .Where(getChat => getChat.groupid == room)
                .Select(getChat => new V1GetChatMessage
                {
                    Sender = getChat.Sender,
                    Reciever = (Guid)getChat.Reciever,
                    groupid = getChat.groupid,
                    Message = getChat.Message,
                    MessageSent = getChat.MessageSent,
                }).ToListAsync();
            return Ok(new V1Result<List<V1GetChatMessage>>(responseProfiles));

        }
        else if (room == 0)
        {
            if (profile1 == null || profile2 == null)
            {
                return NotFound(new V1Result<V1GetChatMessage>($"No profile found with the search {profileGuid1} or {profileGuid2}"));
            }

            var responseProfiles = await _db.ChatMessage
                        .Where(getChat => getChat.Sender == profileGuid1 && getChat.Reciever == profileGuid2)
            .Select(getChat => new V1GetChatMessage
            {
                Sender = getChat.Sender,
                Reciever = (Guid)getChat.Reciever,
                Message = getChat.Message,
                MessageSent = getChat.MessageSent,
            }).ToListAsync();

            
            return Ok(new V1Result<List<V1GetChatMessage>>(responseProfiles));


        }


        return BadRequest("Invalid request parameters.");
    }

}
