using Serilog;
using Supabase;
using Supabase.Postgrest.Models;

class Operations(Client _supabaseClient)
{
    
    public async void Insert<T>(T model)
        where T : BaseModel, new()
    {
        await _supabaseClient
            .From<T>()
            .Insert(model);

        Log.Information("New {Entity} added: {@Model}", typeof(T).Name, model);

    }
    
    //Id not garantueed
    public async void Update<T>(T model)
    where T : BaseModel, new()
    {
        await _supabaseClient
            .From<T>()
            .Where(a => a.Id == model.Id)
            .Update(model);
    }
}