using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {
	
    public tk2dUIProgressBar currentHealth;
    public tk2dUIProgressBar losingHealth;
	public float smoothTime = 0.5f;
    private float healthBarVelocity = 0.0f;
	
	void Start () {
		PlayerScript player = GameManager.Instance.Player;
		currentHealth.Value = ((float)player.currentHealth)/player.maxHealth;
		losingHealth.Value = ((float)player.currentHealth)/player.maxHealth;
	}
	
    void Update() {
		PlayerScript player = GameManager.Instance.Player;
        losingHealth.Value = Mathf.SmoothDamp( losingHealth.Value, ((float)player.currentHealth)/player.maxHealth, ref healthBarVelocity, smoothTime, Mathf.Infinity, tk2dUITime.deltaTime );
    	currentHealth.Value = ((float)player.currentHealth)/player.maxHealth;
	}
}
