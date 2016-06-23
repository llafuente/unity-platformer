namespace UnityPlatformer {
  /// <summary>
  /// Areas in wich the Character can be.
  /// </summary>
  public enum Areas {
    None =                0,
    Ladder =              1,
    Grabbable =           1 << 2,
    Liquid =              1 << 3,
    Item =                1 << 4,
    Rope =                1 << 5,
  }
}
