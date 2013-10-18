using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {
	
    public tk2dUIProgressBar currentHealth;
    public tk2dUIProgressBar losingHealth;
	public float healthBarSmoothTime = 0.5f;
	
	public Transform thoughtBubble;
	public tk2dTextMesh thoughtText;
	
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
	}
	
	public void showThought(string message) {
		thoughtText.text = message;
		thoughtText.Commit();
		thoughtBubble.gameObject.SetActive(true);
	}
	
	public void hideThought() {
		thoughtBubble.gameObject.SetActive(false);
	}
}
