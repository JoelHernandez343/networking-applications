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
    /// Interaction logic for MaterialMessageControl.xaml
    /// </summary>
    public partial class MaterialMessageControl : UserControl
    {
        public MaterialMessageControl()
        {
            InitializeComponent();
        }

        public string Title
        {
            set
            {
                TitleText.Text = value;
            }
        }

        public string Message
        {
            set
            {
                MessageText.Text = value;
            }
        }
    }
}
