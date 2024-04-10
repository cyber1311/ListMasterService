using Dapper;
using ListMasterService.Models;
using ListMasterService.Models.Groups;
using Npgsql;

namespace ListMasterService.Database;

public class GroupsRepository : IGroupsRepository
{
    private readonly string _connectionString;
    
    private const string InsertIntoGroupsCommand =
        @"insert into groups(id, title, owner_id) VALUES(@Id, @Title, @OwnerId);";

    private const string InsertIntoUsersGroupsCommand =
        @"insert into users_groups(user_id, group_id) VALUES(@UserId, @GroupId);";

    private const string DeleteFromGroupsCommand = 
        @"delete from groups where id = @GroupId;";

    private const string DeleteFromUsersGroupsCommand = 
        @"delete from users_groups where group_id = @GroupId;";
    
    private const string DeleteUserFromUsersGroupsCommand = 
        @"delete from users_groups where user_id = @UserId and group_id = @GroupId;";

    private const string UpdateGroupTitleCommand = 
        @"update groups set title = @Title where id = @Id;";

    private const string GetAllUserGroupsCommand =
        @"select id as Id, title as Title, owner_id as OwnerId from groups inner join users_groups on groups.id = users_groups.group_id where user_id = @UserId;";
    
    private const string GetAllGroupMembersCommand =
        @"select id as Id, name as Name, email as Email from users inner join users_groups on users.id = users_groups.user_id where group_id = @GroupId;";

    private const string GetUserIdFromUsersByEmailCommand =
        @"select id as UserId from users where email = @Email;";
    
    private const string GetIdFromGroupsCommand =
        @"select id as Id from groups where id = @Id;";
    
    private const string GetIdFromUsersGroupsCommand =
        @"select user_id as UserId from users_groups where user_id = @UserId and group_id = @GroupId;";

    
    public GroupsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetValue<string>("Database:ConnectionString");
    }
    
    public async Task<StatusCode> AddGroup(Group request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        { 
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using (var transaction = await connection.BeginTransactionAsync())
            {
                await connection.ExecuteAsync(InsertIntoGroupsCommand, new
                {
                    @Id = request.Id,
                    @Title = request.Title,
                    @OwnerId = request.OwnerId
                }, transaction: transaction);

                await connection.ExecuteAsync(InsertIntoUsersGroupsCommand, new
                {
                    @UserId = request.OwnerId,
                    @GroupId = request.Id
                }, transaction: transaction);

                await transaction.CommitAsync();
            }
        }
        catch (Exception)
        {
            statusCode.Code = 500;
            statusCode.Message = "Operation failed"; 
        }
        
        return statusCode;
    }

    public async Task<StatusCode> DeleteGroup(GroupDeleteRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            await using (var transaction = await connection.BeginTransactionAsync())
            {
                var result = await connection.QueryFirstOrDefaultAsync<Guid?>(GetIdFromUsersGroupsCommand, new
                {
                    @UserId = request.UserId,
                    @GroupId= request.GroupId
                }, transaction: transaction);

                if (result != null)
                {
                    await connection.ExecuteAsync(DeleteFromUsersGroupsCommand, new
                    {
                        @GroupId = request.GroupId
                    }, transaction: transaction);
                    
                    await connection.ExecuteAsync(DeleteFromGroupsCommand, new
                    {
                        @GroupId = request.GroupId,
                    }, transaction: transaction);
                }

                
                await transaction.CommitAsync();
            }
        }
        catch (Exception)
        {
            statusCode.Code = 500;
            statusCode.Message = "Operation failed";
        }
        
        return statusCode;
    }

    public async Task<StatusCode> UpdateGroupTitle(GroupUpdateTitleRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryFirstOrDefaultAsync<Guid?>(GetIdFromGroupsCommand, new
            {
                @Id= request.Id
            });

            if (result != null)
            {
                await connection.ExecuteAsync(UpdateGroupTitleCommand, new
                {
                    @Id = request.Id,
                    @Title = request.Title
                });
            }
            else
            {
                statusCode.Code = 500;
                statusCode.Message = "Такой группы не существует";
            }
            
        }catch (Exception)
        {
            statusCode.Code = 500;
            statusCode.Message = "Operation failed";
        }
        

        return statusCode;
    }

    public async Task<StatusCode> AddGroupMember(GroupMemberAddRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        { 
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using (var transaction = await connection.BeginTransactionAsync())
            {
                var newUserId = await connection.QueryFirstOrDefaultAsync<Guid?>(GetUserIdFromUsersByEmailCommand, new
                {
                    @Email=request.UserToAddEmail
                }, transaction: transaction);
                
                if (newUserId != null)
                {
                    var result = await connection.QueryFirstOrDefaultAsync<Guid?>(GetIdFromUsersGroupsCommand, new
                    {
                        @UserId = newUserId,
                        @GroupId= request.GroupId
                    }, transaction: transaction);
   
                    if (result == null)
                    {
                        await connection.ExecuteAsync(InsertIntoUsersGroupsCommand, new
                        {
                            @UserId = newUserId,
                            @GroupId = request.GroupId
                        }, transaction: transaction);
                    }
                }
                else
                {
                    statusCode.Code = 404;
                    statusCode.Message = "Такого пользователя не существует"; 
                }

                await transaction.CommitAsync();
            }

            
        }
        catch (Exception)
        {
            statusCode.Code = 500;
            statusCode.Message = "Operation failed"; 
        }
        
        return statusCode;
    }

    public async Task<StatusCode> DeleteGroupMember(GroupMemberDeleteRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            await using (var transaction = await connection.BeginTransactionAsync())
            {
                var result = await connection.QueryFirstOrDefaultAsync<Guid?>(GetIdFromUsersGroupsCommand, new
                {
                    @UserId = request.UserToDeleteId,
                    @GroupId= request.GroupId
                }, transaction: transaction);
   
                if (result != null)
                {
                    await connection.ExecuteAsync(DeleteUserFromUsersGroupsCommand, new
                    {
                        @UserId = request.UserToDeleteId,
                        @GroupId = request.GroupId
                    }, transaction: transaction);
                }
                else
                {
                    statusCode.Code = 404;
                    statusCode.Message = "Такого пользователя не существует"; 
                }

                await transaction.CommitAsync();
            }
            
            
        }
        catch (Exception)
        {
            statusCode.Code = 500;
            statusCode.Message = "Operation failed";
        }
        
        return statusCode;
    }

    public async Task<List<Group>?> GetAllUserGroups(Guid userId)
    {
        List<Group> groups;
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var result = await connection.QueryAsync<Group>(GetAllUserGroupsCommand, new
        {
            @UserId=userId
        });
        groups = result.ToList();
        
        return groups;
    }

    public async Task<List<GetAllGroupMembersResponse>?> GetAllGroupMembers(Guid groupId)
    {
        List<GetAllGroupMembersResponse> groupMembers;
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var result = await connection.QueryAsync<GetAllGroupMembersResponse>(GetAllGroupMembersCommand, new
        {
            @GroupId = groupId
        });
        groupMembers = result.ToList();
        
        return groupMembers;
    }
}