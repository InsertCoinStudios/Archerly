using System.Text.Json;
namespace archerly.database.jsondb;

public class JsonDatabaseStore
{
    private readonly string _path;
    private readonly object _lock = new();

    public JsonDatabaseStore(string path)
    {
        _path = path;
    }

    public JsonDatabase Load()
    {
        if (!File.Exists(_path))
            return new JsonDatabase();

        var json = File.ReadAllText(_path);
        return JsonSerializer.Deserialize<JsonDatabase>(json)!;
    }

    public void Save(JsonDatabase db)
    {
        lock (_lock)
        {
            var tmp = _path + ".tmp";

            var json = JsonSerializer.Serialize(
                db,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(tmp, json);
            File.Copy(tmp, _path, true);
            File.Delete(tmp);
        }
    }
}
