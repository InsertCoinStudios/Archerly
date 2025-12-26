using archerly.core;
namespace archerly.core.hunts;

public sealed record Shot(User Player, Animal Target, ShotType Variant, long Points) { }