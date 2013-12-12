using UnityEngine;
using System.Collections;

public class HelpScript : MonoBehaviour
{
	public enum Condition : int 
	{
		Always = 0,
		NoHammer = 1,
		HasHammer = 2,
		FoundOnePart = 3,
		FoundTwoParts = 4,
		FoundAllParts = 5,
	}
	
	public string message;
	
	public Condition condition;
	
	public bool causesPause = false;

	public float fadeTime = 10;
	
	private bool alreadyTriggered = false;
	
	public Transform bossPrefab;
	public Transform music;
	public AudioClip bossMusic;
	
	void OnTriggerEnter(Collider collider)
	{
		PlayerScript player = collider.gameObject.GetComponent<PlayerScript>();
		if (player == null)
			return;
		
		switch(condition)
		{
		case Condition.Always : break;
		case Condition.NoHammer : if(player.powers > 0) return; break;
		case Condition.HasHammer : if(player.powers < 1) return; break;
		case Condition.FoundOnePart : if(GameManager.Instance.partsCollected < 1) return; break;
		case Condition.FoundTwoParts : if(GameManager.Instance.partsCollected < 2) return; break;
		case Condition.FoundAllParts : if(GameManager.Instance.partsCollected < 3) return; 
			if (!alreadyTriggered) {
				Instantiate(bossPrefab, transform.position, bossPrefab.rotation);
				music.audio.Stop();
				music.audio.clip = bossMusic;
				music.audio.Play();
			}
			break;
		default: break;
		}


		if( message != null && message != "" )
		{
			if(!alreadyTriggered) 
			{
				GameManager.Instance.Hud.showThought(message, GetInstanceID());
				if(causesPause)
				{
					Time.timeScale = 0;
					StartCoroutine("WaitForUnpause");
				}
				if(fadeTime > 0)
				{
					StartCoroutine("FadeAfterSeconds", fadeTime);
				}
			}
			alreadyTriggered = true;
		}
		else
			GameManager.Instance.Hud.hideThought();
	}
	
	IEnumerator WaitForUnpause()
	{
		while(!Input.GetButton("Jump"))
			yield return null;
		Time.timeScale = 1;
		GameManager.Instance.Hud.hideThought();
		StopCoroutine("WaitForUnpause");
	}

	IEnumerator FadeAfterSeconds(float fadeTime)
	{
		float t = 0;
		while (t < fadeTime)
		{
			t+= Time.deltaTime;
			yield return null;
		}
		GameManager.Instance.Hud.hideThought(GetInstanceID());
	}
}
