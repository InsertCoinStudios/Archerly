using archerly.models;

namespace archerly.database.repos
{
    public interface IUserRepo
    {
        Task<User?> GetByEmailAsync(string email);
        
        void Add(User user);
        void Update(User user);
        void Delete(User user);
    }
}