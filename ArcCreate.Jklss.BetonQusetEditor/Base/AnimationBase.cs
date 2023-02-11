using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ArcCreate.Jklss.BetonQusetEditor.Base
{
    public class AnimationBase
    {
        /// <summary>
        /// 移动动画
        /// </summary>
        /// <param name="top">目标点相对于上端的位置</param>
        /// <param name="left">目标点相对于左端的位置</param>
        /// <param name="elem">移动元素</param>
        public static void FloatInElement(UIElement elem, double top, double left, double duration = .3)
        {
            try
            {
                DoubleAnimation floatY = new DoubleAnimation()
                {
                    To = top,
                    Duration = TimeSpan.FromSeconds(duration)
                };
                DoubleAnimation floatX = new DoubleAnimation()
                {
                    To = left,
                    Duration = TimeSpan.FromSeconds(duration)
                };

                elem.BeginAnimation(Canvas.TopProperty, floatY);
                elem.BeginAnimation(Canvas.LeftProperty, floatX);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 透明度动画
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="to"></param>
        public static void FloatElement(UIElement elem, double to)
        {
            lock (elem)
            {
                if (to == 1)
                {
                    elem.Visibility = Visibility.Visible;
                }
                DoubleAnimation opacity = new DoubleAnimation()
                {
                    To = to,
                    Duration = new TimeSpan(0, 0, 0, 1, 0)
                };
                EventHandler handler = null;
                opacity.Completed += handler = (s, e) =>
                {
                    opacity.Completed -= handler;
                    if (to == 0)
                    {
                        elem.Visibility = Visibility.Collapsed;
                    }
                    opacity = null;
                };
                elem.BeginAnimation(UIElement.OpacityProperty, opacity);
            }
        }
        /// <summary>
        /// 缓动动画-缩放动画
        /// </summary>
        /// <param name="element">控件名</param>
        /// <param name="point">元素开始动画的位置</param>
        /// <param name="from">元素开始的大小</param>
        /// <param name="from">元素到达的大小</param>
        public static void ScaleEasingAnimationShow(FrameworkElement element, Point point, double from, double to)
        {
            lock (element)
            {
                ScaleTransform scale = new ScaleTransform();
                element.RenderTransform = scale;
                element.RenderTransformOrigin = point;//定义圆心位置        
                EasingFunctionBase easeFunction = new PowerEase()
                {
                    EasingMode = EasingMode.EaseOut,
                    Power = 5
                };
                DoubleAnimation scaleAnimation = new DoubleAnimation()
                {
                    From = from,                                   //起始值
                    To = to,                                     //结束值
                    EasingFunction = easeFunction,                    //缓动函数
                    Duration = new TimeSpan(0, 0, 0, 1, 0)  //动画播放时间
                };
                AnimationClock clock = scaleAnimation.CreateClock();
                scale.ApplyAnimationClock(ScaleTransform.ScaleXProperty, clock);
                scale.ApplyAnimationClock(ScaleTransform.ScaleYProperty, clock);
            }
        }
        /// <summary>
        /// 淡入动画 (控件名, 0：上方；1：右方；2：下方；3：左方, 淡入的距离，持续时间)
        /// </summary>
        /// <param name="element">控件名</param>
        /// <param name="direction">0：上方；1：右方；2：下方；3：左方</param>
        /// <param name="distance">淡入的距离</param>
        /// <param name="duration">持续时间</param>
        public static void Appear(FrameworkElement element, int direction = 0, int distance = 20, double duration = .3)
        {

            //将所选控件的Visibility属性改为Visible，这里要首先执行否则动画看不到
            ObjectAnimationUsingKeyFrames VisbilityAnimation = new ObjectAnimationUsingKeyFrames();
            DiscreteObjectKeyFrame kf = new DiscreteObjectKeyFrame(Visibility.Visible, new TimeSpan(0, 0, 0));
            VisbilityAnimation.KeyFrames.Add(kf);
            element.BeginAnimation(Border.VisibilityProperty, VisbilityAnimation);

            //创建新的缩放动画
            TranslateTransform TT = new TranslateTransform();
            element.RenderTransform = TT;
            //创建缩放动画函数，可以自己修改
            EasingFunctionBase easeFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 2 };

            //判断动画方向
            if (direction == 0)
            {
                DoubleAnimation Animation = new DoubleAnimation(-distance, 0, new Duration(TimeSpan.FromSeconds(duration)));
                Animation.EasingFunction = easeFunction;
                element.RenderTransform.BeginAnimation(TranslateTransform.YProperty, Animation);
            }
            else if (direction == 1)
            {
                DoubleAnimation Animation = new DoubleAnimation(distance, 0, new Duration(TimeSpan.FromSeconds(duration)));
                Animation.EasingFunction = easeFunction;
                element.RenderTransform.BeginAnimation(TranslateTransform.XProperty, Animation);
            }
            else if (direction == 2)
            {
                DoubleAnimation Animation = new DoubleAnimation(distance, 0, new Duration(TimeSpan.FromSeconds(duration)));
                Animation.EasingFunction = easeFunction;
                element.RenderTransform.BeginAnimation(TranslateTransform.YProperty, Animation);
            }
            else if (direction == 3)
            {
                DoubleAnimation Animation = new DoubleAnimation(-distance, 0, new Duration(TimeSpan.FromSeconds(duration)));
                Animation.EasingFunction = easeFunction;
                element.RenderTransform.BeginAnimation(TranslateTransform.XProperty, Animation);
            }
            else throw new Exception("无效的方向！");

            //将所选控件的可见度按动画函数方式显现
            DoubleAnimation OpacityAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(duration)));
            OpacityAnimation.EasingFunction = easeFunction;
            element.BeginAnimation(Border.OpacityProperty, OpacityAnimation);
        }

        /// <summary>
        /// 淡出动画(控件名, 0：上方；1：右方；2：下方；3：左方, 淡出的距离，持续时间)
        /// </summary>
        /// <param name="element">控件名</param>
        /// <param name="direction">0：上方；1：右方；2：下方；3：左方</param>
        /// <param name="distance">淡出的距离</param>
        /// <param name="duration">持续时间</param>
        public static void Disappear(FrameworkElement element, int direction = 0, int distance = 20, double duration = .3)
        {
            //创建新的缩放动画
            TranslateTransform TT = new TranslateTransform();
            element.RenderTransform = TT;
            //创建缩放动画函数，可以自己修改
            EasingFunctionBase easeFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 3 };

            //判断动画方向
            if (direction == 0)
            {
                DoubleAnimation Animation = new DoubleAnimation(-distance, new Duration(TimeSpan.FromSeconds(duration)));
                Animation.EasingFunction = easeFunction;
                element.RenderTransform.BeginAnimation(TranslateTransform.YProperty, Animation);
            }
            else if (direction == 1)
            {
                DoubleAnimation Animation = new DoubleAnimation(distance, new Duration(TimeSpan.FromSeconds(duration)));
                Animation.EasingFunction = easeFunction;
                element.RenderTransform.BeginAnimation(TranslateTransform.XProperty, Animation);
            }
            else if (direction == 2)
            {
                DoubleAnimation Animation = new DoubleAnimation(distance, new Duration(TimeSpan.FromSeconds(duration)));
                Animation.EasingFunction = easeFunction;
                element.RenderTransform.BeginAnimation(TranslateTransform.YProperty, Animation);
            }
            else if (direction == 3)
            {
                DoubleAnimation Animation = new DoubleAnimation(-distance, new Duration(TimeSpan.FromSeconds(duration)));
                Animation.EasingFunction = easeFunction;
                element.RenderTransform.BeginAnimation(TranslateTransform.XProperty, Animation);
            }
            else
                throw new Exception("无效的方向！");

            //将所选控件的可见度按动画函数方式消失
            DoubleAnimation OpacityAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(duration)));
            OpacityAnimation.EasingFunction = easeFunction;
            element.BeginAnimation(Border.OpacityProperty, OpacityAnimation);

            //将所选控件的Visibility属性改为Collapsed，这样不占用空间
            ObjectAnimationUsingKeyFrames VisbilityAnimation = new ObjectAnimationUsingKeyFrames();
            DiscreteObjectKeyFrame kf = new DiscreteObjectKeyFrame(Visibility.Hidden, new TimeSpan(0, 0, 1));
            VisbilityAnimation.KeyFrames.Add(kf);
            element.BeginAnimation(Border.VisibilityProperty, VisbilityAnimation);
        }

        public static void WobbleUI(FrameworkElement element, double distanceX = 2, double distanceY = 1,double oldX =0 , double oldY =0, double centerX = 105, double centerY = 50, double duration = .3)
        {
            SkewTransform st = new SkewTransform();
            st.CenterX = centerX;
            st.CenterY = centerY;
            element.RenderTransform = st;  
            EasingFunctionBase easeFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 2 };

            DoubleAnimation AnimationX = new DoubleAnimation(oldX, distanceX, new Duration(TimeSpan.FromSeconds(duration)));
            AnimationX.EasingFunction = easeFunction;

            element.RenderTransform.BeginAnimation(SkewTransform.AngleXProperty, AnimationX);

            DoubleAnimation AnimationY = new DoubleAnimation(oldY, distanceY, new Duration(TimeSpan.FromSeconds(duration)));
            AnimationY.EasingFunction = easeFunction;
            
            element.RenderTransform.BeginAnimation(SkewTransform.AngleYProperty, AnimationY);

        }

        //public static void 
    }
}
