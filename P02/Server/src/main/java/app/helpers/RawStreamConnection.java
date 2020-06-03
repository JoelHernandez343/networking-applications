package app.helpers;

import java.io.*;
import java.net.Socket;

public class RawStreamConnection {
    public Socket client;
    InputStream reader;
    OutputStream writer;

    public RawStreamConnection(Socket client) throws IOException {
        this.client = client;

        reader = client.getInputStream();
        writer = client.getOutputStream();

        System.out.println("Connected to " + client.getInetAddress().getHostAddress() + " through port " + client.getPort());
    }

    public void close() throws IOException{
        reader.close();
        writer.close();
        client.close();
    }

    public int readInt() throws IOException{
        byte [] buffer = new byte[4];
        reader.read(buffer, 0, 4);

        int r = 0;
        for (int i = 3; i >= 0; --i)
            r = (r << 8) | (buffer[i] & 0xff);

        return r;
    }

    public long readLong() throws IOException{
        byte [] buffer = new byte[8];
        reader.read(buffer, 0, 8);

        long r = 0;
        for (int i = 7; i >= 0; --i)
            r = (r << 8) | (buffer[i] & 0xff);

        return r;
    }

    public String readUTF() throws IOException{
        int length = readInt();
        byte [] buffer = new byte[length];
        reader.read(buffer, 0, length);

        return new String(buffer, 0, length);
    }

    private void write(byte [] buffer, int length) throws IOException{
        writer.write(buffer, 0, length);
        writer.flush();
    }

    public void write(int val) throws IOException{
        byte [] buffer = new byte[4];
        for (int i = 0; i < 4; ++i)
            buffer[i] = (byte)((val >> (i * 8)) & 0xff);

        write(buffer, buffer.length);
    }

    public void write(long val) throws IOException{
        byte [] buffer = new byte[8];
        for (int i = 0; i < 8; ++i)
            buffer[i] = (byte)((val >> (i * 8)) & 0xff);

        write(buffer, buffer.length);
    }

    public void write(String message) throws IOException{
        write(message.getBytes().length);

        byte [] buffer = message.getBytes();
        write(buffer, buffer.length);
    }

    public void sendFile(File file) throws IOException {
        DataInputStream fileReader = new DataInputStream(new FileInputStream(file));

        write(file.getName());
        write(file.length());

        long sent = 0;
        while (sent < file.length()) {

            byte [] buffer = new byte[4096];
            int readed = fileReader.read(buffer);
            write(buffer, readed);

            sent += readed;
            int percentage = (int)((sent * 100) / file.length());
            System.out.print("\r");
            System.out.print("Sending " + file.getName() + "[" + percentage + "%]");

        }

        System.out.print("\r");
        System.out.println("Sent " + file.getName());

        fileReader.close();
    }
}
