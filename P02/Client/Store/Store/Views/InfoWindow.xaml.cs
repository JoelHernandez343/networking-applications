using MaterialDesignThemes.Wpf;
using Store.Model;
using Store.Views.Controls;
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
using System.Windows.Shapes;

namespace Store.Views
{
    /// <summary>
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
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
                ReservedBox.Text = item.Reserved.ToString();

                // Images
                PrincipalImage.Source = item.Images[0];

                CarrouselStack.Children.Clear();
                foreach (var image in item.Images)
                {
                    CarrouselStack.Children.Add(new CarruselItemControl { Image = image });
                }

            }
        }

        public InfoWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(ReservedBox.Text, out int NewReserved) || NewReserved < 0)
            {
                DialogHost.Show(new MaterialMessageControl
                {
                    Title = "Error en la cantidad reservada",
                    Message = "Ingrese una cantidad entera positiva o 0 en Cantidad Reservada."
                }, "InfoMaterialMessage");
                return;
            }

            if (NewReserved > Item.Quantity)
            {
                DialogHost.Show(new MaterialMessageControl
                {
                    Title = "Error en la cantidad reservada",
                    Message = "No puede reservar más productos de los que hay disponibles."
                }, "InfoMaterialMessage");
                return;
            }

            Item.Reserved = NewReserved;
            Close();
        }
    }
}
