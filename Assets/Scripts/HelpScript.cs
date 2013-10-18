using UnityEngine;
using System.Collections;

public class HelpScript : MonoBehaviour {
	public int script = 0;
	
	void OnTriggerEnter(Collider collider) {
		PlayerScript player = collider.gameObject.GetComponent<PlayerScript>();
		if (player != null) {
			if (script == 0) {
				GameManager.Instance.Hud.showThought("Move with WASD or Arrow Keys.");
			} else if (script == 1) {
				GameManager.Instance.Hud.showThought("If I had my hammer I could smash these.");
				//I think my hammer flew to the east.
			} else if (script == 2) {
				GameManager.Instance.Hud.showThought("Attack with your hammer Q or F.");
				//You can smash rocks with your hammer.
			} else if (script == 3) {
				GameManager.Instance.Hud.showThought("E to push platform on dirt.");
				//Space to jump on platform on dirt.
			}
			else
				GameManager.Instance.Hud.hideThought();
		}
	}
}
