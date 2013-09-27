using UnityEngine;
using Pathfinding;
using System.Collections;

public class BearScript : CharacterScript {
	
    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 5;
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
		player = (PlayerScript) FindObjectOfType(typeof(PlayerScript));
	}
	
	override protected void Update () {
		base.Update();
		timeSinceLastPath += Time.deltaTime;
		
		// First, make sure we have an up-to-date path to the player
		if((currentPath == null || timeSinceLastPath > timeToRepath) && !searchingForPath) {
			seeker.StartPath(transform.position, player.transform.position, OnPathComplete);
			searchingForPath = true;
		}
		if(currentPath == null)
		{
			//Debug.Log("No Path");
			return;
		}
		
		DoMovement();
		
		// TODO LATER: MAKE THE BEAR ATTACK
		//spawnHitBox(team);
	}
	
	public void OnPathComplete(Path p)
	{
        if (!p.error) {
			//Debug.Log ("path complete");
            currentPath = p;
			searchingForPath = false;
			timeSinceLastPath = 0;
            currentWaypoint = 0;
        }
	}
	
	private void DoMovement()
	{
        if (currentWaypoint >= currentPath.vectorPath.Count) {
			currentPath = null;
            //Debug.Log ("End Of Path Reached");
            return;
        }
        
        //Direction to the next waypoint
		Vector3 targetLocation = currentPath.vectorPath[currentWaypoint];
		targetLocation.y = transform.position.y;
        Vector3 dir = (targetLocation-transform.position).normalized;
		
		processInput(dir.x, dir.z);
		playAnimation();
		
        dir *= speed * Time.deltaTime;
		
		//Debug.Log(currentWaypoint);
		//Debug.Log("We are going to move: " + dir);
        controller.Move(dir);
		//Debug.Log("Current location: " + transform.position);
		//Debug.Log("Waypoint location: " + targetLocation);
        
        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector3.Distance (transform.position, targetLocation) < nextWaypointDistance) {
            //Debug.Log("moving to next");
			currentWaypoint++;
            return;
        }
	}
}
