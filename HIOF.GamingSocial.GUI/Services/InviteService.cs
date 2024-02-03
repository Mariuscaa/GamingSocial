using HIOF.GamingSocial.GUI.Model;

namespace HIOF.GamingSocial.GUI.Services;

public class InviteService
{
    private readonly GroupService _groupService;
    private readonly FriendService _friendService;

    private readonly ILogger<InviteService> _logger;
    private readonly HttpClient _httpClient;
    private string ProfileInformationApiBase = "https://localhost:7087";

    public InviteService(ILogger<InviteService> logger, 
        HttpClient httpClient, GroupService groupService, FriendService friendService)
    {
        _logger = logger;
        _httpClient = httpClient;
        _groupService = groupService;
        _friendService = friendService;
    }

    public async Task<V3Result<V3Invite>> SendInviteAsync(V3PostInvite postInvite)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ProfileInformationApiBase}/V3/Invite", postInvite);
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3Invite>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3Invite> object.");
                return new V3Result<V3Invite>("Ran into an unknown issue when adding user to group.");
            }
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<V3Result<V3Invite>>();
        }
    }


    public async Task<V3Result<List<V3Invite>>> GetInvitesForProfileAsync(Guid profileGuid)
    {
        var response = await _httpClient.GetAsync($"{ProfileInformationApiBase}/V3/Invite?receiverGuid={profileGuid}");

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<List<V3Invite>>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<List<V3Invite>> object.");
                return new V3Result<List<V3Invite>>("Ran into an unknown issue when getting invites.");
            }
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<V3Result<List<V3Invite>>>();
        }
    }

    public async Task<V3Result<V3Invite>> AcceptInviteAsync(V3Invite invite)
    {
        if (invite.InviteType == "Group")
        {
            var postGroupMembership = new V3PostGroupMembership
            {
                GroupId = (int)invite.RelatedId,
                Member = new V3Member()
                {
                    ProfileGuid = invite.ReceiverGuid,
                }
            };

            var addUserToGroupResult = await _groupService.AddUserToGroupAsync(postGroupMembership);
            if (addUserToGroupResult.Errors.Count != 0)
            {
                // If user already in group
                if (addUserToGroupResult.Errors[0] == "User is already a member of this group.")
                {
                    // Delete invite
                    var deleteInviteResult = await DeleteInviteAsync(invite.InviteId);
                    if (deleteInviteResult.Errors.Count != 0)
                    {
                        _logger.LogWarning("Error during accept: " + string.Join(", ", deleteInviteResult.Errors));
                        return new V3Result<V3Invite>("Failed to delete invite. User is however in the group.");
                    }
                    else
                    {
                        return deleteInviteResult;
                    }
                }
                else
                {
                    return new V3Result<V3Invite>(addUserToGroupResult.Errors[0]);
                }
            }

            else
            {
                var deleteInviteResult = await DeleteInviteAsync(invite.InviteId);
                if (deleteInviteResult.Errors.Count != 0)
                {
                    _logger.LogWarning("Error during invite deletion: " + string.Join(", ", deleteInviteResult.Errors));
                    return new V3Result<V3Invite>("Error during invite deletion: " + string.Join(", ", deleteInviteResult.Errors));
                }
                else
                {
                    return deleteInviteResult;
                }
            }
        }
        else
        {
            var addFriendResult = await _friendService.AddToFriendsList(invite.SenderGuid, invite.ReceiverGuid);
            if (addFriendResult.Errors.Count != 0)
            {
                _logger.LogWarning("Error during post to friend list: " + string.Join(", ", addFriendResult.Errors));
                return new V3Result<V3Invite>($"Failed to add friend. Reason: {addFriendResult.Errors[0]}");
            }
            else
            {
                var removeInviteResult = await DeleteInviteAsync(invite.InviteId);
                if (removeInviteResult.Errors.Count != 0)
                {
                    _logger.LogWarning("Error invitation deletion: " + string.Join(", ", removeInviteResult.Errors));
                    return new V3Result<V3Invite>("Failed to delete invite. Friend connection have been saved though.");
                }
                else
                {
                    return removeInviteResult;
                }
            }
        }
    }

    public async Task<V3Result<V3Invite>> DeleteInviteAsync(int inviteId)
    {
        var response = await _httpClient.DeleteAsync($"{ProfileInformationApiBase}/V3/Invite?inviteId={inviteId}");
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3Invite>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3Invite> object.");
                return new V3Result<V3Invite>("Ran into an unknown issue when deleting invite.");
            }
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<V3Result<V3Invite>>();
        }
    }

}
