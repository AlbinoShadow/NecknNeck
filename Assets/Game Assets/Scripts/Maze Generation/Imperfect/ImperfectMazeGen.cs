/*
 * Name: ImperfectMazeGen.cs
 * 
 * Use: Generates an imperfect maze, when imperfect maze is defined as
 *      a maze that "splits" and is not one single path. It must use its
 *      own vertice connect code.
 * 
 * Method: It generates the maze via depth searching that occasionally
 *         moves backwards to create a different path, leading to various
 *         outward paths.
 */

using UnityEngine;
using System.Collections;

public enum status
{
    WALL,
    PATH,
    POWERUP
};

public class ImperfectMazeGen : MonoBehaviour
{
    // Public variables.
	public int mazeLength;              // Length of the maze (y-coordinate).
	public int mazeWidth;               // Width of the maze (x-coordinate).
	public int startX = 1, startY = 1;  // Starting coordinate.
	public int seed;                    // Seed used to generate the maze.
	public float scale = 0.3f;          // Size of the walls.

    // Private variables
    private Transform Walls;        // Used to select wall objects.
    private Transform Paths;        // Used to select path objects.
    private Transform Others;       // Used to select path objects.
	private int[,] maze;            // Coordinates of the maze (maze objects mapped to these coordinates).
	private int totalCells;         // Total "cells" in the maze.
	private int visitedCells;       // Amount of cells that have been visited/edited.
	private int maxLength;          // Max length (y-coordinate).
	private int maxWidth;           // Max width (x-coordinate).
	private float largestX;         // X-Coordinate furthest from the spawn point.
	private float largestY;         // Y-Coordinate furthest from the spawn point.
    private Randomizer randInfo;    // Used to select the randomizer script.
    private Initialization init;     // Used to select the script that initializes "things".

    // Used to find dead-ends.
    private int[] deadEndX;         // Used to store dead ends, for possible power-up locations and what not.
    private int[] deadEndY;         // Used to store dead ends, for possible power-up locations and what not.
    private int deadEndIndex = 0;   // Index number of given dead-ends.
	
    // Initializes the maze by creating a net of walls with paths being the individual cells.
	void initMaze(int[,] maze)
    {
        // Loop through the cells:
		for(int i = 0; i < maxLength; i++){
			for(int j = 0; j < maxWidth; j++){
                // Set every other coordinate as a wall, and the others as paths.
				if(i % 2 == 0 || j % 2 == 0)
					maze[i,j] = (int)status.WALL;
				else
					maze[i,j] = (int)status.PATH;
			}
		}
	}
	
    // Checks whether a point within the maze is a closed off cell or not.
    bool isClosed(int[,] maze, int y, int x)
    {
        if(maze[y - 1,x] == (int)status.WALL
           && maze[y,x - 1] == (int)status.WALL
           && maze[y,x + 1] == (int)status.WALL
           && maze[y + 1,x] == (int)status.WALL)
            return true;
        return false;
    }
    
    bool isDeadEnd(int[,] maze, int y, int x)
    {
        int count = 0;
        if(maze[y - 1,x] == (int)status.WALL)
            count++;
        if(maze[y,x - 1] == (int)status.WALL)
            count++;
        if(maze[y,x + 1] == (int)status.WALL)
            count++;
        if(maze[y + 1,x] == (int)status.WALL)
            count++;

        if(count == 3)
            return true;
        return false;
    }
	
