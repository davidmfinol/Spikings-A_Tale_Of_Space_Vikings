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
			// TODO: ALLOW YOU TO CLIMB UP CLIFFS IF YOU'RE ON A PLATFORM
			// SOMETHING LIKE: TAKE A VECTOR FROM THE CLIFF'S POSITION, SHOOT IT TOWARDS THE PLAYER, AND ALLOW JUMPING IF YOU HIT A PLATFORM OBJECT
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
	
	// Make the player automatically jump all th way down a cliff, if possible
	IEnumerator JumpCliff(ControllerColliderHit hit) {
		GameObject gameObject = hit.collider.gameObject;
		
		// Returns us how far and in what direction we need to move to get across one tile
		Vector3 moveVector = getMoveVector(); 
		
		// Returns the location of the nearest cliff wall in the direction of moveVector
		Vector3 hitVector = checkAcrossCollision(gameObject.transform.position, moveVector, 1 << 14);
		
		// Make sure that we're actually looking at the the nearest cliff
		if (hitVector.Equals(hit.transform.position)) {
			
			// Now get the actual position of the cliff that we are trying to jump over
			Vector3 collision = gameObject.transform.position;
			
			// Long cliffs are only located to the south
			// So if that cliff is to the south, we want to look at the bottom of the cliff
			bool isSouth = false;
			if (direction == (int) DIRECTIONS.SOUTH) {
				isSouth = true;
				collision = checkCliffCollision(collision + moveVector); //DOES THIS WORK CORRECTLY?
			}
			if (collision != Vector3.one && checkAcrossCollision(collision, moveVector, 1 << 12) == Vector3.one) {
				noInterrupt = true;
				// TODO: HAVE ANIMATION!
				// anima = (int)(ANIMATIONS.JUMP);
				// PlayAnimation();
				
				// We define how far and in what direction we need to move to get across the cliff
				moveVector+= (collision - transform.position) ; //+ new Vector3(0, 0, -75)
				Vector3 normalizedMove = moveVector.normalized;
				
				// Move across the cliff
				float distTraveled = 0;
				while (distTraveled < moveVector.magnitude) {
					Vector3 movement = Time.deltaTime * normalizedMove * speed;
					transform.position = transform.position + movement;
					distTraveled += movement.magnitude;
					yield return null;
				}
				
				// If we're moving left or right, we also move down one tile
				if(!isSouth) {
					//TODO: MAKE SURE THAT THE SPOT BELOW IS OPEN
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
