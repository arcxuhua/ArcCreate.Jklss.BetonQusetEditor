using ArcCreate.Jklss.BetonQusetEditor.ViewModel;
using ArcCreate.Jklss.Model.MainWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            this.DataContext = new MainWindowViewModel();//由于架构问题将ViewModel绑定到后台
            MessageBar.Visibility = Visibility.Hidden;

            TransformGroup tg = cvmenu.RenderTransform as TransformGroup;
            tg.Children.Add(new TranslateTransform(-cvmenu.Width/2, -cvmenu.Height/2)); //centerX和centerY用外部包装元素的坐标，不能用内部被变换的Canvas元素的坐标
        }

        Point previousPoint;
        bool isTranslateStart = false;
        private void Scrollviewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
            {
                previousPoint = e.GetPosition(outsaid);
                isTranslateStart = true;
            }
            
        }

        private void Scrollviewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
            {
                if (isTranslateStart)
                {
                    isTranslateStart = false;
                }
            }
            
        }

        private void Scrollviewer_MouseMove(object sender, MouseEventArgs e)
        {
            if ( e.MiddleButton == MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
            {
                if (isTranslateStart)
                {
                    Point currentPoint = e.GetPosition(outsaid); //不能用 inside，必须用outside
                    Vector v = currentPoint - previousPoint;
                    TransformGroup tg = cvmenu.RenderTransform as TransformGroup;
                    tg.Children.Add(new TranslateTransform(v.X, v.Y)); //centerX和centerY用外部包装元素的坐标，不能用内部被变换的Canvas元素的坐标
                    previousPoint = currentPoint;
                }

            }
            
        }

        private void outside_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Point currentPoint = e.GetPosition(outsaid); //不能用 inside，必须用outside

                TransformGroup tg = cvmenu.RenderTransform as TransformGroup;

                double s = ((double)e.Delta) / 1000.0 + 1.0;

                //centerX和centerY用外部包装元素的坐标，不能用内部被变换的Canvas元素的坐标
                tg.Children.Add(new ScaleTransform(s, s, currentPoint.X, currentPoint.Y));

                e.Handled = true;
            }
        }
    }
}
