using UnityEngine;
using System.Collections;

public class ItemScript : TileScript {
	public int power = 0;
	public int health = 0;
	public bool activated = false;
	
	override protected void Start() {
		base.Start();
	}
}
