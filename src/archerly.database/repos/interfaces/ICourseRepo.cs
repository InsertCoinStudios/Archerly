using archerly.entities;
namespace archerly.database.repos.interfaces;

public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetByNameAsync(string name);
}