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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<PrevControl> items;

        public MainWindow()
        {
            InitializeComponent();
            InitAll();
        }

        private void ErrorConnectionButton_Click(object sender, RoutedEventArgs e) => InitAll();

        public void HideFirstElements()
        {
            MainGrid.Visibility = Visibility.Collapsed;
            ForceUpdateButton.Visibility = Visibility.Collapsed;
        }

        public async void InitAll()
        {
            HideFirstElements();

            ErrorConnectionMessage.Text = "Conectando con 127.0.0.1 en 1234";
            ErrorConnectionTitle.Text = "Conectando";
            ErrorConnectionButton.IsEnabled = false;

            if (!(await Task.Run(() => ServerConnection.Initialize("127.0.0.1", 1234))))
            {
                ErrorConnectionMessage.Text = "Presione este botón para intentar de nuevo";
                ErrorConnectionTitle.Text = "Sin conexión";
                ErrorConnectionButton.IsEnabled = true;
                return;
            }

            NoConnection.Visibility = Visibility.Collapsed;
            MainGrid.Visibility = Visibility.Visible;
            ForceUpdateButton.Visibility = Visibility.Visible;

            UpdateAll();
        }

        private void ForceUpdateButton_Click(object sender, RoutedEventArgs e) => UpdateAll();

        public async void UpdateAll()
        {
            await ItemList.Update();
            UpdateGrid();
        }

        public void UpdateGrid()
        {
            items = new List<PrevControl>();
            MainWrapPanel.Children.Clear();

            for (int i = 0; i < ItemList.List.Count; ++i)
            {
                items.Add(new PrevControl { Item = ItemList.List[i] });
                MainWrapPanel.Children.Add(items[i]);
            }
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            var cart = new CartWindow { Owner = this };
            cart.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ServerConnection.Close();
        }
    }
}
