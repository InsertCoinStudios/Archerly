using Supabase;
using archerly.entities;
using archerly.database.repos.interfaces;

namespace archerly.database.repos;

public class UserRepo : IUserRepository
{
    private readonly Client _supabaseClient;

    public UserRepo(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        var user = await _supabaseClient
            .From<User>()
            .Where(u => u.Id == userId) // <-- updated to match schema
            .Single();
        return user;
    }

    public async Task<User?> GetByNickAsync(string nick)
    {
        var user = await _supabaseClient
            .From<User>()
            .Where(u => u.Nickname == nick)
            .Single();
        return user;
    }

    public async Task<User?> AddAsync(User user)
    {
        var response = await _supabaseClient
            .From<User>()
            .Insert(user);

        // Return the first inserted user, or null if none
        return response.Models.FirstOrDefault();
    }

    public async Task<User?> UpdateAsync(User user)
    {
        var response = await _supabaseClient
            .From<User>()
            .Where(u => u.Id == user.Id)
            .Update(user);

        // Return the first updated user, or null if none
        return response.Models.FirstOrDefault();
    }

    public async Task DeleteAsync(User user)
    {
        await _supabaseClient
            .From<User>()
            .Where(u => u.Id == user.Id)
            .Delete();
    }

    public async Task<List<User>> GetAllAsync()
    {
        var response = await _supabaseClient
            .From<User>()
            .Get();

        // Return the list of users (empty list if none)
        return response.Models ?? new List<User>();
    }
}
