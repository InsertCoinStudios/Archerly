using archerly.database.repos;
using archerly.entities;
using Serilog;

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
   
    private async void MakeNewUser()
    {
        User user = new User();
        user.FirstName = "John";
        user.LastName = "Doe";
        user.IsAdmin = true;
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
    [Fact]
    private async void NewHunt()
    {
        //Add Player
        var options = new Supabase.SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();
        
        SupaBaseUserRepo userRepo = new SupaBaseUserRepo(supabase);
        var user1 = userRepo.GetByUserNickAsync("JohnDoe");
        var user2 = userRepo.GetByUserNickAsync("Test");
        
        //Get Course
        var courseRepo = new SupaBaseCourseRepo(supabase);
        var courseAnimalRepo = new SupaBaseCourseAnimalsRepo(supabase);
        var animalRepo = new SupaBaseAnimalRepo(supabase);
        
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
        
        //Shoot first Shot user 1
        Shot shot = new Shot();
        shot.AnimalId = animals.First().Id;
        shot.UserId = user1.Id;
        shot.Kind = "2";
        shot.Score = 25;
        shot.ShotNumber = 1;
        
        SupaBaseShotRepo shotRepo = new SupaBaseShotRepo(supabase);
        await shotRepo.Insert(shot);

        var check = shotRepo.GetAll();
        Log.Logger.Information("shotRepo.GetAll {check}", check);
        Assert.NotNull(check);

    }
}
