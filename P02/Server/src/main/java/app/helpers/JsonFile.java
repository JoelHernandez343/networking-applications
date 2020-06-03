package app.helpers;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

import java.io.*;
import java.lang.reflect.Type;

public class JsonFile {
    public static String readFile(File file) throws IOException {
        BufferedReader reader = new BufferedReader(new FileReader(file));

        StringBuilder json = new StringBuilder();
        String aux;
        while ((aux = reader.readLine()) != null)
            json.append(aux);

        reader.close();

        return json.toString();
    }

    public static <T> T getListOfFile(File file, Type t) throws IOException {
        String json = readFile(file);
        Gson gson = new Gson();

        return gson.fromJson(json, t);
    }

    public static <T> void writeJsonToFile(File file, T object) throws IOException {
        Gson gson = new GsonBuilder().setPrettyPrinting().create();
        String string = gson.toJson(object);

        OutputStream writer = new FileOutputStream(file, false);
        writer.write(string.getBytes());
        writer.close();
    }
}
