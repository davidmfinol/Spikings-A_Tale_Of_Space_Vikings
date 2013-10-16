﻿using UnityEngine;
using System.Collections;

public class PlayerScript : CharacterScript {
	public int speed = 1000;
	public int script = 0;
	
	override protected void Start () {
		base.Start();
		team = (int) TEAMS.PLAYER;
		powers = 0;
	}
	
	// Update is called once per frame
	override protected void Update () {
		base.Update();
		if (Input.GetButtonDown("Fire1") && powers % 2 == 1) {
			attack();
		}
		float x = Input.GetAxis("Horizontal") * speed;
		float z = Input.GetAxis("Vertical") * speed;
		move(x, z);
	}
	
	override protected void OnDeath () {
		currentHealth = maxHealth;
		transform.position = GameManager.Instance.startPoint.position;
	}
	
	private void OnControllerColliderHit(ControllerColliderHit hit) {
		GameObject gameObject = hit.collider.gameObject;
		if(gameObject.CompareTag("Platform")) {
			if (Input.GetButton("Jump")) {
				transform.position = transform.position + getMoveVector();
			} else if (Input.GetButton("Fire2")) {
				Vector3 position = gameObject.transform.position + getMoveVector();
				if (checkDownCollision(position, 1 << 11)) {
					gameObject.transform.position = position;
				}
			}
		} else if (gameObject.CompareTag("Cliff")) {
			Vector3 moveVector = getMoveVector();
			Vector3 hitVector = checkAcrossCollision(gameObject.transform.position, moveVector, 1 << 14);
			if (hitVector.Equals(hit.transform.position)) {
				Vector3 collision = gameObject.transform.position;
				if (direction == (int) DIRECTIONS.SOUTH) {
					collision = checkCliffCollision(collision + moveVector);
				}
				if (collision != Vector3.one && checkAcrossCollision(collision, moveVector, 1 << 12) == Vector3.one) {
					transform.position = collision + moveVector - new Vector3(0, 0, -75);
					//controller.Move(moveVector);
				}
			}
		} else if (gameObject.CompareTag("Item")) {
			powers += gameObject.GetComponent<ItemScript>().power;
			Destroy(gameObject);
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
		if (Physics.Raycast(position, Vector3.down, 8, layerMask)) {
			return true;
		}
		return false;
	}
	
	private Vector3 checkAcrossCollision(Vector3 position, Vector3 direction, int layerMask) {
		RaycastHit hit;
		if (Physics.Raycast(position, direction, out hit, 90, layerMask)) {
			return hit.transform.position;
		}
		return Vector3.one;
	}
	
	private Vector3 checkCliffCollision(Vector3 position) {
		RaycastHit hit;
		if (Physics.Raycast(position, Vector3.back, out hit, Mathf.Infinity, 1 << 13)) {
			return hit.transform.position;
		}
		return Vector3.one;
	}
	
	private void OnGUI()
	{
		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.white;
		style.fontSize = 30;
		if (script == 0) {
			GUI.TextField(new Rect(10, 70, 250, 20), "Move with WASD or Arrow Keys.", style);
		} else if (script == 1) {
			GUI.TextField(new Rect(10, 70, 250, 20), "If I had my hammer I could smash these", style);
			GUI.TextField(new Rect(10, 110, 250, 20), "I think my hammer flew to the east", style);
		} else if (script == 2) {
			GUI.TextField(new Rect(10, 70, 250, 20), "Attack with your hammer Q or F.", style);
			GUI.TextField(new Rect(10, 110, 250, 20), "You can smash rocks with your hammer", style);
		} else if (script == 3) {
			GUI.TextField(new Rect(10, 70, 250, 20), "E to push platform on dirt.", style);
			GUI.TextField(new Rect(10, 110, 250, 20), "Space to jump on platform on dirt.", style);
		}
	}
}
