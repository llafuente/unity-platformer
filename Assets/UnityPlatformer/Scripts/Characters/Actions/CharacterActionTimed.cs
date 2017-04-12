using System;
using UnityEngine;

// TODO honor UpdateManager.timeScale

namespace UnityPlatformer {
  public enum CharacterActionTimedState {
    Ready,
    Casting,
    InProgress,
    Cooldown
  };

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
    protected Delay casting;
    /// <summary>
    /// durationTime in frames
    /// </summary>
    protected Delay inprogress;
    /// <summary>
    /// cooldownTime in frames
    /// </summary>
    protected Delay cooldown;
    /// <summary>
    /// Current action state
    /// </summary>
    protected CharacterActionTimedState state = CharacterActionTimedState.Ready;
    /// <summary>
    /// Create Delays
    /// </summary>
    public override void OnEnable() {
      base.OnEnable();

      if (casting == null) {
        casting = UpdateManager.GetDelay(castTime);
      }

      if (inprogress == null) {
        inprogress = UpdateManager.GetDelay(castTime + durationTime);
      }

      if (cooldown == null) {
        cooldown = UpdateManager.GetDelay(castTime + durationTime + cooldownTime);
      }

      casting.Clear();
      inprogress.Clear();
      cooldown.Clear();
    }
    /// <summary>
    /// Dispose Delays
    /// </summary>
    public override void OnDisable() {
      base.OnDisable();

      UpdateManager.DisposeDelay(casting);
      UpdateManager.DisposeDelay(inprogress);
      UpdateManager.DisposeDelay(cooldown);

      casting = null;
      inprogress = null;
      cooldown = null;
    }
    /// <summary>
    /// Start action!
    /// </summary>
    public override void GainControl(float delta) {
      base.GainControl(delta);

      casting.Reset();
      inprogress.Reset();
      cooldown.Reset();
      state = CharacterActionTimedState.Casting;
    }
    /// <summary>
    /// interrupt current action
    /// </summary>
    /// <param name="resetCooldown">Reset cooldown. Allow to cast again!</param>
    public void Interrupt(bool resetCooldown) {
      if (onInterrupt != null) {
        onInterrupt();
      }

      if (resetCooldown) {
        casting.Clear();
        inprogress.Clear();
        cooldown.Clear();
      }

      // TODO REVIEW call LoseControl manually...
    }

    /// <summary>
    /// in castTime?
    /// </summary>
    public bool IsCasting() {
      return !casting.Fulfilled();
    }
    /// <summary>
    /// after castTime, in durationTime?
    /// </summary>
    public bool IsActionInProgress() {
      return !IsCasting() && !inprogress.Fulfilled();
    }
    /// <summary>
    /// attack ended
    /// </summary>
    public bool IsDone() {
      return inprogress.Fulfilled();
    }
    /// <summary>
    /// Can perform action?
    /// </summary>
    public bool IsCooldown() {
      return cooldown.Fulfilled();
    }

    public override int WantsToUpdate(float delta) {
      if (state == CharacterActionTimedState.Casting && casting.Fulfilled()) {
        state = CharacterActionTimedState.InProgress;
      }

      if (state == CharacterActionTimedState.InProgress && inprogress.Fulfilled()) {
        state = CharacterActionTimedState.Cooldown;
      }

      if (state == CharacterActionTimedState.Cooldown && cooldown.Fulfilled()) {
        state = CharacterActionTimedState.Ready;
      }

      // NOTE this MUST not used...
      // return desired priority at the sub-class
      return 0;
    }
  }
}