    // Creates the coordinates of the maze based on recursive backtracking with a randomized backtrack 1/4 of the steps.
	void mazeAlgorithm(int[,] maze)
    {
		int nextX = startX;                             // Next x-coordinate, starting at the starting point.
		int nextY = startY;                             // Next y-coordinate, starting at the starting point.
		int indexMove = 0;                              // Move # basically.
		int neighbourValid;                             // Amount of neighbor cells that are valid.
		int[] neighbourValidX = new int[4];             // Array to store which sides are valid.
		int[] neighbourValidY = new int[4];             // Array to store which sides are valid.
		int[] indexLocX = new int[totalCells + 1];      // Used to index/backtrack where the generation has been.
        int[] indexLocY = new int[totalCells + 1];      // Used to index/backtrack where the generation has been.
		Random.seed = seed;                             // Sets the seed of the randomizer file to seed.
		int[] possStep = new int[4];                    // Stores possible steps.
		int randStep;                                   // Random step that's chosen.
        int randomVal;                                  // Random value that's calculated.
		int changeRandVal;                              // Change in the random value.
        bool[] indexVisited = new bool[totalCells * 2];   // Used to index what has been visited.
		
		//Variables for finding furthest point.
		int mazeWeightCount = 0;                            // Weight count to find furthest point from the start.
		int highMazeCount = mazeWeightCount;                // New "heaviest" weight.
		int[,] mazeWeight = new int[maxLength,maxWidth];    // Weighted coordinates in the maze.
		
        visitedCells = 1;       // Total amount of cells that have been edited/visited.
        indexLocX[0] = startX;  // Sets the first part of the index to the starting point.
        indexLocY[0] = startY;  // Sets the first part of the index to the starting point.

		while (visitedCells < totalCells)
        {
			neighbourValid = -1;    // Set it so no neighbors are valid.

            // Check if the cell below exists and is closed:
			if (nextY - 2 > 0 && isClosed(maze, nextY - 2, nextX))
            {
				neighbourValid++;                               // If so increase count of valid neighbours.
				neighbourValidY[neighbourValid] = nextY - 2;    // Set the y-coordinate of the valid neighbour.
                neighbourValidX[neighbourValid] = nextX;        // Set the x-coordinate of the valid neighbour.
				possStep[neighbourValid] = 1;                   // Set it as a possible step labeled "1".
			}

            // Check if the cell to the left exists and is closed:
			if (nextX - 2 > 0 && isClosed(maze, nextY, nextX - 2))
            {
                neighbourValid++;                               // If so increase count of valid neighbours.
                neighbourValidY[neighbourValid] = nextY;        // Set the y-coordinate of the valid neighbour.
                neighbourValidX[neighbourValid] = nextX - 2;    // Set the x-coordinate of the valid neighbour.
                possStep[neighbourValid] = 2;                   // Set it as a possible step labeled "1".
			}

            // Check if the cell to the right exists and is closed:
			if (nextX + 2 < maxWidth && isClosed(maze, nextY, nextX + 2))
            {
                neighbourValid++;                               // If so increase count of valid neighbours.
                neighbourValidY[neighbourValid] = nextY;        // Set the y-coordinate of the valid neighbour.
                neighbourValidX[neighbourValid] = nextX + 2;    // Set the x-coordinate of the valid neighbour.
                possStep[neighbourValid] = 3;                   // Set it as a possible step labeled "1".
			}

            // Check if the cell above exists and is closed:
			if (nextY + 2 < maxLength && isClosed(maze, nextY + 2, nextX))
            {
                neighbourValid++;                               // If so increase count of valid neighbours.
                neighbourValidY[neighbourValid] = nextY + 2;    // Set the y-coordinate of the valid neighbour.
                neighbourValidX[neighbourValid] = nextX;        // Set the x-coordinate of the valid neighbour.
                possStep[neighbourValid] = 4;                   // Set it as a possible step labeled "1".
			}
			
            // If there are no valid neighbours:
            if (neighbourValid == -1)
            {
                //if (indexVisited[indexMove] == false && indexVisited[indexMove - 1] == true)
                if(isDeadEnd(maze, nextY, nextX))
                {
                    deadEndX[deadEndIndex] = nextX;             // Mark this point as a dead-end.
                    deadEndY[deadEndIndex] = nextY;             // Mark this point as a dead-end.
                    deadEndIndex++;                             // Move to next index spot for dead-ends.
                }
                indexVisited[indexMove] = true;             // Mark cell as visited.
                indexMove--;                                // Move the index back a step.
                nextY = indexLocY[indexMove];               // The next y-coordinate is the previously indexed spot.
                nextX = indexLocX[indexMove];               // The next x-coordinate is the previously indexed spot.
                mazeWeightCount = mazeWeight[nextY,nextX];  // Maze weight is set to the new spot.
			}

            // If there are valid neighbours:
			if (neighbourValid != -1)
            {
				changeRandVal = Random.Range(0, 101);               // Pick a number between 0 and 100.
				Random.seed += seed;                                // Add seed to the seed in the Random class, to get a different value.
                randomVal = Random.Range(0, (neighbourValid + 1));  // Max is exclusive; pick a random valid area.
				Random.seed -= seed;                                // Return the seed to normal.
				nextX = neighbourValidX[randomVal];                 // Make the next x-coordinate equal to the random valid area.
                nextY = neighbourValidY[randomVal];                 // Make the next y-coordinate equal to the random valid area.
				randStep = possStep[randomVal];                     // Set the random valid area to the next step.
                indexVisited[indexMove] = true;                     // Mark cell as visited.
				
                // "Break" the wall between the cell of the next step and the current step.
				if (randStep == 1){
					maze[nextY + 1, nextX] = (int)status.PATH;
				}
				else if (randStep == 2){
					maze[nextY, nextX + 1] = (int)status.PATH;
				}
				else if (randStep == 3){
					maze[nextY, nextX - 1] = (int)status.PATH;
				}
				else if (randStep == 4){
					maze[nextY - 1, nextX] = (int)status.PATH;
				}

                mazeWeightCount++;                          // Increase the weight as you move away from the start.
				mazeWeight[nextY, nextX] = mazeWeightCount; // Set the weight of the new coordinates as the new weight.
				
                // If the new weight is the heaviest cell:
				if(mazeWeightCount >= highMazeCount)
                {
					highMazeCount = mazeWeightCount;    // Set the new heaviest value as the new value.
					largestX = nextX;                   // Set the coordinates of the heaviest spot.
                    largestY = nextY;                   // Set the coordinates of the heaviest spot.
				}
				
				indexMove++;                    // Increase the index to the next spot.
				indexLocX[indexMove] = nextX;   // Set the coordinates of the new indexed spot.
                indexLocY[indexMove] = nextY;   // Set the coordinates of the new indexed spot.
				visitedCells++;                 // Increase the number of cells that have been visited/edited.
				
                // If the changeRandVal is 75-100:
				if (changeRandVal > 74)
                {
					changeRandVal = Random.Range(0, indexMove);     // Set it to randomed indexed value.
					nextY = indexLocY[indexMove - changeRandVal];   // Set the next x-coordinate as a new previously indexed value.
                    nextX = indexLocX[indexMove - changeRandVal];   // Set the next y-coordinate as a new previously indexed value.
					mazeWeightCount = mazeWeight[nextY,nextX];      // Track the new weight of the new cell.
					indexMove++;                                    // Index this movement.
					indexLocX[indexMove] = nextX;                   // Index the new coordinates.
                    indexLocY[indexMove] = nextY;                   // Index the new coordinates.
				}
			}
		}
	}

