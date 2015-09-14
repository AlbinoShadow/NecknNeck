using UnityEngine;

public class Randomizer : MonoBehaviour
{
    public int mazeLength;
    public int mazeWidth;
    public int mazeSeed;
    public bool debugMode;
    public int seed;
    public int mazeType;
    public Vector2 startPoint;
    public float randHue;
    
    //Incomplete, just using it so the code works correctly once this is active.
    int randMazeType(int seed)
    {
        if(seed < 50)
            return 1; //perfect
        else
            return 2; //imperfect
    }
    
    void mazeManager(int type)
    {
        GameObject mazeObj = GameObject.FindGameObjectWithTag(Tags.maze);
        
        mazeLength = Random.Range(15,21);
        Random.seed *= 2;
        mazeWidth = Random.Range(15,21);
        Random.seed /= 2;
        mazeSeed = Random.seed;
        
        if(type == 1)
        {
            mazeObj.AddComponent<MazeGeneration>();
        }
        else if(type == 2)
        {
            mazeObj.AddComponent<ImperfectMazeGen>();
        }
        else
        {
            Debug.LogError("Maze type value neither perfect nor imperfect");
        }
    }
    
    void Awake()
    {
        int curRanVal;
        
        seed = (int)System.DateTime.Now.Ticks;
        if(debugMode == true)
            seed = 314519214;
        Random.seed = seed;
        curRanVal = Random.Range(0,100);
        Random.seed = seed * 2;
        randHue = Random.Range(0.0f,1.0f);

        //mazeType = randMazeType(curRanVal);
        mazeManager(mazeType);
    }
}
