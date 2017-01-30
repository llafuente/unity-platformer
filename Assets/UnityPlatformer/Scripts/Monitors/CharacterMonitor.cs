using System;
using UnityEngine;
using System.Reflection;


namespace UnityPlatformer {
  /// <summary>
  /// Monitor for Character
  /// </summary>
  [RequireComponent (typeof (Character))]
  public class CharacterMonitor : PlatformerCollider2DMonitor {
    /// <summary>
    /// Character
    /// </summary>
    Character character;
    /// <summary>
    /// Log state changes
    /// </summary>
    bool logStateChanges = false;

    override public void Start() {
      base.Start ();
      character = GetComponent<Character> ();
      if (logStateChanges) {
        character.onStateChange += LogStateChanges;
      }
    }
    /// <summary>
    /// Log state changes
    /// </summary>
    void LogStateChanges(States before, States after) {
      // this is a lot noisy, useful for debuging for nothing more...
      Debug.LogFormat("state before: {0} / after: {1}", before, after);
    }

    override public void Update() {
      base.Update ();

      string states = "";
      Type typ = typeof(StatesMask);
      FieldInfo[] fields = typ.GetFields();
      int[] values = (int[]) Enum.GetValues(typ);
      int i = 0;
      foreach (var item in fields) { /*Enum.GetValues(typeof(StatesMask))) {*/
        if (item.Name == "value__") {
          continue;
        }

        if (((int)values[i] & (int)character.state) != 0) {
          states += item.Name + ",";
        }

        ++i;
      }

      text += string.Format(
        "Area: {0}\n"+
        "State: {1}\n"+
        "Facing: {15} ForceAni: {12}\n"+
        "Ladder: {2} IsAboveTop {3} IsBelowBottom {4}\n" +
        "Fence: {16}\n" +
        "Liquid: {9}  IsBelowSurface {10} / {11}\n" +
        "Rope: {13} @ {14}\n" +
        "Platform: {5}\n" +
        "Jump: {6}\n" +
        "Velocity: {7} - {8}\n",
        character.area.ToString(),
        states,
        character.ladder ? character.ladder.gameObject : null,
        character.ladder ? character.ladder.IsAboveTop(character, character.feet) : false,
        character.ladder ? character.ladder.IsBelowBottom(character, character.feet) : false,
        character.platform,
        character.lastJumpDistance,
        character.velocity.ToString("F4"), character.collisions.velocity.ToString("F4"),
        character.liquid,
        character.liquid ? character.liquid.IsBelowSurface(character, 1.5f) : false,
        character.liquid ? character.liquid.DistanceToSurface(character, 1.5f) : -1,
        character.forceAnimation,
        character.rope, character.ropeIndex,
        character.faceDir,
        character.fence
      );
    }
  }
}
