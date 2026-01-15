using archerly.entities;
using Serilog;
using Supabase;

namespace archerly.database.repos;

public class SupaBaseCourseAnimalsRepo
{
    private readonly Client _supabaseClient;
    private readonly Operations _operation;

    public SupaBaseCourseAnimalsRepo(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<CourseAnimal?> GetByIdAsync(Guid id)
    {
        var canimal = await _supabaseClient
            .From<CourseAnimal>()
            .Where(ca => ca.Id == id)
            .Single();
            
        Log.Information("Getting course animal with id {id}", id);
        return canimal;
    }

    public async Task<IEnumerable<CourseAnimal>> GetAllAsync()
    {
        var canimals = await _supabaseClient
            .From<CourseAnimal>()
            .Get();
            
        return canimals.Models;

    }

    public void Insert(CourseAnimal canimal)
    {
        _operation.Insert(canimal);   
    }

    public void Update(CourseAnimal animal)
    {
    }

    public void Delete(CourseAnimal animal)
    {
    }

}