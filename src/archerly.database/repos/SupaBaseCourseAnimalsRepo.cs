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
        _operation = new Operations(_supabaseClient);
    }

    public async Task<List<CourseAnimal>> GetByCourseIdAsync(Guid id)
    {
        var canimal = await _supabaseClient
            .From<CourseAnimal>()
            .Where(ca => ca.CourseId == id)
            .Get();
            
        Log.Information("Getting course animal with id {id}", id);
        return canimal.Models;
    }

    public async Task<IEnumerable<CourseAnimal>> GetAllAsync()
    {
        try
        {

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
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