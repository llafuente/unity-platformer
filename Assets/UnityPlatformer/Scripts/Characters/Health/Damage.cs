using UnityEngine;
using UnityEngine.UI;

namespace UnityPlatformer {
  public class Damage : MonoBehaviour {

    [Comment("Damage amount")]
    public int amount = 1;
    public DamageType type = DamageType.Default;
    public CharacterHealth causer;

    // TODO direction will be calc, someday :)
    internal Vector3 direction;

    void Start() {
      if (causer == null) {
        Debug.LogWarning("causer cannot be null", this);
      }
    }
  }
}
