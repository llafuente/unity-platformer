using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
	public class PlatformerLadder : MonoBehaviour {
		public void EnableLadder(Collider2D o) {
			PlateformerPlayer p = o.GetComponent<PlateformerPlayer>();
			if (p) {
				Collider2D col = GetComponent<Collider2D>();
				p.EnterLadderArea(col.bounds);
			}
		}

		public void DisableLadder(Collider2D o) {
			PlateformerPlayer p = o.GetComponent<PlateformerPlayer>();
			if (p) {
				Collider2D col = GetComponent<Collider2D>();
				p.ExitLadderArea(col.bounds);
			}
		}

		void OnTriggerEnter2D(Collider2D o) {
			EnableLadder (o);
		}

		void OnTriggerStay2D(Collider2D o) {
			EnableLadder (o);
		}

		public void OnTriggerExit2D(Collider2D o) {
			DisableLadder (o);
		}
	}
}
