using archerly.database.repos.interfaces;
using archerly.entities;

namespace archerly.database.jsondb.repositories;

public class UserRepository : JsonRepositoryBase, IUserRepository
{
    public UserRepository(JsonDatabaseStore store) : base(store) { }

    public Task<User?> GetByIdAsync(Guid id)
    {
        var db = Store.Load();
        return Task.FromResult<User?>(
            db.Users.FirstOrDefault(u => u.Id == id)
        );
    }

    public Task<User?> GetByNickAsync(string nickname)
    {
        var db = Store.Load();
        return Task.FromResult<User?>(
            db.Users.FirstOrDefault(
                u => u.Nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase)
            )
        );
    }

    public Task<List<User>> GetAllAsync()
    {
        var db = Store.Load();
        return Task.FromResult(db.Users.ToList());
    }

    public Task<User?> AddAsync(User entity)
    {
        var db = Store.Load();

        if (entity.Id == Guid.Empty)
            throw new InvalidOperationException("User ID must be provided externally.");

        if (db.Users.Any(u => u.Id == entity.Id))
            throw new InvalidOperationException("User already exists.");

        db.Users.Add(entity);
        Store.Save(db);

        return Task.FromResult<User?>(entity);
    }

    public Task<User?> UpdateAsync(User entity)
    {
        var db = Store.Load();

        var index = db.Users.FindIndex(u => u.Id == entity.Id);
        if (index < 0)
            return Task.FromResult<User?>(null);

        db.Users[index] = entity;
        Store.Save(db);

        return Task.FromResult<User?>(entity);
    }

    public Task DeleteAsync(Guid id)
    {
        var db = Store.Load();
        db.Users.RemoveAll(u => u.Id == id);
        Store.Save(db);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User entity) => DeleteAsync(entity.Id);
}
