namespace UnityPlatformer {
  /// <summary>
  /// List of Damage types
  ///
  /// NOTE this should be edited in your game
  /// </summary>
  public enum DamageType {
    Default =    1,
    Physical =   1 << 2,
    Magical =    1 << 3,
    Fire =       1 << 4,
    Water =      1 << 5,
    Electrical = 1 << 6,
    Poison =     1 << 7,
    Shadow =     1 << 8,
    Lethal =     1 << 9,
    // Choose your pain!
  };
}
