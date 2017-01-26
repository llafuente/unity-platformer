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

  public class TrackCharacterAccel : SerializableDictionary<Character, float> {};

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
    public StatesMask state;

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
    override public void CharacterEnter(Character p) {
      //Debug.LogFormat("CharacterEnter: {0} {1}", p, p.IsOnAnyState((States)state));
      // only the first one enable the track
      if (p.track == null) {
        if (!p.IsOnAnyState((States)state)) {
          return;
        }

        Log.Silly("(Track) Enter " + p.gameObject.GetFullName());
        p.onBeforeMove += Accelerate;
        p.EnterArea(Areas.Track);
        p.track = new TrackData(this);
        //p.worldVelocity += velocity;
        if (onEnter != null) {
          onEnter(p);
        }
      }
    }
    /// <summary>
    /// Disable track
    /// </summary>
    override public void CharacterExit(Character p) {
      // same as above, only diable if we leave the section we are grabbing
      if (p.track != null && p.track.track == this) {
        Log.Silly("(Track) Leave " + p.gameObject.GetFullName());
        p.onBeforeMove -= Accelerate;
        p.ExitArea(Areas.Track);
        // when character exit, we must continue to apply the velocity
        // but this time not as world, as 'Character' velocity to keep
        // a smooth exit
        // this will be ok unless you exceeed terminalVelocity
        p.worldVelocity -= p.track.appliedAcceleration;
        p.velocity += p.track.appliedAcceleration;
        p.track = null;
        if (onExit != null) {
          onExit(p);
        }
      }
    }
    override public void CharacterStay(Character p) {
      //Debug.LogFormat("CharacterStay: {0} {1} {2} {3} {4}", p, p.IsOnAnyState((States)state), p.track, p.worldVelocity, p.velocity);
      // already in? maybe exit
      if (p.track != null && p.track.track == this) {
        // check
        if (!p.IsOnAnyState((States)state)) {
          CharacterExit(p);
        }
      } else {
        // not in? maybe enter
        if (p.IsOnAnyState((States)state)) {
          CharacterEnter(p);
        }
      }
    }
  }
}
