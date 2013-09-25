using UnityEngine;
using Pathfinding;
using System.Collections;

public class BearScript : CharacterScript {
	
	public float speed = 100;
    
    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3;
	
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
		if((currentPath == null || timeSinceLastPath > 2) && !searchingForPath) {
			seeker.StartPath(transform.position, player.transform.position, OnPathComplete);
			searchingForPath = true;
		}
		if(currentPath == null)
		{
			Debug.Log("No Path");
			return;
		}
		
		Debug.Log ((currentPath.vectorPath));
		
		DoMovement();
		
		// TODO LATER: MAKE THE BEAR ATTACK
		spawnHitBox(team);
	}
	
	public void OnPathComplete(Path p)
	{
        if (!p.error) {
            currentPath = p;
			searchingForPath = false;
			timeSinceLastPath = 0;
            currentWaypoint = 0;
        }
	}
	
	private void DoMovement()
	{
        if (currentWaypoint >= currentPath.vectorPath.Count) {
            Debug.Log ("End Of Path Reached");
            return;
        }
        
        //Direction to the next waypoint
        Vector3 dir = (currentPath.vectorPath[currentWaypoint]-transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;
        controller.Move (dir);
        
        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector3.Distance (transform.position,currentPath.vectorPath[currentWaypoint]) < nextWaypointDistance) {
            currentWaypoint++;
            return;
        }
	}
}
