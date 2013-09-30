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
}
