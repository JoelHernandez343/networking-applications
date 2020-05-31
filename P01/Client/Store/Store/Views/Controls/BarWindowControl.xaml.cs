using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Interaction logic for BarWindowControl.xaml
    /// </summary>
    public partial class BarWindowControl : UserControl
    {
        Window Window;

        public BarWindowControl()
        {
            InitializeComponent();

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            Loaded += (s, e) =>
            {
                Window = Window.GetWindow(this);
                Window.SizeChanged += (ss, ee) =>
                {
                    if (Window.WindowState == WindowState.Maximized)
                        MaxResIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowCollapse;
                    else
                        MaxResIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowExpand;
                };
            };

        }

        // Algorithm by leebickmtu and groaner at StackOverflow: https://stackoverflow.com/questions/11703833/dragmove-and-maximize

        bool RestoreIfMove = false;

        private void SwitchState()
        {
            if (Window.WindowState == WindowState.Normal)
                Window.WindowState = WindowState.Maximized;
            else
                Window.WindowState = WindowState.Normal;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (Window.ResizeMode == ResizeMode.CanResize || Window.ResizeMode == ResizeMode.CanResizeWithGrip)
                    SwitchState();
                return;
            }
            else if (Window.WindowState == WindowState.Maximized)
            {
                RestoreIfMove = true;
                return;
            }

            Window.DragMove();
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RestoreIfMove = false;
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (RestoreIfMove)
            {
                RestoreIfMove = false;

                double percentHorizontal = e.GetPosition(this).X / ActualWidth;
                double targetHorizontal = Window.RestoreBounds.Width * percentHorizontal;

                double percentVertical = e.GetPosition(this).Y / ActualHeight;
                double targetVertical = Window.RestoreBounds.Height * percentVertical;

                Window.WindowState = WindowState.Normal;

                GetCursorPos(out POINT lMousePosition);

                Window.Left = lMousePosition.X - targetHorizontal;
                Window.Top = lMousePosition.Y - targetVertical;

                Window.DragMove();
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        private void MaxResButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchState();
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            Window.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window.Close();
        }
    }
}
