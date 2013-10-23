using UnityEngine;
using System.Collections;

public class PlayerScript : CharacterScript {
	public int speed = 500;
	//for checkpoint stuff
	public bool pastCheck = false;
	
	
	override protected void Start () {
		base.Start();
		team = (int) TEAMS.PLAYER;
		powers = 0;
	}
	
	// Update is called once per frame
	override protected void Update () {
		base.Update();
		if (Input.GetButtonDown("Fire1") && powers % 2 == 1) {
			attack();
		}
		float x = Input.GetAxis("Horizontal") * speed;
		float z = Input.GetAxis("Vertical") * speed;
		move(x, z);
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
		} else if (gameObject.CompareTag("Cliff Top")) {
			StartCoroutine("JumpCliff", hit);
		} else if (gameObject.CompareTag("Item")) {
			powers += gameObject.GetComponent<ItemScript>().power;
			Destroy(gameObject);
		}
	}
	
	private void InteractWithPlatform(GameObject gameObject) {
		if (Input.GetButton("Jump")) {
			StartCoroutine("JumpPlatform");
		} else if (Input.GetButtonDown("Fire2")) {
			// TODO: DON'T FORCE THIS TO BE GRID-BASED
			// SHOULD PROBABLY BE ABLE TO JUST SET ANIMA = PUSHING AND CALL PLAYANIMATION() OVER AND OVER WHILE ALLOWING REGULAR MOVEMENT?
			Vector3 position = gameObject.transform.position + getMoveVector();
			if (checkDownCollision(position, 1 << 11)) {
				gameObject.transform.position = position;
				transform.position += getMoveVector();
			}
		}
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
		// TODO: HAVE ANIMATION
		// anima = (int)(ANIMATIONS.JUMP);
		// PlayAnimation();
		
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
		
		Vector3 moveVector = getMoveVector(); // Returns us how far and in what direction we need to move to get across one tile
		Vector3 hitVector = checkAcrossCollision(gameObject.transform.position, moveVector, 1 << 14);// Check to see if we're approaching the cliff from the correct side
		Vector3 platformVector = checkAcrossCollision(gameObject.transform.position, -moveVector, 1 << 15);// Check to see if the player is on a platform
		
		// Climb up cliff if on platform
		if (platformVector != Vector3.one) {
			
			if (checkAcrossCollision(gameObject.transform.position-moveVector, moveVector, 1 << 12) == Vector3.one) {
				noInterrupt = true;
				// TODO: HAVE ANIMATION!
				// anima = (int)(ANIMATIONS.JUMP);
				// PlayAnimation();
				Vector3 normalizedMove = moveVector.normalized;
				// Move up the cliff
				float distTraveled = 0;
				while (distTraveled < moveVector.magnitude) {
					Vector3 movement = Time.deltaTime * normalizedMove * speed;
					transform.position = transform.position + movement;
					distTraveled += movement.magnitude;
					yield return null;
				}
				noInterrupt = false;
			}
			
		}
		// climb up cliff if on the correct side of the cliff
		else if (hitVector.Equals(hit.transform.position)) { 
			
			Vector3 collision = gameObject.transform.position;
			bool isSouth = false;
			if (direction == (int) DIRECTIONS.SOUTH) {
				isSouth = true;
				collision = checkCliffCollision(collision + moveVector);
			}
			if (collision != Vector3.one && checkAcrossCollision(collision, moveVector, 1 << 12) == Vector3.one) {
				noInterrupt = true;
				// TODO: HAVE ANIMATION!
				// anima = (int)(ANIMATIONS.JUMP);
				// PlayAnimation();
				moveVector+= (collision - transform.position) ;
				Vector3 normalizedMove = moveVector.normalized;
				float distTraveled = 0;
				while (distTraveled < moveVector.magnitude) {
					Vector3 movement = Time.deltaTime * normalizedMove * speed;
					transform.position = transform.position + movement;
					distTraveled += movement.magnitude;
					yield return null;
				}
				if(!isSouth) {
					//TODO: MAKE SURE THAT THE SPOT BELOW IS OPEN (also, perhaps merge motion)
					distTraveled = 0;
					while (distTraveled < 128) {
						Vector3 movement = Time.deltaTime * Vector3.back * speed;
						transform.position = transform.position + movement;
						distTraveled += movement.magnitude;
						yield return null;
					}
				}
				noInterrupt = false;
			}
			
		}
		StopCoroutine("JumpCliff");
	}
	
	
}
