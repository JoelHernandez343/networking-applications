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
    /// Interaction logic for CartItemControl.xaml
    /// </summary>
    public partial class CartItemControl : UserControl
    {
        private Item item;
        public Item Item
        {
            get => item;
            set
            {
                item = value;
                TitleText.Text = item.Name;

                if (item.HasDiscount)
                {
                    NormalPrice.Visibility = Visibility.Visible;

                    NormalPrice.Text = item.NormalPrice.ToString("C2");
                    DiscountPriceText.Text = item.DiscountPrice.ToString("C2");
                }
                else
                {
                    NormalPrice.Visibility = Visibility.Collapsed;

                    DiscountPriceText.Text = item.NormalPrice.ToString("C2");
                }

                QuantityText.Text = item.Quantity.ToString();
                ReservedText.Text = item.Reserved.ToString();

                MainImage.Source = item.Images[0];
            }
        }

        public int Reserved
        {
            get => Item.Reserved;
            set
            {
                ReservedText.Text = item.Reserved.ToString();
            }
        }

        public CartItemControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);

            var info = new InfoWindow 
            {
                Owner = window,
                Item = Item
            };
            info.ShowDialog();

            (window as CartWindow).UpdateAll();
        }
    }
}
