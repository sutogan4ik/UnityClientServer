using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.IO;


public class EventListener : MonoBehaviour {
	public Rigidbody player;
	StreamWriter writer;
	NetworkStream stream;
	string id;
	// Use this for initialization
	void Start () {
		print("Connection");
		TcpClient client = new TcpClient ("127.0.0.1", 16000);
		stream = client.GetStream();
		stream.ReadTimeout = 10;
		//stream.WriteTimeout = 2;
		if (stream.CanRead) {
			writer = new StreamWriter(stream);
			print("Writer created");
			readData ();
		}
	}
	
	public void handleEvent(Vector3 position, Quaternion rotation){
		print (id);
		JSONObject json = new JSONObject ();
		json.AddField ("action", "move");
		JSONObject pos = new JSONObject();

		pos.AddField ("X", position.x.ToString());
		pos.AddField ("Y", position.y.ToString());
		pos.AddField ("Z", position.z.ToString());
		json.AddField ("position", pos);
		JSONObject rot = new JSONObject ();
		rot.AddField ("X", rotation.x.ToString());
		rot.AddField ("Y", rotation.y.ToString());
		rot.AddField ("Z", rotation.z.ToString());
		rot.AddField ("W", rotation.w.ToString());
		json.AddField ("rotation", rot);
		json.AddField ("id", id);
		send (json.ToString());

	}

	public void sendMove(Vector3 move, bool crouch, bool jump){
		JSONObject json = new JSONObject ();
		json.AddField ("action", "moveChar");
		JSONObject pos = new JSONObject();
		
		pos.AddField ("X", move.x.ToString());
		pos.AddField ("Y", move.y.ToString());
		pos.AddField ("Z", move.z.ToString());
		json.AddField ("position", pos);
		json.AddField ("id", id);
		json.AddField ("crouch", crouch);
		json.AddField ("jump", jump);
		send (json.ToString());
	}

	private void send(string json){
		writer.Write (json);
		writer.Flush ();
	}

	void Update () {
		readData ();
	}

	void readData(){
		if (stream.CanRead) {
			try{
				byte[] bLen = new Byte[4];
				int data = stream.Read(bLen, 0, 4);
				if(data > 0){
					int len = BitConverter.ToInt32( bLen, 0);
					print("len = " + len);
					Byte[] buff = new byte[1024];
					data = stream.Read (buff, 0, len);
					if (data > 0) {
						string result = Encoding.ASCII.GetString (buff, 0, data);
						stream.Flush();
						parseData(result);
					}
				}
			}catch (Exception ex){

			}
		}
	}

	void parseData(string data){
		JSONObject json = new JSONObject (data);
		string action = json.GetField ("action").str;
		print(action + "parse data" + data);
		JSONObject pos = json.GetField("position");
		Single pX = Convert.ToSingle (pos.GetField ("X").str);
		Single pY = Convert.ToSingle (pos.GetField ("Y").str);
		Single pZ = Convert.ToSingle (pos.GetField ("Z").str);
		Vector3 position = new Vector3 (pX, pY, pZ);
		print ("new vector = x-" + pos.GetField ("X").str + " y-" + pos.GetField ("Y").str);
		JSONObject rot = json.GetField("rotation");
		Single rX = Convert.ToSingle (rot.GetField ("X").str);
		Single rY = Convert.ToSingle (rot.GetField ("Y").str);
		Single rZ = Convert.ToSingle (rot.GetField ("Z").str);
		Single rW = Convert.ToSingle (rot.GetField ("W").str);
		Quaternion rotation = new Quaternion (rX, rY, rZ, rW);
		switch (action) {
			case "start":
				this.id = json.GetField ("id").str;
				createPlayer ();
				break;
			case "newPlayer":
				createNewClient (json.GetField ("id").str, position, rotation);
				break;
			case "move":
				moveClient (json.GetField ("id").str, position, rotation);
				break;
		}

	}

	void createPlayer(){
		Instantiate(player, new Vector3(0, 1, 0), new Quaternion());
	}

	void createNewClient(string id, Vector3 pos, Quaternion rot){
		Respawn resp = GameObject.FindGameObjectWithTag ("respawn").GetComponent<Respawn> ();
		resp.addClient (id, pos, rot);
	}

	void moveClient(string id, Vector3 pos, Quaternion rot){
		Respawn resp = GameObject.FindGameObjectWithTag ("respawn").GetComponent<Respawn> ();
		resp.moveClient (id, pos, rot);
	}
}
