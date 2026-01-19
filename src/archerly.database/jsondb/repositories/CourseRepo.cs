using archerly.database.repos.interfaces;
using archerly.entities;

namespace archerly.database.jsondb.repositories;

public class CourseRepository : JsonRepositoryBase, ICourseRepository
{
    public CourseRepository(JsonDatabaseStore store) : base(store) { }

    public Task<Course?> GetByIdAsync(Guid id)
    {
        var db = Store.Load();
        return Task.FromResult(db.Courses.FirstOrDefault(c => c.Id == id));
    }

    public Task<Course?> GetByNameAsync(string name)
    {
        var db = Store.Load();
        return Task.FromResult(
            db.Courses.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        );
    }

    public Task<List<Course>> GetAllAsync()
    {
        var db = Store.Load();
        return Task.FromResult(db.Courses.ToList());
    }

    public Task<Course?> AddAsync(Course entity)
    {
        var db = Store.Load();

        if (entity.Id == Guid.Empty)
            entity.Id = Guid.NewGuid();

        db.Courses.Add(entity);
        Store.Save(db);

        return Task.FromResult<Course?>(entity);
    }

    public Task<Course?> UpdateAsync(Course entity)
    {
        var db = Store.Load();
        var index = db.Courses.FindIndex(c => c.Id == entity.Id);

        if (index < 0)
            return Task.FromResult<Course?>(null);

        db.Courses[index] = entity;
        Store.Save(db);

        return Task.FromResult<Course?>(entity);
    }

    public Task DeleteAsync(Guid id)
    {
        var db = Store.Load();
        db.Courses.RemoveAll(c => c.Id == id);
        Store.Save(db);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Course entity) => DeleteAsync(entity.Id);
}
