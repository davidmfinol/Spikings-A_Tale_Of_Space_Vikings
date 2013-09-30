using UnityEngine;
using System.Collections;

public class PlayerScript : CharacterScript {
	public int speed = 1000;
	
	private Vector3 east = new Vector3(-30, -75, 0);
	private Vector3 west = new Vector3(30, -75, 0);
	private Vector3 south = new Vector3(0, -75, 0);
	private Vector3 north = new Vector3(0, -75, 0);
	
	override protected void Start () {
		base.Start();
		team = (int) TEAMS.PLAYER;
	}
	
	// Update is called once per frame
	override protected void Update () {
		base.Update();
		if (Input.GetButtonDown("Fire1")) {
			spawnHitBox(team);
		}
		float x = Input.GetAxis("Horizontal") * speed;
		float z = Input.GetAxis("Vertical") * speed;
		//Debug.Log(x + " " + z);
		Vector3 movement = new Vector3(x, 0/*TODO: -speed */, z);
		processInput(x, z);
		changeCollider();
		playAnimation();
		movement *= Time.deltaTime;
		controller.Move(movement);
	}
	
	private void changeCollider() {
		if (direction == (int) DIRECTIONS.EAST) {
			controller.center = east;
//			controller.transform.localPosition = east;
		} else if (direction == (int) DIRECTIONS.NORTH) {
			controller.center = north;
//			controller.transform.localPosition = north;
		} else if (direction == (int) DIRECTIONS.WEST) {
			controller.center = west;
//			controller.transform.localPosition = west;
		} else if (direction == (int) DIRECTIONS.SOUTH) {
			controller.center = south;
//			controller.transform.localPosition = south;
		}
	}
}
