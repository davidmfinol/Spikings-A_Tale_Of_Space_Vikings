using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	private static GameManager _instance;
	
	private tk2dTileMap mapData;
	private PlayerScript player;

	void Start () {
		mapData = (tk2dTileMap) FindObjectOfType(typeof(tk2dTileMap));
		player = FindObjectOfType(typeof(PlayerScript)) as PlayerScript;
	}
	
	public tk2dTileMap MapData {
		get {
			return mapData;
		}
	}
	public PlayerScript Player {
		get {
			return player;
		}
	}
	
	public static GameManager Instance {
		get {
            if (!_instance) {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;
                if (!_instance)
                    Debug.LogError("GameManager missing from Scene.");
            }
            return _instance;
        }
	}
}
