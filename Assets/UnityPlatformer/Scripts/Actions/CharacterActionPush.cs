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

    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 20;

    #endregion

    int faceDir = 0;
    Cooldown pushingCD;

    float velocityXSmoothing;
    CharacterActionGroundMovement groundMovement;

    public override void Start() {
      base.Start();
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
      groundMovement.Move(speed, ref velocityXSmoothing, accelerationTime);
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
      int idx = -1;
      float yPos = float.PositiveInfinity;
      for (int i = 0; i < count; ++i) {
        Box b = hits[i].collider.gameObject.GetComponent<Box>();
        if (b != null) {
          // guard against dark arts
          if (Configuration.IsBox(b.collider.gameObject) && yPos > b.transform.position.y) {
            yPos = b.transform.position.y;
            idx = i;
          }
        }
      }

      if (idx != -1) {
        hits[idx].collider.gameObject.GetComponent<Box>().collider.Move(velocity);
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
