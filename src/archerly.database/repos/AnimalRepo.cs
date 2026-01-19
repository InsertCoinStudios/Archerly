using Supabase;
using archerly.entities;
using archerly.database.repos.interfaces;
using Serilog;
namespace archerly.database.repos;

public class AnimalRepository : IAnimalRepository
{
    private readonly Client _supabaseClient;

    public AnimalRepository(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<Animal?> GetByIdAsync(Guid id)
    {
        Log.Information("Getting animal with id {Id}", id);

        return await _supabaseClient
            .From<Animal>()
            .Where(a => a.Id == id)
            .Single();
    }
    public async Task<List<Animal>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        if (ids == null || !ids.Any())
            return new List<Animal>();

        var response = await _supabaseClient
            .From<Animal>()
            .Where(a => ids.Contains(a.Id))
            .Get();

        return response.Models ?? new List<Animal>();
    }

    public async Task<List<Animal>> GetAllAsync()
    {
        var response = await _supabaseClient
            .From<Animal>()
            .Get();

        return response.Models ?? new List<Animal>();
    }

    public async Task<Animal?> AddAsync(Animal animal)
    {
        var response = await _supabaseClient
            .From<Animal>()
            .Insert(animal);

        Log.Information("New animal added: {@Animal}", animal);
        return response.Models.FirstOrDefault();
    }

    public async Task<Animal?> UpdateAsync(Animal animal)
    {
        var response = await _supabaseClient
            .From<Animal>()
            .Where(a => a.Id == animal.Id)
            .Update(animal);

        return response.Models.FirstOrDefault();
    }

    public async Task DeleteAsync(Animal animal)
    {
        await DeleteAsync(animal.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseClient
            .From<Animal>()
            .Where(a => a.Id == id)
            .Delete();
    }
}