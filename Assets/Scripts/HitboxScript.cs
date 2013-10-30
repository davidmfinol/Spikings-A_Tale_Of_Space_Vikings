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
	public bool autoSelfDestruct = true;
	
	void Start () {
		if(!autoSelfDestruct)
			return;
		
		Destroy(gameObject, delay);
		Destroy(transform.parent.gameObject, delay);
	}
	
	private void OnTriggerEnter(Collider collider) {
		//print ("collision.collider.gameobject is " + collider.gameObject);
		GameObject gameObject = collider.gameObject;
		EnemyNPCScript enemy = gameObject.GetComponent<EnemyNPCScript>();
		PlayerScript player = gameObject.GetComponent<PlayerScript>();
		RockScript rock = gameObject.GetComponent<RockScript>();
		if(team == (int) TEAMS.PLAYER && enemy != null) {
			enemy.takeHit(this);
		} else if (team == (int) TEAMS.ENEMY && player != null) {
			player.takeHit(this);
		} else if (smash && team == (int) TEAMS.PLAYER && rock != null) {
			rock.Smash();
		}
	}
}
