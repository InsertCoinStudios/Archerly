namespace archerly.database.repos;

using archerly.entities;
using archerly.database.repos.interfaces;
using Supabase;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CourseAnimalRepository : ICourseAnimalRepository
{
    private readonly Client _supabaseClient;

    public CourseAnimalRepository(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    /* ============================= */
    /* ICompositeRepository methods */
    /* ============================= */

    public async Task<CourseAnimal?> GetByIdAsync(Guid courseId, Guid animalId)
    {
        return await _supabaseClient
            .From<CourseAnimal>()
            .Where(ca => ca.CourseId == courseId && ca.AnimalId == animalId)
            .Single();
    }

    public async Task<List<CourseAnimal>> GetAllAsync()
    {
        var response = await _supabaseClient
            .From<CourseAnimal>()
            .Get();

        return response.Models ?? new List<CourseAnimal>();
    }

    public async Task<CourseAnimal?> AddAsync(CourseAnimal entity)
    {
        var response = await _supabaseClient
            .From<CourseAnimal>()
            .Insert(entity);

        return response.Models.FirstOrDefault();
    }

    public async Task<CourseAnimal?> UpdateAsync(CourseAnimal entity)
    {
        var response = await _supabaseClient
            .From<CourseAnimal>()
            .Where(ca => ca.CourseId == entity.CourseId && ca.AnimalId == entity.AnimalId)
            .Update(entity);

        return response.Models.FirstOrDefault();
    }

    public async Task DeleteAsync(Guid courseId, Guid animalId)
    {
        await _supabaseClient
            .From<CourseAnimal>()
            .Where(ca => ca.CourseId == courseId && ca.AnimalId == animalId)
            .Delete();
    }
    public async Task DeleteAsync(CourseAnimal entity)
    {
        await DeleteAsync(entity.CourseId, entity.AnimalId);
    }

    /* ============================= */
    /* ICourseAnimalRepository specific */
    /* ============================= */

    public async Task<List<CourseAnimal>> GetByCourseIdAsync(Guid courseId)
    {
        var response = await _supabaseClient
            .From<CourseAnimal>()
            .Where(ca => ca.CourseId == courseId)
            .Get();

        return response.Models ?? new List<CourseAnimal>();
    }

    public async Task<List<CourseAnimal>> GetByAnimalIdAsync(Guid animalId)
    {
        var response = await _supabaseClient
            .From<CourseAnimal>()
            .Where(ca => ca.AnimalId == animalId)
            .Get();

        return response.Models ?? new List<CourseAnimal>();
    }
}
