using UnityEngine;
using System.Collections;

public class SkyboxScript : MonoBehaviour {

	public Transform target;
	public float scrollSpeed = 0.5F;

	void Update() {
		Vector2 vec = new Vector2 (-target.position.x * scrollSpeed, -target.position.z * scrollSpeed);
		GetComponent<Renderer>().material.SetTextureOffset("_MainTex", vec);
	}
}
