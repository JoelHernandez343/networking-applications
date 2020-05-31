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
    /// Interaction logic for TitleBarWindow.xaml
    /// </summary>
    public partial class TitleBarWindow : UserControl
    {
        public String Title
        {
            set
            {
                TitleText.Text = value;
            }
        }

        public bool ShowAbout
        {
            set
            {
                AboutPopup.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool HorizontalCenterTitle
        {
            set
            {
                if (value)
                {
                    TitleText.Margin = new Thickness(0);
                    TitleText.HorizontalAlignment = HorizontalAlignment.Center;
                }
                else
                {
                    TitleText.Margin = new Thickness(74, 0, 0, 0);
                    TitleText.HorizontalAlignment = HorizontalAlignment.Left;
                }
            }
        }

        public TitleBarWindow()
        {
            InitializeComponent();
            Title = "This is a title";
            ShowAbout = true;
            HorizontalCenterTitle = false;
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            var about = new AboutWindow();
            about.Owner = Window.GetWindow(this);
            about.ShowDialog();
        }
    }
}
