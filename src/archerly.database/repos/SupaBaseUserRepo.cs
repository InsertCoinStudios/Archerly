using archerly.entities;
using Supabase;

namespace archerly.database.repos;

public class SupaBaseUserRepo
{
    private readonly Client _supabaseClient;
    public SupaBaseUserRepo(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<User?> GetByUserIdlAsync(Guid userid)
    {
        var user = await _supabaseClient
            .From<User>()
            .Where(u => u.UserId == userid)
            .Single();
        return user;
    }

    public async Task<User?> GetByUserNickAsync(string nick)
    {
        var user = await _supabaseClient
            .From<User>()
            .Where(u => u.Nickname == nick)
            .Single();
        return user;
    }

    public async Task Add(User user)
    {
        await _supabaseClient
            .From<User>()
            .Insert(user);
    }

    public async Task Update(User user)
    {
        await _supabaseClient
            .From<User>()
            .Where(u => u.Id == user.Id)
            .Update(user);
    }

    public async Task Delete(User user)
    {
        await _supabaseClient
            .From<User>()
            .Where(u => u.Id == user.Id)
            .Delete();
    }
}