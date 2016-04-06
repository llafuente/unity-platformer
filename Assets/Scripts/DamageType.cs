using UnityEngine;
using UnityEngine.UI;
namespace UnityPlatformer {
  [RequireComponent (typeof (Rigidbody2D))]
  public class DamageType : MonoBehaviour {
    public enum Type {
      DEFAULT,
      PHYSICAL,
      MAGICAL,
      FIRE,
      WATER,
      ELECTRICAL,
      POISON,
      SHADOW
    };

    [Comment("Damage amount")]
    public int amount = 1;
    public Type type = Type.DEFAULT;
    // TODO Causer / Owner

    [HideInInspector]
    // direction will be calc
    public Vector3 direction;
  }
}
