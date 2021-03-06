﻿using UnityEngine;
using System.Collections;

public class PlayerScript : CharacterScript {
	
	public int speed = 500;
	
	public GameObject hammerThrowPrefab;
	public GameObject shadowPrefab;
	
	public AudioClip mead_sound;
	public AudioClip death_sound;
	public AudioClip hurt_sound;

	public AudioClip pickup_sound; //for parts
	public AudioClip repair_sound;
	public AudioClip repair_sound2;

	private Vector3 platformOffset = new Vector3(0, 0, 128);
	
	override protected void Start () {
		base.Start();
		team = (int) TEAMS.PLAYER;
		powers = 0;
		combo = true;
	}
	
	// Update is called once per frame
	override protected void Update () {
		base.Update();
		if (Input.GetButtonDown("Fire1") && powers % 2 == 1) {
			attack();
		}
		else if (Input.GetButtonDown("Fire3") && powers % 2 == 1){
			StartCoroutine("hammerThrow");
		}
		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");
		Vector3 movement = new Vector3(x, 0, z);
		movement = movement.normalized * speed;
		move(movement.x, movement.z);
	}
	
	
	IEnumerator hammerThrow() {
		StartCoroutine("PlayNoInterruptAnimation", (int) ANIMATIONS.THROW);
		while(noInterrupt)
			yield return null;
		
		GameObject handPosition = new GameObject();
		handPosition.transform.parent = transform;
		handPosition.transform.position = transform.position + new Vector3(0, 0, 75);
		
		GameObject spawnedHammer = (GameObject) Instantiate(hammerThrowPrefab, handPosition.transform.position, hammerThrowPrefab.transform.rotation);
		HammerThrow hammer = spawnedHammer.GetComponent<HammerThrow>();
		
		hammer.parentOb = handPosition;
		hammer.direction = direction;
		hammer.throwDirection = getMoveVector().normalized;
		hammer.smashing = powers > 1;
		
		powers--;
		isDoneThrowing();
		StopCoroutine("hammerThrow");
	}
	
	
	override protected void OnDeath () {
		GetComponent<AudioSource>().PlayOneShot (death_sound);
		StartCoroutine("RespawnWithCamera");
	}

	IEnumerator RespawnWithCamera()
	{
		SmoothFollow2D cameraScript = Camera.main.GetComponent<SmoothFollow2D>();
		cameraScript.smoothTime = 1.0f;
		cameraScript.target = GameManager.Instance.spawnPoint;

		while ((cameraScript.transform.position - new Vector3(0, 0, cameraScript.zOffset) - GameManager.Instance.spawnPoint.position).magnitude > 256.0f)
		{
			//Debug.Log((cameraScript.transform.position - new Vector3(0, 0, cameraScript.zOffset) - GameManager.Instance.spawnPoint.position).magnitude);
			yield return null;
		}
		
		currentHealth = maxHealth;
		transform.position = GameManager.Instance.spawnPoint.position;

		yield return null;
		cameraScript.smoothTime = 0f;
		cameraScript.target = GameManager.Instance.Player.transform;
		StopCoroutine("RespawnWithCamera");
	}
	
	
	private void OnControllerColliderHit(ControllerColliderHit hit) {
		GameObject gameObject = hit.collider.gameObject;
		if(gameObject.CompareTag("Platform")) {
			InteractWithPlatform(gameObject);
		} else if (gameObject.CompareTag("Cliff Top") || gameObject.CompareTag("Cliff Side")) {
			ArrayList list = new ArrayList(2);
			list.Add(hit.collider.gameObject);
			list.Add(hit.transform.position);
			StartCoroutine("JumpCliff", list);
		} else if (gameObject.CompareTag("Item")) {
			/*
			ItemScript item = gameObject.GetComponent<ItemScript>();
			powers += item.power;
			currentHealth += item.health;
			//if it's Mead
			if(item.health > 0) {
				audio.PlayOneShot(mead_sound);
			}
			if(currentHealth > maxHealth)
				currentHealth = maxHealth;
			if(!item.activated && item.health == 0 && item.power == 0)
			{
				audio.PlayOneShot (pickup_sound);
				GameManager.Instance.partsCollected++;
				item.activated = true;
			}
			Destroy(gameObject);*/
		} else if(gameObject.CompareTag("Spaceship")) {
			tk2dSpriteAnimator shipAnim = gameObject.GetComponent<tk2dSpriteAnimator>();
			if(GameManager.Instance.partsCollected == 1 && !shipAnim.IsPlaying("1Part"))
			{
				GetComponent<AudioSource>().PlayOneShot (repair_sound);
				shipAnim.Play("1Part");
			}
			else if(GameManager.Instance.partsCollected == 2 && !shipAnim.IsPlaying("2Part"))
			{
				GetComponent<AudioSource>().PlayOneShot (repair_sound2);
				shipAnim.Play("2Part");
			}
			else if(GameManager.Instance.partsCollected > 2 && !shipAnim.IsPlaying("AllParts"))
			{
				GetComponent<AudioSource>().PlayOneShot (repair_sound);
				shipAnim.Play("AllParts");
			}
		}
		
	}
	
	
	private void InteractWithPlatform(GameObject platform) {
		if ( CanPushPlatform(platform) && !noInterrupt) {
			StartCoroutine("PushPlatform", platform);
		} else if (!noInterrupt) {
			StartCoroutine("JumpPlatform", platform);
		}
	}
	
