﻿using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.MainWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Forms;
using TextBox = System.Windows.Controls.TextBox;
using ArcCreate.Jklss.Model.ThumbModel;

namespace ArcCreate.Jklss.BetonQusetEditor.Base
{
    public class CreateThumbsBase
    {
        private static int Uid = 0;

        public async Task<ReturnModel> CreateThumb(ThumbClass thumbClass,string MainFilePath,MainWindow window)
        {
            var result = new ReturnModel();

            if (string.IsNullOrEmpty(MainFilePath))
            {
                result.SetError("请填写正确的地址");

                return result;
            }

            var myResourceDictionary = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/MainWindows/MainWindowDictionary.xaml", UriKind.RelativeOrAbsolute) // 指定样式文件的路径
            };

            var classes = string.Empty;

            switch (thumbClass)
            {
                case ThumbClass.Player:
                    classes = "Player";
                    break;
                case ThumbClass.NPC:
                    classes = "NPC";
                    break;
                case ThumbClass.Conditions:
                    classes = "Conditions";
                    break;
                case ThumbClass.Events:
                    classes = "Events";
                    break;
                case ThumbClass.Objectives:
                    classes = "Objectives";
                    break;
                case ThumbClass.Journal:
                    classes = "Journal";
                    break;
                case ThumbClass.Items:
                    classes = "Items";
                    break;
                case ThumbClass.Subject:
                    classes = "Main";
                    break;
            }

            var myButtonStyle = myResourceDictionary[classes] as Style; // 通过key找到指定的样式
            Thumb thumb = new Thumb()
            {
                Height = 148,
                Width = 400,
                Style = myButtonStyle,
                Uid = Uid.ToString(),
            };

            var getthumb =await Task.Run(() =>
            {
                Uid++;

                var model = new MainWindowModels.SaveChird()
                {
                    Saver = thumb,
                    Children = new List<Thumb>(),
                    Fathers = new List<Thumb>(),
                    CanFather = true,
                    Main = null,
                    thumbClass = thumbClass
                };

                MainWindowModels.saveThumbs.Add(model);

                window.cvmenu.Dispatcher.Invoke(new Action(() =>
                {
                    window.cvmenu.Children.Add(thumb);
                    try
                    {
                        var getTransForm = window.cvmenu.RenderTransform as TransformGroup;

                        var x = 0.00;
                        var y = 0.00;

                        foreach (var item in getTransForm.Children)
                        {
                            var getScroller = item as TranslateTransform;
                            if (getScroller != null)
                            {
                                x += getScroller.X;
                                y += getScroller.Y;
                            }
                        }

                        Canvas.SetTop(thumb, 262 - thumb.Height / 2 - y);
                        Canvas.SetLeft(thumb, 675.5 - thumb.Width / 2 - x);
                    }
                    catch
                    {
                        Canvas.SetTop(thumb, 262 - thumb.Height / 2);
                        Canvas.SetLeft(thumb, 675.5 - thumb.Width / 2);
                    }
                }));

                return thumb;
            });

            result.SetSuccese("生成成功", getthumb);

            return result;
        }

        /// <summary>
        /// 从Thumb中获取控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="thumb"></param>
        /// <returns></returns>
        protected static object GetControl(string name, Thumb thumb)
        {
            return thumb.Template.FindName(name, thumb);
        }
    }
}