package code;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

import java.io.*;
import java.net.Socket;

public class FileTransfer {

    private Socket client;
    private String folderRoot;
    private DataInputStream receiver;
    private DataOutputStream sender;
    private FSP status;

    public FileTransfer(Socket client, String folderRoot) throws IOException {

        this.folderRoot = folderRoot;
        this.client     = client;

        this.receiver = new DataInputStream  (this.client.getInputStream());
        this.sender   = new DataOutputStream (this.client.getOutputStream());

    }

    public void close() throws IOException{

        sender.writeInt(FSP.C_CLOSE_CONECTION.getVal());
        sender.flush();

        closeServer();

    }

    public void closeServer() throws IOException {

        sender.close();
        receiver.close();
        client.close();

    }

    public void send(String relativePath) throws  IOException{

        File file = new File(folderRoot + relativePath);
        if (!file.exists()){
            System.out.println("File " + file.getAbsolutePath() + " doesn't exits.");
            return;
        }

        if (file.isDirectory())
            send(new Folder(file), folderRoot, "");
        else
            send(new Archive(file), folderRoot, "");

    }

    public void send(Folder folder, String originPath, String destPath) throws IOException{

        String relative = File.separator + folder.getName();

        createRemoteDirectory(destPath + relative);

        for (Archive archive : folder.listFiles())
            send(archive, originPath + relative, destPath + relative);

        for (Folder f : folder.listFolders())
            send(f, originPath + relative, destPath + relative);

    }

    public void send(Archive archive, String originPath, String destPath) throws  IOException{
        sendFile(new File(originPath + File.separator  + archive.getName()), destPath);
    }

    private void createRemoteDirectory(String path) throws IOException{

        sender.writeInt(FSP.R_CREATE_DIRECTORY.getVal());
        sender.flush();

        sender.writeUTF(path);
        sender.flush();

        accept();
        if (status == FSP.S_SUCCESS)
            System.out.println("Created " + path);
        else
            System.out.println("Error creating " + path);

    }

    private void sendFile(File file, String destPath) throws IOException{

        sender.writeInt(FSP.R_SENT_FILE.getVal());
        sender.flush();

        DataInputStream fileReader = new DataInputStream(new FileInputStream(file));

        sender.writeUTF(destPath);
        sender.flush();
        sender.writeUTF(file.getName());
        sender.flush();
        sender.writeLong(file.length());

        long sent = 0;

        while (sent < file.length()){

            byte[] buffer = new byte[4096];
            int readed = fileReader.read(buffer);

            sender.write(buffer, 0, readed);
            sender.flush();

            sent += readed;
            int percentage = (int)((sent * 100) / file.length());
            System.out.print("\r");
            System.out.print("Sending " + file.getName() + "[" + percentage + "%]");

        }

        System.out.print("\r");

        accept();
        if (status == FSP.S_SUCCESS)
            System.out.println("Sent " + file.getName() + " to " + destPath);
        else
            System.out.println("Error sending " + file.getName() + " to " + destPath);

        fileReader.close();

    }

    public void deleteRemote(String path) throws IOException {

        sender.writeInt(FSP.R_REMOVE.getVal());
        sender.flush();
        sender.writeUTF(path);
        sender.flush();

        accept();

        if (status == FSP.S_SUCCESS)
            System.out.println("Removed " + path);
        else
            System.out.println("Cannot remove " + path);

    }

    public Folder requestTreeDirectory() throws IOException {

        sender.writeInt(FSP.R_TREE_DIRECTORY.getVal());
        sender.flush();

        Gson gson = new GsonBuilder().create();

        return gson.fromJson(receiver.readUTF(), Folder.class);

    }

    public void requestFile(String relativePathName, String destPath) throws IOException{

        sender.writeInt(FSP.R_FILE.getVal());
        sender.flush();
        sender.writeUTF(relativePathName);
        sender.flush();

        accept();
        if (status == FSP.E_ERROR) {
            System.out.println("Requested file " + relativePathName + " doesn't exits on the server.");
            return;
        }

        sender.writeUTF(destPath);
        sender.flush();

        accept();
        receiveFile();

    }

