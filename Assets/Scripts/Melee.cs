using UnityEngine;
using System.Collections;

public class Melee : MonoBehaviour {
	
	public GameObject damageSquare;
	
	public ArrayList list= new ArrayList();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown(KeyCode.Z))
		{
			Vector3 pos = transform.position;
			pos.x = pos.x + 1;
		list.Add(Instantiate (damageSquare, pos ,transform.rotation));
			
		
			
		
		}
	//	foreach(GameObject obj in list){
	//	Destroy(obj);	
	//	}
	
	}
}
