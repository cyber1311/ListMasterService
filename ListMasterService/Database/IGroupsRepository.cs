using ListMasterService.Models;
using ListMasterService.Models.Groups;

namespace ListMasterService.Database;

public interface IGroupsRepository
{
    Task<StatusCode> AddGroup(Group request);
    Task<StatusCode> DeleteGroup(GroupDeleteRequest request);
    Task<StatusCode> UpdateGroupTitle(GroupUpdateTitleRequest request);
    Task<StatusCode> AddGroupMember(GroupMemberAddRequest request);
    Task<StatusCode> DeleteGroupMember(GroupMemberDeleteRequest request);
    Task<List<Group>?> GetAllUserGroups(Guid userId);
    Task<List<GetAllGroupMembersResponse>?> GetAllGroupMembers(Guid groupId);
}