using UnityEngine;
using System.Collections;

public class SmoothFollow2D : MonoBehaviour {
	
	public Transform target;
	public float smoothTime = 0.3f;
	private Vector3 velocity;
	
	void Update () {
		Vector3 tmp = transform.position;
		tmp.x = Mathf.SmoothDamp(transform.position.x, target.position.x, ref velocity.x, smoothTime);
		tmp.z = Mathf.SmoothDamp(transform.position.z, target.position.z, ref velocity.z, smoothTime);
		transform.position = tmp;
	}
}