    /*--------- SERVER SIDE ---------*/
    public void serverProcess() throws IOException, IllegalStateException {

        while (true) {

            accept();

            boolean exit = false;

            switch (status) {

                case R_SENT_FILE:
                    receiveFile();
                    break;
                case R_CREATE_DIRECTORY:
                    createDirectory();
                    break;
                case R_TREE_DIRECTORY:
                    sendTreeDirectory();
                    break;
                case R_REMOVE:
                    deleteFile();
                    break;
                case R_FILE:
                    sendRequestedFile();
                    break;
                default:
                    exit = true;
                    break;

            }

            if (exit) break;

        }

        closeServer();

        if (status == FSP.C_CLOSE_CONECTION)
            System.out.println("Closed connection with " + client.getInetAddress().getHostAddress());
        else
            throw new IllegalStateException("Malformed communication, non expected status: " + status + ". Closed connection with " + client.getInetAddress().getHostAddress());

    }

    private void accept() throws IOException{

        status = FSP.values()[receiver.readInt()];

    }

    private void receiveFile() throws IOException {

        try {

            String destPath = folderRoot + receiver.readUTF();

            mkdir(destPath);

            String fileName = receiver.readUTF();
            long size       = receiver.readLong();

            File file = new File(destPath + File.separator + fileName);

            System.out.println("Ready to receive " + fileName + " of " + size + " bytes");

            DataOutputStream fileWriter = new DataOutputStream(new FileOutputStream(file));
            long received = 0;

            while (received < size) {

                byte[] buffer = new byte[4096];
                int readed = receiver.read(buffer);

                fileWriter.write(buffer, 0, readed);

                received += readed;
                int percentage = (int)((received * 100) / size);
                System.out.print("\r");
                System.out.print("Receiving: " + file.getName() + "[" + percentage + "%]");

            }

            System.out.print("\r");
            System.out.println("Received " + file.getName() + " to " + destPath);

            sender.writeInt(FSP.S_SUCCESS.getVal());

        } catch (IOException ioe){

            System.out.println("Error receiving file: " + ioe.getMessage());
            sender.writeInt(FSP.E_ERROR.getVal());

        }
    }

    private void createDirectory() throws IOException{

        try {
            String path = folderRoot + receiver.readUTF();
            System.out.println("Creating " + path);
            mkdir(path);
            System.out.println("Created " + path);

            sender.writeInt(FSP.S_SUCCESS.getVal());
        } catch (IOException ioe) {

            System.out.println("Error creating directory");
            sender.writeInt(FSP.E_ERROR.getVal());

            throw ioe;
        }

    }

    private void sendTreeDirectory() throws IOException {

        System.out.println("Request to send the directory tree, building and sending...");
        Folder tree = new Folder(new File(folderRoot));

        Gson gson = new GsonBuilder().create();

        sender.writeUTF(gson.toJson(tree));

        System.out.println("Sent directory tree");

    }

    private void deleteFile() throws IOException {

        String filePath = folderRoot + receiver.readUTF();
        File file = new File(filePath);

       try {

           if (file.isDirectory())
               deleteFiles(file.listFiles());

           if (!file.delete()){
               System.out.println("Cannot delete " + filePath);
               sender.writeInt(FSP.E_ERROR.getVal());

               return;
           }

           System.out.println("Removed " + filePath);
           sender.writeInt(FSP.S_SUCCESS.getVal());

       } catch (Exception e) {

           System.out.println("Cannot delete " + filePath + e.getMessage());
           sender.writeInt(FSP.E_ERROR.getVal());

       }

    }

    private void deleteFiles(File [] files) {

        for (File file : files) {

            if (file.isDirectory())
                deleteFiles(file.listFiles());

            if (file.delete())
                System.out.println("Removed: " + file.getAbsolutePath());
            else
                System.out.println("Cannot remove: " + file.getAbsolutePath());

        }

    }

    private void sendRequestedFile() throws IOException {

        String path = folderRoot + receiver.readUTF();

        File file = new File(path);

        if (!file.exists()) {
            sender.writeInt(FSP.E_ERROR.getVal());
            sender.flush();
            return;
        }

        sender.writeInt(FSP.S_SUCCESS.getVal());
        sender.flush();

        String destPath = receiver.readUTF();

        sendFile(file, destPath);

    }

    public static boolean mkdir(String path) {

        File file = new File(path);
        return file.mkdirs();

    }

}
