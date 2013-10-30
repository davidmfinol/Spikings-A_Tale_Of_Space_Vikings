using UnityEngine;
using System.Collections;

public class BabyGiantScript : EnemyNPCScript {
	
	private bool deadFromThrow = false;
	public override void takeHit (HitboxScript hit)
	{
		if(hit.isThrownHammer)
			deadFromThrow = true;
		
		OnDeath();
	}
	
	override protected void OnDeath () {
		base.OnDeath();
		if(!isDead && deadFromThrow) {
			anima = (int) (ANIMATIONS.HIT);
			playAnimation();
		}
	}
}
