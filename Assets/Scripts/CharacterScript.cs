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
	THROW = 8,
	CATCH = 9,
	JUMP = 10
}

public abstract class CharacterScript : TileScript {
	public int maxHealth = 50;
	public int currentHealth = 0;
	public GameObject hitBox;
	public bool combo = false;

	protected int direction = (int) DIRECTIONS.EAST;
	protected int anima = (int) ANIMATIONS.IDLE;
	protected int team = (int) TEAMS.PLAYER;
	
	/*  1 = attack
	 *  2 = smash
	 *  4 = pull
	 */
	protected int powers;
	
	protected CharacterController controller;
	protected tk2dSprite sprite;
	protected tk2dSpriteAnimator anim;
	
	private bool isAttacking;
	private bool isBeingHit;
	protected bool noInterrupt;
	
// for combos
	public static ComboSequence[] combos = new ComboSequence[3]; //Combo class. Fill this out in Inspector.
	float comboMaxTime = 1.0f; //How long a timer for combo lasts in seconds. Fill this out in Inspector.
	float comboSpanTime = .25f; //The span of when a combo can start. Fill this out in Inspector.
	float comboMidPoint = .5f; //The position within the maxtime of a combo the span is active. Fill this out in Inspector.
	private int currentCombo = -1;
	private float comboTimeout = .0f;
	
//hitBoxHolder
	public GameObject HitPic;
	
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
		if(currentHealth <= 0) {
			OnDeath();
		}
		
		// Make our sprite layer correctly
		Vector3 pos = sprite.transform.position;
		pos.y = GameManager.Instance.MapData.height-((int)pos.z) / 128 + 0.5f;
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
		if(comboTimeout > 0) {
			DecreaseTime();
		}
	}
	
	virtual protected void OnDeath() {
		Destroy(gameObject);
	}
	
	protected void spawnHitBox(int team) {
		Vector3 pos = Vector3.zero;
		pos.x += 167;
		GameObject box = (GameObject) Instantiate(hitBox, pos, Quaternion.identity);
		box.GetComponent<HitboxScript>().team = team;
		if (powers >> 1 % 2 == 1) {
			box.GetComponent<HitboxScript>().smash = true;
		}
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
			}
		} else if (anima == (int) ANIMATIONS.SMASH) {
			if (!isAttacking && !anim.IsPlaying("smash" + direction + hasAttack)) {
				Debug.Log ("play smash");
				isAttacking = true;
				anim.Play("smash" + direction + hasAttack);
			}
		} else if (anima == (int) ANIMATIONS.SPIN) {
			if (!isAttacking && !anim.IsPlaying("spin" + direction + hasAttack)) {
				Debug.Log ("play spin");
				isAttacking = true;
				anim.Play("spin" + direction + hasAttack);
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
		if (combos) { //makes it so you have enable combo aka for player
			ComboTime(); //Doing logic for timing before animation
		}
		
		if (currentCombo == 0) {
			Debug.Log ("jab");
			anima = (int) ANIMATIONS.ATTACK;
		} else if(currentCombo == 1) {
			Debug.Log ("smash");
			anima = (int) ANIMATIONS.SMASH;
		} else {
			Debug.Log ("spin");
			anima = (int) ANIMATIONS.SPIN;
		}
		if (!isAttacking) {
			spawnHitBox(team);
		}
		playAnimation();
		GetComponentInChildren<AudioSource>().Play();
	}
	
	//HammerThrow Method
	
	
	
	public void takeHit(HitboxScript hit){
		if(anima == (int) (ANIMATIONS.DIE) || noInterrupt)
			return;
		
		this.currentHealth-=hit.damage;
		anima = (int) ANIMATIONS.HIT;
		playAnimation();
		
		//hitPic display
		GameObject instance = Instantiate(HitPic, sprite.transform.position + Vector3.up, sprite.transform.rotation) as GameObject;
		Destroy(instance, 0.25f);	
	}
	
	//Combo method
	private void ComboTime() {
		if(currentCombo<combos.Length-1 &&
			comboTimeout>0 &&
			comboTimeout>comboMidPoint - comboSpanTime &&
			comboTimeout<comboMidPoint+comboSpanTime ||
			currentCombo==-1) {
			currentCombo++;
		} else {
			currentCombo = -1;	
		}
		comboTimeout = comboMaxTime;
	}
	
	private void DecreaseTime () {
		comboTimeout -= 1 * Time.deltaTime;	
	}
	
	public class ComboSequence {
		string comboName;
		string comboAnimation;
		AudioClip comboSound;
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