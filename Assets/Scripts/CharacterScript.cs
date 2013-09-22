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
	WALK = 1
}

public class CharacterScript : MonoBehaviour {
	public int speed = 1000;
	public GameObject hitBox;

	private int direction = 0;
	private int anima = 0;
	
	private CharacterController controller;
	private tk2dSpriteAnimator anim;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
		anim = GetComponent<tk2dSpriteAnimator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1")) {
			Vector3 pos = transform.position;
			pos.x += 167;
			Instantiate(hitBox, pos, Quaternion.identity);
		}
		float x = Input.GetAxis("Horizontal") * speed;
		float z = Input.GetAxis("Vertical") * speed;
		//Debug.Log(x + " " + z);
		Vector3 movement = new Vector3(x, 0, z);
		processInput(x, z);
		playAnimation();
		movement *= Time.deltaTime;
		controller.Move(movement);
	}
	
	private void playAnimation() {
		if (anima == (int) ANIMATIONS.IDLE) {
			if (!anim.IsPlaying("idle" + direction)) {
				anim.Play("idle" + direction);
			}
		} else if (anima == (int) ANIMATIONS.WALK) {
			
		}
	}
	
	private void processInput(float x, float z) {
		//bool xIsZero = isZero(x);
		//bool zIsZero = isZero(z);
		bool xIsZero = x == 0;
		bool zIsZero = z == 0;
		if (xIsZero && zIsZero) {
			anima = 0;
		} else if (xIsZero) {
			if (z > 0) {
				direction = (int) DIRECTIONS.NORTH;
			} else {
				direction = (int) DIRECTIONS.SOUTH;
			}
			//anima = 1;
		} else if (zIsZero) {
			if (x > 0) {
				direction = (int) DIRECTIONS.EAST;
			} else {
				direction = (int) DIRECTIONS.WEST;
			}
			//anima = 1;
		} else {
			
		}
	}
	
	/*private bool isZero(float num) {
		if (num > 0.01) {
			return false;
		} else if (num < -0.01) {
			return false;
		} else {
			return true;
		}
	}*/
}
