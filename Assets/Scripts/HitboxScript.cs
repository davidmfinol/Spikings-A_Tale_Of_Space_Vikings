using UnityEngine;
using System.Collections;

public enum TEAMS : int {
	PLAYER = 0,
	ENEMY = 1
}

public class HitboxScript : MonoBehaviour {
	
	public float delay = 0.5f;
	public int team = 0;
	public int damage = 10;
	public bool smash = false;
	public bool isThrownHammer = false;
	public bool isSpin = false;
	
	public AudioClip impact_sound;
	public AudioClip impact_sound2;
	public AudioClip impact_sound3;
	
	public AudioClip roxhurt_sound;
	
	public AudioSource hitbox_audio;
	
	void Start () {
		if(isThrownHammer)
			return;
		
		Destroy(gameObject, delay);
		Destroy(transform.parent.gameObject, delay);
		
		//if(isSpin)
		//	GetComponent<tk2dSpriteAnimator>().Play("Spin");
	}
	
	private void OnTriggerEnter(Collider collider) {
		//print ("collision.collider.gameobject is " + collider.gameObject);
		GameObject gameObject = collider.gameObject;
		EnemyNPCScript enemy = gameObject.GetComponent<EnemyNPCScript>();
		PlayerScript player = gameObject.GetComponent<PlayerScript>();
		RockScript rock = gameObject.GetComponent<RockScript>();
		if(team == (int) TEAMS.PLAYER && enemy != null) {
			//this sound doesn't play
			audio.PlayOneShot(impact_sound);
			enemy.takeHit(this);
		} else if (team == (int) TEAMS.ENEMY && player != null) {
			audio.PlayOneShot (roxhurt_sound);
			Debug.Log("um");
			player.takeHit(this);
		} else if (smash && team == (int) TEAMS.PLAYER && rock != null) {
			rock.Smash();
		}
	}
}
