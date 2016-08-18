using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  public class Box : MonoBehaviour {
    /// <summary>
    /// Real Body, what is going to be moved
    /// </summary>
    public Character boxCharacter;
    public MovingPlatform boxMovingPlatform;
    internal BoxCollider2D body;

    void OnEnable() {
      body = GetComponent<BoxCollider2D>();
    }
  }
}
