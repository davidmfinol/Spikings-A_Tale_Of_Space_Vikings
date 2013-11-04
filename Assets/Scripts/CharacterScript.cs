using UnityEngine;
using System.Collections;

public enum DIRECTIONS : int {
	EAST = 0,
	NORTH = 2,
	WEST = 4,
	SOUTH = 6
}

public enum ANIMATIONS : int {
	IDLE = 0,
	WALK = 1,
	ATTACK = 2,
	SMASH = 3,
	SPIN = 4,
	HIT = 5,
	DIE = 6,
	FALL = 7,
	JUMP = 8, 
	THROW = 9,
	CATCH = 10,
	PUSH = 11
}

public abstract class CharacterScript : TileScript {
	public int maxHealth = 50;
	public int currentHealth = 0;
	public GameObject hitBox;
	public GameObject HitPic;

	protected int direction = (int) DIRECTIONS.EAST;
	protected int anima = (int) ANIMATIONS.IDLE;
	protected int team = (int) TEAMS.PLAYER;
	
	/*  1 = attack
	 *  2 = smash
	 *  4 = pull
	 */
	public int powers;
	
	protected CharacterController controller;
	protected tk2dSprite sprite;
	protected tk2dSpriteAnimator anim;
	
	private bool isAttacking;
	private bool isBeingHit;
	protected bool isDead;
	protected bool noInterrupt;
	
// for combos
	public bool combo = false; // Whether the combo system is active. Set in code.
	public float comboMaxTime = 1.0f; // How long a timer for combo lasts in seconds. Fill this out in Inspector.
	public float comboSpanTime = .25f; // The span of when a combo can start. Fill this out in Inspector.
	public float comboMidPoint = .5f; // The position within the maxtime of a combo the span is active. Fill this out in Inspector.
	
	private int currentCombo = 0;
	private float comboTimeout = .0f;
	
	override protected void Start () {
		//base.Start();
		currentHealth = maxHealth;
		controller = GetComponent<CharacterController>();
		sprite = GetComponentInChildren<tk2dSprite>();
		anim = GetComponentInChildren<tk2dSpriteAnimator>();
		isAttacking = false;
		isBeingHit = false;
		noInterrupt = false;
		powers = 1;
	}
	
	virtual protected void Update () {
		if(!isDead && currentHealth <= 0) {
			OnDeath();
		}
		
		// Make our sprite layer correctly
		Vector3 pos = sprite.transform.position;
		pos.y = GameManager.Instance.MapData.height-((int)pos.z) / 128 + 0.6f;
		sprite.transform.position = pos;
		
		// Some combat control overhead
		int hasAttack = powers % 2;
		if (isAttacking && checkAttackAnimations()) {
			isAttacking = false;
		}
		if(isBeingHit && !anim.IsPlaying("hit0" + hasAttack) && !anim.IsPlaying("hit2" + hasAttack) && !anim.IsPlaying("hit4" + hasAttack) && !anim.IsPlaying("hit6" + hasAttack)){
			isBeingHit = false;
		}
		
		//added this in for combos
		DecreaseComboTime();
	}
	
	virtual protected void OnDeath() {
		isDead = true;
		Destroy(gameObject);
	}
	
	/*
	 * attacktype = 0, basic normal attack
	 * attacktype = 1, double damage
	 * attacktype = 2, double damage, aoe
	 */
	protected void spawnHitBox(int team, int attackType = 0) {
		Vector3 pos = Vector3.zero;
		if(attackType < 2)
			pos.x += controller.radius * 2;
		GameObject box = (GameObject) Instantiate(hitBox, pos, Quaternion.identity);
		HitboxScript hitBoxScript = box.GetComponent<HitboxScript>();
		hitBoxScript.team = team;
		if (powers >> 1 % 2 == 1) {
			hitBoxScript.smash = true;
		}
		if(attackType > 0)
			hitBoxScript.damage *= 2;
		if(attackType == 2)
			box.transform.localScale = box.transform.localScale * 4;
		
		GameObject buffer = new GameObject();
		box.transform.parent = buffer.transform;
		buffer.transform.parent = transform;
		buffer.transform.position = transform.position;
		Vector3 rot = new Vector3(0, -360 * (direction / 8.0f), 0);
		buffer.transform.Rotate(rot);
	}
	
	protected virtual void processInput(float x, float z) {
		bool xIsZero = x == 0;
		bool zIsZero = z == 0;
		if (xIsZero && zIsZero) {
			anima = 0;
		} else if (xIsZero) {
			checkZ(z);
		} else if (zIsZero) {
			checkX(x);
		} else {
			checkX(x);
		}
	}
	private void checkZ(float z) {
		if (z > 0) {
			direction = (int) DIRECTIONS.NORTH;
		} else {
			direction = (int) DIRECTIONS.SOUTH;
		}
		anima = 1;
	}
	private void checkX(float x) {
		if (x > 0) {
			direction = (int) DIRECTIONS.EAST;
		} else {
			direction = (int) DIRECTIONS.WEST;
		}
		anima = 1;
	}
	
