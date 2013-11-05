using UnityEngine;
using System.Collections;

public class ImageCycler : MonoBehaviour {
	public Material[] intro_sec = new Material[4];
	int index = 0;
	
	// Use this for initialization
	void Start () {
	
	}
	
	float timer = 0;
	
	// Update is called once per frame
	void Update () {
//		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        if (Physics.Raycast(ray, 100))
//            print("Hit this box thing");
		if(Input.GetKeyDown("f") || Input.GetKeyDown ("q")) {
			change_image_delay();
		}
		if(Input.GetKeyDown("space")) {
			Application.LoadLevel("CorbenSpace");
		}
	
	}
	
	void change_image_delay() {
		Invoke("change_image", timer);
	}
	
	void change_image() {
		index++;
		if (index >= intro_sec.Length)
			Application.LoadLevel("CorbenSpace");
		else
			renderer.material = intro_sec[index];
		
	}
}
