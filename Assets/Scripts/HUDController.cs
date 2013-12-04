using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {
	
    public Transform healthCircle;
	
    public Transform Hammer;
	public Transform Part1;
	public Transform Part2;
    public Transform Part3;

	public Transform thoughtBubble;
	public tk2dTextMesh thoughtText;

	private int currentThoughtID = 0;
	
    void Update() {
		// Health
		PlayerScript player = GameManager.Instance.Player;
		healthCircle.rotation = Quaternion.AngleAxis(Mathf.Min((1.0f-((float)player.currentHealth)/player.maxHealth) * 180, 180), Vector3.forward);

		// Inventory
        Hammer.gameObject.SetActive(player.powers % 2 == 1);
        Part2.gameObject.SetActive( GameManager.Instance.partsCollected > 0 );
        Part3.gameObject.SetActive( GameManager.Instance.partsCollected > 1 );
        Part1.gameObject.SetActive( GameManager.Instance.partsCollected > 2 );
		
	}
	
	public void showThought(string message, int thoughtID) {
		currentThoughtID = thoughtID;
		thoughtBubble.gameObject.SetActive(true);
		StartCoroutine("showThoughtBubble", message);
	}

	IEnumerator showThoughtBubble(string message) {
		StopCoroutine("FadeThought");
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
		StopCoroutine("showThoughtBubble");
	}
	
	public void hideThought(int thoughtID = 0) {
		if(thoughtID > 0 && currentThoughtID != thoughtID)
			return;
		
		thoughtText.gameObject.SetActive(false);
		StartCoroutine("FadeThought");
	}

	IEnumerator FadeThought() {
		StopCoroutine("showThoughtBubble");
		thoughtBubble.GetComponent<tk2dSpriteAnimator>().Play("BubbleFadeOut");
		float t = 0;
		while (t < 2)
		{
			t+= Time.deltaTime;
			yield return null;
		}
		thoughtBubble.gameObject.SetActive(false);
		StopCoroutine("FadeThought");
	}
}
