using UnityEngine;
using System.Collections;

public class PlayerScript : CharacterScript {
	public int speed = 500;
	//for checkpoint stuff
	public bool pastCheck = false;
	
	public GameObject hammerThrowPrefab;
	public GameObject shadowPrefab;
	
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
		else if (Input.GetButtonDown("Fire3") && powers %2 ==1){
			hammerThrow();
		}
		float x = Input.GetAxis("Horizontal") * speed;
		float z = Input.GetAxis("Vertical") * speed;
		move(x, z);
	}
	
	
	
	//HammerThrow Method
	protected void hammerThrow() {
		if(anima == (int) ANIMATIONS.HIT || anima == (int) ANIMATIONS.DIE || noInterrupt) {
			return;
		}
		
		GameObject handPosition = new GameObject();
		handPosition.transform.parent = transform;
		handPosition.transform.position = transform.position + new Vector3(0, 0, 75);
		
		GameObject spawnedHammer = (GameObject) Instantiate(hammerThrowPrefab, handPosition.transform.position, hammerThrowPrefab.transform.rotation);
		HammerThrow hammer = spawnedHammer.GetComponent<HammerThrow>();
		
		hammer.parentOb = handPosition;
		hammer.throwDirection = getMoveVector().normalized;
		hammer.smashing = powers > 1;
		
		powers--;
		
		StartCoroutine("PlayNoInterruptAnimation", (int) ANIMATIONS.THROW);
	}
	
	
	
	override protected void OnDeath () {
		currentHealth = maxHealth;
		//editing for checkpoint
		//if(pastCheck){
		 // transform.position = GameManager.Instance.spawnPoint.position;
		//}
		//else{
		transform.position = GameManager.Instance.spawnPoint.position;
		//}
	}
	
	private void OnControllerColliderHit(ControllerColliderHit hit) {
		GameObject gameObject = hit.collider.gameObject;
		if(gameObject.CompareTag("Platform")) {
			InteractWithPlatform(gameObject);
		} else if (gameObject.CompareTag("Cliff Top") || gameObject.CompareTag("Cliff Side")) {
			StartCoroutine("JumpCliff", hit);
		} else if (gameObject.CompareTag("Item")) {
			ItemScript item = gameObject.GetComponent<ItemScript>();
			powers += item.power;
			currentHealth += item.health;
			Destroy(gameObject);
		}
		
	}
	
	private void InteractWithPlatform(GameObject gameObject) {
		if (Input.GetButton("Jump")) {
			StartCoroutine("JumpPlatform");
		} else if (Input.GetButtonDown("Fire2")) {
			// TODO: DON'T FORCE THIS TO BE GRID-BASED
			// SHOULD PROBABLY BE ABLE TO JUST SET ANIMA = PUSHING AND CALL PLAYANIMATION() OVER AND OVER WHILE ALLOWING REGULAR MOVEMENT?
			Vector3 position = gameObject.transform.position;
			position.y = 0;
			Vector3 displacement = getMoveVector();
			Vector3 hitVector = checkAcrossCollision(position, displacement, 1 << 14);//check to see if pushing by a cliff
			if (hitVector == Vector3.one) { // no cliff
				position = gameObject.transform.position + displacement;
			} else {
				if (checkVector(hitVector, position)) { // over cliff
					displacement += Vector3.back * 128;
					position = gameObject.transform.position + displacement;
				} else { //into cliff
					position = Vector3.one;
				}
			}
			if (position != Vector3.one && checkDownCollision(position, 1 << 11)) {
				gameObject.transform.position = position;
				transform.position += getMoveVector() / 2;
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
		if (Physics.Raycast(position, Vector3.down, Mathf.Infinity, layerMask)) {
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
	
	IEnumerator JumpPlatform() {
		noInterrupt = true; // this animation/action cannot be interrupted
		anima = (int)(ANIMATIONS.JUMP);
		playAnimation();
		
		// TODO: Fix movement so you can avoid getting on platform
		
		Vector3 moveVector = getMoveVector();
		moveVector = moveVector.normalized;
		
		float distTraveled = 0;
		while (distTraveled < 75) {
			Vector3 movement = Time.deltaTime * moveVector * speed;
			transform.position = transform.position + movement;
			distTraveled += movement.magnitude;
			yield return null;
		}
		
		noInterrupt = false;
		StopCoroutine("JumpPlatform");
	}
	
	// Make the player automatically jump all the way down a cliff, if possible
	IEnumerator JumpCliff(ControllerColliderHit hit) {
		GameObject gameObject = hit.collider.gameObject;
		bool isCliffTop = gameObject.CompareTag("Cliff Top");
		Vector3 moveVector = getMoveVector(); // Returns us how far and in what direction we need to move to get across one tile
		Vector3 hitVector = checkAcrossCollision(gameObject.transform.position, moveVector, 1 << 14);// Check to see if we're approaching the cliff from the correct side
		bool onPlatform = checkDownCollision(transform.position + new Vector3(0, 1000, 0), 1 << 15);// Check to see if the player is on a platform
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
		// climb up cliff if on the correct side of the cliff
		else if (isCliffTop && hitVector.Equals(hit.transform.position)) { 
			Vector3 collision = gameObject.transform.position;
			bool isSouthOrNorth = false;
			if (direction == (int) DIRECTIONS.SOUTH) {
				isSouthOrNorth = true;
				collision = checkCliffCollision(collision + moveVector);
			} else if (direction == (int) DIRECTIONS.NORTH) {
				isSouthOrNorth = true;
			}
			bool spaceEmpty = false;
			if (isSouthOrNorth) {
				spaceEmpty = checkAcrossCollision(collision, moveVector, 1 << 12) == Vector3.one;
			} else {
				//TODO: does not check for collisions with cliff side
				spaceEmpty = checkAcrossCollision(collision + Vector3.back * 128, moveVector, 1 << 12) == Vector3.one;
			}
			if (collision != Vector3.one && spaceEmpty) {
				noInterrupt = true;
				anima = (int)(ANIMATIONS.FALL);
				playAnimation();
				moveVector+= (collision - transform.position);
				moveVector.y = 0;
				Vector3 position = transform.position;
				position.y = GameManager.Instance.MapData.height-((int)position.z) / 128;
				if (!isSouthOrNorth) {
					position += Vector3.back * 128 + moveVector;
				} else {
					position += moveVector;
				}
				GameObject shadow = (GameObject) Instantiate(shadowPrefab, position, shadowPrefab.transform.rotation);
				Vector3 normalizedMove = moveVector.normalized;
				float distTraveled = 0;
				while (distTraveled < moveVector.magnitude) {
					Vector3 movement = Time.deltaTime * normalizedMove * speed;
					transform.position = transform.position + movement;
					distTraveled += movement.magnitude;
					yield return null;
				}
				if(!isSouthOrNorth) {
					//TODO: (also, perhaps merge motion)
					distTraveled = 0;
					while (distTraveled < 128) {
						Vector3 movement = Time.deltaTime * Vector3.back * speed;
						transform.position = transform.position + movement;
						distTraveled += movement.magnitude;
						yield return null;
					}
				}
				Destroy (shadow);
				noInterrupt = false;
			}
			
		}
		StopCoroutine("JumpCliff");
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
}
