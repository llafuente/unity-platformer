using System;
using UnityEngine;
using UnityPlatformer.Characters;

namespace UnityPlatformer.AI {
  ///<summary>
  /// Patrol Artificial inteligence.
  /// NOTE does not require anything but it's recommended:
  /// CharacterActionAirMovement and CharacterActionGroundMovement
  ///</summary>
  public class AIGoomba: Enemy {
    public enum Facing {
      Left = -1,
      Right = 1
    };

    public Facing initialFacing;
    [Comment("Distance to test if ground is on left/right side. Helps when Enemy standing on platform moving down.")]
    public float rayLengthFactor = 1.0f;
    [Comment("Do not fall on platform edge. Go back.")]
    public bool doNotFall = true;


    Facing facing;
    AIInput input;

    public override void Start() {
      base.Start();
      input = GetComponent<AIInput>();
      controller.collisions.onLeftWall += OnLeftWall;
      controller.collisions.onRightWall += OnRightWall;

      facing = initialFacing;
    }

    void OnLeftWall() {
      facing = Facing.Right;
      input.SetX((float) facing);
    }

    void OnRightWall() {
      facing = Facing.Left;
      input.SetX((float) facing);
    }

    void Toogle() {
      facing = facing == Facing.Left ? Facing.Right : Facing.Left;
      input.SetX((float) facing);
    }

    public override void ManagedUpdate(float delta) {
      if (doNotFall) {
        if (!controller.IsGroundOnLeft (rayLengthFactor)) {
          OnLeftWall ();
        } else if (!controller.IsGroundOnRight (rayLengthFactor)) {
          OnRightWall ();
        }
      }

      base.ManagedUpdate(delta);
    }
  }
}
