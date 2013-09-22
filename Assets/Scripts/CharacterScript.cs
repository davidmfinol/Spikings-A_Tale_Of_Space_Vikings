using UnityEngine;
using System.Collections;

public enum DIRECTIONS : int {
	EAST = 0,
	NORTH = 6,
	WEST = 4,
	SOUTH = 2
}

public enum ANIMATIONS : int {
	IDLE = 0,
	WALK = 1
}

public class CharacterScript : MonoBehaviour {
	public GameObject hitBox;

	protected int direction = (int) DIRECTIONS.EAST;
	protected int anima = (int) ANIMATIONS.IDLE;
	protected int team = (int) TEAMS.PLAYER;
	
	protected CharacterController controller;
	protected tk2dSpriteAnimator anim;

	// Use this for initialization
	virtual protected void Start () {
		controller = GetComponent<CharacterController>();
		anim = GetComponent<tk2dSpriteAnimator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	protected void spawnHitBox(int team) {
		Vector3 pos = Vector3.zero;
		pos.x += 167;
		GameObject box = (GameObject) Instantiate(hitBox, pos, Quaternion.identity);
		box.GetComponent<HitboxScript>().team = team;
		GameObject buffer = new GameObject();
		box.transform.parent = buffer.transform;
		buffer.transform.parent = transform;
		buffer.transform.position = transform.position;
		Vector3 rot = new Vector3(0, 360 * (direction / 8.0f), 0);
		buffer.transform.Rotate(rot);
	}
}
