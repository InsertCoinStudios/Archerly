using archerly.database.repos.interfaces;
using archerly.entities;

namespace archerly.database.jsondb.repositories;

public class CourseAnimalRepository
    : JsonRepositoryBase, ICourseAnimalRepository
{
    public CourseAnimalRepository(JsonDatabaseStore store) : base(store) { }

    public Task<CourseAnimal?> GetByIdAsync(Guid courseId, Guid animalId)
    {
        var db = Store.Load();
        return Task.FromResult(
            db.CourseAnimals.FirstOrDefault(
                ca => ca.CourseId == courseId && ca.AnimalId == animalId
            )
        );
    }

    public Task<List<CourseAnimal>> GetAllAsync()
    {
        var db = Store.Load();
        return Task.FromResult(db.CourseAnimals.ToList());
    }

    public Task<List<CourseAnimal>> GetByCourseIdAsync(Guid courseId)
    {
        var db = Store.Load();
        return Task.FromResult(
            db.CourseAnimals.Where(ca => ca.CourseId == courseId).ToList()
        );
    }

    public Task<List<CourseAnimal>> GetByAnimalIdAsync(Guid animalId)
    {
        var db = Store.Load();
        return Task.FromResult(
            db.CourseAnimals.Where(ca => ca.AnimalId == animalId).ToList()
        );
    }

    public Task<CourseAnimal?> AddAsync(CourseAnimal entity)
    {
        var db = Store.Load();
        db.CourseAnimals.Add(entity);
        Store.Save(db);
        return Task.FromResult<CourseAnimal?>(entity);
    }

    public Task<CourseAnimal?> UpdateAsync(CourseAnimal entity)
    {
        var db = Store.Load();

        var index = db.CourseAnimals.FindIndex(
            ca => ca.CourseId == entity.CourseId && ca.AnimalId == entity.AnimalId
        );

        if (index < 0)
            return Task.FromResult<CourseAnimal?>(null);

        db.CourseAnimals[index] = entity;
        Store.Save(db);

        return Task.FromResult<CourseAnimal?>(entity);
    }

    public Task DeleteAsync(Guid courseId, Guid animalId)
    {
        var db = Store.Load();
        db.CourseAnimals.RemoveAll(
            ca => ca.CourseId == courseId && ca.AnimalId == animalId
        );
        Store.Save(db);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(CourseAnimal entity)
        => DeleteAsync(entity.CourseId, entity.AnimalId);
}
