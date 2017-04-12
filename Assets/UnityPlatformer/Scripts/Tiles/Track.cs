using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  /// <summary>
  /// Data necessary to calculate Track velocity per Character
  /// </summary>
  public class TrackData {
    /// <summary>
    /// Track owner
    /// </summary>
    public Track track;
    /// <summary>
    /// Mathf.SmoothDamp
    /// </summary>
    public Vector3 appliedAcceleration = Vector3.zero;
    /// <summary>
    /// for Mathf.SmoothDamp
    /// </summary>
    public Vector3 smoothing = Vector3.zero;
    /// <summary>
    /// constructor
    /// </summary>
    public TrackData(Track t) {
      track = t;
    }
  };

  /// <summary>
  /// Track tile: Modify characters velocity, could be used for wind, moving floors
  ///
  /// NOTE: The velocity applied is applied after terminal velocity so it's an
  /// extra.\n
  /// NOTE: Tracks should not overlap. Character will enter the first one it hit
  /// then when leave the track will enter the other, only one track at a time
  /// </summary>
  public class Track : BoxTileTrigger {
    /// <summary>
    /// Velocity that will be added to characters inside track
    /// </summary>
    public Vector3 velocity = Vector3.zero;
    /// <summary>
    /// Time to reach max velocity
    /// </summary>
    public float accelerationTime = 0.5f;

#if UNITY_EDITOR
    /// <summary>
    /// Set layer to Configuration.tracksMask
    /// </summary>
    public override void Reset() {
      base.Reset();
      gameObject.layer = Configuration.instance.tracksMask;
    }
#endif

    /// <summary>
    /// Combo to check character state
    /// </summary>
    [Comment("Enable track if character is on given state.")]
    [EnumFlagsAttribute(order = 1)]
    public CharacterStatesCheck characterState;
    /// <summary>
    /// Delegate type
    /// </summary>
    public delegate void CharacterEvent(Character c);
    /// <summary>
    /// Called when a new Character enter the Track area
    /// </summary>
    public CharacterEvent onEnter;
    /// <summary>
    /// Called when a new Character leave the Track area
    /// </summary>
    public CharacterEvent onExit;
    /// <summary>
    /// Calculate character velocity
    /// </summary>
    protected void Accelerate(Character character, float delta) {
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
    /// Character enter: set track property and call onEnter
    /// </summary>
    override public void CharacterEnter(Character character) {
      Debug.LogFormat("CharacterEnter: {0}", character);
      // only the first track is enabled
      if (character.track == null) {
        if (!characterState.ValidStates(character)) {
          return;
        }

        Log.Silly("(Track) Enter " + character.gameObject.GetFullName());

        // delegate when to move to character class
        // this prevent fights between tile modifiers
        character.onBeforeMove += Accelerate;
        character.EnterArea(Areas.Track);
        character.track = new TrackData(this);

        if (onEnter != null) {
          onEnter(character);
        }
      }
    }
    /// <summary>
    /// Character leaves: remove track property and call onExit
    /// </summary>
    override public void CharacterExit(Character character) {
      // only leave if i'm the track that the character is using
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
    /// <summary>
    /// This is called when Character is in, two things can happen
    /// I'm alredy in this track: OK
    /// I'm not in this track: Enter if no other track is used
    /// </summary>
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
