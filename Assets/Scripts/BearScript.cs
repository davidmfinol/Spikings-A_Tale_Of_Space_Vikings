﻿using UnityEngine;
using System.Collections;

public class BearScript : CharacterScript {
	
	override protected void Start () {
		base.Start();
		team = (int) TEAMS.ENEMY;
	}
	// Update is called once per frame
	void Update () {
		spawnHitBox(team);
	}
}
