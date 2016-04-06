using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
	public interface UpdateEntity {
    void ManagedUpdate(float delta);
  }
}
