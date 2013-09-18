using UnityEngine;
using System.Collections;

public class DelayedDestroy : MonoBehaviour {
	
	public float delay = 0.5f;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Destroy (gameObject, delay);
	
	}
	
	
	
}
