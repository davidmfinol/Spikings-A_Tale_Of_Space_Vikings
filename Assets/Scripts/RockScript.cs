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
			Debug.Log("destroyed rock");
			renderer.enabled = false;
			destroying = false;
		}
	}
	
	 public void Smash () {
		Debug.Log("smash");
		if(!anim.IsPlaying("destroy")) {
			Debug.Log ("play animation");
			anim.Play("destroy");
			destroying = true;
		}
	}
}
