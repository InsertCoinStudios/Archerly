using Supabase.Postgrest.Models;
namespace archerly.database.repos.interfaces;

public interface ICompositeRepository<TEntity, TKey1, TKey2>
    where TEntity : BaseModel
{
    /// <summary>
    /// Get entity by composite primary key
    /// </summary>
    Task<TEntity?> GetByIdAsync(TKey1 key1, TKey2 key2);

    /// <summary>
    /// Get all entities
    /// </summary>
    Task<List<TEntity>> GetAllAsync();

    /// <summary>
    /// Insert new entity and return it
    /// </summary>
    Task<TEntity?> AddAsync(TEntity entity);

    /// <summary>
    /// Update entity and return updated version
    /// </summary>
    Task<TEntity?> UpdateAsync(TEntity entity);

    /// <summary>
    /// Delete entity by composite primary key
    /// </summary>
    Task DeleteAsync(TKey1 key1, TKey2 key2);
}