	private bool CanPushPlatform(GameObject platform)
	{
		Vector3 endPosition = platform.transform.position;
		Vector3 displacement = getMoveVector();
		Vector3 endPositionY = endPosition;
		endPositionY.y = 0;
		Vector3 hitVector = checkAcrossCollision(endPositionY, displacement, 1 << 14);//check to see if pushing by a cliff
		
		bool overCliff = hitVector != Vector3.one && checkVector(hitVector, endPosition);
		bool intoCliff = hitVector != Vector3.one && !checkVector(hitVector, endPosition);
		
		if(overCliff)
			endPosition += Vector3.back * 128;
		endPosition += displacement;
		
		bool endOnMud = checkDownCollision(endPosition, 1 << 11);
		bool obstacleInWay = checkAcrossCollision(endPositionY, displacement, 1 << 12) != Vector3.one;
		
		return !intoCliff && endOnMud && !obstacleInWay;
	}
	
	IEnumerator PushPlatform(GameObject platform) {
		noInterrupt = true;
		
		// We need these variables (even though already calculated in CanPushPlatform)
		Vector3 displacement = getMoveVector();
		Vector3 endPositionY = platform.transform.position;
		endPositionY.y = 0;
		Vector3 hitVector = checkAcrossCollision(endPositionY, displacement, 1 << 14);//check to see if pushing by a cliff
		bool overCliff = hitVector != Vector3.one && checkVector(hitVector, platform.transform.position);
		
		// Start the pushing
		anima = (int) ANIMATIONS.PUSH;
		playAnimation();
		
		Vector3 origPos = transform.position;
		Vector3 origPlat = platform.transform.position;
		
		float distTraveled = 0;
		while (distTraveled < displacement.magnitude) {
			Vector3 movement = Time.deltaTime * displacement.normalized * speed;
			transform.position = transform.position + movement;
			platform.transform.position = platform.transform.position + movement;
			distTraveled += movement.magnitude;
			yield return null;
		}
		
		transform.position = origPos + displacement*0.9f;
		platform.transform.position = origPlat + displacement;
		
		
		if(overCliff) {
			anima = (int) ANIMATIONS.IDLE;
			playAnimation();
			displacement = Vector3.back * 128;
			distTraveled = 0;
			while (distTraveled < displacement.magnitude) {
				Vector3 movement = Time.deltaTime * displacement.normalized * speed;
				platform.transform.position = platform.transform.position + movement;
				distTraveled += movement.magnitude;
				yield return null;
			}
		}

		noInterrupt = false;
		StopCoroutine("PushPlatform");

		if(!overCliff && CanPushPlatform(platform) && (Input.GetAxis("Horizontal") > 0 && direction == (int)(DIRECTIONS.EAST) ||
		   Input.GetAxis("Horizontal") < 0 && direction == (int)(DIRECTIONS.WEST) ||
		   Input.GetAxis("Vertical") > 0 && direction == (int)(DIRECTIONS.NORTH) ||
		   Input.GetAxis("Vertical") < 0 && direction == (int)(DIRECTIONS.SOUTH)))
		{
			transform.position = origPos + displacement;
			StartCoroutine("PushPlatform", platform);
		}
	}
	
