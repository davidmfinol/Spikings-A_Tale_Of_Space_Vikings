using UnityEngine;
using System.Collections;

public class HelpScript : MonoBehaviour
{
	public enum Condition : int 
	{
		Always = 0,
		NoHammer = 1,
		HasHammer = 2,
	}
	
	public string message;
	
	public Condition condition;
	
	public bool causesPause = false;
	
	private bool alreadyTriggered = false;
	
	
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
		default: break;
		}
		
		if( message != null && message != "" )
		{
			if(!alreadyTriggered) 
			{
				GameManager.Instance.Hud.showThought(message, causesPause);
				if(causesPause)
				{
					Time.timeScale = 0;
					StartCoroutine("WaitForUnpause");
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
}
