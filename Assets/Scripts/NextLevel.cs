using UnityEngine;
using System.Collections;

public class NextLevel : MonoBehaviour {
	public string NextLevelName;
	
	void OnTriggerEnter (Collider collider) {
		PlayerScript player = collider.gameObject.GetComponent<PlayerScript>();
		if(player != null)
			Application.LoadLevel(NextLevelName);
	}
}
