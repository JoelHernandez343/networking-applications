using MaterialDesignThemes.Wpf;
using Store.Helpers;
using Store.Helpers.Pdf;
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
    /// Interaction logic for CartWindow.xaml
    /// </summary>
    public partial class CartWindow : Window
    {
        private bool Bought = false;
        private double Total;

        public CartWindow()
        {
            InitializeComponent();
            UpdateAll();
        }

        private List<CartItemControl> items;

        public void UpdateAll()
        {
            items = new List<CartItemControl>();
            CartListStack.Children.Clear();

            Total = 0.0;

            int i = 0;
            foreach (var item in ItemList.List)
            {
                if (item.Reserved > 0)
                {
                    items.Add(new CartItemControl { Item = item });
                    CartListStack.Children.Add(items[i++]);

                    Total += item.Reserved * (item.HasDiscount ? item.DiscountPrice : item.NormalPrice);
                }
            }

            TotalText.Text = Total.ToString("C2");

            BuyButton.IsEnabled = !(items.Count == 0);
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailBox.Text;

            if (!IsValidEmail(email))
            {
                await DialogHost.Show(new MaterialMessageControl 
                { 
                    Title = "Error en el correo", 
                    Message = "Ingrese un correo válido."
                });
                return;
            }

            var card = CardBox.Text;
            if (!int.TryParse(card, out int cardNumber) || cardNumber < 0)
            {
                await DialogHost.Show(new MaterialMessageControl
                {
                    Title = "Error en el número de tarjeta",
                    Message = "Ingrese solo números en el campo de número de tarjeta."
                });
                return;
            }

            var purchased = new List<PurchaseUpdate>();
            foreach (var item in ItemList.List)
            {
                if (item.Reserved > 0)
                {
                    purchased.Add(new PurchaseUpdate { Key = item.Key, Purchased = item.Reserved });
                }
            }

            await ServerConnection.MakePurchase(email, cardNumber, Total, purchased);

            await DialogHost.Show(new MaterialMessageControl
            {
                Title = "Compra exitosa",
                Message = $"Se realizó una compra exitosa por {TotalText.Text}. Generando pdf."
            }) ;

            if (await PdfHelper.CreateAndSaveFile(email, cardNumber))
            {
                await DialogHost.Show(new MaterialMessageControl
                {
                    Title = "Ticket generado y guardado",
                    Message = $"Se guardó su ticket."
                });
            }

            Bought = true;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow window = Owner as MainWindow;

            if (Bought)
                window.UpdateAll();
            else
                window.UpdateGrid();
        }
    }
}
