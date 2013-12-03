using UnityEngine;
using System.Collections;

public class ItemScript : TileScript {
	public int power = 0;
	public int health = 0;
	public bool activated = false;
	tk2dSpriteAnimator itemAnim;
	
	override protected void Start() {
		base.Start();
		itemAnim = GetComponent<tk2dSpriteAnimator>();
	}

	void Update() {
		if(power == 0 && health == 0) {
			if(GameManager.Instance.partsCollected == 0) 
				itemAnim.Play("Part2");
			else if(GameManager.Instance.partsCollected == 1) 
				itemAnim.Play("Part1");
			else 
				itemAnim.Play("Part3");
		}
	}
}
