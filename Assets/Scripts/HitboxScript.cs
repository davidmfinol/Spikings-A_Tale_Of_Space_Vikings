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
	
	void Start () {
		Destroy(gameObject, delay);
		Destroy(transform.parent.gameObject, delay);
	}
	
	private void OnTriggerEnter(Collider collider)
	{
		//print ("collision.collider.gameobject is " + collider.gameObject);
		if(team == (int) TEAMS.PLAYER && collider.gameObject.GetComponent<BearScript>() != null) {
			BearScript bear = collider.gameObject.GetComponent<BearScript>();
			collider.audio.PlayDelayed(0.2f);
			bear.currentHealth -= damage;
		} else if (team == (int) TEAMS.ENEMY && collider.gameObject.GetComponent<PlayerScript>() != null) {
			collider.gameObject.GetComponent<PlayerScript>().currentHealth -= damage;
		} else if (team == (int) TEAMS.PLAYER && collider.gameObject.GetComponent<RockScript>() != null) {
			collider.gameObject.GetComponent<RockScript>().Smash();
		}
	}
}
