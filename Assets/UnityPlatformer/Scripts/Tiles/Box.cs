using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  /// <summary>
  /// Box Tile
  /// A box is a combination of a Character and MovingPlatform.
  /// </summary>
  public class Box : MonoBehaviour {
    /// <summary>
    /// Real Body, what is going to be moved
    /// </summary>
    public Character boxCharacter;
    /// <summary>
    /// Fake MovingPlatform so Character can stand over the box and move with it
    /// </summary>
    public MovingPlatform boxMovingPlatform;
    internal BoxCollider2D body;

    void OnEnable() {
      body = GetComponent<BoxCollider2D>();
    }
  }
}
