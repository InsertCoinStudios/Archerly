using Serilog;
using Supabase;
using Supabase.Postgrest.Models;

class Operations(Client _supabaseClient)
{

    public async Task Insert<T>(T model) 
        where T : BaseModel, new()
    {
        await _supabaseClient
            .From<T>()
            .Insert(model);
    }
    
    //Id not guaranteed ?
    /*public async void Update<T>(T model)
    where T : BaseModel, new()
    {
        await _supabaseClient
            .From<T>()
            .Where(a => a.Id == model.Id)
            .Update(model);
    }*/
}