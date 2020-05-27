package code;

import java.io.File;

public class Archive {

    private String name;

    public Archive(File file){
        name = file.getName();
    }

    public String getName(){
        return name;
    }

}
