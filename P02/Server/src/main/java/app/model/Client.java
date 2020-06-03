package app.model;

import com.google.gson.annotations.Expose;

import java.util.ArrayList;
import java.util.List;

public class Client {
    public String Email;
    public List<Purchase> Purchases;

    public Client(String email){
        Email = email;
        Purchases = new ArrayList<Purchase>();
    }
}
