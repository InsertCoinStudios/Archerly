using archerly.entities;
namespace archerly.database.repos.interfaces;

public interface IAnimalRepository : IRepository<Animal>
{
    // You can add animal-specific methods here in the future
    public Task<List<Animal>> GetByIdsAsync(IEnumerable<Guid> ids);
}
