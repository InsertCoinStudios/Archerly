using archerly.entities;
namespace archerly.database.repos.interfaces;

public interface IShotRepository : IRepository<Shot>
{
    Task<List<Shot>> GetAllByPlayerAsync(Guid playerId);
    Task<List<Shot>> GetAllByPlayerAndAnimalAsync(Guid playerId, Guid animalId);
    Task<List<Shot>> GetAllByHuntAsync(Guid huntId);
}