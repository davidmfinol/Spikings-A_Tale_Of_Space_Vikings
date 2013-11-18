using UnityEngine;
using System.Collections;

public class HammerThrow : MonoBehaviour {

	public float speed;
	public int direction;
	public float throwDistance;
	public GameObject hitboxPrefab;
	public GameObject parentOb;
	public Vector3 throwDirection;
	public bool smashing;
	
	public tk2dSprite sprite;
	public tk2dSpriteAnimator anim;
	
	private float distanceTraveled;
	private GameObject hitBoxHolder;
	private GameObject hitbox;
	
	// Use this for initialization
	void Start () {
		
		//make this object a parent of the child
		transform.parent = parentOb.transform;
		
		distanceTraveled = 0;
		
		hitBoxHolder = new GameObject("Hammer Hitbox Holder");
		hitBoxHolder.transform.parent = transform;
		hitbox = (GameObject) Instantiate(hitboxPrefab, transform.position, transform.rotation);
		hitbox.transform.parent = hitBoxHolder.transform;
		hitbox.GetComponent<HitboxScript>().team = (int) TEAMS.PLAYER;

		hitbox.GetComponent<HitboxScript>().smash = smashing;

		hitbox.GetComponent<HitboxScript>().isThrownHammer = true;
		
		if(direction == (int)(DIRECTIONS.NORTH))
			anim.Play("UpHammer");
		if(direction == (int)(DIRECTIONS.WEST))
			anim.Play("LeftHammer");
		if(direction == (int)(DIRECTIONS.SOUTH))
			anim.Play("DownHammer");
		if(direction == (int)(DIRECTIONS.EAST))
			anim.Play("RightHammer");
	}
	
	// Update is called once per frame
	void Update () {
		if(!(distanceTraveled < throwDistance)) {
			throwDirection = (parentOb.transform.position-transform.position).normalized;
		}
		
		// Move the hammer
		Vector3 throwMovement = throwDirection * speed * Time.deltaTime;
		Vector3 pos = transform.position + throwMovement; // movement based off throw
		pos.y = 0;
		transform.position =  pos;
		
		// Order the sprite
		pos = sprite.transform.position;
		pos.y = GameManager.Instance.MapData.height-((int)pos.z) / 128 + 0.6f;
		sprite.transform.position = pos;
		
		distanceTraveled += Mathf.Abs(throwMovement.magnitude);

		Bounds bounds = ((BoxCollider)collider).bounds;
		if(bounds.Intersects(GameManager.Instance.Player.collider.bounds))
			OnTriggerEnter(GameManager.Instance.Player.collider);
	}
	
	void OnTriggerEnter(Collider collider){
		
		PlayerScript player = collider.GetComponent<PlayerScript>();
		
		if(player != null && distanceTraveled > throwDistance){
			Destroy(gameObject);
			player.powers++;
			player.StartCoroutine("PlayNoInterruptAnimation", (int) ANIMATIONS.CATCH);
		}
		
	}
}