    // Generates where the power-ups should be located.
    void powerUpGeneration(int[,] maze)
    {
        int numberOfDeadEnds = deadEndIndex;    // Total amount of dead ends.
        int numberOfPowerUps = numberOfDeadEnds / 4;
        int[] placedPowerUp = new int[numberOfDeadEnds + 1];
        int count = 0;

        for(int i = numberOfPowerUps + 1; i > 0; i--)
        {
            deadEndIndex = Random.Range(0,numberOfDeadEnds);     // Select a random dead end.

            if(deadEndIndex == placedPowerUp[count])
            {
                // Already taken.
                i++;
            }
            else
            {
                maze[deadEndY[deadEndIndex],deadEndX[deadEndIndex]] = (int)status.POWERUP;  // Set coordinate to the power-up.
                placedPowerUp[count] = deadEndIndex;
                count++;
            }
        }
    }
	
    // Instantiate the maze... or rather create the objects according to the generated coordinates.
    void instantiateMaze(int[,] maze)
    {
        Paths = transform.GetChild(1).GetComponent<Transform>();    // Get the Paths object.
        Walls = transform.GetChild(2).GetComponent<Transform>();    // Get the Walls object (used as a parent for the generated wall).
        Others = transform.GetChild(0).GetComponent<Transform>();   // Get the Others object.

        Object wall = init.wallObj;
        Object path = init.pathObj;
        Object endPoint = init.endPointObj;
        Object startPoint = init.startPointObj;
        Object powerUp = init.powerUp;

		Vector3 placerLoc = new Vector3(0,0);       // Placement location vector, initiated at (0, 0).
		Vector3 reset = new Vector3(maxWidth,0,0);  // Reset vector, initiated at (maxWidth, 0, 0).

        // Create the path, at the location 0, 0, with the identity rotation.
		GameObject pathing = (GameObject)Instantiate(path, placerLoc, Quaternion.identity);
		pathing.transform.parent = Paths;                                           // Put the created path as a child of Paths.
		pathing.transform.localScale = new Vector3(maxWidth - 1, maxLength - 1);    // Scale the path to fit.
		pathing.transform.localPosition = new Vector3(maxWidth/2,maxLength/2);      // Move it to the center of the maze, to line it with the walls.

        // Loop through all of the maze:
		for(int i = 0; i < maxLength; i++){
			for(int j = 0; j < maxWidth; j++){
				// Starts at [0, 0] which is bottom left.
                // If the given coordinate is a wall:
				if(maze[i,j] == (int)status.WALL)
                {
                    // Create a wall tile, which are connected via the connect verts file.
					GameObject tile = (GameObject)Instantiate(wall, placerLoc, Quaternion.identity);
					tile.transform.parent = Walls;                                  // Make the tile a child of Walls.
                    tile.transform.localScale = new Vector3(scale, scale, scale);   // Scale it accordingly.
				}
                // If the coord is supposed to be a power-up:
                if(maze[i,j] == (int)status.POWERUP)
                {
                    GameObject powerUpObj = (GameObject)Instantiate(powerUp, placerLoc, Quaternion.identity);
                    powerUpObj.transform.parent = transform.GetChild(0).GetComponent<Transform>();
                }
				placerLoc += Vector3.right; // Move the "placer" to the right every iteration.
			}
			placerLoc += Vector3.up;    // Every iteration that moves to the next row, move the "placer" up.
			placerLoc -= reset;         // Also, reset the "placer" back to the according x-coordinate.
		}

        // Instantiate the start point.
        GameObject start = (GameObject)Instantiate(startPoint,new Vector2(startX,startY),Quaternion.identity);
		GameObject end = (GameObject)Instantiate(endPoint,new Vector2(largestX,largestY),Quaternion.identity);
        start.transform.parent = Others;    // Make the start point a child of Paths.
        end.transform.parent = Others;      // Make the end point a child of Paths.
	}
	
