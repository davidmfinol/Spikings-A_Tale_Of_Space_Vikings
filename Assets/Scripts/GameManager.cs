﻿using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	public Transform spawnPoint;
	public Transform playerPrefab;
	public Transform hudPrefab;
	
	private tk2dTileMap mapData;
	private PlayerScript player;
	private HUDController hud;
	
	private static GameManager _instance;

	void Start () {
		mapData = (tk2dTileMap) FindObjectOfType(typeof(tk2dTileMap));
		player = ((Transform)Instantiate(playerPrefab, spawnPoint.position, playerPrefab.rotation)).GetComponent<PlayerScript>();
		Camera.main.GetComponent<SmoothFollow2D>().target = player.transform;
		hud = ((Transform)Instantiate(hudPrefab, hudPrefab.position, hudPrefab.rotation)).GetComponent<HUDController>();
		hud.hideThought();
		_instance = this;
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
	public HUDController Hud {
		get {
			return hud;
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
