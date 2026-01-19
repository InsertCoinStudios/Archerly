using archerly.entities;
namespace archerly.database.repos.interfaces;

public interface IUserRepository : IRepository<User>
{
    // You can add extra user-specific methods here if needed later
    Task<User?> GetByNickAsync(string nickname);
}