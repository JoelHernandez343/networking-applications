using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Store.Model
{
    public class Item
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool HasDiscount { get; set; }
        public double NormalPrice { get; set; }
        public double DiscountPrice { get; set; }
        public int Quantity { get; set; }
        public List<string> ImageUbications { get; set; }

        public int Reserved { get; set; }

        public List<BitmapSource> Images { get; set; }
    }
}
