using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  /// <summary>
  /// Track tile
  /// </summary>
  public class Track : BoxTileTrigger {
    /// <summary>
    /// Velocity that will be add to characters inside track
    /// </summary>
    public Vector3 velocity;
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
    public States state;
    /// <summary>
    /// Enable track
    /// </summary>
    override public void CharacterEnter(Character p) {
      // only the first one enable the track
      if (p.track == null) {
        if (state != States.None && !p.IsOnState(state)) {
          return;
        }

        Log.Silly("(Track) Enter " + p.gameObject.GetFullName());
        p.EnterArea(Areas.Track);
        p.track = this;
        p.worldVelocity += velocity;
      }
    }
    /// <summary>
    /// Disable track
    /// </summary>
    override public void CharacterExit(Character p) {
      // same as above, only diable if we leave the section we are grabbing
      if (p.track == this) {
        Log.Silly("(Track) Leave " + p.gameObject.GetFullName());
        p.ExitArea(Areas.Track);
        p.track = null;
        p.worldVelocity -= velocity;
      }
    }
    override public void CharacterStay(Character p) {
      Debug.Log(p.IsOnState(state));
      // already in? maybe exit
      if (p.track == this) {
        // check
        if (state != States.None && !p.IsOnState(state)) {
          CharacterExit(p);
        }
      } else {
        // not in? maybe enter
        if (state != States.None && p.IsOnState(state)) {
          CharacterEnter(p);
        }
      }
    }
  }
}
