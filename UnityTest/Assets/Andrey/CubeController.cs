using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class CubeController : MonoBehaviour {

	public float speed = 10.0F;
	public float rotationSpeed = 100.0F;
	EventListener listener;

	// Use this for initialization
	void Start () {
		listener = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<EventListener>();
	}
	
	// Update is called once per frame
	void Update () {

		float translation = Input.GetAxis("Vertical") * speed;
		float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
		translation *= Time.deltaTime;
		rotation *= Time.deltaTime;
		transform.Translate(0, 0, translation);
		transform.Rotate(0, rotation, 0);
		if (Input.anyKey) {
			listener.handleEvent(transform.position, transform.rotation);
		}
	}
}