	IEnumerator JumpPlatform(GameObject platform) {
		noInterrupt = true; // this entire action cannot be interrupted
		sprite.SortingOrder = 1;
		
		// Start with the jump up
		anima = (int)(ANIMATIONS.JUMP);
		playAnimation();

		Vector3 targetPosition = platform.transform.position + platformOffset;
		targetPosition.y = 0;
		Vector3 moveVector = targetPosition - transform.position;
		float distTraveled = 0;
		while (distTraveled < moveVector.magnitude) {
			Vector3 movement = Time.deltaTime * moveVector.normalized * speed;
			transform.position = transform.position + movement;
			distTraveled += movement.magnitude;
			yield return null;
		}
		transform.position = targetPosition;

		StopCoroutine("JumpPlatform");
		StartCoroutine("StayPlatform", platform);
	}
	
	IEnumerator LandPlatform(GameObject platform) {
		noInterrupt = true; // this entire action cannot be interrupted
		sprite.SortingOrder = 1;
		
		// Start with the jump up
		anima = (int)(ANIMATIONS.FALL);
		playAnimation();
		
		Vector3 targetPosition = platform.transform.position + platformOffset;
		targetPosition.y = 0;
		Vector3 moveVector = targetPosition - transform.position;
		float distTraveled = 0;
		while (distTraveled < moveVector.magnitude) {
			Vector3 movement = Time.deltaTime * moveVector.normalized * speed;
			transform.position = transform.position + movement;
			distTraveled += movement.magnitude;
			yield return null;
		}
		transform.position = targetPosition;
		
		StopCoroutine("LandPlatform");
		StartCoroutine("StayPlatform", platform);
	}

	IEnumerator StayPlatform(GameObject platform) {
		sprite.SortingOrder = 1;
		isStationary = true;
		noInterrupt = false;

		Vector3 startCheckpoint = transform.position;
		bool shouldmove = false;
		while(!shouldmove) {
			float x = Input.GetAxis("Horizontal");
			float z = Input.GetAxis("Vertical");
			processInput(x, z);
			
			if(!isAttacking && !isBeingHit && !noInterrupt && !isDead)
			{
				anima = (int)(ANIMATIONS.IDLE);
				playAnimation();
			}
			
			Vector3 direction = getMoveVector();
			if (direction == new Vector3(0, 0, 128)) {
				direction = new Vector3(0, 0, 0);
			}
			Debug.DrawLine(startCheckpoint, startCheckpoint + getMoveVector());

			shouldmove = (x != 0 || z != 0) && !Physics.Raycast(startCheckpoint, direction.normalized, direction.magnitude, 1 << 12) && checkDownCollision(startCheckpoint + direction, 1 << 8);
			yield return null;
		}
		isStationary = false;
		noInterrupt = true;
		// Then move off the platform
		Vector3 moveVector = getMoveVector() - platformOffset;
		RaycastHit cliffHit;
		if(Physics.Raycast(startCheckpoint - platformOffset, moveVector + platformOffset, out cliffHit, (moveVector + platformOffset).magnitude, (1 << 13) | (1 << 14) ) ) {
			noInterrupt = false;
			sprite.SortingOrder = 0;
			StopCoroutine("StayPlatform");
			JumpCliffFromPlatform(platform, cliffHit);
		}
		else {
			anima = (int)(ANIMATIONS.FALL);
			playAnimation();
			float distTraveled = 0;
			Vector3 startPoint = transform.position;
			while (distTraveled < moveVector.magnitude) {
				Vector3 movement = Time.deltaTime * moveVector.normalized * speed;
				transform.position = transform.position + movement;
				distTraveled += movement.magnitude;
				yield return null;
			}
			transform.position = startPoint + moveVector;
			sprite.SortingOrder = 0;
			noInterrupt = false;
			StopCoroutine("StayPlatform");
		}
	}

