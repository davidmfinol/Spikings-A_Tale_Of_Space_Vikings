using UnityEngine;
using System.Collections;

public class BabyGiantScript : EnemyNPCScript {
	
	public override void takeHit (HitboxScript hit)
	{
		OnDeath();
	}
}
