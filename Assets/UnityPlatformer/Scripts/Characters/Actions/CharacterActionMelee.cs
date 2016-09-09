using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Melee attack
  /// NOTE Can't be interrupted atm
  /// TODO animationName (so we could supports multiple melee attacks)
  /// </summary>
  public class CharacterActionMelee: CharacterActionTimed {
    #region public

    [Serializable]
    public class MeleeDamage {
      // this must contains a DamageType
      public GameObject area;
      public float startAt = 0.0f;

      internal int offsetFrame = 0;
      internal bool active = false;
    }

    public MeleeDamage[] damageAreas = new MeleeDamage[1];
    public string action = "Attack";

    [Space(10)]
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 20;

    #endregion

    #region private

    bool attackHeld = false;

    #endregion

    public override void OnEnable() {
      base.OnEnable();

      damageAreas[0].startAt = damageAreas[0].offsetFrame = 0;
      for (var i = 1; i < damageAreas.Length; ++i) {
        damageAreas[i].offsetFrame = damageAreas[0].offsetFrame + UpdateManager.instance.GetFrameCount (damageAreas[i].startAt);
        // Debug.LogFormat("Offsetframes {0} - {1}", i, damageAreas[i].offsetFrame);
      }

      input.onActionUp += OnActionUp;
      input.onActionDown += OnActionDown;

      Reset();
    }

    public void OnActionDown(string _action) {
      if (_action == action) {
        attackHeld = true;
        Reset();
      }
    }

    public void OnActionUp(string _action) {
      // when button is up, reset, and allow a new jump
      if (_action == action) {
        attackHeld = false;
        Reset();
      }
    }

    public void Reset() {
      for (var i = 0; i < damageAreas.Length; ++i) {
        damageAreas[i].area.SetActive(false);
      }
    }

    public override void StartAction() {
      base.StartAction();

      character.EnterState(States.MeleeAttack);
    }

    public override void EndAction() {
      base.EndAction();

      character.ExitState(States.MeleeAttack);
    }


    /// <summary>
    /// TODO REVIEW continous attack ?
    /// </summary>
    public override int WantsToUpdate(float delta) {
      base.WantsToUpdate(delta);

      if (attackHeld || !IsActionComplete()) {
        // attack starts!
        if (!IsColdown()) {
          StartAction();
          return priority;
        }

        return priority;
      }

      return 0;
    }

    public override void PerformAction(float delta) {
      if (IsActionInProgress()) {
        var offset = actionCounter - castFrames;
        for (var i = 0; i < damageAreas.Length; ++i) {
          if (
            (i == damageAreas.Length -1 && offset >= damageAreas[i].offsetFrame) ||
            (offset >= damageAreas[i].offsetFrame && offset < damageAreas[i + 1].offsetFrame)
          ) {
            damageAreas[i].area.SetActive(true);
          } else {
            damageAreas[i].area.SetActive(false);
          }
        }
      } else {
        damageAreas[damageAreas.Length -1].area.SetActive(false);
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
