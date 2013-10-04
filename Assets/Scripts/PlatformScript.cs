using UnityEngine;
using System.Collections;

public class PlatformScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private void OnTriggerEnter(Collider collider) {
		if(collider.gameObject.GetComponent<PlayerScript>() != null) {
			PlayerScript player = collider.gameObject.GetComponent<PlayerScript>();
			Vector3 position = transform.position + getPushVector(player.transform.position);
			if (checkMud(position)) {
				transform.position = position;
			}
		}
	}
	
	private Vector3 getPushVector(Vector3 playerPosition) {
		Vector3 pushVector = new Vector3(0, 0, 0);
		playerPosition -= transform.position;
		float x = Mathf.Abs(playerPosition.x);
		float z = Mathf.Abs(playerPosition.z);
		if (x > z) {
			if (playerPosition.x < 0) {
				pushVector = new Vector3(128, 0, 0);
			} else if (playerPosition.x > 0) {
				pushVector = new Vector3(-128, 0, 0);
			}
		} else {
			if (playerPosition.z < 0) {
				pushVector = new Vector3(0, 0, 128);
			} else if (playerPosition.z > 0) {
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