    // "Getter" for the maze.
	public int[,] getMaze()
    {
		return maze;
	}
	
	// Use this for initialization, it runs once when the script is awaken.
    void Awake ()
    {
        // Variables:
        // Gets the randomizer's information/variables/functions/etc.
        randInfo = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<Randomizer>();
        init = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<Initialization>();
        mazeLength = randInfo.mazeLength;   // Length of the maze.
        mazeWidth = randInfo.mazeWidth;     // Width of the maze.
        seed = randInfo.mazeSeed;           // Seed to be used to generate the maze.
    
        // Logic:
		maxLength = mazeLength * 2 + 1;         // Max length is the given length times two plus one, to accomodate the walls.
        maxWidth = mazeWidth * 2 + 1;           // Max width is the given width times two plus one, to accomodate the walls.
		totalCells = mazeLength * mazeWidth;    // Total cells is the *given* lengths times the width.
		maze = new int[maxLength,maxWidth];     // Set the maze array to the max x and y coordinates.
        deadEndX = new int[totalCells / 2];     // Instantialize deadEndX to prevent null references.
        deadEndY = new int[totalCells / 2];     // Instantialize deadEndY to prevent null references.
		initMaze(maze);                         // Initialize the maze.
		mazeAlgorithm(maze);                    // Generate the maze's coordinates.
        powerUpGeneration(maze);
		instantiateMaze(maze);                  // Instantiate based on the given coordinates.
	}
}
