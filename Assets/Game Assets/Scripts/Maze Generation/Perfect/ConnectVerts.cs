using UnityEngine;
using System.Collections.Generic;

public enum direction
{
    up,
    right
}
;

public class ConnectVerts : MonoBehaviour
{

    private MazeGeneration mazeFile;
    private float scale;
    private int maxLength;
    private int maxWidth;
    private int count = 0;
    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    //private List<Vector2> newUV = new List<Vector2>(); //Irrelevant until artists are acquired.

    private Mesh mesh; //This is the final result.
    private Vector3 topLeft = new Vector3(0,0);
    private Vector3 topRight = new Vector3(0,0);
    private Vector3 botRight = new Vector3(0,0);

    public void createConnectMesh(direction dir)
    {
        float scaleX = 0.5f;
        float scaleY = 0.5f;
        float scaler = (1 / (scale / 2) - 1);

        //Work clockwise with meshes.
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for(int i = 0;i < triangles.Length;i++)
            newTriangles.Add(triangles[i]);
        for(int i = 0;i < vertices.Length;i++)
            newVertices.Add(vertices[i]);

        if(dir == direction.up)
        {
            topLeft = new Vector3(-scaleX,scaleY * scaler);
            topRight = new Vector3(scaleX,scaleY * scaler);
                
            newVertices.Add(topLeft);   //Top Left
            newVertices.Add(topRight);  //Top Right
            newTriangles.Add(4 + (count * 2));
            newTriangles.Add(5 + (count * 2));
            newTriangles.Add(3);
            newTriangles.Add(5 + (count * 2));
            newTriangles.Add(1);
            newTriangles.Add(3);
        }
        else if(dir == direction.right)
        {
            topRight = new Vector3(scaleX * scaler,scaleY);
            botRight = new Vector3(scaleX * scaler,-scaleY);
                
            newVertices.Add(topRight);  //Top Right
            newVertices.Add(botRight);  //Bottom Right
            newTriangles.Add(1);
            newTriangles.Add(4 + (count * 2));
            newTriangles.Add(2);
            newTriangles.Add(4 + (count * 2));
            newTriangles.Add(5 + (count * 2));
            newTriangles.Add(2);
        }

        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();
        newTriangles.Clear();
        newVertices.Clear();
        count++;
    }

    bool isValidWall(int[,] maze,int x,int y)
    {
        mazeFile = GameObject.FindGameObjectWithTag(Tags.maze).GetComponent<MazeGeneration>();
        if(maze[y,x] == 0)//0 is for wall, 1 is for path
            return true;
        return false;
    }

    void connectWalls(int[,] maze)
    {
        mazeFile = GameObject.FindGameObjectWithTag(Tags.maze).GetComponent<MazeGeneration>();
        int maxLength = mazeFile.mazeLength * 2 + 1;
        int maxWidth = mazeFile.mazeWidth * 2 + 1;
        int j = (int)this.transform.position.y;
        int i = (int)this.transform.position.x;

        //You don't need down or left functions because you can clear everything with just up and right.
        if(j + 1 < maxLength && isValidWall(maze,i,(j + 1)))
        {
            createConnectMesh(direction.up);
        }
        if(i + 1 < maxWidth && isValidWall(maze,(i + 1),j))
        {
            createConnectMesh(direction.right);
        }
    }

    // Use this for initialization
    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        mazeFile = GameObject.FindGameObjectWithTag(Tags.maze).GetComponent<MazeGeneration>();
        scale = mazeFile.scale;
        int[,] mazeData = mazeFile.getMaze();
        connectWalls(mazeData);
        if(mesh.vertices.Length > 6)
        {
            gameObject.AddComponent<PolygonCollider2D>();
            PolygonCollider2D polyColl = gameObject.GetComponent<PolygonCollider2D>();
            List<Vector2> verts = new List<Vector2>();
            verts.Add(mesh.vertices[4]);
            verts.Add(mesh.vertices[5]);
            verts.Add(mesh.vertices[1]);
            verts.Add(mesh.vertices[6]);
            verts.Add(mesh.vertices[7]);
            verts.Add(mesh.vertices[0]);
            polyColl.SetPath(0,verts.ToArray());
        }
        else
            gameObject.AddComponent<BoxCollider2D>();
    }
}
