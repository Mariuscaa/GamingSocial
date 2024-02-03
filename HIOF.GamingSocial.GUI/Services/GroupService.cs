using HIOF.GamingSocial.GUI.Model;

namespace HIOF.GamingSocial.GUI.Services;

public class GroupService
{
    private readonly ILogger<GroupService> _logger;
    private readonly HttpClient _httpClient;
    private string ProfileInformationApiBase = "https://localhost:7087";

    public GroupService(ILogger<GroupService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<V3Result<V3Group>> CreateGroupAsync(V3PostGroup postGroup)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ProfileInformationApiBase}/V3/Group", postGroup);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3Group>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3Group> object.");
                return new V3Result<V3Group>("Ran into an unknown issue when creating a group.");
            }
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<V3Result<V3Group>>();
        }
    }

    public async Task<V3Result<List<V3Group>>> GetGroupsAsync(string searchQuery, bool includeHidden = true)
    {
        HttpResponseMessage response;
        if (includeHidden == true)
        {
            response = await _httpClient.GetAsync($"{ProfileInformationApiBase}/V3/Group?searchString={searchQuery}");
        }
        else
        {
            response = await _httpClient.GetAsync($"{ProfileInformationApiBase}/V3/Group?searchString={searchQuery}&includeHidden=false");
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<List<V3Group>>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<List<V3Group>> object.");
                return new V3Result<List<V3Group>>("Ran into an unknown issue when creating a group.");
            }
        }
        return await response.Content.ReadFromJsonAsync<V3Result<List<V3Group>>>();
    }
    public async Task<V3Result<V3ProfileGroups>> GetGroupsForProfileAsync(Guid profileGuid)
    {
        var response = await _httpClient.GetAsync($"{ProfileInformationApiBase}/V3/GroupMembership/Profile/{profileGuid}");

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3ProfileGroups>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3ProfileGroups> object.");
                return new V3Result<V3ProfileGroups>("Ran into an unknown issue when getting groups.");
            }
        }
        return await response.Content.ReadFromJsonAsync<V3Result<V3ProfileGroups>>();
    }

    public async Task<V3Result<V3Group>> GetSingleGroupAsync(int groupId)
    {
        var response = await _httpClient.GetAsync($"{ProfileInformationApiBase}/V3/Group/{groupId}");
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3Group>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3Group> object.");
                return new V3Result<V3Group>("Ran into an unknown issue when getting group.");
            }
        }
        return await response.Content.ReadFromJsonAsync<V3Result<V3Group>>();
    }

    public async Task<V3Result<V3GroupMemberships>> AddUserToGroupAsync(V3PostGroupMembership postGroupMembership)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ProfileInformationApiBase}/V3/GroupMembership", postGroupMembership);

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3GroupMemberships>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3GroupMemberships> object.");
                return new V3Result<V3GroupMemberships>("Ran into an unknown issue when adding user to group.");
            }
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<V3Result<V3GroupMemberships>>();
        }
    }

    public async Task<V3Result<V3GroupMemberships>> GetGroupMembership(Guid profileGuid, int groupId)
    {

        var response = await _httpClient.GetAsync($"{ProfileInformationApiBase}/V3/GroupMembership/{groupId}?profileGuid={profileGuid}");

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3GroupMemberships>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3GroupMemberships> object.");
                return new V3Result<V3GroupMemberships>("Ran into an unknown issue when getting group membership.");
            }
        }
        return await response.Content.ReadFromJsonAsync<V3Result<V3GroupMemberships>>();
    }

    public async Task<V3Result<V3GroupMemberships>> GetAllGroupMembersAsync(int groupId)
    {
        var response = await _httpClient.GetAsync($"{ProfileInformationApiBase}/V3/GroupMembership/{groupId}");

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3GroupMemberships>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3GroupMemberships> object.");
                return new V3Result<V3GroupMemberships>("Ran into an unknown issue when getting group membership.");
            }
        }
        return await response.Content.ReadFromJsonAsync<V3Result<V3GroupMemberships>>();
    }
    public async Task<V3Result<V3GroupMemberships>> RemoveFromGroupAsync(int groupId, Guid? profileGuid = null)
    {
        string url;
        if (profileGuid == null)
        {
            url = $"{ProfileInformationApiBase}/V3/GroupMembership?groupId={groupId}";
        }
        else
        {
            url = $"{ProfileInformationApiBase}/V3/GroupMembership?profileGuid={profileGuid}&groupId={groupId}";
        }
        var response = await _httpClient.DeleteAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3GroupMemberships>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3GroupMemberships> object.");
                return new V3Result<V3GroupMemberships>("Ran into an unknown issue when removing user from group.");
            }
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<V3Result<V3GroupMemberships>>();
        }
    }
    public async Task<V3Result<V3GroupMemberships>> ChangeMemberTypeAsync(int groupId, Guid profileGuid, string memberType)
    {
        var response = await _httpClient.PatchAsync(
            $"{ProfileInformationApiBase}/V3/GroupMembership?groupId={groupId}&profileGuid={profileGuid}&memberType={memberType}", null);

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3GroupMemberships>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3GroupMemberships> object.");
                return new V3Result<V3GroupMemberships>("Ran into an unknown issue while changing member type.");
            }
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<V3Result<V3GroupMemberships>>();
        }
    }
    public async Task<V3Result<V3Group>> DeleteGroupAsync(int groupId)
    {
        var response = await _httpClient.DeleteAsync($"{ProfileInformationApiBase}/V3/Group?groupId={groupId}");
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3Group>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3Group> object.");
                return new V3Result<V3Group>("Ran into an unknown issue while deleting group.");
            }
        }
        else
        {
            var removeMembersResult = await RemoveFromGroupAsync(groupId);
            if (removeMembersResult != null)
            {
                if (removeMembersResult.Errors.Count != 0)
                {
                    _logger.LogError($"Failed to remove members from group {groupId}. Group has however been deleted.");
                }
                else
                {
                    _logger.LogInformation($"Removed all members from group {groupId} after the group was deleted.");
                }
            }
            return await response.Content.ReadFromJsonAsync<V3Result<V3Group>>();
        }
    }

    public async Task<V3Result<V3Group>> EditGroupAsync(V3PatchGroup patchGroup)
    {
        var response = await _httpClient.PatchAsJsonAsync($"{ProfileInformationApiBase}/V3/Group/{patchGroup.GroupId}", patchGroup);
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<V3Result<V3Group>>();
            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                _logger.LogError("Failed to deserialize the response body into a V3Result<V3Group> object.");
                return new V3Result<V3Group>("Ran into an unknown issue while editing group.");
            }
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<V3Result<V3Group>>();
        }
    }
}
