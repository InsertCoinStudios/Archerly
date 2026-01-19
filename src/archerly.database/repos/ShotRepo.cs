using archerly.entities;
using archerly.database.repos.interfaces;
using Supabase;
namespace archerly.database.repos;

public class ShotRepository : IShotRepository
{
    private readonly Client _supabaseClient;

    public ShotRepository(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<Shot?> GetByIdAsync(Guid id)
    {
        return await _supabaseClient
            .From<Shot>()
            .Where(s => s.Id == id)
            .Single();
    }

    public async Task<List<Shot>> GetAllAsync()
    {
        var response = await _supabaseClient
            .From<Shot>()
            .Get();

        return response.Models ?? new List<Shot>();
    }

    public async Task<Shot?> AddAsync(Shot shot)
    {
        var response = await _supabaseClient
            .From<Shot>()
            .Insert(shot);

        return response.Models.FirstOrDefault();
    }

    public async Task<Shot?> UpdateAsync(Shot shot)
    {
        var response = await _supabaseClient
            .From<Shot>()
            .Where(s => s.Id == shot.Id)
            .Update(shot);

        return response.Models.FirstOrDefault();
    }

    public async Task DeleteAsync(Shot shot)
    {
        await _supabaseClient
            .From<Shot>()
            .Where(s => s.Id == shot.Id)
            .Delete();
    }

    // Shot-specific queries ------------------------

    public async Task<List<Shot>> GetAllByPlayerAsync(Guid playerId)
    {
        var response = await _supabaseClient
            .From<Shot>()
            .Where(s => s.UserId == playerId)
            .Get();

        return response.Models ?? new List<Shot>();
    }

    public async Task<List<Shot>> GetAllByPlayerAndAnimalAsync(Guid playerId, Guid animalId)
    {
        var response = await _supabaseClient
            .From<Shot>()
            .Where(s => s.UserId == playerId && s.AnimalId == animalId)
            .Get();

        return response.Models ?? new List<Shot>();
    }

    public async Task<List<Shot>> GetAllByHuntAsync(Guid huntId)
    {
        var response = await _supabaseClient
            .From<Shot>()
            .Where(s => s.HuntId == huntId)
            .Get();

        return response.Models ?? new List<Shot>();
    }
}