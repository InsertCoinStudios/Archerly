using System.Reflection.Metadata;
using archerly.database.repos;
using archerly.entities;

namespace archerly.api;

public class CourseService
{
    private readonly SupaBaseCourseRepo _courseRepo;
    private readonly SupaBaseCourseAnimalsRepo _courseAnimalsRepo;
    private readonly Supabase.Client _client;

    public CourseService(Supabase.Client supabaseClient)
    {
        _courseRepo = new SupaBaseCourseRepo(supabaseClient);
        _courseAnimalsRepo = new SupaBaseCourseAnimalsRepo(supabaseClient);
        _client = supabaseClient;
    }

    /// <summary>
    /// Inserts a new course along with its associated animals
    /// </summary>
    public async Task InsertCourseAsync(CourseDto dto)
    {
        Animal[] resolved = await CourseDto.ResolveAnimals(dto.TargetsInOrder, _client);
        // Convert DTO to Course entity
        var course = Course.From(dto.Name, dto.Location, dto.Info, dto.Difficulty, resolved);

        // Insert course
        await _courseRepo.Insert(course);

        // Insert associated CourseAnimal entries
        if (resolved != null)
        {
            for (int i = 0; i < resolved.Length; i++)
            {
                var animal = resolved[i];
                if (animal is null) { continue; }
                var courseAnimal = new CourseAnimal
                {
                    CourseId = course.Id,
                    AnimalId = animal.Id,
                    OrderNumber = i.ToString(),
                };

                await _courseAnimalsRepo.Insert(courseAnimal);
            }
        }
    }



    /// <summary>
    /// Updates an existing course along with its animals
    /// </summary>
    public async Task UpdateCourseAsync(Guid id, CourseDto dto)
    {
        var course = await _courseRepo.GetByIdAsync(id);
        if (course == null)
            throw new Exception("Course not found");

        // Update course fields
        course.Name = dto.Name;
        course.Location = dto.Location;
        course.Info = dto.Info;
        course.DifficultyId = dto.Difficulty;
        course.Difficulty = dto.Difficulty switch
        {
            0 => "easy",
            1 => "medium",
            2 => "hard",
            _ => ""
        };

        await _courseRepo.Update(course);

        // Delete existing animals
        var existingAnimals = await _courseAnimalsRepo.GetByCourseIdAsync(course.Id);
        foreach (var ea in existingAnimals)
        {
            await _courseAnimalsRepo.Delete(ea);
        }

        // Insert updated animals
        var resolved = await CourseDto.ResolveAnimals(dto.TargetsInOrder, _client);
        if (resolved != null)
        {
            for (int i = 0; i < resolved.Length; i++)
            {
                var animal = resolved[i];
                if (animal == null) continue;

                var courseAnimal = new CourseAnimal
                {
                    CourseId = course.Id,
                    AnimalId = animal.Id,
                    OrderNumber = i.ToString()
                };

                await _courseAnimalsRepo.Insert(courseAnimal);
            }
        }
    }

    /// <summary>
    /// Deletes a course and all associated animals
    /// </summary>
    public async Task DeleteCourseAsync(Guid courseId)
    {
        var course = await _courseRepo.GetByIdAsync(courseId);
        if (course == null)
            throw new Exception("Course not found");

        var animals = await _courseAnimalsRepo.GetByCourseIdAsync(courseId);
        foreach (var a in animals)
        {
            await _courseAnimalsRepo.Delete(a);
        }

        await _courseRepo.Delete(course);
    }

    public async Task<CourseDto?> GetByIdAsync(Guid courseId)
    {
        var course = await _courseRepo.GetByIdAsync(courseId);
        if (course == null) return null;

        // Get associated animals
        var courseAnimals = await _courseAnimalsRepo.GetByCourseIdAsync(course.Id);
        var animalIds = courseAnimals.ConvertAll(ca => ca.AnimalId);

        return new CourseDto
        {
            Name = course.Name,
            Location = course.Location,
            Info = course.Info,
            Difficulty = course.DifficultyId,
            TargetsInOrder = animalIds
        };
    }

    public async Task<List<CourseDto>> GetAllAsync()
    {
        var courses = await _courseRepo.GetAll();
        var courseDtos = new List<CourseDto>();

        foreach (var course in courses)
        {
            var courseAnimals = await _courseAnimalsRepo.GetByCourseIdAsync(course.Id);
            var animalIds = courseAnimals.ConvertAll(ca => ca.AnimalId);

            courseDtos.Add(new CourseDto
            {
                Name = course.Name,
                Location = course.Location,
                Info = course.Info,
                Difficulty = course.DifficultyId,
                TargetsInOrder = animalIds
            });
        }

        return courseDtos;
    }

}