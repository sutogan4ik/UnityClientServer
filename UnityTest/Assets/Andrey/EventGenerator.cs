using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class EventGenerator : MonoBehaviour {
	EventListener listener;

	// Use this for initialization
	void Start () {
		listener = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<EventListener>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKey) {
			listener.handleEvent(transform.position, transform.rotation);
		}
	}
}
