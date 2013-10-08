using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {
	
    public tk2dUIProgressBar healthBar;
	public float smoothTime = 0.5f;
    private float healthBarVelocity = 0.0f;
	
    void Update() {
		PlayerScript player = GameManager.Instance.Player;
        healthBar.Value = Mathf.SmoothDamp( healthBar.Value, ((float)player.currentHealth)/player.maxHealth, ref healthBarVelocity, smoothTime, Mathf.Infinity, tk2dUITime.deltaTime );
    }
}
