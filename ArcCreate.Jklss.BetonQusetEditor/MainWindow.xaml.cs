using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.BetonQusetEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            ScalerTBlock.Text = Math.Round(Scale * 100, 2).ToString() + "%";
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

                outsaid.PreviewMouseMove += Scrollviewer_MouseMove;
            }
            
        }

        private void Scrollviewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released && e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
            {
                if (isTranslateStart)
                {
                    isTranslateStart = false;
                }

                outsaid.PreviewMouseMove -= Scrollviewer_MouseMove;
            }
            
        }

        private void Scrollviewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
            {
                if (isTranslateStart)
                {
                    Point currentPoint = e.GetPosition(outsaid); //不能用 inside，必须用outside
                    Vector v = currentPoint - previousPoint;
                    TransformGroup tg = cvmenu.RenderTransform as TransformGroup;

                    var have = false;

                    foreach (var child in tg.Children)
                    {
                        var t = child as TranslateTransform;

                        if (t != null)
                        {
                            t.X += v.X / Scale;
                            t.Y += v.Y / Scale;

                            have = true;
                            break;
                        }

                    }

                    if (!have)
                    {
                        tg.Children.Add(new TranslateTransform(v.X, v.Y)); //centerX和centerY用外部包装元素的坐标，不能用内部被变换的Canvas元素的坐标
                    }

                    previousPoint = currentPoint;
                }

            }
        }

        private static double Scale = 1;

        private void outside_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Point currentPoint = e.GetPosition(outsaid); //不能用 inside，必须用outside

                TransformGroup tg = cvmenu.RenderTransform as TransformGroup;
                double s = 0;
                if (e.Delta > 0)
                {
                    s = 0.05;
                }
                else
                {
                    s = -0.05;
                }
                

                var have = false;
                foreach (var child in tg.Children)
                {
                    var t = child as ScaleTransform;
                    if(t!= null)
                    {
                        var sx = t.ScaleX + s;

                        if (sx <= 3 && sx >= 0.05)
                        {
                            t.ScaleX += s;
                            Scale = sx;
                            t.ScaleY += s;
                            t.CenterX = currentPoint.X;
                            t.CenterY = currentPoint.Y;
                            ScalerTBlock.Text = Math.Round(Scale * 100, 2).ToString() + "%";
                        }
                        have = true;
                        break;
                    }
                }

                if (!have)
                {
                    //centerX和centerY用外部包装元素的坐标，不能用内部被变换的Canvas元素的坐标
                    tg.Children.Add(new ScaleTransform(s+1.0, s+1.0, currentPoint.X, currentPoint.Y));
                }

                e.Handled = true;
                
            }
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
