using UnityEngine;
using Pathfinding;
using System.Collections;

public class BearScript : CharacterScript {
	
	public float timeToRepath = 1;
	public float speed = 500;
	
	// A* Pathfinding Variables
	private PlayerScript player;
	private Seeker seeker;
	private Path currentPath;
	private int currentWaypoint;
	private bool searchingForPath;
	private float timeSinceLastPath;
	
	override protected void Start () {
		base.Start();
		team = (int) TEAMS.ENEMY;
		seeker = GetComponent<Seeker>();
	}
	
	override protected void Update () {
		base.Update();
		timeSinceLastPath += Time.deltaTime;
		
		// First, make sure we have an up-to-date path to the player
		if((currentPath == null || timeSinceLastPath > timeToRepath) && !searchingForPath) {
			seeker.StartPath(collider.transform.position, GameManager.Instance.Player.collider.transform.position, OnPathComplete);
			searchingForPath = true;
		}
		if(currentPath == null) {
			//Debug.Log("No Path");
			return;
		}
		
		if ((GameManager.Instance.Player.transform.position - transform.position).magnitude <= 200) {
			attack();
		}
		
		DoMovement();
	}
	
	override protected void OnDeath ()
	{
		Destroy(gameObject, 0.4f);
	}
	
	public void OnPathComplete(Path p) {
        if (!p.error) {
			//Debug.Log ("path complete");
            currentPath = p;
			timeSinceLastPath = 0;
            currentWaypoint = 0;
        }
		searchingForPath = false;
	}
	
	private void DoMovement() {
		Node endNode = currentPath.path[currentPath.path.Count-1];
        if (currentWaypoint >= currentPath.vectorPath.Count || !endNode.walkable) {
			currentPath = null;
            //Debug.Log ("End Of Path Reached");
            return;
        }
		
        //Direction to the next waypoint
		Vector3 targetLocation = currentPath.vectorPath[currentWaypoint];
		targetLocation.y = collider.transform.position.y;
        Vector3 dir = (targetLocation-collider.transform.position).normalized;
		
		move (dir.x * speed, dir.z * speed);
		//Debug.Log("Current location: " + transform.position);
		//Debug.Log("Waypoint location: " + targetLocation);
        
        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (collider.bounds.Contains(currentPath.vectorPath[currentWaypoint])) {
            //Debug.Log("moving to next");
			currentWaypoint++;
            return;
        }
	}
}
