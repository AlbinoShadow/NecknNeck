using UnityEngine;
using System.Collections;

public class MazeGeneration : MonoBehaviour {
	public int mazeLength;
	public int mazeWidth;
	public int startX = 1, startY = 1;
	public int seed;
	public float scale = 0.3f;
	public Object wall;
	public Object path;
	public Object startPoint;
	public Object endPoint;

	private Transform Walls;
	private Transform Paths;
	private int[,] maze;
	private int totalCells;
	private int visitedCells;
	private int maxLength;
	private int maxWidth;
	private float largestX;
    private float largestY;
    private Randomizer randInfo;

	void initMaze(int[,] maze) {
		for(int i = 0; i < maxLength; i++){
			for(int j = 0; j < maxWidth; j++){
				if(i % 2 == 0 || j % 2 == 0)
					maze[i,j] = (int)status.WALL;
				else
					maze[i,j] = (int)status.PATH;
			}
		}
	}

	bool isClosed(int[,] maze, int y, int x) {
		if(maze[y - 1,x] == (int)status.WALL
		   && maze[y,x - 1] == (int)status.WALL
		   && maze[y,x + 1] == (int)status.WALL
		   && maze[y + 1,x] == (int)status.WALL)
			return true;
		return false;
	}

	void mazeAlgorithm(int[,] maze) {
		int nextX = startX;
		int nextY = startY;
		int indexMove = 0; //Move # basically.a
		int neighbourValid;
		int[] neighbourValidX = new int[4];
		int[] neighbourValidY = new int[4];
		int[] indexLocX = new int[totalCells + 1];
		int[] indexLocY = new int[totalCells + 1];
		Random.seed = seed;
		int[] possStep = new int[4];
		int randStep;
		int randomVal;
		int changeRandVal;

		//Variables for finding furthest point.
		int mazeWeightCount = 0;
		int highMazeCount = mazeWeightCount;
		int[,] mazeWeight = new int[maxLength,maxWidth];

		visitedCells = 1;

		while (visitedCells < totalCells){
			neighbourValid = -1;
			if(nextY - 2 > 0 && isClosed(maze, nextY - 2, nextX)) { //Down
				neighbourValid++;
				neighbourValidY[neighbourValid] = nextY - 2;
				neighbourValidX[neighbourValid] = nextX;
				possStep[neighbourValid] = 1;
			}
			if(nextX - 2 > 0 && isClosed(maze, nextY, nextX - 2)) { //Left
				neighbourValid++;
				neighbourValidY[neighbourValid] = nextY;
				neighbourValidX[neighbourValid] = nextX - 2;
				possStep[neighbourValid] = 2;
			}
			if(nextX + 2 < maxWidth && isClosed(maze, nextY, nextX + 2)) { //Right
				neighbourValid++;
				neighbourValidY[neighbourValid] = nextY;
				neighbourValidX[neighbourValid] = nextX + 2;
				possStep[neighbourValid] = 3;
			}
			if(nextY + 2 < maxLength && isClosed(maze, nextY + 2, nextX)) { //Up
				neighbourValid++;
				neighbourValidY[neighbourValid] = nextY + 2;
				neighbourValidX[neighbourValid] = nextX;
				possStep[neighbourValid] = 4;
			}

			if(neighbourValid == -1){
				nextY = indexLocY[indexMove];
				nextX = indexLocX[indexMove];
				mazeWeightCount = mazeWeight[nextY,nextX];
				indexMove--;
			}
			if(neighbourValid != -1){
				changeRandVal = Random.Range(0, 101);
				Random.seed += seed;
				randomVal = Random.Range(0, (neighbourValid + 1)); //Max is exclusive.
				Random.seed -= seed;
				nextX = neighbourValidX[randomVal];
				nextY = neighbourValidY[randomVal];
				randStep = possStep[randomVal];

				if(randStep == 1){
					maze[nextY + 1, nextX] = (int)status.PATH;
				}
				else if(randStep == 2){
					maze[nextY, nextX + 1] = (int)status.PATH;
				}
				else if(randStep == 3){
					maze[nextY, nextX - 1] = (int)status.PATH;
				}
				else if(randStep == 4){
					maze[nextY - 1, nextX] = (int)status.PATH;
				}
				mazeWeightCount++;
				mazeWeight[nextY, nextX] = mazeWeightCount;

				if(mazeWeightCount >= highMazeCount){
					highMazeCount = mazeWeightCount;
					largestX = nextX;
					largestY = nextY;
				}

				indexMove++;
				indexLocX[indexMove] = nextX;
				indexLocY[indexMove] = nextY;
				visitedCells++;

				if(changeRandVal > 74){
					changeRandVal = Random.Range(0, indexMove);
					nextY = indexLocY[indexMove - changeRandVal];
					nextX = indexLocX[indexMove - changeRandVal];
					mazeWeightCount = mazeWeight[nextY,nextX];
					indexMove++;
					indexLocX[indexMove] = nextX;
					indexLocY[indexMove] = nextY;
				}
			}
		}
	}

    void instantiateMaze(int[,] maze) {
        Paths = transform.GetChild(0).GetComponent<Transform>();
        Walls = transform.GetChild(1).GetComponent<Transform>();
        wall = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Game Assets/Prefabs/Objects/WallObject.prefab",typeof(Object));;
        path = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Game Assets/Prefabs/Objects/PathObject.prefab",typeof(Object));;
        endPoint = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Game Assets/Prefabs/Objects/End Point.prefab",typeof(Object));;
        startPoint = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Game Assets/Prefabs/Objects/Start Point.prefab",typeof(Object));;

		Vector3 placerLoc = new Vector3(0,0);
		Vector3 reset = new Vector3(maxWidth,0,0);
		GameObject pathing = (GameObject)Instantiate(path, placerLoc, Quaternion.identity);
		pathing.transform.parent = Paths;
		pathing.transform.localScale = new Vector3(maxWidth - 1, maxLength - 1);
		pathing.transform.localPosition = new Vector3(maxWidth/2,maxLength/2);
		for(int i = 0; i < maxLength; i++){
			for(int j = 0; j < maxWidth; j++){
				//Starts at [0, 0] which is bottom left.
				if(maze[i,j] == (int)status.WALL){
					GameObject tile = (GameObject)Instantiate(wall, placerLoc, Quaternion.identity);
					tile.transform.parent = Walls;
					tile.transform.localScale = new Vector3(scale, scale, scale);
				}
				placerLoc += Vector3.right;
			}
			placerLoc += Vector3.up;
			placerLoc -= reset;
		}
		GameObject start = (GameObject)Instantiate(startPoint,new Vector2(startX,startY),Quaternion.identity);
		GameObject end = (GameObject)Instantiate(endPoint,new Vector2(largestX,largestY),Quaternion.identity);
		start.transform.parent = Paths;
		end.transform.parent = Paths;
	}

	public int[,] getMaze(){
		return maze;
	}

	// Use this for initialization
    void Awake () {
        randInfo = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<Randomizer>();
        mazeLength = randInfo.mazeLength;
        mazeWidth = randInfo.mazeWidth;
        seed = randInfo.mazeSeed;

		maxLength = mazeLength * 2 + 1;
		maxWidth = mazeWidth * 2 + 1;
		totalCells = mazeLength * mazeWidth;
		maze = new int[maxLength,maxWidth];
		initMaze(maze);
		mazeAlgorithm(maze);
		instantiateMaze(maze);
	}
}
