using UnityEngine;
using Pathfinding;
using System.Collections;

[RequireComponent(typeof(AstarPath))]
public class Astar2DToolkit : MonoBehaviour {

	// Use this for initialization
	void Start () {
		tk2dTileMap mapData = (tk2dTileMap) FindObjectOfType(typeof(tk2dTileMap));
		
		// Get the tilemaps' collider layers' mesh
		Mesh mesh = mapData.Layers[0].GetChunk(0,0).colliderMesh;
		
		// Create a new mesh graph
		NavMeshGraph newMeshGroup = new NavMeshGraph();
		newMeshGroup.sourceMesh = mesh;
		newMeshGroup.rotation = new Vector3(45, 0, 0);
				
		// Add it
		//GetComponent<AstarPath>().astarData.AddGraph( newMeshGroup );
	}
	
}
