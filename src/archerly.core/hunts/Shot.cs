using archerly.core;
namespace archerly.core.hunts;

public sealed record Shot(Guid Player, Guid Target, ShotType Variant, long Points) { }