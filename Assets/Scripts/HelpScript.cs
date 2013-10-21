using UnityEngine;
using System.Collections;

public class HelpScript : MonoBehaviour {
	public string message;
	
	void OnTriggerEnter(Collider collider) {
		PlayerScript player = collider.gameObject.GetComponent<PlayerScript>();
		if (player != null) {
			if( message != null && message != "")
				GameManager.Instance.Hud.showThought(message);
			else
				GameManager.Instance.Hud.hideThought();
		}
	}
}
