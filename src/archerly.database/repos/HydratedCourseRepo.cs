using archerly.database.repos.interfaces;
using Supabase;
using archerly.entities;
namespace archerly.database.repos;

public class HydratedCourseRepository : IHydratedCourseRepository
{
    private readonly Supabase.Client _client;
    private readonly ICourseRepository _courseRepo;
    private readonly ICourseAnimalRepository _courseAnimalRepo;
    private readonly IAnimalRepository _animalRepo;

    public HydratedCourseRepository(
        Client client,
        ICourseRepository courseRepo,
        ICourseAnimalRepository courseAnimalRepo,
        IAnimalRepository animalRepo)
    {
        _client = client;
        _courseRepo = courseRepo;
        _courseAnimalRepo = courseAnimalRepo;
        _animalRepo = animalRepo;
    }

    public async Task<HydratedCourse?> GetByIdAsync(Guid id)
    {
        var course = await _courseRepo.GetByIdAsync(id);
        if (course == null) return null;

        var courseAnimals = await _courseAnimalRepo.GetByCourseIdAsync(id);
        var animalIds = courseAnimals.Select(ca => ca.AnimalId).ToList();
        var animals = await _animalRepo.GetByIdsAsync(animalIds);

        return new HydratedCourse
        {
            Id = course.Id,
            Name = course.Name,
            Location = course.Location,
            Difficulty = course.Difficulty,
            Info = course.Info,
            Animals = animals
        };
    }
    public async Task<HydratedCourse?> GetByNameAsync(string name)
    {
        var course = await _courseRepo.GetByNameAsync(name);
        if (course == null) return null;

        var courseAnimals = await _courseAnimalRepo.GetByCourseIdAsync(course.Id);
        var animalIds = courseAnimals.Select(ca => ca.AnimalId).ToList();
        var animals = await _animalRepo.GetByIdsAsync(animalIds);

        return new HydratedCourse
        {
            Id = course.Id,
            Name = course.Name,
            Location = course.Location,
            Difficulty = course.Difficulty,
            Info = course.Info,
            Animals = animals
        };
    }

    public async Task<List<HydratedCourse>> GetAllAsync()
    {
        var courses = await _courseRepo.GetAllAsync();
        var courseAnimals = await _courseAnimalRepo.GetAllAsync();
        var animals = await _animalRepo.GetAllAsync();

        return courses.Select(course =>
        {
            var linkedAnimals = courseAnimals
                .Where(ca => ca.CourseId == course.Id)
                .Join(animals, ca => ca.AnimalId, a => a.Id, (ca, a) => a)
                .ToList();

            return new HydratedCourse
            {
                Id = course.Id,
                Name = course.Name,
                Location = course.Location,
                Difficulty = course.Difficulty,
                Info = course.Info,
                Animals = linkedAnimals
            };
        }).ToList();
    }

    public async Task<HydratedCourse?> UpdateAsync(HydratedCourse entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        // Update the main course table
        await _courseRepo.UpdateAsync(new Course
        {
            Id = entity.Id,
            Name = entity.Name,
            Location = entity.Location,
            Difficulty = entity.Difficulty,
            Info = entity.Info
        });

        // Sync CourseAnimal links
        var currentLinks = await _courseAnimalRepo.GetByCourseIdAsync(entity.Id);
        var newAnimalIds = entity.Animals.Select(a => a.Id).ToHashSet();

        // 2a: Delete old links that are no longer associated
        foreach (var ca in currentLinks.Where(ca => !newAnimalIds.Contains(ca.AnimalId)))
        {
            await _courseAnimalRepo.DeleteAsync(ca); // DeleteAsync handles both CourseId + AnimalId
        }

        // 2b: Add new links that are missing
        var existingAnimalIds = currentLinks.Select(ca => ca.AnimalId).ToHashSet();
        for (int i = 0; i < entity.Animals.Count; i++)
        {
            var animal = entity.Animals[i];

            if (!existingAnimalIds.Contains(animal.Id))
            {
                var newLink = new CourseAnimal
                {
                    CourseId = entity.Id,
                    AnimalId = animal.Id,
                    Order = i  // Use the index in the list as the order
                };

                await _courseAnimalRepo.AddAsync(newLink);
            }
        }
        return await GetByIdAsync(entity.Id); // return updated hydrated course
    }
    public async Task<HydratedCourse?> AddAsync(HydratedCourse entity)
    {
        // 1️⃣ Insert the course itself
        var insertedCourse = await _courseRepo.AddAsync(new Course
        {
            Name = entity.Name,
            Location = entity.Location,
            Difficulty = entity.Difficulty,
            Info = entity.Info
        });

        if (insertedCourse == null)
            return null;

        // 2️⃣ Insert the animals links with proper order
        for (int i = 0; i < entity.Animals.Count; i++)
        {
            var animal = entity.Animals[i];
            var link = new CourseAnimal
            {
                CourseId = insertedCourse.Id,
                AnimalId = animal.Id,
                Order = i
            };

            await _courseAnimalRepo.AddAsync(link);
        }

        // 3️⃣ Return the fully hydrated course
        return await GetByIdAsync(insertedCourse.Id);
    }

    public async Task DeleteAsync(HydratedCourse entity)
    {
        await DeleteAsync(entity.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var links = await _courseAnimalRepo.GetByCourseIdAsync(id);
        foreach (var link in links)
        {
            await _courseAnimalRepo.DeleteAsync(link);
        }

        // 2️⃣ Delete the course itself
        var courseToDelete = await _courseRepo.GetByIdAsync(id);
        if (courseToDelete != null)
        {
            await _courseRepo.DeleteAsync(courseToDelete);
        }
    }
}