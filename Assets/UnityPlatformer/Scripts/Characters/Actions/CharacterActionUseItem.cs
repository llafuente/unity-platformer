using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// TODO We need some kind of debounce / Use delay
  /// </summary>
  public class CharacterActionUseItem: CharacterAction {
    /// <summary>
    /// Offset position
    /// </summary>
    public Vector3 characterOffset = new Vector3(0, 0, 0);
    /// <summary>
    /// Trigger input Action 'Use'
    /// </summary>
    public string actionUse = "Use";

    [Space(10)]

    /// <summary>
    /// Action priority
    /// </summary>
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 30;
    /// <summary>
    /// On(Areas.Item) and item is usable by me and hit 'Use' action!
    /// </summary>
    public override int WantsToUpdate(float delta) {
      if (
        character.IsOnArea(Areas.Item) &&
        pc2d.collisions.below &&
        character.item.IsUsableBy(character) &&
        input.IsActionHeld(actionUse)
      ) {
        return priority;
      }
      return 0;
    }
    /// <summary>
    /// Use the item, all logic is in the Item.Use
    /// </summary>
    public override void PerformAction(float delta) {
      character.item.Use(character);
    }
    /// <summary>
    /// REVIEW do default actions. but maybe should do nothing...
    /// </summary>
    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
