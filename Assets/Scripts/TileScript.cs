using UnityEngine;
using System.Collections;

public class TileScript : MonoBehaviour {

	virtual protected void Start() {
		Vector3 pos = transform.position;
		pos.y = (GameManager.Instance.MapData.height*128-((int)transform.position.z)) / 128 + pos.y;
		transform.position = pos;
	}
}