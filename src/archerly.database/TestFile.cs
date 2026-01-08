using archerly.database.repos;

namespace archerly.database;

public class TestFile
{
    
    private async void SetUp()
    {
        var url = Environment.GetEnvironmentVariable("https://xvbnlycdrewhoyhulylj.supabase.co");
        var key = Environment.GetEnvironmentVariable("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Inh2Ym5seWNkcmV3aG95aHVseWxqIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2NDc3MTEyNiwiZXhwIjoyMDgwMzQ3MTI2fQ.-UnbXm07mFZ8ezVRYQIZRxkbgjJ9rB3kmWAXuLgOgQs");

        var options = new Supabase.SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        SupaBaseAnimalRepo animalRepo = new SupaBaseAnimalRepo(supabase);
        var animal = await animalRepo.GetAllAsync();
        string species = animal.First().Name;
        Console.WriteLine(species);
    }
}