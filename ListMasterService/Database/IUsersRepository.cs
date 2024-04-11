using ListMasterService.Models;

namespace ListMasterService.Database;

public interface IUsersRepository
{
    Task<StatusCode> AddUser(User user);
    Task<StatusCode> DeleteUser(UserDeleteRequest request);
    Task<StatusCode> UpdateUserName(UserUpdateNameRequest request);
    Task<StatusCode> UpdateUserEmail(UserUpdateEmailRequest request);
    Task<StatusCode> UpdateUserPassword(UserUpdatePasswordRequest request);

    Task<User?> GetUser(GetUserByEmailRequest request);

    Task<bool> ExistsUser(string email);

    Task<string?> GetUserEmail(Guid userId);
    Task<List<string>?> GetUserEmails(Guid ownerId, Guid listId);
}