using Dapper;
using ToDoApp.Models;
using ToDoApp.Repositories;
using ToDoApp.Utilities;

namespace ToDoApp.Repositories;

public interface IToDoListRepository
{
    Task<ToDoList> Create(ToDoList Item);
    Task<bool> Update(ToDoList Item);
    Task<bool> Delete(int Id);
    Task<List<ToDoList>> GetList();
    Task<ToDoList> GetById(int Id);
}

public class ToDoListRepository : BaseRepository, IToDoListRepository
{
    public ToDoListRepository(IConfiguration config) : base(config)
    {

    }

    public async Task<ToDoList> Create(ToDoList Item)
    {
        ToDoList data = new ToDoList();
        var query = $@"INSERT INTO {TableNames.to_do_list} 
        (title, description, created_at, updated_at, is_completed, is_deleted, user_id) 
        VALUES (@Title, @Description, @CreatedAt, @UpdatedAt, @IsCompleted, @IsDeleted, @UserId) 
        RETURNING *";

        using (var con = NewConnection)
            data = await con.QuerySingleAsync<ToDoList>(query, Item);

        return data;
    }

    public async Task<bool> Delete(int ToDoId)
    {
        var query = $@"DELETE FROM {TableNames.to_do_list} WHERE to_do_id = @ToDoId";

        using (var con = NewConnection)
            return await con.ExecuteAsync(query, new { ToDoId }) > 0;
    }

    public async Task<ToDoList> GetById(int ToDoId)
    {
        var query = $@"SELECT * FROM {TableNames.to_do_list} WHERE  to_do_id = @ToDoId";

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<ToDoList>(query, new { ToDoId });
    }

    public async Task<List<ToDoList>> GetList()
    {
        var query = $@"SELECT * FROM {TableNames.to_do_list}";

        using (var con = NewConnection)
            return (await con.QueryAsync<ToDoList>(query)).AsList();
    }

    public async Task<bool> Update(ToDoList Item)
    {
        var query = $@"UPDATE {TableNames.to_do_list} 
        SET title = @Title, description = @Description 
         WHERE to_do_id = @ToDoId";

        using (var con = NewConnection)
            return await con.ExecuteAsync(query, Item) > 0;
    }
}