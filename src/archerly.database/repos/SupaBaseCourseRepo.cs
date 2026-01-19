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
        _operation = new Operations(_supabaseClient);
    }

    public async Task<List<Course>> GetAll()
    {
        var courses = await _supabaseClient
            .From<Course>()
            .Get();
        
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
    
    public async Task<Course?> GetByNameAsync(string cname)
    {
            var course = await _supabaseClient
                .From<Course>()
                .Where(a => a.Name == cname)
                .Single();
            
            if (course == null) throw new Exception("Course not found");
            
            return course;
    }

    public async void Insert(Course course)
    {
        await _operation.Insert(course);
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