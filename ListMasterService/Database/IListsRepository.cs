using ListMasterService.Models;

namespace ListMasterService.Database;

public interface IListsRepository
{
    Task<StatusCode> AddList(ListCreateRequest request);
    Task<StatusCode> DeleteList(ListDeleteRequest request);
    Task<StatusCode> UpdateListTitle(ListUpdateTitleRequest request);
    Task<StatusCode> UpdateListElements(ListUpdateElementsRequest request);
    
    Task<StatusCode> DeleteListShare(DeleteListShareRequest request);
    Task<StatusCode> CancelShareForUser(DeleteListShareRequest request);
    Task<bool> ExistsListInLists(Guid id);
    
    Task<bool> ExistsListInUsersLists(Guid id);
    Task<List?> GetList(GetListRequest request);
    Task<string?> GetListTitle(Guid listId);

    Task<List<List>?> GetAllUserLists(Guid userId);
    
    Task<List<ListUser>?> GetAllListUsers(Guid listId);
    
    Task<StatusCode> CopyList(ListCopyRequest request);
    
    Task<StatusCode> DuplicateList(ListDuplicateRequest request);
    
    Task<StatusCode> ShareList(ListShareRequest request);
}