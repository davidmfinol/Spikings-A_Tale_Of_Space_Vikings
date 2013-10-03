using UnityEngine;
using System.Collections;

public class BearSpawn : MonoBehaviour {
	
	public Transform bearPrefab;
	public int TimeBetweenSpawns = 2;
	
	private float time = 0;
	
	// Update is called once per frame
	void Update ()
	{
		time+= Time.deltaTime;
		if(time > TimeBetweenSpawns)
		{
			Instantiate(bearPrefab, transform.position, transform.rotation);
			time = 0;
		}
	}
}
