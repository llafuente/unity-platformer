// onBeforeMove
using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Climb a ladder
  /// TODO moveToCenterTime/Speed
  /// </summary>
  public class CharacterActionPush: CharacterAction {
    #region public

    public float speed = 4;
    public float pushingStartTime = 0.5f;
    //public float maxWeight =4f;

    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 20;

    #endregion

    int faceDir = 0;
    Cooldown pushingCD;

    public override void Start() {
      base.Start();
      pushingCD = new Cooldown(pushingStartTime);

      character.onBeforeMove += OnBeforeMove;
    }

    public override int WantsToUpdate(float delta) {

      float x = input.GetAxisRawX();

      if (x > 0) {
        if (faceDir != 1) {
          pushingCD.Reset();
        }

        faceDir = 1;
        if (IsBoxRight() && pushingCD.IncReady()) {
          return priority;
        }
        return 0;
      }
      if (x < 0) {
        if (faceDir != -1) {
          pushingCD.Reset();
        }

        faceDir = -1;
        if (IsBoxLeft() && pushingCD.IncReady()) {
          return priority;
        }
        return 0;
      }

      pushingCD.Reset();
      return 0;
    }

    bool IsBox(ref RaycastHit2D[] hits, int count) {
      for (int i = 0; i < count; ++i) {
        Box b = hits[i].collider.gameObject.GetComponent<Box>();
        if (b != null) {
          // guard against dark arts
          if (Configuration.IsBox(b.collider.gameObject)) {
            return true;
          }
        }
      }

      return false;
    }

    bool IsBoxRight() {
      return IsBox(ref character.pc2d.collisions.rightHits, character.pc2d.collisions.rightHitsIdx);
    }

    bool IsBoxLeft() {
      return IsBox(ref character.pc2d.collisions.leftHits, character.pc2d.collisions.leftHitsIdx);
    }

    /// <summary>
    /// EnterState and start centering
    /// </summary>
    public override void GainControl(float delta) {
      base.GainControl(delta);

      character.EnterState(States.Pushing);
    }

    /// <summary>
    /// EnterState and start centering
    /// </summary>
    public override void LoseControl(float delta) {
      base.LoseControl(delta);

      character.ExitState(States.Pushing);
    }

    public override void PerformAction(float delta) {
    }

    public void OnBeforeMove(Character ch, float delta) {
      if (ch.IsOnState(States.Pushing)) {
        if (ch.pc2d.collisions.faceDir == 1) {
          PushBox(ch.velocity * delta, ref ch.pc2d.collisions.rightHits, ch.pc2d.collisions.rightHitsIdx);
        } else {
          PushBox(ch.velocity * delta, ref ch.pc2d.collisions.leftHits, ch.pc2d.collisions.leftHitsIdx);
        }
      }
    }

    public void PushBox(Vector3 velocity, ref RaycastHit2D[] hits, int count) {
      Debug.Log(velocity.ToString("F4"));

      for (int i = 0; i < count; ++i) {
        Box b = hits[i].collider.gameObject.GetComponent<Box>();
        if (b != null) {
          // guard against dark arts
          if (Configuration.IsBox(b.collider.gameObject)) {
            b.collider.Move(velocity);
            return;
          }
        }
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
