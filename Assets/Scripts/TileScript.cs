using UnityEngine;
using System.Collections;

public class TileScript : MonoBehaviour {

	virtual protected void Start() {
		Vector3 pos = transform.position;
		pos.y = GameManager.Instance.MapData.height-((int)pos.z) / 128 + pos.y;
		transform.position = pos;
		if(collider != null && collider is BoxCollider) {
			pos = ((BoxCollider)collider).center;
			pos.z = (GameManager.Instance.MapData.height-((int)pos.z) / 128 + pos.y);
			((BoxCollider)collider).center = pos;
		}
	}
}