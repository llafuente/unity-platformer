namespace UnityPlatformer {
  /// <summary>
  /// Alignament of Character, if you need to create groups do it here!
  /// </summary>
  public enum Alignment {
    None =                0,
    // Player and player allied
    Allied =              1,
    // Enemy side
    Enemy =               1 << 2,
    // meant to be all static damage the world made, like spikes!
    World =               1 << 3,
  }
}
