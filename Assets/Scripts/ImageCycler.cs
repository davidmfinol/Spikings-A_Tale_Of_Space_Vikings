using UnityEngine;
using UnityEngine.SceneManagement;

public class ImageCycler : MonoBehaviour {

	public Material[] intro_sec = new Material[4];
	public Transform loading;

	private int index = 0;
	private float timer = 0;
    private bool sceneLoading = false;

	void Update () {

		if(Input.GetButtonDown("Fire1")) {
			change_image_delay();
		}
		if(Input.GetButtonDown("Fire3") && !sceneLoading) {
			loading.Translate(0, 0, -2);
			SceneManager.LoadScene(1);
            sceneLoading = true;
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
            SceneManager.LoadScene(1);
            sceneLoading = true;
        }
		else
			GetComponent<Renderer>().material = intro_sec[index];

	}
}
