using UnityEngine;
using System.Collections;

public class TreeTop : TileScript {
	
	override protected void Start () {
		Vector3 pos = transform.position;
		pos.y = GameManager.Instance.MapData.height-((int)pos.z) / 128 + pos.y + 1;
		transform.position = pos;
	}
}
