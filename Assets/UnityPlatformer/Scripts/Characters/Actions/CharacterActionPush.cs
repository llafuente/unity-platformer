// onBeforeMove
using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Push objects (Box)
  /// NOTE require CharacterActionGroundMovement
  /// </summary>
  public class CharacterActionPush: CharacterAction {
    #region public

    [Comment("Movement speed")]
    public float speed = 3;
    [Comment("Time to reach max speed")]
    public float accelerationTime = .1f;
    [Comment("Time to pushing before start moving the object")]
    public float pushingStartTime = 0.5f;
    //public float maxWeight =4f;
    [Comment("Limit Push to X")]
    public bool forbidVerticalPush = true;

    [Space(10)]
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 20;

    #endregion

    int faceDir = 0;
    Cooldown pushingCD;

    float velocityXSmoothing;
    CharacterActionGroundMovement groundMovement;

    public override void OnEnable() {
      base.OnEnable();

      pushingCD = new Cooldown(pushingStartTime);
      groundMovement = character.GetAction<CharacterActionGroundMovement>();

      character.onBeforeMove += OnBeforeMove;
    }

    public override int WantsToUpdate(float delta) {

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

    public override void PerformAction(float delta) {
      groundMovement.Move(speed, ref velocityXSmoothing, accelerationTime);
    }

    public void OnBeforeMove(Character ch, float delta) {
      if (ch.IsOnState(States.Pushing)) {
        PushBox(
          ch.velocity * delta,
          ref ch.pc2d.collisions.contacts,
          ch.faceDir == Facing.Right ? Directions.Right : Directions.Left,
          ch.pc2d.collisions.contactsIdx,
          delta
        );
      }
    }

    public void PushBox(Vector3 velocity, ref PlatformerCollider2D.Contacts[] contacts, Directions dir, int count, float delta) {
      if (forbidVerticalPush) {
        velocity.y = 0.0f;
      }
      // sarch the lowest box and push it
      float minY = Mathf.Infinity;
      int index = -1;
      Log.Silly("(Push) PushBox.count {0}", count);
      for (int i = 0; i < count; ++i) {
        if (contacts[i].dir != dir) continue;

        Box b = contacts[i].hit.collider.gameObject.GetComponent<Box>();
        if (b != null) {
          if (!Configuration.IsBox(b.boxCharacter.gameObject)) {
            Debug.LogWarning("Found a Character that should be a box", b.boxCharacter.gameObject);
            return;
          }

          float y = b.transform.position.y;
          Log.Silly("(Push) Found a box at index {0} {1} {2} {3}", i, b, minY, y);
          if (y < minY) {
            minY = y;
            index = i;
          }
        }
      }

      Log.Silly("(Push) will push {0} {1}", index, velocity.ToString("F4"));

      if (index != -1) {
        Box b = contacts[index].hit.collider.gameObject.GetComponent<Box>();
        b.boxCharacter.pc2d.Move(velocity, delta);
        b.boxMovingPlatform.PlatformerUpdate(delta);
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
