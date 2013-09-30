using UnityEngine;
using System.Collections;

public enum TEAMS : int {
	PLAYER = 0,
	ENEMY = 1
}

public class HitboxScript : MonoBehaviour {
	
	public float delay = 0.5f;
	public int team = 0;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Destroy(gameObject, delay);
		Destroy(transform.parent.gameObject, delay);
	}
	
	private void OnTriggerEnter(Collider collider)
	{
		print ("collision.collider.gameobject is " + collider.gameObject);
		if(team == (int) TEAMS.PLAYER && collider.gameObject.GetComponent<BearScript>() != null) {
			Destroy(collider.gameObject);
		} else if (team == (int) TEAMS.ENEMY && collider.gameObject.GetComponent<PlayerScript>() != null) {
			Destroy(collider.gameObject);
		} else if (team == (int) TEAMS.PLAYER && collider.gameObject.GetComponent<RockScript>() != null) {
			collider.gameObject.GetComponent<RockScript>().Smash();
		}
	}
}
