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
            UpdateAll();
        }

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
    }
}
