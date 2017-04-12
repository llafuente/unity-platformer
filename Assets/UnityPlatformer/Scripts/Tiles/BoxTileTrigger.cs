using System;
using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  // TODO REVIEW memory allocation

  /// <summary>
  /// Base class for a Square trigger (BoxCollider2D)
  /// </summary>
  [RequireComponent (typeof (BoxCollider2D))]
  [RequireComponent (typeof (Rigidbody2D))]
  public class BoxTileTrigger : Physhic2DMonoBehaviour {
    /// <summary>
    /// cached BoxCollider2D component
    /// </summary>
    [HideInInspector]
    public BoxCollider2D body;
    /// <summary>
    /// List of character in the Tile
    /// </summary>
    [HideInInspector]
    public Character[] characters;
    /// <summary>
    /// How many character are in the Tile
    /// </summary>
    [HideInInspector]
    public int charCount;

    /// <summary>
    /// Force BoxCollider2D to be trigger using: Utils.KinematicTrigger and
    /// initialize character list
    /// </summary>
    override public void Start() {
      base.Start();
      body = GetComponent<BoxCollider2D>();
      Utils.KinematicTrigger(gameObject);

      if (characters == null) {
        characters = new Character[4];
        charCount = 0;
      }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Set BoxCollider2D to be trigger using: Utils.KinematicTrigger
    /// </summary>
    public virtual void Reset() {
      Utils.KinematicTrigger(gameObject);
    }
#endif

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
        CharacterEnter(h.owner.character);
      }
    }
    /// <summary>
    /// if Hitbox with EnterAreas leave -> CharacterExit
    /// </summary>
    public virtual void OnTriggerExit2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        CharacterExit(h.owner.character);
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
