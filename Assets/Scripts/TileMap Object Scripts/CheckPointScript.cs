﻿using UnityEngine;
using System.Collections;

public class CheckPointScript : TileScript {
	
	public bool isTeleport = false;
	
	protected tk2dSprite sprite;
	protected tk2dSpriteAnimator anim;
	
	override protected void Start() {
		base.Start();
		sprite = GetComponent<tk2dSprite>();
		anim = GetComponent<tk2dSpriteAnimator>();
	}
	
	void OnTriggerEnter(Collider collider) {
		PlayerScript player = collider.gameObject.GetComponent<PlayerScript>();
		if (player != null) {
			CheckPointScript prevCheck = GameManager.Instance.spawnPoint.GetComponent<CheckPointScript>();
			if(prevCheck != null)
				prevCheck.anim.Play("SpawnPointStatic");
			GameManager.Instance.spawnPoint = transform;
			anim.Play("SpawnPointAnimation");
			if(isTeleport)
				player.transform.position = GameManager.Instance.centerPoint.position;
		}
	}
}
