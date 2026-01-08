using archerly.models;
namespace archerly.database.repos;

public interface IAnimalRepo
{
    Task<Animal?> GetByIdAsync(Guid id);
    Task<IEnumerable<Animal>> GetAllAsync();
    void Insert(Animal animal);
    void Update(Animal animal);
    void Delete(Animal animal);
}