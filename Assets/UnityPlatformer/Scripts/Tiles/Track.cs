using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  public class TrackData {
    public Track track;
    public Vector3 appliedAcceleration = Vector3.zero;
    public Vector3 smoothing = Vector3.zero;

    public TrackData(Track t) {
      track = t;
    }
  };

  /// <summary>
  /// Track tile
  /// </summary>
  public class Track : BoxTileTrigger {
    /// <summary>
    /// Velocity that will be add to characters inside track
    /// </summary>
    public Vector3 velocity = Vector3.zero;
    public float accelerationTime = 0.5f;
#if UNITY_EDITOR
    /// <summary>
    /// Set layer to Configuration.ropesMask
    /// </summary>
    public override void Reset() {
      base.Reset();
      gameObject.layer = Configuration.instance.tracksMask;
    }
#endif
    [Comment("Enable track if character is on given state.")]
    [EnumFlagsAttribute(order = 1)]
        /// <summary>
    /// Combo to check character state
    /// </summary>
    public CharacterStatesCheck characterState;

    public delegate void CharacterEvent(Character c);
    public CharacterEvent onEnter;
    public CharacterEvent onExit;
    public void Accelerate(Character character, float delta) {
      Vector3 last = character.track.appliedAcceleration;
      // do not use Vector3.SmoothDamp has a different meaning...
      character.track.appliedAcceleration.x = Mathf.SmoothDamp (
        character.track.appliedAcceleration.x,
        velocity.x,
        ref character.track.smoothing.x,
        accelerationTime
      );
      character.track.appliedAcceleration.y = Mathf.SmoothDamp (
        character.track.appliedAcceleration.y,
        velocity.y,
        ref character.track.smoothing.y,
        accelerationTime
      );
      character.track.appliedAcceleration.z = Mathf.SmoothDamp (
        character.track.appliedAcceleration.z,
        velocity.z,
        ref character.track.smoothing.z,
        accelerationTime
      );

      character.worldVelocity += character.track.appliedAcceleration - last;
    }
    /// <summary>
    /// Enable track
    /// </summary>
    override public void CharacterEnter(Character character) {
      Debug.LogFormat("CharacterEnter: {0}", character);
      // only the first one enable the track
      if (character.track == null) {
        if (!characterState.ValidStates(character)) {
          return;
        }

        Log.Silly("(Track) Enter " + character.gameObject.GetFullName());
        character.onBeforeMove += Accelerate;
        character.EnterArea(Areas.Track);
        character.track = new TrackData(this);
        //character.worldVelocity += velocity;
        if (onEnter != null) {
          onEnter(character);
        }
      }
    }
    /// <summary>
    /// Disable track
    /// </summary>
    override public void CharacterExit(Character character) {
      // same as above, only diable if we leave the section we are grabbing
      if (character.track != null && character.track.track == this) {
        Log.Silly("(Track) Leave " + character.gameObject.GetFullName());
        character.onBeforeMove -= Accelerate;
        character.ExitArea(Areas.Track);
        // when character exit, we must continue to apply the velocity
        // but this time not as world, as 'Character' velocity to keep
        // a smooth exit
        // this will be ok unless you exceeed terminalVelocity
        character.worldVelocity -= character.track.appliedAcceleration;
        character.velocity += character.track.appliedAcceleration;
        character.track = null;
        if (onExit != null) {
          onExit(character);
        }
      }
    }
    override public void CharacterStay(Character character) {
      //Debug.LogFormat("CharacterStay: {0} {1} {2} {3} {4}", p, p.IsOnAnyState((States)state), p.track, p.worldVelocity, p.velocity);
      // already in? maybe exit
      if (character.track != null && character.track.track == this) {
        // check
        if (!characterState.ValidStates(character)) {
          CharacterExit(character);
        }
      } else {
        // not in? maybe enter
        if (characterState.ValidStates(character)) {
          CharacterEnter(character);
        }
      }
    }
  }
}
