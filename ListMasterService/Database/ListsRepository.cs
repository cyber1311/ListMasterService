using Dapper;
using ListMasterService.Models;
using Npgsql;
using System.Text.Json;

namespace ListMasterService.Database;

public class ListsRepository : IListsRepository
{
    private readonly string _connectionString;
    
    private const string InsertIntoListsCommand =
        @"insert into lists(id, title, elements, is_shared, owner_id) VALUES(@Id, @Title, @Elements, @IsShared, @OwnerId);";

    private const string InsertIntoUsersListsCommand =
        @"insert into users_lists(user_id, list_id) VALUES(@UserId, @ListId);";

    private const string DeleteFromListsCommand = 
        @"delete from lists where id = @ListId;";

    private const string DeleteFromUsersListsCommand = 
        @"delete from users_lists where list_id = @ListId and user_id = @UserId;";
    
    private const string DeleteListFromUsersListsCommand = 
        @"delete from users_lists where list_id = @ListId;";
    
    private const string DeleteShareFromUsersListsCommand = 
        @"delete from users_lists where user_id <> @UserId and list_id = @ListId;";
    
    private const string CancelShareForUserFromUsersListsCommand = 
        @"delete from users_lists where user_id = @UserId and list_id = @ListId;";

    private const string UpdateListTitleCommand = 
        @"update lists set title = @Title where id = @Id;";
    
    private const string UpdateListElementsCommand = 
        @"update lists set elements = @Elements where id = @Id;";
    
    private const string UpdateListIsSharedCommand = 
        @"update lists set is_shared = @IsShared where id = @Id;";
    
    private const string GetIdFromListsCommand =
        @"select id as Id from lists where id = @Id;";
    
    private const string GetIdFromUsersListsCommand =
        @"select user_id as UserId from users_lists where user_id = @UserId and list_id = @ListId;";

    private const string GetListFromUsersListsCommand =
        @"select id as Id, title as Title, elements as Elements, is_shared as IsShared, owner_id as OwnerId from lists inner join users_lists on lists.id = users_lists.list_id where user_id = @UserId and list_id = @ListId;";

    private const string GetListFromListsCommand =
        @"select id as Id, title as Title, elements as Elements, is_shared as IsShared, owner_id as OwnerId from lists where id = @Id;";
    
    private const string GetListTitleCommand =
        @"select title as Title from lists where id = @Id;";

    
    private const string GetUserIdFromUsersListsCommand =
        @"select user_id as UserId from users_lists where user_id = @UserId;";
    
    private const string GetUserIdFromUsersByEmailCommand =
        @"select id as UserId from users where email = @Email;";

    private const string GetAllUserListsCommand =
        @"select id as Id, title as Title, elements as Elements, is_shared as IsShared, owner_id as OwnerId from lists inner join users_lists on lists.id = users_lists.list_id where user_id = @UserId;";
    
    private const string GetIdByOwnerId =
        @"select id as Id from lists where owner_id = @UserId;";

    private const string GetAllListUsersCommand =
        @"select id as Id, name as Name, email as Email from users inner join users_lists on users.id = users_lists.user_id where list_id = @ListId;";

