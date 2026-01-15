using archerly.entities;
using Supabase;

namespace archerly.database.repos;

public class SupaBaseShotRepo
{
    private readonly Client _supabaseClient;
    private readonly Operations _operations;

    public SupaBaseShotRepo(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
        _operations = new Operations(_supabaseClient);
    }

    public async Task<List<Shot>> GetAll()
    {
        var courses = await _supabaseClient
            .From<Shot>()
            .Get();
            
        return courses.Models;
    }

    public async Task<List<Shot>> GetAllByPlayerAndAnimal(int playerId, int animalId)
    {
        var shots = await _supabaseClient
            .From<Shot>()
            .Where(a => a.UserId == playerId && a.AnimalId == animalId)
            .Get();
        return shots.Models;
    }

    public void Insert(Shot shot)
    {
        _operations.Insert(shot);
    }
    
    
}