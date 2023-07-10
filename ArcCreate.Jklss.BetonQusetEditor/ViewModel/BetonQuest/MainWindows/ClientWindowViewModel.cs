using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.BetonQusetEditor.Base.ClientBase;
using ArcCreate.Jklss.BetonQusetEditor.Base.FileLoader;
using ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest;
using ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest.Data;
using ArcCreate.Jklss.BetonQusetEditor.ViewModel.Data;
using ArcCreate.Jklss.BetonQusetEditor.Windows;
using ArcCreate.Jklss.BetonQusetEditor.Windows.Data;
using ArcCreate.Jklss.BetonQusetEditor.Windows.Market;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.Data;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.SocketModel;
using ArcCreate.Jklss.Model.ThumbInfoWindow;
using ArcCreate.Jklss.Model.ThumbModel;
using ArcCreate.Jklss.Model.ThumbModel.CommandModel;
using ArcCreate.Jklss.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using static ArcCreate.Jklss.BetonQusetEditor.ViewModel.MainWindows.ClientWindowViewModel;
using Button = System.Windows.Controls.Button;
using ComboBox = System.Windows.Controls.ComboBox;
using HelpToolModel = ArcCreate.Jklss.BetonQusetEditor.Base.ClientBase.HelpToolModel;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using SaveChilds = ArcCreate.Jklss.BetonQusetEditor.Base.ClientBase.SaveChilds;
using Thumb = System.Windows.Controls.Primitives.Thumb;
using ThumbCoordinateModel = ArcCreate.Jklss.BetonQusetEditor.Base.ClientBase.ThumbCoordinateModel;
using ThumbInfoModel = ArcCreate.Jklss.BetonQusetEditor.Base.ClientBase.ThumbInfoModel;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.MainWindows
{
    public partial class ClientWindowViewModel : ObservableObject
    {
        #region 构造函数
        /// <summary>
        /// 禁止零参数构造函数，使用即报错
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public ClientWindowViewModel()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 基础构造方法用于正常打开该窗体
        /// </summary>
        /// <param name="mainWindow"></param>
        public ClientWindowViewModel(MainWindow mainWindow)
        {
            this.window = mainWindow;

            TranslateXProp = -1277400 / 2;

            TranslateYProp = -509800 / 2;

            this.CardItems.Add(bordCardViewModel);

            GetAllCardDel = new _GetAllCardDel(GetAllCard);

            ShowMessageDel = new _ShowMessageDel(ShowMessage);

            GetSelecteCardDel = new _GetSelecteCardDel(GetSelectCard);

            SetSelectCardInfoDel = new _SetSelectCardInfoDel(SetSelectCardInfo);

            GetClienteViewModelDel = new _GetClienteViewModelDel(GetClienteViewModel);

            ThumbInfoWindow window = new ThumbInfoWindow();

            thumbInfoWindow = window;

            thumbInfoWindow.DataContext = new ThumbInfoWindowViewModel();

            thumbInfoWindow.Left = window.ActualWidth + window.Left;

            thumbInfoWindow.Top = window.Top;

            thumbInfoWindow.Height = window.Height;

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

            Task.Run(() =>
            {
                LoadWindow();
            });
        }

        #endregion

        #region 字段与属性

        public ReturnModel returnModel = new ReturnModel(); 

        public CardViewModel selectCardInfo = null;

        private UserActivityBase ActBase = new UserActivityBase();//按键

        private List<CardViewModel> selectCard = new List<CardViewModel>();

        public GridData SelectData = null;

        public static List<ContisionsCmdModel> contisionProp = new List<ContisionsCmdModel>();//Contitions语法构造器模型

        public static List<EventCmdModel> eventProp = new List<EventCmdModel>();

        public static List<ObjectiveCmdModel> objectiveProp = new List<ObjectiveCmdModel>();

        public static ThumbInfoWindow thumbInfoWindow;

        private BordCardViewModel bordCardViewModel = new BordCardViewModel() { CvZIndex = 2,IsDraw=true};

        public MainWindow window = null;

        [ObservableProperty]
        private bool _IsProtectName = false;

        [ObservableProperty]
        private string _PageName = string.Empty;

        [ObservableProperty]
        private string _MainFilePath = string.Empty;

        [ObservableProperty]
        private string _LoadingMessage = string.Empty;

        [ObservableProperty]
        private Visibility _LoadingShow = Visibility.Hidden;

        [ObservableProperty]
        private string _SearchText = string.Empty;

        [ObservableProperty]
        private ThumbClass _SearchType = ThumbClass.Subject;

        [ObservableProperty]
        private List<ThumbClass> _SearchListType = new List<ThumbClass>();

        [ObservableProperty]
        private bool _IsFindFile = false;

        [ObservableProperty]
        private bool _isHaveSubjcet = false;

        /// <summary>
        /// 下方消息通知
        /// </summary>
        [ObservableProperty]
        private string _Message = string.Empty;

        [ObservableProperty]
        private string _ThumbNums = string.Empty;

        [ObservableProperty]
        private string _ThumbLineNums = string.Empty;

        [ObservableProperty]
        public ObservableCollection<CardViewModel> _CardItems = new ObservableCollection<CardViewModel>();

        [ObservableProperty]
        private double _TranslateXProp = 0.00;

        [ObservableProperty]
        private double _TranslateYProp = 0.00;

        [ObservableProperty]
        private double _RotateXProp = 0.00;

        [ObservableProperty]
        private double _RotateYProp = 0.00;

        [ObservableProperty]
        private double _ScaleTransform_ScaleXProp = 1.00;

        [ObservableProperty]
        private double _ScaleTransform_ScaleYProp = 1.00;

        [ObservableProperty]
        private double _ScaleTransform_CenterYProp = 0.00;

        [ObservableProperty]
        private double _ScaleTransform_CenterXProp = 0.00;

        private double Scale = 1;

        private Point previousPoint;

        private bool isTranslateStart = false;

        private bool isCanMove = false;//鼠标是否移动

        private Point tempStartPoint;//起始坐标

        #endregion

        #region 委托与事件定义

        public delegate ObservableCollection<CardViewModel> _GetAllCardDel();

        public static _GetAllCardDel GetAllCardDel;



        public delegate void _ShowMessageDel(string txt);

        public static _ShowMessageDel ShowMessageDel;



        public delegate List<CardViewModel> _GetSelecteCardDel();

        public static _GetSelecteCardDel GetSelecteCardDel;



        public delegate void _SetSelectCardInfoDel(CardViewModel card);

        public static _SetSelectCardInfoDel SetSelectCardInfoDel;



        public delegate ClientWindowViewModel _GetClienteViewModelDel();
        public static _GetClienteViewModelDel GetClienteViewModelDel;

        #endregion

        #region 操作层方法

        [RelayCommand()]
        private void ProtectNameClick(System.Windows.Controls.CheckBox checkBox)
        {
            foreach (var item in CardItems)
            {
                item.IsProtectName = IsProtectName;
            }
        }

        /// <summary>
        /// 创建新的对话主体
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        [RelayCommand()]
        private async Task CreateNewTalk(Window window)
        {
            await CreateCard<SubjectCardViewModel>(ThumbClass.Subject);
        }

        /// <summary>
        /// 创建新的NPC对话
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        [RelayCommand()]
        private async Task CreateNpcTalk(Window window)
        {
            await CreateCard<AnyCardViewModel>(ThumbClass.NPC);
        }

        /// <summary>
        /// 创建新的玩家对话
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        [RelayCommand()]
        private async Task CreatePlayerTalk(Window window)
        {
            await CreateCard<AnyCardViewModel>(ThumbClass.Player);
        }

        /// <summary>
        /// 创建新的条件
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        [RelayCommand()]
        private async Task CreateConditions(Window window)
        {
            await CreateCard<AnyCardViewModel>(ThumbClass.Conditions);
        }

        /// <summary>
        /// 创建新的事件
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        [RelayCommand()]
        private async Task CreateEvents(Window window)
        {
            await CreateCard<AnyCardViewModel>(ThumbClass.Events);
        }

        /// <summary>
        /// 创建新的目标
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        [RelayCommand()]
        private async Task CreateObjectives(Window window)
        {
            await CreateCard<AnyCardViewModel>(ThumbClass.Objectives);
        }

        /// <summary>
        /// 创建新的任务笔记
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        [RelayCommand()]
        private async Task CreateJournal(Window window)
        {
            await CreateCard<CardViewModel>(ThumbClass.Journal);
        }

        /// <summary>
        /// 创建新的物品
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        [RelayCommand()]
        private async Task CreateItems(Window window)
        {
            await CreateCard<CardViewModel>(ThumbClass.Items);
        }

        [RelayCommand()]
        private async Task CvMenuLoaded(Canvas canvas)
        {
            canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
        }

        [RelayCommand()]
        private async Task BordOutSideLoaded(Border outside)
        {
            outside.PreviewMouseWheel += Outside_PreviewMouseWheel;
            outside.PreviewMouseDown += Outside_PreviewMouseDown;
            outside.PreviewMouseUp += Outside_PreviewMouseUp;
        }

        [RelayCommand()]
        private async Task SaveJson(Window window)
        {
            window.IsEnabled = false;
            LoadingShow = Visibility.Visible;

            LoadingMessage = "正在处理~";

            if (string.IsNullOrEmpty(MainFilePath))
            {
                window.IsEnabled = true;
                LoadingShow = Visibility.Hidden;
                ShowMessage("main入口文件不存在请重新指定Main.yml文件");

                return;
            }

            if (!FileService.IsHaveFile(MainFilePath))
            {
                window.IsEnabled = true;
                LoadingShow = Visibility.Hidden;
                ShowMessage("main入口文件不存在请重新指定Main.yml文件");

                return;
            }

            var saveBase = new ConfigReaderAndWriter(ConditionsCardChanged.saveAllValue,EventCardChanged.saveAllValue,ObjectiveCardChanged.saveAllValue,ConversationCardChanged.saveAllValue, CardItems);

            var back = await saveBase.SaveToJson(MainFilePath,SelectData.Name, SelectData.Code);

            if (!back.Succese)
            {
                window.IsEnabled = true;
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
                window.IsEnabled = true;
                LoadingShow = Visibility.Hidden;
                ShowMessage("保存失败，请尝试重新保存");
                return;
            }

            var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

            if (getModel.Token != SocketModel.token)
            {
                window.IsEnabled = true;
                LoadingShow = Visibility.Hidden;
                ShowMessage("您的Token异常请重新登录后保存");
                return;
            }

            var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

            if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.SaveToJson || !getRealMessage.IsLogin)
            {
                window.IsEnabled = true;
                LoadingShow = Visibility.Hidden;
                ShowMessage("服务器异常");
                return;
            }

            if (this.SelectData.Code == -1)
            {
                this.SelectData.Code = Convert.ToInt32(getRealMessage.Other);
            }
            window.IsEnabled = true;
            LoadingShow = Visibility.Hidden;
            ShowMessage("配置保存至云端成功");

            return;
        }

        [RelayCommand()]
        private async Task Close(Window window)
        {
            Environment.Exit(0);
        }

        [RelayCommand()]
        private async Task Narrow(Window window)
        {
            window.WindowState = WindowState.Minimized;
        }

        [RelayCommand()]
        private async Task Minimize(Window window)
        {
            if (window.WindowState == WindowState.Maximized)
            {
                window.WindowState = WindowState.Normal;
            }
            else if (window.WindowState == WindowState.Normal)
            {
                window.WindowState = WindowState.Maximized;
            }
        }

        [RelayCommand()]
        private async Task OpenMarket(Window window)
        {
            MarketWindow mWindow = new MarketWindow();

            mWindow.ShowDialog();
        }

        [RelayCommand()]
        private async Task LocationChanged(Window window)
        {
            thumbInfoWindow.Left = window.ActualWidth + window.Left;
            thumbInfoWindow.Top = window.Top;
            thumbInfoWindow.Height = window.Height;
        }

        [RelayCommand()]
        private async Task SaveYaml(Window window)
        {
            window.IsEnabled = false;
            LoadingShow = Visibility.Visible;

            LoadingMessage = "正在处理~";
            if (!FileService.IsHaveFile(MainFilePath))
            {
                window.IsEnabled = true;
                LoadingShow = Visibility.Hidden;
                ShowMessage("main入口文件不存在请重新指定Main.yml文件");

                return;
            }

            var saveBase = new ConfigReaderAndWriter(ConditionsCardChanged.saveAllValue, EventCardChanged.saveAllValue, ObjectiveCardChanged.saveAllValue, ConversationCardChanged.saveAllValue, CardItems);

            var back = await saveBase.SaveToJson(MainFilePath,SelectData.Name, SelectData.Code, true);

            if (!back.Succese)
            {
                window.IsEnabled = true;
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
                window.IsEnabled = true;
                LoadingShow = Visibility.Hidden;
                ShowMessage("保存失败，请尝试重新保存");
                return;
            }

            var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

            if (getModel.Token != SocketModel.token)
            {
                window.IsEnabled = true;
                LoadingShow = Visibility.Hidden;
                ShowMessage("您的Token异常请重新登录后保存");
                return;
            }

            var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

            if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.SaveToYaml || !getRealMessage.IsLogin)
            {
                if (getRealMessage != null)
                {
                    window.IsEnabled = true;
                    LoadingShow = Visibility.Hidden;
                    ShowMessage(getRealMessage.Message);
                    return;
                }
                else
                {
                    window.IsEnabled = true;
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

            var result = await saveBase.InputToYaml(FileService.JsonToProp<YamlSaver>(getRealMessage.Message),MainFilePath);
            window.IsEnabled = true;
            LoadingShow = Visibility.Hidden;
            ShowMessage(result.Text);

            return;
        }

        [RelayCommand()]
        private async Task UpDateDataToMarket(Window window)
        {

        }

        [RelayCommand()]
        private async Task SelectMainFile(Border border)
        {
            var getBacks = await SelectFilePath();

            if (!getBacks.Succese)
            {
                return;
            }

            MainFilePath = getBacks.Text;
        }

        [RelayCommand()]
        private async Task OpenInfoWindow(Window window)
        {
            if (thumbInfoWindow.Visibility == Visibility.Hidden)
            {
                thumbInfoWindow.Visibility = Visibility.Visible;
            }
            else
            {
                thumbInfoWindow.Visibility = Visibility.Hidden;
            }
        }

        [RelayCommand()]
        private async Task SelectSearch(Window window)
        {
            if (!CardItems.Where(t => t.Type == SearchType && t.ConfigName.Contains(SearchText)).Any())
            {
                ShowMessage("未找到该卡片");
                return;
            }

            var getCard = CardItems.Where(t=>t.Type == SearchType&&t.ConfigName.Contains(SearchText)).FirstOrDefault();

            var y = -getCard.CvTop + ((window as MainWindow).outsaid.ActualHeight / 2) - ((double)getCard.ThumbHeight / 2);
            var x = -getCard.CvLeft + ((window as MainWindow).outsaid.ActualWidth / 2) - ((double)getCard.ThumbWidth / 2);

            TranslateXProp = x;
            TranslateYProp = y;
        }

        [RelayCommand()]
        private void PointBuy(Window window)
        {
            window.IsEnabled = false;

            PayWindow payWindow = new PayWindow();

            payWindow.ShowDialog();

            window.IsEnabled = true;
        }

        #endregion

        #region 具体方法

        #region 事件

        #region 有关窗体移动的事件

        private void Outside_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var outside = (Border)sender;

            if (e.MiddleButton == MouseButtonState.Released && e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
            {
                if (isTranslateStart)
                {
                    isTranslateStart = false;
                }

                outside.PreviewMouseMove -= Outside_PreviewMouseMove;
            }
        }

        private void Outside_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var outside = (Border)sender;

            if (e.MiddleButton == MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
            {
                previousPoint = e.GetPosition(outside);
                isTranslateStart = true;

                outside.PreviewMouseMove += Outside_PreviewMouseMove;
            }

        }

        private void Outside_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var outside = (Border)sender;

            if (e.MiddleButton == MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
            {
                if (isTranslateStart)
                {
                    Point currentPoint = e.GetPosition(outside); //不能用 inside，必须用outside
                    Vector v = currentPoint - previousPoint;

                    this.TranslateXProp += v.X / Scale;
                    this.TranslateYProp += v.Y / Scale;

                    previousPoint = currentPoint;
                }

            }

        }

        private void Outside_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var outside = (Border)sender;

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Point currentPoint = e.GetPosition(outside); //不能用 inside，必须用outside

                double s = 0;
                if (e.Delta > 0)
                {
                    s = 0.05;
                }
                else
                {
                    s = -0.05;
                }

                var sx = this.ScaleTransform_ScaleXProp + s;

                if (sx <= 3 && sx >= 0.05)
                {
                    Scale = sx;
                    this.ScaleTransform_ScaleXProp += s;

                    this.ScaleTransform_ScaleYProp += s;
                    this.ScaleTransform_CenterXProp = currentPoint.X;
                    this.ScaleTransform_CenterYProp = currentPoint.Y;
                }
                e.Handled = true;

            }
        }


        #endregion

        #region Canvas事件

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var cvmenu = (Canvas)sender;

            selectCard.Clear();

            Point tempEndPoint = e.GetPosition(cvmenu);

            var getStart = tempStartPoint;

            foreach (var item in CardItems.Where(t=>!t.IsDraw&&!t.IsLine).ToList())
            {
                var num = false;

                var getCoor = GetCardPoint(item);

                for (int i = 0; i < getCoor.Count; i++)
                {
                    if (getCoor[i].X > getStart.X && getCoor[i].Y > getStart.Y && getCoor[i].X < tempEndPoint.X && getCoor[i].Y < tempEndPoint.Y)
                    {
                        num = true;
                    }
                    if (getCoor[i].X < getStart.X && getCoor[i].Y < getStart.Y && getCoor[i].X > tempEndPoint.X && getCoor[i].Y > tempEndPoint.Y)
                    {
                        num = true;
                    }
                }

                if(num)
                {
                    item.ShadowColor = Brushes.DarkOrange.Color;
                    selectCard.Add(item);
                }
            }

            bordCardViewModel.Bord_Visibility = Visibility.Hidden;
            bordCardViewModel.Bord_Height = 0;
            bordCardViewModel.Bord_Width = 0;
            isCanMove = !isCanMove;
            cvmenu.MouseMove -= Canvas_MouseMove;

        }

        private List<Point> GetCardPoint(CardViewModel nowCard)
        {
            var points = new List<Point>();

            points.Add(new Point(nowCard.CvLeft, nowCard.CvTop));

            points.Add(new Point(nowCard.CvLeft + (double)nowCard.ThumbWidth, nowCard.CvTop));

            points.Add(new Point(nowCard.CvLeft, nowCard.CvTop + (double)nowCard.ThumbHeight));

            points.Add(new Point(nowCard.CvLeft + (double)nowCard.ThumbWidth, nowCard.CvTop + (double)nowCard.ThumbHeight));

            return points;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cvmenu = (Canvas)sender;

            if (!isCanMove)
            {
                isCanMove = true;
                tempStartPoint = e.GetPosition(cvmenu);
                bordCardViewModel.Bord_Visibility = Visibility.Visible;
                cvmenu.MouseMove += Canvas_MouseMove;
            }

        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var cvmenu = (Canvas)sender;

            if (isCanMove)
            {
                Point tempEndPoint = e.GetPosition(cvmenu);
                //绘制跟随鼠标移动的方框
                DrawMultiselectBorder(tempEndPoint, tempStartPoint);
            }
        }

        /// <summary>
        /// 绘制跟随鼠标移动的方框
        /// </summary>
        private void DrawMultiselectBorder(Point endPoint, Point startPoint)
        {
            bordCardViewModel.Bord_Width = Math.Abs(endPoint.X - startPoint.X);
            bordCardViewModel.Bord_Height = Math.Abs(endPoint.Y - startPoint.Y);
            if (endPoint.X - startPoint.X >= 0)
            {
                bordCardViewModel.CvLeft = startPoint.X;

            }
            else
            {
                bordCardViewModel.CvLeft = endPoint.X;

            }

            if (endPoint.Y - startPoint.Y >= 0)
            {
                bordCardViewModel.CvTop = startPoint.Y;

            }
            else
            {
                bordCardViewModel.CvTop = endPoint.Y;
            }

        }

        #endregion

        #endregion

        private static async Task<ReturnModel> SelectFilePath()
        {
            var result = new ReturnModel();

            var path = string.Empty;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择Main.yml";
            dialog.Filter = "入口文件|main.yml";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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

        /// <summary>
        /// 用于自定义的扣费
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public async Task<ReturnModel> PayPoint(int point)
        {
            var result = new ReturnModel();

            var message = new MessageModel()
            {
                IsLogin = SocketModel.isLogin,
                JsonInfo = JsonInfo.UsePath,
                UserName = SocketModel.userName,
                Message = point.ToString(),
                Path = "PayPoints"
            };

            var jsonMessage = FileService.SaveToJson(message);

            var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);

            if (getMessage == null || !getMessage.Succese)
            {
                result.SetError("请求失败");

                return result;
            }

            var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

            if (getModel.Token != SocketModel.token)
            {
                result.SetError("请求失败");

                return result;
            }

            var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

            if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.UsePath || getRealMessage.Path != "PayPoints" || !getRealMessage.IsLogin)
            {
                result.SetError("请求失败");

                return result;
            }
            result.SetSuccese(getRealMessage.Message);

            return result;
        }

        /// <summary>
        /// 发放底部消息
        /// </summary>
        /// <param name="txt"></param>
        public async void ShowMessage(string txt)
        {
            await Task.Run(() =>
            {
                window.Dispatcher.Invoke(new Action(() =>
                {
                    window.MessageBar.Visibility = Visibility.Visible;
                    Message = txt;
                    AnimationBase.Appear(window.MessageBar);
                }));

                Thread.Sleep(3000);


                window.Dispatcher.Invoke(new Action(() =>
                {
                    AnimationBase.Disappear(window.MessageBar);
                }));
            });
        }

        /// <summary>
        /// 用于委托获取所有卡片信息
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<CardViewModel> GetAllCard()
        {
            return CardItems;
        }

        public List<CardViewModel> GetSelectCard()
        {
            return selectCard;
        }

        private async void LoadWindow()
        {
            window.Dispatcher.Invoke(new Action(() =>
            {
                window.IsEnabled = false;
            }));
            

            LoadingShow = Visibility.Visible;

            LoadingMessage = "正在加载条件模型~";

            var contisionLoader = new ContisionLoaderBase();

            var conJson = contisionLoader.Saver().Result;

            contisionLoader.jsons = conJson;

            if (string.IsNullOrEmpty(conJson))
            {
                return;
            }

            contisionProp = await contisionLoader.Loader();

            LoadingMessage = "正在加载事件模型~";

            var eventLoader = new EventLoaderBase();

            var eventJson = await eventLoader.Saver();

            eventLoader.jsons = eventJson;

            if (string.IsNullOrEmpty(eventJson))
            {
                return;
            }

            eventProp = await eventLoader.Loader();

            LoadingMessage = "正在加载目标模型~";

            var objectiveLoader = new ObjectiveLoaderBase();

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

            window.Dispatcher.Invoke(new Action(async () =>
            {
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
                            CardItems.Clear();
                            continue;
                        }
                        else
                        {
                            PageName = SelectData.Name;
                            break;
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
                                    CardItems.Clear();
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

                ActBase.KeyDown += ActBase_KeyDown;

                window.IsEnabled = true;
                LoadingShow = Visibility.Hidden;
            }));
        }

        /// <summary>
        /// 随机字符的生成
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        private string GetRandomString(int len)
        {
            string s = "123456789abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ";
            string reValue = string.Empty;
            Random rd = new Random();
            while (reValue.Length < len)
            {
                string s1 = s[rd.Next(0, s.Length)].ToString();
                if (reValue.IndexOf(s1) == -1)
                    reValue += s1;
            }
            return reValue;
        }

        /// <summary>
        /// 读取云端配置文件
        /// </summary>
        /// <returns></returns>
        private async Task<ReturnModel> ReadJson()
        {
            var result = new ReturnModel();

            window.IsEnabled = false;

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

            var saveHelpTool = new List<HelpToolModel>();

            var dic = new List<ThumbCoordinateModel>();

            var disPath = FileService.GetFileDirectory(MainFilePath);

            try
            {
                saveMainInfo = FileService.JsonToProp<List<ThumbsModels>>(getData.main);

                saveAllChildInfo = FileService.JsonToProp<List<SaveChilds>>(getData.maininfo);

                saveNPCEOInfo = FileService.JsonToProp<List<ThumbInfoModel>>(getData.data);

                saveJournalInfo = FileService.JsonToProp<List<ThumbsModels>>(getData.journaldata);

                saveItemsInfo = FileService.JsonToProp<List<ThumbsModels>>(getData.itemsdata);

                saveHelpTool = FileService.JsonToProp<List<HelpToolModel>>(getData.helptooldata);

                dic = FileService.JsonToProp<List<ThumbCoordinateModel>>(getData.coordinate);
            }
            catch
            {
                ShowMessage("数据文件损坏！请不要擅自修改数据文件！如果您没有备份数据文件那么您将失去数据！");
                result.SetError();
                return result;
            }

            if (saveMainInfo == null || saveAllChildInfo == null || dic == null)
            {
                ShowMessage("未找到配置文件");
                result.SetError();
                return result;
            }

            #region 卡片创建

            foreach (var item in saveMainInfo)
            {
                var getCoor = dic.Where(t => t.thumbClass == ThumbClass.Subject && t.Name == item.Config).FirstOrDefault();

                var cardView = await CreateCard<SubjectCardViewModel>(ThumbClass.Subject, false, item.Config, getCoor.X, getCoor.Y);

                cardView.NPC_ID = item.Type;

                cardView.ItemContent = item.Text;

                ThumbNums = CardItems.Count.ToString();
            }

            foreach (var item in saveNPCEOInfo)
            {
                var getCoor = dic.Where(t => t.thumbClass == item.thumbClass && t.Name == item.Name).FirstOrDefault();
                if (item.thumbClass == ThumbClass.Player || item.thumbClass == ThumbClass.NPC)
                {
                    if (item.thumbClass == ThumbClass.NPC)
                    {
                        var cardView = await CreateCard<AnyCardViewModel>(ThumbClass.NPC, false, item.Name, getCoor.X, getCoor.Y);

                        var getSaveInfo = ConversationCardChanged.saveAllValue[cardView];

                        foreach (var i in item.data)
                        {
                            foreach (var j in i.Value)
                            {
                                foreach (var n in j.Value)
                                {
                                    foreach (var m in n.Value)
                                    {
                                        try
                                        {
                                            if (getSaveInfo[i.Key][j.Key][n.Key].ContainsKey(m.Key))
                                            {
                                                getSaveInfo[i.Key][j.Key][n.Key][m.Key] = m.Value;
                                            }
                                            else
                                            {
                                                getSaveInfo[i.Key][j.Key][n.Key].Add(m.Key, m.Value);
                                            }

                                        }
                                        catch
                                        {
                                            ShowMessage($"[{item.Name}]卡片数据添加失败");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (item.thumbClass == ThumbClass.Player)
                    {
                        var cardView = await CreateCard<AnyCardViewModel>(ThumbClass.Player, false, item.Name, getCoor.X, getCoor.Y);

                        var getSaveInfo = ConversationCardChanged.saveAllValue[cardView];

                        foreach (var i in item.data)
                        {
                            foreach (var j in i.Value)
                            {
                                foreach (var n in j.Value)
                                {
                                    foreach (var m in n.Value)
                                    {
                                        try
                                        {
                                            if (getSaveInfo[i.Key][j.Key][n.Key].ContainsKey(m.Key))
                                            {
                                                getSaveInfo[i.Key][j.Key][n.Key][m.Key] = m.Value;
                                            }
                                            else
                                            {
                                                getSaveInfo[i.Key][j.Key][n.Key].Add(m.Key, m.Value);
                                            }

                                        }
                                        catch
                                        {
                                            ShowMessage($"[{item.Name}]卡片数据添加失败");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    ThumbNums = CardItems.Count.ToString();
                }
                else
                {
                    switch (item.thumbClass)
                    {
                        case ThumbClass.Conditions:

                            var cardView = await CreateCard<AnyCardViewModel>(ThumbClass.Conditions, false, item.Name, getCoor.X, getCoor.Y);

                            if (item.data == null)
                            {
                                ShowMessage($"该卡片没有相关数据哦[{item.Name}]");

                                break;
                            }

                            var getCmd = item.data.Keys.First();

                            try
                            {
                                cardView.SelectType = getCmd;
                                await cardView.conditionsCardChanged.TypeChanged();
                            }
                            catch
                            {
                                ShowMessage($"您的语法模型中不存在相关模型[{item.Name}]");
                            }

                            if (!ConditionsCardChanged.saveAllValue.ContainsKey(cardView)|| ConditionsCardChanged.saveAllValue[cardView].Count<=0)
                            {
                                ShowMessage($"该卡片没有相关数据哦[{item.Name}]");

                                break;
                            }

                            var getSaveInfo = ConditionsCardChanged.saveAllValue[cardView];

                            foreach (var i in item.data)
                            {
                                foreach (var j in i.Value)
                                {
                                    foreach (var n in j.Value)
                                    {
                                        foreach (var m in n.Value)
                                        {
                                            try
                                            {
                                                if (getSaveInfo[i.Key][j.Key][n.Key].ContainsKey(m.Key))
                                                {
                                                    getSaveInfo[i.Key][j.Key][n.Key][m.Key] = m.Value;
                                                }
                                                else
                                                {
                                                    getSaveInfo[i.Key][j.Key][n.Key].Add(m.Key, m.Value);
                                                }
                                                
                                            }
                                            catch
                                            {
                                                ShowMessage($"[{item.Name}]卡片数据添加失败,您的语法模型与数据不匹配");
                                            }
                                        }
                                    }
                                }
                            }

                            ThumbNums = CardItems.Count.ToString();

                            break;

                        case ThumbClass.Events:

                            cardView = await CreateCard<AnyCardViewModel>(ThumbClass.Events, false, item.Name, getCoor.X, getCoor.Y);

                            if (item.data == null)
                            {
                                ShowMessage($"该卡片没有相关数据哦[{item.Name}]");

                                break;
                            }

                            getCmd = item.data.Keys.First();

                            try
                            {
                                cardView.SelectType = getCmd;
                                await cardView.eventCardChanged.TypeChanged();
                            }
                            catch
                            {
                                ShowMessage($"您的语法模型中不存在相关模型[{item.Name}]");
                            }

                            if (!EventCardChanged.saveAllValue.ContainsKey(cardView) || EventCardChanged.saveAllValue[cardView].Count <= 0)
                            {
                                ShowMessage($"该卡片没有相关数据哦[{item.Name}]");

                                break;
                            }

                            getSaveInfo = EventCardChanged.saveAllValue[cardView];

                            foreach (var i in item.data)
                            {
                                foreach (var j in i.Value)
                                {
                                    foreach (var n in j.Value)
                                    {
                                        foreach (var m in n.Value)
                                        {
                                            try
                                            {
                                                if (getSaveInfo[i.Key][j.Key][n.Key].ContainsKey(m.Key))
                                                {
                                                    getSaveInfo[i.Key][j.Key][n.Key][m.Key] = m.Value;
                                                }
                                                else
                                                {
                                                    getSaveInfo[i.Key][j.Key][n.Key].Add(m.Key, m.Value);
                                                }

                                            }
                                            catch
                                            {
                                                ShowMessage($"[{item.Name}]卡片数据添加失败,您的语法模型与数据不匹配");
                                            }
                                        }
                                    }
                                }
                            }

                            ThumbNums = CardItems.Count.ToString();

                            break;

                        case ThumbClass.Objectives:

                            cardView = await CreateCard<AnyCardViewModel>(ThumbClass.Objectives, false, item.Name, getCoor.X, getCoor.Y);

                            if (item.data == null)
                            {
                                ShowMessage($"该卡片没有相关数据哦[{item.Name}]");

                                break;
                            }

                            getCmd = item.data.Keys.First();

                            try
                            {
                                cardView.SelectType = getCmd;
                                await cardView.objectiveCardChanged.TypeChanged();
                            }
                            catch
                            {
                                ShowMessage($"您的语法模型中不存在相关模型[{item.Name}]");
                            }

                            if (!ObjectiveCardChanged.saveAllValue.ContainsKey(cardView) || ObjectiveCardChanged.saveAllValue[cardView].Count <= 0)
                            {
                                ShowMessage($"该卡片没有相关数据哦[{item.Name}]");

                                break;
                            }

                            getSaveInfo = ObjectiveCardChanged.saveAllValue[cardView];

                            foreach (var i in item.data)
                            {
                                foreach (var j in i.Value)
                                {
                                    foreach (var n in j.Value)
                                    {
                                        foreach (var m in n.Value)
                                        {
                                            try
                                            {
                                                if (getSaveInfo[i.Key][j.Key][n.Key].ContainsKey(m.Key))
                                                {
                                                    getSaveInfo[i.Key][j.Key][n.Key][m.Key] = m.Value;
                                                }
                                                else
                                                {
                                                    getSaveInfo[i.Key][j.Key][n.Key].Add(m.Key, m.Value);
                                                }

                                            }
                                            catch
                                            {
                                                ShowMessage($"[{item.Name}]卡片数据添加失败,您的语法模型与数据不匹配");
                                            }
                                        }
                                    }
                                }
                            }

                            ThumbNums = CardItems.Count.ToString();
                            break;
                    }
                }
            }

            foreach (var item in saveJournalInfo)
            {
                var getCoor = dic.Where(t => t.thumbClass == ThumbClass.Journal && t.Name == item.Config).FirstOrDefault();

                var cardView = await CreateCard<CardViewModel>(ThumbClass.Journal, false, item.Config, getCoor.X, getCoor.Y);

                cardView.ItemContent = item.Text;

                ThumbNums = CardItems.Count.ToString();
            }

            foreach (var item in saveItemsInfo)
            {
                var getCoor = dic.Where(t => t.thumbClass == ThumbClass.Items && t.Name == item.Config).FirstOrDefault();

                var cardView = await CreateCard<CardViewModel>(ThumbClass.Items, false, item.Config, getCoor.X, getCoor.Y);

                cardView.ItemContent = item.Text;

                ThumbNums = CardItems.Count.ToString();
            }
            #endregion

            #region 关系建立

            try
            {
                var getMainThumb = CardItems.Where(t => t.Type == ThumbClass.Subject && !t.IsDraw && !t.IsLine).ToList();

                foreach (var item in getMainThumb)
                {
                    var getMainInfo = saveAllChildInfo.Where(t=>t.Saver == item.ConfigName).FirstOrDefault();

                    if(getMainInfo == null)
                    {
                        ShowMessage($"未找到卡片[{item.ConfigName}]的信息");
                    }

                    foreach (var i in getMainInfo.Children)
                    {
                        var findChild = CardItems.Where(t => t.ConfigName == i&&t.Type == ThumbClass.NPC).FirstOrDefault();
                        findChild.MainCard = item;
                        if (findChild == null)
                        {
                            ShowMessage($"未找到卡片[{i}]的信息");
                        }

                        var newLine = new LineCardViewModel()
                        {
                            LineLeft = item,
                            LineRight = findChild,
                        };

                        CardItems.Add(newLine);

                        ChangeTheLine(newLine);

                        if (!item.Right.Contains(findChild))
                        {
                            item.Right.Add(findChild);
                        }

                        if (!findChild.Left.Contains(item))
                        {
                            findChild.Left.Add(item);
                        }
                    }

                    //foreach (var i in getMainInfo.Fathers)
                    //{
                    //    var findFather = CardItems.Where(t => t.ConfigName == i && t.Type == ThumbClass.Events).FirstOrDefault();

                    //    if (findFather == null)
                    //    {
                    //        ShowMessage($"未找到卡片[{i}]的信息");
                    //    }

                    //    var newLine = new LineCardViewModel()
                    //    {
                    //        LineLeft = findFather,
                    //        LineRight = item,
                    //    };

                    //    CardItems.Add(newLine);

                    //    ChangeTheLine(newLine);

                    //    if (!findFather.Right.Contains(item))
                    //    {
                    //        findFather.Right.Add(findChild);
                    //    }

                    //    if (!findChild.Left.Contains(item))
                    //    {
                    //        findChild.Left.Add(item);
                    //    }
                    //}
                }
            }
            catch
            {
                ShowMessage("未找到相关控件，请尝试重新读取！  44");
                result.SetError();
                return result;
            }

            var getAllThumb = CardItems.Where(t => t.Type != ThumbClass.Subject).ToList();

            foreach (var item in getAllThumb)
            {
                if (item.Type == ThumbClass.NPC || item.Type == ThumbClass.Player)
                {
                    if (!ConversationCardChanged.saveAllValue.ContainsKey(item as AnyCardViewModel))
                    {
                        continue;
                    }

                    foreach (var i in ConversationCardChanged.saveAllValue[item as AnyCardViewModel])
                    {
                        switch (GetRealCmd(i.Key))
                        {
                            case "conditions":
                                foreach (var j in i.Value["conditions"]["第 1 条参数"])
                                {
                                    var value = j.Value.TrimStart('!');

                                    if (CardItems.Where(t => t.ConfigName == value && t.Type == ThumbClass.Conditions).Any())
                                    {
                                        var getinfo = CardItems.Where(t => t.ConfigName == value && t.Type == ThumbClass.Conditions).FirstOrDefault();

                                        var classoverBack = await item.CardCanBeClassify(item,getinfo);

                                        if (classoverBack.Succese)
                                        {
                                            var newLine = new LineCardViewModel()
                                            {
                                                LineLeft = item,
                                                LineRight = getinfo,
                                            };

                                            CardItems.Add(newLine);

                                            ChangeTheLine(newLine);

                                            if (!item.Right.Contains(getinfo))
                                            {
                                                item.Right.Add(getinfo);
                                            }

                                            if (!getinfo.Left.Contains(item))
                                            {
                                                getinfo.Left.Add(item);
                                            }

                                            if (item.Type == ThumbClass.Subject)
                                            {
                                                item.GiveMainCard(getinfo, item);
                                            }
                                            else
                                            {
                                                if (item.MainCard != null)
                                                {
                                                    item.GiveMainCard(getinfo, item.MainCard);
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case "events":
                                foreach (var j in i.Value["events"]["第 1 条参数"])
                                {
                                    var value = j.Value;

                                    if (CardItems.Where(t => t.ConfigName == value && t.Type == ThumbClass.Events).Any())
                                    {
                                        var getinfo = CardItems.Where(t => t.ConfigName == value && t.Type == ThumbClass.Events).FirstOrDefault();

                                        var classoverBack = await item.CardCanBeClassify(item, getinfo);

                                        if (classoverBack.Succese)
                                        {
                                            var newLine = new LineCardViewModel()
                                            {
                                                LineLeft = item,
                                                LineRight = getinfo,
                                            };

                                            CardItems.Add(newLine);

                                            ChangeTheLine(newLine);

                                            if (!item.Right.Contains(getinfo))
                                            {
                                                item.Right.Add(getinfo);
                                            }

                                            if (!getinfo.Left.Contains(item))
                                            {
                                                getinfo.Left.Add(item);
                                            }

                                            if (item.Type == ThumbClass.Subject)
                                            {
                                                item.GiveMainCard(getinfo, item);
                                            }
                                            else
                                            {
                                                if (item.MainCard != null)
                                                {
                                                    item.GiveMainCard(getinfo, item.MainCard);
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case "pointer":
                                if (item.Type == ThumbClass.NPC)
                                {
                                    foreach (var j in i.Value["pointer"]["第 1 条参数"])
                                    {
                                        var value = j.Value;

                                        if (CardItems.Where(t => t.ConfigName == value && t.Type == ThumbClass.Player).Any())
                                        {
                                            var getinfo = CardItems.Where(t => t.ConfigName == value && t.Type == ThumbClass.Player).FirstOrDefault();

                                            var classoverBack = await item.CardCanBeClassify(item, getinfo);

                                            if (classoverBack.Succese)
                                            {
                                                var newLine = new LineCardViewModel()
                                                {
                                                    LineLeft = item,
                                                    LineRight = getinfo,
                                                };

                                                CardItems.Add(newLine);

                                                ChangeTheLine(newLine);

                                                if (!item.Right.Contains(getinfo))
                                                {
                                                    item.Right.Add(getinfo);
                                                }

                                                if (!getinfo.Left.Contains(item))
                                                {
                                                    getinfo.Left.Add(item);
                                                }

                                                if (item.Type == ThumbClass.Subject)
                                                {
                                                    item.GiveMainCard(getinfo, item);
                                                }
                                                else
                                                {
                                                    if (item.MainCard != null)
                                                    {
                                                        item.GiveMainCard(getinfo, item.MainCard);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var j in i.Value["pointer"]["第 1 条参数"])
                                    {
                                        var value = j.Value;

                                        if (CardItems.Where(t => t.ConfigName == value && t.Type == ThumbClass.NPC).Any())
                                        {
                                            var getinfo = CardItems.Where(t => t.ConfigName == value && t.Type == ThumbClass.NPC).FirstOrDefault();

                                            var classoverBack = await item.CardCanBeClassify(item, getinfo);

                                            if (classoverBack.Succese)
                                            {
                                                var newLine = new LineCardViewModel()
                                                {
                                                    LineLeft = item,
                                                    LineRight = getinfo,
                                                };

                                                CardItems.Add(newLine);

                                                ChangeTheLine(newLine);

                                                if (!item.Right.Contains(getinfo))
                                                {
                                                    item.Right.Add(getinfo);
                                                }

                                                if (!getinfo.Left.Contains(item))
                                                {
                                                    getinfo.Left.Add(item);
                                                }

                                                if (item.Type == ThumbClass.Subject)
                                                {
                                                    item.GiveMainCard(getinfo, item);
                                                }
                                                else
                                                {
                                                    if (item.MainCard != null)
                                                    {
                                                        item.GiveMainCard(getinfo, item.MainCard);
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
                else
                {
                    switch (item.Type)
                    {
                        case ThumbClass.Conditions:
                            if (!ConditionsCardChanged.saveAllValue.ContainsKey(item as AnyCardViewModel))
                            {
                                continue;
                            }

                            foreach (var i in ConditionsCardChanged.saveAllValue[item as AnyCardViewModel])
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
                                                else if (getNeedClass == ThumbClass.Items)
                                                {
                                                    changeinfo = changeinfo.Split(':')[0];
                                                }

                                                if (CardItems.Where(t => t.ConfigName == changeinfo && t.Type == getNeedClass).Any())
                                                {
                                                    var getinfo = CardItems.Where(t => t.ConfigName == changeinfo && t.Type == getNeedClass).FirstOrDefault();

                                                    var classoverBack = await item.CardCanBeClassify(item, getinfo);

                                                    if (classoverBack.Succese)
                                                    {
                                                        var newLine = new LineCardViewModel()
                                                        {
                                                            LineLeft = item,
                                                            LineRight = getinfo,
                                                        };

                                                        CardItems.Add(newLine);

                                                        ChangeTheLine(newLine);


                                                        if (!item.Right.Contains(getinfo))
                                                        {
                                                            item.Right.Add(getinfo);
                                                        }

                                                        if (!getinfo.Left.Contains(item))
                                                        {
                                                            getinfo.Left.Add(item);
                                                        }

                                                        if (item.Type == ThumbClass.Subject)
                                                        {
                                                            item.GiveMainCard(getinfo, item);
                                                        }
                                                        else
                                                        {
                                                            if (item.MainCard != null)
                                                            {
                                                                item.GiveMainCard(getinfo, item.MainCard);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ShowMessage($"条件卡片[{item.ConfigName}]与卡片[类型:{getNeedClass}][名称:{changeinfo}]的关联失败！");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case ThumbClass.Events:
                            if (!EventCardChanged.saveAllValue.ContainsKey(item as AnyCardViewModel))
                            {
                                continue;
                            }

                            foreach (var i in EventCardChanged.saveAllValue[item as AnyCardViewModel])
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
                                                else if (getNeedClass == ThumbClass.Items)
                                                {
                                                    changeinfo = changeinfo.Split(':')[0];
                                                }

                                                if (CardItems.Where(t => t.ConfigName == changeinfo && t.Type == getNeedClass).Any())
                                                {
                                                    var getinfo = CardItems.Where(t => t.ConfigName == changeinfo && t.Type == getNeedClass).FirstOrDefault();

                                                    var classoverBack = await item.CardCanBeClassify(item, getinfo);

                                                    if (classoverBack.Succese)
                                                    {
                                                        var newLine = new LineCardViewModel()
                                                        {
                                                            LineLeft = item,
                                                            LineRight = getinfo,
                                                        };

                                                        CardItems.Add(newLine);

                                                        ChangeTheLine(newLine);

                                                        if (!item.Right.Contains(getinfo))
                                                        {
                                                            item.Right.Add(getinfo);
                                                        }

                                                        if (!getinfo.Left.Contains(item))
                                                        {
                                                            getinfo.Left.Add(item);
                                                        }

                                                        if (item.Type == ThumbClass.Subject)
                                                        {
                                                            item.GiveMainCard(getinfo, item);
                                                        }
                                                        else
                                                        {
                                                            if (item.MainCard != null)
                                                            {
                                                                item.GiveMainCard(getinfo, item.MainCard);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ShowMessage($"事件卡片[{item.ConfigName}]与卡片[类型:{getNeedClass}][名称:{changeinfo}]的关联失败！");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case ThumbClass.Objectives:
                            if (!ObjectiveCardChanged.saveAllValue.ContainsKey(item as AnyCardViewModel))
                            {
                                continue;
                            }

                            foreach (var i in ObjectiveCardChanged.saveAllValue[item as AnyCardViewModel])
                            {
                                var getRealCmd = GetRealCmd(i.Key);

                                var findModel = objectiveProp.Where(t => t.MainClass == getRealCmd).First();

                                if (findModel.NeedTpye != null && findModel.NeedTpye.Count > 0)
                                {
                                    foreach (var j in i.Value)
                                    {//j.key是命令

                                        if (!findModel.NeedTpye.Where(t => t.Key == j.Key).Any())
                                        {
                                            continue;
                                        }

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
                                                else if (getNeedClass == ThumbClass.Items)
                                                {
                                                    changeinfo = changeinfo.Split(':')[0];
                                                }

                                                if (CardItems.Where(t => t.ConfigName == changeinfo && t.Type == getNeedClass).Any())
                                                {
                                                    var getinfo = CardItems.Where(t => t.ConfigName == changeinfo && t.Type == getNeedClass).FirstOrDefault();

                                                    var classoverBack = await item.CardCanBeClassify(item, getinfo);

                                                    if (classoverBack.Succese)
                                                    {
                                                        var newLine = new LineCardViewModel()
                                                        {
                                                            LineLeft = item,
                                                            LineRight = getinfo,
                                                        };

                                                        CardItems.Add(newLine);

                                                        ChangeTheLine(newLine);

                                                        if (!item.Right.Contains(getinfo))
                                                        {
                                                            item.Right.Add(getinfo);
                                                        }

                                                        if (!getinfo.Left.Contains(item))
                                                        {
                                                            getinfo.Left.Add(item);
                                                        }

                                                        if (item.Type == ThumbClass.Subject)
                                                        {
                                                            item.GiveMainCard(getinfo, item);
                                                        }
                                                        else
                                                        {
                                                            if (item.MainCard != null)
                                                            {
                                                                item.GiveMainCard(getinfo, item.MainCard);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ShowMessage($"目标卡片[{item.ConfigName}]与卡片[类型:{getNeedClass}][名称:{changeinfo}]的关联失败！");
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

            #region 帮助录入

            foreach (var item in saveHelpTool)
            {
                if (!CardItems.Where(t => t.ConfigName == item.Name && t.Type == item.Class).Any())
                {
                    ShowMessage($"帮助配置中未找到相关控件{item.Name}");
                    continue;
                }

                var thumb = CardItems.Where(t => t.ConfigName == item.Name && t.Type == item.Class).FirstOrDefault();

                thumb.HelpInfo = item.Tool;
            }

            #endregion

            result.SetSuccese();
            return result;
        }

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

            var saveBase = new ConfigReaderAndWriter(ConditionsCardChanged.saveAllValue, EventCardChanged.saveAllValue, ObjectiveCardChanged.saveAllValue, ConversationCardChanged.saveAllValue, CardItems);

            var npcnum = 0;

            var playernum = 0;

            var treeViewBase = new TreeViewBase();

            var x = this.window.outsaid.ActualWidth / 2 - 400 / 2 - this.TranslateXProp;

            var y = this.window.outsaid.ActualHeight / 2 - 148 / 2 - this.TranslateYProp;

            #region 控件的生成与数据的绑定

            for (int i = 0; i < allConversations.Count; i++)
            {
                var subjectCardView = await CreateCard<SubjectCardViewModel>(ThumbClass.Subject, false, FileService.GetFilePathToFileName(allConversationsFilePath[i]), x + i * 500, y);

                subjectCardView.NPC_ID = getMain.npcs.Where(t => t.Value == FileService.GetFilePathToFileName(allConversationsFilePath[i])).First().Key;

                subjectCardView.ItemContent = allConversations[i].quester;

                ThumbNums = CardItems.Count.ToString();

                foreach (var item in allConversations[i].NPC_options)
                {
                    var cardView = await CreateCard<AnyCardViewModel>(ThumbClass.NPC, false, item.Key, x + npcnum * 500, y + 200);

                    ThumbNums = CardItems.Count.ToString();

                    #region 数据录入

                    var getText = item.Value.text;

                    for (int j = 0; j < getText.Count; j++)
                    {
                        try
                        {
                            if (ConversationCardChanged.saveAllValue[cardView]["文案: text"]["text"]["第 1 条参数"].ContainsKey($"第 {j + 1} 项"))
                            {
                                ConversationCardChanged.saveAllValue[cardView]["文案: text"]["text"]["第 1 条参数"][$"第 {j + 1} 项"] = getText[j];
                            }
                            else
                            {
                                ConversationCardChanged.saveAllValue[cardView]["文案: text"]["text"]["第 1 条参数"].Add($"第 {j + 1} 项", getText[j]);
                            }
                        }
                        catch
                        {
                            ShowMessage($"NPC卡片[{item.Key}]数据录入错误，如果发生此错误请尝试重新加载并报告jk@jklss.cn");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Value.conditions))
                    {
                        var getConditionsProp = await saveBase.PlayerAndNpcAnalysis(item.Value.conditions);

                        if (!getConditionsProp.Succese)
                        {
                            result.SetError();
                            return result;
                        }

                        var conditionsProp = getConditionsProp.Backs as Dictionary<string, Dictionary<string, string>>;

                        foreach (var j in conditionsProp["第 1 条参数"])
                        {
                            try
                            {
                                if (ConversationCardChanged.saveAllValue[cardView]["触发条件: conditions"]["conditions"]["第 1 条参数"].ContainsKey(j.Key))
                                {
                                    ConversationCardChanged.saveAllValue[cardView]["触发条件: conditions"]["conditions"]["第 1 条参数"][j.Key] = j.Value;
                                }
                                else
                                {
                                    ConversationCardChanged.saveAllValue[cardView]["触发条件: conditions"]["conditions"]["第 1 条参数"].Add(j.Key, j.Value);
                                }
                            }
                            catch
                            {
                                ShowMessage($"NPC卡片[{item.Key}]数据录入错误，如果发生此错误请尝试重新加载并报告jk@jklss.cn");
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Value.events))
                    {
                        var getEventProp = await saveBase.PlayerAndNpcAnalysis(item.Value.events);

                        if (!getEventProp.Succese)
                        {
                            result.SetError();
                            return result;
                        }

                        var EventProp = getEventProp.Backs as Dictionary<string, Dictionary<string, string>>;

                        foreach (var j in EventProp["第 1 条参数"])
                        {
                            try
                            {
                                if (ConversationCardChanged.saveAllValue[cardView]["触发事件: events"]["events"]["第 1 条参数"].ContainsKey(j.Key))
                                {
                                    ConversationCardChanged.saveAllValue[cardView]["触发事件: events"]["events"]["第 1 条参数"][j.Key] = j.Value;
                                }
                                else
                                {
                                    ConversationCardChanged.saveAllValue[cardView]["触发事件: events"]["events"]["第 1 条参数"].Add(j.Key, j.Value);
                                }
                            }
                            catch
                            {
                                ShowMessage($"NPC卡片[{item.Key}]数据录入错误，如果发生此错误请尝试重新加载并报告jk@jklss.cn");
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Value.pointer))
                    {
                        var getPointerProp = await saveBase.PlayerAndNpcAnalysis(item.Value.pointer);

                        if (!getPointerProp.Succese)
                        {
                            result.SetError();
                            return result;
                        }

                        var PointerProp = getPointerProp.Backs as Dictionary<string, Dictionary<string, string>>;

                        foreach (var j in PointerProp["第 1 条参数"])
                        {
                            try
                            {
                                if (ConversationCardChanged.saveAllValue[cardView]["存储对话: pointer"]["pointer"]["第 1 条参数"].ContainsKey(j.Key))
                                {
                                    ConversationCardChanged.saveAllValue[cardView]["存储对话: pointer"]["pointer"]["第 1 条参数"][j.Key] = j.Value;
                                }
                                else
                                {
                                    ConversationCardChanged.saveAllValue[cardView]["存储对话: pointer"]["pointer"]["第 1 条参数"].Add(j.Key, j.Value);
                                }
                            }
                            catch
                            {
                                ShowMessage($"NPC卡片[{item.Key}]数据录入错误，如果发生此错误请尝试重新加载并报告jk@jklss.cn");
                            }
                        }
                    }
                    #endregion

                    npcnum++;
                }

                foreach (var item in allConversations[i].player_options)
                {
                    var cardView = await CreateCard<AnyCardViewModel>(ThumbClass.Player, false, item.Key, x + playernum * 500, y + 400);

                    ThumbNums = CardItems.Count.ToString();

                    #region 数据录入

                    var getText = item.Value.text;

                    for (int j = 0; j < getText.Count; j++)
                    {
                        try
                        {
                            if (ConversationCardChanged.saveAllValue[cardView]["文案: text"]["text"]["第 1 条参数"].ContainsKey($"第 {j + 1} 项"))
                            {
                                ConversationCardChanged.saveAllValue[cardView]["文案: text"]["text"]["第 1 条参数"][$"第 {j + 1} 项"] = getText[j];
                            }
                            else
                            {
                                ConversationCardChanged.saveAllValue[cardView]["文案: text"]["text"]["第 1 条参数"].Add($"第 {j + 1} 项", getText[j]);
                            }
                        }
                        catch
                        {
                            ShowMessage($"NPC卡片[{item.Key}]数据录入错误，如果发生此错误请尝试重新加载并报告jk@jklss.cn");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Value.conditions))
                    {
                        var getConditionsProp = await saveBase.PlayerAndNpcAnalysis(item.Value.conditions);

                        if (!getConditionsProp.Succese)
                        {
                            result.SetError();
                            return result;
                        }

                        var conditionsProp = getConditionsProp.Backs as Dictionary<string, Dictionary<string, string>>;

                        foreach (var j in conditionsProp["第 1 条参数"])
                        {
                            try
                            {
                                if (ConversationCardChanged.saveAllValue[cardView]["触发条件: conditions"]["conditions"]["第 1 条参数"].ContainsKey(j.Key))
                                {
                                    ConversationCardChanged.saveAllValue[cardView]["触发条件: conditions"]["conditions"]["第 1 条参数"][j.Key] = j.Value;
                                }
                                else
                                {
                                    ConversationCardChanged.saveAllValue[cardView]["触发条件: conditions"]["conditions"]["第 1 条参数"].Add(j.Key, j.Value);
                                }
                            }
                            catch
                            {
                                ShowMessage($"Player卡片[{item.Key}]数据录入错误，如果发生此错误请尝试重新加载并报告jk@jklss.cn");
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Value.events))
                    {
                        var getEventProp = await saveBase.PlayerAndNpcAnalysis(item.Value.events);

                        if (!getEventProp.Succese)
                        {
                            result.SetError();
                            return result;
                        }

                        var EventProp = getEventProp.Backs as Dictionary<string, Dictionary<string, string>>;

                        foreach (var j in EventProp["第 1 条参数"])
                        {
                            try
                            {
                                if (ConversationCardChanged.saveAllValue[cardView]["触发事件: events"]["events"]["第 1 条参数"].ContainsKey(j.Key))
                                {
                                    ConversationCardChanged.saveAllValue[cardView]["触发事件: events"]["events"]["第 1 条参数"][j.Key] = j.Value;
                                }
                                else
                                {
                                    ConversationCardChanged.saveAllValue[cardView]["触发事件: events"]["events"]["第 1 条参数"].Add(j.Key, j.Value);
                                }
                            }
                            catch
                            {
                                ShowMessage($"Player卡片[{item.Key}]数据录入错误，如果发生此错误请尝试重新加载并报告jk@jklss.cn");
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Value.pointer))
                    {
                        var getPointerProp = await saveBase.PlayerAndNpcAnalysis(item.Value.pointer);

                        if (!getPointerProp.Succese)
                        {
                            result.SetError();
                            return result;
                        }

                        var PointerProp = getPointerProp.Backs as Dictionary<string, Dictionary<string, string>>;

                        foreach (var j in PointerProp["第 1 条参数"])
                        {
                            try
                            {
                                if (ConversationCardChanged.saveAllValue[cardView]["存储对话: pointer"]["pointer"]["第 1 条参数"].ContainsKey(j.Key))
                                {
                                    ConversationCardChanged.saveAllValue[cardView]["存储对话: pointer"]["pointer"]["第 1 条参数"][j.Key] = j.Value;
                                }
                                else
                                {
                                    ConversationCardChanged.saveAllValue[cardView]["存储对话: pointer"]["pointer"]["第 1 条参数"].Add(j.Key, j.Value);
                                }
                            }
                            catch
                            {
                                ShowMessage($"Player卡片[{item.Key}]数据录入错误，如果发生此错误请尝试重新加载并报告jk@jklss.cn");
                            }
                        }
                    }
                    #endregion

                    playernum++;
                }

            }//生成对话主体及Npc与Player对话

            for (int i = 0; i < getConditions.Count; i++)
            {
                var cardView = await CreateCard<AnyCardViewModel>(ThumbClass.Conditions, false, getConditions[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries)[0], x + i * 500, y + 600);

                ThumbNums = CardItems.Count.ToString();

                #region 数据处理

                var fg = getConditions[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                var backs = await saveBase.ConditionAnalysis(fg[1].Trim('\''));

                if (!backs.Succese)
                {
                    result.SetError();
                    return result;
                }

                var getConditionsProp = backs.Backs as Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>;

                var getCmd = string.Empty;

                foreach (var item in cardView.AllType)
                {
                    var fgf = item.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    if (fgf[1] == getConditionsProp.Keys.First())
                    {
                        cardView.SelectType = item;
                        getCmd = item;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(getCmd))
                {
                    ShowMessage($"[{cardView.ConfigName}]卡片数据添加失败,您的语法模型中不存在该语法");
                    continue;
                }

                var getSaveInfo = ConditionsCardChanged.saveAllValue[cardView];

                foreach (var k in getConditionsProp)
                {
                    foreach (var j in k.Value)
                    {
                        foreach (var n in j.Value)
                        {
                            foreach (var m in n.Value)
                            {
                                try
                                {
                                    if (getSaveInfo[getCmd][j.Key][n.Key].ContainsKey(m.Key))
                                    {
                                        getSaveInfo[getCmd][j.Key][n.Key][m.Key] = m.Value;
                                    }
                                    else
                                    {
                                        getSaveInfo[getCmd][j.Key][n.Key].Add(m.Key, m.Value);
                                    }

                                }
                                catch
                                {
                                    ShowMessage($"[{cardView.ConfigName}]卡片数据添加失败,您的语法模型与数据不匹配");
                                }
                            }
                        }
                    }
                }

                #endregion
            }

            for (int i = 0; i < getEvents.Count; i++)
            {
                var cardView = await CreateCard<AnyCardViewModel>(ThumbClass.Events, false, getEvents[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries)[0], x + i * 500, y + 800);

                ThumbNums = CardItems.Count.ToString();

                #region 数据处理

                var fg = getEvents[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                var backs = await saveBase.EventAnalysis(fg[1].Trim('\''));

                if (!backs.Succese)
                {
                    result.SetError();
                    return result;
                }

                var getConditionsProp = backs.Backs as Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>;

                var getCmd = string.Empty;

                foreach (var item in cardView.AllType)
                {
                    var fgf = item.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    if (fgf[1] == getConditionsProp.Keys.First())
                    {
                        cardView.SelectType = item;
                        getCmd = item;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(getCmd))
                {
                    ShowMessage($"[{cardView.ConfigName}]卡片数据添加失败,您的语法模型中不存在该语法");
                    continue;
                }

                var getSaveInfo = EventCardChanged.saveAllValue[cardView];

                foreach (var k in getConditionsProp)
                {
                    foreach (var j in k.Value)
                    {
                        foreach (var n in j.Value)
                        {
                            foreach (var m in n.Value)
                            {
                                try
                                {
                                    if (getSaveInfo[getCmd][j.Key][n.Key].ContainsKey(m.Key))
                                    {
                                        getSaveInfo[getCmd][j.Key][n.Key][m.Key] = m.Value;
                                    }
                                    else
                                    {
                                        getSaveInfo[getCmd][j.Key][n.Key].Add(m.Key, m.Value);
                                    }

                                }
                                catch
                                {
                                    ShowMessage($"[{cardView.ConfigName}]卡片数据添加失败,您的语法模型与数据不匹配");
                                }
                            }
                        }
                    }
                }

                #endregion
            }

            for (int i = 0; i < getObjectives.Count; i++)
            {
                var cardView = await CreateCard<AnyCardViewModel>(ThumbClass.Objectives, false, getObjectives[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries)[0], x + i * 500, y + 1000);

                ThumbNums = CardItems.Count.ToString();

                #region 数据处理

                var fg = getObjectives[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                var backs = await saveBase.ObjectiveAnalysis(fg[1].Trim('\''));

                if (!backs.Succese)
                {
                    ShowMessage($"[{cardView.ConfigName}]卡片数据添加失败,您的语法模型中不存在该语法");
                    continue;
                }

                var getConditionsProp = backs.Backs as Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>;

                var getCmd = string.Empty;

                foreach (var item in cardView.AllType)
                {
                    var fgf = item.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    if (fgf[1] == getConditionsProp.Keys.First())
                    {
                        cardView.SelectType = item;
                        getCmd = item;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(getCmd))
                {
                    ShowMessage($"[{cardView.ConfigName}]卡片数据添加失败,您的语法模型中不存在该语法");
                    continue;
                }

                var getSaveInfo = ObjectiveCardChanged.saveAllValue[cardView];

                foreach (var k in getConditionsProp)
                {
                    foreach (var j in k.Value)
                    {
                        foreach (var n in j.Value)
                        {
                            foreach (var m in n.Value)
                            {
                                try
                                {
                                    if (getSaveInfo[getCmd][j.Key][n.Key].ContainsKey(m.Key))
                                    {
                                        getSaveInfo[getCmd][j.Key][n.Key][m.Key] = m.Value;
                                    }
                                    else
                                    {
                                        getSaveInfo[getCmd][j.Key][n.Key].Add(m.Key, m.Value);
                                    }

                                }
                                catch
                                {
                                    ShowMessage($"[{cardView.ConfigName}]卡片数据添加失败,您的语法模型与数据不匹配");
                                }
                            }
                        }
                    }
                }

                #endregion
            }

            var jnum = 0;

            foreach (var item in getJournal)
            {
                var cardView = await CreateCard<CardViewModel>(ThumbClass.Journal, false, item.Key, x + jnum * 500, y + 1200);

                cardView.ItemContent = item.Value;

                ThumbNums = CardItems.Count.ToString();

                jnum++;
            }

            for (int i = 0; i < getItems.Count; i++)
            {
                var fg = getItems[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                var getLest = getItems[i].Split(fg[0], StringSplitOptions.RemoveEmptyEntries);

                var cardView = await CreateCard<CardViewModel>(ThumbClass.Items, false, fg[0], x + i * 500, y + 1400);

                cardView.ItemContent = getLest[0].TrimStart('\'').TrimEnd('\'');

                ThumbNums = CardItems.Count.ToString();
            }

            #endregion

            #region 关系建立

            try
            {
                var getMainThumb = CardItems.Where(t => t.Type == ThumbClass.Subject && !t.IsDraw && !t.IsLine).ToList();

                foreach (var item in getMainThumb)
                {
                    var getMainInfo = allConversations.Where(t => t.quester == item.ItemContent).FirstOrDefault();

                    if (getMainInfo == null)
                    {
                        ShowMessage($"未找到卡片[{item.ConfigName}]的信息");
                    }

                    var getFg = getMainInfo.first.Trim('\"').Split(",", StringSplitOptions.RemoveEmptyEntries);

                    foreach (var i in getFg)
                    {
                        var findChild = CardItems.Where(t => t.ConfigName == i && t.Type == ThumbClass.NPC).FirstOrDefault();
                        findChild.MainCard = item;
                        if (findChild == null)
                        {
                            ShowMessage($"未找到卡片[{i}]的信息");
                        }

                        var newLine = new LineCardViewModel()
                        {
                            LineLeft = item,
                            LineRight = findChild,
                        };

                        CardItems.Add(newLine);

                        ChangeTheLine(newLine);

                        if (!item.Right.Contains(findChild))
                        {
                            item.Right.Add(findChild);
                        }

                        if (!findChild.Left.Contains(item))
                        {
                            findChild.Left.Add(item);
                        }

                    }
                }
            }
            catch
            {
                ShowMessage("未找到相关控件，请尝试重新读取!");
                result.SetError();
                return result;
            }

            var getAllThumb = CardItems.Where(t => t.Type != ThumbClass.Subject).ToList();

            foreach (var item in getAllThumb)
            {
                if (item.Type == ThumbClass.NPC || item.Type == ThumbClass.Player)
                {
                    if (!ConversationCardChanged.saveAllValue.ContainsKey(item as AnyCardViewModel))
                    {
                        continue;
                    }

                    foreach (var i in ConversationCardChanged.saveAllValue[item as AnyCardViewModel])
                    {
                        switch (GetRealCmd(i.Key))
                        {
                            case "conditions":
                                foreach (var j in i.Value["conditions"]["第 1 条参数"])
                                {
                                    var value = j.Value.TrimStart('!');

                                    if (CardItems.Where(t => t.ConfigName == value && t.Type == ThumbClass.Conditions).Any())
                                    {
                                        var getinfo = CardItems.Where(t => t.ConfigName == value && t.Type == ThumbClass.Conditions).FirstOrDefault();

                                        var classoverBack = await item.CardCanBeClassify(item, getinfo);

                                        if (classoverBack.Succese)
                                        {
                                            var newLine = new LineCardViewModel()
                                            {
                                                LineLeft = item,
                                                LineRight = getinfo,
                                            };

                                            CardItems.Add(newLine);

                                            ChangeTheLine(newLine);

                                            if (!item.Right.Contains(getinfo))
                                            {
                                                item.Right.Add(getinfo);
                                            }

                                            if (!getinfo.Left.Contains(item))
                                            {
                                                getinfo.Left.Add(item);
                                            }

                                            if (item.Type == ThumbClass.Subject)
                                            {
                                                item.GiveMainCard(getinfo, item);
                                            }
                                            else
                                            {
                                                if (item.MainCard != null)
                                                {
                                                    item.GiveMainCard(getinfo, item.MainCard);
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case "events":
                                foreach (var j in i.Value["events"]["第 1 条参数"])
                                {
                                    var value = j.Value;

                                    if (CardItems.Where(t => t.ConfigName == value && t.Type == ThumbClass.Events).Any())
                                    {
                                        var getinfo = CardItems.Where(t => t.ConfigName == value && t.Type == ThumbClass.Events).FirstOrDefault();

                                        var classoverBack = await item.CardCanBeClassify(item, getinfo);

                                        if (classoverBack.Succese)
                                        {
                                            var newLine = new LineCardViewModel()
                                            {
                                                LineLeft = item,
                                                LineRight = getinfo,
                                            };

                                            CardItems.Add(newLine);

                                            ChangeTheLine(newLine);

                                            if (!item.Right.Contains(getinfo))
                                            {
                                                item.Right.Add(getinfo);
                                            }

                                            if (!getinfo.Left.Contains(item))
                                            {
                                                getinfo.Left.Add(item);
                                            }

                                            if (item.Type == ThumbClass.Subject)
                                            {
                                                item.GiveMainCard(getinfo, item);
                                            }
                                            else
                                            {
                                                if (item.MainCard != null)
                                                {
                                                    item.GiveMainCard(getinfo, item.MainCard);
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case "pointer":
                                if (item.Type == ThumbClass.NPC)
                                {
                                    foreach (var j in i.Value["pointer"]["第 1 条参数"])
                                    {
                                        var value = j.Value;

                                        if (CardItems.Where(t => t.ConfigName == value && t.Type == ThumbClass.Player).Any())
                                        {
                                            var getinfo = CardItems.Where(t => t.ConfigName == value && t.Type == ThumbClass.Player).FirstOrDefault();

                                            var classoverBack = await item.CardCanBeClassify(item, getinfo);

                                            if (classoverBack.Succese)
                                            {
                                                var newLine = new LineCardViewModel()
                                                {
                                                    LineLeft = item,
                                                    LineRight = getinfo,
                                                };

                                                CardItems.Add(newLine);

                                                ChangeTheLine(newLine);

                                                if (!item.Right.Contains(getinfo))
                                                {
                                                    item.Right.Add(getinfo);
                                                }

                                                if (!getinfo.Left.Contains(item))
                                                {
                                                    getinfo.Left.Add(item);
                                                }

                                                if (item.Type == ThumbClass.Subject)
                                                {
                                                    item.GiveMainCard(getinfo, item);
                                                }
                                                else
                                                {
                                                    if (item.MainCard != null)
                                                    {
                                                        item.GiveMainCard(getinfo, item.MainCard);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var j in i.Value["pointer"]["第 1 条参数"])
                                    {
                                        var value = j.Value;

                                        if (CardItems.Where(t => t.ConfigName == value && t.Type == ThumbClass.NPC).Any())
                                        {
                                            var getinfo = CardItems.Where(t => t.ConfigName == value && t.Type == ThumbClass.NPC).FirstOrDefault();

                                            var classoverBack = await item.CardCanBeClassify(item, getinfo);

                                            if (classoverBack.Succese)
                                            {
                                                var newLine = new LineCardViewModel()
                                                {
                                                    LineLeft = item,
                                                    LineRight = getinfo,
                                                };

                                                CardItems.Add(newLine);

                                                ChangeTheLine(newLine);

                                                if (!item.Right.Contains(getinfo))
                                                {
                                                    item.Right.Add(getinfo);
                                                }

                                                if (!getinfo.Left.Contains(item))
                                                {
                                                    getinfo.Left.Add(item);
                                                }

                                                if (item.Type == ThumbClass.Subject)
                                                {
                                                    item.GiveMainCard(getinfo, item);
                                                }
                                                else
                                                {
                                                    if (item.MainCard != null)
                                                    {
                                                        item.GiveMainCard(getinfo, item.MainCard);
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
                else
                {
                    switch (item.Type)
                    {
                        case ThumbClass.Conditions:
                            if (!ConditionsCardChanged.saveAllValue.ContainsKey(item as AnyCardViewModel))
                            {
                                continue;
                            }

                            foreach (var i in ConditionsCardChanged.saveAllValue[item as AnyCardViewModel])
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
                                                else if (getNeedClass == ThumbClass.Items)
                                                {
                                                    changeinfo = changeinfo.Split(':')[0];
                                                }

                                                if (CardItems.Where(t => t.ConfigName == changeinfo && t.Type == getNeedClass).Any())
                                                {
                                                    var getinfo = CardItems.Where(t => t.ConfigName == changeinfo && t.Type == getNeedClass).FirstOrDefault();

                                                    var classoverBack = await item.CardCanBeClassify(item, getinfo);

                                                    if (classoverBack.Succese)
                                                    {
                                                        var newLine = new LineCardViewModel()
                                                        {
                                                            LineLeft = item,
                                                            LineRight = getinfo,
                                                        };

                                                        CardItems.Add(newLine);

                                                        ChangeTheLine(newLine);
                                                        if (!item.Right.Contains(getinfo))
                                                        {
                                                            item.Right.Add(getinfo);
                                                        }

                                                        if (!getinfo.Left.Contains(item))
                                                        {
                                                            getinfo.Left.Add(item);
                                                        }

                                                        if (item.Type == ThumbClass.Subject)
                                                        {
                                                            item.GiveMainCard(getinfo, item);
                                                        }
                                                        else
                                                        {
                                                            if (item.MainCard != null)
                                                            {
                                                                item.GiveMainCard(getinfo, item.MainCard);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ShowMessage($"条件卡片[{item.ConfigName}]与卡片[类型:{getNeedClass}][名称:{changeinfo}]的关联失败！");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case ThumbClass.Events:
                            if (!EventCardChanged.saveAllValue.ContainsKey(item as AnyCardViewModel))
                            {
                                continue;
                            }

                            foreach (var i in EventCardChanged.saveAllValue[item as AnyCardViewModel])
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
                                                else if (getNeedClass == ThumbClass.Items)
                                                {
                                                    changeinfo = changeinfo.Split(':')[0];
                                                }

                                                if (CardItems.Where(t => t.ConfigName == changeinfo && t.Type == getNeedClass).Any())
                                                {
                                                    var getinfo = CardItems.Where(t => t.ConfigName == changeinfo && t.Type == getNeedClass).FirstOrDefault();

                                                    var classoverBack = await item.CardCanBeClassify(item, getinfo);

                                                    if (classoverBack.Succese)
                                                    {
                                                        var newLine = new LineCardViewModel()
                                                        {
                                                            LineLeft = item,
                                                            LineRight = getinfo,
                                                        };

                                                        CardItems.Add(newLine);

                                                        ChangeTheLine(newLine);

                                                        CardItems.Add(newLine);

                                                        if (!item.Right.Contains(getinfo))
                                                        {
                                                            item.Right.Add(getinfo);
                                                        }

                                                        if (!getinfo.Left.Contains(item))
                                                        {
                                                            getinfo.Left.Add(item);
                                                        }

                                                        if (item.Type == ThumbClass.Subject)
                                                        {
                                                            item.GiveMainCard(getinfo, item);
                                                        }
                                                        else
                                                        {
                                                            if (item.MainCard != null)
                                                            {
                                                                item.GiveMainCard(getinfo, item.MainCard);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ShowMessage($"事件卡片[{item.ConfigName}]与卡片[类型:{getNeedClass}][名称:{changeinfo}]的关联失败！");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case ThumbClass.Objectives:
                            if(!ObjectiveCardChanged.saveAllValue.ContainsKey(item as AnyCardViewModel))
                            {
                                continue;
                            }

                            foreach (var i in ObjectiveCardChanged.saveAllValue[item as AnyCardViewModel])
                            {
                                var getRealCmd = GetRealCmd(i.Key);

                                var findModel = objectiveProp.Where(t => t.MainClass == getRealCmd).First();

                                if (findModel.NeedTpye != null && findModel.NeedTpye.Count > 0)
                                {
                                    foreach (var j in i.Value)
                                    {//j.key是命令

                                        if (!findModel.NeedTpye.Where(t => t.Key == j.Key).Any())
                                        {
                                            continue;
                                        }

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
                                                else if (getNeedClass == ThumbClass.Items)
                                                {
                                                    changeinfo = changeinfo.Split(':')[0];
                                                }

                                                if (CardItems.Where(t => t.ConfigName == changeinfo && t.Type == getNeedClass).Any())
                                                {
                                                    var getinfo = CardItems.Where(t => t.ConfigName == changeinfo && t.Type == getNeedClass).FirstOrDefault();

                                                    var classoverBack = await item.CardCanBeClassify(item, getinfo);

                                                    if (classoverBack.Succese)
                                                    {
                                                        var newLine = new LineCardViewModel()
                                                        {
                                                            LineLeft = item,
                                                            LineRight = getinfo,
                                                        };

                                                        CardItems.Add(newLine);

                                                        ChangeTheLine(newLine);

                                                        if (!item.Right.Contains(getinfo))
                                                        {
                                                            item.Right.Add(getinfo);
                                                        }

                                                        if (!getinfo.Left.Contains(item))
                                                        {
                                                            getinfo.Left.Add(item);
                                                        }

                                                        if (item.Type == ThumbClass.Subject)
                                                        {
                                                            item.GiveMainCard(getinfo, item);
                                                        }
                                                        else
                                                        {
                                                            if (item.MainCard != null)
                                                            {
                                                                item.GiveMainCard(getinfo, item.MainCard);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ShowMessage($"目标卡片[{item.ConfigName}]与卡片[类型:{getNeedClass}][名称:{changeinfo}]的关联失败！");
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

            result.SetSuccese();
            return result;
        }

        public void SetSelectCardInfo(CardViewModel card)
        {
            selectCardInfo = card;
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
        /// 删除卡片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActBase_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)//键盘的Enter，子自行设定。
            {
                if (window == null )
                {
                    return;
                }

                if (selectCardInfo == null)
                {
                    return;
                }

                if (System.Windows.MessageBox.Show("你确定将其删除，这将是不可逆的操作？", "删除", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    ShowMessage("取消了删除~");
                }

                if (!CardItems.Contains(selectCardInfo))
                {
                    ShowMessage("不存在该卡片！");
                }

                var getLeftHave = CardItems.Where(t => t.Left.Contains(selectCardInfo)).ToList();

                var getRightHave = CardItems.Where(t => t.Right.Contains(selectCardInfo)).ToList();

                var getLine = CardItems.Where(t => t.IsLine).ToList();

                var getRealLine = new List<LineCardViewModel>();

                foreach (var item in getLine)
                {
                    getRealLine.Add(item as LineCardViewModel);
                }

                var getLineHave = getRealLine.Where(t => t.LineLeft == selectCardInfo || t.LineRight == selectCardInfo).ToList();

                foreach (var item in getLineHave)
                {
                    CardItems.Remove(item);
                }//删除链接线

                foreach (var item in getRightHave)
                {
                    if(item.Type == ThumbClass.NPC|| item.Type == ThumbClass.Player)
                    {
                        if(!ConversationCardChanged.saveAllValue.ContainsKey(item as AnyCardViewModel))
                        {
                            break;
                        }

                        foreach(var i in ConversationCardChanged.saveAllValue[item as AnyCardViewModel])
                        {
                            foreach (var j in i.Value)
                            {
                                foreach (var m in j.Value)
                                {
                                    foreach (var n in m.Value)
                                    {
                                        if(n.Value == selectCardInfo.ConfigName)
                                        {
                                            ConversationCardChanged.saveAllValue[item as AnyCardViewModel][i.Key][j.Key][m.Key][n.Key] = string.Empty;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if(item.Type == ThumbClass.Conditions)
                    {
                        if (!ConditionsCardChanged.saveAllValue.ContainsKey(item as AnyCardViewModel))
                        {
                            break;
                        }

                        foreach (var i in ConditionsCardChanged.saveAllValue[item as AnyCardViewModel])
                        {
                            foreach (var j in i.Value)
                            {
                                foreach (var m in j.Value)
                                {
                                    foreach (var n in m.Value)
                                    {
                                        if (n.Value == selectCardInfo.ConfigName)
                                        {
                                            ConditionsCardChanged.saveAllValue[item as AnyCardViewModel][i.Key][j.Key][m.Key][n.Key] = string.Empty;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if(item.Type == ThumbClass.Events)
                    {
                        if (!EventCardChanged.saveAllValue.ContainsKey(item as AnyCardViewModel))
                        {
                            break;
                        }

                        foreach (var i in EventCardChanged.saveAllValue[item as AnyCardViewModel])
                        {
                            foreach (var j in i.Value)
                            {
                                foreach (var m in j.Value)
                                {
                                    foreach (var n in m.Value)
                                    {
                                        if (n.Value == selectCardInfo.ConfigName)
                                        {
                                            EventCardChanged.saveAllValue[item as AnyCardViewModel][i.Key][j.Key][m.Key][n.Key] = string.Empty;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if(item.Type == ThumbClass.Objectives)
                    {
                        if (!ObjectiveCardChanged.saveAllValue.ContainsKey(item as AnyCardViewModel))
                        {
                            break;
                        }

                        foreach (var i in ObjectiveCardChanged.saveAllValue[item as AnyCardViewModel])
                        {
                            foreach (var j in i.Value)
                            {
                                foreach (var m in j.Value)
                                {
                                    foreach (var n in m.Value)
                                    {
                                        if (n.Value == selectCardInfo.ConfigName)
                                        {
                                            ObjectiveCardChanged.saveAllValue[item as AnyCardViewModel][i.Key][j.Key][m.Key][n.Key] = string.Empty;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    item.Right.Remove(selectCardInfo);
                }//删除此卡片子级的数据

                foreach (var item in getLeftHave)
                {
                    item.Left.Remove(selectCardInfo);
                }//删除此卡片父级的数据

                CardItems.Remove(selectCardInfo);//卡片删除

                ShowMessage("删除成功！");
            }
        }

        private void ChangeTheLine(LineCardViewModel item)
        {
            item.CvLeft = item.LineLeft.CvLeft + (double)item.LineLeft.ThumbWidth / 2;

            item.CvTop = item.LineLeft.CvTop + (double)item.LineLeft.ThumbHeight / 2;

            item.X = -((item.LineLeft.CvLeft - item.LineRight.CvLeft) - Math.Abs((double)item.LineLeft.ThumbWidth - (double)item.LineRight.ThumbWidth) / 2);

            item.Y = -((item.LineLeft.CvTop - item.LineRight.CvTop) - Math.Abs((double)item.LineLeft.ThumbHeight - (double)item.LineRight.ThumbHeight) / 2);
        }

        /// <summary>
        /// 创建卡片
        /// </summary>
        /// <typeparam name="T">卡片类型</typeparam>
        /// <param name="thumbClass">卡片类型</param>
        /// <param name="isPay">是否扣除积分</param>
        /// <param name="configName">卡片名称（仅对主卡片有用）</param>
        /// <param name="cvLeft">X轴坐标</param>
        /// <param name="cvTop">Y轴坐标</param>
        /// <returns></returns>
        public async Task<T> CreateCard<T>(ThumbClass thumbClass,bool isPay = true,string configName = ""
            ,double cvLeft= 0.00,double cvTop = 0.00) where T:CardViewModel
        {
            CardViewModel cardView = null;

            var reCvLeft = 0.00;

            var reCvTop = 0.00;

            var reConfigName = string.Empty;

            #region 修正坐标

            if (cvLeft == 0.00)
            {
                reCvLeft = this.window.outsaid.ActualWidth / 2 - 400 / 2 - this.TranslateXProp;
            }
            else
            {
                reCvLeft = cvLeft;
            }

            if(cvTop == 0.00)
            {
                reCvTop = this.window.outsaid.ActualHeight / 2 - 148 / 2 - this.TranslateYProp;
            }
            else
            {
                reCvTop = cvTop;
            }

            #endregion
            switch (thumbClass)
            {
                case ThumbClass.Subject:

                    cardView = new SubjectCardViewModel()
                    {
                        ConfigName = configName,
                        CvLeft = reCvLeft,
                        CvTop = reCvTop,
                        Type = ThumbClass.Subject,
                        IsProtectName = IsProtectName,

                        ShurtcutIdea = new ObservableCollection<ShurtcutIdeaBtnViewModel>
                        {
                            new ShurtcutIdeaBtnViewModel
                            {
                                CardName = "创建NPC对话卡片",
                                ThumbClassName = ThumbClass.NPC,
                            },

                        }
                    };

                    if (!isPay)
                    {
                        CardItems.Add(cardView);
                        break;
                    }

                    try
                    {
                        var success = await PayPoint(1);

                        ShowMessage(success.Text);

                        if (!success.Succese)
                        {
                            break;
                        }

                        CardItems.Add(cardView);
                    }
                    catch
                    {
                        ShowMessage("创建卡片失败！");
                    }

                    break;

                case ThumbClass.NPC:

                    var name = string.Empty;

                    if (string.IsNullOrEmpty(configName))
                    {
                        name = "NPC_" + GetRandomString(5);

                        while (CardItems.Where(t => t.ConfigName == name).Any())
                        {
                            name = "NPC_" + GetRandomString(5);
                        }
                    }
                    else
                    {
                        name = configName;
                    }

                    cardView = new AnyCardViewModel(true)
                    {
                        ConfigName = name,
                        CvLeft = reCvLeft,
                        CvTop = reCvTop,
                        Type = ThumbClass.NPC,
                        IsProtectName = IsProtectName,
                        AllType = new ObservableCollection<string>
                        {
                            "文案: text",
                            "触发条件: conditions",
                            "触发事件: events",
                            "存储对话: pointer",
                        },

                        ShurtcutIdea = new ObservableCollection<ShurtcutIdeaBtnViewModel>
                        {
                            new ShurtcutIdeaBtnViewModel
                            {
                                CardName = "创建玩家对话卡片",
                                ThumbClassName = ThumbClass.Player,
                            },

                            new ShurtcutIdeaBtnViewModel
                            {
                                CardName = "创建条件卡片",
                                ThumbClassName = ThumbClass.Conditions,
                            },

                            new ShurtcutIdeaBtnViewModel
                            {
                                CardName = "创建事件卡片",
                                ThumbClassName = ThumbClass.Events,
                            },

                        }
                    };

                    if (!isPay)
                    {
                        CardItems.Add(cardView);
                        break;
                    }

                    try
                    {
                        var success = await PayPoint(1);

                        ShowMessage(success.Text);

                        if (!success.Succese)
                        {
                            break;
                        }

                        CardItems.Add(cardView);
                    }
                    catch
                    {
                        ShowMessage("创建卡片失败！");
                    }

                    break;

                case ThumbClass.Player:

                    name = string.Empty;

                    if (string.IsNullOrEmpty(configName))
                    {
                        name = "Player_" + GetRandomString(5);

                        while (CardItems.Where(t => t.ConfigName == name).Any())
                        {
                            name = "Player_" + GetRandomString(5);
                        }
                    }
                    else
                    {
                        name = configName;
                    }

                    cardView = new AnyCardViewModel(true)
                    {
                        ConfigName = name,
                        CvLeft = reCvLeft,
                        CvTop = reCvTop,
                        Type = ThumbClass.Player,
                        IsProtectName = IsProtectName,
                        AllType = new ObservableCollection<string>
                        {
                            "文案: text",
                            "触发条件: conditions",
                            "触发事件: events",
                            "存储对话: pointer",
                        },

                        ShurtcutIdea = new ObservableCollection<ShurtcutIdeaBtnViewModel>
                        {
                            new ShurtcutIdeaBtnViewModel
                            {
                                CardName = "创建NPC对话卡片",
                                ThumbClassName = ThumbClass.NPC,
                            },

                            new ShurtcutIdeaBtnViewModel
                            {
                                CardName = "创建条件卡片",
                                ThumbClassName = ThumbClass.Conditions,
                            },

                            new ShurtcutIdeaBtnViewModel
                            {
                                CardName = "创建事件卡片",
                                ThumbClassName = ThumbClass.Events,
                            },

                        }
                    }; 
                    
                    if (!isPay)
                    {
                        CardItems.Add(cardView);
                        break;
                    }

                    try
                    {
                        var success = await PayPoint(1);

                        ShowMessage(success.Text);

                        if (!success.Succese)
                        {
                            break;
                        }

                        CardItems.Add(cardView);
                    }
                    catch
                    {
                        ShowMessage("创建卡片失败！");
                    }

                    break;

                case ThumbClass.Conditions:

                    name = string.Empty;

                    if (string.IsNullOrEmpty(configName))
                    {
                        name = "Condition_" + GetRandomString(5);

                        while (CardItems.Where(t => t.ConfigName == name).Any())
                        {
                            name = "Condition_" + GetRandomString(5);
                        }
                    }
                    else
                    {
                        name = configName;
                    }

                    cardView = new AnyCardViewModel(contisionProp)
                    {
                        ConfigName = name,
                        CvLeft = reCvLeft,
                        CvTop = reCvTop,
                        Type = ThumbClass.Conditions,
                        IsProtectName = IsProtectName,
                        AllType = new ObservableCollection<string>
                        {
                            "背包中的物品: item",
                            "手持物品: hand",
                            "或门: or",
                            "与门: and",
                            "地点: location",
                            "生命值: health",
                            "经验: experience",
                            "权限: permission",
                            "点数: point",
                            "标签: tag",
                            "防具: armor",
                            "药水效果: effect",
                            "时间: time",
                            "天气: weather",
                            "高度: height",
                            "护甲值: rating",
                            "随机: random",
                            "潜行: sneak",
                            "日记条目: journal",
                            "方块状态检测: testforblock",
                            "空余背包格: empty",
                            "队伍: party",
                            "区域内怪物: monsters",
                            "目标: objective",
                            "检查条件: check",
                            "箱子物品: chestitem",
                            "计分板: score",
                            "资金(Vault): money",
                            "McMMO等级: mcmmolevel",
                            "WorldGuard区域: score",
                            "PlayerPoints点券: playerpoints",
                            "Heroes阶等: heroesclass",
                            "Heroes技能: heroesskill",
                            "魔杖（Magic）: wand",
                            "类别|职业(SkillApi): skillapiclass",
                            "等级(SkillApi): skillapilevel",
                            "任务(Quest): quest",
                        }
                    };

                    if (!isPay)
                    {
                        CardItems.Add(cardView);
                        break;
                    }

                    try
                    {
                        var success = await PayPoint(1);

                        ShowMessage(success.Text);

                        if (!success.Succese)
                        {
                            break;
                        }

                        CardItems.Add(cardView);
                    }
                    catch
                    {
                        ShowMessage("创建卡片失败！");
                    }

                    break;

                case ThumbClass.Events:

                    name = string.Empty;

                    if (string.IsNullOrEmpty(configName))
                    {
                        name = "Event_" + GetRandomString(5);

                        while (CardItems.Where(t => t.ConfigName == name).Any())
                        {
                            name = "Event_" + GetRandomString(5);
                        }
                    }
                    else
                    {
                        name = configName;
                    }

                    cardView = new AnyCardViewModel(eventProp)
                    {
                        ConfigName = name,
                        CvLeft = reCvLeft,
                        CvTop = reCvTop,
                        Type = ThumbClass.Events,
                        IsProtectName = IsProtectName,
                        AllType = new ObservableCollection<string>
                        {
                            "消息: message",
                            "命令: command",
                            "传送: teleport",
                            "点数: point",
                            "标签: tag",
                            "目标: objective",
                            "日记: journal",
                            "闪电: lightning",
                            "爆炸: explosion",
                            "给予物品: give",
                            "移除物品: take",
                            "药水效果: effect",
                            "对话: conversation",
                            "杀死玩家: kill",
                            "召唤怪物: spawn",
                            "时间: time",
                            "天气: weather",
                            "多事件组: folder",
                            "放置方块: setblock",
                            "伤害玩家: damage",
                            "组队: party",
                            "清除怪物: clear",
                            "运行事件: run",
                            "给予日记: givejournal",
                            "代发指令: sudo",
                            "箱中放置: chestgive",
                            "箱中移除: chesttake",
                            "清理箱子: chestclear",
                            "目标点: compass",
                            "删除任务: cancel",
                            "计分板: score",
                            "权限(Vault): permission",
                            "资金(Vault): money",
                            "生成神话生物(MythicMobs): mspawnmob",
                            "McMMO经验值: mcmmoexp",
                            "PlayerPoints点券: playerpoints",
                            "Heroes经验值: heroesexp",
                            "任务（Quest）: quest",
                        }
                    };

                    if (!isPay)
                    {
                        CardItems.Add(cardView);
                        break;
                    }

                    try
                    {
                        var success = await PayPoint(1);

                        ShowMessage(success.Text);

                        if (!success.Succese)
                        {
                            break;
                        }

                        CardItems.Add(cardView);
                    }
                    catch
                    {
                        ShowMessage("创建卡片失败！");
                    }

                    break;

                case ThumbClass.Objectives:

                    name = string.Empty;

                    if (string.IsNullOrEmpty(configName))
                    {
                        name = "Objective_" + GetRandomString(5);

                        while (CardItems.Where(t => t.ConfigName == name).Any())
                        {
                            name = "Objective_" + GetRandomString(5);
                        }
                    }
                    else
                    {
                        name = configName;
                    }

                    cardView = new AnyCardViewModel(objectiveProp)
                    {
                        ConfigName = name,
                        CvLeft = reCvLeft,
                        CvTop = reCvTop,
                        Type = ThumbClass.Objectives,
                        IsProtectName = IsProtectName,
                        AllType = new ObservableCollection<string>
                        {
                            "位置: location",
                            "方块: block",
                            "击杀生物: mobkill",
                            "动作: action",
                            "死亡: die",
                            "合成: craft",
                            "熔炼: smelt",
                            "驯服: tame",
                            "等待: delay",
                            "射箭: arrow",
                            "经验值: experience",
                            "踩上压力板: step",
                            "注销: logout",
                            "输入密码: password",
                            "垂钓: fish",
                            "剪羊毛: shear",
                            "附魔: enchant",
                            "装箱: chestput",
                            "炼药: potion",
                            "击杀NPC（Citizens）: npckill",
                            "与NPC互动（Citizens）: npcInteract",
                            "击杀神话生物(MythicMobs): mmobkill",
                            "WorldGuard区域: region",
                        }
                    };

                    if (!isPay)
                    {
                        CardItems.Add(cardView);
                        break;
                    }

                    try
                    {
                        var success = await PayPoint(1);

                        ShowMessage(success.Text);

                        if (!success.Succese)
                        {
                            break;
                        }

                        CardItems.Add(cardView);
                    }
                    catch
                    {
                        ShowMessage("创建卡片失败！");
                    }

                    break;

                case ThumbClass.Journal:

                    name = string.Empty;

                    if (string.IsNullOrEmpty(configName))
                    {
                        name = "Journal_" + GetRandomString(5);

                        while (CardItems.Where(t => t.ConfigName == name).Any())
                        {
                            name = "Journal_" + GetRandomString(5);
                        }
                    }
                    else
                    {
                        name = configName;
                    }

                    cardView = new CardViewModel()
                    {
                        ConfigName = name,
                        CvLeft = reCvLeft,
                        CvTop = reCvTop,
                        Type = ThumbClass.Journal,
                        IsProtectName = IsProtectName,
                        ThumbWidth = 400.00,
                        ThumbHeight = 148.00,
                    };

                    if (!isPay)
                    {
                        CardItems.Add(cardView);
                        break;
                    }

                    try
                    {
                        var success = await PayPoint(1);

                        ShowMessage(success.Text);

                        if (!success.Succese)
                        {
                            break;
                        }

                        CardItems.Add(cardView);
                    }
                    catch
                    {
                        ShowMessage("创建卡片失败！");
                    }

                    break;

                case ThumbClass.Items:

                    name = string.Empty;

                    if (string.IsNullOrEmpty(configName))
                    {
                        name = "Item_" + GetRandomString(5);

                        while (CardItems.Where(t => t.ConfigName == name).Any())
                        {
                            name = "Item_" + GetRandomString(5);
                        }
                    }
                    else
                    {
                        name = configName;
                    }

                    cardView = new CardViewModel()
                    {
                        ConfigName = name,
                        CvLeft = reCvLeft,
                        CvTop = reCvTop,
                        Type = ThumbClass.Items,
                        IsProtectName = IsProtectName,
                        ThumbWidth = 400.00,
                        ThumbHeight = 148.00,
                    };

                    if (!isPay)
                    {
                        CardItems.Add(cardView);
                        break;
                    }

                    try
                    {
                        var success = await PayPoint(1);

                        ShowMessage(success.Text);

                        if (!success.Succese)
                        {
                            break;
                        }

                        CardItems.Add(cardView);
                    }
                    catch
                    {
                        ShowMessage("创建卡片失败！");
                    }

                    break;
            }

            return (T)cardView;
        }

        public ClientWindowViewModel GetClienteViewModel()
        {
            return this;
        }

        #endregion

    }

    /// <summary>
    /// 所有卡片的基类
    /// </summary>
    public partial class CardViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ShurtcutIdeaBtnViewModel> _ShurtcutIdea = new ObservableCollection<ShurtcutIdeaBtnViewModel>();

        [ObservableProperty]
        private Visibility _IsClassify = Visibility.Hidden;

        [ObservableProperty]
        private bool _IsProtectName = false;

        [ObservableProperty]
        private bool _IsDraw = false;

        [ObservableProperty]
        public object _ThumbWidth;

        [ObservableProperty]
        public object _ThumbHeight;

        [ObservableProperty]
        private bool _IsLine = false;

        [ObservableProperty]
        public ThumbClass _Type;

        [ObservableProperty]
        public string _ConfigName = string.Empty;

        [ObservableProperty]
        public string _HelpInfo = string.Empty;

        [ObservableProperty]
        public string _ItemContent = string.Empty;

        [ObservableProperty]
        public double _CvLeft = 0.00;

        [ObservableProperty]
        public double _CvTop = 0.00;

        [ObservableProperty]
        public int _CvZIndex = 0;

        [ObservableProperty]
        public Color _ShadowColor = Brushes.Black.Color;

        [ObservableProperty]
        public CardViewModel _MainCard;

        /// <summary>
        /// 父级
        /// </summary>
        [ObservableProperty]
        public List<CardViewModel> _Left = new List<CardViewModel>();

        /// <summary>
        /// 子集
        /// </summary>
        [ObservableProperty]
        public List<CardViewModel> _Right = new List<CardViewModel>();

        private Point thumbLestPoint = new Point();

        [RelayCommand()]
        public async Task HelpTool(Button btn)
        {
            HelpToolSettingWindow window = new HelpToolSettingWindow();

            var isHaveTool = string.IsNullOrEmpty(HelpInfo);

            if (isHaveTool)
            {
                window.DataContext = new HelpToolSetWindowViewModel(HelpInfo);
            }
            else
            {
                window.DataContext = new HelpToolSetWindowViewModel("该命令还没有帮助提示");
            }

            window.ShowDialog();

            btn.ToolTip = window.Tag as string;

            HelpInfo = btn.ToolTip as string;
        }

        [RelayCommand()]
        public async Task ThumbLoaded(Thumb thumb)
        {
            if (!IsDraw && !IsLine)
            {
                thumb.PreviewMouseLeftButtonUp += Thumb_PreviewMouseLeftButtonUp;
                thumb.PreviewMouseLeftButtonDown += Thumb_PreviewMouseLeftButtonDown;
                thumb.DragCompleted += Thumb_DragCompleted;
                thumb.DragDelta += Thumb_DragDelta;
                thumb.DragStarted += Thumb_DragStarted;
            }
        }

        /// <summary>
        /// 控件拖拽开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            var nowThumb = (Thumb)sender;
            var nowThumbContent = ((Thumb)sender).DataContext as CardViewModel;

            thumbLestPoint.X = nowThumbContent.CvLeft;
            thumbLestPoint.Y = nowThumbContent.CvTop;

            var getSelectCard = GetSelecteCardDel();

            if (getSelectCard.Count > 0)
            {
                return;
            }

            IsClassify = Visibility.Visible;
        }

        /// <summary>
        /// 控件拖拽过程中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var getSelectCard = GetSelecteCardDel();
            if (getSelectCard.Count > 0)
            {
                foreach (var i in getSelectCard)
                {
                    double nTop = i.CvTop + e.VerticalChange;

                    double nLeft = i.CvLeft + e.HorizontalChange;

                    if (nTop <= 0)
                        nTop = 0;
                    if (nTop >= (504400 - (double)i.ThumbHeight))
                        nTop = 504400 - (double)i.ThumbHeight;
                    if (nLeft <= 0)
                        nLeft = 0;
                    if (nLeft >= (1267800 - (double)i.ThumbWidth))
                        nLeft = 1267800 - (double)i.ThumbWidth;

                    i.CvLeft = nLeft;

                    i.CvTop = nTop;

                    var nowThumbContent = i;

                    //链接线的处理
                    try
                    {
                        var getCardInfo = ClientWindowViewModel.GetAllCardDel().Where(t => t.IsLine == true).ToList();

                        var realCardIndo = new List<LineCardViewModel>();

                        foreach (var card in getCardInfo)
                        {
                            realCardIndo.Add(card as LineCardViewModel);
                        }

                        var getNeedChangeLeftLine = realCardIndo.Where(t => t.LineLeft == nowThumbContent).ToList();

                        var getNeedChangeRightLine = realCardIndo.Where(t => t.LineRight == nowThumbContent).ToList();

                        foreach (var item in getNeedChangeLeftLine)
                        {
                            item.CvLeft = item.LineLeft.CvLeft + (double)item.LineLeft.ThumbWidth / 2;

                            item.CvTop = item.LineLeft.CvTop + (double)item.LineLeft.ThumbHeight / 2;

                            item.X = -((nowThumbContent.CvLeft - item.LineRight.CvLeft) - Math.Abs((double)item.LineLeft.ThumbWidth - (double)item.LineRight.ThumbWidth) / 2);

                            item.Y = -((nowThumbContent.CvTop - item.LineRight.CvTop) - Math.Abs((double)item.LineLeft.ThumbHeight - (double)item.LineRight.ThumbHeight) / 2);

                        }

                        foreach (var item in getNeedChangeRightLine)
                        {
                            item.CvLeft = item.LineLeft.CvLeft + (double)item.LineLeft.ThumbWidth / 2;

                            item.CvTop = item.LineLeft.CvTop + (double)item.LineLeft.ThumbHeight / 2;

                            item.X = -((item.LineLeft.CvLeft - item.LineRight.CvLeft) - Math.Abs((double)item.LineLeft.ThumbWidth - (double)item.LineRight.ThumbWidth) / 2);

                            item.Y = -((item.LineLeft.CvTop - item.LineRight.CvTop) - Math.Abs((double)item.LineLeft.ThumbHeight - (double)item.LineRight.ThumbHeight) / 2);

                        }
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                var nowThumb = (Thumb)sender;

                var nowThumbContent = ((Thumb)sender).DataContext as CardViewModel;

                double nTop = nowThumbContent.CvTop + e.VerticalChange;

                double nLeft = nowThumbContent.CvLeft + e.HorizontalChange;

                if (nTop <= 0)
                    nTop = 0;
                if (nTop >= (504400 - nowThumb.ActualHeight))
                    nTop = 504400 - nowThumb.ActualHeight;
                if (nLeft <= 0)
                    nLeft = 0;
                if (nLeft >= (1267800 - nowThumb.ActualWidth))
                    nLeft = 1267800 - nowThumb.ActualWidth;

                nowThumbContent.CvLeft = nLeft;

                nowThumbContent.CvTop = nTop;

                //链接线的处理
                try
                {
                    var getCardInfo = ClientWindowViewModel.GetAllCardDel().Where(t => t.IsLine == true).ToList();

                    var realCardIndo = new List<LineCardViewModel>();

                    foreach (var card in getCardInfo)
                    {
                        realCardIndo.Add(card as LineCardViewModel);
                    }

                    var getNeedChangeLeftLine = realCardIndo.Where(t => t.LineLeft == nowThumbContent).ToList();

                    var getNeedChangeRightLine = realCardIndo.Where(t => t.LineRight == nowThumbContent).ToList();

                    foreach (var item in getNeedChangeLeftLine)
                    {
                        item.CvLeft = item.LineLeft.CvLeft + (double)item.LineLeft.ThumbWidth / 2;

                        item.CvTop = item.LineLeft.CvTop + (double)item.LineLeft.ThumbHeight / 2;

                        item.X = -((nowThumbContent.CvLeft - item.LineRight.CvLeft) - Math.Abs((double)item.LineLeft.ThumbWidth - (double)item.LineRight.ThumbWidth) / 2);

                        item.Y = -((nowThumbContent.CvTop - item.LineRight.CvTop) - Math.Abs((double)item.LineLeft.ThumbHeight - (double)item.LineRight.ThumbHeight) / 2);

                    }

                    foreach (var item in getNeedChangeRightLine)
                    {
                        item.CvLeft = item.LineLeft.CvLeft + (double)item.LineLeft.ThumbWidth / 2;

                        item.CvTop = item.LineLeft.CvTop + (double)item.LineLeft.ThumbHeight / 2;

                        item.X = -((item.LineLeft.CvLeft - item.LineRight.CvLeft) - Math.Abs((double)item.LineLeft.ThumbWidth - (double)item.LineRight.ThumbWidth) / 2);

                        item.Y = -((item.LineLeft.CvTop - item.LineRight.CvTop)-Math.Abs((double)item.LineLeft.ThumbHeight - (double)item.LineRight.ThumbHeight) /2);

                    }
                }
                catch
                {

                }

                var getBack = await ThumbClassification(nowThumbContent);

                if (!getBack.Succese)
                {
                    return;
                }

                var getChange = getBack.Backs as CardViewModel;

                getChange.IsClassify = Visibility.Visible;
            }

            
        }

        /// <summary>
        /// 控件停止拖拽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var getSelectCard = GetSelecteCardDel();

            if (getSelectCard.Count > 0)
            {
                return;
            }

            var nowThumb = (Thumb)sender;
            var nowThumbContent = ((Thumb)sender).DataContext as CardViewModel;

            try
            {
                var getCardInfo = ClientWindowViewModel.GetAllCardDel().Where(t => t.IsLine == true).ToList();

                var realCardIndo = new List<LineCardViewModel>();

                foreach (var card in getCardInfo)
                {
                    realCardIndo.Add(card as LineCardViewModel);
                }

                var getNeedChangeLeftLine = realCardIndo.Where(t => t.LineLeft == nowThumbContent).ToList();

                var getNeedChangeRightLine = realCardIndo.Where(t => t.LineRight == nowThumbContent).ToList();

                foreach (var item in getNeedChangeLeftLine)
                {
                    item.CvLeft = item.LineLeft.CvLeft + (double)item.LineLeft.ThumbWidth / 2;

                    item.CvTop = item.LineLeft.CvTop + (double)item.LineLeft.ThumbHeight / 2;

                    item.X = -((nowThumbContent.CvLeft - item.LineRight.CvLeft) - Math.Abs((double)item.LineLeft.ThumbWidth - (double)item.LineRight.ThumbWidth) / 2);

                    item.Y = -((nowThumbContent.CvTop - item.LineRight.CvTop) - Math.Abs((double)item.LineLeft.ThumbHeight - (double)item.LineRight.ThumbHeight) / 2);

                }

                foreach (var item in getNeedChangeRightLine)
                {
                    item.CvLeft = item.LineLeft.CvLeft + (double)item.LineLeft.ThumbWidth / 2;

                    item.CvTop = item.LineLeft.CvTop + (double)item.LineLeft.ThumbHeight / 2;

                    item.X = -((item.LineLeft.CvLeft - item.LineRight.CvLeft) - Math.Abs((double)item.LineLeft.ThumbWidth - (double)item.LineRight.ThumbWidth) / 2);

                    item.Y = -((item.LineLeft.CvTop - item.LineRight.CvTop) - Math.Abs((double)item.LineLeft.ThumbHeight - (double)item.LineRight.ThumbHeight) / 2);

                }

                var getNeedCardInfo = ClientWindowViewModel.GetAllCardDel().Where(t => !t.IsLine && !t.IsDraw && t.IsClassify == Visibility.Visible).ToList();

                foreach (var item in getNeedCardInfo)
                {
                    item.IsClassify = Visibility.Hidden;
                }
            }
            catch
            {

            }

            var getBack = await ThumbClassification(nowThumbContent);

            if (!getBack.Succese)
            {
                return;
            }

            nowThumbContent.CvLeft = thumbLestPoint.X;
            nowThumbContent.CvTop = thumbLestPoint.Y;

            var fatherCard = getBack.Backs as CardViewModel;

            if (fatherCard.Right.Contains(nowThumbContent)|| nowThumbContent.Left.Contains(fatherCard))
            {
                if (MessageBox.Show("确定要将其取消归类？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var allCard = GetAllCardDel();

                    var getLines = allCard.Where(t => t.IsLine).ToList();

                    var getRealLine = new List<LineCardViewModel>();

                    foreach (var item in getLines)
                    {
                        getRealLine.Add(item as LineCardViewModel);
                    }

                    var getNeedLine = getRealLine.Where(t => (t.LineRight == nowThumbContent || t.LineLeft == nowThumbContent)
                    && (t.LineRight == fatherCard || t.LineLeft == fatherCard)).FirstOrDefault();

                    if(getNeedLine != null)
                    {
                        allCard.Remove(getNeedLine);
                    }

                    nowThumbContent.Left.Remove(fatherCard);
                    nowThumbContent.Right.Remove(fatherCard);

                    fatherCard.Left.Remove(nowThumbContent);
                    fatherCard.Right.Remove(nowThumbContent);

                    #region 信息清空
                    {
                        if (nowThumbContent.Type == ThumbClass.NPC || nowThumbContent.Type == ThumbClass.Player)
                        {
                            if (!ConversationCardChanged.saveAllValue.ContainsKey(nowThumbContent as AnyCardViewModel))
                            {
                                return;
                            }

                            foreach (var i in ConversationCardChanged.saveAllValue[nowThumbContent as AnyCardViewModel])
                            {
                                foreach (var j in i.Value)
                                {
                                    foreach (var m in j.Value)
                                    {
                                        foreach (var n in m.Value)
                                        {
                                            if (n.Value == fatherCard.ConfigName)
                                            {
                                                ConversationCardChanged.saveAllValue[nowThumbContent as AnyCardViewModel][i.Key][j.Key][m.Key][n.Key] = string.Empty;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (nowThumbContent.Type == ThumbClass.Conditions)
                        {
                            if (!ConditionsCardChanged.saveAllValue.ContainsKey(nowThumbContent as AnyCardViewModel))
                            {
                                return;
                            }

                            foreach (var i in ConditionsCardChanged.saveAllValue[nowThumbContent as AnyCardViewModel])
                            {
                                foreach (var j in i.Value)
                                {
                                    foreach (var m in j.Value)
                                    {
                                        foreach (var n in m.Value)
                                        {
                                            if (n.Value == fatherCard.ConfigName)
                                            {
                                                ConditionsCardChanged.saveAllValue[nowThumbContent as AnyCardViewModel][i.Key][j.Key][m.Key][n.Key] = string.Empty;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (nowThumbContent.Type == ThumbClass.Events)
                        {
                            if (!EventCardChanged.saveAllValue.ContainsKey(nowThumbContent as AnyCardViewModel))
                            {
                                return;
                            }

                            foreach (var i in EventCardChanged.saveAllValue[nowThumbContent as AnyCardViewModel])
                            {
                                foreach (var j in i.Value)
                                {
                                    foreach (var m in j.Value)
                                    {
                                        foreach (var n in m.Value)
                                        {
                                            if (n.Value == fatherCard.ConfigName)
                                            {
                                                EventCardChanged.saveAllValue[nowThumbContent as AnyCardViewModel][i.Key][j.Key][m.Key][n.Key] = string.Empty;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (nowThumbContent.Type == ThumbClass.Objectives)
                        {
                            if (!ObjectiveCardChanged.saveAllValue.ContainsKey(nowThumbContent as AnyCardViewModel))
                            {
                                return;
                            }

                            foreach (var i in ObjectiveCardChanged.saveAllValue[nowThumbContent as AnyCardViewModel])
                            {
                                foreach (var j in i.Value)
                                {
                                    foreach (var m in j.Value)
                                    {
                                        foreach (var n in m.Value)
                                        {
                                            if (n.Value == fatherCard.ConfigName)
                                            {
                                                ObjectiveCardChanged.saveAllValue[nowThumbContent as AnyCardViewModel][i.Key][j.Key][m.Key][n.Key] = string.Empty;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    {
                        if (fatherCard.Type == ThumbClass.NPC || fatherCard.Type == ThumbClass.Player)
                        {
                            if (!ConversationCardChanged.saveAllValue.ContainsKey(fatherCard as AnyCardViewModel))
                            {
                                return;
                            }

                            foreach (var i in ConversationCardChanged.saveAllValue[fatherCard as AnyCardViewModel])
                            {
                                foreach (var j in i.Value)
                                {
                                    foreach (var m in j.Value)
                                    {
                                        foreach (var n in m.Value)
                                        {
                                            if (n.Value == nowThumbContent.ConfigName)
                                            {
                                                ConversationCardChanged.saveAllValue[fatherCard as AnyCardViewModel][i.Key][j.Key][m.Key][n.Key] = string.Empty;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (fatherCard.Type == ThumbClass.Conditions)
                        {
                            if (!ConditionsCardChanged.saveAllValue.ContainsKey(fatherCard as AnyCardViewModel))
                            {
                                return;
                            }

                            foreach (var i in ConditionsCardChanged.saveAllValue[fatherCard as AnyCardViewModel])
                            {
                                foreach (var j in i.Value)
                                {
                                    foreach (var m in j.Value)
                                    {
                                        foreach (var n in m.Value)
                                        {
                                            if (n.Value == nowThumbContent.ConfigName)
                                            {
                                                ConditionsCardChanged.saveAllValue[fatherCard as AnyCardViewModel][i.Key][j.Key][m.Key][n.Key] = string.Empty;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (fatherCard.Type == ThumbClass.Events)
                        {
                            if (!EventCardChanged.saveAllValue.ContainsKey(fatherCard as AnyCardViewModel))
                            {
                                return;
                            }

                            foreach (var i in EventCardChanged.saveAllValue[fatherCard as AnyCardViewModel])
                            {
                                foreach (var j in i.Value)
                                {
                                    foreach (var m in j.Value)
                                    {
                                        foreach (var n in m.Value)
                                        {
                                            if (n.Value == nowThumbContent.ConfigName)
                                            {
                                                EventCardChanged.saveAllValue[fatherCard as AnyCardViewModel][i.Key][j.Key][m.Key][n.Key] = string.Empty;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (fatherCard.Type == ThumbClass.Objectives)
                        {
                            if (!ObjectiveCardChanged.saveAllValue.ContainsKey(fatherCard as AnyCardViewModel))
                            {
                                return;
                            }

                            foreach (var i in ObjectiveCardChanged.saveAllValue[fatherCard as AnyCardViewModel])
                            {
                                foreach (var j in i.Value)
                                {
                                    foreach (var m in j.Value)
                                    {
                                        foreach (var n in m.Value)
                                        {
                                            if (n.Value == nowThumbContent.ConfigName)
                                            {
                                                ObjectiveCardChanged.saveAllValue[fatherCard as AnyCardViewModel][i.Key][j.Key][m.Key][n.Key] = string.Empty;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion


                }
                else
                {
                    ClientWindowViewModel.ShowMessageDel("终止操作");
                }

                return;
            }

            if (MessageBox.Show("确定要将其归类？","提示",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                //基础判断能否进行归类处理
                var back = await CardCanBeClassify(fatherCard, nowThumbContent);

                if (!back.Succese)
                {
                    ClientWindowViewModel.ShowMessageDel("归类失败！不允许这样的归类");
                    return;
                }

                //开始查询高级归类

                if(fatherCard.Type == ThumbClass.Conditions)
                {
                    var realFatherCard = fatherCard as AnyCardViewModel;

                    var getRealoFatherCmd = TxtSplit(realFatherCard.SelectType, ": ")[1];

                    var getFatherModel = contisionProp.Where(t => t.MainClass == getRealoFatherCmd).FirstOrDefault();

                    var realChildCard = nowThumbContent as AnyCardViewModel;

                    if (TxtSplit(realChildCard.SelectType, ": ").Count != 2)
                    {
                        ClientWindowViewModel.ShowMessageDel("终止操作,子卡片没有选择类型");

                        return;
                    }

                    var getRealChildCmd = TxtSplit(realChildCard.SelectType, ": ")[1];

                    var getChildModel = contisionProp.Where(t => t.MainClass == getRealChildCmd).FirstOrDefault();

                    var newList = new List<string>();

                    var newDic = new Dictionary<string, List<string>>();

                    foreach (var fatherModel in getFatherModel.NeedTpye)
                    {
                        foreach (var item in fatherModel.Value)
                        {
                            if(item.Value == nowThumbContent.Type)
                            {
                                if (!newList.Contains(fatherModel.Key))
                                {
                                    newList.Add(fatherModel.Key);
                                }
                                
                                if (!newDic.ContainsKey(fatherModel.Key))
                                {
                                    newDic.Add(fatherModel.Key, new List<string>
                                    {
                                        $"第 {item.Key+1} 条参数"
                                    });
                                }
                                else
                                {
                                    newDic[fatherModel.Key].Add($"第 {item.Key + 1} 条参数");
                                }
                                
                            }
                        }
                    }

                    ThumbSetWindow thumbSetWindow = new ThumbSetWindow();

                    if (nowThumbContent.Type == ThumbClass.Items)
                    {
                        thumbSetWindow.DataContext = new ThumbSetWindowViewModel()
                        {
                            IsEnabel = false,//用于条件取反

                            IsNegate = false,//条件取反是否选中

                            UseItem = true,//用于规定物品的数量

                            Classifications = newList,

                            SaveTerms = newDic

                        };

                    }

                    else if(nowThumbContent.Type == ThumbClass.Conditions)
                    {
                        if (getChildModel.isContisionCmd)
                        {
                            thumbSetWindow.DataContext = new ThumbSetWindowViewModel()
                            {
                                IsEnabel = true,//用于条件取反

                                IsNegate = false,//条件取反是否选中

                                UseItem = false,//用于规定物品的数量

                                Classifications = newList,

                                SaveTerms = newDic

                            };
                        }
                        else
                        {
                            thumbSetWindow.DataContext = new ThumbSetWindowViewModel()
                            {
                                IsEnabel = false,//用于条件取反

                                IsNegate = false,//条件取反是否选中

                                UseItem = false,//用于规定物品的数量

                                Classifications = newList,

                                SaveTerms = newDic

                            };
                        }
                    }
                    else
                    {
                        thumbSetWindow.DataContext = new ThumbSetWindowViewModel()
                        {
                            IsEnabel = false,//用于条件取反

                            IsNegate = false,//条件取反是否选中

                            UseItem = false,//用于规定物品的数量

                            Classifications = newList,

                            SaveTerms = newDic

                        };
                    }

                    while (thumbSetWindow.Tag == null)
                    {
                        thumbSetWindow.ShowDialog();
                    }

                    if (!(bool)thumbSetWindow.Tag)
                    {
                        ClientWindowViewModel.ShowMessageDel("终止操作");
                        return;
                    }

                    var childName = nowThumbContent.ConfigName;

                    if((thumbSetWindow.DataContext as ThumbSetWindowViewModel).IsEnabel&& (thumbSetWindow.DataContext as ThumbSetWindowViewModel).IsNegate)
                    {
                        childName = "!" + childName;
                    }
                    else if((thumbSetWindow.DataContext as ThumbSetWindowViewModel).UseItem)
                    {
                        if(string.IsNullOrEmpty((thumbSetWindow.DataContext as ThumbSetWindowViewModel).ItemNum))
                        {
                            childName += ":" + (thumbSetWindow.DataContext as ThumbSetWindowViewModel).ItemNum;
                        }
                        
                    }

                    var getCmd = (thumbSetWindow.DataContext as ThumbSetWindowViewModel).ClassificationsSeleted;

                    var getPar = (thumbSetWindow.DataContext as ThumbSetWindowViewModel).TermsSeleted;

                    if(getCmd == getRealoFatherCmd)//是主命令
                    {
                        var getNum = TxtSplit(getPar, " ");

                        if (getNum.Count != 3)
                        {
                            ClientWindowViewModel.ShowMessageDel("归类失败，出现了意外的情况！");

                            return;
                        }

                        if (Convert.ToInt32(getNum[1]) == 1)//主参数
                        {
                            if (getFatherModel.TextNum != -1 && getFatherModel.TextNum != 1)
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，这是不允许的归类情况！");

                                return;
                            }

                            if(getFatherModel.TextNum == -1)
                            {
                                if(!ConditionsCardChanged.saveAllValue.TryGetValue(realFatherCard,out var val))
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                                try
                                {
                                    if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                        return;
                                    }

                                    if(string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                    {
                                        val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                    }
                                    else
                                    {
                                        var getnowNum = val[realFatherCard.SelectType][getCmd][getPar];

                                        var nowNum = 0;

                                        foreach (var i in getnowNum)
                                        {
                                            var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                            if (fg.Length != 3)
                                            {
                                                ClientWindowViewModel.ShowMessageDel("归类失败，存储数据异常！");

                                                return;
                                            }

                                            var num = Convert.ToInt32(fg[1]);

                                            if (num > nowNum)
                                            {
                                                nowNum = num;
                                            }
                                        }

                                        val[realFatherCard.SelectType][getCmd][getPar].Add($"第 {nowNum + 1} 项", childName);

                                        realFatherCard.AllItem.Add($"第 {nowNum + 1} 项");
                                    }
                                }
                                catch
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                            }
                            else if(getFatherModel.TextNum == 1)
                            {
                                if (!ConditionsCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }

                                try
                                {
                                    if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                        return;
                                    }

                                    if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                    {
                                        val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                    }
                                    else
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片只能有一个子卡片，如果想要替换请先取消原卡片的归类！");

                                        return;
                                    }
                                }
                                catch
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                            }
                        }
                        else
                        {
                            var getRealNum = Convert.ToInt32(getNum[1]) - 2;

                            var getSplitWord = getFatherModel.TextSplitWords[getRealNum].j;

                            if (getSplitWord != -1 && getSplitWord != 1)
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，这是不允许的归类情况！");

                                return;
                            }

                            if (getSplitWord == -1)
                            {
                                if (!ConditionsCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                                try
                                {
                                    if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                        return;
                                    }

                                    if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                    {
                                        val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                    }
                                    else
                                    {
                                        var getnowNum = val[realFatherCard.SelectType][getCmd][getPar];

                                        var nowNum = 0;

                                        foreach (var i in getnowNum)
                                        {
                                            var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                            if (fg.Length != 3)
                                            {
                                                ClientWindowViewModel.ShowMessageDel("归类失败，存储数据异常！");

                                                return;
                                            }

                                            var num = Convert.ToInt32(fg[1]);

                                            if (num > nowNum)
                                            {
                                                nowNum = num;
                                            }
                                        }

                                        val[realFatherCard.SelectType][getCmd][getPar].Add($"第 {nowNum + 1} 项", childName);

                                        realFatherCard.AllItem.Add($"第 {nowNum + 1} 项");
                                    }
                                }
                                catch
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                            }
                            else if (getSplitWord == 1)
                            {
                                if (!ConditionsCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }

                                try
                                {
                                    if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                        return;
                                    }

                                    if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                    {
                                        val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                    }
                                    else
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片只能有一个子卡片，如果想要替换请先取消原卡片的归类！");

                                        return;
                                    }
                                }
                                catch
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                            }
                        }
                    }
                    else if(getFatherModel.ChildClasses.Where(t=>t.ChildClass == getCmd).Any())//是子命令
                    {
                        if(getPar!="第 1 条参数")
                        {
                            ClientWindowViewModel.ShowMessageDel("归类失败，出现了意外的情况！");

                            return;
                        }

                        var getFatherModelChildClass = getFatherModel.ChildClasses.Where(t => t.ChildClass == getCmd).FirstOrDefault();

                        var getSplitWord = getFatherModelChildClass.ChildTextNum;

                        if (getSplitWord != -1 && getSplitWord != 1)
                        {
                            ClientWindowViewModel.ShowMessageDel("归类失败，这是不允许的归类情况！");

                            return;
                        }

                        if (getSplitWord == -1)
                        {
                            if (!ConditionsCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                return;
                            }
                            try
                            {
                                if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                    return;
                                }

                                if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                {
                                    val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                }
                                else
                                {
                                    var getnowNum = val[realFatherCard.SelectType][getCmd][getPar];

                                    var nowNum = 0;

                                    foreach (var i in getnowNum)
                                    {
                                        var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                        if (fg.Length != 3)
                                        {
                                            ClientWindowViewModel.ShowMessageDel("归类失败，存储数据异常！");

                                            return;
                                        }

                                        var num = Convert.ToInt32(fg[1]);

                                        if (num > nowNum)
                                        {
                                            nowNum = num;
                                        }
                                    }

                                    val[realFatherCard.SelectType][getCmd][getPar].Add($"第 {nowNum + 1} 项", childName);

                                    realFatherCard.AllItem.Add($"第 {nowNum + 1} 项");
                                }
                            }
                            catch
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                return;
                            }
                        }
                        else if (getSplitWord == 1)
                        {
                            if (!ConditionsCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                return;
                            }

                            try
                            {
                                if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                    return;
                                }

                                if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                {
                                    val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                }
                                else
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片只能有一个子卡片，如果想要替换请先取消原卡片的归类！");

                                    return;
                                }
                            }
                            catch
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                return;
                            }
                        }
                    }
                }
                else if(fatherCard.Type == ThumbClass.Events)
                {
                    var realFatherCard = fatherCard as AnyCardViewModel;

                    var getRealoFatherCmd = TxtSplit(realFatherCard.SelectType, ": ")[1];

                    var getFatherModel = eventProp.Where(t => t.MainClass == getRealoFatherCmd).FirstOrDefault();

                    var realChildCard = nowThumbContent as AnyCardViewModel;

                    if (TxtSplit(realChildCard.SelectType, ": ").Count != 2)
                    {
                        ClientWindowViewModel.ShowMessageDel("终止操作,子卡片没有选择类型");

                        return;
                    }

                    var getRealChildCmd = TxtSplit(realChildCard.SelectType, ": ")[1];

                    var getChildModel = contisionProp.Where(t => t.MainClass == getRealChildCmd).FirstOrDefault();

                    var newList = new List<string>();

                    var newDic = new Dictionary<string, List<string>>();

                    foreach (var fatherModel in getFatherModel.NeedTpye)
                    {
                        foreach (var item in fatherModel.Value)
                        {
                            if (item.Value == nowThumbContent.Type)
                            {
                                if (!newList.Contains(fatherModel.Key))
                                {
                                    newList.Add(fatherModel.Key);
                                }

                                if (!newDic.ContainsKey(fatherModel.Key))
                                {
                                    newDic.Add(fatherModel.Key, new List<string>
                                    {
                                        $"第 {item.Key+1} 条参数"
                                    });
                                }
                                else
                                {
                                    newDic[fatherModel.Key].Add($"第 {item.Key + 1} 条参数");
                                }

                            }
                        }
                    }

                    ThumbSetWindow thumbSetWindow = new ThumbSetWindow();

                    if (nowThumbContent.Type == ThumbClass.Items)
                    {
                        thumbSetWindow.DataContext = new ThumbSetWindowViewModel()
                        {
                            IsEnabel = false,//用于条件取反

                            IsNegate = false,//条件取反是否选中

                            UseItem = true,//用于规定物品的数量

                            Classifications = newList,

                            SaveTerms = newDic

                        };

                    }

                    else if (nowThumbContent.Type == ThumbClass.Conditions)
                    {
                        if (getChildModel.isContisionCmd)
                        {
                            thumbSetWindow.DataContext = new ThumbSetWindowViewModel()
                            {
                                IsEnabel = true,//用于条件取反

                                IsNegate = false,//条件取反是否选中

                                UseItem = false,//用于规定物品的数量

                                Classifications = newList,

                                SaveTerms = newDic

                            };
                        }
                        else
                        {
                            thumbSetWindow.DataContext = new ThumbSetWindowViewModel()
                            {
                                IsEnabel = false,//用于条件取反

                                IsNegate = false,//条件取反是否选中

                                UseItem = false,//用于规定物品的数量

                                Classifications = newList,

                                SaveTerms = newDic

                            };
                        }
                    }
                    else
                    {
                        thumbSetWindow.DataContext = new ThumbSetWindowViewModel()
                        {
                            IsEnabel = false,//用于条件取反

                            IsNegate = false,//条件取反是否选中

                            UseItem = false,//用于规定物品的数量

                            Classifications = newList,

                            SaveTerms = newDic

                        };
                    }

                    while (thumbSetWindow.Tag == null)
                    {
                        thumbSetWindow.ShowDialog();
                    }

                    if (!(bool)thumbSetWindow.Tag)
                    {
                        ClientWindowViewModel.ShowMessageDel("终止操作");
                        return;
                    }

                    var childName = nowThumbContent.ConfigName;

                    if ((thumbSetWindow.DataContext as ThumbSetWindowViewModel).IsEnabel && (thumbSetWindow.DataContext as ThumbSetWindowViewModel).IsNegate)
                    {
                        childName = "!" + childName;
                    }
                    else if ((thumbSetWindow.DataContext as ThumbSetWindowViewModel).UseItem)
                    {
                        if (string.IsNullOrEmpty((thumbSetWindow.DataContext as ThumbSetWindowViewModel).ItemNum))
                        {
                            childName += ":" + (thumbSetWindow.DataContext as ThumbSetWindowViewModel).ItemNum;
                        }

                    }

                    var getCmd = (thumbSetWindow.DataContext as ThumbSetWindowViewModel).ClassificationsSeleted;

                    var getPar = (thumbSetWindow.DataContext as ThumbSetWindowViewModel).TermsSeleted;

                    if (getCmd == getRealoFatherCmd)//是主命令
                    {
                        var getNum = TxtSplit(getPar, " ");

                        if (getNum.Count != 3)
                        {
                            ClientWindowViewModel.ShowMessageDel("归类失败，出现了意外的情况！");

                            return;
                        }

                        if (Convert.ToInt32(getNum[1]) == 1)//主参数
                        {
                            if (getFatherModel.TextNum != -1 && getFatherModel.TextNum != 1)
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，这是不允许的归类情况！");

                                return;
                            }

                            if (getFatherModel.TextNum == -1)
                            {
                                if (!EventCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                                try
                                {
                                    if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                        return;
                                    }

                                    if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                    {
                                        val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                    }
                                    else
                                    {
                                        var getnowNum = val[realFatherCard.SelectType][getCmd][getPar];

                                        var nowNum = 0;

                                        foreach (var i in getnowNum)
                                        {
                                            var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                            if (fg.Length != 3)
                                            {
                                                ClientWindowViewModel.ShowMessageDel("归类失败，存储数据异常！");

                                                return;
                                            }

                                            var num = Convert.ToInt32(fg[1]);

                                            if (num > nowNum)
                                            {
                                                nowNum = num;
                                            }
                                        }

                                        val[realFatherCard.SelectType][getCmd][getPar].Add($"第 {nowNum + 1} 项", childName);

                                        realFatherCard.AllItem.Add($"第 {nowNum + 1} 项");
                                    }
                                }
                                catch
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                            }
                            else if (getFatherModel.TextNum == 1)
                            {
                                if (!EventCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }

                                try
                                {
                                    if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                        return;
                                    }

                                    if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                    {
                                        val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                    }
                                    else
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片只能有一个子卡片，如果想要替换请先取消原卡片的归类！");

                                        return;
                                    }
                                }
                                catch
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                            }
                        }
                        else
                        {
                            var getRealNum = Convert.ToInt32(getNum[1]) - 2;

                            var getSplitWord = getFatherModel.TextSplitWords[getRealNum].j;

                            if (getSplitWord != -1 && getSplitWord != 1)
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，这是不允许的归类情况！");

                                return;
                            }

                            if (getSplitWord == -1)
                            {
                                if (!EventCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                                try
                                {
                                    if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                        return;
                                    }

                                    if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                    {
                                        val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                    }
                                    else
                                    {
                                        var getnowNum = val[realFatherCard.SelectType][getCmd][getPar];

                                        var nowNum = 0;

                                        foreach (var i in getnowNum)
                                        {
                                            var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                            if (fg.Length != 3)
                                            {
                                                ClientWindowViewModel.ShowMessageDel("归类失败，存储数据异常！");

                                                return;
                                            }

                                            var num = Convert.ToInt32(fg[1]);

                                            if (num > nowNum)
                                            {
                                                nowNum = num;
                                            }
                                        }

                                        val[realFatherCard.SelectType][getCmd][getPar].Add($"第 {nowNum + 1} 项", childName);

                                        realFatherCard.AllItem.Add($"第 {nowNum + 1} 项");
                                    }
                                }
                                catch
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                            }
                            else if (getSplitWord == 1)
                            {
                                if (!EventCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }

                                try
                                {
                                    if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                        return;
                                    }

                                    if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                    {
                                        val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                    }
                                    else
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片只能有一个子卡片，如果想要替换请先取消原卡片的归类！");

                                        return;
                                    }
                                }
                                catch
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                            }
                        }
                    }
                    else if (getFatherModel.ChildClasses.Where(t => t.ChildClass == getCmd).Any())//是子命令
                    {
                        if (getPar != "第 1 条参数")
                        {
                            ClientWindowViewModel.ShowMessageDel("归类失败，出现了意外的情况！");

                            return;
                        }

                        var getFatherModelChildClass = getFatherModel.ChildClasses.Where(t => t.ChildClass == getCmd).FirstOrDefault();

                        var getSplitWord = getFatherModelChildClass.ChildTextNum;

                        if (getSplitWord != -1 && getSplitWord != 1)
                        {
                            ClientWindowViewModel.ShowMessageDel("归类失败，这是不允许的归类情况！");

                            return;
                        }

                        if (getSplitWord == -1)
                        {
                            if (!EventCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                return;
                            }
                            try
                            {
                                if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                    return;
                                }

                                if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                {
                                    val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                }
                                else
                                {
                                    var getnowNum = val[realFatherCard.SelectType][getCmd][getPar];

                                    var nowNum = 0;

                                    foreach (var i in getnowNum)
                                    {
                                        var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                        if (fg.Length != 3)
                                        {
                                            ClientWindowViewModel.ShowMessageDel("归类失败，存储数据异常！");

                                            return;
                                        }

                                        var num = Convert.ToInt32(fg[1]);

                                        if (num > nowNum)
                                        {
                                            nowNum = num;
                                        }
                                    }

                                    val[realFatherCard.SelectType][getCmd][getPar].Add($"第 {nowNum + 1} 项", childName);

                                    realFatherCard.AllItem.Add($"第 {nowNum + 1} 项");
                                }
                            }
                            catch
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                return;
                            }
                        }
                        else if (getSplitWord == 1)
                        {
                            if (!EventCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                return;
                            }

                            try
                            {
                                if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                    return;
                                }

                                if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                {
                                    val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                }
                                else
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片只能有一个子卡片，如果想要替换请先取消原卡片的归类！");

                                    return;
                                }
                            }
                            catch
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                return;
                            }
                        }
                    }
                }
                else if (fatherCard.Type == ThumbClass.Objectives)
                {
                    var realFatherCard = fatherCard as AnyCardViewModel;

                    var getRealoFatherCmd = TxtSplit(realFatherCard.SelectType, ": ")[1];

                    var getFatherModel = objectiveProp.Where(t => t.MainClass == getRealoFatherCmd).FirstOrDefault();

                    var realChildCard = nowThumbContent as AnyCardViewModel;

                    if (TxtSplit(realChildCard.SelectType, ": ").Count != 2)
                    {
                        ClientWindowViewModel.ShowMessageDel("终止操作,子卡片没有选择类型");

                        return;
                    }

                    var getRealChildCmd = TxtSplit(realChildCard.SelectType, ": ")[1];

                    var getChildModel = contisionProp.Where(t => t.MainClass == getRealChildCmd).FirstOrDefault();

                    var newList = new List<string>();

                    var newDic = new Dictionary<string, List<string>>();

                    foreach (var fatherModel in getFatherModel.NeedTpye)
                    {
                        foreach (var item in fatherModel.Value)
                        {
                            if (item.Value == nowThumbContent.Type)
                            {
                                if (!newList.Contains(fatherModel.Key))
                                {
                                    newList.Add(fatherModel.Key);
                                }

                                if (!newDic.ContainsKey(fatherModel.Key))
                                {
                                    newDic.Add(fatherModel.Key, new List<string>
                                    {
                                        $"第 {item.Key+1} 条参数"
                                    });
                                }
                                else
                                {
                                    newDic[fatherModel.Key].Add($"第 {item.Key + 1} 条参数");
                                }

                            }
                        }
                    }

                    ThumbSetWindow thumbSetWindow = new ThumbSetWindow();

                    if (nowThumbContent.Type == ThumbClass.Items)
                    {
                        thumbSetWindow.DataContext = new ThumbSetWindowViewModel()
                        {
                            IsEnabel = false,//用于条件取反

                            IsNegate = false,//条件取反是否选中

                            UseItem = true,//用于规定物品的数量

                            Classifications = newList,

                            SaveTerms = newDic

                        };

                    }

                    else if (nowThumbContent.Type == ThumbClass.Conditions)
                    {
                        if (getChildModel.isContisionCmd)
                        {
                            thumbSetWindow.DataContext = new ThumbSetWindowViewModel()
                            {
                                IsEnabel = true,//用于条件取反

                                IsNegate = false,//条件取反是否选中

                                UseItem = false,//用于规定物品的数量

                                Classifications = newList,

                                SaveTerms = newDic

                            };
                        }
                        else
                        {
                            thumbSetWindow.DataContext = new ThumbSetWindowViewModel()
                            {
                                IsEnabel = false,//用于条件取反

                                IsNegate = false,//条件取反是否选中

                                UseItem = false,//用于规定物品的数量

                                Classifications = newList,

                                SaveTerms = newDic

                            };
                        }
                    }
                    else
                    {
                        thumbSetWindow.DataContext = new ThumbSetWindowViewModel()
                        {
                            IsEnabel = false,//用于条件取反

                            IsNegate = false,//条件取反是否选中

                            UseItem = false,//用于规定物品的数量

                            Classifications = newList,

                            SaveTerms = newDic

                        };
                    }

                    while (thumbSetWindow.Tag == null)
                    {
                        thumbSetWindow.ShowDialog();
                    }

                    if (!(bool)thumbSetWindow.Tag)
                    {
                        ClientWindowViewModel.ShowMessageDel("终止操作");
                        return;
                    }

                    var childName = nowThumbContent.ConfigName;

                    if ((thumbSetWindow.DataContext as ThumbSetWindowViewModel).IsEnabel && (thumbSetWindow.DataContext as ThumbSetWindowViewModel).IsNegate)
                    {
                        childName = "!" + childName;
                    }
                    else if ((thumbSetWindow.DataContext as ThumbSetWindowViewModel).UseItem)
                    {
                        if (string.IsNullOrEmpty((thumbSetWindow.DataContext as ThumbSetWindowViewModel).ItemNum))
                        {
                            childName += ":" + (thumbSetWindow.DataContext as ThumbSetWindowViewModel).ItemNum;
                        }

                    }

                    var getCmd = (thumbSetWindow.DataContext as ThumbSetWindowViewModel).ClassificationsSeleted;

                    var getPar = (thumbSetWindow.DataContext as ThumbSetWindowViewModel).TermsSeleted;

                    if (getCmd == getRealoFatherCmd)//是主命令
                    {
                        var getNum = TxtSplit(getPar, " ");

                        if (getNum.Count != 3)
                        {
                            ClientWindowViewModel.ShowMessageDel("归类失败，出现了意外的情况！");

                            return;
                        }

                        if (Convert.ToInt32(getNum[1]) == 1)//主参数
                        {
                            if (getFatherModel.TextNum != -1 && getFatherModel.TextNum != 1)
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，这是不允许的归类情况！");

                                return;
                            }

                            if (getFatherModel.TextNum == -1)
                            {
                                if (!ObjectiveCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                                try
                                {
                                    if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                        return;
                                    }

                                    if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                    {
                                        val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                    }
                                    else
                                    {
                                        var getnowNum = val[realFatherCard.SelectType][getCmd][getPar];

                                        var nowNum = 0;

                                        foreach (var i in getnowNum)
                                        {
                                            var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                            if (fg.Length != 3)
                                            {
                                                ClientWindowViewModel.ShowMessageDel("归类失败，存储数据异常！");

                                                return;
                                            }

                                            var num = Convert.ToInt32(fg[1]);

                                            if (num > nowNum)
                                            {
                                                nowNum = num;
                                            }
                                        }

                                        val[realFatherCard.SelectType][getCmd][getPar].Add($"第 {nowNum + 1} 项", childName);

                                        realFatherCard.AllItem.Add($"第 {nowNum + 1} 项");
                                    }
                                }
                                catch
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                            }
                            else if (getFatherModel.TextNum == 1)
                            {
                                if (!ObjectiveCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }

                                try
                                {
                                    if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                        return;
                                    }

                                    if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                    {
                                        val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                    }
                                    else
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片只能有一个子卡片，如果想要替换请先取消原卡片的归类！");

                                        return;
                                    }
                                }
                                catch
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                            }
                        }
                        else
                        {
                            var getRealNum = Convert.ToInt32(getNum[1]) - 2;

                            var getSplitWord = getFatherModel.TextSplitWords[getRealNum].j;

                            if (getSplitWord != -1 && getSplitWord != 1)
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，这是不允许的归类情况！");

                                return;
                            }

                            if (getSplitWord == -1)
                            {
                                if (!ObjectiveCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                                try
                                {
                                    if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                        return;
                                    }

                                    if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                    {
                                        val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                    }
                                    else
                                    {
                                        var getnowNum = val[realFatherCard.SelectType][getCmd][getPar];

                                        var nowNum = 0;

                                        foreach (var i in getnowNum)
                                        {
                                            var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                            if (fg.Length != 3)
                                            {
                                                ClientWindowViewModel.ShowMessageDel("归类失败，存储数据异常！");

                                                return;
                                            }

                                            var num = Convert.ToInt32(fg[1]);

                                            if (num > nowNum)
                                            {
                                                nowNum = num;
                                            }
                                        }

                                        val[realFatherCard.SelectType][getCmd][getPar].Add($"第 {nowNum + 1} 项", childName);

                                        realFatherCard.AllItem.Add($"第 {nowNum + 1} 项");
                                    }
                                }
                                catch
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                            }
                            else if (getSplitWord == 1)
                            {
                                if (!ObjectiveCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }

                                try
                                {
                                    if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                        return;
                                    }

                                    if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                    {
                                        val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                    }
                                    else
                                    {
                                        ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片只能有一个子卡片，如果想要替换请先取消原卡片的归类！");

                                        return;
                                    }
                                }
                                catch
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                    return;
                                }
                            }
                        }
                    }
                    else if (getFatherModel.ChildClasses.Where(t => t.ChildClass == getCmd).Any())//是子命令
                    {
                        if (getPar != "第 1 条参数")
                        {
                            ClientWindowViewModel.ShowMessageDel("归类失败，出现了意外的情况！");

                            return;
                        }

                        var getFatherModelChildClass = getFatherModel.ChildClasses.Where(t => t.ChildClass == getCmd).FirstOrDefault();

                        var getSplitWord = getFatherModelChildClass.ChildTextNum;

                        if (getSplitWord != -1 && getSplitWord != 1)
                        {
                            ClientWindowViewModel.ShowMessageDel("归类失败，这是不允许的归类情况！");

                            return;
                        }

                        if (getSplitWord == -1)
                        {
                            if (!ObjectiveCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                return;
                            }
                            try
                            {
                                if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                    return;
                                }

                                if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                {
                                    val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                }
                                else
                                {
                                    var getnowNum = val[realFatherCard.SelectType][getCmd][getPar];

                                    var nowNum = 0;

                                    foreach (var i in getnowNum)
                                    {
                                        var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                        if (fg.Length != 3)
                                        {
                                            ClientWindowViewModel.ShowMessageDel("归类失败，存储数据异常！");

                                            return;
                                        }

                                        var num = Convert.ToInt32(fg[1]);

                                        if (num > nowNum)
                                        {
                                            nowNum = num;
                                        }
                                    }

                                    val[realFatherCard.SelectType][getCmd][getPar].Add($"第 {nowNum + 1} 项", childName);

                                    realFatherCard.AllItem.Add($"第 {nowNum + 1} 项");
                                }
                            }
                            catch
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                return;
                            }
                        }
                        else if (getSplitWord == 1)
                        {
                            if (!ObjectiveCardChanged.saveAllValue.TryGetValue(realFatherCard, out var val))
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                return;
                            }

                            try
                            {
                                if (val[realFatherCard.SelectType][getCmd][getPar].Count < 1)
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片请至少有一个项！");

                                    return;
                                }

                                if (string.IsNullOrEmpty(val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key]))
                                {
                                    val[realFatherCard.SelectType][getCmd][getPar][val[realFatherCard.SelectType][getCmd][getPar].FirstOrDefault().Key] = childName;
                                }
                                else
                                {
                                    ClientWindowViewModel.ShowMessageDel("归类失败，父级卡片只能有一个子卡片，如果想要替换请先取消原卡片的归类！");

                                    return;
                                }
                            }
                            catch
                            {
                                ClientWindowViewModel.ShowMessageDel("归类失败，未找到存储的数据！");

                                return;
                            }
                        }
                    }
                }
                else if (fatherCard.Type == ThumbClass.NPC)
                {
                    var realFatherCard = fatherCard as AnyCardViewModel;

                    var realChildCard = nowThumbContent as AnyCardViewModel;

                    switch (realChildCard.Type)
                    {
                        case ThumbClass.Player:
                            var getSave = ConversationCardChanged.saveAllValue[realFatherCard]["存储对话: pointer"]["pointer"]["第 1 条参数"];

                            if (getSave.Count < 1)
                            {
                                ClientWindowViewModel.ShowMessageDel("主卡片至少需要保留一个项");
                                return;
                            }

                            var childName = nowThumbContent.ConfigName;

                            if (string.IsNullOrEmpty(getSave[getSave.FirstOrDefault().Key]))
                            {
                                getSave[getSave.FirstOrDefault().Key] = childName;
                            }
                            else
                            {
                                var nowNum = 0;

                                foreach (var i in getSave)
                                {
                                    var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                    if (fg.Length != 3)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("添加错误！");

                                        return;
                                    }

                                    var num = Convert.ToInt32(fg[1]);

                                    if (num > nowNum)
                                    {
                                        nowNum = num;
                                    }
                                }

                                getSave.Add($"第 {nowNum + 1} 项", childName);

                                realFatherCard.AllItem.Add($"第 {nowNum + 1} 项");
                            }

                            break;

                        case ThumbClass.Conditions:
                            getSave = ConversationCardChanged.saveAllValue[realFatherCard]["触发条件: conditions"]["conditions"]["第 1 条参数"];

                            if (getSave.Count < 1)
                            {
                                ClientWindowViewModel.ShowMessageDel("主卡片至少需要保留一个项");
                                return;
                            }

                            childName = nowThumbContent.ConfigName;

                            ThumbSetWindow thumbSetWindow = new ThumbSetWindow();

                            thumbSetWindow.DataContext = new ThumbSetWindowViewModel()
                            {
                                IsEnabel = true,//用于条件取反

                                IsNegate = false,//条件取反是否选中

                                UseItem = false,//用于规定物品的数量

                                Classifications = new List<string> { "conditions" },

                                SaveTerms = new Dictionary<string, List<string>> { { "conditions",new List<string> { "第 1 条参数"} } }

                            };

                            while(thumbSetWindow.Tag == null)
                            {
                                thumbSetWindow.ShowDialog();
                            }

                            if (!(bool)thumbSetWindow.Tag)
                            {
                                ClientWindowViewModel.ShowMessageDel("错误的操作");
                                return;
                            }

                            if((thumbSetWindow.DataContext as ThumbSetWindowViewModel).IsNegate)
                            {
                                childName = "!" + childName;
                            }

                            if (string.IsNullOrEmpty(getSave[getSave.FirstOrDefault().Key]))
                            {
                                getSave[getSave.FirstOrDefault().Key] = childName;
                            }
                            else
                            {
                                var nowNum = 0;

                                foreach (var i in getSave)
                                {
                                    var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                    if (fg.Length != 3)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("添加错误！");

                                        return;
                                    }

                                    var num = Convert.ToInt32(fg[1]);

                                    if (num > nowNum)
                                    {
                                        nowNum = num;
                                    }
                                }

                                getSave.Add($"第 {nowNum + 1} 项", childName);

                                realFatherCard.AllItem.Add($"第 {nowNum + 1} 项");
                            }
                            break;

                        case ThumbClass.Events:
                            getSave = ConversationCardChanged.saveAllValue[realFatherCard]["触发事件: events"]["events"]["第 1 条参数"];

                            if (getSave.Count < 1)
                            {
                                ClientWindowViewModel.ShowMessageDel("主卡片至少需要保留一个项");
                                return;
                            }

                            childName = nowThumbContent.ConfigName;

                            if (string.IsNullOrEmpty(getSave[getSave.FirstOrDefault().Key]))
                            {
                                getSave[getSave.FirstOrDefault().Key] = childName;
                            }
                            else
                            {
                                var nowNum = 0;

                                foreach (var i in getSave)
                                {
                                    var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                    if (fg.Length != 3)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("添加错误！");

                                        return;
                                    }

                                    var num = Convert.ToInt32(fg[1]);

                                    if (num > nowNum)
                                    {
                                        nowNum = num;
                                    }
                                }

                                getSave.Add($"第 {nowNum + 1} 项", childName);

                                realFatherCard.AllItem.Add($"第 {nowNum + 1} 项");
                            }

                            break;

                        default:
                            ClientWindowViewModel.ShowMessageDel("不允许的归类");
                            return;
                    }
                }
                else if (fatherCard.Type == ThumbClass.Player)
                {
                    var realFatherCard = fatherCard as AnyCardViewModel;

                    var realChildCard = nowThumbContent as AnyCardViewModel;

                    switch (realChildCard.Type)
                    {
                        case ThumbClass.NPC:

                            var getSave = ConversationCardChanged.saveAllValue[realFatherCard]["存储对话: pointer"]["pointer"]["第 1 条参数"];

                            if (getSave.Count < 1)
                            {
                                ClientWindowViewModel.ShowMessageDel("主卡片至少需要保留一个项");
                                return;
                            }

                            var childName = nowThumbContent.ConfigName;

                            if (string.IsNullOrEmpty(getSave[getSave.FirstOrDefault().Key]))
                            {
                                getSave[getSave.FirstOrDefault().Key] = childName;
                            }
                            else
                            {
                                ClientWindowViewModel.ShowMessageDel("主卡片已与另一个子卡片归类，请先解除，再次归类");
                                return;
                            }

                            break;

                        case ThumbClass.Conditions:
                            getSave = ConversationCardChanged.saveAllValue[realFatherCard]["触发条件: conditions"]["conditions"]["第 1 条参数"];

                            if (getSave.Count < 1)
                            {
                                ClientWindowViewModel.ShowMessageDel("主卡片至少需要保留一个项");
                                return;
                            }

                            childName = nowThumbContent.ConfigName;

                            ThumbSetWindow thumbSetWindow = new ThumbSetWindow();

                            thumbSetWindow.DataContext = new ThumbSetWindowViewModel()
                            {
                                IsEnabel = true,//用于条件取反

                                IsNegate = false,//条件取反是否选中

                                UseItem = false,//用于规定物品的数量

                                Classifications = new List<string> { "conditions" },

                                SaveTerms = new Dictionary<string, List<string>> { { "conditions", new List<string> { "第 1 条参数" } } }

                            };

                            while (thumbSetWindow.Tag == null)
                            {
                                thumbSetWindow.ShowDialog();
                            }

                            if (!(bool)thumbSetWindow.Tag)
                            {
                                ClientWindowViewModel.ShowMessageDel("错误的操作");
                                return;
                            }

                            if ((thumbSetWindow.DataContext as ThumbSetWindowViewModel).IsNegate)
                            {
                                childName = "!" + childName;
                            }

                            if (string.IsNullOrEmpty(getSave[getSave.FirstOrDefault().Key]))
                            {
                                getSave[getSave.FirstOrDefault().Key] = childName;
                            }
                            else
                            {
                                var nowNum = 0;

                                foreach (var i in getSave)
                                {
                                    var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                    if (fg.Length != 3)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("添加错误！");

                                        return;
                                    }

                                    var num = Convert.ToInt32(fg[1]);

                                    if (num > nowNum)
                                    {
                                        nowNum = num;
                                    }
                                }

                                getSave.Add($"第 {nowNum + 1} 项", childName);

                                realFatherCard.AllItem.Add($"第 {nowNum + 1} 项");
                            }
                            break;

                        case ThumbClass.Events:
                            getSave = ConversationCardChanged.saveAllValue[realFatherCard]["触发事件: events"]["events"]["第 1 条参数"];

                            if (getSave.Count < 1)
                            {
                                ClientWindowViewModel.ShowMessageDel("主卡片至少需要保留一个项");
                                return;
                            }

                            childName = nowThumbContent.ConfigName;

                            if (string.IsNullOrEmpty(getSave[getSave.FirstOrDefault().Key]))
                            {
                                getSave[getSave.FirstOrDefault().Key] = childName;
                            }
                            else
                            {
                                var nowNum = 0;

                                foreach (var i in getSave)
                                {
                                    var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                    if (fg.Length != 3)
                                    {
                                        ClientWindowViewModel.ShowMessageDel("添加错误！");

                                        return;
                                    }

                                    var num = Convert.ToInt32(fg[1]);

                                    if (num > nowNum)
                                    {
                                        nowNum = num;
                                    }
                                }

                                getSave.Add($"第 {nowNum + 1} 项", childName);

                                realFatherCard.AllItem.Add($"第 {nowNum + 1} 项");
                            }

                            break;

                        default:
                            ClientWindowViewModel.ShowMessageDel("不允许的归类");
                            return;
                    }
                }


                if (!fatherCard.Right.Contains(nowThumbContent))
                {
                    fatherCard.Right.Add(nowThumbContent);
                }

                if (!nowThumbContent.Left.Contains(fatherCard))
                {
                    nowThumbContent.Left.Add(fatherCard);
                }

                if(fatherCard.Type == ThumbClass.Subject)
                {
                    GiveMainCard(nowThumbContent, fatherCard);
                }
                else
                {
                    if(fatherCard.MainCard != null)
                    {
                        GiveMainCard(nowThumbContent, fatherCard.MainCard);
                    }
                }

                var getCardInfo = ClientWindowViewModel.GetAllCardDel();

                getCardInfo.Add(new LineCardViewModel()
                {
                    LineLeft = fatherCard,

                    LineRight = nowThumbContent,

                    CvLeft = fatherCard.CvLeft + (double)ThumbHeight / 2,

                    CvTop = fatherCard.CvTop + (double)ThumbWidth / 2,

                    X = -(CvLeft - (nowThumbContent.CvLeft + (double)ThumbHeight / 2)),

                    Y = -(CvTop - (nowThumbContent.CvTop + (double)ThumbWidth / 2)),

                });
            }
            else
            {
                ClientWindowViewModel.ShowMessageDel("终止操作");
            }
        }

        /// <summary>
        /// 控件左键单击后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Thumb_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var getSelectCard = GetSelecteCardDel();

            getSelectCard.Clear();//清空框选的卡片集合

            var getAllCardInfo = ClientWindowViewModel.GetAllCardDel();

            await Task.Run(() =>
            {
                foreach (var item in getAllCardInfo)
                {
                    item.ShadowColor = Brushes.Black.Color;
                }
            });

            var nowThumb = (Thumb)sender;
            var nowThumbContent = nowThumb.DataContext as CardViewModel;

            nowThumbContent.ShadowColor = Brushes.Red.Color;

            foreach (var item in nowThumbContent.Right)
            {
                item.ShadowColor = Brushes.GreenYellow.Color;
            }

            foreach (var item in nowThumbContent.Left)
            {
                item.ShadowColor = Brushes.Yellow.Color;
            }
        }

        /// <summary>
        /// 控件左键单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Thumb_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var nowThumb = (Thumb)sender;
            var nowThumbContent = nowThumb.DataContext as CardViewModel;

            ClientWindowViewModel.SetSelectCardInfoDel(nowThumbContent);//委托将选中的Card传回

            if(nowThumbContent.Type == ThumbClass.Subject|| nowThumbContent.Type == ThumbClass.Journal|| nowThumbContent.Type == ThumbClass.Items)
            {
                return;
            }

            if(nowThumbContent.Type == ThumbClass.Conditions)
            {
                if(ConditionsCardChanged.saveAllValue.TryGetValue(nowThumbContent as AnyCardViewModel,out var vl))
                {
                    await ChangeTheTreeView(vl);
                }
            }
            else if(nowThumbContent.Type == ThumbClass.Events)
            {
                if (EventCardChanged.saveAllValue.TryGetValue(nowThumbContent as AnyCardViewModel, out var vl))
                {
                    await ChangeTheTreeView(vl);
                }
            }
            else if(nowThumbContent.Type == ThumbClass.Objectives)
            {
                if (ObjectiveCardChanged.saveAllValue.TryGetValue(nowThumbContent as AnyCardViewModel, out var vl))
                {
                    await ChangeTheTreeView(vl);
                }
            }
            else
            {
                if (ConversationCardChanged.saveAllValue.TryGetValue(nowThumbContent as AnyCardViewModel, out var vl))
                {
                    await ChangeTheTreeView(vl);
                }
            }
        }

        /// <summary>
        /// 获取卡片是否被拖拽到其他卡片上
        /// </summary>
        /// <param name="nowCard"></param>
        /// <returns></returns>
        private async Task<ReturnModel> ThumbClassification(CardViewModel nowCard)
        {
            var getCardInfo = ClientWindowViewModel.GetAllCardDel();

            var result = await Task.Run(() =>
            {
                var back = new ReturnModel();

                var getNowCardPoints = GetCardPoint(nowCard);

                foreach (var item in getCardInfo)
                {
                    if (item.IsDraw||item.IsLine)//防止绘画图形被归类
                    {
                        continue;
                    }

                    var getCardPoints = GetCardPoint(item);

                    if (getNowCardPoints[0].X > getCardPoints[0].X && getNowCardPoints[0].X < getCardPoints[1].X)
                    {
                        if (getNowCardPoints[0].Y > getCardPoints[0].Y && getNowCardPoints[0].Y < getCardPoints[2].Y)
                        {
                            back.SetSuccese("", item);

                            return back;
                        }
                    }

                    if (getNowCardPoints[1].X > getCardPoints[0].X && getNowCardPoints[1].X < getCardPoints[1].X)
                    {
                        if (getNowCardPoints[1].Y > getCardPoints[0].Y && getNowCardPoints[1].Y < getCardPoints[2].Y)
                        {
                            back.SetSuccese("", item);

                            return back;
                        }
                    }

                    if (getNowCardPoints[2].X > getCardPoints[0].X && getNowCardPoints[2].X < getCardPoints[1].X)
                    {
                        if (getNowCardPoints[2].Y > getCardPoints[0].Y && getNowCardPoints[2].Y < getCardPoints[2].Y)
                        {
                            back.SetSuccese("", item);

                            return back;
                        }
                    }

                    if (getNowCardPoints[3].X > getCardPoints[0].X && getNowCardPoints[3].X < getCardPoints[1].X)
                    {
                        if (getNowCardPoints[3].Y > getCardPoints[0].Y && getNowCardPoints[3].Y < getCardPoints[2].Y)
                        {
                            back.SetSuccese("", item);

                            return back;
                        }
                    }
                }

                back.SetError();

                return back;
            });

            return result;
        }

        private List<Point> GetCardPoint(CardViewModel nowCard)
        {
            var points = new List<Point>();

            points.Add(new Point(nowCard.CvLeft, nowCard.CvTop));

            points.Add(new Point(nowCard.CvLeft+ (double)nowCard.ThumbWidth, nowCard.CvTop));

            points.Add(new Point(nowCard.CvLeft, nowCard.CvTop+ (double)nowCard.ThumbHeight));

            points.Add(new Point(nowCard.CvLeft+ (double)nowCard.ThumbWidth, nowCard.CvTop+ (double)nowCard.ThumbHeight));

            return points;
        }

        /// <summary>
        /// 判断两张卡片能否归类(不进行数据判断)
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnModel> CardCanBeClassify(CardViewModel father,CardViewModel child)
        {
            var back = await Task.Run(() =>
            {
                var result = new ReturnModel();

                switch (father.Type)
                {
                    case ThumbClass.Subject://特殊
                        if (child.Type == ThumbClass.NPC)
                        {
                            result.SetSuccese("", ThumbClass.NPC);
                            return result;
                        }
                        break;
                    case ThumbClass.NPC://特殊
                        if (child.Type == ThumbClass.Player || child.Type == ThumbClass.Conditions || child.Type == ThumbClass.Events)
                        {
                            result.SetSuccese("", child.Type);
                            return result;
                        }
                        break;
                    case ThumbClass.Player://特殊
                        if (child.Type == ThumbClass.NPC || child.Type == ThumbClass.Conditions || child.Type == ThumbClass.Events)
                        {
                            result.SetSuccese("", child.Type);
                            return result;
                        }
                        break;
                    case ThumbClass.Conditions:
                        var realCardInfo = father as AnyCardViewModel;

                        try
                        {
                            var getRealType = TxtSplit(realCardInfo.SelectType, ": ")[1];

                            var getGrammar = ClientWindowViewModel.contisionProp.Where(t => t.MainClass == getRealType).FirstOrDefault();

                            foreach (var item in getGrammar.NeedTpye)
                            {
                                foreach (var t in item.Value)
                                {
                                    if (t.Value == child.Type)
                                    {
                                        result.SetSuccese("", child.Type);
                                        return result;
                                    }
                                }
                            }

                            result.SetError();
                            return result;
                        }
                        catch
                        {
                            result.SetError();
                            return result;
                        }
                    case ThumbClass.Events:
                        realCardInfo = father as AnyCardViewModel;

                        try
                        {
                            var getRealType = TxtSplit(realCardInfo.SelectType, ": ")[1];

                            var getGrammar = ClientWindowViewModel.eventProp.Where(t => t.MainClass == getRealType).FirstOrDefault();

                            foreach (var item in getGrammar.NeedTpye)
                            {
                                foreach (var t in item.Value)
                                {
                                    if (t.Value == child.Type)
                                    {
                                        result.SetSuccese("", child.Type);
                                        return result;
                                    }
                                }
                            }

                            result.SetError();
                            return result;
                        }
                        catch
                        {
                            result.SetError();
                            return result;
                        }
                    case ThumbClass.Objectives:
                        realCardInfo = father as AnyCardViewModel;

                        try
                        {
                            var getRealType = TxtSplit(realCardInfo.SelectType, ": ")[1];

                            var getGrammar = ClientWindowViewModel.objectiveProp.Where(t => t.MainClass == getRealType).FirstOrDefault();

                            foreach (var item in getGrammar.NeedTpye)
                            {
                                foreach (var t in item.Value)
                                {
                                    if (t.Value == child.Type)
                                    {
                                        result.SetSuccese("", child.Type);
                                        return result;
                                    }
                                }
                            }

                            result.SetError();
                            return result;
                        }
                        catch
                        {
                            result.SetError();
                            return result;
                        }
                    default:
                        result.SetError();
                        return result;
                }

                result.SetError();
                return result;
            });

            return back;
        }

        /// <summary>
        /// 用于递归赋予MainCard
        /// </summary>
        public void GiveMainCard(CardViewModel nowCard, CardViewModel mainCard)
        {
            nowCard.MainCard = mainCard;

            if (nowCard.Right.Count > 0)
            {
                foreach (var item in nowCard.Right)
                {
                    if(item.Type == ThumbClass.Player|| item.Type == ThumbClass.NPC)

                    GiveMainCard(item, mainCard);
                }
            }
        }

        public async Task<ReturnModel> ChangeTheTreeView(Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> saveValue)
        {
            var result = new ReturnModel();

            var getTreeController = ClientWindowViewModel.thumbInfoWindow.TreeView_Tv;

            var nodes = new List<DefinitionNode>();

            await Task.Run(() =>
            {
                foreach (var item in saveValue)
                {
                    foreach (var i in item.Value)
                    {
                        var two = new DefinitionNode
                        {
                            Name = i.Key,
                            FontColor = "White"
                        };

                        nodes.Add(two);

                        foreach (var j in i.Value)
                        {
                            var three = new DefinitionNode
                            {
                                Name = j.Key,
                                FontColor = "White"
                            };

                            two.Children.Add(three);

                            foreach (var m in j.Value)
                            {
                                var four = new DefinitionNode() { };

                                if (!string.IsNullOrEmpty(m.Value))
                                {
                                    four.Name = m.Key + " ------ 已保存";
                                    four.FontColor = "#1f640a";
                                }
                                else
                                {
                                    four.Name = m.Key + " ------ 未保存";
                                    four.FontColor = "#f6003c";
                                }

                                three.Children.Add(four);
                            }
                        }
                    }
                }
            });

            getTreeController.ItemsSource = nodes;

            result.SetSuccese();

            return result;
        }
        
        /// <summary>
        /// 文本分割
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="fg"></param>
        /// <returns></returns>
        protected static List<string> TxtSplit(string txt, string fg)
        {
            var getSqlit = txt.Split(new string[] { fg }, StringSplitOptions.RemoveEmptyEntries);

            var newList = new List<string>(getSqlit);

            return newList;
        }
    }

    public partial class SubjectCardViewModel: CardViewModel
    {
        public SubjectCardViewModel()
        {
            CvZIndex = 1;
            ThumbWidth = 400.00;
            ThumbHeight = 148.00;
        }

        [ObservableProperty]
        private string _NPC_ID = string.Empty;
    }

    public partial class AnyCardViewModel : CardViewModel
    {
        public AnyCardViewModel()
        {
            CvZIndex = 1;
        }
        public AnyCardViewModel(List<ContisionsCmdModel> savecmdModels)
        {
            ThumbWidth = 400.00;
            ThumbHeight = 300.00;
            conditionsCardChanged = new ConditionsCardChanged(savecmdModels,this);
            CvZIndex = 1;
        }

        public AnyCardViewModel(List<EventCmdModel> savecmdModels)
        {
            ThumbWidth = 400.00;
            ThumbHeight = 300.00;
            eventCardChanged = new EventCardChanged(savecmdModels, this);
            CvZIndex = 1;
        }

        public AnyCardViewModel(List<ObjectiveCmdModel> savecmdModels)
        {
            ThumbWidth = 400.00;
            ThumbHeight = 300.00;
            objectiveCardChanged = new ObjectiveCardChanged(savecmdModels, this);
            CvZIndex = 1;
        }

        public AnyCardViewModel(bool isConvert)
        {
            if (!isConvert)
            {
                throw new NotImplementedException();
            }
            ThumbWidth = 400.00;
            ThumbHeight = 350.00;
            conversationCardChanged = new ConversationCardChanged(this);

            CvZIndex = 1;
        }

        #region 属性与字段
        public ConversationCardChanged conversationCardChanged;

        public ConditionsCardChanged conditionsCardChanged;

        public EventCardChanged eventCardChanged;

        public ObjectiveCardChanged objectiveCardChanged;

        [ObservableProperty]
        private string _TypeHelp = string.Empty;

        [ObservableProperty]
        private string _CmdHelp = string.Empty;

        [ObservableProperty]
        private string _ParameterHelp = string.Empty;

        [ObservableProperty]
        private string _ItemHelp = string.Empty;

        [ObservableProperty]
        private string _SelectType = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _AllType = new ObservableCollection<string>();

        [ObservableProperty]
        private string _SelectCmd = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _AllCmd = new ObservableCollection<string>();

        [ObservableProperty]
        private string _SelectParameter = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _AllParameter = new ObservableCollection<string>();

        [ObservableProperty]
        private string _SelectItem = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _AllItem = new ObservableCollection<string>();

        [ObservableProperty]
        private bool _ItemAddIsEnable = false;

        [ObservableProperty]
        private bool _ItemRemoveIsEnable = false;

        [ObservableProperty]
        private string _ItemSelectContent = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _Item_Items = new ObservableCollection<string>();

        [ObservableProperty]
        private bool _ItemCoBoxIsEnable = false;

        [ObservableProperty]
        private Visibility _ItemCoBoxVisibility = Visibility.Hidden;

        #endregion

        #region 方法及命令

        #region 操作层

        [RelayCommand()]
        private async Task TypeLoaded(ComboBox coBox)
        {
            coBox.SelectionChanged += TypeCoBox_SelectChanged;
        }

        [RelayCommand()]
        private async Task CmdLoaded(ComboBox coBox)
        {
            coBox.SelectionChanged += CmdCoBox_SelectChanged;
        }

        [RelayCommand()]
        private async Task ParameterLoaded(ComboBox coBox)
        {
            coBox.SelectionChanged += ParmeterCoBox_SelectChanged;
        }

        [RelayCommand()]
        private async Task ItemLoaded(ComboBox coBox)
        {
            coBox.SelectionChanged += ItemCoBox_SelectChanged;
        }

        [RelayCommand()]
        private async Task ItemAdd(ComboBox coBox)
        {
            if (Type == ThumbClass.Conditions)
            {
                if (conditionsCardChanged != null)
                {
                    var back = await conditionsCardChanged.ItemAdd();
                    ShowMessageDel(back.Text);
                }
            }
            else if (Type == ThumbClass.Events)
            {
                if (eventCardChanged != null)
                {
                    var back = await eventCardChanged.ItemAdd();
                    ShowMessageDel(back.Text);
                }
            }
            else if (Type == ThumbClass.Objectives)
            {
                if (objectiveCardChanged != null)
                {
                    var back = await objectiveCardChanged.ItemAdd();
                    ShowMessageDel(back.Text);
                }
            }
            else if (Type == ThumbClass.NPC || Type == ThumbClass.Player)
            {
                if(conversationCardChanged != null)
                {
                    var back = await conversationCardChanged.ItemAdd();
                    ShowMessageDel(back.Text);
                }
            }
        }

        [RelayCommand()]
        private async Task ItemRemove(ComboBox coBox)
        {
            if (Type == ThumbClass.Conditions)
            {
                if (conditionsCardChanged != null&&!string.IsNullOrEmpty(SelectItem))
                {
                    var back = await conditionsCardChanged.ItemRemove();
                    ShowMessageDel(back.Text);
                }
            }
            else if (Type == ThumbClass.Events)
            {
                if (eventCardChanged != null)
                {
                    var back = await eventCardChanged.ItemRemove();

                    ShowMessageDel(back.Text);
                }
            }
            else if (Type == ThumbClass.Objectives)
            {
                if (objectiveCardChanged != null)
                {
                    var back = await objectiveCardChanged.ItemRemove();

                    ShowMessageDel(back.Text);
                }
            }
            else if (Type == ThumbClass.NPC || Type == ThumbClass.Player)
            {
                if (conversationCardChanged != null)
                {
                    var back = await conversationCardChanged.ItemRemove();
                    ShowMessageDel(back.Text);
                }
            }

        }

        [RelayCommand()]
        private async Task Save(Window mainWindow)
        {
            var type = SelectType;

            var cmd = SelectCmd;

            var par = SelectParameter;

            var item = SelectItem;

            if (Type == ThumbClass.Conditions)
            {
                if (conditionsCardChanged != null)
                {
                    try
                    {
                        if(ItemCoBoxIsEnable&& ItemCoBoxVisibility == Visibility.Visible)
                        {
                            ConditionsCardChanged.saveAllValue[this][type][cmd][par][item] = ItemSelectContent;
                        }
                        else
                        {
                            ConditionsCardChanged.saveAllValue[this][type][cmd][par][item] = ItemContent;
                        }
                        

                        ShowMessageDel("保存成功");
                    }
                    catch
                    {
                        ShowMessageDel("保存失败！");
                    }
                }
            }
            else if (Type == ThumbClass.Events)
            {
                if (eventCardChanged != null)
                {
                    try
                    {
                        if (ItemCoBoxIsEnable && ItemCoBoxVisibility == Visibility.Visible)
                        {
                            EventCardChanged.saveAllValue[this][type][cmd][par][item] = ItemSelectContent;
                        }
                        else
                        {
                            EventCardChanged.saveAllValue[this][type][cmd][par][item] = ItemContent;
                        }


                        ShowMessageDel("保存成功");
                    }
                    catch
                    {
                        ShowMessageDel("保存失败！");
                    }
                }
            }
            else if (Type == ThumbClass.Objectives)
            {
                if (objectiveCardChanged != null)
                {
                    try
                    {
                        if (ItemCoBoxIsEnable && ItemCoBoxVisibility == Visibility.Visible)
                        {
                            ObjectiveCardChanged.saveAllValue[this][type][cmd][par][item] = ItemSelectContent;
                        }
                        else
                        {
                            ObjectiveCardChanged.saveAllValue[this][type][cmd][par][item] = ItemContent;
                        }


                        ShowMessageDel("保存成功");
                    }
                    catch
                    {
                        ShowMessageDel("保存失败！");
                    }
                }
            }
            else if (Type == ThumbClass.NPC || Type == ThumbClass.Player)
            {
                if (conversationCardChanged != null)
                {
                    try
                    {
                        if (ItemCoBoxIsEnable && ItemCoBoxVisibility == Visibility.Visible)
                        {
                            ConversationCardChanged.saveAllValue[this][type][cmd][par][item] = ItemSelectContent;
                        }
                        else
                        {
                            ConversationCardChanged.saveAllValue[this][type][cmd][par][item] = ItemContent;
                        }


                        ShowMessageDel("保存成功");
                    }
                    catch
                    {
                        ShowMessageDel("保存失败！");
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Combox事件

        private async void TypeCoBox_SelectChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Type == ThumbClass.Conditions)
            {
                if (conditionsCardChanged != null)
                {
                    await conditionsCardChanged.TypeChanged();
                }
            }
            else if(Type == ThumbClass.Events)
            {
                if (eventCardChanged != null)
                {
                    await eventCardChanged.TypeChanged();
                }
            }
            else if(Type == ThumbClass.Objectives)
            {
                if (objectiveCardChanged != null)
                {
                    await objectiveCardChanged.TypeChanged();
                }
            }
            else if (Type == ThumbClass.NPC||Type == ThumbClass.Player)
            {
                await conversationCardChanged.TypeChanged();
            }
        }

        private async void CmdCoBox_SelectChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Type == ThumbClass.Conditions)
            {
                if (conditionsCardChanged != null)
                {
                    await conditionsCardChanged.CmdChanged();
                }
            }
            else if (Type == ThumbClass.Events)
            {
                if (eventCardChanged != null)
                {
                    await eventCardChanged.CmdChanged();
                }
            }
            else if (Type == ThumbClass.Objectives)
            {
                if (objectiveCardChanged != null)
                {
                    await objectiveCardChanged.CmdChanged();
                }
            }
            else if (Type == ThumbClass.NPC || Type == ThumbClass.Player)
            {
                await conversationCardChanged.CmdChanged();
            }
        }

        private async void ParmeterCoBox_SelectChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Type == ThumbClass.Conditions)
            {
                if (conditionsCardChanged != null)
                {
                    await conditionsCardChanged.ParameterChanged();
                }
            }
            else if (Type == ThumbClass.Events)
            {
                if (eventCardChanged != null)
                {
                    await eventCardChanged.ParameterChanged();
                }
            }
            else if (Type == ThumbClass.Objectives)
            {
                if (objectiveCardChanged != null)
                {
                    await objectiveCardChanged.ParameterChanged();
                }
            }
            else if (Type == ThumbClass.NPC || Type == ThumbClass.Player)
            {
                await conversationCardChanged.ParameterChanged();
            }
        }

        private async void ItemCoBox_SelectChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Type == ThumbClass.Conditions)
            {
                if (conditionsCardChanged != null)
                {
                    await conditionsCardChanged.ItemChanged();
                }
            }
            else if (Type == ThumbClass.Events)
            {
                if (eventCardChanged != null)
                {
                    await eventCardChanged.ItemChanged();
                }
            }
            else if (Type == ThumbClass.Objectives)
            {
                if (objectiveCardChanged != null)
                {
                    await objectiveCardChanged.ItemChanged();
                }
            }
            else if (Type == ThumbClass.NPC || Type == ThumbClass.Player)
            {
                await conversationCardChanged.ItemChanged();
            }
        }

        #endregion
    }

    public partial class BordCardViewModel : CardViewModel
    {
        public BordCardViewModel()
        {
            ThumbHeight = "Auto";
            ThumbWidth = "Auto";
        }

        [ObservableProperty]
        private double _Bord_Width = 0.00;

        [ObservableProperty]
        private double _Bord_Height = 0.00;

        [ObservableProperty]
        private Visibility _Bord_Visibility = Visibility.Hidden;
    }

    public partial class LineCardViewModel : CardViewModel
    {
        public LineCardViewModel()
        {
            this.IsLine = true;
            ThumbHeight = "Auto";
            ThumbWidth = "Auto";
        }

        [ObservableProperty]
        private double _X = 0.00;

        [ObservableProperty]
        private double _Y = 0.00;

        public CardViewModel LineRight { get; set; }

        public CardViewModel LineLeft { get; set; }
    }

    public partial class ShurtcutIdeaBtnViewModel: ObservableObject
    {
        [ObservableProperty]
        private string _CardName = string.Empty;

        [ObservableProperty]
        private ThumbClass _ThumbClassName = ThumbClass.Subject;

        [RelayCommand()]
        private async void CreateIdea()
        {
            var getClientVM = ClientWindowViewModel.GetClienteViewModelDel();
            switch (ThumbClassName)
            {
                case ThumbClass.Subject:
                    await getClientVM.CreateCard<SubjectCardViewModel>(ThumbClassName);
                    break;

                case ThumbClass.NPC:
                    await getClientVM.CreateCard<AnyCardViewModel>(ThumbClassName);
                    break;

                case ThumbClass.Player:
                    await getClientVM.CreateCard<AnyCardViewModel>(ThumbClassName);
                    break;

                case ThumbClass.Conditions:
                    await getClientVM.CreateCard<AnyCardViewModel>(ThumbClassName);
                    break;

                case ThumbClass.Events:
                    await getClientVM.CreateCard<AnyCardViewModel>(ThumbClassName);
                    break;

                case ThumbClass.Objectives:
                    await getClientVM.CreateCard<AnyCardViewModel>(ThumbClassName);
                    break;

                case ThumbClass.Journal:
                    await getClientVM.CreateCard<CardViewModel>(ThumbClassName);
                    break;

                case ThumbClass.Items:
                    await getClientVM.CreateCard<CardViewModel>(ThumbClassName);
                    break;
            }
        }
    }
}
