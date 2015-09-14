using UnityEngine;
using System.Collections;

public class MergeMesh : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Transform walls = transform.GetChild(2);
		MeshFilter[] meshFilters = walls.GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		for(int i = 0;i < meshFilters.Length;i++) {
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
		}
		Mesh mesh = transform.GetComponent<MeshFilter>().mesh;
		mesh.CombineMeshes(combine);
		transform.gameObject.SetActive(true);
		mesh.Optimize();
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
