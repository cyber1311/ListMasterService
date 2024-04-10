using Dapper;
using Npgsql;
using ListMasterService.Models;

namespace ListMasterService.Database;

public class UsersRepository : IUsersRepository
{
    private readonly string _connectionString;
    
    private const string InsertCommand =
        @"insert into users(id, email, name, password) VALUES(@Id, @Email, @Name, @Password);";
    
    private const string GetEmailCommand =
        @"select email as Email from users where email = @Email;";
    
    private const string GetIdCommand =
        @"select id as Id from users where id = @Id;";
    
    private const string GetUserByEmailCommand =
        @"select id as Id, email as Email, name as Name, password as Password from users where email = @Email;";

    private const string DeleteFromUsersCommand = 
        @"delete from users where id = @Id;";
    
    private const string DeleteFromUsersListsCommand = 
        @"delete from users_lists where list_id in (select id from lists where owner_id = @UserId) or user_id = @UserId;";
    
    private const string DeleteFromUsersGroupsCommand = 
        @"delete from users_groups where group_id in (select id from groups where owner_id = @UserId) or user_id = @UserId;";
    
    private const string DeleteGroupCommand = 
        @"delete from groups where owner_id = @OwnerId;";
    
    private const string UpdateUserNameCommand = 
        @"update users set name = @Name where id = @Id;";
    
    private const string UpdateUserEmailCommand = 
        @"update users set email = @Email where id = @Id;";
    
    private const string UpdateUserPasswordCommand = 
        @"update users set password = @Password where id = @Id;";
    
    private const string GetAllUserListsIdCommand =
        @"select list_id as ListId from users_lists where user_id = @UserId;";
    
    private const string DeleteFromListsCommand = 
        @"delete from lists where owner_id = @OwnerId;";


    public UsersRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetValue<string>("Database:ConnectionString");
    }
    
    public async Task<StatusCode> AddUser(User user)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();
            
            var result = await connection.QueryFirstOrDefaultAsync<string>(GetEmailCommand, new
            {
                @Email = user.Email
            });

            if (result != null)
            {
                statusCode.Code = 500;
                statusCode.Message = "Такой пользователь уже существует";
            }
            else
            {
                await connection.ExecuteAsync(InsertCommand, new
                {
                    @Id = user.Id,
                    @Email = user.Email,
                    @Name = user.Name,
                    @Password = user.Password,
                });
            }
            
        }
        catch (Exception)
        {
            statusCode.Code = 500;
            statusCode.Message = "Operation failed";
        }
        
        return statusCode;
        
    }
    public async Task<StatusCode> DeleteUser(UserDeleteRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var user = await connection.QueryFirstOrDefaultAsync<Guid?>(GetIdCommand, new
            {
                @Id = request.Id
            });
        
            if (user == null)
            {
                statusCode.Code = 404;
                statusCode.Message = "Такого пользователя не существует";
            }
            else
            {
                await using (var transaction = await connection.BeginTransactionAsync())
                {
                    
                    await connection.ExecuteAsync(DeleteFromUsersListsCommand, new
                    {
                        @UserId = request.Id
                    }, transaction: transaction);
                    
                    await connection.ExecuteAsync(DeleteFromListsCommand, new
                    {
                        @OwnerId = request.Id,
                    }, transaction: transaction);
                    await connection.ExecuteAsync(DeleteFromUsersGroupsCommand, new
                    {
                        @UserId = request.Id,
                    }, transaction: transaction);
                    
                    await connection.ExecuteAsync(DeleteGroupCommand, new
                    {
                        @OwnerId = request.Id,
                    }, transaction: transaction);
                    
                    await connection.ExecuteAsync(DeleteFromUsersCommand, new
                    {
                        @Id = request.Id
                    }, transaction: transaction);

                    await transaction.CommitAsync();
                }
            }
        }
        catch (Exception e)
        {
            statusCode.Code = 500;
            statusCode.Message = "Operation failed";
        }
        

        return statusCode;
    }

    public async Task<StatusCode> UpdateUserName(UserUpdateNameRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var user = await connection.QueryFirstOrDefaultAsync<Guid?>(GetIdCommand, new
            {
                @Id = request.Id
            });
        
            if (user == null)
            {
                statusCode.Code = 500;
                statusCode.Message = "Такого пользователя не существует";
            }
            else
            {
                await connection.ExecuteAsync(UpdateUserNameCommand, new
                {
                    @Id = request.Id,
                    @Name = request.Name
                });
            }
        }
        catch (Exception e)
        {
            statusCode.Code = 500;
            statusCode.Message = "Operation failed";
        }

        return statusCode;
    }


    public async Task<StatusCode> UpdateUserEmail(UserUpdateEmailRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var user = await connection.QueryFirstOrDefaultAsync<Guid?>(GetIdCommand, new
            {
                @Id = request.Id
            });
        
            if (user == null)
            {
                statusCode.Code = 500;
                statusCode.Message = "Такого пользователя не существует";
            }
            else
            {
                await connection.ExecuteAsync(UpdateUserEmailCommand, new
                {
                    @Id = request.Id,
                    @Email = request.Email
                });
            }
        }
        catch (Exception e)
        {
            statusCode.Code = 500;
            statusCode.Message = "Operation failed";
        }
        
        return statusCode;
    }

    public async Task<StatusCode> UpdateUserPassword(UserUpdatePasswordRequest request)
    {
        var statusCode = new StatusCode(code: 200, message: "OK");
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var user = await connection.QueryFirstOrDefaultAsync<Guid?>(GetIdCommand, new
            {
                @Id = request.Id
            });
        
            if (user == null)
            {
                statusCode.Code = 500;
                statusCode.Message = "Такого пользователя не существует";
            }
            else
            {
                await connection.ExecuteAsync(UpdateUserPasswordCommand, new
                {
                    @Id = request.Id,
                    @Password = request.Password
                });
            }

        }
        catch (Exception e)
        {
            statusCode.Code = 500;
            statusCode.Message = "Operation failed";
        }
        
        return statusCode;
    }
    
    public async Task<bool> ExistsUser(string email)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var result = await connection.QueryFirstOrDefaultAsync<string?>(GetEmailCommand, new
        {
            @Email= email
        });
        return result != null;
    }

    public async Task<User?> GetUser(GetUserByEmailRequest request)
    {
        var user = new User();
        if (await ExistsUser(request.Email))
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            user = await connection.QueryFirstOrDefaultAsync<User>(GetUserByEmailCommand, new
            {
                @Email=request.Email
            });
        }
        
        return user;
    }
}