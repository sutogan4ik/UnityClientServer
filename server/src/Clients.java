import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by prog on 13.03.15.
 */
public class Clients implements ReceiveListener {
    private List<Client> clients = new ArrayList<Client>();

    public Clients() {
        clients = new ArrayList<Client>();
    }

    public void addClient(Client client){
        System.out.println("addClient");
        clients.add(client);
        sendConnectNewPlayer(client);
        getAllPlayers(client);
    }

    @Override
    public void dataReceive(Client client, String data) {
        sendBroadcast(client, data);
    }

    public void sendBroadcast(Client client,  String data){
        System.out.println("sendBroadcast");
        for(Client item : clients){
            if(item != client) {
                item.sendToClient(data);
            }
        }
    }

    private void sendConnectNewPlayer(Client client){
        System.out.println("sendConnectNewPlayer");
        JSONObject json = new JSONObject();
        try {
            json.put("id", client.getId());
            json.put("action", "newPlayer");
            json.put("TAG", "sendConnectNewPlayer");
            JSONObject pos = new JSONObject();
            synchronized (client.position) {
                pos.put("X", client.position.x);
                pos.put("Y", client.position.y);
                pos.put("Z", client.position.z);
            }
            json.put("position", pos);
            JSONObject rot = new JSONObject();
            synchronized (client.rotation) {
                rot.put("X", client.rotation.x);
                rot.put("Y", client.rotation.y);
                rot.put("Z", client.rotation.z);
                rot.put("W", client.rotation.w);
            }
            json.put("rotation", rot);
            sendBroadcast(client, json.toString());
        } catch (JSONException e) {
            e.printStackTrace();
        }
    }

    private void getAllPlayers(Client client){
        System.out.println("getAllPlayers");
        for(Client item : clients){
            if(item != client) {
                JSONObject json = new JSONObject();
                try {
                    json.put("id", client.getId());
                    json.put("action", "newPlayer");
                    json.put("TAG", "getAllPlayers");
                    JSONObject pos = new JSONObject();
                        pos.put("X", item.position.x);
                        pos.put("Y", item.position.y);
                        pos.put("Z", item.position.z);

                    json.put("position", pos);
                    JSONObject rot = new JSONObject();
                        rot.put("X", item.rotation.x);
                        rot.put("Y", item.rotation.y);
                        rot.put("Z", item.rotation.z);
                        rot.put("W", item.rotation.w);

                    json.put("rotation", rot);
                } catch (JSONException e) {
                    e.printStackTrace();
                }
                client.sendToClient(json.toString());
            }
        }
    }
}
