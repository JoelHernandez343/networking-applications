package app.helpers;

import com.google.gson.Gson;

import java.io.File;
import java.io.IOException;
import java.net.ServerSocket;

public class StoreServer {
    int port;
    Gson gson;
    String path;
    File itemsFile;

    ServerSocket serverSocket;
    RawStreamConnection client;

    Instruction state;

    public StoreServer(int port, String path) throws Exception{
        this.port = port;
        this.path = path;
        itemsFile = new File(path + File.separator + "db/Items.json");

        serverSocket = new ServerSocket(port);
    }

    public void accept() throws IOException {
        System.out.println("Waiting connection at port " + port + " ...");
        client = new RawStreamConnection(serverSocket.accept());
    }

    void closeConnection() throws IOException {
        client.close();
        System.out.println("Closed connection with " + client.client.getInetAddress().getHostAddress());
    }

    Instruction acceptInstruction() throws IOException {
        state = Instruction.values()[client.readInt()];
        return state;
    }

    void waitOk() throws IOException{
        if (acceptInstruction() != Instruction.OK)
            throw new IllegalStateException("Illegal response by the client.");
    }

    public void storeProcess() throws IOException {

        try {
            while (acceptInstruction() != Instruction.DISCONNECT) {
                switch (state) {
                    case REQUEST_DB:
                        sendDb();
                        break;
                    case REQUEST_IMAGES:
                        sendImages();
                        break;
                    case POST_PURCHASE:
                        break;
                    default:
                        throw new IllegalStateException("Illegal response by the client.");
                }
            }
        }
        catch (IllegalStateException e) {
            System.out.println(e.getMessage());
        }
        catch (IOException e) {
            e.printStackTrace();
        }

        closeConnection();
    }

    void sendDb() throws IOException {
        String stringDb = JsonFile.readFile(itemsFile);
        client.write(stringDb);

        waitOk();
    }

    void sendImages() throws IOException {
        String key = client.readUTF();

        while (acceptInstruction() == Instruction.REQUEST_ONE_IMAGE)
            sendImage(key);

        if (state != Instruction.FINISHED_PROCESS)
            throw new IllegalStateException("Illegal response by the client.");
    }

    void sendImage(String key) throws IOException {
        String name = client.readUTF();
        File image = new File(path + File.separator + "images" + File.separator + key + "_" + name);
        client.sendFile(image);

        waitOk();
    }

}
