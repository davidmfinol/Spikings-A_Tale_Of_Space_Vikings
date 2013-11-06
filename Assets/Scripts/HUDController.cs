using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {
	
    public tk2dUIProgressBar currentHealth;
    public tk2dUIProgressBar losingHealth;
	public float healthBarSmoothTime = 0.5f;
	
	public Transform thoughtBubble;
	public tk2dTextMesh thoughtText;
	
	public Transform Hammer;
	
	public tk2dTextMesh currentPoints;
	
    private float healthBarVelocity = 0.0f;
	
	void Start () {
		PlayerScript player = GameManager.Instance.Player;
		currentHealth.Value = ((float)player.currentHealth)/player.maxHealth;
		losingHealth.Value = ((float)player.currentHealth)/player.maxHealth;
	}
	
    void Update() {
		PlayerScript player = GameManager.Instance.Player;
        losingHealth.Value = Mathf.SmoothDamp( losingHealth.Value, ((float)player.currentHealth)/player.maxHealth, ref healthBarVelocity, healthBarSmoothTime, Mathf.Infinity, tk2dUITime.deltaTime );
    	currentHealth.Value = ((float)player.currentHealth)/player.maxHealth;
		Hammer.gameObject.SetActive(player.powers % 2 == 1);
		
		currentPoints.text = GameManager.Instance.partsCollected.ToString() + "/3 Parts Found";
        currentPoints.Commit();
	}
	
	public void showThought(string message, bool continueText = false) {
		if(continueText) {
			int pad = thoughtText.maxChars - message.Length - 13;
			for(int i = 0; i < pad ; ++i)
				message += " ";
			message += "[PRESS SPACE]";
			thoughtText.color = Color.red;
		}
		else
			thoughtText.color = Color.black;
		thoughtText.text = message;
		thoughtText.Commit();
		thoughtBubble.gameObject.SetActive(true);
	}
	
	public void hideThought() {
		thoughtBubble.gameObject.SetActive(false);
	}
}
