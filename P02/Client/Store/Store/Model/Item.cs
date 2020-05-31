using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Store.Model
{
    class Item
    {
        public string name;
        public string description;
        public bool hasDiscount;
        public double normalPrice;
        public double discountPrice;
        public int quantity;

        public int reserved;

        public List<BitmapSource> images;
    }
}
