using archerly.database.repos.interfaces;
using archerly.entities;

namespace archerly.database.jsondb.repositories;

public class HydratedCourseRepository
    : JsonRepositoryBase, IHydratedCourseRepository
{
    public HydratedCourseRepository(JsonDatabaseStore store) : base(store) { }

    public Task<HydratedCourse?> GetByIdAsync(Guid id)
    {
        var db = Store.Load();
        var course = db.Courses.FirstOrDefault(c => c.Id == id);
        if (course == null) return Task.FromResult<HydratedCourse?>(null);

        return Task.FromResult(Build(course, db));
    }

    public Task<HydratedCourse?> GetByNameAsync(string name)
    {
        var db = Store.Load();
        var course = db.Courses.FirstOrDefault(
            c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        return course == null
            ? Task.FromResult<HydratedCourse?>(null)
            : Task.FromResult(Build(course, db));
    }

    public Task<List<HydratedCourse>> GetAllAsync()
    {
        var db = Store.Load();
        return Task.FromResult(db.Courses.Select(c => Build(c, db)).ToList());
    }

    public Task<HydratedCourse?> AddAsync(HydratedCourse _)
        => throw new NotSupportedException();

    public Task<HydratedCourse?> UpdateAsync(HydratedCourse _)
        => throw new NotSupportedException();

    public Task DeleteAsync(Guid _)
        => throw new NotSupportedException();

    public Task DeleteAsync(HydratedCourse _)
        => throw new NotSupportedException();

    private static HydratedCourse Build(Course course, JsonDatabase db)
    {
        var animals =
            from cx in db.CourseAnimals
            join a in db.Animals on cx.AnimalId equals a.Id
            where cx.CourseId == course.Id
            orderby cx.Order
            select a;

        return new HydratedCourse
        {
            Id = course.Id,
            Name = course.Name,
            Location = course.Location,
            Difficulty = course.Difficulty,
            Info = course.Info,
            Animals = animals.ToList()
        };
    }
}
