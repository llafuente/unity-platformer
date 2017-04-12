namespace UnityPlatformer {
  /// <summary>
  /// Character Alignment
  ///
  /// By default Character cannot deal damage to Character in the same
  /// Alignment unless friendlyFire is on either CharacterHealth
  /// </summary>
  public enum Alignment {
    None =                0,
    /// Player and player allied
    Allied =              1,
    /// Enemy side
    Enemy =               1 << 2,
    /// Spikes and the rest of static Damage
    World =               1 << 3,
  }
}
