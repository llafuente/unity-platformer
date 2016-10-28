using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Push objects (Box)
  ///
  /// NOTE require CharacterActionGroundMovement
  /// </summary>
  public class CharacterActionPush: CharacterAction {
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
    /// Time to pushing before start moving the object
    /// </summary>
    [Comment("Time to pushing before start moving the object")]
    public float pushingStartTime = 0.5f;
    /// <summary>
    /// Limit Push to X
    ///
    /// if false, you will see problems while on slopes, but maybe
    /// it's desired...
    /// </summary>
    [Comment("Limit Push to X")]
    public bool forbidVerticalMovement = true;

    [Space(10)]

    /// <summary>
    /// Action priority
    /// </summary>
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 20;
    /// <summary>
    /// Where the Character was facing, for reseting pushingCD
    /// </summary>
    int faceDir = 0;
    /// <summary>
    /// Time Pushing to give some time before actually push
    /// </summary>
    Cooldown pushingCD;
    /// <summary>
    /// for Mathf.SmoothDamp
    /// </summary>
    float velocityXSmoothing;
    /// <summary>
    /// for Mathf.SmoothDamp
    /// </summary>
    CharacterActionGroundMovement groundMovement;

    public override void OnEnable() {
      base.OnEnable();

      pushingCD = new Cooldown(pushingStartTime);
      groundMovement = character.GetAction<CharacterActionGroundMovement>();
      if (groundMovement == null) {
        Debug.LogWarning("CharacterActionGroundMovement is required by CharacterActionPush");
      }

      character.onBeforeMove += OnBeforeMove;
    }
    /// <summary>
    /// Pushing and object enough time?
    /// </summary>
    public override int WantsToUpdate(float delta) {
      if (!pc2d.collisions.below) {
        return 0;
      }

      float x = input.GetAxisRawX();

      if (x > 0) {
        if (faceDir != 1) {
          pushingCD.Reset();
        }

        faceDir = 1;
        if (character.IsBox(Directions.Right) && pushingCD.IncReady()) {
          return priority;
        }
        return 0;
      }
      if (x < 0) {
        if (faceDir != -1) {
          pushingCD.Reset();
        }

        faceDir = -1;
        if (character.IsBox(Directions.Left) && pushingCD.IncReady()) {
          return priority;
        }
        return 0;
      }

      pushingCD.Reset();
      return 0;
    }
    /// <summary>
    /// EnterState and start centering
    /// </summary>
    public override void GainControl(float delta) {
      base.GainControl(delta);

      character.EnterState(States.Pushing);
      Log.level = LogLevel.Silly;
      Log.Silly("(Push) {0} Start pushing", gameObject.name);
    }
    /// <summary>
    /// EnterState and start centering
    /// </summary>
    public override void LoseControl(float delta) {
      base.LoseControl(delta);

      character.ExitState(States.Pushing);

      Log.Silly("(Push) {0} Stop pushing", gameObject.name);
      Log.level = LogLevel.Info;
    }
    /// <summary>
    /// Move Character, because the magic is in OnBeforeMove
    /// </summary>
    public override void PerformAction(float delta) {
      groundMovement.Move(speed, ref velocityXSmoothing, accelerationTime);
    }
    /// <summary>
    /// Move the Box before moving the character so there is a gap between them
    /// </summary>
    public void OnBeforeMove(Character ch, float delta) {
      // TODO test if give better results
      // disable all boxes
      // move character
      // enable all boxes
      // after move it use movedLastFrame as velocity
      // NOTE should not kill the character!

      if (ch.IsOnState(States.Pushing)) {
        PushBox(
          ch.velocity * delta,
          ch.faceDir == Facing.Right ? Directions.Right : Directions.Left,
          delta
        );
      }
    }
    /// <summary>
    /// Move a Box
    /// </summary>
    public void PushBox(Vector3 velocity, Directions dir, float delta) {
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
