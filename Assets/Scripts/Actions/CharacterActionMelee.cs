using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Melee attack
  /// NOTE Can't be interrupt
  /// </summary>
  public class CharacterActionMelee: CharacterAction {
    #region public
    [Serializable]
    public class TriggersData {
      public GameObject area;
      public float startAt = 0.0f;

      [HideInInspector]
      public int offsetFrame = 0;
      [HideInInspector]
      public bool active = false;
    }

    [Comment("Time before enable the first damage area")]
    public float castTime = 0;
    public float durationTime = 1.0f;
    [Comment("Time since player start casting until being able to cast again")]
    public float coldownTime  = 3.0f;
    public TriggersData[] damageAreas = new TriggersData[1];

    public string action = "Attack";
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 20;
    #endregion

    #region private

    bool attackHeld = false;

    int castFrames = 0;
    int durationFrames = 0;
    int coldownFrames = 0;

    int atCounter = 0;
    int cdCounter = 0;

    #endregion

    public override void Start() {
      base.Start();

      Debug.Log("character" + character);

      castFrames = UpdateManager.instance.GetFrameCount (castTime);
      durationFrames = UpdateManager.instance.GetFrameCount (durationTime);
      coldownFrames = UpdateManager.instance.GetFrameCount (coldownTime);

      damageAreas[0].startAt = damageAreas[0].offsetFrame = 0;
      for (var i = 1; i < damageAreas.Length; ++i) {
        damageAreas[i].offsetFrame = damageAreas[0].offsetFrame + UpdateManager.instance.GetFrameCount (damageAreas[i].startAt);
        Debug.LogFormat("Offsetframes {0} - {1}", i, damageAreas[i].offsetFrame);
      }

      input.onActionUp += OnActionUp;
      input.onActionDown += OnActionDown;

      cdCounter = coldownFrames + 1;
      atCounter = coldownFrames + 1;
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
        damageAreas[i].area.active = false;
      }
    }

    public void StartAction() {
      cdCounter = 0;
      atCounter = 0;
    }

    /// <summary>
    /// Casting time
    /// </summary>
    public bool IsCasting() {
      return atCounter < castFrames;
    }
    /// <summary>
    /// Attacking time
    /// </summary>
    public bool IsAttacking() {
      return atCounter - castFrames < durationFrames;
    }
    /// <summary>
    /// attack ended
    /// </summary>
    public bool IsAttackComplete() {
      return atCounter - castFrames > durationFrames;
    }

    public bool IsColdown() {
      return cdCounter < coldownFrames;
    }

    /// <summary>
    /// TODO REVIEW jump changes when moved to action, investigate
    /// </summary>
    public override int WantsToUpdate(float delta) {
      ++cdCounter; // update here, to handle coldown properly
      ++atCounter;

      if (attackHeld || !IsAttackComplete()) {
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
      if (IsAttacking()) {
        var offset = atCounter - castFrames;
        for (var i = 0; i < damageAreas.Length; ++i) {
          if (
            (i == damageAreas.Length -1 && offset >= damageAreas[i].offsetFrame) ||
            (offset >= damageAreas[i].offsetFrame && offset < damageAreas[i + 1].offsetFrame)
          ) {
            damageAreas[i].area.active = true;
          } else {
            damageAreas[i].area.active = false;
          }
        }
      } else {
        damageAreas[damageAreas.Length -1].area.active = false;
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
