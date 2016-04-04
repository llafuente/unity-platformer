using System;
using UnityEngine;


public class Enemy: AliveEntity
{
	override public void Die() {
		UpdateManager.enemies.Remove (this);
		base.Die();
	}
}
