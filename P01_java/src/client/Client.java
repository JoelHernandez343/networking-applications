package client;

import code.Archive;
import code.FileTransfer;
import code.Folder;

import java.io.BufferedReader;
import java.io.File;
import java.io.InputStreamReader;
import java.net.InetAddress;
import java.net.Socket;

public class Client {

    private static String getInternalPath(){

        String path = Client.class.getResource(Client.class.getSimpleName() + ".class").toString();

        int i = 0;
        for (; path.charAt(i) != File.separatorChar ; ++i);
        path = path.substring(i);

        File aux = new File(path);
        return aux.getParent();

    }

    public static void printHelp(String root) {

        System.out.println("Using local directory: " + root);

        System.out.println("OPTIONS:");
        System.out.println(">> 1. Send files to the server.");
        System.out.println(">> 2. Download data from the server.");
        System.out.println(">> 3. Delete a file from the server.");
        System.out.println(">> 4. View remote tree directory.");
        System.out.println(">> 5. View local tree directory.");
        System.out.println(">> 6. Exit");
        System.out.println(">> Another. view this help.");
        System.out.println("By Joel H.");

    }

    public static void main (String [] args){

        if (args.length == 0)
            throw new IllegalArgumentException("The file folder must be indicated");

        String root = args[0].equals("flag") ? getInternalPath() + File.separator + "root" : args[0];
        FileTransfer.mkdir(root);

        try {

            InetAddress dest = InetAddress.getByName("localhost");
            Socket client = new Socket(dest, 1234);

            FileTransfer fileTransfer = new FileTransfer(client, root);
            BufferedReader in = new BufferedReader(new InputStreamReader(System.in));

            printHelp(root);
            boolean stay = true;

            while (stay) {

                System.out.println("Write a number:");

                String opt = in.readLine();

                switch (opt) {
                    case "1":
                        System.out.println("-- SEND FILE / FOLDER --");
                        System.out.println("Write the file / folder / path beginning with /(Linux) or \\ (Windows)");
                        String relativePath = in.readLine();

                        fileTransfer.send(relativePath);

                        break;
                    case "2":
                        System.out.println("-- DOWNLOAD FILE / FOLDER --");
                        System.out.println("Write the file / folder / path beginning with /(Linux) or \\ (Windows)");
                        String remotePath = in.readLine();
                        System.out.println("Write the destination (same rules):");
                        String destPath = in.readLine();

                        fileTransfer.requestFile(remotePath, destPath);

                        break;
                    case "3":
                        System.out.println("-- DELETE FILE / FOLDER --");
                        System.out.println("Write the file / folder / path beginning with /(Linux) or \\ (Windows)");
                        String toDelete = in.readLine();

                        fileTransfer.deleteRemote(toDelete);

                        break;
                    case "4":
                        System.out.println("-- VIEW REMOTE DIRECTORY --");
                        System.out.println("Remote:");
                        fileTransfer.requestTreeDirectory().show();
                        break;

                    case "5":
                        System.out.println("-- VIEW LOCAL DIRECTORY --");
                        System.out.println("Local:");
                        new Folder(new File(root)).show();
                        break;

                    case "6":
                        stay = false;
                        break;
                    default:
                        printHelp(root);
                        break;
                }

            }

            System.out.println("bye.");
            fileTransfer.close();

        } catch (Exception e) {
            System.out.println(e.getMessage());
            e.printStackTrace();
        }

    }

}
