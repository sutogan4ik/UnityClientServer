import org.json.JSONException;
import org.json.JSONObject;

import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.nio.ByteBuffer;
import java.util.UUID;

/**
 * Created by prog on 13.03.15.
 */
public class Client {
    private Socket client;
    private ReceiveListener listener;
    private InputStream inputStream;
    private OutputStream outputStream;
    private String id = UUID.randomUUID().toString();
    public volatile Position position;
    public volatile Rotation rotation;
    public Client(Socket client, ReceiveListener listener) throws IOException {
        this.client = client;
        this.listener = listener;
        inputStream = client.getInputStream();
        outputStream = client.getOutputStream();
        position = new Position();
        rotation = new Rotation();
        new ReadThread().start();
        sendStart();
    }

    public String getId() {
        return id;
    }

    private void sendStart(){
        try {
            JSONObject json = new JSONObject();
            json.put("action", "start");
            json.put("id", id);
            JSONObject pos = new JSONObject();
            pos.put("X", position.x);
            pos.put("Y", position.y);
            pos.put("Z", position.z);
            json.put("position", pos);
            JSONObject rot = new JSONObject ();
            rot.put("X", rotation.x);
            rot.put("Y", rotation.y);
            rot.put("Z", rotation.z);
            rot.put("W", rotation.w);
            json.put("rotation", rot);
            sendToClient(json.toString());
        } catch (JSONException e) {
            e.printStackTrace();
        }
    }

    public void sendToClient(String json){
        System.out.println("sendToClient");
        try {

            byte[] bytes = json.getBytes();
            byte[] bytesSize = intToByteArray(json.length());
            outputStream.write(bytesSize, 0, 4);
            outputStream.write(bytes, 0, bytes.length);
            outputStream.flush();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private class ReadThread extends Thread{

        @Override
        public void run() {
            super.run();
            byte[] bytes = new byte[1024];
            while (!client.isClosed()){
                try {
                    int data = inputStream.read(bytes);
                    if (data != -1){
                        String string = new String(bytes, 0, data);
                        System.out.println(string);
                        JSONObject jsonObject = new JSONObject(string);
                        JSONObject pos = jsonObject.optJSONObject("position");
                            position.x = pos.optString("X");
                            position.y = pos.optString("Y");
                            position.z = pos.optString("Z");

                        JSONObject rot = jsonObject.optJSONObject("rotation");
                            rotation.x = rot.optString("X");
                            rotation.y = rot.optString("Y");
                            rotation.z = rot.optString("Z");
                            rotation.w = rot.optString("W");

                        listener.dataReceive(Client.this, string);
                    }
                } catch (IOException e) {
                    e.printStackTrace();
                    break;
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }
            System.out.println("Close");
        }
    }

    public static byte[] intToByteArray(int a)
    {
        byte[] ret = new byte[4];
        ret[0] = (byte) (a & 0xFF);
        ret[1] = (byte) ((a >> 8) & 0xFF);
        ret[2] = (byte) ((a >> 16) & 0xFF);
        ret[3] = (byte) ((a >> 24) & 0xFF);
        return ret;
    }
}
