using archerly.database.repos.interfaces;
using archerly.entities;

namespace archerly.database.jsondb.repositories;

public class ShotRepository : JsonRepositoryBase, IShotRepository
{
    public ShotRepository(JsonDatabaseStore store) : base(store) { }

    public Task<Shot?> GetByIdAsync(Guid id)
    {
        var db = Store.Load();
        return Task.FromResult<Shot?>(
            db.Shots.FirstOrDefault(s => s.Id == id)
        );
    }

    public Task<List<Shot>> GetAllAsync()
    {
        var db = Store.Load();
        return Task.FromResult(db.Shots.ToList());
    }

    public Task<List<Shot>> GetAllByPlayerAsync(Guid playerId)
    {
        var db = Store.Load();
        return Task.FromResult(
            db.Shots.Where(s => s.UserId == playerId).ToList()
        );
    }

    public Task<List<Shot>> GetAllByPlayerAndAnimalAsync(Guid playerId, Guid animalId)
    {
        var db = Store.Load();
        return Task.FromResult(
            db.Shots.Where(
                s => s.UserId == playerId && s.AnimalId == animalId
            ).ToList()
        );
    }

    public Task<List<Shot>> GetAllByHuntAsync(Guid huntId)
    {
        var db = Store.Load();
        return Task.FromResult(
            db.Shots.Where(s => s.HuntId == huntId).ToList()
        );
    }

    public Task<List<Shot>> GetHighScoreShotsForAsync(Guid userId, Guid huntId)
    {
        var db = Store.Load();

        var shots = db.Shots
            .Where(s => s.UserId == userId && s.HuntId == huntId)
            .OrderByDescending(s => s.Score)
            .ToList();

        return Task.FromResult(shots);
    }

    public Task<Shot?> AddAsync(Shot entity)
    {
        var db = Store.Load();

        if (entity.Id == Guid.Empty)
            entity.Id = Guid.NewGuid();

        db.Shots.Add(entity);
        Store.Save(db);

        return Task.FromResult<Shot?>(entity);
    }

    public Task<Shot?> UpdateAsync(Shot entity)
    {
        var db = Store.Load();

        var index = db.Shots.FindIndex(s => s.Id == entity.Id);
        if (index < 0)
            return Task.FromResult<Shot?>(null);

        db.Shots[index] = entity;
        Store.Save(db);

        return Task.FromResult<Shot?>(entity);
    }

    public Task DeleteAsync(Guid id)
    {
        var db = Store.Load();
        db.Shots.RemoveAll(s => s.Id == id);
        Store.Save(db);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Shot entity) => DeleteAsync(entity.Id);
}
