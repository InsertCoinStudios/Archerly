using archerly.database.repos.interfaces;
using archerly.entities;

namespace archerly.database.jsondb.repositories;

public class HydratedCourseRepository
    : JsonRepositoryBase, IHydratedCourseRepository
{
    public HydratedCourseRepository(JsonDatabaseStore store) : base(store) { }
    private readonly ICourseRepository _courseRepo;
    private readonly ICourseAnimalRepository _courseAnimalRepo;
    private readonly IAnimalRepository _animalRepo;

    public HydratedCourseRepository(
        JsonDatabaseStore store,
        ICourseRepository courseRepo,
        ICourseAnimalRepository courseAnimalRepo,
        IAnimalRepository animalRepo) : base(store)
    {
        _courseRepo = courseRepo;
        _courseAnimalRepo = courseAnimalRepo;
        _animalRepo = animalRepo;
    }

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

    public async Task<HydratedCourse?> AddAsync(HydratedCourse entity)
    {
        var db = Store.Load();

        // Add course
        var course = new Course
        {
            Id = entity.Id != Guid.Empty ? entity.Id : Guid.NewGuid(),
            Name = entity.Name,
            Location = entity.Location,
            Difficulty = entity.Difficulty,
            Info = entity.Info
        };
        db.Courses.Add(course);

        // Add animals (if new) and course-animal links
        for (int i = 0; i < entity.Animals.Count; i++)
        {
            var animal = entity.Animals[i];

            if (!db.Animals.Any(a => a.Id == animal.Id))
                db.Animals.Add(animal);

            db.CourseAnimals.Add(new CourseAnimal
            {
                CourseId = course.Id,
                AnimalId = animal.Id,
                Order = i
            });
        }

        Store.Save(db);

        return Build(course, db);
    }

    public async Task<HydratedCourse?> UpdateAsync(HydratedCourse entity)
    {
        var db = Store.Load();

        // Update course
        var index = db.Courses.FindIndex(c => c.Id == entity.Id);
        if (index < 0) return null;

        db.Courses[index] = new Course
        {
            Id = entity.Id,
            Name = entity.Name,
            Location = entity.Location,
            Difficulty = entity.Difficulty,
            Info = entity.Info
        };

        // Delete existing course-animal links
        db.CourseAnimals.RemoveAll(ca => ca.CourseId == entity.Id);

        // Re-add animals in order
        for (int i = 0; i < entity.Animals.Count; i++)
        {
            var animal = entity.Animals[i];

            if (!db.Animals.Any(a => a.Id == animal.Id))
                db.Animals.Add(animal);

            db.CourseAnimals.Add(new CourseAnimal
            {
                CourseId = entity.Id,
                AnimalId = animal.Id,
                Order = i
            });
        }

        Store.Save(db);

        return Build(db.Courses[index], db);
    }

    public async Task DeleteAsync(Guid id)
    {
        var db = Store.Load();

        // Remove course-animal links
        db.CourseAnimals.RemoveAll(ca => ca.CourseId == id);

        // Remove course
        db.Courses.RemoveAll(c => c.Id == id);

        Store.Save(db);
    }

    // Delete by entity
    public Task DeleteAsync(HydratedCourse entity)
        => DeleteAsync(entity.Id);

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
