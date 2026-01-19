using Supabase.Postgrest.Models;
namespace archerly.database.repos.interfaces;

public interface IRepository<T>
//where T : BaseModel
{
    /// <summary>
    /// Get an entity by its primary key (Guid for UUIDs or long for bigint)
    /// </summary>
    Task<T?> GetByIdAsync(Guid id);
    /// <summary>
    /// Get all entities
    /// </summary>
    Task<List<T>> GetAllAsync();

    /// <summary>
    /// Insert a new entity and return the inserted object
    /// </summary>
    Task<T?> AddAsync(T entity);

    /// <summary>
    /// Update an existing entity and return the updated object
    /// </summary>
    Task<T?> UpdateAsync(T entity);

    /// <summary>
    /// Delete an entity
    /// </summary>
    Task DeleteAsync(T entity);
    Task DeleteAsync(Guid id);
}
