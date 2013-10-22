using UnityEngine;
using System.Collections;

public class PlatformScript : TileScript {
	
	override protected void Start () {
		Vector3 pos = transform.position;
		float sortValue = GameManager.Instance.MapData.height-((int)pos.z) / 128 + pos.y - 1;
		pos.y = sortValue;
		transform.position = pos;
		if(collider != null && collider is BoxCollider) {
			pos = ((BoxCollider)collider).center;
			pos.z += sortValue;
			((BoxCollider)collider).center = pos;
		}
	}
}
