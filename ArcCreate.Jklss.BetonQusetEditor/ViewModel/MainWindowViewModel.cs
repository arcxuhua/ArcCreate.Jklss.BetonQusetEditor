using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.BetonQusetEditor.Base.FileLoader;
using ArcCreate.Jklss.BetonQusetEditor.Windows;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.ThumbInfoWindow;
using ArcCreate.Jklss.Model.ThumbModel;
using ArcCreate.Jklss.Model.ThumbModel.CommandModel;
using ArcCreate.Jklss.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static ArcCreate.Jklss.Model.MainWindow.MainWindowModels;
using ComboBox = System.Windows.Controls.ComboBox;
using TextBox = System.Windows.Controls.TextBox;
using TreeView = System.Windows.Controls.TreeView;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel
{
    public class MainWindowViewModel : NotifyBase
    {
        #region 静态全局变量
        private static int Uid = 0;

        private UserActivityBase ActBase = new UserActivityBase();//按键
        public static MainWindowModels mainWindowModels { get; set; } = new MainWindowModels();//绑定Model

        public static MainWindow mainWindow = null;//存储窗体

        private static List<SaveLine> saveLines = new List<SaveLine>();//链接线存储

        private static Thumb nowThumb = null;//当前选中的Thumb

        private static List<double> thumbcanvas = new List<double>();//thumb在Canvas中的相对位置

        public static List<ContisionsCmdModel> contisionProp = new List<ContisionsCmdModel>();//Contitions语法构造器模型

        public static ContisionLoaderBase contisionLoader = new ContisionLoaderBase();

        public static EventLoaderBase eventLoader = new EventLoaderBase();

        public static List<EventCmdModel> eventProp = new List<EventCmdModel>();

        public static ObjectiveLoaderBase objectiveLoader = new ObjectiveLoaderBase();

        public static List<ObjectiveCmdModel> objectiveProp = new List<ObjectiveCmdModel>();

        private static ThumbInfoWindow thumbInfoWindow = null;

        public static SaveResult saveResult = null;

        public static PlayerLoaderBase playerLoader = new PlayerLoaderBase();

        public static NpcLoaderBase npcLoader = new NpcLoaderBase();
        //static MainWindowModel.saveThumbs存储在MainWindowModel中

        #endregion

        #region 属性字段

        public bool isHaveSubjcet
        {
            get
            {
                return mainWindowModels.isHaveSubjcet;
            }
            set
            {
                mainWindowModels.isHaveSubjcet = value;
                this.NotifyChanged();
            }
        }

        /// <summary>
        /// 消息通知
        /// </summary>
        public string Message
        {
            get
            {
                return mainWindowModels.Message;
            }
            set
            {
                mainWindowModels.Message = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        /// <summary>
        /// Main文件地址
        /// </summary>
        public string MainFilePath
        {
            get
            {
                return mainWindowModels.MainFilePath;
            }
            set
            {
                mainWindowModels.MainFilePath = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public string ThumbNums
        {
            get
            {
                return mainWindowModels.ThumbNums;
            }
            set
            {
                mainWindowModels.ThumbNums = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public string ThumbChecks
        {
            get
            {
                return mainWindowModels.ThumbChecks;
            }
            set
            {
                mainWindowModels.ThumbChecks = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        #endregion

        #region 命令

        #region 按钮命令
        /// <summary>
        /// 按键命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ActBase_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Delete)//键盘的Enter，子自行设定。
            {
                if (mainWindow == null)
                {
                    return;
                }

                if (nowThumb == null)
                {
                    return;
                }

                if (System.Windows.MessageBox.Show("你确定将其删除，这将是不可逆的操作？", "删除", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    //父子级同时需要删除该控件
                    var getNeedDeleteThumb = await FindSaveThumbInfo(nowThumb);

                    if (getNeedDeleteThumb == null)
                    {
                        return;
                    }

                    foreach (var item in getNeedDeleteThumb.Children)
                    {
                        var getDeleteThumb = await FindSaveThumbInfo(item);

                        getDeleteThumb.Fathers.Remove(nowThumb);
                    }

                    foreach (var item in getNeedDeleteThumb.Fathers)
                    {
                        var getDeleteThumb = await FindSaveThumbInfo(item);

                        getDeleteThumb.Children.Remove(nowThumb);
                    }

                    //链接线的删除
                    var needDeleteLines = new List<SaveLine>();

                    foreach (var item in saveLines)
                    {
                        if (item.ChirldName == nowThumb || item.FatherName == nowThumb)
                        {
                            needDeleteLines.Add(item);
                        }
                    }

                    foreach (var item in needDeleteLines)
                    {
                        mainWindow.cvmenu.Children.Remove(item.line);

                        saveLines.Remove(item);
                    }

                    mainWindowModels.SaveThumbInfo.Remove(nowThumb);

                    mainWindow.cvmenu.Children.Remove(nowThumb);

                    MainWindowModels.saveThumbs.Remove(getNeedDeleteThumb);

                    nowThumb = null;
                    ShowMessage("删除成功！");
                }
                else
                {
                    ShowMessage("取消了删除~");
                }
            }
        }
        #endregion

        #region ComBox事件绑定

        private RelayCommand<System.Windows.Controls.ComboBox> _ComBoxLoadedCommand;

        public RelayCommand<System.Windows.Controls.ComboBox> ComBoxLoadedCommand
        {
            get
            {
                if (_ComBoxLoadedCommand == null)
                {
                    _ComBoxLoadedCommand = new RelayCommand<System.Windows.Controls.ComboBox>((cb) =>
                    {
                        cb.SelectionChanged += Cb_SelectionChanged;
                        cb.DragEnter += Cb_DragEnter;

                    });
                }
                return _ComBoxLoadedCommand;
            }
            set { _ComBoxLoadedCommand = value; }
        }

        private void Cb_DragEnter(object sender, System.Windows.DragEventArgs e)
        {

        }

        /// <summary>
        /// 当选择改变时触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var thumbClass = (await FindSaveThumbInfo(nowThumb)).thumbClass;

            if(thumbClass!=ThumbClass.Player&&thumbClass != ThumbClass.NPC)
            {
                var getSaver = new List<SaveLine>();

                for (int i = 0; i < saveLines.Count; i++)
                {
                    if (saveLines[i].FatherName == nowThumb)
                    {
                        getSaver.Add(saveLines[i]);
                    }
                }

                var bidui = new GetThumbInfoBase();

                for (int i = 0; i < getSaver.Count; i++)
                {
                    var fatherInfo = await FindSaveThumbInfo(getSaver[i].FatherName);

                    var chirldInfo = await FindSaveThumbInfo(getSaver[i].ChirldName);

                    if (!(await bidui.GetThisCanFather(fatherInfo, chirldInfo)).Succese)//获取改变选项后能否实现归类
                    {
                        //如果不能 则 删除 saveLines 中关系链接及 MainWindowModel.saveThumbs中父级元素和相对应子集元素

                        try
                        {
                            fatherInfo.Children.Remove(getSaver[i].ChirldName);

                            chirldInfo.Fathers.Remove(getSaver[i].FatherName);

                            mainWindow.cvmenu.Children.Remove(getSaver[i].line);

                            saveLines.Remove(getSaver[i]);
                            ThumbChecks = saveLines.Count.ToString();
                        }
                        catch (Exception ex)
                        {
                            ShowMessage("删除时错误具体信息：\n" + ex);
                        }
                    }
                }
            }

            #region 控件的改变
            
            var control = sender as ComboBox;
            switch (thumbClass)
            {
                case ThumbClass.Player:
                    var loaderBacks = await playerLoader.ChangeThumb(control.SelectedItem.ToString(), nowThumb, thumbInfoWindow.TreeView_Tv);

                    if (loaderBacks != null && !loaderBacks.Succese)
                    {
                        ShowMessage(loaderBacks.Text);
                    }

                    break;
                case ThumbClass.NPC:
                    loaderBacks = await npcLoader.ChangeThumb(control.SelectedItem.ToString(), nowThumb, thumbInfoWindow.TreeView_Tv);

                    if (loaderBacks != null && !loaderBacks.Succese)
                    {
                        ShowMessage(loaderBacks.Text);
                    }
                    break;
                case ThumbClass.Conditions:

                    var loaderBack = await contisionLoader.ChangeThumb(contisionProp, control.SelectedItem.ToString(), nowThumb);

                    if (loaderBack != null && !loaderBack.Succese)
                    {
                        ShowMessage(loaderBack.Text);
                    }

                    var back = await ChangeTheTreeView();

                    if (back != null && !back.Succese)
                    {
                        ShowMessage(back.Text);
                    }
                    if (mainWindowModels.SaveThumbInfo.ContainsKey(nowThumb))
                    {
                        mainWindowModels.SaveThumbInfo[nowThumb].Clear();
                    }
                    break;

                case ThumbClass.Events:

                    loaderBack = await eventLoader.ChangeThumb(eventProp, control.SelectedItem.ToString(), nowThumb);

                    if (loaderBack != null && !loaderBack.Succese)
                    {
                        ShowMessage(loaderBack.Text);
                    }

                    back = await ChangeTheTreeView();

                    if (back != null && !back.Succese)
                    {
                        ShowMessage(back.Text);
                    }
                    if (mainWindowModels.SaveThumbInfo.ContainsKey(nowThumb))
                    {
                        mainWindowModels.SaveThumbInfo[nowThumb].Clear();
                    }
                    break;

                case ThumbClass.Objectives:

                    loaderBack = await objectiveLoader.ChangeThumb(objectiveProp, control.SelectedItem.ToString(), nowThumb);

                    if (loaderBack != null && !loaderBack.Succese)
                    {
                        ShowMessage(loaderBack.Text);
                    }

                    back = await ChangeTheTreeView();

                    if (back != null && !back.Succese)
                    {
                        ShowMessage(back.Text);
                    }
                    if (mainWindowModels.SaveThumbInfo.ContainsKey(nowThumb))
                    {
                        mainWindowModels.SaveThumbInfo[nowThumb].Clear();
                    }
                    break;
            }

            
            #endregion
        }

        #endregion

        #region Thumb事件绑定

        /// <summary>
        /// Thumb控件停止拖拽时触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var info = await ThumbClassification(sender as Thumb);

            if (info == null || info.IsThumb == false || info.backs == null)
            {
                return;
            }

            if (System.Windows.MessageBox.Show("你确定将其归类为子元素？", "请选择", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var backs = await ThumbClassOver(info.backs, sender as Thumb);

                if (backs.Succese)
                {
                    ShowMessage("成功归类");
                    DrawThumbLine(sender as Thumb, info.backs);

                }
                else
                {
                    ShowMessage(backs.Text);
                }
            }

            Canvas.SetTop(sender as Thumb, thumbcanvas[0]);
            Canvas.SetLeft(sender as Thumb, thumbcanvas[1]);
        }

        /// <summary>
        /// Thumb控件拖拽时触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb myThumb = (Thumb)sender;
            double nTop = Canvas.GetTop(myThumb) + e.VerticalChange;
            double nLeft = Canvas.GetLeft(myThumb) + e.HorizontalChange;
            if (nTop <= 0)
                nTop = 0;
            if (nTop >= (5044 - myThumb.Height))
                nTop = 5044 - myThumb.Height;
            if (nLeft <= 0)
                nLeft = 0;
            if (nLeft >= (12678 - myThumb.Width))
                nLeft = 12678 - myThumb.Width;

            Canvas.SetTop(myThumb, nTop);
            Canvas.SetLeft(myThumb, nLeft);

            await DrawAllThumpLine(myThumb);
        }

        /// <summary>
        /// Thumb控件开始拖拽时触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            nowThumb = (Thumb)sender;
            Thumb myThumb = (Thumb)sender;
            double nTop = Canvas.GetTop(myThumb);
            double nLeft = Canvas.GetLeft(myThumb);
            thumbcanvas.Clear();
            thumbcanvas.Add(nTop);
            thumbcanvas.Add(nLeft);
        }

        private async void Thumb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var first = false;

            if (nowThumb != (Thumb)sender)
            {
                first = true;
            }

            if (first)
            {
                nowThumb = (Thumb)sender;

                contisionLoader.getThumb = (Thumb)sender;

                eventLoader.getThumb = (Thumb)sender;

                objectiveLoader.getThumb = (Thumb)sender;

                playerLoader.getThumb = (Thumb)sender;

                npcLoader.getThumb = (Thumb)sender;

                var back = await ChangeTheTreeView();

                if (back != null && !back.Succese)
                {
                    ShowMessage(back.Text);
                }
            }

            first = false;
        }

        #endregion

        #region 按钮命令

        /// <summary>
        /// 输出为YML文件
        /// </summary>
        public CommandBase _SaveYaml;
        public CommandBase SaveYaml
        {
            get
            {
                if (_SaveYaml == null)
                {
                    _SaveYaml = new CommandBase();
                    _SaveYaml.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        if (!FileService.IsHaveFile(MainFilePath))
                        {
                            ShowMessage("main入口文件不存在请重新指定Main.yml文件");
                        }

                        var saveBase = new SaveAndReadYamlBase(MainFilePath,objectiveProp,eventProp,contisionProp,saveThumbs,mainWindowModels.SaveThumbInfo);

                        var back = await saveBase.SaveToYaml();

                        ShowMessage(back.Text);
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _SaveYaml;
            }
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        public CommandBase _SelectFilePathCmd;
        public CommandBase SelectFilePathCmd
        {
            get
            {
                if (_SelectFilePathCmd == null)
                {
                    _SelectFilePathCmd = new CommandBase();
                    _SelectFilePathCmd.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
                        dialog.Multiselect = false;//该值确定是否可以选择多个文件
                        dialog.Title = "请选择Main.yml";
                        dialog.Filter = "main入口文件(main.yml)|main.yml";
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            string file = dialog.FileName;
                            MainFilePath = file;
                        }
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _SelectFilePathCmd;
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        public CommandBase _ReadFilePathCmd;
        public CommandBase ReadFilePathCmd
        {
            get
            {
                if (_ReadFilePathCmd == null)
                {
                    _ReadFilePathCmd = new CommandBase();
                    _ReadFilePathCmd.DoExecute = new Action<object>(obj =>//回调函数
                    {

                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _ReadFilePathCmd;
            }
        }

        /// <summary>
        /// 如何创建文件
        /// </summary>
        public CommandBase _CreateFilePathCmd;
        public CommandBase CreateFilePathCmd
        {
            get
            {
                if (_CreateFilePathCmd == null)
                {
                    _CreateFilePathCmd = new CommandBase();
                    _CreateFilePathCmd.DoExecute = new Action<object>(obj =>//回调函数
                    {

                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreateFilePathCmd;
            }
        }

        /// <summary>
        /// 创建新的对话主体
        /// </summary>
        public CommandBase _CreateNewTalkCmd;
        public CommandBase CreateNewTalkCmd
        {
            get
            {
                if (_CreateNewTalkCmd == null)
                {
                    _CreateNewTalkCmd = new CommandBase();
                    _CreateNewTalkCmd.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        if (!string.IsNullOrEmpty(MainFilePath))
                        {
                            var window = obj as MainWindow;

                            var myResourceDictionary = new ResourceDictionary
                            {
                                Source = new Uri("pack://application:,,,/MainWindows/MainWindowDictionary.xaml", UriKind.RelativeOrAbsolute) // 指定样式文件的路径
                            };

                            var myButtonStyle = myResourceDictionary["Main"] as Style; // 通过key找到指定的样式

                            Thumb thumb = new Thumb()
                            {
                                Height = 148,
                                Width = 400,
                                Style = myButtonStyle,
                                Uid = Uid.ToString(),
                            };
                            Uid++;
                            thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                            thumb.DragCompleted += Thumb_DragCompleted;
                            thumb.DragDelta += Thumb_DragDelta;
                            thumb.DragStarted += Thumb_DragStarted;

                            var model = new MainWindowModels.SaveChird()
                            {
                                Saver = thumb,
                                Children = new List<Thumb>(),
                                Fathers = new List<Thumb>(),
                                CanFather = false,
                                Main = thumb,
                                thumbClass = ThumbClass.Subject
                            };

                            MainWindowModels.saveThumbs.Add(model);
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

                            //saveScrollViewer = window.Scrollviewer_Tree;

                            ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                            if (mainWindow == null)
                            {
                                mainWindow = window;
                            }
                            ActBase.KeyDown -= ActBase_KeyDown;
                            ActBase.KeyDown += ActBase_KeyDown;
                            isHaveSubjcet = true;
                        }
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreateNewTalkCmd;
            }
        }

        /// <summary>
        /// 创建新的玩家对话
        /// </summary>
        public CommandBase _CreatePlayerTalkCmd;
        public CommandBase CreatePlayerTalkCmd
        {
            get
            {
                if (_CreatePlayerTalkCmd == null)
                {
                    _CreatePlayerTalkCmd = new CommandBase();
                    _CreatePlayerTalkCmd.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        if (!string.IsNullOrEmpty(MainFilePath))
                        {
                            var window = obj as MainWindow;

                            var myResourceDictionary = new ResourceDictionary
                            {
                                Source = new Uri("pack://application:,,,/MainWindows/MainWindowDictionary.xaml", UriKind.RelativeOrAbsolute) // 指定样式文件的路径
                            };

                            var myButtonStyle = myResourceDictionary["Player"] as Style; // 通过key找到指定的样式

                            Thumb thumb = new Thumb()
                            {
                                Height = 148,
                                Width = 400,
                                Style = myButtonStyle,
                                Uid = Uid.ToString(),
                            };
                            Uid++;

                            thumb.DragCompleted += Thumb_DragCompleted;
                            thumb.DragDelta += Thumb_DragDelta;
                            thumb.DragStarted += Thumb_DragStarted;
                            thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;

                            var model = new MainWindowModels.SaveChird()
                            {
                                Saver = thumb,
                                Children = new List<Thumb>(),
                                Fathers = new List<Thumb>(),
                                CanFather = true,
                                Main = null,
                                thumbClass = ThumbClass.Player
                            };

                            MainWindowModels.saveThumbs.Add(model);
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

                            //saveScrollViewer = window.Scrollviewer_Tree;

                            ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                            var infoModel = new ThumbInfoWindowModel()
                            {
                                TreeItems = new Dictionary<string, Dictionary<string, Dictionary<string, bool>>>
                                {
                                    {"text",new Dictionary<string, Dictionary<string, bool>>()
                                    {
                                        {"第 1 条参数",new Dictionary<string, bool>
                                        {
                                            {"第 1 项",false }
                                        } }
                                    } },
                                    {"conditions",new Dictionary<string, Dictionary<string, bool>>()
                                    {
                                        {"第 1 条参数",new Dictionary<string, bool>
                                        {
                                            {"第 1 项",false }
                                        } }
                                    } },
                                    {"events",new Dictionary<string, Dictionary<string, bool>>()
                                    {
                                        {"第 1 条参数",new Dictionary<string, bool>
                                        {
                                            {"第 1 项",false }
                                        } }
                                    } },
                                    {"pointer",new Dictionary<string, Dictionary<string, bool>>()
                                    {
                                        {"第 1 条参数",new Dictionary<string, bool>
                                        {
                                            {"第 1 项",false }
                                        } }
                                    } },
                                }
                            };


                            playerLoader.saveThumbInfoWindowModel = new Dictionary<Thumb, ThumbInfoWindowModel>()
                            {
                                {thumb,infoModel},
                            };
                        }
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreatePlayerTalkCmd;
            }
        }

        /// <summary>
        /// 创建新的NPC对话
        /// </summary>
        public CommandBase _CreateNpcTalkCmd;
        public CommandBase CreateNpcTalkCmd
        {
            get
            {
                if (_CreateNpcTalkCmd == null)
                {
                    _CreateNpcTalkCmd = new CommandBase();
                    _CreateNpcTalkCmd.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        if (!string.IsNullOrEmpty(MainFilePath))
                        {
                            var window = obj as MainWindow;

                            var myResourceDictionary = new ResourceDictionary
                            {
                                Source = new Uri("pack://application:,,,/MainWindows/MainWindowDictionary.xaml", UriKind.RelativeOrAbsolute) // 指定样式文件的路径
                            };

                            var myButtonStyle = myResourceDictionary["NPC"] as Style; // 通过key找到指定的样式

                            Thumb thumb = new Thumb()
                            {
                                Height = 148,
                                Width = 400,
                                Style = myButtonStyle,
                                Uid = Uid.ToString(),
                            };
                            Uid++;

                            thumb.DragCompleted += Thumb_DragCompleted;
                            thumb.DragDelta += Thumb_DragDelta;
                            thumb.DragStarted += Thumb_DragStarted;
                            thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;

                            var model = new MainWindowModels.SaveChird()
                            {
                                Saver = thumb,
                                Children = new List<Thumb>(),
                                Fathers = new List<Thumb>(),
                                CanFather = true,
                                Main = null,
                                thumbClass = ThumbClass.NPC
                            };

                            MainWindowModels.saveThumbs.Add(model);
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

                            //saveScrollViewer = window.Scrollviewer_Tree;

                            ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                            var infoModel = new ThumbInfoWindowModel()
                            {
                                TreeItems = new Dictionary<string, Dictionary<string, Dictionary<string, bool>>>
                                {
                                    {"text",new Dictionary<string, Dictionary<string, bool>>()
                                    {
                                        {"第 1 条参数",new Dictionary<string, bool>
                                        {
                                            {"第 1 项",false }
                                        } }
                                    } },
                                    {"conditions",new Dictionary<string, Dictionary<string, bool>>()
                                    {
                                        {"第 1 条参数",new Dictionary<string, bool>
                                        {
                                            {"第 1 项",false }
                                        } }
                                    } },
                                    {"events",new Dictionary<string, Dictionary<string, bool>>()
                                    {
                                        {"第 1 条参数",new Dictionary<string, bool>
                                        {
                                            {"第 1 项",false }
                                        } }
                                    } },
                                    {"pointer",new Dictionary<string, Dictionary<string, bool>>()
                                    {
                                        {"第 1 条参数",new Dictionary<string, bool>
                                        {
                                            {"第 1 项",false }
                                        } }
                                    } },
                                }
                            };


                            npcLoader.saveThumbInfoWindowModel = new Dictionary<Thumb, ThumbInfoWindowModel>()
                            {
                                {thumb,infoModel},
                            };
                        }
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreateNpcTalkCmd;
            }
        }

        /// <summary>
        /// 创建新的条件
        /// </summary>
        public CommandBase _CreateConditionsCmd;
        public CommandBase CreateConditionsCmd
        {
            get
            {
                if (_CreateConditionsCmd == null)
                {
                    _CreateConditionsCmd = new CommandBase();
                    _CreateConditionsCmd.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        if (!string.IsNullOrEmpty(MainFilePath))
                        {
                            var window = obj as MainWindow;

                            var myResourceDictionary = new ResourceDictionary
                            {
                                Source = new Uri("pack://application:,,,/MainWindows/MainWindowDictionary.xaml", UriKind.RelativeOrAbsolute) // 指定样式文件的路径
                            };

                            var myButtonStyle = myResourceDictionary["Conditions"] as Style; // 通过key找到指定的样式

                            Thumb thumb = new Thumb()
                            {
                                Height = 148,
                                Width = 400,
                                Style = myButtonStyle,
                                Uid = Uid.ToString(),
                            };
                            Uid++;

                            thumb.DragCompleted += Thumb_DragCompleted;
                            thumb.DragDelta += Thumb_DragDelta;
                            thumb.DragStarted += Thumb_DragStarted;
                            thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;

                            var model = new MainWindowModels.SaveChird()
                            {
                                Saver = thumb,
                                Children = new List<Thumb>(),
                                Fathers = new List<Thumb>(),
                                CanFather = true,
                                Main = null,
                                thumbClass = ThumbClass.Conditions
                            };

                            MainWindowModels.saveThumbs.Add(model);
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

                            //saveScrollViewer = window.Scrollviewer_Tree;

                            ThumbNums = MainWindowModels.saveThumbs.Count.ToString();
                        }
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreateConditionsCmd;
            }
        }

        /// <summary>
        /// 创建新的条件
        /// </summary>
        public CommandBase _CreateEventsCmd;
        public CommandBase CreateEventsCmd
        {
            get
            {
                if (_CreateEventsCmd == null)
                {
                    _CreateEventsCmd = new CommandBase();
                    _CreateEventsCmd.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        if (!string.IsNullOrEmpty(MainFilePath))
                        {
                            var window = obj as MainWindow;

                            var myResourceDictionary = new ResourceDictionary
                            {
                                Source = new Uri("pack://application:,,,/MainWindows/MainWindowDictionary.xaml", UriKind.RelativeOrAbsolute) // 指定样式文件的路径
                            };

                            var myButtonStyle = myResourceDictionary["Events"] as Style; // 通过key找到指定的样式

                            Thumb thumb = new Thumb()
                            {
                                Height = 148,
                                Width = 400,
                                Style = myButtonStyle,
                                Uid = Uid.ToString(),
                            };
                            Uid++;

                            thumb.DragCompleted += Thumb_DragCompleted;
                            thumb.DragDelta += Thumb_DragDelta;
                            thumb.DragStarted += Thumb_DragStarted;
                            thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;

                            var model = new MainWindowModels.SaveChird()
                            {
                                Saver = thumb,
                                Children = new List<Thumb>(),
                                Fathers = new List<Thumb>(),
                                CanFather = true,
                                Main = null,
                                thumbClass = ThumbClass.Events
                            };

                            MainWindowModels.saveThumbs.Add(model);
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

                            //saveScrollViewer = window.Scrollviewer_Tree;

                            ThumbNums = MainWindowModels.saveThumbs.Count.ToString();
                        }
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreateEventsCmd;
            }
        }

        /// <summary>
        /// 创建新的条件
        /// </summary>
        public CommandBase _CreateObjectivesCmd;
        public CommandBase CreateObjectivesCmd
        {
            get
            {
                if (_CreateObjectivesCmd == null)
                {
                    _CreateObjectivesCmd = new CommandBase();
                    _CreateObjectivesCmd.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        if (!string.IsNullOrEmpty(MainFilePath))
                        {
                            var window = obj as MainWindow;

                            var myResourceDictionary = new ResourceDictionary
                            {
                                Source = new Uri("pack://application:,,,/MainWindows/MainWindowDictionary.xaml", UriKind.RelativeOrAbsolute) // 指定样式文件的路径
                            };

                            var myButtonStyle = myResourceDictionary["Objectives"] as Style; // 通过key找到指定的样式

                            Thumb thumb = new Thumb()
                            {
                                Height = 148,
                                Width = 400,
                                Style = myButtonStyle,
                                Uid = Uid.ToString(),
                            };
                            Uid++;

                            thumb.DragCompleted += Thumb_DragCompleted;
                            thumb.DragDelta += Thumb_DragDelta;
                            thumb.DragStarted += Thumb_DragStarted;
                            thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;

                            var model = new MainWindowModels.SaveChird()
                            {
                                Saver = thumb,
                                Children = new List<Thumb>(),
                                Fathers = new List<Thumb>(),
                                CanFather = true,
                                Main = null,
                                thumbClass = ThumbClass.Objectives
                            };

                            MainWindowModels.saveThumbs.Add(model);
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

                            //saveScrollViewer = window.Scrollviewer_Tree;

                            ThumbNums = MainWindowModels.saveThumbs.Count.ToString();
                        }
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreateObjectivesCmd;
            }
        }

        /// <summary>
        /// 创建新的条件
        /// </summary>
        public CommandBase _CreateJournalCmd;
        public CommandBase CreateJournalCmd
        {
            get
            {
                if (_CreateJournalCmd == null)
                {
                    _CreateJournalCmd = new CommandBase();
                    _CreateJournalCmd.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        if (!string.IsNullOrEmpty(MainFilePath))
                        {
                            var window = obj as MainWindow;

                            var myResourceDictionary = new ResourceDictionary
                            {
                                Source = new Uri("pack://application:,,,/MainWindows/MainWindowDictionary.xaml", UriKind.RelativeOrAbsolute) // 指定样式文件的路径
                            };

                            var myButtonStyle = myResourceDictionary["Journal"] as Style; // 通过key找到指定的样式

                            Thumb thumb = new Thumb()
                            {
                                Height = 148,
                                Width = 400,
                                Style = myButtonStyle,
                                Uid = Uid.ToString(),
                            };
                            Uid++;

                            thumb.DragCompleted += Thumb_DragCompleted;
                            thumb.DragDelta += Thumb_DragDelta;
                            thumb.DragStarted += Thumb_DragStarted;
                            thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;

                            var model = new MainWindowModels.SaveChird()
                            {
                                Saver = thumb,
                                Children = new List<Thumb>(),
                                Fathers = new List<Thumb>(),
                                CanFather = true,
                                Main = null,
                                thumbClass = ThumbClass.Journal
                            };

                            MainWindowModels.saveThumbs.Add(model);
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

                            //saveScrollViewer = window.Scrollviewer_Tree;

                            ThumbNums = MainWindowModels.saveThumbs.Count.ToString();
                        }
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreateJournalCmd;
            }
        }

        /// <summary>
        /// 创建新的条件
        /// </summary>
        public CommandBase _CreateItemsCmd;
        public CommandBase CreateItemsCmd
        {
            get
            {
                if (_CreateItemsCmd == null)
                {
                    _CreateItemsCmd = new CommandBase();
                    _CreateItemsCmd.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        if (!string.IsNullOrEmpty(MainFilePath))
                        {
                            var window = obj as MainWindow;

                            var myResourceDictionary = new ResourceDictionary
                            {
                                Source = new Uri("pack://application:,,,/MainWindows/MainWindowDictionary.xaml", UriKind.RelativeOrAbsolute) // 指定样式文件的路径
                            };

                            var myButtonStyle = myResourceDictionary["Items"] as Style; // 通过key找到指定的样式

                            Thumb thumb = new Thumb()
                            {
                                Height = 148,
                                Width = 400,
                                Style = myButtonStyle,
                                Uid = Uid.ToString(),
                            };
                            Uid++;

                            thumb.DragCompleted += Thumb_DragCompleted;
                            thumb.DragDelta += Thumb_DragDelta;
                            thumb.DragStarted += Thumb_DragStarted;
                            thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;

                            var model = new MainWindowModels.SaveChird()
                            {
                                Saver = thumb,
                                Children = new List<Thumb>(),
                                Fathers = new List<Thumb>(),
                                CanFather = true,
                                Main = null,
                                thumbClass = ThumbClass.Items
                            };

                            MainWindowModels.saveThumbs.Add(model);
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

                            //saveScrollViewer = window.Scrollviewer_Tree;

                            ThumbNums = MainWindowModels.saveThumbs.Count.ToString();
                        }
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreateItemsCmd;
            }
        }

        /// <summary>
        /// 所有Save按键
        /// </summary>
        public CommandBase _SaveCommand;
        public CommandBase SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                {
                    _SaveCommand = new CommandBase();
                    _SaveCommand.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        var getInfo = await FindSaveThumbInfo(nowThumb);

                        if (getInfo.thumbClass == ThumbClass.Conditions|| getInfo.thumbClass == ThumbClass.Events|| getInfo.thumbClass == ThumbClass.Objectives
                        || getInfo.thumbClass == ThumbClass.Player|| getInfo.thumbClass == ThumbClass.NPC)
                        {
                            var haveInfoMain = mainWindowModels.SaveThumbInfo.ContainsKey(nowThumb);

                            if (!haveInfoMain)
                            {
                                mainWindowModels.SaveThumbInfo.Add(nowThumb, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>());
                            }

                            var getConditions_CBox = GetControl("Conditions_CBox", nowThumb) as ComboBox;

                            if (getConditions_CBox.SelectedItem == null)
                            {
                                return;
                            }

                            var haveInfoCmd = mainWindowModels.SaveThumbInfo[nowThumb].ContainsKey(getConditions_CBox.SelectedItem.ToString());

                            if (!haveInfoCmd)
                            {
                                if (mainWindowModels.SaveThumbInfo[nowThumb].Count > 0)
                                {
                                    mainWindowModels.SaveThumbInfo[nowThumb].Clear();
                                }

                                mainWindowModels.SaveThumbInfo[nowThumb].Add(getConditions_CBox.SelectedItem.ToString(), new Dictionary<string, Dictionary<string, Dictionary<string, string>>>());
                            }

                            var getConditionsCmdEdit_CBox = GetControl("ConditionsCmdEdit_CBox", nowThumb) as ComboBox;

                            if (getConditionsCmdEdit_CBox.SelectedItem == null)
                            {
                                return;
                            }

                            var haveInfoCmdEdit = mainWindowModels.SaveThumbInfo[nowThumb]
                            [getConditions_CBox.SelectedItem.ToString()].ContainsKey(getConditionsCmdEdit_CBox.SelectedItem.ToString());

                            if (!haveInfoCmdEdit)
                            {
                                mainWindowModels.SaveThumbInfo[nowThumb]
                                [getConditions_CBox.SelectedItem.ToString()].Add(getConditionsCmdEdit_CBox.SelectedItem.ToString(), new Dictionary<string, Dictionary<string, string>>());
                            }

                            var getConditionsCmdparameterEdit_CBox = GetControl("ConditionsCmdparameterEdit_CBox", nowThumb) as ComboBox;

                            if (getConditionsCmdparameterEdit_CBox.SelectedItem == null)
                            {
                                return;
                            }

                            var haveInfoCmdparameterEdit = mainWindowModels.SaveThumbInfo[nowThumb]
                            [getConditions_CBox.SelectedItem.ToString()]
                            [getConditionsCmdEdit_CBox.SelectedItem.ToString()].ContainsKey(getConditionsCmdparameterEdit_CBox.SelectedItem.ToString());

                            if (!haveInfoCmdparameterEdit)
                            {
                                mainWindowModels.SaveThumbInfo[nowThumb]
                                [getConditions_CBox.SelectedItem.ToString()]
                                [getConditionsCmdEdit_CBox.SelectedItem.ToString()].Add(getConditionsCmdparameterEdit_CBox.SelectedItem.ToString(),new Dictionary<string, string>());
                            }

                            var getConditionsCmdProjectEdit_CBox = GetControl("ConditionsCmdProjectEdit_CBox", nowThumb) as ComboBox;

                            if (getConditionsCmdProjectEdit_CBox.SelectedItem == null)
                            {
                                return;
                            }

                            var haveInfoCmdProjectEdit = mainWindowModels.SaveThumbInfo[nowThumb]
                            [getConditions_CBox.SelectedItem.ToString()]
                            [getConditionsCmdEdit_CBox.SelectedItem.ToString()]
                            [getConditionsCmdparameterEdit_CBox.SelectedItem.ToString()].ContainsKey(getConditionsCmdProjectEdit_CBox.SelectedItem.ToString());

                            if (!haveInfoCmdProjectEdit)
                            {
                                mainWindowModels.SaveThumbInfo[nowThumb]
                                [getConditions_CBox.SelectedItem.ToString()]
                                [getConditionsCmdEdit_CBox.SelectedItem.ToString()]
                                [getConditionsCmdparameterEdit_CBox.SelectedItem.ToString()].Add(getConditionsCmdProjectEdit_CBox.SelectedItem.ToString(),null);
                            }

                            var vlue = string.Empty;

                            var getConditions_ComboBox = GetControl("Conditions_ComboBox", nowThumb) as ComboBox;

                            var getConditions_TBox = GetControl("Conditions_TBox", nowThumb) as TextBox;

                            if (getConditions_ComboBox != null)
                            {
                                if (getConditions_ComboBox.Visibility == Visibility.Visible)
                                {
                                    if (getConditions_ComboBox.SelectedItem == null)
                                    {
                                        return;
                                    }

                                    vlue = getConditions_ComboBox.SelectedItem.ToString();
                                }
                                else
                                {
                                    vlue = getConditions_TBox.Text;
                                }
                            }
                            else
                            {
                                vlue = getConditions_TBox.Text;
                            }

                            mainWindowModels.SaveThumbInfo[nowThumb]
                            [getConditions_CBox.SelectedItem.ToString()]
                            [getConditionsCmdEdit_CBox.SelectedItem.ToString()]
                            [getConditionsCmdparameterEdit_CBox.SelectedItem.ToString()]
                            [getConditionsCmdProjectEdit_CBox.SelectedItem.ToString()] = vlue;

                            ShowMessage("保存成功");
                            var treeViewBase = new TreeViewBase();

                            if (mainWindowModels.SaveThumbInfo.ContainsKey(nowThumb))
                            {
                                foreach (var item in mainWindowModels.SaveThumbInfo[nowThumb])//此层只会循环一次
                                {
                                    foreach (var i in item.Value)
                                    {
                                        foreach (var j in i.Value)
                                        {
                                            foreach (var n in j.Value)
                                            {
                                                try
                                                {
                                                    
                                                    if(getInfo.thumbClass == ThumbClass.Conditions)
                                                    {
                                                        contisionLoader.saveThumbInfoWindowModel[nowThumb].TreeItems[i.Key][j.Key][n.Key] = true;
                                                        await treeViewBase.AddItemToTreeView(thumbInfoWindow.TreeView_Tv, i.Key, j.Key, n.Key, true);
                                                    }
                                                    else if(getInfo.thumbClass == ThumbClass.Events)
                                                    {
                                                        eventLoader.saveThumbInfoWindowModel[nowThumb].TreeItems[i.Key][j.Key][n.Key] = true;
                                                        await treeViewBase.AddItemToTreeView(thumbInfoWindow.TreeView_Tv, i.Key, j.Key, n.Key, true);
                                                    }
                                                    else if(getInfo.thumbClass == ThumbClass.Objectives)
                                                    {
                                                        objectiveLoader.saveThumbInfoWindowModel[nowThumb].TreeItems[i.Key][j.Key][n.Key] = true;
                                                        await treeViewBase.AddItemToTreeView(thumbInfoWindow.TreeView_Tv, i.Key, j.Key, n.Key, true);
                                                    }
                                                    else if(getInfo.thumbClass == ThumbClass.Player)
                                                    {
                                                        playerLoader.saveThumbInfoWindowModel[nowThumb].TreeItems[i.Key][j.Key][n.Key] = true;
                                                        await treeViewBase.AddItemToTreeView(thumbInfoWindow.TreeView_Tv, i.Key, j.Key, n.Key, true);
                                                    }
                                                    else if(getInfo.thumbClass == ThumbClass.NPC)
                                                    {
                                                        npcLoader.saveThumbInfoWindowModel[nowThumb].TreeItems[i.Key][j.Key][n.Key] = true;
                                                        await treeViewBase.AddItemToTreeView(thumbInfoWindow.TreeView_Tv, i.Key, j.Key, n.Key, true);
                                                    }
                                                }
                                                catch
                                                {

                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _SaveCommand;
            }
        }

        #endregion

        #region 其他事件绑定

        private RelayCommand<Window> _LoadedCommand;

        public RelayCommand<Window> LoadedCommand
        {
            get
            {
                if (_LoadedCommand == null)
                {
                    _LoadedCommand = new RelayCommand<Window>(async (wd) => {

                        mainWindow = wd as MainWindow;

                        contisionProp = await contisionLoader.Loader();

                        eventProp = await eventLoader.Loader();

                        objectiveProp = await objectiveLoader.Loader();

                        ThumbInfoWindow window = new ThumbInfoWindow();

                        thumbInfoWindow = window;

                        window.WindowStartupLocation = WindowStartupLocation.Manual;

                        window.Left = mainWindow.ActualWidth + mainWindow.Left - 10;

                        window.Top = mainWindow.Top;

                        window.Show();

                        window.DataContext = new ThumbInfoWindowViewModel();

                    });
                }
                return _LoadedCommand;
            }
            set { _LoadedCommand = value; }
        }

        private RelayCommand<Window> _ClosedCommand;

        public RelayCommand<Window> ClosedCommand
        {
            get
            {
                if (_ClosedCommand == null)
                {
                    _ClosedCommand = new RelayCommand<Window>((wd) => {

                        Environment.Exit(0);
                    });
                }
                return _ClosedCommand;
            }
            set { _ClosedCommand = value; }
        }

        private RelayCommand<Window> _LocationChanged;

        public RelayCommand<Window> LocationChanged
        {
            get
            {
                if (_LocationChanged == null)
                {
                    _LocationChanged = new RelayCommand<Window>((wd) => {
                        thumbInfoWindow.Left = mainWindow.ActualWidth + mainWindow.Left - 10;
                        thumbInfoWindow.Top = mainWindow.Top;
                    });
                }
                return _LocationChanged;
            }
            set { _LocationChanged = value; }
        }

        #endregion

        #endregion

        #region 具体方法

        /// <summary>
        /// 更改TreeItem内容(已丢用)
        /// </summary>
        /// <param name="one">命令</param>
        /// <param name="two">参数</param>
        /// <param name="three">项</param>
        /// <returns></returns>
        [Obsolete]
        private async Task<ReturnModel> ChangeTreeItem(string one,string two,string three)
        {
            var result = new ReturnModel();

            if (thumbInfoWindow == null)
            {
                result.SetError();

                return result;
            }

            var tree = thumbInfoWindow.TreeView_Tv;

            await Task.Run(() =>
            {
                var newList = new List<DefinitionNode>();
                foreach (var item in tree.Items)
                {
                    var getTreeItem = item as DefinitionNode;

                    if (getTreeItem.Name == one)
                    {
                        var getTwo = getTreeItem.Children.Where(t => t.Name == two).First();

                        foreach (var child in getTwo.Children)
                        {
                            var getReal = child.Name.Split(new string[] { " ------ " }, StringSplitOptions.RemoveEmptyEntries);

                            if (getReal[0] == three)
                            {
                                child.Name = three + " ------ 已保存";
                                child.FontColor = "#1f640a";
                            }
                        }
                    }

                    newList.Add(getTreeItem);
                }

                tree.Dispatcher.Invoke(new Action(() => {
                    tree.ItemsSource = newList;
                }));
                
            });

            result.SetSuccese();

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

        private async Task<ReturnModel> ChangeTheTreeView()
        {
            var thumbinfo = await FindSaveThumbInfo(nowThumb);

            if (thumbinfo != null && thumbinfo.thumbClass == ThumbClass.Conditions)
            {
                var control = nowThumb.Template.FindName("Conditions_CBox", nowThumb) as ComboBox;

                if (control.Items.Count > 0 && control.SelectedItem != null && thumbInfoWindow != null)
                {
                   return await contisionLoader.ChangeTheTree(thumbInfoWindow.FindName("TreeView_Tv") as TreeView, contisionProp, control.SelectedItem.ToString());
                }

            }

            if (thumbinfo != null && thumbinfo.thumbClass == ThumbClass.Events)
            {
                var control = nowThumb.Template.FindName("Conditions_CBox", nowThumb) as ComboBox;

                if (control.Items.Count > 0 && control.SelectedItem != null && thumbInfoWindow != null)
                {
                    return await eventLoader.ChangeTheTree(thumbInfoWindow.FindName("TreeView_Tv") as TreeView, eventProp, control.SelectedItem.ToString());
                }

            }

            if (thumbinfo != null && thumbinfo.thumbClass == ThumbClass.Objectives)
            {
                var control = nowThumb.Template.FindName("Conditions_CBox", nowThumb) as ComboBox;

                if (control.Items.Count > 0 && control.SelectedItem != null && thumbInfoWindow != null)
                {
                    return await objectiveLoader.ChangeTheTree(thumbInfoWindow.FindName("TreeView_Tv") as TreeView, objectiveProp, control.SelectedItem.ToString());
                }

            }

            if (thumbinfo != null && thumbinfo.thumbClass == ThumbClass.Player)
            {
                return await playerLoader.ChangeTheTree(thumbInfoWindow.FindName("TreeView_Tv") as TreeView);
            }

            if (thumbinfo != null && thumbinfo.thumbClass == ThumbClass.NPC)
            {
                return await npcLoader.ChangeTheTree(thumbInfoWindow.FindName("TreeView_Tv") as TreeView);
            }

            return null;
        }

        /// <summary>
        /// 整理全部Thumb信息为实体类（已弃用）
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        private async Task<ReturnModel> SaveAllThumbs()
        {
            var back = new ReturnModel();

            var createJson = new Dictionary<Thumb,ConversationsModel>();

            var mainModel = new MainConfigModel();

            var conditions = string.Empty;

            var events = string.Empty;

            var objcetives = string.Empty;

            var journal = string.Empty;

            var items = string.Empty;

            foreach (var item in MainWindowModels.saveThumbs)
            {
                if(item.thumbClass == ThumbClass.Subject)
                {
                    var create = new ConversationsModel();

                    create.quester = GetThumbInfoBase.GetThumbInfo(item).Text;

                    create.first = GetThumbInfoBase.GetThumbInfo(await FindSaveThumbInfo(item.Children[0])).Config;
                    
                    create.stop = false;

                    createJson.Add(item.Saver, create);

                    mainModel.npcs.Add(GetThumbInfoBase.GetThumbInfo(item).Config, create.quester);//第一项是NPC名称，第二项是配置名称
                }

                if(item.thumbClass == ThumbClass.NPC)
                {
                    var needCon = new List<SaveChird>();

                    var needEvent = new List<SaveChird>();

                    var needPlayerTalk = new List<SaveChird>();

                    foreach (var getter in item.Children)
                    {
                        var getInfo = await FindSaveThumbInfo(getter);

                        if (getInfo.thumbClass == ThumbClass.Conditions)
                        {
                            needCon.Add(getInfo);
                        }

                        if(getInfo.thumbClass == ThumbClass.Events)
                        {
                            needEvent.Add(getInfo);
                        }

                        if(getInfo.thumbClass == ThumbClass.Player)
                        {
                            needPlayerTalk.Add(getInfo);
                        }
                    }

                    var con = string.Empty;//构建对话需要的条件

                    var even = string.Empty;//构建对话需要的事件

                    var talks = string.Empty;//构建对话需要的回话

                    for (int i = 0; i < needCon.Count; i++)
                    {
                        if (string.IsNullOrEmpty(con))
                        {
                            con += GetThumbInfoBase.GetThumbInfo(needCon[i]).Config;
                        }
                        else
                        {
                            con += "," + GetThumbInfoBase.GetThumbInfo(needCon[i]).Config;
                        }
                    }

                    for (int i = 0; i < needEvent.Count; i++)
                    {
                        if (string.IsNullOrEmpty(even))
                        {
                            even += GetThumbInfoBase.GetThumbInfo(needEvent[i]).Config;
                        }
                        else
                        {
                            even += "," + GetThumbInfoBase.GetThumbInfo(needEvent[i]).Config;
                        }
                    }

                    for (int i = 0; i < needPlayerTalk.Count; i++)
                    {
                        if (string.IsNullOrEmpty(talks))
                        {
                            talks += GetThumbInfoBase.GetThumbInfo(needPlayerTalk[i]).Config;
                        }
                        else
                        {
                            talks += "," + GetThumbInfoBase.GetThumbInfo(needPlayerTalk[i]).Config;
                        }
                    }

                    var config = GetThumbInfoBase.GetThumbInfo(item);

                    var talk = new List<string>(config.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));

                    var newTalk = new AllTalk()
                    {
                        conditions = con,
                        events = even,
                        text = talk,
                        pointer = talks

                    };

                    createJson[item.Main].NPC_options.Add(config.Config, newTalk);

                }

                if(item.thumbClass == ThumbClass.Player)
                {
                    var needCon = new List<SaveChird>();

                    var needEvent = new List<SaveChird>();

                    var needPlayerTalk = new List<SaveChird>();

                    foreach (var getter in item.Children)
                    {
                        var getInfo = await FindSaveThumbInfo(getter);

                        if (getInfo.thumbClass == ThumbClass.Conditions)
                        {
                            needCon.Add(getInfo);
                        }

                        if (getInfo.thumbClass == ThumbClass.Events)
                        {
                            needEvent.Add(getInfo);
                        }

                        if (getInfo.thumbClass == ThumbClass.Player)
                        {
                            needPlayerTalk.Add(getInfo);
                        }
                    }

                    var con = string.Empty;//构建对话需要的条件

                    var even = string.Empty;//构建对话需要的事件

                    var talks = string.Empty;//构建对话需要的回话

                    for (int i = 0; i < needCon.Count; i++)
                    {
                        if (string.IsNullOrEmpty(con))
                        {
                            con += GetThumbInfoBase.GetThumbInfo(needCon[i]).Config;
                        }
                        else
                        {
                            con += "," + GetThumbInfoBase.GetThumbInfo(needCon[i]).Config;
                        }
                    }

                    for (int i = 0; i < needEvent.Count; i++)
                    {
                        if (string.IsNullOrEmpty(even))
                        {
                            even += GetThumbInfoBase.GetThumbInfo(needEvent[i]).Config;
                        }
                        else
                        {
                            even += "," + GetThumbInfoBase.GetThumbInfo(needEvent[i]).Config;
                        }
                    }

                    for (int i = 0; i < needPlayerTalk.Count; i++)
                    {
                        if (string.IsNullOrEmpty(talks))
                        {
                            talks += GetThumbInfoBase.GetThumbInfo(needPlayerTalk[i]).Config;
                        }
                        else
                        {
                            talks += "," + GetThumbInfoBase.GetThumbInfo(needPlayerTalk[i]).Config;
                        }
                    }

                    var config = GetThumbInfoBase.GetThumbInfo(item);

                    var talk = new List<string>(config.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));

                    var newTalk = new AllTalk()
                    {
                        conditions = con,
                        events = even,
                        text = talk,
                        pointer = talks

                    };

                    createJson[item.Main].player_options.Add(config.Config, newTalk);
                }

                if (item.thumbClass == ThumbClass.Conditions)
                {
                    var getInfo = GetThumbInfoBase.GetThumbInfo(item);

                    var makeConditons = string.Empty;

                    var getCon = new List<SaveChird>();//获取其下是否含有条件

                    foreach (var getter in item.Children)
                    {
                        var getGetterInfo = await FindSaveThumbInfo(getter);

                        getCon.Add(getGetterInfo);
                    }

                    foreach (var getter in getCon)
                    {
                        var getConditionInfo = GetThumbInfoBase.GetThumbInfo(getter);

                        if (string.IsNullOrEmpty(makeConditons))
                        {
                            makeConditons += getConditionInfo.Config;
                        }
                        else
                        {
                            makeConditons += ","+ getConditionInfo.Config;
                        }
                    }

                    if (string.IsNullOrEmpty(makeConditons))
                    {
                        conditions += getInfo.Config + ": '"
                                                + getInfo.Type + " "
                                                + getInfo.Text
                                                + "'\r\n";
                    }
                    else
                    {
                        conditions += getInfo.Config + ": '"
                                                + getInfo.Type + " "
                                                + getInfo.Text + " "
                                                + makeConditons
                                                + "'\r\n";
                    }
                }

                if(item.thumbClass == ThumbClass.Events)
                {
                    var getInfo = GetThumbInfoBase.GetThumbInfo(item);

                    var makeConditons = string.Empty;

                    var getCon = new List<SaveChird>();//获取其下是否含有条件

                    foreach (var getter in item.Children)
                    {
                        var getGetterInfo = await FindSaveThumbInfo(getter);

                        getCon.Add(getGetterInfo);
                    }

                    foreach (var getter in getCon)
                    {
                        var getConditionInfo = GetThumbInfoBase.GetThumbInfo(getter);

                        if (string.IsNullOrEmpty(makeConditons))
                        {
                            makeConditons += getConditionInfo.Config;
                        }
                        else
                        {
                            makeConditons += "," + getConditionInfo.Config;
                        }
                    }

                    if (string.IsNullOrEmpty(makeConditons))
                    {
                        events += getInfo.Config + ": '"
                        + getInfo.Type + " "
                        + getInfo.Text
                        + "'\r\n";
                    }
                    else
                    {
                        events += getInfo.Config + ": '"
                        + getInfo.Type + " "
                        + getInfo.Text + " "
                        + makeConditons
                        + "'\r\n";
                    }

                    
                }

                if(item.thumbClass == ThumbClass.Objectives)
                {
                    var getInfo = GetThumbInfoBase.GetThumbInfo(item);

                    var getCon = new List<SaveChird>();//获取其下是否含有事件

                    var getNothing = new List<SaveChird>();

                    foreach (var getter in item.Children)
                    {
                        var getGetterInfo = await FindSaveThumbInfo(getter);

                        if (getGetterInfo.thumbClass == ThumbClass.Events)
                        {
                            getCon.Add(getGetterInfo);
                        }
                        else
                        {
                            getNothing.Add(getGetterInfo);
                        }
                    }

                    var makeConditons = string.Empty;

                    foreach (var getter in getCon)
                    {
                        var getConditionInfo = GetThumbInfoBase.GetThumbInfo(getter);

                        if (string.IsNullOrEmpty(makeConditons))
                        {
                            makeConditons += getConditionInfo.Config;
                        }
                        else
                        {
                            makeConditons += ","+getConditionInfo.Config;
                        }
                    }

                    var makeNoting = string.Empty;

                    foreach (var getter in getNothing)
                    {
                        var getConditionInfo = GetThumbInfoBase.GetThumbInfo(getter);

                        if (string.IsNullOrEmpty(makeConditons))
                        {
                            makeNoting += getConditionInfo.Config;
                        }
                        else
                        {
                            makeNoting += "," + getConditionInfo.Config;
                        }
                    }

                    if (string.IsNullOrEmpty(makeConditons))
                    {
                        if (string.IsNullOrEmpty(makeNoting))
                        {
                            objcetives += getInfo.Config + ": '"
                        + getInfo.Type + " "
                        + getInfo.Text
                        + "'\r\n";
                        }
                        else
                        {
                            objcetives += getInfo.Config + ": '"
                        + getInfo.Type + " "
                        + getInfo.Text + " "
                        + makeNoting
                        + "'\r\n";
                        }
                        
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(makeNoting))
                        {
                            objcetives += getInfo.Config + ": '"
                            + getInfo.Type + " "
                            + getInfo.Text
                            + " events:"
                            + makeConditons
                            + "'\r\n";
                        }
                        else
                        {
                            objcetives += getInfo.Config + ": '"
                            + getInfo.Type + " "
                            + getInfo.Text + " "
                            + makeNoting
                            + " events:"
                            + makeConditons
                            + "'\r\n";
                        }
                        
                    }
                }

                if(item.thumbClass == ThumbClass.Journal)
                {
                    var getInfo = GetThumbInfoBase.GetThumbInfo(item);

                    var fgTalk = getInfo.Text.Split(new string[] { "\r\n"},StringSplitOptions.RemoveEmptyEntries);

                    var makeTalk = string.Empty;

                    for (int i = 0; i < fgTalk.Length; i++)
                    {
                        makeTalk += "  " + fgTalk[i] + "\r\n";
                    }

                    journal += getInfo.Config + ": |\r\n"
                        + makeTalk
                        + "\r\n";
                }

                if(item.thumbClass == ThumbClass.Items)
                {
                    var getInfo = GetThumbInfoBase.GetThumbInfo(item);

                    items += getInfo.Config + ":"
                        + getInfo.Text
                        + "\r\n";
                }
            }

            var backer = new AllConfigModel()
            {
                allTalk = createJson,
                mainConfigModel = mainModel,
                conditions = conditions,
                events = events,
                objcetives = objcetives,
                journal = journal,
                items = items,
            };

            back.SetSuccese("", backer);

            return back;
        }

        /// <summary>
        /// 返回Thumb
        /// </summary>
        private class BackThumb
        {
            public bool IsThumb { get; set; }

            public Thumb backs { get; set; }
        }

        /// <summary>
        /// 两个控件进行归类
        /// </summary>
        /// <param name="thumbFt">父级</param>
        /// <param name="thumbCl">子级</param>
        /// <returns></returns>
        private async Task<ReturnModel> ThumbClassOver(Thumb thumbFt,Thumb thumbCl)
        {
            var back = new ReturnModel();

            var get_ThumbCl = await FindSaveThumbInfo(thumbCl);

            var get_ThumbFt = await FindSaveThumbInfo(thumbFt);
            
            #region 添加条件

            if (get_ThumbCl == null || get_ThumbFt == null)
            {
                back.SetError("获取父级或子级控件错误！\n这是一个严重的问题,请发送邮件报告jk@jklss.cn");
                return back;
            }

            if (get_ThumbCl.Fathers.Contains(thumbFt) || get_ThumbFt.Children.Contains(thumbCl))
            {
                back.SetError("每个控件相同归类仅允许一次");
                return back;
            }

            var comparison = new GetThumbInfoBase();

            var cando = await comparison.GetThisCanFather(get_ThumbFt, get_ThumbCl);

            if (!cando.Succese)
            {
                back.SetError(@"不允许的归类，请查看WIKI https://www.mcbbs.net/forum.php?mod=viewthread&tid=676922");
                return back;
            }

            #endregion

            var result = await AddTextToThumb(get_ThumbFt, get_ThumbCl);

            if (!result.Succese)
            {
                back.SetError(result.Text);
                return back;
            }

            //将父级添加进子级中的父级集合
            get_ThumbCl.Fathers.Add(get_ThumbFt.Saver);

            if (get_ThumbFt.thumbClass == ThumbClass.Subject)
            {
                get_ThumbCl.Main = get_ThumbFt.Saver;

                foreach (var item in get_ThumbCl.Children)
                {
                    var get_ChildThumb = await FindSaveThumbInfo(item);

                    get_ChildThumb.Main = get_ThumbFt.Saver;
                }
            }
            else
            {
                get_ThumbCl.Main = get_ThumbFt.Main;

                foreach (var item in get_ThumbCl.Children)
                {
                    var get_ChildThumb = await FindSaveThumbInfo(item);

                    get_ChildThumb.Main = get_ThumbFt.Main;
                }
            }

            get_ThumbFt.Children.Add(get_ThumbCl.Saver);
            ThumbChecks = saveLines.Count.ToString();
            back.SetSuccese();
            return back;
        }

        private async Task<ReturnModel> AddTextToThumb(SaveChird father,SaveChird chird)
        {
            var result = new ReturnModel();

            if(father.thumbClass!=ThumbClass.Conditions&&father.thumbClass != ThumbClass.Events&& father.thumbClass != ThumbClass.Objectives&&
                father.thumbClass!=ThumbClass.Player&&father.thumbClass!=ThumbClass.NPC)
            {
                result.SetSuccese();

                return result;
            }

            if(string.IsNullOrEmpty((GetControl("ConditionsConfig_TBox", chird.Saver) as TextBox).Text))
            {
                result.SetError("子级的配置名称未填写");
                return result;
            }

            var chirdClass = chird.thumbClass;

            if (father.thumbClass == ThumbClass.Player)
            {
                var cmd = string.Empty;

                var getRealCmd = string.Empty;

                var two = "第 1 条参数";

                switch (chirdClass)
                {
                    case ThumbClass.NPC:
                        cmd = "存储对话: pointer";
                        getRealCmd = "pointer";
                        break;
                    case ThumbClass.Conditions:
                        cmd = "触发条件: conditions";
                        getRealCmd = "conditions";
                        break;
                    case ThumbClass.Events:
                        cmd = "触发事件: events";
                        getRealCmd = "events";
                        break;
                    default:
                        result.SetError("不允许的归类");
                        return result;
                }

                ThumbSetWindow setWindow = new ThumbSetWindow();

                if (chirdClass == ThumbClass.Conditions)
                {
                    setWindow.DataContext = new ThumbSetWindowViewModel()
                    {
                        IsEnabel = true,

                        Classifications = new List<string>
                        {
                            getRealCmd
                        },

                        SaveTerms = new Dictionary<string, List<string>>()
                        {
                            {getRealCmd,new List<string>{ two } }
                        }
                    };
                }
                else
                {
                    setWindow.DataContext = new ThumbSetWindowViewModel()
                    {
                        IsEnabel = false,

                        Classifications = new List<string>
                        {
                            getRealCmd
                        },

                        SaveTerms = new Dictionary<string, List<string>>()
                        {
                            {getRealCmd,new List<string>{ two } }
                        }
                    };
                }

                setWindow.ShowDialog();

                if (!mainWindowModels.SaveThumbInfo.ContainsKey(father.Saver))
                {
                    mainWindowModels.SaveThumbInfo.Add(father.Saver, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>()
                        {
                            { cmd,new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                                {
                                    { saveResult.One,new Dictionary<string, Dictionary<string, string>>()
                                    {
                                        { saveResult.Two,new Dictionary<string, string>()}
                                    } }
                                }
                            }
                        });
                }

                if (!mainWindowModels.SaveThumbInfo[father.Saver].ContainsKey(cmd))
                {
                    mainWindowModels.SaveThumbInfo[father.Saver].Add(cmd, new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                        {
                            { saveResult.One,new Dictionary<string, Dictionary<string, string>>()
                                {
                                    { saveResult.Two,new Dictionary<string, string>()}
                                }
                            }
                        });
                }

                if (!mainWindowModels.SaveThumbInfo[father.Saver][cmd].ContainsKey(saveResult.One))
                {
                    mainWindowModels.SaveThumbInfo[father.Saver][cmd].Add(saveResult.One, new Dictionary<string, Dictionary<string, string>>
                        {
                            { saveResult.Two,new Dictionary<string, string>()}
                        });
                }

                if (!mainWindowModels.SaveThumbInfo[father.Saver][cmd][saveResult.One].ContainsKey(saveResult.Two))
                {
                    mainWindowModels.SaveThumbInfo[father.Saver][cmd][saveResult.One].Add(saveResult.Two, new Dictionary<string, string>
                    {

                    });
                }

                try
                {
                    var newChirldText = (GetControl("ConditionsConfig_TBox", chird.Saver) as TextBox).Text;

                    if (saveResult.Three)
                    {
                        newChirldText = "!" + newChirldText;
                    }

                    var cs = mainWindowModels.SaveThumbInfo[father.Saver]
                        [cmd]
                        [saveResult.One]
                        [saveResult.Two];

                    var num = 0;

                    foreach (var item in cs)
                    {
                        var fgf = item.Key.Split(' ');

                        if (int.Parse(fgf[1]) > num)
                        {
                            num = int.Parse(fgf[1]);//取最大项
                        }
                    }

                    var treeBase = new TreeViewBase();

                    await treeBase.AddItemToSaves(father.Saver, saveResult.One, saveResult.Two,
                        $"第 {num + 1} 项", cmd, newChirldText, true,
                        playerLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);

                    //await treeBase.AddItemToTreeView(thumbInfoWindow.FindName("TreeView_Tv") as TreeView, saveResult.One, saveResult.Two, $"第 {num + 1} 项",true);

                    await treeBase.AddItemToComBox(father.Saver, cmd, saveResult.One, saveResult.Two, $"第 {num + 1} 项");

                    await ChangeTheTreeView();

                    result.SetSuccese();

                    return result;
                }
                catch
                {
                    result.SetError("发生了一个严重的错误，错误码：002");

                    return result;
                }
            }
            else if(father.thumbClass == ThumbClass.NPC)
            {
                var cmd = string.Empty;

                var getRealCmd = string.Empty;

                var two = "第 1 条参数";

                switch (chirdClass)
                {
                    case ThumbClass.Player:
                        cmd = "存储对话: pointer";
                        getRealCmd = "pointer";
                        break;
                    case ThumbClass.Conditions:
                        cmd = "触发条件: conditions";
                        getRealCmd = "conditions";
                        break;
                    case ThumbClass.Events:
                        cmd = "触发事件: events";
                        getRealCmd = "events";
                        break;
                    default:
                        result.SetError("不允许的归类");
                        return result;
                }

                ThumbSetWindow setWindow = new ThumbSetWindow();

                if (chirdClass == ThumbClass.Conditions)
                {
                    setWindow.DataContext = new ThumbSetWindowViewModel()
                    {
                        IsEnabel = true,

                        Classifications = new List<string>
                        {
                            getRealCmd
                        },

                        SaveTerms = new Dictionary<string, List<string>>()
                        {
                            {getRealCmd,new List<string>{ two } }
                        }
                    };
                }
                else
                {
                    setWindow.DataContext = new ThumbSetWindowViewModel()
                    {
                        IsEnabel = false,

                        Classifications = new List<string>
                        {
                            getRealCmd
                        },

                        SaveTerms = new Dictionary<string, List<string>>()
                        {
                            {getRealCmd,new List<string>{ two } }
                        }
                    };
                }

                setWindow.ShowDialog();

                if (!mainWindowModels.SaveThumbInfo.ContainsKey(father.Saver))
                {
                    mainWindowModels.SaveThumbInfo.Add(father.Saver, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>()
                        {
                            { cmd,new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                                {
                                    { saveResult.One,new Dictionary<string, Dictionary<string, string>>()
                                    {
                                        { saveResult.Two,new Dictionary<string, string>()}
                                    } }
                                }
                            }
                        });
                }

                if (!mainWindowModels.SaveThumbInfo[father.Saver].ContainsKey(cmd))
                {
                    mainWindowModels.SaveThumbInfo[father.Saver].Add(cmd, new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                        {
                            { saveResult.One,new Dictionary<string, Dictionary<string, string>>()
                                {
                                    { saveResult.Two,new Dictionary<string, string>()}
                                }
                            }
                        });
                }

                if (!mainWindowModels.SaveThumbInfo[father.Saver][cmd].ContainsKey(saveResult.One))
                {
                    mainWindowModels.SaveThumbInfo[father.Saver][cmd].Add(saveResult.One, new Dictionary<string, Dictionary<string, string>>
                        {
                            { saveResult.Two,new Dictionary<string, string>()}
                        });
                }

                if (!mainWindowModels.SaveThumbInfo[father.Saver][cmd][saveResult.One].ContainsKey(saveResult.Two))
                {
                    mainWindowModels.SaveThumbInfo[father.Saver][cmd][saveResult.One].Add(saveResult.Two, new Dictionary<string, string>
                    {

                    });
                }

                try
                {
                    var newChirldText = (GetControl("ConditionsConfig_TBox", chird.Saver) as TextBox).Text;

                    if (saveResult.Three)
                    {
                        newChirldText = "!" + newChirldText;
                    }

                    var cs = mainWindowModels.SaveThumbInfo[father.Saver]
                        [cmd]
                        [saveResult.One]
                        [saveResult.Two];

                    var num = 0;

                    foreach (var item in cs)
                    {
                        var fgf = item.Key.Split(' ');

                        if (int.Parse(fgf[1]) > num)
                        {
                            num = int.Parse(fgf[1]);//取最大项
                        }
                    }

                    var treeBase = new TreeViewBase();

                    await treeBase.AddItemToSaves(father.Saver, saveResult.One, saveResult.Two,
                        $"第 {num + 1} 项", cmd, newChirldText, true,
                        npcLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);

                    //await treeBase.AddItemToTreeView(thumbInfoWindow.FindName("TreeView_Tv") as TreeView, saveResult.One, saveResult.Two, $"第 {num + 1} 项",true);

                    await treeBase.AddItemToComBox(father.Saver, cmd, saveResult.One, saveResult.Two, $"第 {num + 1} 项");

                    await ChangeTheTreeView();

                    result.SetSuccese();

                    return result;
                }
                catch
                {
                    result.SetError("发生了一个严重的错误，错误码：002");

                    return result;
                }
            }
            
            switch (father.thumbClass)
            {
                case ThumbClass.Conditions:

                    var getComb = GetControl("Conditions_CBox", father.Saver) as ComboBox;

                    if (getComb == null || getComb.SelectedItem == null)
                    {
                        result.SetError("请先选择父级的类型");

                        return result;
                    }

                    var fg = getComb.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    var realCmd = string.Empty;

                    if(fg.Length > 1)
                    {
                        realCmd = fg[2];
                    }
                    else
                    {
                        realCmd = getComb.SelectedItem.ToString();
                    }

                    var getFatherJson = contisionProp.Find(t => t.MainClass == realCmd);

                    var oneList = new List<string>();

                    var twoDic = new Dictionary<string, List<string>>();

                    foreach (var item in getFatherJson.NeedTpye)
                    {
                        var getDic = item.Value;

                        foreach (var i in getDic)
                        {
                            if(i.Value == chird.thumbClass)
                            {
                                oneList.Add(item.Key);

                                if (twoDic.ContainsKey(item.Key))
                                {
                                    twoDic[item.Key].Add($"第 {i.Key+1} 条参数");
                                }
                                else
                                {
                                    twoDic.Add(item.Key, new List<string>()
                                    {
                                        $"第 {i.Key+1} 条参数"
                                    });
                                }
                            }
                        }
                    }

                    if (oneList.Count == 0 || twoDic.Count == 0)
                    {
                        result.SetError("不允许的归类");

                        return result;
                    }

                    ThumbSetWindow setWindow = new ThumbSetWindow();

                    if (chirdClass == ThumbClass.Conditions)
                    {
                        setWindow.DataContext = new ThumbSetWindowViewModel()
                        {
                            IsEnabel = true,

                            Classifications = oneList,

                            SaveTerms = twoDic
                        };
                    }
                    else
                    {
                        setWindow.DataContext = new ThumbSetWindowViewModel()
                        {
                            IsEnabel = false,

                            Classifications = oneList,

                            SaveTerms = twoDic
                        };
                    }

                    setWindow.ShowDialog();

                    if (!mainWindowModels.SaveThumbInfo.ContainsKey(father.Saver))
                    {
                        mainWindowModels.SaveThumbInfo.Add(father.Saver, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>()
                        {
                            { getComb.SelectedItem.ToString(),new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                                {
                                    { saveResult.One,new Dictionary<string, Dictionary<string, string>>()
                                    {
                                        { saveResult.Two,new Dictionary<string, string>()}
                                    } }
                                }
                            }
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver].ContainsKey(getComb.SelectedItem.ToString()))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver].Add(getComb.SelectedItem.ToString(), new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                        {
                            { saveResult.One,new Dictionary<string, Dictionary<string, string>>()
                                {
                                    { saveResult.Two,new Dictionary<string, string>()}
                                }
                            }
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver][getComb.SelectedItem.ToString()].ContainsKey(saveResult.One))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver][getComb.SelectedItem.ToString()].Add(saveResult.One, new Dictionary<string, Dictionary<string, string>>
                        {
                            { saveResult.Two,new Dictionary<string, string>()}
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver][getComb.SelectedItem.ToString()][saveResult.One].ContainsKey(saveResult.Two))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver][getComb.SelectedItem.ToString()][saveResult.One].Add(saveResult.Two, new Dictionary<string, string>
                        {

                        });
                    }

                    try
                    {
                        var newChirldText = (GetControl("ConditionsConfig_TBox", chird.Saver) as TextBox).Text;

                        if (saveResult.Three)
                        {
                            newChirldText = "!" + newChirldText;
                        }

                        var cs = mainWindowModels.SaveThumbInfo[father.Saver]
                            [getComb.SelectedItem.ToString()]
                            [saveResult.One]
                            [saveResult.Two];

                        var num = 0;

                        foreach (var item in cs)
                        {
                            var fgf = item.Key.Split(' ');

                            if (int.Parse(fgf[1]) > num)
                            {
                                num = int.Parse(fgf[1]);//取最大项
                            }
                        }

                        //mainWindowModels.SaveThumbInfo[father.Saver]
                        //    [getComb.SelectedItem.ToString()]
                        //    [saveResult.One]
                        //    [saveResult.Two].Add($"第 {num + 1} 项", newChirldText);

                        var treeBase = new TreeViewBase();

                        await treeBase.AddItemToSaves(father.Saver, saveResult.One, saveResult.Two, 
                            $"第 {num + 1} 项", getComb.SelectedItem.ToString(), newChirldText, true, 
                            contisionLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);

                        //await treeBase.AddItemToTreeView(thumbInfoWindow.FindName("TreeView_Tv") as TreeView, saveResult.One, saveResult.Two, $"第 {num + 1} 项",true);

                        await treeBase.AddItemToComBox(father.Saver, getComb.SelectedItem.ToString(), saveResult.One, saveResult.Two, $"第 {num + 1} 项");

                        await ChangeTheTreeView();

                        result.SetSuccese();

                        return result;
                    }
                    catch
                    {
                        result.SetError("发生了一个严重的错误，错误码：002");

                        return result;
                    }
                case ThumbClass.Events:

                    getComb = GetControl("Conditions_CBox", father.Saver) as ComboBox;

                    if (getComb == null || getComb.SelectedItem == null)
                    {
                        result.SetError("请先选择父级的类型");

                        return result;
                    }

                    fg = getComb.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    realCmd = string.Empty;

                    if (fg.Length > 1)
                    {
                        realCmd = fg[2];
                    }
                    else
                    {
                        realCmd = getComb.SelectedItem.ToString();
                    }

                    var getFatherJson_event = eventProp.Find(t => t.MainClass == realCmd);

                    oneList = new List<string>();

                    twoDic = new Dictionary<string, List<string>>();

                    foreach (var item in getFatherJson_event.NeedTpye)
                    {
                        var getDic = item.Value;

                        foreach (var i in getDic)
                        {
                            if (i.Value == chird.thumbClass)
                            {
                                oneList.Add(item.Key);

                                if (twoDic.ContainsKey(item.Key))
                                {
                                    twoDic[item.Key].Add($"第 {i.Key + 1} 条参数");
                                }
                                else
                                {
                                    twoDic.Add(item.Key, new List<string>()
                                    {
                                        $"第 {i.Key+1} 条参数"
                                    });
                                }
                            }
                        }
                    }

                    if (oneList.Count == 0 || twoDic.Count == 0)
                    {
                        result.SetError("不允许的归类");

                        return result;
                    }

                    setWindow = new ThumbSetWindow();

                    if (chirdClass == ThumbClass.Conditions)
                    {
                        setWindow.DataContext = new ThumbSetWindowViewModel()
                        {
                            IsEnabel = true,

                            Classifications = oneList,

                            SaveTerms = twoDic
                        };
                    }
                    else
                    {
                        setWindow.DataContext = new ThumbSetWindowViewModel()
                        {
                            IsEnabel = false,

                            Classifications = oneList,

                            SaveTerms = twoDic
                        };
                    }

                    setWindow.ShowDialog();

                    if (!mainWindowModels.SaveThumbInfo.ContainsKey(father.Saver))
                    {
                        mainWindowModels.SaveThumbInfo.Add(father.Saver, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>()
                        {
                            { getComb.SelectedItem.ToString(),new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                                {
                                    { saveResult.One,new Dictionary<string, Dictionary<string, string>>()
                                    {
                                        { saveResult.Two,new Dictionary<string, string>()}
                                    } }
                                }
                            }
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver].ContainsKey(getComb.SelectedItem.ToString()))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver].Add(getComb.SelectedItem.ToString(), new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                        {
                            { saveResult.One,new Dictionary<string, Dictionary<string, string>>()
                                {
                                    { saveResult.Two,new Dictionary<string, string>()}
                                }
                            }
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver][getComb.SelectedItem.ToString()].ContainsKey(saveResult.One))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver][getComb.SelectedItem.ToString()].Add(saveResult.One, new Dictionary<string, Dictionary<string, string>>
                        {
                            { saveResult.Two,new Dictionary<string, string>()}
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver][getComb.SelectedItem.ToString()][saveResult.One].ContainsKey(saveResult.Two))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver][getComb.SelectedItem.ToString()][saveResult.One].Add(saveResult.Two, new Dictionary<string, string>
                        {

                        });
                    }

                    try
                    {
                        var newChirldText = (GetControl("ConditionsConfig_TBox", chird.Saver) as TextBox).Text;

                        if (saveResult.Three)
                        {
                            newChirldText = "!" + newChirldText;
                        }

                        var cs = mainWindowModels.SaveThumbInfo[father.Saver]
                            [getComb.SelectedItem.ToString()]
                            [saveResult.One]
                            [saveResult.Two];

                        var num = 0;

                        foreach (var item in cs)
                        {
                            var fgf = item.Key.Split(' ');

                            if (int.Parse(fgf[1]) > num)
                            {
                                num = int.Parse(fgf[1]);//取最大项
                            }
                        }

                        //mainWindowModels.SaveThumbInfo[father.Saver]
                        //    [getComb.SelectedItem.ToString()]
                        //    [saveResult.One]
                        //    [saveResult.Two].Add($"第 {num + 1} 项", newChirldText);

                        var treeBase = new TreeViewBase();

                        await treeBase.AddItemToSaves(father.Saver, saveResult.One, saveResult.Two,
                            $"第 {num + 1} 项", getComb.SelectedItem.ToString(), newChirldText, true,
                            eventLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);

                        //await treeBase.AddItemToTreeView(thumbInfoWindow.FindName("TreeView_Tv") as TreeView, saveResult.One, saveResult.Two, $"第 {num + 1} 项", true);

                        await treeBase.AddItemToComBox(father.Saver, getComb.SelectedItem.ToString(), saveResult.One, saveResult.Two, $"第 {num + 1} 项");

                        result.SetSuccese();

                        return result;
                    }
                    catch
                    {
                        result.SetError("发生了一个严重的错误，错误码：002");

                        return result;
                    }

                case ThumbClass.Objectives:

                    getComb = GetControl("Conditions_CBox", father.Saver) as ComboBox;

                    if (getComb == null || getComb.SelectedItem == null)
                    {
                        result.SetError("请先选择父级的类型");

                        return result;
                    }

                    fg = getComb.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    realCmd = string.Empty;

                    if (fg.Length > 1)
                    {
                        realCmd = fg[2];
                    }
                    else
                    {
                        realCmd = getComb.SelectedItem.ToString();
                    }

                    var getFatherJson_objective = objectiveProp.Find(t => t.MainClass == realCmd);

                    oneList = new List<string>();

                    twoDic = new Dictionary<string, List<string>>();

                    foreach (var item in getFatherJson_objective.NeedTpye)
                    {
                        var getDic = item.Value;

                        foreach (var i in getDic)
                        {
                            if (i.Value == chird.thumbClass)
                            {
                                oneList.Add(item.Key);

                                if (twoDic.ContainsKey(item.Key))
                                {
                                    twoDic[item.Key].Add($"第 {i.Key + 1} 条参数");
                                }
                                else
                                {
                                    twoDic.Add(item.Key, new List<string>() { $"第 {i.Key + 1} 条参数" });
                                }
                            }
                        }
                    }

                    if (oneList.Count == 0 || twoDic.Count == 0)
                    {
                        result.SetError("不允许的归类");

                        return result;
                    }

                    setWindow = new ThumbSetWindow();

                    if (chirdClass == ThumbClass.Conditions)
                    {
                        setWindow.DataContext = new ThumbSetWindowViewModel()
                        {
                            IsEnabel = true,

                            Classifications = oneList,

                            SaveTerms = twoDic
                        };
                    }
                    else
                    {
                        setWindow.DataContext = new ThumbSetWindowViewModel()
                        {
                            IsEnabel = false,

                            Classifications = oneList,

                            SaveTerms = twoDic
                        };
                    }

                    setWindow.ShowDialog();

                    if (!mainWindowModels.SaveThumbInfo.ContainsKey(father.Saver))
                    {
                        mainWindowModels.SaveThumbInfo.Add(father.Saver, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>()
                        {
                            { getComb.SelectedItem.ToString(),new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                                {
                                    { saveResult.One,new Dictionary<string, Dictionary<string, string>>()
                                    {
                                        { saveResult.Two,new Dictionary<string, string>()}
                                    } }
                                }
                            }
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver].ContainsKey(getComb.SelectedItem.ToString()))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver].Add(getComb.SelectedItem.ToString(), new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                        {
                            { saveResult.One,new Dictionary<string, Dictionary<string, string>>()
                                {
                                    { saveResult.Two,new Dictionary<string, string>()}
                                } 
                            }
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver][getComb.SelectedItem.ToString()].ContainsKey(saveResult.One))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver][getComb.SelectedItem.ToString()].Add(saveResult.One, new Dictionary<string, Dictionary<string, string>>
                        {
                            { saveResult.Two,new Dictionary<string, string>()}
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver][getComb.SelectedItem.ToString()][saveResult.One].ContainsKey(saveResult.Two))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver][getComb.SelectedItem.ToString()][saveResult.One].Add(saveResult.Two, new Dictionary<string, string>
                        {

                        });
                    }

                    try
                    {
                        var newChirldText = (GetControl("ConditionsConfig_TBox", chird.Saver) as TextBox).Text;

                        if (saveResult.Three)
                        {
                            newChirldText = "!" + newChirldText;
                        }

                        var cs = mainWindowModels.SaveThumbInfo[father.Saver]
                            [getComb.SelectedItem.ToString()]
                            [saveResult.One]
                            [saveResult.Two];

                        var num = 0;

                        foreach (var item in cs)
                        {
                            var fgf = item.Key.Split(' ');

                            if (int.Parse(fgf[1]) > num)
                            {
                                num = int.Parse(fgf[1]);//取最大项
                            }
                        }

                        //mainWindowModels.SaveThumbInfo[father.Saver]
                        //    [getComb.SelectedItem.ToString()]
                        //    [saveResult.One]
                        //    [saveResult.Two].Add($"第 {num + 1} 项", newChirldText);

                        var treeBase = new TreeViewBase();

                        await treeBase.AddItemToSaves(father.Saver, saveResult.One, saveResult.Two,
                            $"第 {num + 1} 项", getComb.SelectedItem.ToString(), newChirldText, true,
                            objectiveLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);

                        //await treeBase.AddItemToTreeView(thumbInfoWindow.FindName("TreeView_Tv") as TreeView, saveResult.One, saveResult.Two, $"第 {num + 1} 项", true);

                        await treeBase.AddItemToComBox(father.Saver, getComb.SelectedItem.ToString(), saveResult.One, saveResult.Two, $"第 {num + 1} 项");

                        result.SetSuccese();

                        return result;
                    }
                    catch
                    {
                        result.SetError("发生了一个严重的错误，错误码：002");

                        return result;
                    }
            }

            result.SetSuccese();

            return result;
        }

        /// <summary>
        /// 查询被存储在SaveChirld中的信息
        /// </summary>
        /// <returns></returns>
        private async Task<SaveChird> FindSaveThumbInfo(Thumb thumb)
        {
            SaveChird save = null;

            await Task.Run(() =>
            {
                foreach (var item in MainWindowModels.saveThumbs)
                {
                    if (item.Saver == thumb)
                    {
                        save = item;
                        break;
                    }
                }
            });

            return save;
        }

        /// <summary>
        /// 查询Thumb是否被拖拽至其他Thumb中
        /// </summary>
        /// <param name="thumb"></param>
        /// <returns></returns>
        private async Task<BackThumb> ThumbClassification(Thumb thumb)
        {
            BackThumb backInfo = null;

            try
            {
                var points = await GetThumbCoordinate(thumb);

                UIElementCollection childrens = null;
                mainWindow.Dispatcher.Invoke(new Action(() =>
                {
                    childrens = mainWindow.cvmenu.Children;
                }));

                foreach (var item in childrens)
                {
                    var thumbItem = item as Thumb;
                    if (thumbItem != thumb)
                    {
                        try
                        {
                            var thumbPoints = await GetThumbCoordinate(thumbItem);

                            //四点判断是否存在于其他Thumb中
                            if (points[0] > thumbPoints[0] && points[0] < thumbPoints[2] && points[1] > thumbPoints[1] && points[1] < thumbPoints[3])
                            {
                                backInfo = new BackThumb()
                                {
                                    IsThumb = true,
                                    backs = thumbItem
                                };
                                return backInfo;
                            }
                            else if (points[2] > thumbPoints[0] && points[2] < thumbPoints[2] && points[3] > thumbPoints[1] && points[3] < thumbPoints[3])
                            {
                                backInfo = new BackThumb()
                                {
                                    IsThumb = true,
                                    backs = thumbItem
                                };
                                return backInfo;
                            }
                            else if (points[4] > thumbPoints[0] && points[4] < thumbPoints[2] && points[5] > thumbPoints[1] && points[5] < thumbPoints[3])
                            {
                                backInfo = new BackThumb()
                                {
                                    IsThumb = true,
                                    backs = thumbItem
                                };
                                return backInfo;
                            }
                            else if (points[6] > thumbPoints[0] && points[6] < thumbPoints[2] && points[7] > thumbPoints[1] && points[7] < thumbPoints[3])
                            {
                                backInfo = new BackThumb()
                                {
                                    IsThumb = true,
                                    backs = thumbItem
                                };
                                return backInfo;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                return backInfo;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取Thumb元素的四点坐标
        /// </summary>
        /// <param name="thumb"></param>
        /// <returns></returns>
        private async Task<List<double>> GetThumbCoordinate(Thumb thumb)
        {
            var top_x = 0.00;
            var top_y = 0.00;

            var bottom_x = 0.00;
            var bottom_y = 0.00;

            mainWindow.Dispatcher.Invoke(new Action(() =>
            {
                top_x = Canvas.GetTop(thumb);
                top_y = Canvas.GetLeft(thumb);
                bottom_x = top_x + thumb.Height;
                bottom_y = top_y + thumb.Width;
            }));
            List<double> points = await Task.Run(() =>
            {
                List<double> dian = new List<double>();

                

                var right_x = top_x;
                var right_y = bottom_y;

                var left_x = bottom_x;
                var left_y = top_y;

                dian.Add(top_x);
                dian.Add(top_y);
                dian.Add(bottom_x);
                dian.Add(bottom_y);
                dian.Add(right_x);
                dian.Add(right_y);
                dian.Add(left_x);
                dian.Add(left_y);

                return dian;
            });

            return points;
        }

        /// <summary>
        /// 拖动Thumb时改变连接Thumb的线
        /// </summary>
        /// <param name="thumb"></param>
        private async Task DrawAllThumpLine(Thumb thumb)
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < saveLines.Count; i++)
                {
                    if (saveLines[i].ChirldName == thumb)//移动的是子集
                    {
                        saveLines[i].ChirldName = thumb;

                        mainWindow.cvmenu.Dispatcher.Invoke(new Action(() =>
                        {
                            foreach (var item in mainWindow.cvmenu.Children)
                            {
                                var item_Line = item as Line;
                                if (item_Line == saveLines[i].line)
                                {
                                    item_Line.X2 = Canvas.GetLeft(thumb) + thumb.Width / 2 - (Canvas.GetLeft(saveLines[i].FatherName) + saveLines[i].FatherName.Width / 2);
                                    item_Line.Y2 = Canvas.GetTop(thumb) - (Canvas.GetTop(saveLines[i].FatherName) + saveLines[i].FatherName.Height);
                                    Canvas.SetLeft(item_Line, Canvas.GetLeft(saveLines[i].FatherName) + saveLines[i].FatherName.Width / 2);
                                    Canvas.SetTop(item_Line, Canvas.GetTop(saveLines[i].FatherName) + saveLines[i].FatherName.Height);
                                    System.Windows.Controls.Panel.SetZIndex(item_Line, 0);
                                    saveLines[i].line = item_Line;
                                }
                            }
                        }));

                        
                    }
                    if (saveLines[i].FatherName == thumb)//移动的是父级
                    {
                        saveLines[i].FatherName = thumb;
                        mainWindow.cvmenu.Dispatcher.Invoke(new Action(() =>
                        {
                            foreach (var item in mainWindow.cvmenu.Children)
                            {
                                var item_Line = item as Line;
                                if (item_Line == saveLines[i].line)
                                {
                                    item_Line.X2 = Canvas.GetLeft(saveLines[i].ChirldName) + saveLines[i].ChirldName.Width / 2 - (Canvas.GetLeft(thumb) + thumb.Width / 2);
                                    item_Line.Y2 = Canvas.GetTop(saveLines[i].ChirldName) - (Canvas.GetTop(thumb) + thumb.Height);
                                    Canvas.SetLeft(item_Line, Canvas.GetLeft(thumb) + thumb.Width / 2);
                                    Canvas.SetTop(item_Line, Canvas.GetTop(thumb) + thumb.Height);
                                    System.Windows.Controls.Panel.SetZIndex(item_Line, 0);
                                    saveLines[i].line = item_Line;
                                }
                            }
                        }));
                    }
                }
            });
            
        }

        /// <summary>
        /// 首次连接Thumb时绘制线
        /// </summary>
        /// <param name="thumb_1">子集</param>
        /// <param name="thumb_2">父级</param>
        private void DrawThumbLine(Thumb thumb_1, Thumb thumb_2)
        {
            mainWindow.Dispatcher.Invoke(new Action(() =>
            {
                DrawLine(new Point()
                {
                    X = Canvas.GetLeft(thumb_2) + thumb_2.Width / 2,
                    Y = Canvas.GetTop(thumb_2) + thumb_2.Height,
                }, new Point()
                {
                    X = Canvas.GetLeft(thumb_1) + thumb_1.Width / 2,
                    Y = Canvas.GetTop(thumb_1),
                },
                new SaveLine()
                {
                    FatherName = thumb_2,
                    ChirldName = thumb_1
                });
            }));
        }

        /// <summary>
        /// 绘制线条
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="saveLine"></param>
        private void DrawLine(Point p1, Point p2, SaveLine saveLine)
        {
            var line = new Line();
            
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 2;
            line.X1 = 0;
            line.Y1 = 0;
            line.X2 = p2.X - p1.X;
            line.Y2 = p2.Y - p1.Y;
            
            mainWindow.cvmenu.Children.Add(line);
            Canvas.SetZIndex(line, 0);
            Canvas.SetLeft(line, p1.X);
            Canvas.SetTop(line, p1.Y);
            saveLine.line = line;
            saveLines.Add(saveLine);
        }

        private async void ShowMessage(string txt)
        {
            await Task.Run(() =>
            {
                mainWindow.Dispatcher.Invoke(new Action(() =>
                {
                    mainWindow.MessageBar.Visibility = Visibility.Visible;
                    Message = txt;
                    AnimationBase.Appear(mainWindow.MessageBar);
                }));

                Thread.Sleep(3000);


                mainWindow.Dispatcher.Invoke(new Action(() =>
                {
                    AnimationBase.Disappear(mainWindow.MessageBar);
                }));
            });
        }

        #endregion

        public class SaveLine
        {
            public Line line { get; set; }

            public Thumb FatherName { get; set; }

            public Thumb ChirldName { get; set; }
        }

        public class SaveResult
        {
            public string One { get; set; }

            public string Two { get; set; }

            public bool Three { get; set; }
        }
    }
}
