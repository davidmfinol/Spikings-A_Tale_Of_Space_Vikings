using UnityEngine;
using System.Collections;

public class TileScript : MonoBehaviour {

	virtual protected void Start() {
		Vector3 pos = transform.position;
		float sortValue = GameManager.Instance.MapData.height-((int)pos.z) / 128 + pos.y;
		pos.y = sortValue;
		transform.position = pos;
		if(GetComponent<Collider>() != null && GetComponent<Collider>() is BoxCollider) {
			pos = ((BoxCollider)GetComponent<Collider>()).center;
			pos.z += sortValue;
			((BoxCollider)GetComponent<Collider>()).center = pos;
		}
	}
}