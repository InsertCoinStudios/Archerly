using archerly.database.repos;

namespace archerly.tests;

public class UnitTest1
{
    //This Test is so I know if I get any models back or naw and can access its attributes
    [Fact]
    private async Task SetUp()
    {
        var url = "https://xvbnlycdrewhoyhulylj.supabase.co";
        var key = "";
        var options = new Supabase.SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        SupaBaseAnimalRepo animalRepo = new SupaBaseAnimalRepo(supabase);
        var animal = await animalRepo.GetAllAsync();
        string species = animal.First().Name;
        
        Assert.NotNull(species);
    }
}
