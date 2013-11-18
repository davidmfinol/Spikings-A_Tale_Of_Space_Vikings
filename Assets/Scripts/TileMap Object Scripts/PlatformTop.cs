using UnityEngine;
using System.Collections;

public class PlatformTop : TreeTop {

	Transform bottomPlatform;

	override protected void Start() {
		base.Start();
		RaycastHit hit;
		Physics.Raycast(transform.position + new Vector3(0, 128, -128), Vector3.down, out hit, Mathf.Infinity, 1 << 15);
		bottomPlatform = hit.collider.gameObject.transform;
	}

	void LateUpdate() {
		Vector3 temp = bottomPlatform.position;
		temp.z = bottomPlatform.position.z + 128;
		transform.position = temp;
	}
}
