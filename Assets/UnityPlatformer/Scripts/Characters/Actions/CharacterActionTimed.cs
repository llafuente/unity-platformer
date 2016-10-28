using System;
using UnityEngine;

// TODO honor UpdateManager.timeScale

namespace UnityPlatformer {
  /// <summary>
  /// Manage actions that has the loop: cast -> duration -> cooldown .
  ///
  /// Timeline explanation:\n
  /// Character hit the cast action: time = 0 (Play animation if we start casting)
  /// action has a castTime. During 0 to castTime we don't deal damage\n
  /// action has a durationTime. During castTime to castTime + durationTime
  /// we deal damage\n
  /// We have to wail from 0 to cooldownTime to cast again
  /// </summary>
  public abstract class CharacterActionTimed : CharacterAction {
    /// <summary>
    /// Casting time
    ///
    /// Time the character is waving their hands doing no Damage :)\n
    /// Time before enable the first damage area
    /// </summary>
    [Comment("Time before enable the first damage area")]
    public float castTime = 0;
    /// <summary>
    /// Action duration
    ///
    /// Duration of the sparks flying around killing people!\n
    /// Time after castTime
    /// </summary>
    [Comment("Time after 'Cast time': Action/Animation duration")]
    public float durationTime = 1.0f;
    /// <summary>
    /// Action/Animation duration
    ///
    /// Time after durationTime
    /// </summary>
    [Comment("Time since player start casting until being able to cast again")]
    public float cooldownTime  = 3.0f;
    /// <summary>
    /// Notify action is interrupted
    ///
    /// NOTE It's not used in the library right now, but you can
    /// </summary>
    public Action onInterrupt;
    /// <summary>
    /// castTime in frames
    /// </summary>
    protected int castFrames = 0;
    /// <summary>
    /// durationTime in frames
    /// </summary>
    protected int durationFrames = 0;
    /// <summary>
    /// cooldownTime in frames
    /// </summary>
    protected int cooldownFrames = 0;
    /// <summary>
    /// count frames the action is running
    /// </summary>
    protected int actionCounter = 0;
    /// <summary>
    /// counter for cooldown
    /// </summary>
    protected int cooldownCounter = 0;
    /// <summary>
    /// conver time to frames
    /// </summary>
    public override void OnEnable() {
      base.OnEnable();

      castFrames = UpdateManager.instance.GetFrameCount (castTime);
      durationFrames = UpdateManager.instance.GetFrameCount (durationTime);
      cooldownFrames = UpdateManager.instance.GetFrameCount (cooldownTime);

      cooldownCounter = cooldownFrames + 1;
      actionCounter = cooldownFrames + 1;
    }
    /// <summary>
    /// Start action!
    /// </summary>
    public virtual void StartAction() {
      cooldownCounter = 0;
      actionCounter = 0;
    }
    /// <summary>
    /// End action!
    /// </summary>
    public virtual void EndAction() {}
    /// <summary>
    /// interrupt current action
    /// </summary>
    /// <param name="resetCooldown">Reset cooldown. Allow to cast again!</param>
    public void Interrupt(bool resetCooldown) {
      if (onInterrupt != null) {
        onInterrupt();
      }

      actionCounter = cooldownFrames + 1;
      if (resetCooldown) {
        cooldownCounter = cooldownFrames + 1;
      }

      EndAction();
    }

    /// <summary>
    /// Casting time
    /// </summary>
    public bool IsCasting() {
      return actionCounter < castFrames;
    }
    /// <summary>
    /// Attacking time
    /// </summary>
    public bool IsActionInProgress() {
      return actionCounter - castFrames < durationFrames;
    }
    /// <summary>
    /// attack ended
    /// </summary>
    public bool IsActionComplete() {
      return actionCounter - castFrames > durationFrames;
    }
    /// <summary>
    /// Can perform action?
    /// </summary>
    public bool IsCooldown() {
      return cooldownCounter < cooldownFrames;
    }

    public override int WantsToUpdate(float delta) {
      ++cooldownCounter; // update here, to handle cooldown properly
      ++actionCounter;

      if (actionCounter == durationFrames + castFrames) {
        Debug.Log("Attack ended!");
        EndAction();
      }

      // this is not used... that's why we don't have priority in this action
      return 0;
    }
  }
}
