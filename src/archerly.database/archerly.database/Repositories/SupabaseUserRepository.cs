using DefaultNamespace;

namespace archarly.database;
using Supabase;
using archerly.database.Entities;
public class SupabaseUserRepository : IUserRepository
{
    private readonly Supabase.Client? _supaCLient;
    
    public void SupabaseUserRepository(IConfiguration configuration)
    {
         var supaUrl = configuration["SupabaseUrl:Url"];
         var supaKey = configuration["SupabaseUrl:Key"];
            
        _supaClient = new Supabase.Client(supaUrl, supaKey);
        _supaClient.InitalizeAsync().Wait();
    }
    
    public async Task<User> GetUserById(int id)
    {
        return await _supaClient.From<User>().Where(u => u.Id == id).Single();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }
}