	protected void playAnimation() {
		int hasAttack = powers % 2;
		if (anima == (int) ANIMATIONS.IDLE) {
			if (!anim.IsPlaying("idle" + direction + hasAttack)) {
				anim.Play("idle" + direction + hasAttack);
			}
		} else if (anima == (int) ANIMATIONS.WALK) {
			if (!anim.IsPlaying("walk" + direction + hasAttack)) {
				anim.Play("walk" + direction + hasAttack);
			}
		} else if (anima == (int) ANIMATIONS.ATTACK) {
			if (!isAttacking && !anim.IsPlaying("attack" + direction + hasAttack)) {
				Debug.Log ("play jab");
				isAttacking = true;
				anim.Play("attack" + direction + hasAttack);
				// TODO: PLAY SOUNDS AS APPROPRIATE
				GetComponentInChildren<AudioSource>().Play();
				spawnHitBox(team);
			}
		} else if (anima == (int) ANIMATIONS.SMASH) {
			if (!anim.IsPlaying("smash" + direction + hasAttack)) {
				Debug.Log ("play smash");
				isAttacking = true;
				anim.Play("smash" + direction + hasAttack);
				// TODO: PLAY SOUNDS AS APPROPRIATE
				GetComponentInChildren<AudioSource>().Play();
				spawnHitBox(team, 1);
			}
		} else if (anima == (int) ANIMATIONS.SPIN) {
			if (!anim.IsPlaying("spin" + direction + hasAttack)) {
				Debug.Log ("play spin");
				isAttacking = true;
				anim.Play("spin" + direction + hasAttack);
				currentCombo = 0;	
				// TODO: PLAY SOUNDS AS APPROPRIATE
				GetComponentInChildren<AudioSource>().Play();
				spawnHitBox(team, 2);
			}
		} else if (anima == (int) ANIMATIONS.HIT) {
			if (!anim.IsPlaying("hit" + direction + hasAttack)) {
				isBeingHit = true;
				anim.Play("hit" + direction + hasAttack);
			}
		} else if (anima == (int) ANIMATIONS.DIE) {
			if (!anim.IsPlaying("die" + direction + hasAttack)) {
				isBeingHit = true;
				anim.Play("die" + direction + hasAttack);
			}
		} else if (anima == (int) ANIMATIONS.FALL) {
			if (!anim.IsPlaying("fall" + direction + hasAttack)) {
				anim.Play("fall" + direction + hasAttack);
			}
		} else if (anima == (int) ANIMATIONS.JUMP) {
			if (!anim.IsPlaying("jump" + direction + hasAttack)) {
				anim.Play("jump" + direction + hasAttack);
			}
		} else if (anima == (int) ANIMATIONS.THROW) {
			if (!anim.IsPlaying("throw" + direction + hasAttack)) {
				anim.Play("throw" + direction + hasAttack);
			}
		} else if (anima == (int) ANIMATIONS.CATCH) {
			if (!anim.IsPlaying("catch" + direction + hasAttack)) {
				anim.Play("catch" + direction + hasAttack);
			}
		} else if (anima == (int) ANIMATIONS.PUSH) {
			if (!anim.IsPlaying("push" + direction + hasAttack)) {
				anim.Play("push" + direction + hasAttack);
			}
		}
	}
	
	protected void move(float x, float z) {
		if (isAttacking || isBeingHit || anima == (int) (ANIMATIONS.DIE) || noInterrupt)
			return;
		
		processInput(x, z);
		playAnimation();
		Vector3 movement = new Vector3(x, 0, z);
		movement *= Time.deltaTime;
		controller.Move(movement);
		
		// Make sure we stay on y = 0
		Vector3 pos = transform.position;
		pos.y = 0;
		transform.position = pos;
	}
	
	protected void attack() {
		if(anima == (int) ANIMATIONS.HIT || anima == (int) ANIMATIONS.DIE || noInterrupt) {
			return;
		}
		
		//added in for combos
		if (combo) { //makes it so you have enable combo aka for player
			IncreaseComboTime(); //Doing logic for timing before animation
		}
		
		if ( !combo || currentCombo == 0) {
			Debug.Log ("jab");
			anima = (int) ANIMATIONS.ATTACK;
		} else if(currentCombo == 1) {
			Debug.Log ("smash");
			anima = (int) ANIMATIONS.SMASH;
		} else {
			Debug.Log ("spin");
			anima = (int) ANIMATIONS.SPIN;
		}
		
		playAnimation();
		
	}
	
	//Combo method
	private void IncreaseComboTime() {
		if (comboTimeout < 0) {
			Debug.Log("Combo reset!");
			currentCombo = 0;	
			comboTimeout = comboMaxTime;
		}
		else if(currentCombo < 2 &&
			comboTimeout > 0 &&
			comboTimeout > comboMidPoint - comboSpanTime &&
			comboTimeout < comboMidPoint + comboSpanTime) {
			Debug.Log("Combo increase!");
			currentCombo++;
			comboTimeout = comboMaxTime;
		} 
	}
	private void DecreaseComboTime () {
		comboTimeout -= 1.0f * Time.deltaTime;	
	}
	
	public virtual void takeHit(HitboxScript hit){
		if(anima == (int) (ANIMATIONS.DIE) || noInterrupt)
			return;
		
		this.currentHealth-=hit.damage;
		anima = (int) ANIMATIONS.HIT;
		playAnimation();
		
		//hitPic display
		GameObject instance = Instantiate(HitPic, sprite.transform.position + Vector3.up, sprite.transform.rotation) as GameObject;
		Destroy(instance, 0.25f);	
	}
	
	private bool checkAttackAnimations() {
		int hasAttack = powers % 2;
		if (!anim.IsPlaying("attack0" + hasAttack) && !anim.IsPlaying("attack2" + hasAttack) && !anim.IsPlaying("attack4" + hasAttack) && !anim.IsPlaying("attack6" + hasAttack)
			&& !anim.IsPlaying("smash0" + hasAttack) && !anim.IsPlaying("smash2" + hasAttack) && !anim.IsPlaying("smash4" + hasAttack) && !anim.IsPlaying("smash6" + hasAttack)
			&& !anim.IsPlaying("spin0" + hasAttack) && !anim.IsPlaying("spin2" + hasAttack) && !anim.IsPlaying("spin4" + hasAttack) && !anim.IsPlaying("spin6" + hasAttack)) {
			return true;
		}
		return false;
	}
}