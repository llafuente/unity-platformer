using UnityEngine;
using UnityEngine.UI;

namespace UnityPlatformer {
  public enum DamageTypes {
    None =       0,
    Default =    1,
    Physical =   1 << 2,
    Magical =    1 << 3,
    Fire =       1 << 4,
    Water =      1 << 5,
    Electrical = 1 << 6,
    Poison =     1 << 7,
    Shadow =     1 << 8
    // Choose your pain!
  };

  public class DamageType : MonoBehaviour {

    [Comment("Damage amount")]
    public int amount = 1;
    public DamageTypes type = DamageTypes.Default;
    public Health causer;

    [HideInInspector]
    // TODO direction will be calc, someday :)
    public Vector3 direction;

    void Start() {
      if (causer == null) {
        Debug.LogWarning("causer cannot be null", this);
      }
    }
  }
}
