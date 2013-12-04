using UnityEngine;
using System.Collections;

public class ImageCycler : MonoBehaviour {
	
	public Material[] intro_sec = new Material[4];
	public string NextLevelName;
	public Transform loading;
	
	private int index = 0;
	private float timer = 0;
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetButtonDown("Fire1")) {
			change_image_delay();
		}
		if(Input.GetButtonDown("Fire3")) {
			loading.Translate(0, 0, -2);
			Application.LoadLevel(NextLevelName);
		}
	
	}
	
	void change_image_delay() {
		Invoke("change_image", timer);
	}
	
	void change_image() {
		index++;
		if (index >= intro_sec.Length)
		{
			loading.Translate(0, 0, -2);
			Application.LoadLevel(NextLevelName);
		}
		else
			renderer.material = intro_sec[index];
		
	}
}
