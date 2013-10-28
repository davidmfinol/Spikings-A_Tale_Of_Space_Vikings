using UnityEngine;
using System.Collections;

public class HammerThrow : MonoBehaviour {

	public float speed;
	public float throwDistance;
	public GameObject hitboxPrefab;
	public GameObject parentOb;
	public Vector3 throwDirection;
	
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
		hitbox.GetComponent<HitboxScript>().autoSelfDestruct = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!(distanceTraveled < throwDistance)) {
			throwDirection = (parentOb.transform.position-transform.position).normalized;
		}
		
		Vector3 throwMovement = throwDirection * speed * Time.deltaTime;
		Vector3 pos = transform.position + throwMovement; // movement based off throw
		pos.y = GameManager.Instance.MapData.height-((int)pos.z) / 128 + 0.6f;
		transform.position =  pos;
		Vector3 hitPos = hitbox.transform.position;
		hitPos.y = 0;
		hitbox.transform.position = hitPos;
		distanceTraveled += Mathf.Abs(throwMovement.magnitude);
	}
}