	private void JumpCliffFromPlatform(GameObject platform, RaycastHit cliffHit) {
		ArrayList list = new ArrayList(3);
		list.Add(cliffHit.collider.gameObject);
		list.Add(cliffHit.transform.position);
		list.Add(true);
		StartCoroutine("JumpCliff", list);
	}

	// Make the player automatically jump all the way down a cliff, if possible
	IEnumerator JumpCliff(ArrayList objectAndHitPosition) {
		GameObject gameObject = objectAndHitPosition[0] as GameObject;
		Vector3 cliffHitPosition = (Vector3) objectAndHitPosition[1];
		bool isCliffTop = gameObject.CompareTag("Cliff Top");
		Vector3 moveVector = getMoveVector(); // Returns us how far and in what direction we need to move to get across one tile
		Vector3 hitVector = checkAcrossCollision(gameObject.transform.position, -moveVector, 1 << 14);// Check to see if we're approaching the cliff from the correct side
		//bool onPlatform = checkDownCollision(transform.position + new Vector3(0, 1000, 0), 1 << 15);// Check to see if the player is on a platform
		bool onPlatform = objectAndHitPosition.Count > 2;
		// Climb up cliff if on platform
		if (onPlatform) {
			Vector3 displacement = moveVector;
			if (direction == (int) DIRECTIONS.NORTH) {
				//TODO: Check to make sure that is legal movement, i.e. if cliff is 1 high
				moveVector *= 2;
				displacement *= 2;
			} else if (direction != (int) DIRECTIONS.SOUTH) {
				moveVector += Vector3.forward * 128;
				displacement += Vector3.back * 128;
			}
			if (checkAcrossCollision(gameObject.transform.position-displacement, getMoveVector(), 1 << 12) == Vector3.one) {
				noInterrupt = true;
				anima = (int)(ANIMATIONS.JUMP);
				playAnimation();
				// Move up the cliff
				Vector3 position = transform.position;
				position.y = GameManager.Instance.MapData.height-((int)position.z) / 128;
				GameObject shadow = (GameObject) Instantiate(shadowPrefab, position, shadowPrefab.transform.rotation);
				float distTraveled = 0;
				Vector3 normalizedMove = moveVector.normalized;
				while (distTraveled < moveVector.magnitude) {
					Vector3 movement = Time.deltaTime * normalizedMove * speed;
					transform.position = transform.position + movement;
					distTraveled += movement.magnitude;
					yield return null;
				}
				Destroy (shadow);
				noInterrupt = false;
			}
			
		}
		// climb down cliff if on the correct side of the cliff
		else if (isCliffTop && hitVector.Equals(cliffHitPosition)) {
			Vector3 collision = gameObject.transform.position;
			Vector3 displacement = Vector3.zero;
			if (direction == (int) DIRECTIONS.SOUTH) {
				displacement = checkCliffCollision(collision + moveVector) - collision + Vector3.back * 128;
			} else if (direction == (int) DIRECTIONS.NORTH) {
				//no displacement needed
			} else {
				displacement = Vector3.back * 128 * 2;
			}
			bool isPlatform = checkDownCollision(collision + displacement, 1 << 15);
			bool isEmpty = !checkDownCollision(collision + displacement, 1 << 12);
			bool isGround = checkDownCollision(collision + displacement, 1 << 8);
			if (isPlatform) {
				RaycastHit platformHit;
				Physics.Raycast(collision + displacement + new Vector3(0, 500, 0), Vector3.down, out platformHit, Mathf.Infinity, (1 << 15));
				StopCoroutine("JumpCliff");
				StartCoroutine("LandPlatform", platformHit.collider.gameObject);
			}
			else if (collision != Vector3.one && isGround && isEmpty) {
				noInterrupt = true;
				anima = (int)(ANIMATIONS.FALL);
				playAnimation();

				// Setting up move Vector to be used to move Roxanne
				moveVector += displacement;
				moveVector.y = 0;

				// Making shadow appear where you land
				Vector3 position = moveVector + transform.position;
				position.y = GameManager.Instance.MapData.height-((int)position.z) / 128;
				GameObject shadow = (GameObject) Instantiate(shadowPrefab, position, shadowPrefab.transform.rotation);

				// Moves Roxanne step by step down the cliff
				Vector3 normalizedMove = moveVector.normalized;
				float distTraveled = 0;
				while (distTraveled < moveVector.magnitude) {
					Vector3 movement = Time.deltaTime * normalizedMove * speed;
					transform.position = transform.position + movement;
					distTraveled += movement.magnitude;
					yield return null;
				}
				Destroy (shadow);
				noInterrupt = false;
				StopCoroutine("JumpCliff");
			}
		}
	}
	
