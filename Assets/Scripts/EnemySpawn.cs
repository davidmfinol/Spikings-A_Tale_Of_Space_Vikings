using UnityEngine;
using System.Collections;

public class EnemySpawn : MonoBehaviour {
	
	public Transform enemyPrefab;
	public int TimeBetweenSpawns = 2;
	
	private float time = 0;
	
	// Update is called once per frame
	void Update ()
	{
		time+= Time.deltaTime;
		if(time > TimeBetweenSpawns)
		{
			Instantiate(enemyPrefab, transform.position, enemyPrefab.rotation);
			time = 0;
		}
	}
}
