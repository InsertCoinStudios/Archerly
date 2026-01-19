using archerly.entities;
namespace archerly.database.repos.interfaces;

public interface ICourseAnimalRepository : ICompositeRepository<CourseAnimal, Guid, Guid>
{
    Task<List<CourseAnimal>> GetByCourseIdAsync(Guid courseId);
    Task<List<CourseAnimal>> GetByAnimalIdAsync(Guid animalId);
}