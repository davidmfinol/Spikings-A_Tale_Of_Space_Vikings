using UnityEngine;
using System.Collections;

public class HelpScript : MonoBehaviour {
	public int script = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider collider) {
		PlayerScript player = collider.gameObject.GetComponent<PlayerScript>();
		if (player != null) {
			player.script = script;
		}
	}
}
