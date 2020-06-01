using Store.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Store.Views.Controls
{
    /// <summary>
    /// Interaction logic for PrevControl.xaml
    /// </summary>
    public partial class PrevControl : UserControl
    {

        private Item item;
        public Item Item
        {
            get => item;
            set
            {
                item = value;
                TitleText.Text = item.Name;
                DescText.Text = item.Description;

                if (item.HasDiscount)
                    DiscountPriceText.Text = item.DiscountPrice.ToString("C0");

                NormalPrice.Text = item.NormalPrice.ToString("C0");
                QuantityText.Text = item.Quantity.ToString();
            }
        }

        public PrevControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var info = new InfoWindow { Owner = Window.GetWindow(this) };
            info.ShowDialog();
        }
    }
}
