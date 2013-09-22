using UnityEngine;
using System.Collections;

public class HitboxScript : MonoBehaviour {
	
	public float delay = 0.5f;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Destroy (gameObject, delay);
	
	}
	
	private void OnTriggerEnter(Collider collider)
	{
		print ("collision.collider.gameobject is " + collider.gameObject);
		if(collider.gameObject.GetComponent<BearScript>() != null) {
			Destroy(collider.gameObject);	
		}	
	}
}
