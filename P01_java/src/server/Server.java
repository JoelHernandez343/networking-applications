package server;

import client.Client;
import code.FileTransfer;
import code.Folder;

import java.io.DataInputStream;
import java.io.EOFException;
import java.io.File;
import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;

public class Server {

    private static String getInternalPath(){

        String path = Server.class.getResource(Server.class.getSimpleName() + ".class").toString();

        int i = 0;
        for (; path.charAt(i) != File.separatorChar ; ++i);
        path = path.substring(i);

        File aux = new File(path);
        return aux.getParent();

    }

    public static void main (String [] args){

        String path = args[0].equals("flag") ? getInternalPath() + File.separator + "root" : args[0];
        FileTransfer.mkdir(path);

        try {

            int port = 1234;
            ServerSocket server = new ServerSocket(port);
            System.out.println("Server started at port " + port + ".");

            while (true) {

                System.out.println("Waiting for connection ...");

                Socket client = server.accept();
                System.out.println("Connected to " + client.getInetAddress().getHostAddress());

                FileTransfer fileTransfer = new FileTransfer(client, path);
                fileTransfer.serverProcess();

                System.out.println("Closed connection with " + client.getInetAddress().getHostAddress());

            }

        } catch (Exception e) {
            e.printStackTrace();
        }

    }

}