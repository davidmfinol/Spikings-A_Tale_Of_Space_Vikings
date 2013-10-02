using UnityEngine;
using System.Collections;

public class RockScript : MonoBehaviour {
	private tk2dSpriteAnimator anim;
	private bool destroying;

	// Use this for initialization
	private void Start () {
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
