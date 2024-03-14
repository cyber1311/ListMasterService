using ListMasterService.Models;

namespace ListMasterService.Database;

public interface IListsRepository
{
    Task<StatusCode> AddList(ListCreateRequest request);
    Task<StatusCode> DeleteList(ListDeleteRequest request);
    Task<StatusCode> UpdateListTitle(ListUpdateTitleRequest request);
    Task<StatusCode> UpdateListElements(ListUpdateElementsRequest request);
    Task<bool> ExistsListInLists(Guid id);
    
    Task<bool> ExistsListInUsersLists(Guid id);
    Task<List?> GetList(GetListRequest request);

    Task<List<List>?> GetAllUserLists(GetAllUserListsRequest request);
    
    Task<StatusCode> CopyList(ListCopyRequest request);
    
    Task<StatusCode> ShareList(ListShareRequest request);
}