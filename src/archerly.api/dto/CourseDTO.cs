using System.Threading.Tasks;
using archerly.core;
using archerly.database.repos;
using archerly.entities;
using Serilog;

namespace archerly.api;

public class CourseDto
{
    public string Name { get; set; }
    public string Location { get; set; }
    public string Info { get; set; }
    /// <summary>Difficulty as int (0 = Easy, 1 = Medium, 2 = Hard)</summary>
    public int Difficulty { get; set; }
    public List<Guid> TargetsInOrder { get; set; } = new();

    public static async Task<Animal[]> ResolveAnimals(List<Guid> targets, Supabase.Client client)
    {
        var repo = new SupaBaseAnimalRepo(client);
        var animals = new List<Animal>();
        foreach (var target in targets)
        {
            try
            {
                var animal = await repo.GetByIdAsync(target);
                if (animal is null)
                {
                    throw new NullReferenceException(nameof(animal));
                }
                animals.Add(animal);
            }
            catch (Exception e)
            {
                Log.Warning(e, $"Function: {nameof(ResolveAnimals)} could not Resolve Target {target}");
                throw;
            }
        }
        return animals.ToArray();
    }
}