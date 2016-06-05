using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
	public interface IUpdateEntity {
    void ManagedUpdate(float delta);
  }
}
