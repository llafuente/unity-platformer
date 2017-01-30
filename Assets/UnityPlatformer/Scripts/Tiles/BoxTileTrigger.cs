using System;
using UnityEngine;
using System.Collections;

// TODO review memory allocation

namespace UnityPlatformer {
  /// <summary>
  /// Base class for Square trigger (BoxCollider2D)
  /// </summary>
  [RequireComponent (typeof (BoxCollider2D))]
  [RequireComponent (typeof (Rigidbody2D))]
  public class BoxTileTrigger : MonoBehaviour {
    // cache
    [HideInInspector]
    public BoxCollider2D body;
    [HideInInspector]
    public Character[] characters;
    [HideInInspector]
    public int charCount;

    /// <summary>
    /// force BoxCollider2D to be trigger
    /// </summary>
    virtual public void Start() {
      body = GetComponent<BoxCollider2D>();
      Reset();

      if (characters == null) {
        characters = new Character[4];
        charCount = 0;
      }
    }

    public virtual void Reset() {
      Utils.KinematicTrigger(gameObject);
    }

    /// <summary>
    /// Get real-world-coordinates center
    /// </summary>
    virtual public Vector3 GetCenter() {
      return body.bounds.center;
    }

    /// <summary>
    /// exit all player
    /// </summary>
    virtual public void OnDestroy() {
      while(charCount != 0) {
        CharacterExit(characters[0]);
      }
    }

    /// <summary>
    /// Add character to the list
    /// </summary>
    virtual public void CharacterEnter(Character p) {
      if (p == null) return;

      if (charCount == characters.Length) {
        Array.Resize(ref characters, charCount + 4);
      }

      characters[charCount++] = p;
    }

    /// <summary>
    /// Remove character from the list
    /// </summary>
    virtual public void CharacterExit(Character p) {
      if (p == null) return;

      bool overwrite = false;
      for (int i = 0; i < charCount; ++i) {
        if (overwrite || characters[i] == p) {
          overwrite = true;
          characters[i] = characters[i + 1];
        }
      }

      if (overwrite) {
        --charCount;
      }
    }
    /// <summary>
    /// Do nothing
    /// </summary>
    virtual public void CharacterStay(Character p) {
    }
    /// <summary>
    /// if Hitbox with EnterAreas enter -> CharacterEnter
    /// </summary>
    public virtual void OnTriggerEnter2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        CharacterEnter(h.owner.GetComponent<Character>());
      }
    }
    /// <summary>
    /// if Hitbox with EnterAreas leave -> CharacterExit
    /// </summary>
    public virtual void OnTriggerExit2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        CharacterExit(h.owner.GetComponent<Character>());
      }
    }
    /// <summary>
    /// if Hitbox with EnterAreas leave -> CharacterExit
    /// </summary>
    public virtual void OnTriggerStay2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        CharacterStay(h.owner.GetComponent<Character>());
      }
    }
  }
}
