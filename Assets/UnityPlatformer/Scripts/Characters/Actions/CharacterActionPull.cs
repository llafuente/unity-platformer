using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Push objects (Box)
  /// NOTE require CharacterActionGroundMovement
  /// </summary>
  public class CharacterActionPull: CharacterAction {
    [Comment("Movement speed")]
    public float speed = 3;
    [Comment("Time to reach max speed")]
    public float accelerationTime = .1f;
    //public float maxWeight =4f;
    [Comment("Limit Push to X")]
    public bool forbidVerticalPush = true;

    [Space(10)]
    [Comment("Must match something @PlatformerInput")]
    public String action = "Pull";

    [Space(10)]
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 20;

    internal int faceDir = 0;

    float velocityXSmoothing;
    CharacterActionGroundMovement groundMovement;

    public override void OnEnable() {
      base.OnEnable();

      groundMovement = character.GetAction<CharacterActionGroundMovement>();

      character.onAfterMove += OnAfterMove;
    }

    public override int WantsToUpdate(float delta) {
      if (!pc2d.collisions.below) {
        return 0;
      }

      float x = input.GetAxisRawX();

      if (input.IsActionHeld(action)) {
        if (x > 0) {
          faceDir = 1;
          if (character.IsBox(Directions.Left)) {
            return priority;
          }
        } else if (x < 0) {
          faceDir = -1;
          if (character.IsBox(Directions.Right)) {
            return priority;
          }
        }
      }

      return 0;
    }

    /// <summary>
    /// EnterState and start centering
    /// </summary>
    public override void GainControl(float delta) {
      base.GainControl(delta);

      character.EnterState(States.Pulling);
      Log.level = LogLevel.Silly;
      Log.Silly("(Push) {0} Start pulling", gameObject.name);
    }

    /// <summary>
    /// EnterState and start centering
    /// </summary>
    public override void LoseControl(float delta) {
      base.LoseControl(delta);

      character.ExitState(States.Pulling);

      Log.Silly("(Push) {0} Stop pulling", gameObject.name);
      Log.level = LogLevel.Info;
    }

    public override void PerformAction(float delta) {
      groundMovement.Move(speed, ref velocityXSmoothing, accelerationTime);
    }

    public void OnAfterMove(Character ch, float delta) {
      if (ch.IsOnState(States.Pulling)) {
        PullBox(
          ch.movedLastFrame, // do not use velocity
          ch.faceDir == Facing.Right ? Directions.Left : Directions.Right,
          delta
        );
      }
    }

    public void PullBox(Vector3 velocity, Directions dir, float delta) {
      if (forbidVerticalPush) {
        velocity.y = 0.0f;
      }

      Box b = character.GetLowestBox(dir);
      if (b) {
        b.boxCharacter.pc2d.Move(velocity, delta);
        b.boxMovingPlatform.PlatformerUpdate(delta);
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
