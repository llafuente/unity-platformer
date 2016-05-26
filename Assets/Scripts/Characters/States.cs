namespace UnityPlatformer {
  /// <summary>
  /// States in wich the Character can be.
  /// NOTE Some can overlap
  /// </summary>
  public enum States {
    None =                0,
    OnGround =            1,
    OnMovingPlatform =    3,
    OnSlope =             1 << 2 | OnGround,
    Jumping =             1 << 3,
    Hanging =             1 << 4 | Jumping,
    Falling =             1 << 5,
    FallingFast =         1 << 6 | Falling,
    Ladder =              1 << 7,
    WallSliding =         1 << 8,
    WallSticking =        1 << 9,
    MeleeAttack =         1 << 10,
    Grabbing =            1 << 11,
    Slipping =            1 << 12,
    Liquid =              1 << 13,

    // debug states, so it can be displayed as text
    SlippingGrounded =    Slipping | OnGround,
    SlippingSlopeGrounded =    Slipping | OnSlope | OnGround,
    //Dashing,
    //Frozen,
    //FreedomState
  }
}
