package app;

import app.helpers.StoreServer;

import java.io.File;

public class Server {

    public static void main(String [] args) {
        int port = 1234;
        try {
            StoreServer store = new StoreServer(port, getJarDirectory());
            while (true) {
                store.accept();
                store.storeProcess();
            }
        } catch (Exception e){
            e.printStackTrace();
        }
    }

    static String getJarDirectory() throws Exception {
        return new File(Server.class.getProtectionDomain().getCodeSource().getLocation().toURI()).getPath();
    }
}