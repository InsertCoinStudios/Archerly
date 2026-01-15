using archerly.database.repos;
using archerly.entities;

namespace archerly.tests;

public class UnitTest1
{
    
    string url = "https://xvbnlycdrewhoyhulylj.supabase.co";
    string key = "sb_publishable_3ASU1hTVILnRVJtfLRdE2g_GSTeg8Sd";
    
    //This Test is so I know if I get any models back or naw and can access its attributes
    [Fact]
    private async Task SetUp()
    {   var options = new Supabase.SupabaseOptions
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
    [Fact]
    private async Task TestGetCourseAndListOfAnimals()
    {
        var options = new Supabase.SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();
        
        SupaBaseCourseRepo courseRepo = new SupaBaseCourseRepo(supabase);
        SupaBaseCourseAnimalsRepo courseAnimalRepo = new SupaBaseCourseAnimalsRepo(supabase);
        SupaBaseAnimalRepo animalRepo = new SupaBaseAnimalRepo(supabase);


        var course = await courseRepo.GetByNameAsync("Wilde Safari");
        Assert.NotNull(course);
        
        var animalsInCourse = await courseAnimalRepo.GetByCourseIdAsync(course.Id);
        Assert.NotNull(animalsInCourse);
        List<Animal> animals = new List<Animal>();
        foreach (var animalInCourse in animalsInCourse)
        {
            var animalModel = await animalRepo.GetByIdAsync(animalInCourse.AnimalId);
            Assert.NotNull(animalModel);
            
            animals.Add(animalModel);
        }
        
        Assert.NotNull(animals);

    }
    [Fact]
    private async void MakeNewUser()
    {
        User user = new User();
        user.FirstName = "John";
        user.LastName = "Doe";
        user.IsAdmin = false;
        user.Nickname = "JohnDoe";

        var options = new Supabase.SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();
        
        SupaBaseUserRepo userRepo = new SupaBaseUserRepo(supabase);
        userRepo.Add(user);

        var newUser = userRepo.GetByUserNickAsync(user.Nickname);
        Assert.NotNull(newUser);
    }

    private void NewHunt()
    {
        //Add Player
    }
}
