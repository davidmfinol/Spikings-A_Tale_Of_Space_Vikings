using UnityEngine;
using System.Collections;

public class HelpScript : MonoBehaviour {
	public int script = 0;
	
	void OnTriggerEnter(Collider collider) {
		PlayerScript player = collider.gameObject.GetComponent<PlayerScript>();
		if (player != null) {
			player.script = script;
		}
	}
}
