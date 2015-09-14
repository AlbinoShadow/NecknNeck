using UnityEngine;
using System.Collections;

public class CameraDebugControl : MonoBehaviour {
	public float speed;
	private Transform player;

	// Use this for initialization
	void Start () {
		player = transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.W))
			player.Translate(Vector3.up * Time.deltaTime * speed, Space.World);
		else if(Input.GetKey(KeyCode.S))
			player.Translate(Vector3.down * Time.deltaTime * speed, Space.World);
		if(Input.GetKey(KeyCode.A))
			player.Translate(Vector3.left * Time.deltaTime * speed, Space.World);
		else if(Input.GetKey(KeyCode.D))
			player.Translate(Vector3.right * Time.deltaTime * speed, Space.World);
	}
}