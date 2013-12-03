using UnityEngine;
using System.Collections;

public class CheckPointScript : TileScript {
	
	public bool isTeleport = false;
	public Transform hyperBeamPrefab;
	
	protected tk2dSprite sprite;
	protected tk2dSpriteAnimator anim;
	private PlayerScript player;
	
	override protected void Start() {
		base.Start();
		sprite = GetComponent<tk2dSprite>();
		anim = GetComponent<tk2dSpriteAnimator>();
	}
	
	void OnTriggerEnter(Collider collider) { 
		player = collider.gameObject.GetComponent<PlayerScript>();
		if (player != null) {
			if(isTeleport) {
				Transform hyperbeam = (Transform) Instantiate(hyperBeamPrefab, transform.position + new Vector3(0, -1, 256), hyperBeamPrefab.rotation);
				hyperbeam.GetComponent<tk2dSpriteAnimator>().AnimationCompleted = FinishTeleportAnimation;
				audio.Play();
			}
			else {
				CheckPointScript prevCheck = GameManager.Instance.spawnPoint.GetComponent<CheckPointScript>();
				if(prevCheck != null)
					prevCheck.anim.Play("SpawnPointStatic");
				GameManager.Instance.spawnPoint = transform;
				anim.Play("SpawnPointAnimation");
			}
		}
	}
	
	void FinishTeleportAnimation(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip) {
		player.transform.position = GameManager.Instance.centerPoint.position;
		Destroy(sprite.gameObject);
	}
}
