using UnityEngine;
using System.Collections;

public class HelpScript : MonoBehaviour {
	public string message;
	private bool alreadyTriggered = false;
	
	void OnTriggerEnter(Collider collider) {
		PlayerScript player = collider.gameObject.GetComponent<PlayerScript>();
		if (player != null) {
			if( message != null && message != "" && !alreadyTriggered)
			{
				GameManager.Instance.Hud.showThought(message);
				alreadyTriggered = true;
			}
			else
				GameManager.Instance.Hud.hideThought();
		}
	}
}
