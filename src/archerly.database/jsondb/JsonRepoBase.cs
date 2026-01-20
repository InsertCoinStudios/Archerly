namespace archerly.database.jsondb;

public abstract class JsonRepositoryBase
{
    protected readonly JsonDatabaseStore Store;

    protected JsonRepositoryBase(JsonDatabaseStore store)
    {
        Store = store;
    }
}