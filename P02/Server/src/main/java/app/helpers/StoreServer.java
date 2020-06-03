package app.helpers;

import app.model.*;
import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import java.io.File;
import java.io.IOException;
import java.lang.reflect.Type;
import java.net.ServerSocket;
import java.util.ArrayList;
import java.util.List;

public class StoreServer {
    int port;
    Gson gson;
    String path;

    File itemsFile;
    List<Item> items;

    File clientsFile;
    List<Client> clients;

    ServerSocket serverSocket;
    RawStreamConnection client;

    Instruction state;

    public StoreServer(int port, String path) throws Exception{
        this.port = port;
        this.path = path;

        itemsFile = new File(path + File.separator + "db" + File.separator + "Items.json");
        items = JsonFile.getListOfFile(itemsFile, new TypeToken<List<Item>>(){}.getType());

        clientsFile = new File(path + File.separator + "db" + File.separator + "Clients.json");
        clients = JsonFile.getListOfFile(clientsFile, new TypeToken<List<Client>>(){}.getType());

        serverSocket = new ServerSocket(port);

        gson = new Gson();
    }

    public void accept() throws IOException {
        System.out.println("Waiting connection at port " + port + " ...");
        client = new RawStreamConnection(serverSocket.accept());
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
                        postedPurchase();
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

    void sendDb() throws IOException {
        System.out.println("Requested DB.");

        String stringDb = JsonFile.readFile(itemsFile);
        client.write(stringDb);

        waitOk();
        System.out.println("Sended DB.");
    }

    void sendImages() throws IOException {
        System.out.println("Requested Images.");
        String key = client.readUTF();

        while (acceptInstruction() == Instruction.REQUEST_ONE_IMAGE)
            sendImage(key);

        if (state != Instruction.FINISHED_PROCESS)
            throw new IllegalStateException("Illegal response by the client.");
    }

    void sendImage(String key) throws IOException {
        String name = key + File.separator + client.readUTF();

        System.out.println("Requested " + name);

        File image = new File(path + File.separator + "images" + File.separator + name);
        client.sendFile(image);

        waitOk();
    }

    void postedPurchase() throws IOException {
        System.out.println("Posted purchase.");

        String email = client.readUTF();
        int cardNumber = client.readInt();
        double total = Double.parseDouble(client.readUTF());

        List<PurchasedItem> purchasedItems = updateItems(client.readUTF());
        addClientsPurchase(email, cardNumber, total, purchasedItems);

        client.write(Instruction.OK.getValue());

        System.out.println("Purchase added to " + email);
    }

    List<PurchasedItem> updateItems(String json) throws IOException {
        List<PurchasedItem> purchasedItems = new ArrayList<PurchasedItem>();

        List<PurchaseUpdate> purchased = gson.fromJson(json, new TypeToken<List<PurchaseUpdate>>(){}.getType());
        for (PurchaseUpdate purchaseUpdate : purchased){
            for (Item item : items){
                if (item.Key.equals(purchaseUpdate.Key)){
                    item.Quantity -= purchaseUpdate.Purchased;

                    purchasedItems.add(new PurchasedItem(item, purchaseUpdate.Purchased));

                    break;
                }
            }
        }
        JsonFile.writeJsonToFile(itemsFile, items);

        return purchasedItems;
    }

    void addClientsPurchase(String email, int cardNumber, double total, List<PurchasedItem> purchasedItems) throws IOException {

        boolean exists = false;

        for (Client client : clients){
            if (client.Email.equals(email)){
                client.Purchases.add(new Purchase(cardNumber, total, purchasedItems));
                exists = true;
                break;
            }
        }
        if (!exists) {
            Client client = new Client(email);
            client.Purchases.add(new Purchase(cardNumber, total, purchasedItems));
            clients.add(client);
        }

        JsonFile.writeJsonToFile(clientsFile, clients);
    }
}
