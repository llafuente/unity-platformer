using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Patrol Artificial inteligence.
  ///
  /// Does not require Actions but it's recommended\n
  /// If no CharacterActionAirMovement it will froze while on air\n
  /// If no CharacterActionGroundMovement it will froze while on ground\n
  /// </summary>
  public class AIGoomba: Enemy {
    /// <summary>
    /// Initial Facing
    /// </summary>
    public Facing initialFacing = Facing.Left;
    /// <summary>
    /// Distance to test if ground is on left/right side.
    /// Helps when Enemy standing on platform moving down.
    /// </summary>
    [Comment("Distance to test if ground is on left/right side. Helps when Enemy standing on platform moving down.")]
    public float rayLengthFactor = 1.0f;
    /// <summary>
    /// true: when reach an edge, change direction\n
    /// false: when reach an edge, continue and fall\n
    /// </summary>
    [Comment("Do not fall on platform edge. Go back.")]
    public bool doNotFall = true;
    /// <summary>
    /// Where is facing now
    /// </summary>
    internal Facing facing;
    /// <summary>
    /// Listen onLeftWall, onRightWall and start moving
    /// </summary>
    override public void Start() {
      base.Start();

      pc2d.onLeftWall += OnLeftWall;
      pc2d.onRightWall += OnRightWall;

      facing = initialFacing;
      input.SetX((float) facing);
    }
    /// <summary>
    /// Move right
    /// </summary>
    virtual public void OnLeftWall() {
      facing = Facing.Right;
      input.SetX((float) facing);
    }
    /// <summary>
    /// Move left
    /// </summary>
    virtual public void OnRightWall() {
      facing = Facing.Left;
      input.SetX((float) facing);
    }
    /// <summary>
    /// Move in the other direction
    /// </summary>
    virtual public void Toogle() {
      facing = facing == Facing.Left ? Facing.Right : Facing.Left;
      input.SetX((float) facing);
    }
    /// <summary>
    /// Check that the Character do not fall
    /// </summary>
    public override void PlatformerUpdate(float delta) {
      if (doNotFall && pc2d.collisions.below) {
        if (!IsGroundOnLeft (rayLengthFactor, delta)) {
          OnLeftWall ();
        } else if (!IsGroundOnRight (rayLengthFactor, delta)) {
          OnRightWall ();
        }
      }

      base.PlatformerUpdate(delta);
    }
  }
}
