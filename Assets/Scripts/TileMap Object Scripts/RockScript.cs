using UnityEngine;
using System.Collections;

public class RockScript : TileScript {
	private tk2dSpriteAnimator anim;
	private bool destroying;
	
	public GameObject meadPrefab;

	override protected void Start () {
		base.Start();
		anim = GetComponent<tk2dSpriteAnimator>();
		destroying = false;
	}
	
	private void Update () {
		if(destroying && !anim.IsPlaying("destroy")) {
			if (Random.Range(0, 4) == 0) {
				Vector3 pos = transform.position;
				pos.y = 0;
				Instantiate(meadPrefab, pos, meadPrefab.transform.rotation);
			}
			Destroy(gameObject);
			destroying = false;
		}
	}
	
	 public void Smash () {
		if(!anim.IsPlaying("destroy")) {
			anim.Play("destroy");
			destroying = true;
			audio.Play ();	
		}
	}
}
