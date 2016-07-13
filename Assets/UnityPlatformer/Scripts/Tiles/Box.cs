using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  public class Box : MonoBehaviour/*, IUpdateEntity*/ {
    /// <summary>
    /// Real Body, what is going to be moved
    /// </summary>
    public Character boxCharacter;
    internal BoxCollider2D body;

    void OnEnable() {
      body = GetComponent<BoxCollider2D>();
    }
  }
}
