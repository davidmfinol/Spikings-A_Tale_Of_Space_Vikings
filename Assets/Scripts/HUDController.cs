using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {
	
    public tk2dUIProgressBar currentHealth;
    public tk2dUIProgressBar losingHealth;
	public float healthBarSmoothTime = 0.5f;
	
    public Transform Hammer;
    public Transform Part1Collected;
    public Transform Part1Missing;
    public Transform Part2Collected;
    public Transform Part2Missing;
    public Transform Part3Collected;
    public Transform Part3Missing;

	public Transform thoughtBubble;
	public tk2dTextMesh thoughtText;

	private int currentThoughtID = 0;
	
    private float healthBarVelocity = 0.0f;
	
	void Start () {
		PlayerScript player = GameManager.Instance.Player;
		currentHealth.Value = ((float)player.currentHealth)/player.maxHealth;
		losingHealth.Value = ((float)player.currentHealth)/player.maxHealth;
	}
	
    void Update() {
		// Health
		PlayerScript player = GameManager.Instance.Player;
        losingHealth.Value = Mathf.SmoothDamp( losingHealth.Value, ((float)player.currentHealth)/player.maxHealth, ref healthBarVelocity, healthBarSmoothTime, Mathf.Infinity, tk2dUITime.deltaTime );
    	currentHealth.Value = ((float)player.currentHealth)/player.maxHealth;

		// Inventory
        Hammer.gameObject.SetActive(player.powers % 2 == 1);
        Part1Collected.gameObject.SetActive( GameManager.Instance.partsCollected > 0 );
        Part1Missing.gameObject.SetActive( !(GameManager.Instance.partsCollected > 0) );
        Part2Collected.gameObject.SetActive( GameManager.Instance.partsCollected > 1 );
        Part2Missing.gameObject.SetActive( !(GameManager.Instance.partsCollected > 1) );
        Part3Collected.gameObject.SetActive( GameManager.Instance.partsCollected > 2 );
        Part3Missing.gameObject.SetActive( !(GameManager.Instance.partsCollected > 2) );
		
	}
	
	public void showThought(string message, int thoughtID) {
		currentThoughtID = thoughtID;
		thoughtBubble.gameObject.SetActive(true);
		StartCoroutine("showThoughtBubble", message);
	}

	IEnumerator showThoughtBubble(string message) {
		if(!thoughtText.gameObject.activeSelf) {
			thoughtBubble.GetComponent<tk2dSpriteAnimator>().Play("BubbleFadeIn");
			float t = 0;
			while (t < 2)
			{
				t+= Time.deltaTime;
	            yield return null;
	        }
		}
		thoughtText.gameObject.SetActive(true);
		thoughtText.color = Color.black;
		thoughtText.text = message;
		thoughtText.Commit();
	}
	
	public void hideThought(int thoughtID = 0) {
		if(thoughtID > 0 && currentThoughtID != thoughtID)
			return;
		
		thoughtText.gameObject.SetActive(false);
		StartCoroutine("FadeThought");
	}

	IEnumerator FadeThought() {
		thoughtBubble.GetComponent<tk2dSpriteAnimator>().Play("BubbleFadeOut");
		float t = 0;
		while (t < 2)
		{
			t+= Time.deltaTime;
			yield return null;
		}
        thoughtBubble.gameObject.SetActive(false);
	}
}
