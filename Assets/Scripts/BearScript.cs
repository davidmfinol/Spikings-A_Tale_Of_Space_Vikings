using UnityEngine;
using System.Collections;

public class BearScript : MonoBehaviour {
	public GameObject hitBox;
	
	//private CharacterController controller;
	private int direction = 0;
	
	// Use this for initialization
	void Start () {
		//controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		spawnHitBox();
	}
	
	private void spawnHitBox() {
		Vector3 pos = Vector3.zero;
		pos.x += 167;
		GameObject box = (GameObject) Instantiate(hitBox, pos, Quaternion.identity);
		box.GetComponent<HitboxScript>().team = (int) TEAMS.ENEMY;
		GameObject buffer = new GameObject();
		box.transform.parent = buffer.transform;
		buffer.transform.parent = transform;
		buffer.transform.position = transform.position;
		Vector3 rot = new Vector3(0, 360 * (direction / 8.0f), 0);
		buffer.transform.Rotate(rot);
	}
}
