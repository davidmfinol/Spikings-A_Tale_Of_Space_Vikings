using UnityEngine;
using System.Collections;

public enum DIRECTIONS : int {
	EAST = 0,
	NORTH = 2,
	WEST = 4,
	SOUTH = 6
}

public enum ANIMATIONS : int {
	IDLE = 0,
	WALK = 1,
	ATTACK = 2
}

public abstract class CharacterScript : MonoBehaviour {
	public static int PointCount = 0;
	
	public GameObject hitBox;

	protected int direction = (int) DIRECTIONS.EAST;
	protected int anima = (int) ANIMATIONS.IDLE;
	protected int team = (int) TEAMS.PLAYER;
	
	protected CharacterController controller;
	protected tk2dSprite sprite;
	protected tk2dSpriteAnimator anim;
	
	private float initialY;
	private bool isAttacking;
	
	// Use this for initialization
	virtual protected void Start () {
		controller = GetComponent<CharacterController>();
		sprite = GetComponent<tk2dSprite>();
		anim = GetComponent<tk2dSpriteAnimator>();
		initialY = transform.position.y;
		isAttacking = false;
	}
	
	// Update is called once per frame
	virtual protected void Update () {
		//TODO: MAKE THIS BASED OFF MAX Z VALUE FOR TILEMAP
		sprite.SortingOrder = 1200-((int)transform.position.z);
		Vector3 correction = transform.position;
		correction.y = initialY;
		transform.position = correction;
		if (isAttacking && !anim.IsPlaying("attack" + direction)) {
			isAttacking = false;
		}
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
		Vector3 rot = new Vector3(0, -360 * (direction / 8.0f), 0);
		buffer.transform.Rotate(rot);
	}
	
	protected void processInput(float x, float z) {
		bool xIsZero = x == 0;
		bool zIsZero = z == 0;
		if (xIsZero && zIsZero) {
			anima = 0;
		} else if (xIsZero) {
			checkZ(z);
		} else if (zIsZero) {
			checkX(x);
		} else {
			checkX(x);
		}
	}
	private void checkZ(float z) {
		if (z > 0) {
			direction = (int) DIRECTIONS.NORTH;
		} else {
			direction = (int) DIRECTIONS.SOUTH;
		}
		anima = 1;
	}
	private void checkX(float x) {
		if (x > 0) {
			direction = (int) DIRECTIONS.EAST;
		} else {
			direction = (int) DIRECTIONS.WEST;
		}
		anima = 1;
	}
	
	protected void playAnimation() {
		if (anima == (int) ANIMATIONS.IDLE) {
			if (!anim.IsPlaying("idle" + direction)) {
				anim.Play("idle" + direction);
			}
		} else if (anima == (int) ANIMATIONS.WALK) {
			if (!anim.IsPlaying("walk" + direction)) {
				anim.Play("walk" + direction);
			}
		} else if (anima == (int) ANIMATIONS.ATTACK) {
			if (!anim.IsPlaying("attack" + direction)) {
				isAttacking = true;
				anim.Play("attack" + direction);
			}
		}
	}
	
	protected void move(float x, float z) {
		if (!isAttacking) {
			processInput(x, z);
			playAnimation();
			Vector3 movement = new Vector3(x, 0/*TODO: -speed */, z);
			movement *= Time.deltaTime;
			controller.Move(movement);
		}
	}
	
	protected void attack() {
		anima = (int) ANIMATIONS.ATTACK;
		playAnimation();
		audio.Play();
		spawnHitBox(team);
	}
	
	void OnGUI()
	{
		GUI.TextField(new Rect(10, 10, 200, 20), "Points: " + PointCount);
	}
}
