using Dapper;
using ToDoApp.Models;
using ToDoApp.Utilities;

namespace ToDoApp.Repositories;

public interface IUsersRepository
{
    Task<Users> Create(Users Item);
    Task<bool> Update(Users Item);
    Task<bool> Delete(int Id);
    Task<List<Users>> GetList();
    Task<Users> GetById(int Id);
     Task<Users> GetByUserName(string  UserName);
}

public class UsersRepository : BaseRepository, IUsersRepository
{
    public UsersRepository(IConfiguration config) : base(config)
    {

    }

    public async Task<Users> Create(Users Item)
    {
        var query = $@"INSERT INTO {TableNames.users} 
        (user_name, gender, passwor_d,  email) 
        VALUES (@UserName, @Gender, @Password,@Email) 
        RETURNING *";

        using (var con = NewConnection)
            return await con.QuerySingleAsync<Users>(query, Item);
    }

    public async Task<bool> Delete(int UserId)
    {
        var query = $@"DELETE FROM {TableNames.users} WHERE user_id = @UserId";

        using (var con = NewConnection)
            return await con.ExecuteAsync(query, new { UserId }) > 0;
    }

    public async Task<Users> GetById(int UserId)
    {
        var query = $@"SELECT * FROM {TableNames.users} WHERE user_id = @UserId";

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Users>(query, new { UserId });
    }

    public async Task<List<Users>> GetList()
    {
        var query = $@"SELECT * FROM {TableNames.users}";

        using (var con = NewConnection)
            return (await con.QueryAsync<Users>(query)).AsList();
    }

    public async Task<bool> Update(Users Item)
    {
        var query = $@"UPDATE {TableNames.users} 
        SET user_name = @UserName, passwor_d = @Password, email = @Email 
         WHERE User_id = @UserId";

        using (var con = NewConnection)
            return await con.ExecuteAsync(query, Item) > 0;
    }
    public async Task<Users> GetByUserName(string UserName)
    {
        var query = $@"SELECT * FROM {TableNames.users} WHERE user_name = @UserName";

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Users>(query, new { UserName });
    }
}