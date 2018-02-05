using UnityEngine;
using Pathfinding;
using System.Collections;

public class BearScript : EnemyNPCScript {
		public AudioClip bear_death;
	
	override protected void Update() {
		base.Update();
		
		if(anima != (int) ANIMATIONS.ATTACK)
			return;
		
		// First, make sure we have an up-to-date path to the player
		if((currentPath == null || timeSinceLastPath > timeToRepath) && !searchingForPath) {
			Vector3 startPoint = transform.position;
			startPoint.y = 0;
			Vector3 endPoint = GameManager.Instance.Player.transform.position;
			endPoint.y = 0;
			seeker.StartPath(startPoint, endPoint, OnPathComplete);
			searchingForPath = true;
		}
		if(currentPath == null) {
			//Debug.Log("No Path");
			movePatrol();
			return;
		}
		
		// Then make sure the path is valid
		Node endNode = currentPath.path[currentPath.path.Count-1];
        if (currentWaypoint >= currentPath.vectorPath.Count || !endNode.walkable) {
			currentPath = null;
            //Debug.Log ("End Of Path Reached");
            return;
        }
		
        // Then get the direction to the next waypoint
		Vector3 targetLocation = currentPath.vectorPath[currentWaypoint];
        Vector3 dir = (targetLocation-transform.position).normalized;
		
		// Then actually do the movement
		Vector3 movement = new Vector3(dir.x * speed, 0, dir.z * speed);
		movement *= Time.deltaTime;
		controller.Move(movement);
		// Make sure we stay on y = 0
		Vector3 pos = transform.position;
		pos.y = 0;
		transform.position = pos;
        
        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
		Bounds bounds = GetComponent<Collider>().bounds;
		Vector3 boundsCenter = bounds.center;
		boundsCenter.y = 0;
		bounds.center = boundsCenter;
        if (bounds.Contains(currentPath.vectorPath[currentWaypoint])) {
            //Debug.Log("moving to next");
			currentWaypoint++;
            return;
        }
	}
	
	protected override bool isInAttackRange ()
	{
		return isInNoticeRange();
	}
	
	protected override void OnDeath() {
		base.OnDeath();
		//play Bear Death Sound
		GetComponent<AudioSource>().enabled = true;
		//this sound doesn't play either
		GetComponent<AudioSource>().PlayOneShot(bear_death);
	}
	
}