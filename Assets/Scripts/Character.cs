using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// A Characte. player movable, npc, enemy...
  /// </summary>
  public class Character: MonoBehaviour {
    CharacterAction[] actions;
    void Start() {
      actions = GetComponents<CharacterAction>();
    }

    // TODO REVIEW FixedUpdate?
    void Update() {
      int prio = 0;
      int tmp;
      CharacterAction action = null;

      foreach (var i in actions) {
        tmp = i.WantsToUpdate();
        if (tmp < 0) {
          i.PerformAction(Time.fixedDeltaTime);
        } else if (prio < tmp) {
          prio = tmp;
          action = i;
        }
      }

      if (action != null) {
        Debug.Log("PerformAction!");
        action.PerformAction(Time.fixedDeltaTime);
      }
    }
  }
}
;