	private bool checkVector(Vector3 one, Vector3 two) {
		float x = one.x - two.x;
		float z = one.z - two.z;
		if (x < 0) {
			x *= -1;
		}
		if (z < 0) {
			z *= -1;
		}
		if (x > .001 || z > .001) {
			return false;
		}
		return true;
	}
	
	private Vector3 getMoveVector() {
		if (direction == (int) DIRECTIONS.EAST) {
			return new Vector3(128, 0, 0);
		} else if (direction == (int) DIRECTIONS.NORTH) {
			return new Vector3(0, 0, 128);
		} else if (direction == (int) DIRECTIONS.WEST) {
			return new Vector3(-128, 0, 0);
		} else if (direction == (int) DIRECTIONS.SOUTH) {
			return new Vector3(0, 0, -128);
		}
		return new Vector3(0, 0, 0);
	}
	
	private bool checkDownCollision(Vector3 position, int layerMask) {
		if (Physics.Raycast(position + new Vector3(0, 10000, 0), Vector3.down, Mathf.Infinity, layerMask)) {
			return true;
		}
		return false;
	}
	
	private Vector3 checkAcrossCollision(Vector3 position, Vector3 direction, int layerMask) {
		RaycastHit hit;
		if (Physics.Raycast(position, direction, out hit, 90, layerMask)) {
			return hit.transform.position;
		}
		return Vector3.one;
	}
	
	private Vector3 checkCliffCollision(Vector3 position) {
		RaycastHit hit;
		if (Physics.Raycast(position, Vector3.back, out hit, Mathf.Infinity, 1 << 13)) {
			return hit.transform.position;
		}
		return Vector3.one;
	}
	
	public IEnumerator PlayNoInterruptAnimation (int animationNum) {
		noInterrupt = true;
		
		anima = animationNum;
		playAnimation();
		
		anim.AnimationCompleted = FinishAnimation;
		while(noInterrupt)
			yield return null;
		
		StopCoroutine("PlayNoInterruptAnimation");
	}
	void FinishAnimation(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip) {
		noInterrupt = false;
		anim.AnimationCompleted = null;
	}

	public override void takeHit (HitboxScript hit)
	{
		base.takeHit (hit);
		GameManager.Instance.Hud.numberOfHitsWaiting++;
	}
}
