using UnityEngine;
using System.Collections;

public class SmoothFollowCamera : MonoBehaviour
{

	public Transform target;
	public float smoothTime = 0.3f;
	private Vector2 velocity;
	private Transform thisTransform;
	private float newX;
	private float newY;

	// Use this for initialization
	void Start()
	{
		thisTransform = transform;
	}
	
	// Update is called once per frame
	void Update()
	{
		newX = Mathf.SmoothDamp(thisTransform.position.x, 
		                                            target.position.x,ref velocity.x,smoothTime);
		newY = Mathf.SmoothDamp(thisTransform.position.y, 
		                                            target.position.y,ref velocity.y,smoothTime);
		thisTransform.position = new Vector3(newX,newY,thisTransform.position.z);
	}
}
