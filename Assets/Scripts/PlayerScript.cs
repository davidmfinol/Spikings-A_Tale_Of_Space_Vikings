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
			attack ();
		}
		float x = Input.GetAxis("Horizontal") * speed;
		float z = Input.GetAxis("Vertical") * speed;
		move(x, z);
	}
	
	private void OnControllerColliderHit(ControllerColliderHit hit) {
		PlatformScript platform = hit.collider.gameObject.GetComponent<PlatformScript>();
		if(platform != null) {
			Vector3 position = platform.transform.position + getPushVector(platform.transform.position);
			if (checkMud(position)) {
				platform.transform.position = position;
			}
		}
	}
	
	private Vector3 getPushVector(Vector3 platformPosition) {
		Vector3 pushVector = new Vector3(0, 0, 0);
		platformPosition = transform.position - platformPosition;
		float x = Mathf.Abs(platformPosition.x);
		float z = Mathf.Abs(platformPosition.z);
		if (x > z) {
			if (platformPosition.x < 0) {
				pushVector = new Vector3(128, 0, 0);
			} else if (platformPosition.x > 0) {
				pushVector = new Vector3(-128, 0, 0);
			}
		} else {
			if (platformPosition.z < 0) {
				pushVector = new Vector3(0, 0, 128);
			} else if (platformPosition.z > 0) {
				pushVector = new Vector3(0, 0, -128);
			}
		}
		return pushVector;
	}
	
	private bool checkMud(Vector3 position) {
		int layerMask = 1 << 10;
		if (Physics.Raycast(position, Vector3.down, 6, layerMask)) {
			return true;
		}
		return false;
	}
}