    public ListsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetValue<string>("Database:ConnectionString");
    }
    
    public async Task<StatusCode> AddList(ListCreateRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        { 
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using (var transaction = await connection.BeginTransactionAsync())
            {
                await connection.ExecuteAsync(InsertIntoListsCommand, new
                {
                    @Id = request.Id,
                    @Title = request.Title,
                    @Elements = request.Elements,
                    @IsShared = request.IsShared,
                    @OwnerId = request.OwnerId
                }, transaction: transaction);

                await connection.ExecuteAsync(InsertIntoUsersListsCommand, new
                {
                    @UserId = request.UserId,
                    @ListId = request.Id
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

    public async Task<StatusCode> CopyList(ListCopyRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        { 
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using (var transaction = await connection.BeginTransactionAsync())
            {
                var list = await connection.QueryFirstOrDefaultAsync<List?>(GetListFromListsCommand, new
                {
                    @Id=request.ListId
                }, transaction: transaction);
                if (list != null)
                {
                    var newUserId = await connection.QueryFirstOrDefaultAsync<Guid?>(GetUserIdFromUsersByEmailCommand, new
                    {
                        @Email=request.NewUserEmail
                    }, transaction: transaction);
                    if (newUserId != null)
                    {
                        await connection.ExecuteAsync(InsertIntoListsCommand, new
                        {
                            @Id = request.NewListId,
                            @Title = list.Title,
                            @Elements = list.Elements,
                            @IsShared = list.IsShared,
                            @OwnerId = newUserId
                        }, transaction: transaction);

                        await connection.ExecuteAsync(InsertIntoUsersListsCommand, new
                        {
                            @UserId = newUserId,
                            @ListId = request.NewListId
                        }, transaction: transaction);

                    }
                    else
                    {
                        statusCode.Code = 404;
                        statusCode.Message = "Такого пользователя не существует";
                    }
                    
                }
                else
                {
                    statusCode.Code = 404;
                    statusCode.Message = "Такого списка не существует"; 
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
    
    public async Task<StatusCode> DuplicateList(ListDuplicateRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        { 
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using (var transaction = await connection.BeginTransactionAsync())
            {
                var list = await connection.QueryFirstOrDefaultAsync<List?>(GetListFromListsCommand, new
                {
                    @Id=request.ListId
                }, transaction: transaction);
                if (list != null)
                {
                    await connection.ExecuteAsync(InsertIntoListsCommand, new
                    {
                        @Id = request.NewListId,
                        @Title = list.Title,
                        @Elements = list.Elements,
                        @IsShared = list.IsShared,
                        @OwnerId = list.OwnerId
                    }, transaction: transaction);

                    await connection.ExecuteAsync(InsertIntoUsersListsCommand, new
                    {
                        @UserId = request.UserId,
                        @ListId = request.NewListId
                    }, transaction: transaction);
                }
                else
                {
                    statusCode.Code = 404;
                    statusCode.Message = "Такого списка не существует"; 
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
    
    public async Task<StatusCode> ShareList(ListShareRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        { 
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var existsList = await ExistsListInLists(request.ListId);
            if (existsList)
            {
                var newUserId = await connection.QueryFirstOrDefaultAsync<Guid?>(GetUserIdFromUsersByEmailCommand, new
                {
                    @Email=request.NewUserEmail
                });

                if (newUserId != null)
                {
                    await using (var transaction = await connection.BeginTransactionAsync())
                    {
                        var exists = await connection.QueryFirstOrDefaultAsync<Guid?>(GetIdFromUsersListsCommand, new
                        {
                            @UserId = newUserId,
                            @ListId= request.ListId
                        }, transaction: transaction);
                        
                        if (exists == null)
                        {
                            await connection.ExecuteAsync(UpdateListIsSharedCommand, new
                            {
                                @Id = request.ListId,
                                @IsShared = true
                            }, transaction: transaction);
                    
                            await connection.ExecuteAsync(InsertIntoUsersListsCommand, new
                            {
                                @UserId = newUserId,
                                @ListId = request.ListId
                            }, transaction: transaction);

                        }

                            
                        await transaction.CommitAsync();
                    }


                }
                else
                {
                    statusCode.Code = 404;
                    statusCode.Message = "Такого пользователя не существует"; 
                }
            }
            else
            {
                statusCode.Code = 404;
                statusCode.Message = "Такого списка не существует"; 
            }
        }
        catch (Exception)
        {
            statusCode.Code = 500;
            statusCode.Message = "Operation failed"; 
        }
        
        return statusCode;
    }
    
    public async Task<StatusCode> DeleteList(ListDeleteRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            await using (var transaction = await connection.BeginTransactionAsync())
            {
                
               var result = await connection.QueryFirstOrDefaultAsync<Guid?>(GetIdFromUsersListsCommand, new
                {
                    @UserId = request.UserId,
                    @ListId= request.Id
                }, transaction: transaction);

                if (result != null)
                {
                    var isOwner = await connection.QueryFirstOrDefaultAsync<Guid?>(GetIdByOwnerId, new
                    {
                        @UserId = request.UserId,
                    }, transaction: transaction);
                    if (isOwner != null)
                    {
                        await connection.ExecuteAsync(DeleteListFromUsersListsCommand, new
                        {
                            @ListId = request.Id,
                        }, transaction: transaction);
                        
                        await connection.ExecuteAsync(DeleteFromListsCommand, new
                        {
                            @ListId = request.Id,
                        }, transaction: transaction);
                    }
                    else
                    {
                        await connection.ExecuteAsync(DeleteFromUsersListsCommand, new
                        {
                            @UserId = request.UserId,
                            @ListId = request.Id
                        }, transaction: transaction);
                    }
                    
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

    public async Task<StatusCode> UpdateListTitle(ListUpdateTitleRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryFirstOrDefaultAsync<Guid?>(GetIdFromListsCommand, new
            {
                @Id= request.Id
            });

            if (result != null)
            {
                await connection.ExecuteAsync(UpdateListTitleCommand, new
                {
                    @Id = request.Id,
                    @Title = request.Title
                });
            }
            else
            {
                statusCode.Code = 500;
                statusCode.Message = "Такого списка не существует";
            }
            
        }catch (Exception)
        {
            statusCode.Code = 500;
            statusCode.Message = "Operation failed";
        }
        

        return statusCode;
    }

    public async Task<StatusCode> UpdateListElements(ListUpdateElementsRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryFirstOrDefaultAsync<Guid?>(GetIdFromListsCommand, new
            {
                @Id= request.Id
            });

            if (result != null)
            {
                await connection.ExecuteAsync(UpdateListElementsCommand, new
                {
                    @Id = request.Id,
                    @Elements = request.Elements
                });
            }
            else
            {
                statusCode.Code = 500;
                statusCode.Message = "Такого списка не существует";
            }
            
        }catch (Exception)
        {
            statusCode.Code = 500;
            statusCode.Message = "Operation failed";
        }
        

        return statusCode;
    }
    
    public async Task<StatusCode> DeleteListShare(DeleteListShareRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryFirstOrDefaultAsync<Guid?>(GetIdFromListsCommand, new
            {
                @Id= request.Id
            });

            if (result != null)
            {
                await using (var transaction = await connection.BeginTransactionAsync())
                {
                    await connection.ExecuteAsync(UpdateListIsSharedCommand, new
                    {
                        @Id = request.Id,
                        @IsShared = false
                    }, transaction: transaction);
                        
                    await connection.ExecuteAsync(DeleteShareFromUsersListsCommand, new
                    {
                        @UserId = request.UserId,
                        @ListId = request.Id
                    }, transaction: transaction);

                    await transaction.CommitAsync();
                }

               
            }
            else
            {
                statusCode.Code = 500;
                statusCode.Message = "Такого списка не существует";
            }
            
        }catch (Exception)
        {
            statusCode.Code = 500;
            statusCode.Message = "Operation failed";
        }
        

        return statusCode;
    }

    public async Task<StatusCode> CancelShareForUser(DeleteListShareRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryFirstOrDefaultAsync<Guid?>(GetIdFromListsCommand, new
            {
                @Id= request.Id
            });

            if (result != null)
            {
                await connection.ExecuteAsync(CancelShareForUserFromUsersListsCommand, new
                {
                    @UserId = request.UserId,
                    @ListId = request.Id
                });
               
            }
            else
            {
                statusCode.Code = 404;
                statusCode.Message = "Такого списка не существует";
            }
            
        }catch (Exception)
        {
            statusCode.Code = 500;
            statusCode.Message = "Operation failed";
        }
        

        return statusCode;
    }

    public async Task<bool> ExistsListInLists(Guid id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var result = await connection.QueryFirstOrDefaultAsync<Guid?>(GetIdFromListsCommand, new
        {
            @Id= id
        });
        return result != null;
    }

    public async Task<List?> GetList(GetListRequest request)
    {
        var list = new List();
        if (await ExistsListInLists(request.Id))
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
           list = await connection.QueryFirstOrDefaultAsync<List>(GetListFromUsersListsCommand, new
            {
                @UserId = request.UserId,
                @ListId = request.Id
            });
        }
        
        return list;
    }
    
    public async Task<string?> GetListTitle(Guid listId)
    {
        string? listTitle = null;
        if (await ExistsListInLists(listId))
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            listTitle = await connection.QueryFirstOrDefaultAsync<string>(GetListTitleCommand, new
            {
                @Id = listId
            });
        }
        
        return listTitle;
    }

    public async Task<bool> ExistsListInUsersLists(Guid id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var result = await connection.QueryFirstOrDefaultAsync<Guid?>(GetUserIdFromUsersListsCommand, new
        {
            @UserId= id
        });
        return result != null;
    }
    
    public async Task<List<List>?> GetAllUserLists(Guid userId){
        var lists = new List<List>();
        if (await ExistsListInUsersLists(userId))
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryAsync<List>(GetAllUserListsCommand, new
            {
                @UserId=userId
            });
            lists = result.ToList();
        }
        
        return lists;
    }

    public async Task<List<ListUser>?> GetAllListUsers(Guid listId)
    {
        var lists = new List<ListUser>();
        if (await ExistsListInLists(listId))
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryAsync<ListUser>(GetAllListUsersCommand, new
            {
                @ListId = listId
            });
            lists = result.ToList();
        }
        
        return lists;
    }
}