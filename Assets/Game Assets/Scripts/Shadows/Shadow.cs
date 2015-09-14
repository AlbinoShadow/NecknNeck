using UnityEngine;
using System.Collections.Generic;

public class Shadow : MonoBehaviour {
	public string searchTag = "Wall";
	public float searchInterval = 0.5f; //In seconds
	public Transform player;
	
	private List<Vector3> collVerts = new List<Vector3>();
	private List<Vector3> emptyVerts = new List<Vector3>();
	private List<int> shadeTriangles = new List<int>();
	private Mesh mesh; //This is the final result.

	void scan(){
		getNearestTagObj();
		display();
	}

	void getNearestTagObj(){
		collVerts.Clear();
		collVerts.Add(transform.InverseTransformPoint(player.position));
		GameObject[] taggedWalls = GameObject.FindGameObjectsWithTag(searchTag);
		float range = 5.0f;

		for(int i = 0; i < taggedWalls.Length;i++) {
			BoxCollider2D box = null;
			PolygonCollider2D poly = null;
			Vector3 objPos = transform.InverseTransformPoint(taggedWalls[i].transform.position);
			float distanceSquared = (objPos - transform.InverseTransformPoint(player.position)).magnitude;

			if(distanceSquared < range){
				box = taggedWalls[i].GetComponent<BoxCollider2D>();
				if(box == null){
					poly = taggedWalls[i].GetComponent<PolygonCollider2D>();
					for(int j = 0; j < poly.points.Length; j++)
						collVerts.Add(transform.InverseTransformPoint(poly.points[j]));
				}
				else{
					float half = 0.5f;
					float offsetX = box.offset.x;
					float offsetY = box.offset.y;
					collVerts.Add(transform.InverseTransformPoint(new Vector2(-half * box.size.x + offsetX, -half * box.size.y + offsetY)));
					collVerts.Add(transform.InverseTransformPoint(new Vector2(-half * box.size.x + offsetX, half * box.size.y + offsetY)));
					collVerts.Add(transform.InverseTransformPoint(new Vector2(half * box.size.x + offsetX, half * box.size.y + offsetY)));
					collVerts.Add(transform.InverseTransformPoint(new Vector2(half * box.size.x + offsetX, -half * box.size.y + offsetY)));
				}
			}
		}
	}

	void setVerts(){
		for(int i = 1;i < collVerts.Count;i++) {
			RaycastHit2D hit = Physics2D.Raycast(player.position, transform.InverseTransformPoint(collVerts[i] - transform.position), Mathf.Infinity, 9);
			if(hit.collider != null){
				//collVerts[i] = hit.point;
			}
		}
	}

	void setTris(){
		int tri1 = 0;
		int tri2 = 1;
		int tri3 = 2;
		for(int i = 0;i < collVerts.Count - 2;i++) {
			shadeTriangles.Add(tri2);
			shadeTriangles.Add(tri3);
			shadeTriangles.Add(tri1);
			tri2++;
			tri3++;
		}
	}

	void display(){
		mesh.Clear();
		mesh.vertices = emptyVerts.ToArray();
		setVerts();
		setTris();
		mesh.vertices = collVerts.ToArray();
		mesh.triangles = shadeTriangles.ToArray();
		mesh.Optimize();
		mesh.RecalculateNormals();
		shadeTriangles.Clear();
	}

	// Use this for initialization
	void Start () {
		mesh = GetComponent<MeshFilter>().mesh;
		InvokeRepeating("scan", 1, searchInterval);
		//InvokeRepeating("display", 1, 0.1f);

	}
	
	// Update is called once per frame
	void Update () {

	}
}
