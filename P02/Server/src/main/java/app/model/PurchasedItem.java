package app.model;

import java.util.List;

public class PurchasedItem {
    public String Key;
    public String Name;
    public String Description;
    public boolean HasDiscount;
    public double NormalPrice;
    public double DiscountPrice;
    public int Purchased;

    public PurchasedItem(Item item, int purchased) {
        Key = item.Key;
        Name = item.Name;
        Description = item.Description;
        HasDiscount = item.HasDiscount;
        NormalPrice = item.NormalPrice;
        DiscountPrice = item.DiscountPrice;
        Purchased = purchased;
    }
}
