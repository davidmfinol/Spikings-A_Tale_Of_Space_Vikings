using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	private static GameManager _instance;
	
	private tk2dTileMap mapData;

	void Start () {
		mapData = (tk2dTileMap) FindObjectOfType(typeof(tk2dTileMap));
	}
	
	void Update () {
	
	}
	
	public tk2dTileMap MapData {
		get {
			return mapData;
		}
	}
	
	public static GameManager Instance {
		get {
			return _instance != null ? _instance : (GameManager) FindObjectOfType(typeof(GameManager));
		}
	}
}
