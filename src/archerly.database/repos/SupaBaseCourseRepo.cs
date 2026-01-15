using archerly.entities;
using Serilog;
using Supabase;
using Supabase.Postgrest.Models;

namespace archerly.database.repos;


public class SupaBaseCourseRepo
{
    private readonly Client _supabaseClient;
    private readonly Operations _operation;

    public SupaBaseCourseRepo(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<List<Course>> GetAll()
    {
        var courses = await _supabaseClient
            .From<Course>()
            .Get();

        foreach (var course in courses.Models)
        {
            var difficulty = await _supabaseClient
                .From<DifficultyM>()
                .Select("id, difficulty")
                .Where(d => d.Id == course.DifficultyId)
                .Single();

            course.Difficulty = difficulty.DifficultyName;
        }
        
        return courses.Models;
    }
    
    public async Task<Course?> GetByIdAsync(Guid id)
    {
        var course = await _supabaseClient
            .From<Course>()
            .Where(a => a.Id == id)
            .Single();
            
        return course;
    }

    public async void Insert(Course course)
    {
        _operation.Insert(course);
    }

    public async void Update(Course course)
    {
        await _supabaseClient
            .From<Course>()
            .Where(c => c.Id == course.Id)
            .Update(course);

    }

    public async void Delete(Course course)
    {
        await _supabaseClient
            .From<Course>()
            .Where(a => a.Id == course.Id)
            .Delete();
    }

    
}