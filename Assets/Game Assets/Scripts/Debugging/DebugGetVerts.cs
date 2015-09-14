using UnityEngine;
using System.Collections;

public class DebugGetVerts : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		print(vertices.ToString());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
