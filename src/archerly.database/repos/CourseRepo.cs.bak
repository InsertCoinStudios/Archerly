using Supabase;
using archerly.entities;
using archerly.database.repos.interfaces;
namespace archerly.database.repos;

public class CourseRepository : ICourseRepository
{
    private readonly Client _supabaseClient;

    public CourseRepository(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    // IRepository<Course> --------------------------

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        return await _supabaseClient
            .From<Course>()
            .Where(c => c.Id == id)
            .Single();
    }

    public async Task<List<Course>> GetAllAsync()
    {
        var response = await _supabaseClient
            .From<Course>()
            .Get();

        return response.Models ?? new List<Course>();
    }

    public async Task<Course?> AddAsync(Course course)
    {
        var response = await _supabaseClient
            .From<Course>()
            .Insert(course);

        return response.Models.FirstOrDefault();
    }

    public async Task<Course?> UpdateAsync(Course course)
    {
        var response = await _supabaseClient
            .From<Course>()
            .Where(c => c.Id == course.Id)
            .Update(course);

        return response.Models.FirstOrDefault();
    }

    public async Task DeleteAsync(Course course)
    {
        await DeleteAsync(course.Id);
    }


    public async Task<Course?> GetByNameAsync(string name)
    {
        return await _supabaseClient
            .From<Course>()
            .Where(c => c.Name == name)
            .Single();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseClient
            .From<Course>()
            .Where(c => c.Id == id)
            .Delete();
    }
}