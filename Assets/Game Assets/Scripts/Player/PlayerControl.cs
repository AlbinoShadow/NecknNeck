using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    public int smooth; // Determines how quickly object moves towards position
    
    private Vector3 targetPosition;
    private float newZ;
    private HashIDs hash;
    private Animator anim;
    private Quaternion rotation;

    public static float AngleSigned(Vector3 v1,Vector3 v2,Vector3 n)
    {
        float angle = Mathf.Atan2(Vector3.Dot(n,Vector3.Cross(v1,v2)),Vector3.Dot(v1,v2)) * Mathf.Rad2Deg;
        if(angle < 0)
            angle += 360.0f;
        return (angle);
    }

    void Awake()
    {
        Randomizer randInfo = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<Randomizer>();
        int x = 1, y = 1;
        anim = GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        if(randInfo.mazeType == 1)
        {
            MazeGeneration maze = GameObject.FindGameObjectWithTag(Tags.maze).GetComponent<MazeGeneration>();
            x = maze.startX;
            y = maze.startY;
        }
        else if(randInfo.mazeType == 2)
        {
            ImperfectMazeGen maze = GameObject.FindGameObjectWithTag(Tags.maze).GetComponent<ImperfectMazeGen>();
            x = maze.startX;
            y = maze.startY;
        }
        this.transform.position = new Vector3(x,y);
    }
    
    void LateUpdate()
    {
        Plane playerPlane = new Plane(Vector3.back,transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitdist = 0.0f;
        Vector3 lookPos;

        if (Input.GetMouseButtonDown(0))
        {
            if(playerPlane.Raycast(ray,out hitdist))
            {
                targetPosition = ray.GetPoint(hitdist);
                lookPos = targetPosition - transform.position;
                rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
                GetComponent<Rigidbody2D>().AddForce(transform.right * smooth * Time.deltaTime);
                anim.SetFloat(hash.speedFloat,5.5f);
            }
        }
        else if(Input.GetMouseButton(0))
        {
            if(playerPlane.Raycast(ray,out hitdist) && Vector3.Distance(transform.position,targetPosition) > 0.5f)
            {
                targetPosition = ray.GetPoint(hitdist);
                lookPos = targetPosition - transform.position;
                rotation = Quaternion.LookRotation(lookPos);

                float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
                GetComponent<Rigidbody2D>().AddForce(transform.right * smooth * Time.deltaTime);
                anim.SetFloat(hash.speedFloat,5.5f);
            }
            else
            {
                targetPosition = ray.GetPoint(hitdist);
            }
        }
        else
            anim.SetFloat(hash.speedFloat,0.0f);
    }
}
