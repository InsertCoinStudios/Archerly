using archerly.entities;
namespace archerly.database.repos.interfaces;

public interface IHydratedCourseRepository : IRepository<HydratedCourse>
{
    /// <summary>
    /// Get a hydrated course by its name
    /// </summary>
    Task<HydratedCourse?> GetByNameAsync(string name);
}