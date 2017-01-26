namespace UnityPlatformer {
  /// <summary>
  /// States in wich the Character can be.
  /// NOTE Some can overlap
  /// </summary>
  public enum States {
    None =                0,
    OnGround =            1,
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
    Pushing =             1 << 14,
    Pulling =             1 << 15,
    Rope =                1 << 16,
    Crounch =             1 << 17,
    Fence =               1 << 18,
    Walking =             1 << 19,
    Running =             1 << 20,


    // debug states, so it can be displayed as text
    SlippingGrounded = Slipping | OnGround,
    MeleeAttackGrounded = MeleeAttack | OnGround,
    SlippingSlopeGrounded = Slipping | OnSlope | OnGround,
    FallingLiquid = Falling | Liquid,
    OnGroundLiquid = OnGround | Liquid,
    OnGroundPushing = OnGround | Pushing,
    NotCrounching = ~Crounch,
    OnGroundCrounching = OnGround | Crounch,

    NotRunning = ~Running,
    NotRope = ~Rope,
    NotMeleeAttack = ~MeleeAttack,
    OnMidAir = Jumping | Falling,


    //Dashing,
    //Frozen,
    //FreedomState
  };

  public enum StatesMask {
    OnGround =            1,
    OnSlope =             1 << 2,
    Jumping =             1 << 3,
    Hanging =             1 << 4,
    Falling =             1 << 5,
    FallingFast =         1 << 6,
    Ladder =              1 << 7,
    WallSliding =         1 << 8,
    WallSticking =        1 << 9,
    MeleeAttack =         1 << 10,
    Grabbing =            1 << 11,
    Slipping =            1 << 12,
    Liquid =              1 << 13,
    Pushing =             1 << 14,
    Pulling =             1 << 15,
    Rope =                1 << 16,
    Crounch =             1 << 17,
    Fence =               1 << 18,
    Walking =             1 << 19,
    Running =             1 << 20,
  };
}
