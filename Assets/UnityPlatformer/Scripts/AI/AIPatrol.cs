using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Patrol Artificial inteligence.
  ///
  /// Does not require Actions but it's recommended because:\n
  /// If no CharacterActionAirMovement it will froze while on air\n
  /// If no CharacterActionGroundMovement it will froze while on ground\n
  /// </summary>
  public class AIPatrol: Enemy {
    [Header("Patrol")]
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
    /// false: when reach an edge, just fall
    /// </summary>
    [Comment("Do not fall on platform edge. Go back.")]
    public bool doNotFall = true;
    /// <summary>
    /// true: when turn set velocity to zero so next frame will start moving
    /// the other direction\n
    /// false: when turn, do not touch velocity, will decelerate
    /// before change direction
    /// </summary>
    public bool resetVelocityOnTurn = false;
    /// <summary>
    /// Where is facing now
    /// </summary>
    internal Facing facing;
    /// <summary>
    /// Listen onLeftWall, onRightWall and start moving in facing direction
    /// </summary>
    public override void Start() {
      base.Start();

      onLeftWall += OnLeftWall;
      onRightWall += OnRightWall;

      facing = initialFacing;
      input.SetX((float) facing);
    }
    /// <summary>
    /// Move right
    /// </summary>
    virtual public void OnLeftWall() {
      //Debug.Log("OnLeftWall");
      facing = Facing.Right;
      if (resetVelocityOnTurn) {
        velocity = Vector3.zero;
      }
      input.SetX((float) facing);
    }
    /// <summary>
    /// Move left
    /// </summary>
    virtual public void OnRightWall() {
      //Debug.Log("OnRightWall");
      facing = Facing.Left;
      if (resetVelocityOnTurn) {
        velocity = Vector3.zero;
      }
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
    /// Move in the other direction
    /// </summary>
    virtual public void Stop() {
      input.SetX(0);
    }
    /// <summary>
    /// Move in the other direction
    /// </summary>
    virtual public void Resume() {
      input.SetX((float) facing);
    }
    /// <summary>
    /// Check that the Character do not fall
    /// </summary>
    public override void LatePlatformerUpdate(float delta) {
      if (doNotFall && collisions.belowFrames > 3) {
        if (!IsGroundOnLeft (rayLengthFactor, delta) && velocity.x < 0) {
          OnLeftWall ();
        } else if (!IsGroundOnRight (rayLengthFactor, delta) && velocity.x > 0) {
          OnRightWall ();
        }
      }

      base.LatePlatformerUpdate(delta);
    }
  }
}
