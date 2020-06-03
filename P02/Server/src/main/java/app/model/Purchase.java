package app.model;

import com.google.gson.annotations.Expose;

import java.util.List;

public class Purchase {
    public int CardNumber;
    public double Total;
    public List<PurchasedItem> Items;

    public Purchase(int cardNumber, double total, List<PurchasedItem> items){
        CardNumber = cardNumber;
        Total = total;
        Items = items;
    }
}
