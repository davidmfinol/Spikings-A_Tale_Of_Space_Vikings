﻿using UnityEngine;
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
			spawnHitBox(team);
		}
		float x = Input.GetAxis("Horizontal") * speed;
		float z = Input.GetAxis("Vertical") * speed;
		//Debug.Log(x + " " + z);
		Vector3 movement = new Vector3(x, 0/*TODO: -speed */, z);
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
