using System.Formats.Tar;
using System.Security.Cryptography;
using archerly.core;
namespace archerly.core.hunts;

public sealed record Shot(Guid Player, Guid Target, ShotType Variant, int Points, int ShotNumber)
{
    public entities.Shot Convert()
    {
        var result = new entities.Shot
        {
            UserId = this.Player,
            AnimalId = this.Target,
            Score = this.Points,
            ShotNumber = this.ShotNumber,
            Kind = this.Variant.ToString()
        };
        return result;
    }
}