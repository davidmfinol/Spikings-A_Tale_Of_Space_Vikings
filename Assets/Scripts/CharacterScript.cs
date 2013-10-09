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
	ATTACK = 2
}

public abstract class CharacterScript : MonoBehaviour {
	public int maxHealth = 50;
	public int currentHealth = 0;
	public GameObject hitBox;

	protected int direction = (int) DIRECTIONS.EAST;
	protected int anima = (int) ANIMATIONS.IDLE;
	protected int team = (int) TEAMS.PLAYER;
	protected int hasAttack;
	
	protected CharacterController controller;
	protected tk2dSprite sprite;
	protected tk2dSpriteAnimator anim;
	
	private float initialY;
	private bool isAttacking;
	
// for combos
	public static ComboSequence[] combos = new ComboSequence[3]; //Combo class. Fill this out in Inspector.
	float comboMaxTime = 1.0f; //How long a timer for combo lasts in seconds. Fill this out in Inspector.
	float comboSpanTime = .25f; //The span of when a combo can start. Fill this out in Inspector.
	float comboMidPoint = .5f; //The position within the maxtime of a combo the span is active. Fill this out in Inspector.
	private int currentCombo = -1;
	private float comboTimeout = .0f;
	
	virtual protected void Start () {
		currentHealth = maxHealth;
		controller = GetComponent<CharacterController>();
		sprite = GetComponent<tk2dSprite>();
		anim = GetComponent<tk2dSpriteAnimator>();
		initialY = transform.position.y;
		isAttacking = false;
		hasAttack = 1;
	}
	
	virtual protected void Update () {
		if(currentHealth <= 0) {
			OnDeath();
		}
		sprite.SortingOrder = GameManager.Instance.MapData.height*GameManager.Instance.MapData.partitionSizeY-((int)transform.position.z);
		Vector3 correction = transform.position;
		correction.y = initialY;
		transform.position = correction;
		if (isAttacking && !anim.IsPlaying("attack0" + hasAttack) && !anim.IsPlaying("attack2" + hasAttack) && !anim.IsPlaying("attack4" + hasAttack) && !anim.IsPlaying("attack6" + hasAttack)) {
			isAttacking = false;
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
		GameObject buffer = new GameObject();
		box.transform.parent = buffer.transform;
		buffer.transform.parent = transform;
		buffer.transform.position = transform.position + new Vector3(0, 0, -75);
		Vector3 rot = new Vector3(0, -360 * (direction / 8.0f), 0);
		buffer.transform.Rotate(rot);
	}
	
	protected void processInput(float x, float z) {
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
				isAttacking = true;
				anim.Play("attack" + direction + hasAttack);
			}
		}
	}
	
	protected void move(float x, float z) {
		if (!isAttacking) {
			processInput(x, z);
			playAnimation();
			Vector3 movement = new Vector3(x, 0/*TODO: -speed */, z);
			movement *= Time.deltaTime;
			controller.Move(movement);
		}
	}
	
	protected void attack() {
		anima = (int) ANIMATIONS.ATTACK;
		
		//added in for combos
		ComboTime(); //Doing logic for timing before animation
		
		//dirty implementation to test animations
		if (currentCombo == 0) {
			anima = (int) ANIMATIONS.ATTACK;
		} else if(currentCombo == 1) {
			anima = (int) ANIMATIONS.ATTACK;
		} else {
			anima = (int) ANIMATIONS.ATTACK;
		}
		if (!isAttacking) {
			spawnHitBox(team);
		}
		playAnimation();
		audio.Play();
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
}