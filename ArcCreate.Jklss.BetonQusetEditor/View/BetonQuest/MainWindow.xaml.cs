using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.BetonQusetEditor.ViewModel;
using ArcCreate.Jklss.BetonQusetEditor.ViewModel.MainWindows;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace ArcCreate.Jklss.BetonQusetEditor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //this.DataContext = new MainWindowViewModel();//由于架构问题将ViewModel绑定到后台    3代

            this.DataContext = new ClientWindowViewModel(this);//由于架构问题将ViewModel绑定到后台   4代

            MessageBar.Visibility = Visibility.Hidden;
            ScalerTBlock.Text = Math.Round(1.00 * 100, 2).ToString() + "%";
            
        }

        private void Move_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        Dictionary<object, Point> saveuse = new Dictionary<object, Point>();

        private void Btn_MouseMove(object sender, MouseEventArgs e)
        {
            var mouseXY = e.GetPosition(sender as Button);

            var x = (mouseXY.X - 105) / 105 * 3.00;

            var y = (mouseXY.Y - 50) / 50 * 2.00;

            if (x < 0.00)
            {
                y = -y;
            }

            var cx = 105 - (mouseXY.X - 105.00);

            var cy = 50 - (mouseXY.Y - 50.00);

            if (!saveuse.TryGetValue(sender, out Point p))
            {
                AnimationBase.WobbleUI(sender as Button, x, y, 0, 0, cx ,cy);
                saveuse.Add(sender, new Point() { X = x, Y = y });
            }
            else
            {
                AnimationBase.WobbleUI(sender as Button, x, y, p.X, p.Y, cx, cy);
                saveuse[sender] = new Point() { X = x,Y= y};
            }
            
        }

        private void Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            AnimationBase.WobbleUI(sender as Button,0,0, saveuse[sender].X, saveuse[sender].Y);
        }
    }
}
