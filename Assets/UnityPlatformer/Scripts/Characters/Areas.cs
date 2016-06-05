namespace UnityPlatformer {
  /// <summary>
  /// Areas in wich the Character can be.
  /// </summary>
  public enum Areas {
    None =                0,
    Ladder =              1,
    Grabbable =           1 << 2,
    Liquid =              1 << 3,
  }
}
