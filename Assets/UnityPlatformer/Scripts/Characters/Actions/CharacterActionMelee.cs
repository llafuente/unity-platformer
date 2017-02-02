using System;
using UnityEngine;
using UnityEngine.Assertions;

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
      [HideInInspector]
      public float endAt = 0;
      /// <summary>
      /// is currently active
      /// </summary>
      [HideInInspector]
      public bool active = false;
    };
    /// <summary>
    /// Set forceAnimation at character if not empty
    /// </summary>
    public string forceAnimation = "";
    /// <summary>
    /// List of Damage areas, something like moving hitboxes
    /// </summary>
    public MeleeDamage[] damageAreas = new MeleeDamage[1];
    /// <summary>
    /// Input action name
    /// </summary>
    public string action = "Attack";
    /// <summary>
    /// Combo to check character state
    /// </summary>
    public CharacterStatesCheck characterState;

    [Space(10)]

    /// <summary>
    /// Action priority
    /// </summary>
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 20;
    /// <summary>
    /// listen action input events
    /// </summary>
    protected bool attackHeld = false;
    protected float startTime = 0.0f;

    public override void OnEnable() {
      base.OnEnable();

      damageAreas[0].startAt = 0;
      for (var i = 0; i < damageAreas.Length - 1; ++i) {
        damageAreas[i].endAt = damageAreas[i + 1].startAt;

        Assert.IsNotNull(damageAreas[i].area,
          "CharacterActionMelee area[" + i +"] is required at " + gameObject.GetFullName());

        Assert.IsTrue(damageAreas[i].startAt < durationTime,
          "CharacterActionMelee area[" + i +"] startAt greater than durationTime at " + gameObject.GetFullName());
      }
      damageAreas[damageAreas.Length - 1].endAt = durationTime;

      input.onActionUp += OnActionUp;
      input.onActionDown += OnActionDown;

      Clear();
    }
    /// <summary>
    /// input.onActionDown callabck
    /// </summary>
    public void OnActionDown(string _action) {
      if (_action == action) {
        attackHeld = true;
        Clear();
      }
    }
    /// <summary>
    /// input.onActionUp callabck
    /// </summary>
    public void OnActionUp(string _action) {
      // when button is up, reset, and allow a new jump
      if (_action == action) {
        attackHeld = false;
        Clear();
      }
    }
    /// <summary>
    /// Disable all areas
    /// </summary>
    public void Clear() {
      for (var i = 0; i < damageAreas.Length; ++i) {
        damageAreas[i].area.SetActive(false);
      }
    }
    /// <summary>
    /// EnterState
    /// </summary>
    public override void GainControl(float delta) {
      base.GainControl(delta);

      character.EnterState(States.MeleeAttack);
      if (forceAnimation.Length != 0) {
        character.forceAnimation = forceAnimation;
      }

      character.meleeInProgress = this;
      startTime = UpdateManager.instance.runningTime;
      character.turnAllowed = false;
    }
    /// <summary>
    /// ExitState
    /// </summary>
    public override void LoseControl(float delta) {
      base.LoseControl(delta);

      character.ExitState(States.MeleeAttack);
      Clear();

      if (character.forceAnimation == forceAnimation) {
        character.forceAnimation = null;
      }

      character.meleeInProgress = null;
      character.turnAllowed = true;
    }
    /// <summary>
    /// attackHeld &amp; and previous attack ended
    ///
    /// TODO REVIEW continous attack ?
    /// </summary>
    public override int WantsToUpdate(float delta) {
      base.WantsToUpdate(delta);

      // only one attack is allowed at a time
      // TODO NOTE REVIEW maybe we may use lastAction but this is more explicit
      if (character.meleeInProgress != null && character.meleeInProgress != this) {
        return 0;
      }

      characterState.ValidStates(character);

      if (attackHeld || !IsDone()) {
        // attack in progress
        return priority;
      }

      return 0;
    }

    public override void PerformAction(float delta) {
      if (state == CharacterActionTimedState.InProgress) {
        float offset = UpdateManager.instance.runningTime - startTime;

        for (int i = 0; i < damageAreas.Length; ++i) {
          if (
            offset >= damageAreas[i].startAt &&
            offset < damageAreas[i].endAt
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
