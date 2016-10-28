using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Push objects (Box)
  /// NOTE require CharacterActionGroundMovement
  /// </summary>
  public class CharacterActionPull: CharacterAction {
    /// <summary>
    /// Movement speed
    /// </summary>
    [Comment("Movement speed")]
    public float speed = 3;
    /// <summary>
    /// Time to reach max speed
    /// </summary>
    [Comment("Time to reach max speed")]
    public float accelerationTime = .1f;
    /// <summary>
    /// Limit Pull movement to X
    ///
    /// if false, you will see problems while on slopes, but maybe
    /// it's desired...
    /// </summary>
    [Comment("Limit Pull movement to X")]
    public bool forbidVerticalMovement = true;

    [Space(10)]

    /// <summary>
    /// Input action name
    /// </summary>
    [Comment("Must match something @PlatformerInput")]
    public String action = "Pull";

    [Space(10)]

    /// <summary>
    /// Action priority
    /// </summary>
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 20;
    /// <summary>
    /// Real facing direction, while pulling you 'walk backwards'
    /// </summary>
    internal int faceDir = 0;
    /// <summary>
    /// for Mathf.SmoothDamp
    /// </summary>
    float velocityXSmoothing;
    /// <summary>
    /// CharacterActionGroundMovement
    /// </summary>
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
    /// EnterState
    /// </summary>
    public override void GainControl(float delta) {
      base.GainControl(delta);

      character.EnterState(States.Pulling);
      //Log.level = LogLevel.Silly;
      Log.Silly("(Push) {0} Start pulling", gameObject.name);
    }
    /// <summary>
    /// ExitState
    /// </summary>
    public override void LoseControl(float delta) {
      base.LoseControl(delta);

      character.ExitState(States.Pulling);

      Log.Silly("(Push) {0} Stop pulling", gameObject.name);
      //Log.level = LogLevel.Info;
    }
    /// <summary>
    /// Move Character, because the magic is in OnAfterMove
    /// </summary>
    public override void PerformAction(float delta) {
      groundMovement.Move(speed, ref velocityXSmoothing, accelerationTime);
    }
    /// <summary>
    /// Move the Box after moving the character, so there is a gap between them
    /// </summary>
    public void OnAfterMove(Character ch, float delta) {
      if (ch.IsOnState(States.Pulling)) {
        PullBox(
          ch.movedLastFrame, // do not use velocity
          ch.faceDir == Facing.Right ? Directions.Left : Directions.Right,
          delta
        );
      }
    }
    /// <summary>
    /// Move a Box
    /// </summary>
    public void PullBox(Vector3 velocity, Directions dir, float delta) {
      if (forbidVerticalMovement) {
        velocity.y = 0.0f;
      }

      Box b = character.GetLowestBox(dir);
      if (b) {
        b.boxCharacter.pc2d.Move(velocity, delta);
        b.boxMovingPlatform.PlatformerUpdate(delta);
      }
    }
    /// <summary>
    /// default
    /// </summary>
    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
