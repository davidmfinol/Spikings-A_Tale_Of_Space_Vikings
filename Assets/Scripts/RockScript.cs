using UnityEngine;
using System.Collections;

public class RockScript : TileScript {
	private tk2dSpriteAnimator anim;
	private bool destroying;

	override protected void Start () {
		base.Start();
		anim = GetComponent<tk2dSpriteAnimator>();
		destroying = false;
	}
	
	private void Update () {
		if(destroying && !anim.IsPlaying("destroy")) {
			Destroy(gameObject);
			destroying = false;
		}
	}
	
	 public void Smash () {
		if(!anim.IsPlaying("destroy")) {
			anim.Play("destroy");
			destroying = true;
		}
	}
}
