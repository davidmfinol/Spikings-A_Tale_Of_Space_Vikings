using UnityEngine;
using System.Collections;

public class NextLevel : MonoBehaviour {
	
	
	void OnTriggerEnter (Collider collider) {
		PlayerScript player = gameObject.GetComponent<PlayerScript>();
		if(player != null)
			Application.LoadLevel("EnemyTest");
	}
}
