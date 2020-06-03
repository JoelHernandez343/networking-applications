package app.model;

import com.google.gson.annotations.Expose;

import java.util.ArrayList;
import java.util.List;

public class Item {
    public String Key;
    public String Name;
    public String Description;
    public boolean HasDiscount;
    public double NormalPrice;
    public double DiscountPrice;
    public int Quantity;
    public List<String> ImageNames;
}
