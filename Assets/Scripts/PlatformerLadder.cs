using UnityEngine;
using System.Collections;

public class PlatformerLadder : MonoBehaviour {

	public void enableLadder(Collider2D o) {
		PlateformerPlayer p = o.GetComponent<PlateformerPlayer>();
		if (p) {
			Collider2D col = GetComponent<Collider2D>();
			p.EnterLadderArea(col.bounds);
		}
	}
	public void disableLadder(Collider2D o) {
		PlateformerPlayer p = o.GetComponent<PlateformerPlayer>();
		if (p) {
			Collider2D col = GetComponent<Collider2D>();
			p.ExitLadderArea(col.bounds);
		}
	}

	void OnTriggerEnter2D(Collider2D o) {
		enableLadder (o);
	}

	void OnTriggerStay2D(Collider2D o) {
		enableLadder (o);
	}

	public void OnTriggerExit2D(Collider2D o) {
		disableLadder (o);

	}
}
