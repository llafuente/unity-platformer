using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Manage actions that hast cast, duration and cooldown.
  /// </summary>
  public abstract class CharacterActionTimed : CharacterAction {
    #region public

    [Comment("Time before enable the first damage area")]
    public float castTime = 0;
    [Comment("Time after cast time the animation last")]
    public float durationTime = 1.0f;
    [Comment("Time since player start casting until being able to cast again")]
    public float cooldownTime  = 3.0f;

    Action onInterrupt;

    #endregion

    #region private

    protected int castFrames = 0;
    protected int durationFrames = 0;
    protected int coldownFrames = 0;

    protected int actionCounter = 0;
    protected int cdCounter = 0;
    #endregion

    public override void Start() {
      base.Start();

      castFrames = UpdateManager.instance.GetFrameCount (castTime);
      durationFrames = UpdateManager.instance.GetFrameCount (durationTime);
      coldownFrames = UpdateManager.instance.GetFrameCount (cooldownTime);

      cdCounter = coldownFrames + 1;
      actionCounter = coldownFrames + 1;
    }

    public virtual void StartAction() {
      cdCounter = 0;
      actionCounter = 0;
    }

    public virtual void EndAction() {
    }

    ///<summary>
    /// interrupt current action, to reset cooldown send true
    ///</summary>
    public void Interrupt(bool resetCd) {
      if (onInterrupt != null) {
        onInterrupt();
      }

      actionCounter = coldownFrames + 1;
      if (resetCd) {
        cdCounter = coldownFrames + 1;
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

    public bool IsColdown() {
      return cdCounter < coldownFrames;
    }

    public override int WantsToUpdate(float delta) {
      ++cdCounter; // update here, to handle coldown properly
      ++actionCounter;

      if (actionCounter == durationFrames + castFrames) {
        Debug.Log("Attck ended!");
        EndAction();
      }

      return 0;
    }
  }
}
