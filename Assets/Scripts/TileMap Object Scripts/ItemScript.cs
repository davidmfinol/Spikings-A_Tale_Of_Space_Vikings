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
				itemAnim.Play("Part1");
			else if(GameManager.Instance.partsCollected == 1) 
				itemAnim.Play("Part2");
			else 
				itemAnim.Play("Part3");
		}
	}

	void OnTriggerStay(Collider other)
	{
		PlayerScript player = other.GetComponent<PlayerScript>();
		if(player == null)
			return;

		player.powers += power;
		player.currentHealth += health;
		//if it's Mead
		if(health > 0) {
			player.audio.PlayOneShot(player.mead_sound);
		}
		if(player.currentHealth > player.maxHealth)
			player.currentHealth = player.maxHealth;
		if(!activated && health == 0 && power == 0)
		{
			player.audio.PlayOneShot (player.pickup_sound);
			GameManager.Instance.partsCollected++;
			activated = true;
		}
		Destroy(gameObject);
	}
}
