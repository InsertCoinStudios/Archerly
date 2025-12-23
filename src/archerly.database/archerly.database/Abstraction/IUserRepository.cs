namespace DefaultNamespace;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}