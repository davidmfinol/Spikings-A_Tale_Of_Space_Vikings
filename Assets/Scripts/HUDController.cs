using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {

    public Transform healthCircle;
	public Transform playerFace;
	public int numberOfHitsWaiting = 0;
	private bool isFlashingRed;

    public Transform Hammer;
	public Transform Part1;
	public Transform Part2;
    public Transform Part3;

	public Transform thoughtBubble;
	public tk2dTextMesh thoughtText;
	private int currentThoughtID = 0;

	public Transform titleCard;
	public Transform controls;

    void Update() {
		// Health
		PlayerScript player = GameManager.Instance.Player;
		healthCircle.rotation = Quaternion.AngleAxis(Mathf.Min(Mathf.Max((1.0f-((float)player.currentHealth)/player.maxHealth) * -180, -180), 0), Vector3.forward);
		if(numberOfHitsWaiting > 0 && !isFlashingRed)
			StartCoroutine("FlashRed");

		// Inventory
        Hammer.gameObject.SetActive(player.powers % 2 == 1);
        Part1.gameObject.SetActive( GameManager.Instance.partsCollected > 0 );
        Part2.gameObject.SetActive( GameManager.Instance.partsCollected > 1 );
        Part3.gameObject.SetActive( GameManager.Instance.partsCollected > 2 );

		controls.gameObject.SetActive(Input.GetKey(KeyCode.Space));

        if (Input.GetKeyDown(KeyCode.Escape)) {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
	}

	IEnumerator FlashRed() {
		isFlashingRed = true;

		TurnRed();
		yield return new WaitForSeconds(0.2f);
		TurnWhite();
		yield return new WaitForSeconds(0.2f);
		TurnRed();
		yield return new WaitForSeconds(0.2f);
		TurnWhite();
		yield return new WaitForSeconds(0.2f);
		TurnRed();
		yield return new WaitForSeconds(0.2f);
		TurnWhite();
		yield return new WaitForSeconds(0.2f);

		isFlashingRed = false;
		numberOfHitsWaiting--;
		StopCoroutine("FlashRed");
	}

	void TurnRed() {
		playerFace.GetComponent<tk2dSprite>().color = Color.red;
		healthCircle.GetComponent<tk2dSprite>().color = Color.red;
		Hammer.GetComponent<tk2dSprite>().color = Color.red;
		Part1.GetComponent<tk2dSprite>().color = Color.red;
		Part2.GetComponent<tk2dSprite>().color = Color.red;
		Part3.GetComponent<tk2dSprite>().color = Color.red;
	}

	void TurnWhite() {
		playerFace.GetComponent<tk2dSprite>().color = Color.white;
		healthCircle.GetComponent<tk2dSprite>().color = Color.white;
		Hammer.GetComponent<tk2dSprite>().color = Color.white;
		Part1.GetComponent<tk2dSprite>().color = Color.white;
		Part2.GetComponent<tk2dSprite>().color = Color.white;
		Part3.GetComponent<tk2dSprite>().color = Color.white;
	}

	public void showThought(string message, int thoughtID) {
		currentThoughtID = thoughtID;
		thoughtBubble.gameObject.SetActive(true);
		StartCoroutine("showThoughtBubble", message);
	}

	IEnumerator showThoughtBubble(string message) {
		StopCoroutine("FadeThought");
		thoughtText.gameObject.SetActive(false);
		thoughtBubble.GetComponent<tk2dSpriteAnimator>().Play("BubbleFadeIn");
		yield return new WaitForSeconds(2);

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
