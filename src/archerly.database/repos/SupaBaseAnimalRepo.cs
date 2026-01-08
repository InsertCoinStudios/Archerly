using archerly.models;
using Serilog;
using Supabase;

namespace archerly.database.repos
{
    public class SupaBaseAnimalRepo : IAnimalRepo
    {
        private readonly Client _supabaseClient;

        public SupaBaseAnimalRepo(Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<Animal?> GetByIdAsync(Guid id)
        {
            var animal = await _supabaseClient
                .From<Animal>()
                .Where(a => a.Id == id)
                .Single();
            
            Log.Information("Getting animal with id {id}", id);
            return animal;
        }

        public async Task<IEnumerable<Animal>> GetAllAsync()
        {
            var animals = await _supabaseClient
                .From<Animal>()
                .Get();
            
            return animals.Models;
        }

        public async void Insert(Animal animal)
        {
            await _supabaseClient
                .From<Animal>()
                .Insert(animal);
                
                Log.Information("New animal {animal} added.", animal);
        }

        public async void Update(Animal animal)
        {
            await _supabaseClient
                .From<Animal>()
                .Where(a => a.Id == animal.Id)
                .Update(animal);
        }

        public async void Delete(Animal animal)
        {
            await _supabaseClient
                .From<Animal>()
                .Where(a => a.Id == animal.Id)
                .Delete();
        }
    }
}