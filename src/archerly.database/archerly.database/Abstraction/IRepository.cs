namespace archarly.database;

public interface IRepository
{
    Task<T?> GetByIdAsync<T>(Guid id);
    Task<IReadOnlyList<T>> GetAllAsync<T>();
}