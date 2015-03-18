import java.io.*;
import java.net.ServerSocket;
import java.net.Socket;

/**
 * Created by prog on 12.03.15.
 */
public class ServerMain {
    public static void main(String[] args){
        try {
            Clients clients = new Clients();

            ServerSocket serverSocket = new ServerSocket(16000);
            while (true) {
                System.out.println("Wait client");
                Socket socket = serverSocket.accept();
                System.out.println("Client connected");
                Client client = new Client(socket, clients);
                clients.addClient(client);
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
