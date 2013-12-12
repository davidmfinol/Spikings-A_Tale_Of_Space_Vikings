using UnityEngine;
using System.Collections;

public class BossScript : EnemyNPCScript {
	void OnDestroy() {
		GameManager.Instance.Hud.showThought("That's the end of that!", GetInstanceID());
		GameManager.Instance.Hud.titleCard.gameObject.SetActive(true);
	}

	protected override void despawn() {

	}
}
