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
using Thumb = System.Windows.Controls.Primitives.Thumb;
using Window = System.Windows.Window;
using System.Windows.Media.Effects;
using static ArcCreate.Jklss.BetonQusetEditor.Base.SaveAndReadYamlBase;
using ArcCreate.Jklss.Model.SocketModel;
using static ArcCreate.Jklss.Model.SocketModel.SocketModel;
using System.Text;
using ArcCreate.Jklss.BetonQusetEditor.Windows.Data;
using ArcCreate.Jklss.BetonQusetEditor.ViewModel.Data;
using ArcCreate.Jklss.Model.Data;
using MessageBox = System.Windows.MessageBox;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel
{
    public class MainWindowViewModel : NotifyBase
    {
        #region 全局变量

        private UserActivityBase ActBase = new UserActivityBase();//按键
        public MainWindowModels mainWindowModels { get; set; } = new MainWindowModels();//绑定Model

        public MainWindow mainWindow = null;//存储窗体

        private List<SaveLine> saveLines = new List<SaveLine>();//链接线存储

        private Thumb nowThumb = null;//当前选中的Thumb

        private List<Thumb> selectThumb = new List<Thumb>();//当前选中的Thumb们

        private List<double> thumbcanvas = new List<double>();//thumb在Canvas中的相对位置

        public List<ContisionsCmdModel> contisionProp = new List<ContisionsCmdModel>();//Contitions语法构造器模型

        public ContisionLoaderBase contisionLoader = null;

        public EventLoaderBase eventLoader = null;

        public List<EventCmdModel> eventProp = new List<EventCmdModel>();

        public ObjectiveLoaderBase objectiveLoader = null;

        public List<ObjectiveCmdModel> objectiveProp = new List<ObjectiveCmdModel>();

        private ThumbInfoWindow thumbInfoWindow = null;

        public static SaveResult saveResult = null;

        public PlayerLoaderBase playerLoader = null;

        public NpcLoaderBase npcLoader = null;

        public GridData SelectData = null;

        //static MainWindowModel.saveThumbs存储在MainWindowModel中

        #endregion

        #region 属性字段

        public string PageName
        {
            get
            {
                return mainWindowModels.PageName;
            }
            set
            {
                mainWindowModels.PageName = value;
                this.NotifyChanged();
            }
        }

        public string LoadingMessage
        {
            get
            {
                return mainWindowModels.LoadingMessage;
            }
            set
            {
                mainWindowModels.LoadingMessage = value;
                this.NotifyChanged();
            }
        }

        public Visibility LoadingShow
        {
            get
            {
                return mainWindowModels.LoadingShow;
            }
            set
            {
                mainWindowModels.LoadingShow = value;
                this.NotifyChanged();
            }
        }

        public string SearchText
        {
            get
            {
                return mainWindowModels.SearchText;
            }
            set
            {
                mainWindowModels.SearchText = value;
                this.NotifyChanged();
            }
        }

        public ThumbClass SearchType
        {
            get
            {
                return mainWindowModels.SearchType;
            }
            set
            {
                mainWindowModels.SearchType = value;
                this.NotifyChanged();
            }
        }

        public List<ThumbClass> SearchListType
        {
            get
            {
                return mainWindowModels.SearchListType;
            }
            set
            {
                mainWindowModels.SearchListType = value;
                this.NotifyChanged();
            }
        }

        public bool IsFindFile
        {
            get
            {
                return mainWindowModels.IsFindFile;
            }
            set
            {
                mainWindowModels.IsFindFile = value;
                this.NotifyChanged();
            }
        }

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

                    var chirld = new List<Thumb>(getNeedDeleteThumb.Children);

                    foreach (var item in chirld)
                    {
                        await ThumbClassOverDelete(nowThumb, item);

                        //var getDeleteThumb = await FindSaveThumbInfo(item);

                        //getDeleteThumb.Fathers.Remove(nowThumb);
                    }

                    var father = new List<Thumb>(getNeedDeleteThumb.Fathers);

                    foreach (var item in father)
                    {
                        await ThumbClassOverDelete(item,nowThumb);

                        //var getDeleteThumb = await FindSaveThumbInfo(item);

                        //getDeleteThumb.Children.Remove(nowThumb);
                    }

                    ////链接线的删除
                    //var needDeleteLines = new List<SaveLine>();

                    //foreach (var item in saveLines)
                    //{
                    //    if (item.ChirldName == nowThumb || item.FatherName == nowThumb)
                    //    {
                    //        needDeleteLines.Add(item);
                    //    }
                    //}

                    //foreach (var item in needDeleteLines)
                    //{
                    //    mainWindow.cvmenu.Children.Remove(item.line);

                    //    saveLines.Remove(item);
                    //}

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
                    _ComBoxLoadedCommand = new RelayCommand<ComboBox>((cb) =>
                    {
                        cb.SelectionChanged += Cb_SelectionChanged;
                    });
                }
                return _ComBoxLoadedCommand;
            }
            set { _ComBoxLoadedCommand = value; }
        }

        bool checkde = false;

        /// <summary>
        /// 当选择改变时触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            checkde = true;
            Thumb nowThumbs = null;
            lock (nowThumb)
            {
                nowThumbs = nowThumb;
            }

            await ComboBoxChangeSeleted(nowThumbs, sender as ComboBox);
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
            if (selectThumb.Count > 0)
            {
                return;
            }

            var info = await ThumbClassification(sender as Thumb);

            if (info == null || info.IsThumb == false || info.backs == null)
            {
                DrawAllThumpLine(sender as Thumb);
                return;
            }

            Canvas.SetTop(sender as Thumb, thumbcanvas[0]);
            Canvas.SetLeft(sender as Thumb, thumbcanvas[1]);

            var get_ThumbCl = await FindSaveThumbInfo(sender as Thumb);

            var get_ThumbFt = await FindSaveThumbInfo(info.backs);

            if (get_ThumbCl.Fathers.Contains(info.backs) || get_ThumbFt.Children.Contains(sender as Thumb))
            {
                if (System.Windows.MessageBox.Show("你确定取消两者的归类？", "请选择", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var back = await ThumbClassOverDelete(info.backs, sender as Thumb);
                    if (back.Succese)
                    {
                        ShowMessage("归类删除成功");

                    }
                    else
                    {
                        ShowMessage(back.Text);
                    }
                }
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
            DrawAllThumpLine(sender as Thumb);
        }

        private static int cfnum = 0;

        /// <summary>
        /// Thumb控件拖拽时触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            cfnum++;
            Thumb myThumb = (Thumb)sender;
            if (selectThumb.Count > 0 && selectThumb.Contains(myThumb))
            {
                for (int i = 0; i < selectThumb.Count; i++)
                {
                    if (cfnum >= 50)
                    {
                        cfnum = 0;

                        DrawAllThumpLine(selectThumb[i]);
                    }
                    double nTop = Canvas.GetTop(selectThumb[i]) + e.VerticalChange;
                    double nLeft = Canvas.GetLeft(selectThumb[i]) + e.HorizontalChange;
                    if (nTop <= 0)
                        nTop = 0;
                    if (nTop >= (504400 - selectThumb[i].Height))
                        nTop = 504400 - selectThumb[i].Height;
                    if (nLeft <= 0)
                        nLeft = 0;
                    if (nLeft >= (1267800 - selectThumb[i].Width))
                        nLeft = 1267800 - selectThumb[i].Width;
                    Canvas.SetTop(selectThumb[i], nTop);
                    Canvas.SetLeft(selectThumb[i], nLeft);
                }
            }
            else
            {
                selectThumb.Clear();

                if (cfnum >= 50)
                {
                    cfnum = 0;

                    DrawAllThumpLine(sender as Thumb);
                }
                double nTop = Canvas.GetTop(myThumb) + e.VerticalChange;
                double nLeft = Canvas.GetLeft(myThumb) + e.HorizontalChange;
                if (nTop <= 0)
                    nTop = 0;
                if (nTop >= (504400 - myThumb.Height))
                    nTop = 504400 - myThumb.Height;
                if (nLeft <= 0)
                    nLeft = 0;
                if (nLeft >= (1267800 - myThumb.Width))
                    nLeft = 1267800 - myThumb.Width;
                Canvas.SetTop(myThumb, nTop);
                Canvas.SetLeft(myThumb, nLeft);
            }

            
        }

        /// <summary>
        /// Thumb控件开始拖拽时触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            nowThumb = (Thumb)sender;

            if (selectThumb.Count > 0&&selectThumb.Contains(nowThumb))
            {
                for (int i = 0; i < selectThumb.Count; i++)
                {
                    (GetControl("ColorModel", selectThumb[i]) as DropShadowEffect).Color = Brushes.GreenYellow.Color;
                }
            }
            else
            {
                selectThumb.Clear();
            }

            double nTop = Canvas.GetTop(nowThumb);
            double nLeft = Canvas.GetLeft(nowThumb);

            thumbcanvas.Clear();
            thumbcanvas.Add(nTop);
            thumbcanvas.Add(nLeft);
        }

        private async void Thumb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (nowThumb != (Thumb)sender)
            {

                if (nowThumb != null)
                {
                    Canvas.SetZIndex(nowThumb, 1);

                    (GetControl("ColorModel", nowThumb) as DropShadowEffect).Color = Brushes.Black.Color;

                    var find = await FindSaveThumbInfo((Thumb)sender);

                    foreach (var item in find.Children)
                    {
                        (GetControl("ColorModel", item) as DropShadowEffect).Color = Brushes.Black.Color;
                    }

                    if (find.Children == null || find.Children.Count == 0)
                    {
                        foreach (var item in MainWindowModels.saveThumbs)
                        {
                            if (item.Saver != nowThumb)
                            {
                                (GetControl("ColorModel", item.Saver) as DropShadowEffect).Color = Brushes.Black.Color;
                            }
                        }
                    }

                    
                }
            }

            nowThumb = (Thumb)sender;

            contisionLoader.getThumb = (Thumb)sender;

            eventLoader.getThumb = (Thumb)sender;

            objectiveLoader.getThumb = (Thumb)sender;

            playerLoader.getThumb = (Thumb)sender;

            npcLoader.getThumb = (Thumb)sender;

            Canvas.SetZIndex((Thumb)sender, 2);

            (GetControl("ColorModel", (Thumb)sender) as DropShadowEffect).Color = Brushes.Red.Color;

            var finds = await FindSaveThumbInfo((Thumb)sender);

            foreach (var item in finds.Children)
            {
                (GetControl("ColorModel", item) as DropShadowEffect).Color = Brushes.Yellow.Color;
            }

            var back = await ChangeTheTreeView();

            if (back != null && !back.Succese)
            {
                ShowMessage(back.Text);
            }


        }

        #endregion

        #region Canvas事件绑定

        private Border currentBoxSelectedBorder = null;//拖动展示的提示框
        private bool isCanMove = false;//鼠标是否移动
        private Point tempStartPoint;//起始坐标

        private void Cvmenu_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isCanMove)
            {
                Point tempEndPoint = e.GetPosition(mainWindow.cvmenu);
                //绘制跟随鼠标移动的方框
                DrawMultiselectBorder(tempEndPoint, tempStartPoint);
            }
        }

        private void Cvmenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            foreach (var child in selectThumb)
            {
                (GetControl("ColorModel", child) as DropShadowEffect).Color = Brushes.Black.Color;
            }
            selectThumb.Clear();

            if (currentBoxSelectedBorder != null)
            {
                
                //获取选框的矩形位置
                Point tempEndPoint = e.GetPosition(mainWindow.cvmenu);
                Rect tempRect = new Rect(tempStartPoint, tempEndPoint);
                //获取子控件

                var list = GetChildObjects<Thumb>(mainWindow.cvmenu);
                foreach (var child in list)
                {//获取子控件矩形位置
                    Rect childRect = new Rect(Canvas.GetLeft(child), Canvas.GetTop(child), child.Width, child.Height);
                    //若子控件与选框相交则改变样式
                    if (childRect.IntersectsWith(tempRect))
                    {
                        (GetControl("ColorModel", child) as DropShadowEffect).Color = Brushes.GreenYellow.Color;
                        selectThumb.Add(child);
                    }
                    
                }
                mainWindow.cvmenu.Children.Remove(currentBoxSelectedBorder);
                currentBoxSelectedBorder = null;
            }

            isCanMove = false;

            mainWindow.cvmenu.MouseMove -= Cvmenu_MouseMove;
        }

        private void Cvmenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isCanMove)
            {
                isCanMove = true;
                tempStartPoint = e.GetPosition(mainWindow.cvmenu);

                mainWindow.cvmenu.MouseMove += Cvmenu_MouseMove;
            }
            
        }

        /// <summary>
        /// 绘制跟随鼠标移动的方框
        /// </summary>
        private void DrawMultiselectBorder(Point endPoint, Point startPoint)
        {
            if (currentBoxSelectedBorder == null)
            {
                currentBoxSelectedBorder = new Border();
                currentBoxSelectedBorder.Background = new SolidColorBrush(new Color() { R = 83, G = 7, B = 179,A=100 });
                currentBoxSelectedBorder.Opacity = 0.4;
                currentBoxSelectedBorder.BorderThickness = new Thickness(1);
                currentBoxSelectedBorder.BorderBrush = new SolidColorBrush(new Color() { R=67,G=11,B=138, A = 100 });
                Canvas.SetZIndex(currentBoxSelectedBorder, 100);
                mainWindow.cvmenu.Children.Add(currentBoxSelectedBorder);
            }
            currentBoxSelectedBorder.Width = Math.Abs(endPoint.X - startPoint.X);
            currentBoxSelectedBorder.Height = Math.Abs(endPoint.Y - startPoint.Y);
            if (endPoint.X - startPoint.X >= 0)
            {
                Canvas.SetLeft(currentBoxSelectedBorder, startPoint.X);

                Console.WriteLine("x:" + startPoint.X);
            }
            else
            {
                Canvas.SetLeft(currentBoxSelectedBorder, endPoint.X);
                Console.WriteLine("x:" + endPoint.X);
            }
               
            if (endPoint.Y - startPoint.Y >= 0)
            {
                Canvas.SetTop(currentBoxSelectedBorder, startPoint.Y);

                Console.WriteLine("y:" + startPoint.Y);
            }
            else
            {
                Canvas.SetTop(currentBoxSelectedBorder, endPoint.Y);

                Console.WriteLine("y:" + endPoint.Y);
            }
                
        }
        /// <summary>
        /// 获得所有子控件
        /// </summary>
        public static List<T> GetChildObjects<T>(System.Windows.DependencyObject obj) where T : System.Windows.FrameworkElement
        {
            System.Windows.DependencyObject child = null;
            List<T> childList = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child is T)
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects<T>(child));
            }
            return childList;
        }

        #endregion

        #region 按键命令

        public CommandBase _GrammarCmd;
        public CommandBase GrammarCmd
        {
            get
            {
                if (_GrammarCmd == null)
                {
                    _GrammarCmd = new CommandBase();
                    _GrammarCmd.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        //GrammarModelWindow window = new GrammarModelWindow();

                        //window.ShowDialog();

                        if(thumbInfoWindow.Visibility == Visibility.Hidden)
                        {
                            thumbInfoWindow.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            thumbInfoWindow.Visibility = Visibility.Hidden;
                        }

                        
                    });//obj是窗口CommandParameter参数传递的值，此处传递为bord
                }
                return _GrammarCmd;
            }
        }

        public CommandBase _SelectSearch;
        public CommandBase SelectSearch
        {
            get
            {
                if (_SelectSearch == null)
                {
                    _SelectSearch = new CommandBase();
                    _SelectSearch.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        var ct = new CreateThumbsBase(mainWindow);

                        var back = await ct.UseNameGetThumb(SearchType, SearchText,true);

                        if (!back.Succese)
                        {
                            ShowMessage("未找到该卡片");
                            return;
                        }

                        var thumbInfo = back.Backs as SaveChird;
                        var y = -Canvas.GetTop(thumbInfo.Saver) + (mainWindow.outsaid.ActualHeight/2)- (thumbInfo.Saver.Height/2);
                        var x = -Canvas.GetLeft(thumbInfo.Saver)+ (mainWindow.outsaid.ActualWidth/2) - (thumbInfo.Saver.Width/2);
                        Console.WriteLine($"卡片 X:{x} Y:{y}");
                        TransformGroup tg = mainWindow.cvmenu.RenderTransform as TransformGroup;

                        var have = false;

                        foreach (var child in tg.Children)
                        {
                            var t = child as TranslateTransform;

                            if (t != null)
                            {
                                Console.WriteLine($"视角 X:{t.X} Y:{t.Y}");
                                t.X = x;
                                t.Y = y;

                                have = true;
                                break;
                            }

                        }

                        if (!have)
                        {
                            tg.Children.Add(new TranslateTransform(x, y)); //centerX和centerY用外部包装元素的坐标，不能用内部被变换的Canvas元素的坐标
                        }
                    });//obj是窗口CommandParameter参数传递的值，此处传递为bord
                }
                return _SelectSearch;
            }
        }

        public CommandBase _SwitchCmd;
        public CommandBase SwitchCmd
        {
            get
            {
                if (_SwitchCmd == null)
                {
                    _SwitchCmd = new CommandBase();
                    _SwitchCmd.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        var getBacks = await SelectFilePath();

                        if (!getBacks.Succese)
                        {
                            return;
                        }

                        MainFilePath = getBacks.Text;
                    });//obj是窗口CommandParameter参数传递的值，此处传递为bord
                }
                return _SwitchCmd;
            }
        }

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
                        mainWindow.IsEnabled = false;
                        LoadingShow = Visibility.Visible;

                        LoadingMessage = "正在处理~";
                        if (!FileService.IsHaveFile(MainFilePath))
                        {
                            mainWindow.IsEnabled = true;
                            LoadingShow = Visibility.Hidden;
                            ShowMessage("main入口文件不存在请重新指定Main.yml文件");
                        }

                        var saveBase = new SaveAndReadYamlBase(MainFilePath, objectiveProp, eventProp, contisionProp, saveThumbs, mainWindowModels.SaveThumbInfo);

                        var back = await saveBase.SaveToJson(SelectData.Name, SelectData.Code);

                        if (!back.Succese)
                        {
                            mainWindow.IsEnabled = true;
                            LoadingShow = Visibility.Hidden;
                            ShowMessage(back.Text);
                            return;
                        }

                        var message = new MessageModel()
                        {
                            IsLogin = SocketModel.isLogin,
                            JsonInfo = JsonInfo.SaveToYaml,
                            UserName = SocketModel.userName,
                            Message = FileService.SaveToJson(back.Backs),
                        };

                        var jsonMessage = FileService.SaveToJson(message);

                        var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);

                        if (getMessage == null || !getMessage.Succese)
                        {
                            mainWindow.IsEnabled = true;
                            LoadingShow = Visibility.Hidden;
                            ShowMessage("保存失败，请尝试重新保存");
                            return;
                        }

                        var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

                        if (getModel.Token != SocketModel.token)
                        {
                            mainWindow.IsEnabled = true;
                            LoadingShow = Visibility.Hidden;
                            ShowMessage("您的Token异常请重新登录后保存");
                            return;
                        }

                        var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

                        if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.SaveToYaml || !getRealMessage.IsLogin)
                        {
                            if (getRealMessage != null)
                            {
                                mainWindow.IsEnabled = true;
                                LoadingShow = Visibility.Hidden;
                                ShowMessage(getRealMessage.Message);
                                return;
                            }
                            else
                            {
                                mainWindow.IsEnabled = true;
                                LoadingShow = Visibility.Hidden;
                                ShowMessage("服务器异常");
                                return;
                            }
                            
                        }
                        if (this.SelectData.Code == -1)
                        {
                            this.SelectData.Code = Convert.ToInt32(getRealMessage.Other);
                        }

                        ShowMessage("配置保存至云端成功，正在输出YML至本地");

                        await Task.Run(() =>
                        {
                            Thread.Sleep(2000);
                        });

                        var result = await saveBase.InputToYaml(FileService.JsonToProp<YamlSaver>(getRealMessage.Message));
                        mainWindow.IsEnabled = true;
                        LoadingShow = Visibility.Hidden;
                        ShowMessage(result.Text);
                        
                        return;
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _SaveYaml;
            }
        }

        /// <summary>
        /// 输出为Json文件
        /// </summary>
        public CommandBase _SaveJson;
        public CommandBase SaveJson
        {
            get
            {
                if (_SaveJson == null)
                {
                    _SaveJson = new CommandBase();
                    _SaveJson.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        mainWindow.IsEnabled = false;
                        LoadingShow = Visibility.Visible;

                        LoadingMessage = "正在处理~";
                        if (!FileService.IsHaveFile(MainFilePath))
                        {
                            mainWindow.IsEnabled = true;
                            LoadingShow = Visibility.Hidden;
                            ShowMessage("main入口文件不存在请重新指定Main.yml文件");
                        }

                        var saveBase = new SaveAndReadYamlBase(MainFilePath, objectiveProp, eventProp, contisionProp, saveThumbs, mainWindowModels.SaveThumbInfo);

                        var back = await saveBase.SaveToJson(SelectData.Name, SelectData.Code);

                        if (!back.Succese)
                        {
                            mainWindow.IsEnabled = true;
                            LoadingShow = Visibility.Hidden;
                            ShowMessage(back.Text);
                            return;
                        }
                        
                        var message = new MessageModel()
                        {
                            IsLogin = SocketModel.isLogin,
                            JsonInfo = JsonInfo.SaveToJson,
                            UserName = SocketModel.userName,
                            Message = FileService.SaveToJson(back.Backs),
                        };

                        var jsonMessage = FileService.SaveToJson(message);

                        var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);

                        if (getMessage == null || !getMessage.Succese)
                        {
                            mainWindow.IsEnabled = true;
                            LoadingShow = Visibility.Hidden;
                            ShowMessage("保存失败，请尝试重新保存");
                            return;
                        }

                        var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

                        if (getModel.Token != SocketModel.token)
                        {
                            mainWindow.IsEnabled = true;
                            LoadingShow = Visibility.Hidden;
                            ShowMessage("您的Token异常请重新登录后保存");
                            return;
                        }

                        var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

                        if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.SaveToJson || !getRealMessage.IsLogin)
                        {
                            mainWindow.IsEnabled = true;
                            LoadingShow = Visibility.Hidden;
                            ShowMessage("服务器异常");
                            return;
                        }

                        if (this.SelectData.Code == -1)
                        {
                            this.SelectData.Code = Convert.ToInt32(getRealMessage.Other);
                        }
                        mainWindow.IsEnabled = true;
                        LoadingShow = Visibility.Hidden;
                        ShowMessage("配置保存至云端成功");
                        
                        return;

                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _SaveJson;
            }
        }

        /// <summary>
        /// 读取YAML文件
        /// </summary>
        public CommandBase _ReadFilePathCmd;
        public CommandBase ReadFilePathCmd
        {
            get
            {
                if (_ReadFilePathCmd == null)
                {
                    _ReadFilePathCmd = new CommandBase();
                    _ReadFilePathCmd.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        
                        
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _ReadFilePathCmd;
            }
        }

        /// <summary>
        /// 读取Json文件
        /// </summary>
        public CommandBase _ReadJsonCmd;
        public CommandBase ReadJsonCmd
        {
            get
            {
                if (_ReadJsonCmd == null)
                {
                    _ReadJsonCmd = new CommandBase();
                    _ReadJsonCmd.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        

                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _ReadJsonCmd;
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
                    _CreateNewTalkCmd.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        CreateThumbsBase createThumbsBase = new CreateThumbsBase(mainWindow);

                        var back = await createThumbsBase.CreateThumb(ThumbClass.Subject, MainFilePath, obj as MainWindow);

                        if (!back.Succese)
                        {
                            ShowMessage(back.Text);

                            return;
                        }

                        if (back.Backs == null)
                        {
                            ShowMessage("错误！返回值为空");

                            return;
                        }

                        var thumb = back.Backs as Thumb;

                        thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                        thumb.DragCompleted += Thumb_DragCompleted;
                        thumb.DragDelta += Thumb_DragDelta;
                        thumb.DragStarted += Thumb_DragStarted;

                        ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                        if (mainWindow == null)
                        {
                            mainWindow = obj as MainWindow;
                        }
                        ActBase.KeyDown -= ActBase_KeyDown;
                        ActBase.KeyDown += ActBase_KeyDown;
                        isHaveSubjcet = true;

                        
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreateNewTalkCmd;
            }
        }


        private int playerNum = 0;
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
                    _CreatePlayerTalkCmd.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        CreateThumbsBase createThumbsBase = new CreateThumbsBase(mainWindow);

                        var back = await createThumbsBase.CreateThumb(ThumbClass.Player, MainFilePath, obj as MainWindow);

                        if (!back.Succese)
                        {
                            ShowMessage(back.Text);

                            return;
                        }

                        if (back.Backs == null)
                        {
                            ShowMessage("错误！返回值为空");

                            return;
                        }

                        var thumb = back.Backs as Thumb;

                        thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                        thumb.DragCompleted += Thumb_DragCompleted;
                        thumb.DragDelta += Thumb_DragDelta;
                        thumb.DragStarted += Thumb_DragStarted;

                        ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                        if (mainWindow == null)
                        {
                            mainWindow = obj as MainWindow;
                        }

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

                        if (playerLoader.saveThumbInfoWindowModel == null)
                        {
                            playerLoader.saveThumbInfoWindowModel = new Dictionary<Thumb, ThumbInfoWindowModel>()
                                    {
                                        {thumb,infoModel},
                                    };
                        }
                        else
                        {
                            playerLoader.saveThumbInfoWindowModel.Add(thumb, infoModel);
                        }

                        await Task.Run(() =>
                        {
                            while ((GetControl("ConditionsConfig_TBox", thumb) as TextBox) == null)
                            {
                                Thread.Sleep(100);
                            }
                            mainWindow.cvmenu.Dispatcher.Invoke(new Action(() =>
                            {
                                (GetControl("ConditionsConfig_TBox", thumb) as TextBox).Text = "PlayerTemp_" + playerNum;

                            }));
                            playerNum++;
                        });

                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreatePlayerTalkCmd;
            }
        }

        private int npcNum = 0;
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
                    _CreateNpcTalkCmd.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        CreateThumbsBase createThumbsBase = new CreateThumbsBase(mainWindow);

                        var back = await createThumbsBase.CreateThumb(ThumbClass.NPC, MainFilePath, obj as MainWindow);

                        if (!back.Succese)
                        {
                            ShowMessage(back.Text);

                            return;
                        }

                        if (back.Backs == null)
                        {
                            ShowMessage("错误！返回值为空");

                            return;
                        }

                        var thumb = back.Backs as Thumb;

                        thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                        thumb.DragCompleted += Thumb_DragCompleted;
                        thumb.DragDelta += Thumb_DragDelta;
                        thumb.DragStarted += Thumb_DragStarted;

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

                        if (npcLoader.saveThumbInfoWindowModel == null)
                        {
                            npcLoader.saveThumbInfoWindowModel = new Dictionary<Thumb, ThumbInfoWindowModel>()
                            {
                                 {thumb,infoModel},
                            };
                        }
                        else
                        {
                            npcLoader.saveThumbInfoWindowModel.Add(thumb, infoModel);
                        }

                        await Task.Run(() =>
                        {
                            while ((GetControl("ConditionsConfig_TBox", thumb) as TextBox) == null)
                            {
                                Thread.Sleep(100);
                            }
                            mainWindow.cvmenu.Dispatcher.Invoke(new Action(() =>
                            {
                                (GetControl("ConditionsConfig_TBox", thumb) as TextBox).Text = "NpcTemp_" + npcNum;

                            }));
                            npcNum++;
                        });
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreateNpcTalkCmd;
            }
        }

        private int conditionsNum = 0;
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
                    _CreateConditionsCmd.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        CreateThumbsBase createThumbsBase = new CreateThumbsBase(mainWindow);

                        var back = await createThumbsBase.CreateThumb(ThumbClass.Conditions, MainFilePath, obj as MainWindow);

                        if (!back.Succese)
                        {
                            ShowMessage(back.Text);

                            return;
                        }

                        if (back.Backs == null)
                        {
                            ShowMessage("错误！返回值为空");

                            return;
                        }

                        var thumb = back.Backs as Thumb;

                        thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                        thumb.DragCompleted += Thumb_DragCompleted;
                        thumb.DragDelta += Thumb_DragDelta;
                        thumb.DragStarted += Thumb_DragStarted;

                        ThumbNums = MainWindowModels.saveThumbs.Count.ToString();
                        await Task.Run(() =>
                        {
                            while ((GetControl("ConditionsConfig_TBox", thumb) as TextBox) == null)
                            {
                                Thread.Sleep(100);
                            }
                            mainWindow.cvmenu.Dispatcher.Invoke(new Action(() =>
                            {
                                (GetControl("ConditionsConfig_TBox", thumb) as TextBox).Text = "ConditionTemp_" + conditionsNum;

                            }));
                            conditionsNum++;
                        });
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreateConditionsCmd;
            }
        }

        private int eventsNum = 0;
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
                    _CreateEventsCmd.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        CreateThumbsBase createThumbsBase = new CreateThumbsBase(mainWindow);

                        var back = await createThumbsBase.CreateThumb(ThumbClass.Events, MainFilePath, obj as MainWindow);

                        if (!back.Succese)
                        {
                            ShowMessage(back.Text);

                            return;
                        }

                        if (back.Backs == null)
                        {
                            ShowMessage("错误！返回值为空");

                            return;
                        }

                        var thumb = back.Backs as Thumb;

                        thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                        thumb.DragCompleted += Thumb_DragCompleted;
                        thumb.DragDelta += Thumb_DragDelta;
                        thumb.DragStarted += Thumb_DragStarted;

                        ThumbNums = MainWindowModels.saveThumbs.Count.ToString();
                        await Task.Run(() =>
                        {
                            while ((GetControl("ConditionsConfig_TBox", thumb) as TextBox) == null)
                            {
                                Thread.Sleep(100);
                            }
                            mainWindow.cvmenu.Dispatcher.Invoke(new Action(() =>
                            {
                                (GetControl("ConditionsConfig_TBox", thumb) as TextBox).Text = "EventTemp_" + eventsNum;
                            }));
                            eventsNum++;
                        });
                        
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreateEventsCmd;
            }
        }

        private int objectivesNum = 0;
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
                    _CreateObjectivesCmd.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        CreateThumbsBase createThumbsBase = new CreateThumbsBase(mainWindow);

                        var back = await createThumbsBase.CreateThumb(ThumbClass.Objectives, MainFilePath, obj as MainWindow);

                        if (!back.Succese)
                        {
                            ShowMessage(back.Text);

                            return;
                        }

                        if (back.Backs == null)
                        {
                            ShowMessage("错误！返回值为空");

                            return;
                        }

                        var thumb = back.Backs as Thumb;

                        thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                        thumb.DragCompleted += Thumb_DragCompleted;
                        thumb.DragDelta += Thumb_DragDelta;
                        thumb.DragStarted += Thumb_DragStarted;

                        ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                        await Task.Run(() =>
                        {
                            while ((GetControl("ConditionsConfig_TBox", thumb) as TextBox) == null)
                            {
                                Thread.Sleep(100);
                            }
                            mainWindow.cvmenu.Dispatcher.Invoke(new Action(() =>
                            {
                                (GetControl("ConditionsConfig_TBox", thumb) as TextBox).Text = "ObjectiveTemp_" + objectivesNum;
                            }));
                            objectivesNum++;
                        });

                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreateObjectivesCmd;
            }
        }

        private int journalsNum = 0;
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
                    _CreateJournalCmd.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        CreateThumbsBase createThumbsBase = new CreateThumbsBase(mainWindow);

                        var back = await createThumbsBase.CreateThumb(ThumbClass.Journal, MainFilePath, obj as MainWindow);

                        if (!back.Succese)
                        {
                            ShowMessage(back.Text);

                            return;
                        }

                        if (back.Backs == null)
                        {
                            ShowMessage("错误！返回值为空");

                            return;
                        }

                        var thumb = back.Backs as Thumb;

                        thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                        thumb.DragCompleted += Thumb_DragCompleted;
                        thumb.DragDelta += Thumb_DragDelta;
                        thumb.DragStarted += Thumb_DragStarted;

                        ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                        await Task.Run(() =>
                        {
                            while ((GetControl("JournalConfig_TBox", thumb) as TextBox) == null)
                            {
                                Thread.Sleep(100);
                            }
                            mainWindow.cvmenu.Dispatcher.Invoke(new Action(() =>
                            {
                                (GetControl("JournalConfig_TBox", thumb) as TextBox).Text = "JournalTemp_" + journalsNum;
                            }));
                            journalsNum++;
                        });
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreateJournalCmd;
            }
        }

        private int itemsNum = 0;
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
                    _CreateItemsCmd.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        CreateThumbsBase createThumbsBase = new CreateThumbsBase(mainWindow);

                        var back = await createThumbsBase.CreateThumb(ThumbClass.Items, MainFilePath, obj as MainWindow);

                        if (!back.Succese)
                        {
                            ShowMessage(back.Text);

                            return;
                        }

                        if (back.Backs == null)
                        {
                            ShowMessage("错误！返回值为空");

                            return;
                        }

                        var thumb = back.Backs as Thumb;

                        thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                        thumb.DragCompleted += Thumb_DragCompleted;
                        thumb.DragDelta += Thumb_DragDelta;
                        thumb.DragStarted += Thumb_DragStarted;

                        ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                        await Task.Run(() =>
                        {
                            while ((GetControl("ItemsConfig_TBox", thumb) as TextBox) == null)
                            {
                                Thread.Sleep(100);
                            }
                            mainWindow.cvmenu.Dispatcher.Invoke(new Action(() =>
                            {
                                (GetControl("ItemsConfig_TBox", thumb) as TextBox).Text = "ItemsTemp_" + itemsNum;
                            }));
                            itemsNum++;
                        });
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

                            var one = string.Empty;

                            var haveInfoCmd = false;

                            if (!mainWindowModels.SaveThumbInfo[nowThumb].ContainsKey((getConditions_CBox.SelectedItem as ComboBoxItem).Content.ToString()))
                            {
                                var fg_one = (getConditions_CBox.SelectedItem as ComboBoxItem).Content.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                                if (fg_one.Length == 3)
                                {
                                    haveInfoCmd = mainWindowModels.SaveThumbInfo[nowThumb].ContainsKey(fg_one[1] + ": " + fg_one[2]);

                                    one = fg_one[1] + ": " + fg_one[2];
                                }
                                else
                                {
                                    haveInfoCmd = false;
                                    one = (getConditions_CBox.SelectedItem as ComboBoxItem).Content.ToString();
                                }
                            }
                            else
                            {
                                haveInfoCmd = true;
                                one = (getConditions_CBox.SelectedItem as ComboBoxItem).Content.ToString();
                            }

                            if (!haveInfoCmd)
                            {
                                if (mainWindowModels.SaveThumbInfo[nowThumb].Count > 0&& getInfo.thumbClass != ThumbClass.Player && getInfo.thumbClass != ThumbClass.NPC)
                                {
                                    mainWindowModels.SaveThumbInfo[nowThumb].Clear();
                                }

                                mainWindowModels.SaveThumbInfo[nowThumb].Add(one, new Dictionary<string, Dictionary<string, Dictionary<string, string>>>());
                            }

                            var getConditionsCmdEdit_CBox = GetControl("ConditionsCmdEdit_CBox", nowThumb) as ComboBox;

                            if (getConditionsCmdEdit_CBox.SelectedItem == null)
                            {
                                return;
                            }

                            var two = string.Empty;

                            var haveInfoCmdEdit = false;

                            var getConditionsCmdEdit_CBoxItem = getConditionsCmdEdit_CBox.SelectedItem as ComboBoxItem;
                            var getConditionsCmdEdit_CBoxItemStr = string.Empty;
                            if (getConditionsCmdEdit_CBoxItem != null)
                            {
                                getConditionsCmdEdit_CBoxItemStr = getConditionsCmdEdit_CBoxItem.Content.ToString();
                            }
                            else
                            {
                                getConditionsCmdEdit_CBoxItemStr = getConditionsCmdEdit_CBox.SelectedItem.ToString();
                            }

                            if (!mainWindowModels.SaveThumbInfo[nowThumb]
                            [one].ContainsKey(getConditionsCmdEdit_CBoxItemStr))
                            {
                                var fg_one = getConditionsCmdEdit_CBoxItemStr.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                                if (fg_one.Length == 3)
                                {
                                    haveInfoCmdEdit = mainWindowModels.SaveThumbInfo[nowThumb][one].ContainsKey(fg_one[1] + ": " + fg_one[2]);

                                    two = fg_one[1] + ": " + fg_one[2];
                                }
                                else
                                {
                                    haveInfoCmdEdit = false;
                                    two = getConditionsCmdEdit_CBoxItemStr;
                                }
                            }
                            else
                            {
                                haveInfoCmdEdit = true;
                                two = getConditionsCmdEdit_CBoxItemStr;
                            }

                            if (!haveInfoCmdEdit)
                            {
                                mainWindowModels.SaveThumbInfo[nowThumb]
                                [one].Add(two, new Dictionary<string, Dictionary<string, string>>());
                            }

                            var getConditionsCmdparameterEdit_CBox = GetControl("ConditionsCmdparameterEdit_CBox", nowThumb) as ComboBox;

                            if (getConditionsCmdparameterEdit_CBox.SelectedItem == null)
                            {
                                return;
                            }

                            var three = string.Empty;
                            
                            var haveInfoCmdparameterEdit = false;

                            var getConditionsCmdparameterEdit_CBoxItem = getConditionsCmdparameterEdit_CBox.SelectedItem as ComboBoxItem;

                            var getConditionsCmdparameterEdit_CBoxItemStr = string.Empty;

                            if (getConditionsCmdparameterEdit_CBoxItem != null)
                            {
                                getConditionsCmdparameterEdit_CBoxItemStr = getConditionsCmdparameterEdit_CBoxItem.Content.ToString();
                            }
                            else
                            {
                                getConditionsCmdparameterEdit_CBoxItemStr = getConditionsCmdparameterEdit_CBox.SelectedItem.ToString();
                            }

                            if (!mainWindowModels.SaveThumbInfo[nowThumb][one][two].ContainsKey(getConditionsCmdparameterEdit_CBoxItemStr))
                            {
                                var fg_one = getConditionsCmdparameterEdit_CBoxItemStr.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                                if (fg_one.Length == 3)
                                {
                                    haveInfoCmdparameterEdit = mainWindowModels.SaveThumbInfo[nowThumb][one][two].ContainsKey(fg_one[1] + ": " + fg_one[2]);

                                    three = fg_one[1] + ": " + fg_one[2];
                                }
                                else
                                {
                                    haveInfoCmdparameterEdit = false;
                                    three = getConditionsCmdparameterEdit_CBoxItemStr;
                                }
                            }
                            else
                            {
                                haveInfoCmdparameterEdit = true;
                                three = getConditionsCmdparameterEdit_CBoxItemStr;
                            }

                            if (!haveInfoCmdparameterEdit)
                            {
                                mainWindowModels.SaveThumbInfo[nowThumb]
                                [one]
                                [two].Add(three, new Dictionary<string, string>());
                            }

                            var getConditionsCmdProjectEdit_CBox = GetControl("ConditionsCmdProjectEdit_CBox", nowThumb) as ComboBox;

                            if (getConditionsCmdProjectEdit_CBox.SelectedItem == null)
                            {
                                return;
                            }

                            var four = string.Empty;

                            var haveInfoCmdProjectEdit = false;

                            var getConditionsCmdProjectEdit_CBoxItem = getConditionsCmdProjectEdit_CBox.SelectedItem as ComboBoxItem;

                            var getConditionsCmdProjectEdit_CBoxItemStr = string.Empty;

                            if (getConditionsCmdProjectEdit_CBoxItem != null)
                            {
                                getConditionsCmdProjectEdit_CBoxItemStr = getConditionsCmdProjectEdit_CBoxItem.Content.ToString();
                            }
                            else
                            {
                                getConditionsCmdProjectEdit_CBoxItemStr = getConditionsCmdProjectEdit_CBox.SelectedItem.ToString();
                            }

                            if (!mainWindowModels.SaveThumbInfo[nowThumb]
                            [one][two][three].ContainsKey(getConditionsCmdProjectEdit_CBoxItemStr))
                            {
                                var fg_one = getConditionsCmdProjectEdit_CBoxItemStr.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                                if (fg_one.Length == 3)
                                {
                                    haveInfoCmdProjectEdit = mainWindowModels.SaveThumbInfo[nowThumb][one][two][three].ContainsKey(fg_one[1] + ": " + fg_one[2]);

                                    four = fg_one[1] + ": " + fg_one[2];
                                }
                                else
                                {
                                    haveInfoCmdProjectEdit = false;
                                    four = getConditionsCmdProjectEdit_CBoxItemStr;
                                }
                            }
                            else
                            {
                                haveInfoCmdProjectEdit = true;
                                four = getConditionsCmdProjectEdit_CBoxItemStr;
                            }

                            if (!haveInfoCmdProjectEdit)
                            {
                                mainWindowModels.SaveThumbInfo[nowThumb]
                                [one]
                                [two]
                                [three].Add(four, null);
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
                                    try
                                    {
                                        vlue = (getConditions_ComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                                    }
                                    catch
                                    {
                                        vlue = getConditions_ComboBox.SelectedItem.ToString();
                                    }
                                    
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
                            [one]
                            [two]
                            [three]
                            [four] = vlue;

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

        public CommandBase _CloseCommand;
        public CommandBase CloseCommand
        {
            get
            {
                if (_CloseCommand == null)
                {
                    _CloseCommand = new CommandBase();
                    _CloseCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        Environment.Exit(0);
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CloseCommand;
            }
        }

        public CommandBase _NarrowCommand;
        public CommandBase NarrowCommand
        {
            get
            {
                if (_NarrowCommand == null)
                {
                    _NarrowCommand = new CommandBase();
                    _NarrowCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        (obj as Window).WindowState = WindowState.Minimized;
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _NarrowCommand;
            }
        }

        public CommandBase _MinimizeCommand;
        public CommandBase MinimizeCommand
        {
            get
            {
                if (_MinimizeCommand == null)
                {
                    _MinimizeCommand = new CommandBase();
                    _MinimizeCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        var window = obj as Window;
                        if (window.WindowState == WindowState.Maximized)
                        {
                            window.WindowState = WindowState.Normal;
                        }
                        else if(window.WindowState == WindowState.Normal)
                        {
                            window.WindowState = WindowState.Maximized;
                        }
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _MinimizeCommand;
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

                        mainWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        playerLoader = new PlayerLoaderBase(mainWindowModels);
                        npcLoader = new NpcLoaderBase(mainWindowModels);
                        ThumbInfoWindow window = new ThumbInfoWindow();

                        thumbInfoWindow = window;

                        window.WindowStartupLocation = WindowStartupLocation.Manual;

                        window.Left = mainWindow.ActualWidth + mainWindow.Left;

                        window.Top = mainWindow.Top;

                        window.Show();

                        window.DataContext = new ThumbInfoWindowViewModel();

                        SearchListType = new List<ThumbClass>
                        {
                            ThumbClass.Subject,
                            ThumbClass.NPC,
                            ThumbClass.Player,
                            ThumbClass.Conditions,
                            ThumbClass.Events,
                            ThumbClass.Items,
                            ThumbClass.Journal,
                            ThumbClass.Objectives,
                        };

                        mainWindow.IsEnabled = false;

                        LoadingShow = Visibility.Visible;

                        LoadingMessage = "正在加载条件模型~";

                        contisionLoader = new ContisionLoaderBase(mainWindow,mainWindowModels);

                        var conJson = await contisionLoader.Saver();
                        
                        contisionLoader.jsons = conJson;

                        if (string.IsNullOrEmpty(conJson))
                        {
                            return;
                        }

                        contisionProp = await contisionLoader.Loader();

                        LoadingMessage = "正在加载事件模型~";

                        eventLoader = new EventLoaderBase(mainWindow, mainWindowModels);

                        var eventJson = await eventLoader.Saver();

                        eventLoader.jsons = eventJson;

                        if (string.IsNullOrEmpty(eventJson))
                        {
                            return;
                        }

                        eventProp = await eventLoader.Loader();

                        LoadingMessage = "正在加载目标模型~";

                        objectiveLoader = new ObjectiveLoaderBase(mainWindow, mainWindowModels);

                        var objJson = await objectiveLoader.Saver();

                        objectiveLoader.jsons = objJson;

                        if (string.IsNullOrEmpty(objJson))
                        {
                            return;
                        }

                        objectiveProp = await objectiveLoader.Loader();

                        LoadingMessage = "加载完毕，ArcCreate欢迎您~";

                        await Task.Run(() =>
                        {
                            Thread.Sleep(3000);
                        });
                        var datacont = new SelectDataWindowViewModel();
                        while (true) 
                        {
                            SelectDataWindow dataWindow = new SelectDataWindow();
                            dataWindow.DataContext = datacont;
                            dataWindow.ShowDialog();
                            SelectData = dataWindow.Tag as GridData;

                            if (SelectData == null)
                            {
                                MessageBox.Show("导入错误，请重试");
                                continue;
                            }
                            MainFilePath = SelectData.FilePath;
                            if (SelectData.Code != -1)
                            {
                                LoadingMessage = "正在从云端拉取数据，请不要操作页面";
                                var back = await ReadJson();

                                if (!back.Succese)
                                {
                                    MessageBox.Show("导入错误，请重试" + back.Text);
                                    continue;
                                }
                                else
                                {
                                    PageName = SelectData.Name;
                                    break;
                                }
                                try
                                {
                                    
                                }
                                catch
                                {
                                    MessageBox.Show("导入错误，请重试","错误");
                                    continue;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(MainFilePath))
                                {
                                    LoadingMessage = "正在向云端请求解析，请不要操作页面";

                                    try
                                    {
                                        var back = await ReadYaml();

                                        if (!back.Succese)
                                        {
                                            MessageBox.Show("导入错误，请重试");
                                            continue;
                                        }
                                        else
                                        {
                                            PageName = SelectData.Name;
                                            break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("解析错误,配置文件中请不要使用多语言结构，\n如果您在更改后还出现此错误请联系\njk@jklss.cn");
                                        continue;
                                    }
                                }
                                else
                                {
                                    PageName = SelectData.Name;
                                    break;
                                }
                            }
                        }

                        mainWindow.IsEnabled = true;
                        LoadingShow = Visibility.Hidden;
                        
                        mainWindow.cvmenu.MouseLeftButtonDown += Cvmenu_MouseLeftButtonDown;
                        mainWindow.cvmenu.MouseLeftButtonUp += Cvmenu_MouseLeftButtonUp;
                        
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
                        thumbInfoWindow.Left = mainWindow.ActualWidth + mainWindow.Left;
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

        private async Task<ReturnModel> ReadYaml()
        {
            var result = new ReturnModel();

            if (string.IsNullOrEmpty(MainFilePath))
            {
                ShowMessage("请选则正确的文件");

                result.SetError();
                return result;
            }
            if (!FileService.IsHaveFile(MainFilePath))
            {
                ShowMessage("main入口文件不存在请重新指定Main.yml文件");
                result.SetError();
                return result;
            }

            var cs = new SaveAndReadYamlBase(MainFilePath, objectiveProp, eventProp, contisionProp, saveThumbs, mainWindowModels.SaveThumbInfo);

            var disPath = FileService.GetFileDirectory(MainFilePath);

            var getConditions = new List<string>(FileService.GetFileText(disPath + @"\conditions.yml")
                .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));//决定生成多少个条件

            var getEvents = new List<string>(FileService.GetFileText(disPath + @"\events.yml")
                .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));//决定生成多少个事件

            var getItems = new List<string>(FileService.GetFileText(disPath + @"\items.yml")
                .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));//决定生成多少个物品

            var getJournaltoProp = FileService.YamlToProp<Dictionary<string, string>>(disPath + @"\journal.yml");//决定生成多少个日记

            var getJournal = new Dictionary<string, string>();

            if (getJournaltoProp != null)
            {
                getJournal = getJournaltoProp;
            }

            var getObjectives = new List<string>(FileService.GetFileText(disPath + @"\objectives.yml")
                .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));//决定生成多少个目标

            var getMain = FileService.YamlToProp<MainConfigModel>(MainFilePath);

            var allConversationsFilePath = FileService.GetDisAllFile(disPath + @"\conversations");

            var allConversations = new List<ConversationsModel>();//决定生成多少个对话主体 玩家与Npc对话

            for (int i = 0; i < allConversationsFilePath.Count; i++)
            {
                allConversations.Add(FileService.YamlToProp<ConversationsModel>(allConversationsFilePath[i]));
            }

            var createThumbs = new CreateThumbsBase(mainWindow);

            var npcnum = 0;

            var playernum = 0;

            var treeViewBase = new TreeViewBase();

            #region 控件的生成与数据的绑定

            for (int i = 0; i < allConversations.Count; i++)
            {
                var back = await createThumbs.CreateThumb(ThumbClass.Subject, MainFilePath, mainWindow, py: i * 500);

                if (!back.Succese)
                {
                    ShowMessage(back.Text);

                    result.SetError();
                    return result;
                }

                if (back.Backs == null)
                {
                    ShowMessage("错误！返回值为空");

                    result.SetError();
                    return result;
                }

                var thumb = back.Backs as Thumb;

                thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                thumb.DragCompleted += Thumb_DragCompleted;
                thumb.DragDelta += Thumb_DragDelta;
                thumb.DragStarted += Thumb_DragStarted;

                ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                ActBase.KeyDown -= ActBase_KeyDown;
                ActBase.KeyDown += ActBase_KeyDown;

                isHaveSubjcet = true;

                await Task.Run(() =>
                {
                    Thread.Sleep(100);
                    mainWindow.cvmenu.Dispatcher.Invoke(new Action(() =>
                    {
                        try
                        {
                            (GetControl("NpcName_TBox", thumb) as TextBox).Text = getMain.npcs.Where(t => t.Value == FileService.GetFilePathToFileName(allConversationsFilePath[i])).First().Key;
                            (GetControl("ShowNpcName_TBox", thumb) as TextBox).Text = allConversations[i].quester;
                            (GetControl("MainName_TBox", thumb) as TextBox).Text = FileService.GetFilePathToFileName(allConversationsFilePath[i]);
                        }
                        catch
                        {

                        }

                    }));
                });

                foreach (var item in allConversations[i].NPC_options)
                {
                    var npcback = await createThumbs.CreateThumb(ThumbClass.NPC, MainFilePath, mainWindow, npcnum * 500, 200);

                    if (!npcback.Succese)
                    {
                        ShowMessage(npcback.Text);

                        result.SetError();
                        return result;
                    }

                    if (npcback.Backs == null)
                    {
                        ShowMessage("错误！返回值为空");

                        result.SetError();
                        return result;
                    }

                    var thumbs = npcback.Backs as Thumb;

                    thumbs.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                    thumbs.DragCompleted += Thumb_DragCompleted;
                    thumbs.DragDelta += Thumb_DragDelta;
                    thumbs.DragStarted += Thumb_DragStarted;

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

                    if (npcLoader.saveThumbInfoWindowModel == null)
                    {
                        npcLoader.saveThumbInfoWindowModel = new Dictionary<Thumb, ThumbInfoWindowModel>()
                                    {
                                        {thumbs,infoModel},
                                    };
                    }
                    else
                    {
                        npcLoader.saveThumbInfoWindowModel.Add(thumbs, infoModel);
                    }


                    await Task.Run(() =>
                    {
                        Thread.Sleep(100);
                        mainWindow.cvmenu.Dispatcher.Invoke(new Action(() =>
                        {

                            (GetControl("ConditionsConfig_TBox", thumbs) as TextBox).Text = item.Key;

                        }));
                        npcNum++;
                    });

                    #region 数据录入

                    if (!mainWindowModels.SaveThumbInfo.ContainsKey(thumbs))
                    {
                        mainWindowModels.SaveThumbInfo.Add(thumbs, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>()
                                    {
                                        {"文案: text",new Dictionary<string, Dictionary<string, Dictionary<string, string>>>(){
                                            { "text", new Dictionary<string, Dictionary<string, string>>(){
                                                {"第 1 条参数",new Dictionary<string, string>() }
                                            } } } },
                                        {"触发条件: conditions",new Dictionary<string, Dictionary<string, Dictionary<string, string>>>(){
                                            { "conditions", new Dictionary<string, Dictionary<string, string>>(){
                                                {"第 1 条参数",new Dictionary<string, string>() }
                                            } } } },
                                        {"触发事件: events",new Dictionary<string, Dictionary<string, Dictionary<string, string>>>(){
                                            { "events", new Dictionary<string, Dictionary<string, string>>(){
                                                {"第 1 条参数",new Dictionary<string, string>() }
                                            } } } },
                                        {"存储对话: pointer",new Dictionary<string, Dictionary<string, Dictionary<string, string>>>(){
                                            { "pointer", new Dictionary<string, Dictionary<string, string>>(){
                                                {"第 1 条参数",new Dictionary<string, string>() }
                                            } } } },
                                    });
                    }

                    var getText = item.Value.text;

                    for (int j = 0; j < getText.Count; j++)
                    {
                        await treeViewBase.AddItemToSaves(thumbs, "text", "第 1 条参数", $"第 {j + 1} 项", "文案: text",
                            getText[j], true, npcLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);
                    }

                    if (!string.IsNullOrEmpty(item.Value.conditions))
                    {
                        var getConditionsProp = await cs.PlayerAndNpcAnalysis(item.Value.conditions);

                        if (!getConditionsProp.Succese)
                        {
                            result.SetError();
                            return result;
                        }

                        var conditionsProp = getConditionsProp.Backs as Dictionary<string, Dictionary<string, string>>;

                        foreach (var j in conditionsProp["第 1 条参数"])
                        {
                            await treeViewBase.AddItemToSaves(thumbs, "conditions", "第 1 条参数", j.Key, "触发条件: conditions",
                                j.Value, true, npcLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Value.events))
                    {
                        var getEventProp = await cs.PlayerAndNpcAnalysis(item.Value.events);

                        if (!getEventProp.Succese)
                        {
                            result.SetError();
                            return result;
                        }

                        var EventProp = getEventProp.Backs as Dictionary<string, Dictionary<string, string>>;

                        foreach (var j in EventProp["第 1 条参数"])
                        {
                            await treeViewBase.AddItemToSaves(thumbs, "events", "第 1 条参数", j.Key, "触发事件: events",
                                j.Value, true, npcLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Value.pointer))
                    {
                        var getPointerProp = await cs.PlayerAndNpcAnalysis(item.Value.pointer);

                        if (!getPointerProp.Succese)
                        {
                            result.SetError();
                            return result;
                        }

                        var PointerProp = getPointerProp.Backs as Dictionary<string, Dictionary<string, string>>;

                        foreach (var j in PointerProp["第 1 条参数"])
                        {
                            await treeViewBase.AddItemToSaves(thumbs, "pointer", "第 1 条参数", j.Key, "存储对话: pointer",
                                j.Value, true, npcLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);
                        }
                    }
                    #endregion

                    npcnum++;
                }

                foreach (var item in allConversations[i].player_options)
                {
                    var playerback = await createThumbs.CreateThumb(ThumbClass.Player, MainFilePath, mainWindow, playernum * 500, 400);

                    if (!playerback.Succese)
                    {
                        ShowMessage(back.Text);

                        result.SetError();
                        return result;
                    }

                    if (playerback.Backs == null)
                    {
                        ShowMessage("错误！返回值为空");

                        result.SetError();
                        return result;
                    }

                    var thumbs = playerback.Backs as Thumb;

                    thumbs.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                    thumbs.DragCompleted += Thumb_DragCompleted;
                    thumbs.DragDelta += Thumb_DragDelta;
                    thumbs.DragStarted += Thumb_DragStarted;

                    ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

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

                    if (playerLoader.saveThumbInfoWindowModel == null)
                    {
                        playerLoader.saveThumbInfoWindowModel = new Dictionary<Thumb, ThumbInfoWindowModel>()
                                    {
                                        {thumbs,infoModel},
                                    };
                    }
                    else
                    {
                        playerLoader.saveThumbInfoWindowModel.Add(thumbs, infoModel);
                    }

                    await Task.Run(() =>
                    {
                        Thread.Sleep(100);
                        mainWindow.cvmenu.Dispatcher.Invoke(new Action(() =>
                        {

                            (GetControl("ConditionsConfig_TBox", thumbs) as TextBox).Text = item.Key;

                        }));
                        playerNum++;
                    });

                    #region 数据录入

                    if (!mainWindowModels.SaveThumbInfo.ContainsKey(thumbs))
                    {
                        mainWindowModels.SaveThumbInfo.Add(thumbs, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>()
                                    {
                                        {"文案: text",new Dictionary<string, Dictionary<string, Dictionary<string, string>>>(){
                                            { "text", new Dictionary<string, Dictionary<string, string>>(){
                                                {"第 1 条参数",new Dictionary<string, string>() }
                                            } } } },
                                        {"触发条件: conditions",new Dictionary<string, Dictionary<string, Dictionary<string, string>>>(){
                                            { "conditions", new Dictionary<string, Dictionary<string, string>>(){
                                                {"第 1 条参数",new Dictionary<string, string>() }
                                            } } } },
                                        {"触发事件: events",new Dictionary<string, Dictionary<string, Dictionary<string, string>>>(){
                                            { "events", new Dictionary<string, Dictionary<string, string>>(){
                                                {"第 1 条参数",new Dictionary<string, string>() }
                                            } } } },
                                        {"存储对话: pointer",new Dictionary<string, Dictionary<string, Dictionary<string, string>>>(){
                                            { "pointer", new Dictionary<string, Dictionary<string, string>>(){
                                                {"第 1 条参数",new Dictionary<string, string>() }
                                            } } } },
                                    });
                    }

                    var getText = item.Value.text;

                    for (int j = 0; j < getText.Count; j++)
                    {
                        await treeViewBase.AddItemToSaves(thumbs, "text", "第 1 条参数", $"第 {j + 1} 项", "文案: text",
                            getText[j], true, playerLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);
                    }

                    if (!string.IsNullOrEmpty(item.Value.conditions))
                    {
                        var getConditionsProp = await cs.PlayerAndNpcAnalysis(item.Value.conditions);

                        if (!getConditionsProp.Succese)
                        {
                            result.SetError();
                            return result;
                        }

                        var conditionsProp = getConditionsProp.Backs as Dictionary<string, Dictionary<string, string>>;

                        foreach (var j in conditionsProp["第 1 条参数"])
                        {
                            await treeViewBase.AddItemToSaves(thumbs, "conditions", "第 1 条参数", j.Key, "触发条件: conditions",
                                j.Value, true, playerLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Value.events))
                    {
                        var getEventProp = await cs.PlayerAndNpcAnalysis(item.Value.events);

                        if (!getEventProp.Succese)
                        {
                            result.SetError();
                            return result;
                        }

                        var EventProp = getEventProp.Backs as Dictionary<string, Dictionary<string, string>>;

                        foreach (var j in EventProp["第 1 条参数"])
                        {
                            await treeViewBase.AddItemToSaves(thumbs, "events", "第 1 条参数", j.Key, "触发事件: events",
                                j.Value, true, playerLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Value.pointer))
                    {
                        var getPointerProp = await cs.PlayerAndNpcAnalysis(item.Value.pointer);

                        if (!getPointerProp.Succese)
                        {
                            result.SetError();
                            return result;
                        }

                        var PointerProp = getPointerProp.Backs as Dictionary<string, Dictionary<string, string>>;

                        foreach (var j in PointerProp["第 1 条参数"])
                        {
                            await treeViewBase.AddItemToSaves(thumbs, "pointer", "第 1 条参数", j.Key, "存储对话: pointer",
                                j.Value, true, playerLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);
                        }
                    }
                    #endregion

                    playernum++;
                }
            }//生成对话主体及Npc与Player对话

            for (int i = 0; i < getConditions.Count; i++)
            {
                var back = await createThumbs.CreateThumb(ThumbClass.Conditions, MainFilePath, mainWindow, i * 500, 600);

                if (!back.Succese)
                {
                    ShowMessage(back.Text);

                    result.SetError();
                    return result;
                }

                if (back.Backs == null)
                {
                    ShowMessage("错误！返回值为空");

                    result.SetError();
                    return result;
                }

                var thumb = back.Backs as Thumb;

                thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                thumb.DragCompleted += Thumb_DragCompleted;
                thumb.DragDelta += Thumb_DragDelta;
                thumb.DragStarted += Thumb_DragStarted;

                ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                #region 数据处理

                if (!mainWindowModels.SaveThumbInfo.ContainsKey(thumb))
                {
                    mainWindowModels.SaveThumbInfo.Add(thumb, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>());
                }

                var fg = getConditions[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                var backs = await cs.ConditionAnalysis(fg[1].Trim('\''));

                if (!backs.Succese)
                {
                    result.SetError();
                    return result;
                }

                var getConditionsProp = backs.Backs as Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>;


                #endregion

                await Task.Run(() =>
                {
                    Thread.Sleep(100);
                    var num = i;
                    mainWindow.cvmenu.Dispatcher.Invoke(new Action(async () =>
                    {
                        var cmd = string.Empty;
                        while (checkde)
                        {
                            await Task.Run(() => { Thread.Sleep(100); });
                        }
                        nowThumb = thumb;
                        while ((GetControl("ConditionsConfig_TBox", thumb) as TextBox) == null)
                        {
                            await Task.Run(() => { Thread.Sleep(100); });
                        }

                        (GetControl("ConditionsConfig_TBox", thumb) as TextBox).Text = getConditions[num].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries)[0];
                        foreach (var item in (GetControl("Conditions_CBox", thumb) as ComboBox).Items)
                        {
                            var fgf = item.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                            if (fgf[2] == getConditionsProp.Keys.First())
                            {
                                (GetControl("Conditions_CBox", thumb) as ComboBox).SelectedItem = item;
                                cmd = item.ToString();
                                mainWindowModels.SaveThumbInfo[thumb].Add(cmd, getConditionsProp[getConditionsProp.Keys.First()]);
                                break;
                            }
                        }
                    }));

                    conditionsNum++;
                });
            }

            for (int i = 0; i < getEvents.Count; i++)
            {

                var back = await createThumbs.CreateThumb(ThumbClass.Events, MainFilePath, mainWindow, i * 500, 800);

                if (!back.Succese)
                {
                    ShowMessage(back.Text);
                    result.SetError();
                    return result;
                }

                if (back.Backs == null)
                {
                    ShowMessage("错误！返回值为空");
                    result.SetError();
                    return result;
                }

                var thumb = back.Backs as Thumb;

                thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                thumb.DragCompleted += Thumb_DragCompleted;
                thumb.DragDelta += Thumb_DragDelta;
                thumb.DragStarted += Thumb_DragStarted;

                ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                #region 数据处理

                if (!mainWindowModels.SaveThumbInfo.ContainsKey(thumb))
                {
                    mainWindowModels.SaveThumbInfo.Add(thumb, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>());
                }

                var fg = getEvents[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                var backs = await cs.EventAnalysis(fg[1].Trim('\''));

                if (!backs.Succese)
                {
                    result.SetError();
                    return result;
                }

                var getConditionsProp = backs.Backs as Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>;


                #endregion

                await Task.Run(() =>
                {
                    Thread.Sleep(100);

                    var num = i;

                    mainWindow.cvmenu.Dispatcher.Invoke(new Action(async () =>
                    {
                        var cmd = string.Empty;
                        while (checkde)
                        {
                            await Task.Run(() => { Thread.Sleep(100); });
                        }
                        nowThumb = thumb;
                        while ((GetControl("ConditionsConfig_TBox", thumb) as TextBox) == null)
                        {
                            await Task.Run(() => { Thread.Sleep(100); });
                        }
                        (GetControl("ConditionsConfig_TBox", thumb) as TextBox).Text = getEvents[num].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries)[0];
                        foreach (var item in (GetControl("Conditions_CBox", thumb) as ComboBox).Items)
                        {
                            var fgf = item.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                            if (fgf[2] == getConditionsProp.Keys.First())
                            {
                                (GetControl("Conditions_CBox", thumb) as ComboBox).SelectedItem = item;
                                cmd = item.ToString();
                                mainWindowModels.SaveThumbInfo[thumb].Add(cmd, getConditionsProp[getConditionsProp.Keys.First()]);
                                break;
                            }
                        }
                    }));
                    eventsNum++;
                });
            }

            for (int i = 0; i < getObjectives.Count; i++)
            {
                var back = await createThumbs.CreateThumb(ThumbClass.Objectives, MainFilePath, mainWindow, i * 500, 1000);

                if (!back.Succese)
                {
                    ShowMessage(back.Text);
                    result.SetError();
                    return result;
                }

                if (back.Backs == null)
                {
                    ShowMessage("错误！返回值为空");
                    result.SetError();
                    return result;
                }

                var thumb = back.Backs as Thumb;

                thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                thumb.DragCompleted += Thumb_DragCompleted;
                thumb.DragDelta += Thumb_DragDelta;
                thumb.DragStarted += Thumb_DragStarted;

                ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                #region 数据处理

                if (!mainWindowModels.SaveThumbInfo.ContainsKey(thumb))
                {
                    mainWindowModels.SaveThumbInfo.Add(thumb, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>());
                }

                var fg = getObjectives[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                var backs = await cs.ObjectiveAnalysis(fg[1].Trim('\''));

                if (!backs.Succese)
                {
                    result.SetError();
                    return result;
                }

                var getConditionsProp = backs.Backs as Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>;


                #endregion

                await Task.Run(() =>
                {
                    Thread.Sleep(100);
                    var num = i;
                    mainWindow.cvmenu.Dispatcher.Invoke(new Action(async () =>
                    {
                        var cmd = string.Empty;
                        while (checkde)
                        {
                            await Task.Run(() => { Thread.Sleep(100); });
                        }
                        nowThumb = thumb;
                        while ((GetControl("ConditionsConfig_TBox", thumb) as TextBox) == null)
                        {
                            await Task.Run(() => { Thread.Sleep(100); });
                        }
                        (GetControl("ConditionsConfig_TBox", thumb) as TextBox).Text = getObjectives[num].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries)[0];
                        foreach (var item in (GetControl("Conditions_CBox", thumb) as ComboBox).Items)
                        {
                            var fgf = item.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                            if (fgf[2] == getConditionsProp.Keys.First())
                            {
                                (GetControl("Conditions_CBox", thumb) as ComboBox).SelectedItem = item;
                                cmd = item.ToString();
                                mainWindowModels.SaveThumbInfo[thumb].Add(cmd, getConditionsProp[getConditionsProp.Keys.First()]);
                                break;
                            }
                        }
                    }));
                    objectivesNum++;
                });
            }

            var jnum = 0;

            foreach (var item in getJournal)
            {
                var back = await createThumbs.CreateThumb(ThumbClass.Journal, MainFilePath, mainWindow, jnum * 500, 1200);

                if (!back.Succese)
                {
                    ShowMessage(back.Text);
                    result.SetError();
                    return result;
                }

                if (back.Backs == null)
                {
                    ShowMessage("错误！返回值为空");
                    result.SetError();
                    return result;
                }

                var thumb = back.Backs as Thumb;

                thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                thumb.DragCompleted += Thumb_DragCompleted;
                thumb.DragDelta += Thumb_DragDelta;
                thumb.DragStarted += Thumb_DragStarted;

                ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                var str = item.Value;

                await Task.Run(() =>
                {
                    Thread.Sleep(100);
                    mainWindow.cvmenu.Dispatcher.Invoke(new Action(async () =>
                    {
                        while ((GetControl("JournalConfig_TBox", thumb) as TextBox) == null)
                        {
                            await Task.Run(() => { Thread.Sleep(100); });
                        }
                        (GetControl("JournalConfig_TBox", thumb) as TextBox).Text = item.Key;

                        (GetControl("Journal_TBox", thumb) as TextBox).Text = str;
                    }));
                    journalsNum++;
                });

                jnum++;
            }

            for (int i = 0; i < getItems.Count; i++)
            {
                var back = await createThumbs.CreateThumb(ThumbClass.Items, MainFilePath, mainWindow, i * 500, 1400);

                if (!back.Succese)
                {
                    ShowMessage(back.Text);
                    result.SetError();
                    return result;
                }

                if (back.Backs == null)
                {
                    ShowMessage("错误！返回值为空");
                    result.SetError();
                    return result;
                }

                var thumb = back.Backs as Thumb;

                thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                thumb.DragCompleted += Thumb_DragCompleted;
                thumb.DragDelta += Thumb_DragDelta;
                thumb.DragStarted += Thumb_DragStarted;

                ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                await Task.Run(() =>
                {
                    Thread.Sleep(100);
                    mainWindow.cvmenu.Dispatcher.Invoke(new Action(async () =>
                    {
                        while ((GetControl("ItemsConfig_TBox", thumb) as TextBox) == null)
                        {
                            await Task.Run(() => { Thread.Sleep(100); });
                        }
                        var fg = getItems[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                        (GetControl("ItemsConfig_TBox", thumb) as TextBox).Text = fg[0];

                        (GetControl("Items_TBox", thumb) as TextBox).Text = fg[1].Trim('\'');
                    }));
                    itemsNum++;
                });
            }

            #endregion

            #region 关系建立
            for (int i = 0; i < allConversations.Count; i++)
            {
                var reback = await createThumbs.UseNameGetThumb(ThumbClass.NPC, allConversations[i].first);
                var mainBack = await createThumbs.UseNameGetThumb(ThumbClass.Subject, allConversations[i].quester);
                if (reback.Succese && mainBack.Succese)
                {
                    var info = reback.Backs as SaveChird;
                    var mainInfo = mainBack.Backs as SaveChird;

                    var classoverBack = await ThumbClassOver(mainInfo.Saver, info.Saver, false);

                    if (classoverBack.Succese)
                    {
                        DrawThumbLine(mainInfo.Saver, info.Saver);
                    }
                }
            }

            foreach (var item in mainWindowModels.SaveThumbInfo)
            {
                var info = await FindSaveThumbInfo(item.Key);

                if (info.thumbClass == ThumbClass.NPC || info.thumbClass == ThumbClass.Player)
                {
                    foreach (var i in mainWindowModels.SaveThumbInfo[item.Key])
                    {
                        switch (GetRealCmd(i.Key))
                        {
                            case "conditions":
                                foreach (var j in i.Value["conditions"]["第 1 条参数"])
                                {
                                    var value = j.Value.TrimStart('!');
                                    var reback = await createThumbs.UseNameGetThumb(ThumbClass.Conditions, value);

                                    if (reback.Succese)
                                    {
                                        var getinfo = reback.Backs as SaveChird;

                                        var classoverBack = await ThumbClassOver(item.Key, getinfo.Saver, false);

                                        if (classoverBack.Succese)
                                        {
                                            DrawThumbLine(item.Key, getinfo.Saver);
                                        }
                                    }
                                }
                                break;
                            case "events":
                                foreach (var j in i.Value["events"]["第 1 条参数"])
                                {
                                    var reback = await createThumbs.UseNameGetThumb(ThumbClass.Events, j.Value);

                                    if (reback.Succese)
                                    {
                                        var getinfo = reback.Backs as SaveChird;

                                        var classoverBack = await ThumbClassOver(item.Key, getinfo.Saver, false);

                                        if (classoverBack.Succese)
                                        {
                                            DrawThumbLine(item.Key, getinfo.Saver);
                                        }
                                    }
                                }
                                break;
                            case "pointer":
                                if (info.thumbClass == ThumbClass.NPC)
                                {
                                    foreach (var j in i.Value["pointer"]["第 1 条参数"])
                                    {
                                        var reback = await createThumbs.UseNameGetThumb(ThumbClass.Player, j.Value);

                                        if (reback.Succese)
                                        {
                                            var getinfo = reback.Backs as SaveChird;

                                            var classoverBack = await ThumbClassOver(item.Key, getinfo.Saver, false);

                                            if (classoverBack.Succese)
                                            {
                                                DrawThumbLine(item.Key, getinfo.Saver);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var j in i.Value["pointer"]["第 1 条参数"])
                                    {
                                        var reback = await createThumbs.UseNameGetThumb(ThumbClass.NPC, j.Value);

                                        if (reback.Succese)
                                        {
                                            var getinfo = reback.Backs as SaveChird;

                                            var classoverBack = await ThumbClassOver(item.Key, getinfo.Saver, false);

                                            if (classoverBack.Succese)
                                            {
                                                DrawThumbLine(item.Key, getinfo.Saver);
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                else
                {
                    switch (info.thumbClass)
                    {
                        case ThumbClass.Conditions:
                            foreach (var i in mainWindowModels.SaveThumbInfo[item.Key])
                            {
                                var getRealCmd = GetRealCmd(i.Key);

                                var findModel = contisionProp.Where(t => t.MainClass == getRealCmd).First();

                                if (findModel.NeedTpye != null && findModel.NeedTpye.Count > 0)
                                {
                                    foreach (var j in i.Value)
                                    {//j.key是命令
                                        var num = 0;
                                        foreach (var n in j.Value)
                                        {//n.key是参数
                                            foreach (var m in n.Value)
                                            {
                                                if (!findModel.NeedTpye.Where(t => t.Key == j.Key).Any())//找到是否存在命令
                                                {
                                                    break;
                                                }

                                                var model = findModel.NeedTpye.Where(t => t.Key == j.Key).First().Value;

                                                if (!model.ContainsKey(num))//找到是否存在参数
                                                {
                                                    break;
                                                }

                                                var getNeedClass = model[num];

                                                var changeinfo = m.Value;

                                                if (findModel.isContisionCmd)
                                                {
                                                    changeinfo = changeinfo.TrimStart('!');
                                                }

                                                var reback = await createThumbs.UseNameGetThumb(getNeedClass, changeinfo);

                                                if (reback.Succese)
                                                {
                                                    var getinfo = reback.Backs as SaveChird;

                                                    var classoverBack = await ThumbClassOver(item.Key, getinfo.Saver, false);

                                                    if (classoverBack.Succese)
                                                    {
                                                        DrawThumbLine(item.Key, getinfo.Saver);
                                                    }
                                                }
                                            }
                                            num++;
                                        }
                                    }
                                }
                            }
                            break;
                        case ThumbClass.Events:
                            foreach (var i in mainWindowModels.SaveThumbInfo[item.Key])
                            {
                                var getRealCmd = GetRealCmd(i.Key);

                                var findModel = eventProp.Where(t => t.MainClass == getRealCmd).First();

                                if (findModel.NeedTpye != null && findModel.NeedTpye.Count > 0)
                                {
                                    foreach (var j in i.Value)
                                    {//j.key是命令
                                        var num = 0;
                                        foreach (var n in j.Value)
                                        {//n.key是参数
                                            foreach (var m in n.Value)
                                            {
                                                if (!findModel.NeedTpye.Where(t => t.Key == j.Key).Any())//找到是否存在命令
                                                {
                                                    break;
                                                }

                                                var model = findModel.NeedTpye.Where(t => t.Key == j.Key).First().Value;

                                                if (!model.ContainsKey(num))//找到是否存在参数
                                                {
                                                    break;
                                                }

                                                var getNeedClass = model[num];

                                                var changeinfo = m.Value;

                                                if (getNeedClass == ThumbClass.Conditions)
                                                {
                                                    changeinfo = changeinfo.TrimStart('!');
                                                }

                                                var reback = await createThumbs.UseNameGetThumb(getNeedClass, changeinfo);

                                                if (reback.Succese)
                                                {
                                                    var getinfo = reback.Backs as SaveChird;

                                                    var classoverBack = await ThumbClassOver(item.Key, getinfo.Saver, false);

                                                    if (classoverBack.Succese)
                                                    {
                                                        DrawThumbLine(item.Key, getinfo.Saver);
                                                    }
                                                }
                                            }
                                            num++;
                                        }
                                    }
                                }
                            }
                            break;
                        case ThumbClass.Objectives:
                            foreach (var i in mainWindowModels.SaveThumbInfo[item.Key])
                            {
                                var getRealCmd = GetRealCmd(i.Key);

                                var findModel = objectiveProp.Where(t => t.MainClass == getRealCmd).First();

                                if (findModel.NeedTpye != null && findModel.NeedTpye.Count > 0)
                                {
                                    foreach (var j in i.Value)
                                    {//j.key是命令
                                        var num = 0;
                                        foreach (var n in j.Value)
                                        {//n.key是参数
                                            foreach (var m in n.Value)
                                            {
                                                if (!findModel.NeedTpye.Where(t => t.Key == j.Key).Any())//找到是否存在命令
                                                {
                                                    break;
                                                }

                                                var model = findModel.NeedTpye.Where(t => t.Key == j.Key).First().Value;

                                                if (!model.ContainsKey(num))//找到是否存在参数
                                                {
                                                    break;
                                                }

                                                var getNeedClass = model[num];

                                                var changeinfo = m.Value;

                                                if (getNeedClass == ThumbClass.Conditions)
                                                {
                                                    changeinfo = changeinfo.TrimStart('!');
                                                }

                                                var reback = await createThumbs.UseNameGetThumb(getNeedClass, changeinfo);

                                                if (reback.Succese)
                                                {
                                                    var getinfo = reback.Backs as SaveChird;

                                                    var classoverBack = await ThumbClassOver(item.Key, getinfo.Saver, false);

                                                    if (classoverBack.Succese)
                                                    {
                                                        DrawThumbLine(item.Key, getinfo.Saver);
                                                    }
                                                }
                                            }
                                            num++;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            #endregion

            await Task.Run(() =>
            {
                while (checkde)
                {
                    Thread.Sleep(100);
                }
            });

            result.SetSuccese();
            return result;
        }
        private async Task<ReturnModel> ReadJson()
        {
            var result = new ReturnModel();

            if (string.IsNullOrEmpty(MainFilePath))
            {
                ShowMessage("请选则正确的文件");
                result.SetError();
                return result;
            }

            if (!FileService.IsHaveFile(MainFilePath))
            {
                ShowMessage("main入口文件不存在请重新指定Main.yml文件");
                result.SetError();
                return result;
            }
            mainWindow.IsEnabled = false;

            var message = new MessageModel()
            {
                IsLogin = SocketModel.isLogin,
                JsonInfo = JsonInfo.GetSaveData,
                UserName = SocketModel.userName,
                Message = "",
                Other = SelectData.Code.ToString()
            };

            var jsonMessage = FileService.SaveToJson(message);

            var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);

            if (getMessage == null || !getMessage.Succese)
            {
                result.SetError();
                return result;
            }

            var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

            if (getModel.Token != SocketModel.token)
            {
                result.SetError();
                return result;
            }

            var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

            if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.GetSaveData || !getRealMessage.IsLogin)
            {
                result.SetError();
                return result;
            }

            var getData = FileService.JsonToProp<savedatum>(getRealMessage.Message);

            if (getData == null)
            {
                result.SetError();
                return result;
            }

            var saveMainInfo = new List<ThumbsModels>();

            var saveAllChildInfo = new List<SaveChilds>();

            var saveNPCEOInfo = new List<ThumbInfoModel>();

            var saveJournalInfo = new List<ThumbsModels>();

            var saveItemsInfo = new List<ThumbsModels>();

            var dic = new List<ThumbCoordinateModel>();

            var disPath = FileService.GetFileDirectory(MainFilePath);

            try
            {
                saveMainInfo = FileService.JsonToProp<List<ThumbsModels>>(getData.main);

                saveAllChildInfo = FileService.JsonToProp<List<SaveChilds>>(getData.maininfo);

                saveNPCEOInfo = FileService.JsonToProp<List<ThumbInfoModel>>(getData.data);

                saveJournalInfo = FileService.JsonToProp<List<ThumbsModels>>(getData.journaldata);

                saveItemsInfo = FileService.JsonToProp<List<ThumbsModels>>(getData.itemsdata);

                dic = FileService.JsonToProp<List<ThumbCoordinateModel>>(getData.coordinate);
            }
            catch
            {
                ShowMessage("数据文件损坏！请不要擅自修改数据文件！如果您没有备份数据文件那么您将失去数据！");
                result.SetError();
                return result;
            }

            var createThumbBase = new CreateThumbsBase(mainWindow);

            if (saveMainInfo == null || saveAllChildInfo == null || dic == null)
            {
                ShowMessage("未找到配置文件");
                result.SetError();
                return result;
            }

            #region Thumb创建

            foreach (var item in saveMainInfo)
            {
                var back = await createThumbBase.CreateThumb(ThumbClass.Subject, MainFilePath, mainWindow);

                if (!back.Succese)
                {
                    ShowMessage("对话主体生成失败,请尝试重新生成!"); 
                    result.SetError();
                    return result;
                }

                var thumb = back.Backs as Thumb;
                nowThumb = thumb;
                thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                thumb.DragCompleted += Thumb_DragCompleted;
                thumb.DragDelta += Thumb_DragDelta;
                thumb.DragStarted += Thumb_DragStarted;

                ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                ActBase.KeyDown -= ActBase_KeyDown;
                ActBase.KeyDown += ActBase_KeyDown;
                isHaveSubjcet = true;

                await Task.Run(() =>
                {
                    Thread.Sleep(500);
                    thumb.Dispatcher.Invoke(new Action(() =>
                    {
                        try
                        {
                            (GetControl("NpcName_TBox", thumb) as TextBox).Text = item.Type;
                            (GetControl("ShowNpcName_TBox", thumb) as TextBox).Text = item.Text;
                            (GetControl("MainName_TBox", thumb) as TextBox).Text = item.Config;
                        }
                        catch
                        {

                        }

                    }));
                });
            }

            foreach (var item in saveNPCEOInfo)
            {
                if (item.thumbClass == ThumbClass.Player || item.thumbClass == ThumbClass.NPC)
                {
                    var npcback = await createThumbBase.CreateThumb(item.thumbClass, MainFilePath, mainWindow);

                    if (!npcback.Succese)
                    {
                        ShowMessage(npcback.Text);
                        result.SetError();
                        return result;
                    }

                    if (npcback.Backs == null)
                    {
                        ShowMessage("错误！返回值为空");
                        result.SetError();
                        return result;
                    }

                    var thumbs = npcback.Backs as Thumb;

                    thumbs.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                    thumbs.DragCompleted += Thumb_DragCompleted;
                    thumbs.DragDelta += Thumb_DragDelta;
                    thumbs.DragStarted += Thumb_DragStarted;

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

                    if (item.thumbClass == ThumbClass.NPC)
                    {
                        if (npcLoader.saveThumbInfoWindowModel == null)
                        {
                            npcLoader.saveThumbInfoWindowModel = new Dictionary<Thumb, ThumbInfoWindowModel>()
                                    {
                                        {thumbs,infoModel},
                                    };
                        }
                        else
                        {
                            npcLoader.saveThumbInfoWindowModel.Add(thumbs, infoModel);
                        }
                    }
                    else if(item.thumbClass == ThumbClass.Player)
                    {
                        if (playerLoader.saveThumbInfoWindowModel == null)
                        {
                            playerLoader.saveThumbInfoWindowModel = new Dictionary<Thumb, ThumbInfoWindowModel>()
                                    {
                                        {thumbs,infoModel},
                                    };
                        }
                        else
                        {
                            playerLoader.saveThumbInfoWindowModel.Add(thumbs, infoModel);
                        }
                    }


                    await Task.Run(() =>
                    {
                        Thread.Sleep(500);
                        thumbs.Dispatcher.Invoke(new Action(() =>
                        {
                            (GetControl("ConditionsConfig_TBox", thumbs) as TextBox).Text = item.Name;
                        }));
                        if (item.thumbClass == ThumbClass.Player)
                        {
                            playerNum++;
                        }
                        else
                        {
                            npcNum++;
                        }

                    });

                }
                else
                {
                    var back = await createThumbBase.CreateThumb(item.thumbClass, MainFilePath, mainWindow);

                    if (!back.Succese)
                    {
                        ShowMessage(back.Text);
                        result.SetError();
                        return result;
                    }

                    if (back.Backs == null)
                    {
                        ShowMessage("错误！返回值为空");
                        result.SetError();
                        return result;
                    }

                    var thumb = back.Backs as Thumb;

                    thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                    thumb.DragCompleted += Thumb_DragCompleted;
                    thumb.DragDelta += Thumb_DragDelta;
                    thumb.DragStarted += Thumb_DragStarted;

                    ThumbNums = MainWindowModels.saveThumbs.Count.ToString();
                    await Task.Run(() =>
                    {
                        Thread.Sleep(100);
                        mainWindow.cvmenu.Dispatcher.Invoke(new Action(async () =>
                        {
                            while ((GetControl("ConditionsConfig_TBox", thumb) as TextBox) == null)
                            {
                                await Task.Run(() => { Thread.Sleep(100); });
                            }

                            (GetControl("ConditionsConfig_TBox", thumb) as TextBox).Text = item.Name;

                            if (item.data != null)
                            {
                                var getRealCmd = GetRealCmd(item.data.Keys.First());

                                foreach (var i in (GetControl("Conditions_CBox", thumb) as ComboBox).Items)
                                {
                                    var fgf = i.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                                    if (fgf[2] == getRealCmd)
                                    {
                                        while (checkde)
                                        {
                                            await Task.Run(() => { Thread.Sleep(100); });
                                        }
                                        nowThumb = thumb;
                                        (GetControl("Conditions_CBox", thumb) as ComboBox).SelectedItem = i;
                                        break;
                                    }
                                }
                            }
                            


                        }));
                        if (item.thumbClass == ThumbClass.Conditions)
                        {
                            conditionsNum++;
                        }
                        else if (item.thumbClass == ThumbClass.Events)
                        {
                            eventsNum++;
                        }
                        else if (item.thumbClass == ThumbClass.Objectives)
                        {
                            objectivesNum++;
                        }

                    });
                }
            }

            foreach (var item in saveJournalInfo)
            {
                var back = await createThumbBase.CreateThumb(ThumbClass.Journal, MainFilePath, mainWindow);

                if (!back.Succese)
                {
                    ShowMessage(back.Text);
                    result.SetError();
                    return result;
                }

                if (back.Backs == null)
                {
                    ShowMessage("错误！返回值为空");
                    result.SetError();
                    return result;
                }

                var thumb = back.Backs as Thumb;
                nowThumb = thumb;
                thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                thumb.DragCompleted += Thumb_DragCompleted;
                thumb.DragDelta += Thumb_DragDelta;
                thumb.DragStarted += Thumb_DragStarted;

                ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                await Task.Run(() =>
                {
                    mainWindow.cvmenu.Dispatcher.Invoke(new Action(async () =>
                    {
                        while ((GetControl("JournalConfig_TBox", thumb) as TextBox) == null)
                        {
                            await Task.Run(() => { Thread.Sleep(100); });
                        }
                        (GetControl("JournalConfig_TBox", thumb) as TextBox).Text = item.Config;
                        (GetControl("Journal_TBox", thumb) as TextBox).Text = item.Text;
                    }));
                    journalsNum++;
                });
            }

            foreach (var item in saveItemsInfo)
            {
                var back = await createThumbBase.CreateThumb(ThumbClass.Items, MainFilePath, mainWindow);

                if (!back.Succese)
                {
                    ShowMessage(back.Text);
                    result.SetError();
                    return result;
                }

                if (back.Backs == null)
                {
                    ShowMessage("错误！返回值为空");
                    result.SetError();
                    return result;
                }

                var thumb = back.Backs as Thumb;
                nowThumb = thumb;
                thumb.PreviewMouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                thumb.DragCompleted += Thumb_DragCompleted;
                thumb.DragDelta += Thumb_DragDelta;
                thumb.DragStarted += Thumb_DragStarted;

                ThumbNums = MainWindowModels.saveThumbs.Count.ToString();

                await Task.Run(() =>
                {
                    mainWindow.cvmenu.Dispatcher.Invoke(new Action(async () =>
                    {
                        while ((GetControl("ItemsConfig_TBox", thumb) as TextBox) == null)
                        {
                            await Task.Run(() => { Thread.Sleep(100); });
                        }
                        (GetControl("ItemsConfig_TBox", thumb) as TextBox).Text = item.Config;
                        (GetControl("Items_TBox", thumb) as TextBox).Text = item.Text;
                    }));
                    itemsNum++;
                });
            }
            #endregion

            #region 数据录入

            var treeView = new TreeViewBase();

            foreach (var item in saveNPCEOInfo)
            {
                var getThumb = await createThumbBase.UseNameGetThumb(item.thumbClass, item.Name);

                if (!getThumb.Succese)
                {
                    ShowMessage("未找到相关控件，请尝试重新读取！");
                    result.SetError();
                    return result;
                }

                var thumb = getThumb.Backs as SaveChird;

                if(item.data == null)
                {
                    continue;
                }

                if (mainWindowModels.SaveThumbInfo.ContainsKey(thumb.Saver))
                {
                    foreach (var i in item.data)
                    {
                        mainWindowModels.SaveThumbInfo[thumb.Saver].Add(i.Key, i.Value);
                    }
                }
                else
                {
                    mainWindowModels.SaveThumbInfo.Add(thumb.Saver, item.data);
                }

                foreach (var i in item.data)
                {//i.key = CMD
                    foreach (var j in i.Value)
                    {//j.key = 命令
                        foreach (var n in j.Value)
                        {//n.key = 参数
                            foreach (var m in n.Value)
                            {//项
                                if (thumb.thumbClass == ThumbClass.Player)
                                {
                                    await treeView.AddItemToSaves(thumb.Saver, j.Key, n.Key, m.Key, "", "", true, playerLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo, false);
                                }
                                else if (thumb.thumbClass == ThumbClass.NPC)
                                {
                                    await treeView.AddItemToSaves(thumb.Saver, j.Key, n.Key, m.Key, "", "", true, npcLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo, false);
                                }
                                //else if(thumb.thumbClass == ThumbClass.Conditions)
                                //{
                                //    await treeView.AddItemToSaves(thumb.Saver, j.Key, n.Key, m.Value, "", "", true, contisionLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo, false);
                                //}
                                //else if (thumb.thumbClass == ThumbClass.Events)
                                //{
                                //    await treeView.AddItemToSaves(thumb.Saver, j.Key, n.Key, m.Value, "", "", true, eventLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo, false);
                                //}
                                //else if(thumb.thumbClass == ThumbClass.Objectives)
                                //{
                                //    await treeView.AddItemToSaves(thumb.Saver, j.Key, n.Key, m.Value, "", "", true, objectiveLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo, false);
                                //}
                            }
                        }
                    }
                }
            }

            foreach (var item in saveAllChildInfo)
            {
                var getThumb = await createThumbBase.UseNameGetThumb(item.thumbClass, item.Saver);

                if (!getThumb.Succese)
                {
                    ShowMessage($"未找到相关控件:{item.Saver}");
                }

                var thumb = getThumb.Backs as SaveChird;

                if (thumb == null)
                {
                    continue;
                }

                foreach (var i in item.Children)
                {
                    var getchild = await createThumbBase.UseNameGetThumb(ThumbClass.NPC, i);

                    if (!getchild.Succese)
                    {
                        ShowMessage("未找到相关控件，请尝试重新读取！ 22");
                        result.SetError();
                        return result;
                    }

                    var getThumbInfo = getchild.Backs as SaveChird;

                    var classoverBack = await ThumbClassOver(thumb.Saver, getThumbInfo.Saver, false);
                }

                foreach (var i in item.Fathers)
                {
                    var getchild = await createThumbBase.UseNameGetThumb(ThumbClass.Events, i);

                    if (!getchild.Succese)
                    {
                        ShowMessage("未找到相关控件，请尝试重新读取！ 33");
                        result.SetError();
                        return result;
                    }

                    var getThumbInfo = getchild.Backs as SaveChird;

                    var classoverBack = await ThumbClassOver(getThumbInfo.Saver, thumb.Saver, false);
                }
            }

            #endregion

            #region 位置迁移
            foreach (var item in dic)
            {
                var getThumb = await createThumbBase.UseNameGetThumb(item.thumbClass, item.Name);

                if (!getThumb.Succese)
                {
                    ShowMessage($"位置配置中未找到相关控件{item.Name}");
                    continue;
                }

                var thumb = getThumb.Backs as SaveChird;

                Canvas.SetLeft(thumb.Saver, item.X);

                Canvas.SetTop(thumb.Saver, item.Y);
            }
            #endregion

            #region 关系建立

            try
            {
                var getMainThumb = MainWindowModels.saveThumbs.Where(t => t.thumbClass == ThumbClass.Subject).ToList();

                foreach (var item in getMainThumb)
                {
                    foreach (var i in item.Children)
                    {
                        DrawThumbLine(item.Saver, i);
                    }

                    foreach (var i in item.Fathers)
                    {
                        DrawThumbLine(i, item.Saver);
                    }
                }
            }
            catch
            {
                ShowMessage("未找到相关控件，请尝试重新读取！  44");
                result.SetError();
                return result;
            }

            foreach (var item in mainWindowModels.SaveThumbInfo)
            {
                var info = await FindSaveThumbInfo(item.Key);

                if (info.thumbClass == ThumbClass.NPC || info.thumbClass == ThumbClass.Player)
                {
                    foreach (var i in mainWindowModels.SaveThumbInfo[item.Key])
                    {
                        switch (GetRealCmd(i.Key))
                        {
                            case "conditions":
                                foreach (var j in i.Value["conditions"]["第 1 条参数"])
                                {
                                    var value = j.Value.TrimStart('!');
                                    var reback = await createThumbBase.UseNameGetThumb(ThumbClass.Conditions, value);

                                    if (reback.Succese)
                                    {
                                        var getinfo = reback.Backs as SaveChird;

                                        var classoverBack = await ThumbClassOver(item.Key, getinfo.Saver, false);

                                        if (classoverBack.Succese)
                                        {
                                            DrawThumbLine(item.Key, getinfo.Saver);
                                        }
                                    }
                                }
                                break;
                            case "events":
                                foreach (var j in i.Value["events"]["第 1 条参数"])
                                {
                                    var reback = await createThumbBase.UseNameGetThumb(ThumbClass.Events, j.Value);

                                    if (reback.Succese)
                                    {
                                        var getinfo = reback.Backs as SaveChird;

                                        var classoverBack = await ThumbClassOver(item.Key, getinfo.Saver, false);

                                        if (classoverBack.Succese)
                                        {
                                            DrawThumbLine(item.Key, getinfo.Saver);
                                        }
                                    }
                                }
                                break;
                            case "pointer":
                                if (info.thumbClass == ThumbClass.NPC)
                                {
                                    foreach (var j in i.Value["pointer"]["第 1 条参数"])
                                    {
                                        var reback = await createThumbBase.UseNameGetThumb(ThumbClass.Player, j.Value);

                                        if (reback.Succese)
                                        {
                                            var getinfo = reback.Backs as SaveChird;

                                            var classoverBack = await ThumbClassOver(item.Key, getinfo.Saver, false);

                                            if (classoverBack.Succese)
                                            {
                                                DrawThumbLine(item.Key, getinfo.Saver);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var j in i.Value["pointer"]["第 1 条参数"])
                                    {
                                        var reback = await createThumbBase.UseNameGetThumb(ThumbClass.NPC, j.Value);

                                        if (reback.Succese)
                                        {
                                            var getinfo = reback.Backs as SaveChird;

                                            var classoverBack = await ThumbClassOver(item.Key, getinfo.Saver, false);

                                            if (classoverBack.Succese)
                                            {
                                                DrawThumbLine(item.Key, getinfo.Saver);
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                else
                {
                    switch (info.thumbClass)
                    {
                        case ThumbClass.Conditions:
                            foreach (var i in mainWindowModels.SaveThumbInfo[item.Key])
                            {
                                var getRealCmd = GetRealCmd(i.Key);

                                var findModel = contisionProp.Where(t => t.MainClass == getRealCmd).First();

                                if (findModel.NeedTpye != null && findModel.NeedTpye.Count > 0)
                                {
                                    foreach (var j in i.Value)
                                    {//j.key是命令

                                        var model = findModel.NeedTpye.Where(t => t.Key == j.Key).First().Value;

                                        var newModel = new Dictionary<string, ThumbClass>();

                                        foreach (var sd in model)
                                        {
                                            newModel.Add($"第 {sd.Key + 1} 条参数", sd.Value);
                                        }

                                        foreach (var n in j.Value)
                                        {//n.key是参数
                                            foreach (var m in n.Value)
                                            {
                                                if (!findModel.NeedTpye.Where(t => t.Key == j.Key).Any())//找到是否存在命令
                                                {
                                                    break;
                                                }

                                                if (!newModel.ContainsKey(n.Key))//找到是否存在参数
                                                {
                                                    break;
                                                }

                                                var getNeedClass = newModel[n.Key];

                                                var changeinfo = m.Value;

                                                if (getNeedClass == ThumbClass.Conditions)
                                                {
                                                    changeinfo = changeinfo.TrimStart('!');
                                                }

                                                var reback = await createThumbBase.UseNameGetThumb(getNeedClass, changeinfo);

                                                if (reback.Succese)
                                                {
                                                    var getinfo = reback.Backs as SaveChird;

                                                    var classoverBack = await ThumbClassOver(item.Key, getinfo.Saver, false);

                                                    if (classoverBack.Succese)
                                                    {
                                                        DrawThumbLine(item.Key, getinfo.Saver);
                                                    }
                                                    else
                                                    {
                                                        ShowMessage(classoverBack.Text);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case ThumbClass.Events:
                            foreach (var i in mainWindowModels.SaveThumbInfo[item.Key])
                            {
                                var getRealCmd = GetRealCmd(i.Key);

                                var findModel = eventProp.Where(t => t.MainClass == getRealCmd).First();

                                if (findModel.NeedTpye != null && findModel.NeedTpye.Count > 0)
                                {
                                    foreach (var j in i.Value)
                                    {//j.key是命令

                                        var model = findModel.NeedTpye.Where(t => t.Key == j.Key).First().Value;

                                        var newModel = new Dictionary<string, ThumbClass>();

                                        foreach (var sd in model)
                                        {
                                            newModel.Add($"第 {sd.Key + 1} 条参数", sd.Value);
                                        }

                                        foreach (var n in j.Value)
                                        {//n.key是参数
                                            foreach (var m in n.Value)
                                            {
                                                if (!findModel.NeedTpye.Where(t => t.Key == j.Key).Any())//找到是否存在命令
                                                {
                                                    break;
                                                }

                                                if (!newModel.ContainsKey(n.Key))//找到是否存在参数
                                                {
                                                    break;
                                                }

                                                var getNeedClass = newModel[n.Key];

                                                var changeinfo = m.Value;

                                                if (getNeedClass == ThumbClass.Conditions)
                                                {
                                                    changeinfo = changeinfo.TrimStart('!');
                                                }

                                                var reback = await createThumbBase.UseNameGetThumb(getNeedClass, changeinfo);

                                                if (reback.Succese)
                                                {
                                                    var getinfo = reback.Backs as SaveChird;

                                                    var classoverBack = await ThumbClassOver(item.Key, getinfo.Saver, false);

                                                    if (classoverBack.Succese)
                                                    {
                                                        DrawThumbLine(item.Key, getinfo.Saver);
                                                    }
                                                    else
                                                    {
                                                        ShowMessage(classoverBack.Text);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case ThumbClass.Objectives:
                            foreach (var i in mainWindowModels.SaveThumbInfo[item.Key])
                            {
                                var getRealCmd = GetRealCmd(i.Key);

                                var findModel = objectiveProp.Where(t => t.MainClass == getRealCmd).First();

                                if (findModel.NeedTpye != null && findModel.NeedTpye.Count > 0)
                                {
                                    foreach (var j in i.Value)
                                    {//j.key是命令

                                        var model = findModel.NeedTpye.Where(t => t.Key == j.Key).First().Value;

                                        var newModel = new Dictionary<string, ThumbClass>();

                                        foreach (var sd in model)
                                        {
                                            newModel.Add($"第 {sd.Key + 1} 条参数", sd.Value);
                                        }

                                        foreach (var n in j.Value)
                                        {//n.key是参数
                                            foreach (var m in n.Value)
                                            {
                                                if (!findModel.NeedTpye.Where(t => t.Key == j.Key).Any())//找到是否存在命令
                                                {
                                                    break;
                                                }

                                                if (!newModel.ContainsKey(n.Key))//找到是否存在参数
                                                {
                                                    break;
                                                }

                                                var getNeedClass = newModel[n.Key];

                                                var changeinfo = m.Value;

                                                if (getNeedClass == ThumbClass.Conditions)
                                                {
                                                    changeinfo = changeinfo.TrimStart('!');
                                                }

                                                var reback = await createThumbBase.UseNameGetThumb(getNeedClass, changeinfo);

                                                if (reback.Succese)
                                                {
                                                    var getinfo = reback.Backs as SaveChird;

                                                    var classoverBack = await ThumbClassOver(item.Key, getinfo.Saver, false);

                                                    if (classoverBack.Succese)
                                                    {
                                                        DrawThumbLine(item.Key, getinfo.Saver);
                                                    }
                                                    else
                                                    {
                                                        ShowMessage(classoverBack.Text);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            #endregion

            await Task.Run(() =>
            {
                while (checkde)
                {
                    Thread.Sleep(100);
                }
            });

            result.SetSuccese();
            return result;
        }

        private static async Task<ReturnModel> SelectFilePath()
        {
            var result = new ReturnModel();

            var path = string.Empty;

            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择Main.yml";
            dialog.Filter = "入口文件|main.yml";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string file = dialog.FileName;

                path = file;
            }
            else
            {
                path = string.Empty;
            }

            result = await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(path))
                {
                    result.SetError();
                    return result;
                }
                else
                {
                    result.SetSuccese(path);
                    return result;
                }
            });

            return result;
        }

        private async Task<ReturnModel> ComboBoxChangeSeleted(Thumb nowThumbs, ComboBox control)
        {
            var result = new ReturnModel();

            var thumbClass = (await FindSaveThumbInfo(nowThumbs)).thumbClass;

            if (thumbClass != ThumbClass.Player && thumbClass != ThumbClass.NPC)
            {
                var getSaver = new List<SaveLine>();

                for (int i = 0; i < saveLines.Count; i++)
                {
                    if (saveLines[i].FatherName == nowThumbs)
                    {
                        getSaver.Add(saveLines[i]);
                    }
                }

                var bidui = new GetThumbInfoBase(objectiveProp,contisionProp,eventProp);

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
                            result.SetError();
                            return result;
                        }
                    }
                }
            }

            #region 控件的改变

            switch (thumbClass)
            {
                case ThumbClass.Player:
                    var loaderBacks = await playerLoader.ChangeThumb((control.SelectedItem as ComboBoxItem).Content.ToString(), nowThumbs, thumbInfoWindow.TreeView_Tv);

                    if (loaderBacks != null && !loaderBacks.Succese)
                    {
                        ShowMessage(loaderBacks.Text);
                        result.SetError();
                        return result;
                    }

                    break;
                case ThumbClass.NPC:
                    loaderBacks = await npcLoader.ChangeThumb((control.SelectedItem as ComboBoxItem).Content.ToString(), nowThumbs, thumbInfoWindow.TreeView_Tv);

                    if (loaderBacks != null && !loaderBacks.Succese)
                    {
                        ShowMessage(loaderBacks.Text);
                        result.SetError();
                        return result;
                    }
                    break;
                case ThumbClass.Conditions:
                    
                    var loaderBack = await contisionLoader.ChangeThumb(contisionProp, (control.SelectedItem as ComboBoxItem).Content.ToString(), nowThumbs);

                    if (loaderBack != null && !loaderBack.Succese)
                    {
                        ShowMessage(loaderBack.Text);
                        result.SetError();
                        return result;
                    }

                    var back = await ChangeTheTreeView();

                    if (back != null && !back.Succese)
                    {
                        ShowMessage(back.Text);
                    }

                    if (mainWindowModels.SaveThumbInfo.ContainsKey(nowThumbs) && mainWindow.IsEnabled && loaderBack.Backs ==null)
                    {
                        mainWindowModels.SaveThumbInfo[nowThumbs].Clear();
                    }
                    break;

                case ThumbClass.Events:

                    loaderBack = await eventLoader.ChangeThumb(eventProp, (control.SelectedItem as ComboBoxItem).Content.ToString(), nowThumbs);

                    if (loaderBack != null && !loaderBack.Succese)
                    {
                        ShowMessage(loaderBack.Text);
                        result.SetError();
                        return result;
                    }

                    back = await ChangeTheTreeView();

                    if (back != null && !back.Succese)
                    {
                        ShowMessage(back.Text);
                    }
                    if (mainWindowModels.SaveThumbInfo.ContainsKey(nowThumbs) && mainWindow.IsEnabled)
                    {
                        mainWindowModels.SaveThumbInfo[nowThumbs].Clear();
                    }
                    break;

                case ThumbClass.Objectives:

                    loaderBack = await objectiveLoader.ChangeThumb(objectiveProp, (control.SelectedItem as ComboBoxItem).Content.ToString(), nowThumbs);

                    if (loaderBack != null && !loaderBack.Succese)
                    {
                        ShowMessage(loaderBack.Text);
                        result.SetError();
                        return result;
                    }

                    back = await ChangeTheTreeView();

                    if (back != null && !back.Succese)
                    {
                        ShowMessage(back.Text);
                    }
                    if (mainWindowModels.SaveThumbInfo.ContainsKey(nowThumbs) && mainWindow.IsEnabled)
                    {
                        mainWindowModels.SaveThumbInfo[nowThumbs].Clear();
                    }
                    break;
            }

            checkde = false;
            #endregion

            result.SetSuccese("");

            return result;
        }

        /// <summary>
        /// 获取真实命令
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        protected static string GetRealCmd(string txt, string fg = ": ")
        {
            var getSqlit = txt.Split(new string[] { fg }, StringSplitOptions.RemoveEmptyEntries);

            if (getSqlit.Length == 3 || getSqlit.Length == 2)
            {
                return getSqlit[getSqlit.Length - 1];
            }
            else
            {
                return txt;
            }
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
            try
            {
                var thumbinfo = await FindSaveThumbInfo(nowThumb);

                if (thumbinfo != null && thumbinfo.thumbClass == ThumbClass.Conditions)
                {
                    var control = nowThumb.Template.FindName("Conditions_CBox", nowThumb) as ComboBox;

                    if (control != null && control.Items.Count > 0 && control.SelectedItem != null && thumbInfoWindow != null)
                    {
                        return await contisionLoader.ChangeTheTree(thumbInfoWindow.FindName("TreeView_Tv") as TreeView, contisionProp, (control.SelectedItem as ComboBoxItem).Content.ToString());
                    }

                }

                if (thumbinfo != null && thumbinfo.thumbClass == ThumbClass.Events)
                {
                    var control = nowThumb.Template.FindName("Conditions_CBox", nowThumb) as ComboBox;



                    if (control!=null&&control.Items.Count > 0 && control.SelectedItem != null && thumbInfoWindow != null)
                    {
                        return await eventLoader.ChangeTheTree(thumbInfoWindow.FindName("TreeView_Tv") as TreeView, eventProp, (control.SelectedItem as ComboBoxItem).Content.ToString());
                    }

                }

                if (thumbinfo != null && thumbinfo.thumbClass == ThumbClass.Objectives)
                {
                    var control = nowThumb.Template.FindName("Conditions_CBox", nowThumb) as ComboBox;

                    if (control != null && control.Items.Count > 0 && control.SelectedItem != null && thumbInfoWindow != null)
                    {
                        return await objectiveLoader.ChangeTheTree(thumbInfoWindow.FindName("TreeView_Tv") as TreeView, objectiveProp, (control.SelectedItem as ComboBoxItem).Content.ToString());
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
            }
            catch
            {
                var back = new ReturnModel();
                back.SetError();

                return back;
            }
            

            return null;
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
        private async Task<ReturnModel> ThumbClassOver(Thumb thumbFt,Thumb thumbCl,bool useAdd = true)
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

            var comparison = new GetThumbInfoBase(objectiveProp, contisionProp, eventProp);

            var cando = await comparison.GetThisCanFather(get_ThumbFt, get_ThumbCl);

            if (!cando.Succese)
            {
                back.SetError(@"不允许的归类，请查看WIKI https://www.mcbbs.net/forum.php?mod=viewthread&tid=676922");
                return back;
            }

            #endregion

            if (useAdd)
            {
                var result = await AddTextToThumb(get_ThumbFt, get_ThumbCl);

                if (!result.Succese)
                {
                    back.SetError(result.Text);
                    return back;
                }
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

        private async Task<ReturnModel> ThumbClassOverDelete(Thumb thumbFt, Thumb thumbCl)
        {
            var result = new ReturnModel();

            var get_ThumbCl = await FindSaveThumbInfo(thumbCl);

            var get_ThumbFt = await FindSaveThumbInfo(thumbFt);

            if (get_ThumbCl == null || get_ThumbFt == null)
            {
                result.SetError("获取父级或子级卡片错误！\n这是一个严重的问题,请发送邮件报告jk@jklss.cn");
                return result;
            }

            if (!get_ThumbCl.Fathers.Contains(thumbFt) || !get_ThumbFt.Children.Contains(thumbCl))
            {
                result.SetError("两个卡片并没有归类过");
                return result;
            }

            if(get_ThumbFt.thumbClass == ThumbClass.Conditions||
                get_ThumbFt.thumbClass == ThumbClass.Events|| 
                get_ThumbFt.thumbClass == ThumbClass.Objectives|| 
                get_ThumbFt.thumbClass == ThumbClass.NPC|| 
                get_ThumbFt.thumbClass == ThumbClass.Player)
            {
                if (mainWindowModels.SaveThumbInfo.ContainsKey(thumbFt))
                {
                    var getName = (GetControl("ConditionsConfig_TBox", get_ThumbCl.Saver) as TextBox).Text;
                    var treeBase = new TreeViewBase();
                    foreach (var item in mainWindowModels.SaveThumbInfo[thumbFt])
                    {
                        foreach (var i in item.Value)
                        {
                            foreach (var j in i.Value)
                            {
                                foreach (var n in j.Value)
                                {
                                    if(n.Value == getName)
                                    {
                                        if(get_ThumbFt.thumbClass == ThumbClass.Conditions)
                                        {
                                            await treeBase.DeleteItemToSaves(get_ThumbFt.Saver, i.Key, j.Key, n.Key, item.Key,
                                                contisionLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);
                                        }
                                        if (get_ThumbFt.thumbClass == ThumbClass.Events)
                                        {
                                            await treeBase.DeleteItemToSaves(get_ThumbFt.Saver, i.Key, j.Key, n.Key, item.Key,
                                                eventLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);
                                        }
                                        if (get_ThumbFt.thumbClass == ThumbClass.Objectives)
                                        {
                                            await treeBase.DeleteItemToSaves(get_ThumbFt.Saver, i.Key, j.Key, n.Key, item.Key,
                                                objectiveLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);
                                        }
                                        if (get_ThumbFt.thumbClass == ThumbClass.NPC)
                                        {
                                            await treeBase.DeleteItemToSaves(get_ThumbFt.Saver, i.Key, j.Key, n.Key, item.Key,
                                                npcLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);
                                        }
                                        if (get_ThumbFt.thumbClass == ThumbClass.Player)
                                        {
                                            await treeBase.DeleteItemToSaves(get_ThumbFt.Saver, i.Key, j.Key, n.Key, item.Key,
                                                playerLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var delLine = saveLines.Where(t => t.FatherName == thumbFt && t.ChirldName == thumbCl).FirstOrDefault();

            if(delLine != null)
            {
                mainWindow.cvmenu.Children.Remove(delLine.line);

                saveLines.Remove(delLine);
            }

            var delLinef = saveLines.Where(t => t.ChirldName == thumbFt && t.FatherName == thumbCl).FirstOrDefault();

            if (delLinef != null)
            {
                mainWindow.cvmenu.Children.Remove(delLinef.line);

                saveLines.Remove(delLinef);
            }

            get_ThumbCl.Fathers.Remove(thumbFt);
            get_ThumbFt.Children.Remove(thumbCl);

            result.SetSuccese();
            return result;
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

                    var fg = (getComb.SelectedItem as ComboBoxItem).Content.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    var realCmd = string.Empty;

                    if(fg.Length > 1)
                    {
                        realCmd = fg[fg.Length - 1];
                    }
                    else
                    {
                        realCmd = (getComb.SelectedItem as ComboBoxItem).Content.ToString();
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

                    while(saveResult == null)
                    {
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
                    }

                    if (!mainWindowModels.SaveThumbInfo.ContainsKey(father.Saver))
                    {
                        mainWindowModels.SaveThumbInfo.Add(father.Saver, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>()
                        {
                            { (getComb.SelectedItem as ComboBoxItem).Content.ToString(),new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                                {
                                    { saveResult.One,new Dictionary<string, Dictionary<string, string>>()
                                    {
                                        { saveResult.Two,new Dictionary<string, string>()}
                                    } }
                                }
                            }
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver].ContainsKey((getComb.SelectedItem as ComboBoxItem).Content.ToString()))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver].Add((getComb.SelectedItem as ComboBoxItem).Content.ToString(), new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                        {
                            { saveResult.One,new Dictionary<string, Dictionary<string, string>>()
                                {
                                    { saveResult.Two,new Dictionary<string, string>()}
                                }
                            }
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver][(getComb.SelectedItem as ComboBoxItem).Content.ToString()].ContainsKey(saveResult.One))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver][(getComb.SelectedItem as ComboBoxItem).Content.ToString()].Add(saveResult.One, new Dictionary<string, Dictionary<string, string>>
                        {
                            { saveResult.Two,new Dictionary<string, string>()}
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver][(getComb.SelectedItem as ComboBoxItem).Content.ToString()][saveResult.One].ContainsKey(saveResult.Two))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver][(getComb.SelectedItem as ComboBoxItem).Content.ToString()][saveResult.One].Add(saveResult.Two, new Dictionary<string, string>
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
                            [(getComb.SelectedItem as ComboBoxItem).Content.ToString()]
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
                        //    [getComb.SelectedItem as ComboBoxItem).Content.ToString()]
                        //    [saveResult.One]
                        //    [saveResult.Two].Add($"第 {num + 1} 项", newChirldText);

                        var treeBase = new TreeViewBase();

                        await treeBase.AddItemToSaves(father.Saver, saveResult.One, saveResult.Two, 
                            $"第 {num + 1} 项", (getComb.SelectedItem as ComboBoxItem).Content.ToString(), newChirldText, true, 
                            contisionLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);

                        //await treeBase.AddItemToTreeView(thumbInfoWindow.FindName("TreeView_Tv") as TreeView, saveResult.One, saveResult.Two, $"第 {num + 1} 项",true);

                        await treeBase.AddItemToComBox(father.Saver, (getComb.SelectedItem as ComboBoxItem).Content.ToString(), saveResult.One, saveResult.Two, $"第 {num + 1} 项");

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
                     
                    fg = (getComb.SelectedItem as ComboBoxItem).Content.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    realCmd = string.Empty;

                    if (fg.Length > 1)
                    {
                        realCmd = fg[fg.Length-1];
                    }
                    else
                    {
                        realCmd = (getComb.SelectedItem as ComboBoxItem).Content.ToString();
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

                    saveResult = null;

                    while (saveResult == null)
                    {
                        var setWindow = new ThumbSetWindow();

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
                    }

                    

                    if (!mainWindowModels.SaveThumbInfo.ContainsKey(father.Saver))
                    {
                        mainWindowModels.SaveThumbInfo.Add(father.Saver, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>()
                        {
                            { (getComb.SelectedItem as ComboBoxItem).Content.ToString(),new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                                {
                                    { saveResult.One,new Dictionary<string, Dictionary<string, string>>()
                                    {
                                        { saveResult.Two,new Dictionary<string, string>()}
                                    } }
                                }
                            }
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver].ContainsKey((getComb.SelectedItem as ComboBoxItem).Content.ToString()))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver].Add((getComb.SelectedItem as ComboBoxItem).Content.ToString(), new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                        {
                            { saveResult.One,new Dictionary<string, Dictionary<string, string>>()
                                {
                                    { saveResult.Two,new Dictionary<string, string>()}
                                }
                            }
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver][(getComb.SelectedItem as ComboBoxItem).Content.ToString()].ContainsKey(saveResult.One))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver][(getComb.SelectedItem as ComboBoxItem).Content.ToString()].Add(saveResult.One, new Dictionary<string, Dictionary<string, string>>
                        {
                            { saveResult.Two,new Dictionary<string, string>()}
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver][(getComb.SelectedItem as ComboBoxItem).Content.ToString()][saveResult.One].ContainsKey(saveResult.Two))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver][(getComb.SelectedItem as ComboBoxItem).Content.ToString()][saveResult.One].Add(saveResult.Two, new Dictionary<string, string>
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
                            [(getComb.SelectedItem as ComboBoxItem).Content.ToString()]
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
                        //    [getComb.SelectedItem as ComboBoxItem).Content.ToString()]
                        //    [saveResult.One]
                        //    [saveResult.Two].Add($"第 {num + 1} 项", newChirldText);

                        var treeBase = new TreeViewBase();

                        await treeBase.AddItemToSaves(father.Saver, saveResult.One, saveResult.Two,
                            $"第 {num + 1} 项", (getComb.SelectedItem as ComboBoxItem).Content.ToString(), newChirldText, true,
                            eventLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);

                        //await treeBase.AddItemToTreeView(thumbInfoWindow.FindName("TreeView_Tv") as TreeView, saveResult.One, saveResult.Two, $"第 {num + 1} 项", true);

                        await treeBase.AddItemToComBox(father.Saver, (getComb.SelectedItem as ComboBoxItem).Content.ToString(), saveResult.One, saveResult.Two, $"第 {num + 1} 项");

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

                    fg = (getComb.SelectedItem as ComboBoxItem).Content.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    realCmd = string.Empty;

                    if (fg.Length > 1)
                    {
                        realCmd = fg[fg.Length - 1];
                    }
                    else
                    {
                        realCmd = (getComb.SelectedItem as ComboBoxItem).Content.ToString();
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

                    while (saveResult == null)
                    {
                        var setWindow = new ThumbSetWindow();

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
                    }

                    

                    if (!mainWindowModels.SaveThumbInfo.ContainsKey(father.Saver))
                    {
                        mainWindowModels.SaveThumbInfo.Add(father.Saver, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>()
                        {
                            { (getComb.SelectedItem as ComboBoxItem).Content.ToString(),new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                                {
                                    { saveResult.One,new Dictionary<string, Dictionary<string, string>>()
                                    {
                                        { saveResult.Two,new Dictionary<string, string>()}
                                    } }
                                }
                            }
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver].ContainsKey((getComb.SelectedItem as ComboBoxItem).Content.ToString()))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver].Add((getComb.SelectedItem as ComboBoxItem).Content.ToString(), new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                        {
                            { saveResult.One,new Dictionary<string, Dictionary<string, string>>()
                                {
                                    { saveResult.Two,new Dictionary<string, string>()}
                                } 
                            }
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver][(getComb.SelectedItem as ComboBoxItem).Content.ToString()].ContainsKey(saveResult.One))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver][(getComb.SelectedItem as ComboBoxItem).Content.ToString()].Add(saveResult.One, new Dictionary<string, Dictionary<string, string>>
                        {
                            { saveResult.Two,new Dictionary<string, string>()}
                        });
                    }

                    if (!mainWindowModels.SaveThumbInfo[father.Saver][(getComb.SelectedItem as ComboBoxItem).Content.ToString()][saveResult.One].ContainsKey(saveResult.Two))
                    {
                        mainWindowModels.SaveThumbInfo[father.Saver][(getComb.SelectedItem as ComboBoxItem).Content.ToString()][saveResult.One].Add(saveResult.Two, new Dictionary<string, string>
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
                            [(getComb.SelectedItem as ComboBoxItem).Content.ToString()]
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
                        //    [getComb.SelectedItem as ComboBoxItem).Content.ToString()]
                        //    [saveResult.One]
                        //    [saveResult.Two].Add($"第 {num + 1} 项", newChirldText);

                        var treeBase = new TreeViewBase();

                        await treeBase.AddItemToSaves(father.Saver, saveResult.One, saveResult.Two,
                            $"第 {num + 1} 项", (getComb.SelectedItem as ComboBoxItem).Content.ToString(), newChirldText, true,
                            objectiveLoader.saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo);

                        //await treeBase.AddItemToTreeView(thumbInfoWindow.FindName("TreeView_Tv") as TreeView, saveResult.One, saveResult.Two, $"第 {num + 1} 项", true);

                        await treeBase.AddItemToComBox(father.Saver, (getComb.SelectedItem as ComboBoxItem).Content.ToString(), saveResult.One, saveResult.Two, $"第 {num + 1} 项");

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
                save = MainWindowModels.saveThumbs.Where(t => t.Saver == thumb).FirstOrDefault();
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
                
                foreach (var item in saveThumbs)
                {
                    var thumbItem = item.Saver;
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
        private void DrawAllThumpLine(Thumb thumb)
        {
            Task.Run(() =>
            {
                
                var clines = saveLines.Where(t => t.ChirldName == thumb).ToList();

                var flines = saveLines.Where(t => t.FatherName == thumb).ToList();
                
                if (clines.Count <= 0 && flines.Count<=0)
                {
                    return;
                }
                for (int i = 0; i < clines.Count; i++)
                {
                    mainWindow.cvmenu.Dispatcher.Invoke(new Action(() =>
                    {
                        foreach (var item in mainWindow.cvmenu.Children)
                        {
                            var item_Line = item as Line;
                            if (item_Line == clines[i].line)
                            {
                                item_Line.X2 = Canvas.GetLeft(thumb) + thumb.Width / 2 - (Canvas.GetLeft(clines[i].FatherName) + clines[i].FatherName.Width / 2);
                                item_Line.Y2 = Canvas.GetTop(thumb) + thumb.Height / 2 - (Canvas.GetTop(clines[i].FatherName) + clines[i].FatherName.Height / 2);

                                Canvas.SetLeft(item_Line, Canvas.GetLeft(clines[i].FatherName) + clines[i].FatherName.Width / 2);
                                Canvas.SetTop(item_Line, Canvas.GetTop(clines[i].FatherName) + clines[i].FatherName.Height / 2);
                                System.Windows.Controls.Panel.SetZIndex(item_Line, 0);
                            }
                        }
                    }));
                }


                for (int i = 0; i < flines.Count; i++)
                {
                    mainWindow.cvmenu.Dispatcher.Invoke(new Action(() =>
                    {
                        foreach (var item in mainWindow.cvmenu.Children)
                        {
                            var item_Line = item as Line;
                            if (item_Line == flines[i].line)
                            {
                                item_Line.X2 = Canvas.GetLeft(flines[i].ChirldName) + flines[i].ChirldName.Width / 2 - (Canvas.GetLeft(thumb) + thumb.Width / 2);
                                item_Line.Y2 = Canvas.GetTop(flines[i].ChirldName) + flines[i].ChirldName.Height / 2 - (Canvas.GetTop(thumb) + thumb.Height / 2);

                                Canvas.SetLeft(item_Line, Canvas.GetLeft(thumb) + thumb.Width / 2);
                                Canvas.SetTop(item_Line, Canvas.GetTop(thumb) + thumb.Height / 2);
                                System.Windows.Controls.Panel.SetZIndex(item_Line, 0);
                            }
                        }
                    }));
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
                    Y = Canvas.GetTop(thumb_2) + thumb_2.Height/2,
                }, new Point()
                {
                    X = Canvas.GetLeft(thumb_1) + thumb_1.Width / 2,
                    Y = Canvas.GetTop(thumb_1)+thumb_2.Height / 2,
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
            
            line.Stroke = Brushes.Indigo;
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
