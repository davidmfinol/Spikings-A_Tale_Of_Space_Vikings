using UnityEngine;
using System.Collections;

public class DestroyedByMelee : MonoBehaviour
{
	
	private void OnCollisionEnter(Collision collision)
	{
		
		print ("collision.collider.gameobject is " + collision.collider.gameObject);
		
		if(collision.collider.gameObject.GetComponent<DelayedDestroy>()!= null)
		{
			Destroy(gameObject);	
		}
			
	}
	
}
