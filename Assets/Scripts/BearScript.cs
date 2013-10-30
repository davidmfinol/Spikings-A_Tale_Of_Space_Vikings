using UnityEngine;
using Pathfinding;
using System.Collections;

public class BearScript : CharacterScript {
	
	public enum PatrolTypes : int {
		Square = 0,
		Vertical = 1,
		Horizontal = 2,
		Randomly = 3
	}
	
	public float timeToRepath = 1;
	public float speed = 500;
	public float noticeRadius = 500;
	public PatrolTypes patrolType;
	private int hitObstacleCount = 0;
	private float lastHit = 0;
	private float lastChange = 0;
	private System.Random randomGen;
	
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
		randomGen = new System.Random();
		currentPath = null;
	}
	
	override protected void Update () {
		base.Update();
		timeSinceLastPath += Time.deltaTime;
		
		if (isInAttackRange() && currentPath != null)
			attack();
		
		if(isInNoticeRange())
			moveAStar();
		else
			movePatrol();
	}
	
	override protected void OnDeath () {
		anima = (int) (ANIMATIONS.DIE);
		playAnimation();
		Destroy(gameObject, 0.95f);
	}
	
	override protected void processInput(float x, float z) {
		if(Mathf.Abs(x) > Mathf.Abs(z)) {
			if(x > 0)
				direction = (int) (DIRECTIONS.EAST);
			else if(x < 0)
				direction = (int) (DIRECTIONS.WEST);
		}
		else if(Mathf.Abs(x) < Mathf.Abs(z)) {
			if(z > 0)
				direction = (int) (DIRECTIONS.NORTH);
			else if(z < 0)
				direction = (int) (DIRECTIONS.SOUTH);
		}
		anima = 1;
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
	
	private void moveAStar() {
		
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
		move (dir.x * speed, dir.z * speed);
		//Debug.Log("Current location: " + transform.position);
		//Debug.Log("Waypoint location: " + targetLocation);
        
        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
		Bounds bounds = collider.bounds;
		Vector3 boundsCenter = bounds.center;
		boundsCenter.y = 0;
		bounds.center = boundsCenter;
        if (bounds.Contains(currentPath.vectorPath[currentWaypoint])) {
            //Debug.Log("moving to next");
			currentWaypoint++;
            return;
        }
	}
	
	private void movePatrol() {
		switch (patrolType) {
		case PatrolTypes.Randomly : 
			moveRandom();
			break;
		case PatrolTypes.Horizontal :
			moveHorizontal();
			break;
		case PatrolTypes.Vertical :
			moveVertical();
			break;
		case PatrolTypes.Square :
			moveSquare();
			break;
		default :
			moveSquare();
			break;
		}
	}
	
	private void moveSquare() {
		// Get the direction based off time
		float timeSplit = ((int)Time.timeSinceLevelLoad) % 8;
		Vector3 dir = Vector3.right;
		if(timeSplit >= 6)
			dir = Vector3.back;
		else if(timeSplit >= 4)
			dir = Vector3.left;
		else if(timeSplit >= 2)
			dir = Vector3.forward;
		
		// Then actually do the movement
		move (dir.x * speed, dir.z * speed);
	}
	
	private void moveVertical() {
		lastChange += Time.deltaTime;
		if(lastChange > 10) {
			hitObstacleCount++;
			lastChange = 0;
		}
		Vector3 dir = hitObstacleCount % 2 == 1 ? Vector3.forward : Vector3.back ;
		move (0, dir.z * speed);
	}
	
	private void moveHorizontal() {
		lastChange += Time.deltaTime;
		if(lastChange > 10) {
			hitObstacleCount++;
			lastChange = 0;
		}
		Vector3 dir = hitObstacleCount % 2 == 1 ? Vector3.left : Vector3.right ;
		move (dir.x * speed, 0);
	}
	
	private void moveRandom() {
		lastChange += Time.deltaTime;
		
		Vector3 dir = Vector3.right;
		if(direction == (int) (DIRECTIONS.SOUTH))
			dir = Vector3.back;
		else if(direction == (int) (DIRECTIONS.WEST))
			dir = Vector3.left;
		else if(direction == (int) (DIRECTIONS.NORTH))
			dir = Vector3.forward;
		
		if( lastChange > 2) {
			int randomNum = randomGen.Next(4);
			dir = Vector3.right;
			if(randomNum == 1)
				dir = Vector3.back;
			else if(randomNum == 2)
				dir = Vector3.left;
			else if(randomNum == 3)
				dir = Vector3.forward;
			lastChange = 0;
		}
		move (dir.x * speed, dir.z * speed);
	}
	
	private bool isInAttackRange() {
		Bounds hitArea = new Bounds(transform.position, new Vector3(150, 100, 150));
		if(direction == (int) (DIRECTIONS.EAST)) {
			hitArea = new Bounds(transform.position + new Vector3(167, 0, 0), new Vector3(150, 100, 150));
		} else if(direction == (int) (DIRECTIONS.NORTH)) {
			hitArea = new Bounds(transform.position + new Vector3(0, 0, 167), new Vector3(150, 100, 150));
			float tempX = hitArea.size.x;
			Vector3 tempSize = hitArea.size;
			tempSize.x = hitArea.size.z;
			tempSize.z = tempX;
			hitArea.size = tempSize;
		} else if(direction == (int) (DIRECTIONS.WEST)) {
			hitArea = new Bounds(transform.position + new Vector3(-167, 0, 0), new Vector3(150, 100, 150));
		} else if(direction == (int) (DIRECTIONS.SOUTH)) {
			hitArea = new Bounds(transform.position + new Vector3(0, 0, -167), new Vector3(150, 100, 150));
			float tempX = hitArea.size.x;
			Vector3 tempSize = hitArea.size;
			tempSize.x = hitArea.size.z;
			tempSize.z = tempX;
			hitArea.size = tempSize;
		}
		Debug.DrawLine(hitArea.center - new Vector3(hitArea.extents.x, -20, 0) + new Vector3(0, 20, hitArea.extents.z), hitArea.center + new Vector3(hitArea.extents.x, 20, 0) + new Vector3(0, 20, hitArea.extents.z));
		Debug.DrawLine(hitArea.center - new Vector3(hitArea.extents.x, -20, 0) + new Vector3(0, 20, hitArea.extents.z), hitArea.center - new Vector3(hitArea.extents.x, -20, 0) - new Vector3(0, -20, hitArea.extents.z));
		Debug.DrawLine(hitArea.center - new Vector3(hitArea.extents.x, -20, 0) - new Vector3(0, -20, hitArea.extents.z), hitArea.center + new Vector3(hitArea.extents.x, 20, 0) - new Vector3(0, -20, hitArea.extents.z));
		Debug.DrawLine(hitArea.center + new Vector3(hitArea.extents.x, 20, 0) + new Vector3(0, 20, hitArea.extents.z), hitArea.center + new Vector3(hitArea.extents.x, 20, 0) - new Vector3(0, -20, hitArea.extents.z));
		return hitArea.Contains(GameManager.Instance.Player.transform.position);
	}
	
	private bool isInNoticeRange() {
		Vector3 startPoint = transform.position;
		startPoint.y = 0;
		Vector3 endPoint = GameManager.Instance.Player.transform.position;
		endPoint.y = 0;
		return (endPoint - startPoint).magnitude <= noticeRadius;
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit) {
		if(Time.time - lastHit > 1) {
			hitObstacleCount++;
			lastHit = Time.time;
		}
	}
}
