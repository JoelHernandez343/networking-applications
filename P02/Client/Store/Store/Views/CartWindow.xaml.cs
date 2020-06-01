using Store.Helpers;
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

            double total = 0.0;

            for (int i = 0, j = 0; i < ItemList.List.Count; ++i)
            {
                if (ItemList.List[i].Reserved > 0)
                {
                    items.Add(new CartItemControl { Item = ItemList.List[i] });
                    CartListStack.Children.Add(items[j++]);

                    if (ItemList.List[i].HasDiscount)
                    {
                        total += ItemList.List[i].DiscountPrice * ItemList.List[i].Reserved;
                    }
                    else
                    {
                        total += ItemList.List[i].NormalPrice * ItemList.List[i].Reserved;
                    }
                }
            }

            TotalText.Text = total.ToString("C0");

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

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailBox.Text;

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Ingrese un correo válido.");
                return;
            }

            var card = CardBox.Text;
            if (!int.TryParse(card, out int cardNumber) || cardNumber < 0)
            {
                MessageBox.Show("Ingrese solo números en el campo de número de tarjeta.");
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
