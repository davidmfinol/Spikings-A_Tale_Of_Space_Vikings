using UnityEngine;
using System.Collections;

public class PlayerScript : CharacterScript {
	public int speed = 1000;
	
	override protected void Start () {
		base.Start();
		team = (int) TEAMS.PLAYER;
	}
	
	// Update is called once per frame
	override protected void Update () {
		base.Update();
		if (Input.GetButtonDown("Fire1")) {
			attack();
		}
		float x = Input.GetAxis("Horizontal") * speed;
		float z = Input.GetAxis("Vertical") * speed;
		move(x, z);
	}
	
	private void OnControllerColliderHit(ControllerColliderHit hit) {
		GameObject gameObject = hit.collider.gameObject;
		if(gameObject.CompareTag("Platform")) {
			if (Input.GetButton("Jump")) {
				transform.position = transform.position + getMoveVector();
			} else if (Input.GetButton("Fire2")) {
				Vector3 position = gameObject.transform.position + getMoveVector();
				if (checkDownCollision(position, 1 << 10)) {
					gameObject.transform.position = position;
				}
			}
		} else if (gameObject.CompareTag("Cliff")) {
			Vector3 moveVector = getMoveVector();
			if (checkAcrossCollision(gameObject.transform.position, moveVector, 1 << 13)) {
				Debug.Log("cliff jumping");
				transform.position += moveVector;
				//controller.Move(moveVector);
			}
		}
	}
	
	private Vector3 getMoveVector() {
		if (direction == (int) DIRECTIONS.EAST) {
			return new Vector3(128, 0, 0);
		} else if (direction == (int) DIRECTIONS.NORTH) {
			return new Vector3(0, 0, 128);
		} else if (direction == (int) DIRECTIONS.WEST) {
			return new Vector3(-128, 0, 0);
		} else if (direction == (int) DIRECTIONS.SOUTH) {
			return new Vector3(0, 0, -128);
		}
		return new Vector3(0, 0, 0);
	}
	
	private bool checkDownCollision(Vector3 position, int layerMask) {
		if (Physics.Raycast(position, Vector3.down, 6, layerMask)) {
			return true;
		}
		return false;
	}
	
	private bool checkAcrossCollision(Vector3 position, Vector3 direction, int layerMask) {
		if (Physics.Raycast(position, direction, 64, layerMask)) {
			return true;
		}
		return false;
	}
}
