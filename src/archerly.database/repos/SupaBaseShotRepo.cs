using archerly.entities;
using Supabase;

namespace archerly.database.repos;

public class SupaBaseShotRepo
{
    private readonly Client _supabaseClient;

    public SupaBaseShotRepo(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<List<Shot>> GetAll()
    {
        var courses = await _supabaseClient
            .From<Shot>()
            .Get();
            
        return courses.Models;
    }
}