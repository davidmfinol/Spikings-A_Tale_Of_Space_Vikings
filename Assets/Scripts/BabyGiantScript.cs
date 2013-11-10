using UnityEngine;
using System.Collections;

public class BabyGiantScript : EnemyNPCScript {
	
	private bool deadFromThrow = false;
	public override void takeHit (HitboxScript hit)
	{
		if(hit.isThrownHammer)
			deadFromThrow = true;
		
		if(!isDead)
			OnDeath();
	}
	
	override protected void OnDeath () {
		isDead = true;
		
		if(deadFromThrow) 
			anima = (int) (ANIMATIONS.HIT);
		else
			anima = (int) (ANIMATIONS.DIE);
		
		playAnimation();
		
		if (Random.Range(0, 4) == 0) {
			Instantiate(meadPrefab, transform.position, meadPrefab.transform.rotation);
		}
		controller.enabled = false;
		timeSinceLastPath = 0;
		//Destroy(gameObject, 0.95f);
	}
}
