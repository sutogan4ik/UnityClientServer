using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.ThirdPerson;


public class Respawn : MonoBehaviour {
	IDictionary<string, GameObject> clients;
	// Use this for initialization
	void Start () {
		clients = new Dictionary<string, GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void addClient(string id, Vector3 pos, Quaternion rot){
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject.AddComponent<Rigidbody> ();
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rot;
		clients.Add (id, gameObject);
	}

	public void moveClient(string id, Vector3 pos, Quaternion rot){
		GameObject gameObject = clients [id];
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rot;
	}
}
