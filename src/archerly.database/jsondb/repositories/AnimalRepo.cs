using archerly.entities;
using archerly.database.repos.interfaces;
namespace archerly.database.jsondb.repositories;

public class AnimalRepository : JsonRepositoryBase, IAnimalRepository
{
    public AnimalRepository(JsonDatabaseStore store) : base(store) { }

    public Task<Animal?> GetByIdAsync(Guid id)
    {
        var db = Store.Load();
        return Task.FromResult(db.Animals.FirstOrDefault(a => a.Id == id));
    }

    public Task<List<Animal>> GetAllAsync()
    {
        var db = Store.Load();
        return Task.FromResult(db.Animals.ToList());
    }

    public Task<List<Animal>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        var set = ids.ToHashSet();
        var db = Store.Load();
        return Task.FromResult(db.Animals.Where(a => set.Contains(a.Id)).ToList());
    }

    public Task<Animal?> AddAsync(Animal entity)
    {
        var db = Store.Load();

        if (entity.Id == Guid.Empty)
            entity.Id = Guid.NewGuid();

        db.Animals.Add(entity);
        Store.Save(db);

        return Task.FromResult<Animal?>(entity);
    }

    public Task<Animal?> UpdateAsync(Animal entity)
    {
        var db = Store.Load();

        var index = db.Animals.FindIndex(a => a.Id == entity.Id);
        if (index < 0)
            return Task.FromResult<Animal?>(null);

        db.Animals[index] = entity;
        Store.Save(db);

        return Task.FromResult<Animal?>(entity);
    }

    public Task DeleteAsync(Animal entity) => DeleteAsync(entity.Id);

    public Task DeleteAsync(Guid id)
    {
        var db = Store.Load();
        db.Animals.RemoveAll(a => a.Id == id);
        Store.Save(db);
        return Task.CompletedTask;
    }
}
