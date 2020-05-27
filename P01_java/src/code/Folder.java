// Created by Joel Hernandez
// Github: https://github.com/JoelHernandez343/AplicacionesDeRedes

package code;

import java.io.File;
import java.util.ArrayList;
import java.util.List;

public class Folder {

    private String folderName;
    private List<Archive> files;
    private List<Folder> folders;

    public Folder(File folder){

        if (!folder.isDirectory()) throw new IllegalArgumentException("Folder provided is not a directory");

        this.folderName = folder.getName();

        this.folders = new ArrayList<>();
        this.files   = new ArrayList<>();
        for (File f : folder.listFiles()) {

            if (f.isDirectory())
                this.folders.add(new Folder(f));
            else
                this.files.add(new Archive(f));

        }

    }

    public List<Archive> listFiles(){
        return files;
    }

    public List<Folder> listFolders(){
        return folders;
    }

    public String getName(){
        return folderName;
    }

    public void show(){
        show("");
    }

    private void show(String tabs){

        System.out.println(tabs + folderName);

        for (Archive a : files)
            System.out.println(tabs + "  " + a.getName() + " (file)");

        for (Folder f : folders)
            f.show(tabs + "  ");

    }

}
