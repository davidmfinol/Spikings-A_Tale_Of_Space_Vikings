using UnityEngine;
using System.Collections;

public class PlatformScript : TileScript {
	
	override protected void Start () {
		Vector3 pos = transform.position;
		float sortValue = GameManager.Instance.MapData.height-((int)pos.z) / 128;
		pos.y = sortValue;
		transform.position = pos;
		if(GetComponent<Collider>() != null && GetComponent<Collider>() is BoxCollider) {
			pos = ((BoxCollider)GetComponent<Collider>()).center;
			pos.z += sortValue;
			((BoxCollider)GetComponent<Collider>()).center = pos;
		}
	}

	void Update() {
		Vector3 pos = transform.position;
		float sortValue = GameManager.Instance.MapData.height-((int)pos.z) / 128;
		pos.y = sortValue;
		transform.position = pos;
		if(GetComponent<Collider>() != null && GetComponent<Collider>() is BoxCollider) {
			pos = ((BoxCollider)GetComponent<Collider>()).center;
			pos.z = sortValue;
			((BoxCollider)GetComponent<Collider>()).center = pos;
		}
	}
}
