using System;
using UnityEngine;

/// TODO animationName (so we could supports multiple melee attacks)

namespace UnityPlatformer {
  /// <summary>
  /// Melee attack
  ///
  /// NOTE Can't be interrupted atm
  /// </summary>
  public class CharacterActionMelee: CharacterActionTimed {
    /// <summary>
    /// Melee Damage data
    /// </summary>
    [Serializable]
    public class MeleeDamage {
      /// <summary>
      /// An object with a DamageType
      /// </summary>
      public GameObject area;
      /// <summary>
      /// When enable the Damage area
      /// </summary>
      public float startAt = 0.0f;
      /// <summary>
      /// time in frames
      /// </summary>
      internal int offsetFrame = 0;
      /// <summary>
      /// is currently active
      /// </summary>
      internal bool active = false;
    }
    /// <summary>
    /// List of Damage areas, something like moving hitboxes
    /// </summary>
    public MeleeDamage[] damageAreas = new MeleeDamage[1];
    /// <summary>
    /// Input action name
    /// </summary>
    public string action = "Attack";

    [Space(10)]

    /// <summary>
    /// Action priority
    /// </summary>
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 20;
    /// <summary>
    /// listen action input events
    /// </summary>
    bool attackHeld = false;

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
    /// <summary>
    /// input.onActionDown callabck
    /// </summary>
    public void OnActionDown(string _action) {
      if (_action == action) {
        attackHeld = true;
        Reset();
      }
    }
    /// <summary>
    /// input.onActionUp callabck
    /// </summary>
    public void OnActionUp(string _action) {
      // when button is up, reset, and allow a new jump
      if (_action == action) {
        attackHeld = false;
        Reset();
      }
    }
    /// <summary>
    /// Disable all areas
    /// </summary>
    public void Reset() {
      for (var i = 0; i < damageAreas.Length; ++i) {
        damageAreas[i].area.SetActive(false);
      }
    }
    /// <summary>
    /// EnterState
    /// </summary>
    public override void StartAction() {
      base.StartAction();

      character.EnterState(States.MeleeAttack);
    }
    /// <summary>
    /// ExitState
    /// </summary>
    public override void EndAction() {
      base.EndAction();

      character.ExitState(States.MeleeAttack);
    }
    /// <summary>
    /// attackHeld &amp; and previous attack ended
    ///
    /// TODO REVIEW continous attack ?
    /// </summary>
    public override int WantsToUpdate(float delta) {
      base.WantsToUpdate(delta);

      if (attackHeld || !IsActionComplete()) {
        // attack starts!
        if (!IsCooldown()) {
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
