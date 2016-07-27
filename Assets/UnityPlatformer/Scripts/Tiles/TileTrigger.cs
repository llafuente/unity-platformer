using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  [RequireComponent (typeof (BoxCollider2D))]
  public class TileTrigger : MonoBehaviour {
    // cache
    internal BoxCollider2D body;
    internal Character[] characters;
    internal int charCount;

    virtual public void Start() {
      body = GetComponent<BoxCollider2D>();
      if (characters == null) {
        characters = new Character[5];
        charCount = 0;
      }
    }

    virtual public void OnDestroy() {
      while(charCount != 0) {
        CharacterExit(characters[0]);
      }
    }

    virtual public void CharacterEnter(Character p) {
      if (p == null) return;
      characters[charCount++] = p;
    }

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

    public virtual void OnTriggerEnter2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        CharacterEnter(h.owner.GetComponent<Character>());
      }
    }

    public virtual void OnTriggerExit2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        CharacterExit(h.owner.GetComponent<Character>());
      }
    }
  }
}
