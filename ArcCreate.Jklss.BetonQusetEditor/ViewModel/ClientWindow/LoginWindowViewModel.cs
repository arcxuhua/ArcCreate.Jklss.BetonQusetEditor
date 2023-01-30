using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.BetonQusetEditor.Windows;
using ArcCreate.Jklss.Model.ClientModel;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.SocketModel;
using ArcCreate.Jklss.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static ArcCreate.Jklss.Model.SocketModel.SocketModel;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.ClientWindow
{
    public class LoginWindowViewModel : NotifyBase
    {
        private static LoginModel model = new LoginModel();

        public static SocketViewModel socketViewModel = new SocketViewModel();

        public static LoginWindow window = null;

        public CommandBase _RegisterCommand;
        public CommandBase RegisterCommand
        {
            get
            {
                if (_RegisterCommand == null)
                {
                    _RegisterCommand = new CommandBase();
                    _RegisterCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        var window = new RegisterWindow();
                        window.Show();
                        (obj as Window).Close();
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _RegisterCommand;
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

        public CommandBase _BackCommand;
        public CommandBase BackCommand
        {
            get
            {
                if (_BackCommand == null)
                {
                    _BackCommand = new CommandBase();
                    _BackCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        ShowPage(0);
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _BackCommand;
            }
        }

        public string UserName
        {
            get
            {
                return model.UserName;
            }
            set
            {
                model.UserName = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }
         
        public string PassWord
        {
            get
            {
                return model.PassWord;
            }
            set
            {
                model.PassWord = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public string WorryMessage
        {
            get
            {
                return model.WorryMessage;
            }
            set
            {
                model.WorryMessage = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public CommandBase _LoginCommand;
        public CommandBase LoginCommand
        {
            get
            {
                if (_LoginCommand == null)
                {
                    _LoginCommand = new CommandBase();
                    _LoginCommand.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        ShowPage(1);
                        if (string.IsNullOrEmpty(UserName))
                        {
                            SendWorryMessage("账号不能为空");
                            return;
                        }

                        if (string.IsNullOrEmpty(PassWord))
                        {
                            SendWorryMessage("密码不能为空");
                            return;
                        }

                        if(!Regex.IsMatch(UserName, @"^(\w)+(\.\w+)*@(\w)+((\.\w{2,3}){1,3})$"))
                        {
                            SendWorryMessage("错误的账号格式，请输入您的邮箱地址");
                            return;
                        }

                        if (!Regex.IsMatch(PassWord, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^%&',;=?$\x22]).{7,15}$"))
                        {
                            SendWorryMessage("错误的密码格式，请输入包含A-Z,a-z,且包含特殊字符的密码");
                            return;
                        }

                        var message = new MessageModel()
                        {
                            IsLogin = false,
                            UserName = UserName,
                            Message = PassWord
                        };

                        var getJson = FileService.SaveToJson(message);

                        try
                        {
                            if (SocketViewModel.socket == null)
                            {
                                await socketViewModel.StarSocketTCP();
                            }
                            
                            await Task.Run(async() =>
                            {
                                while (true)
                                {
                                    if (SocketModel.ClientKeys.ContainsKey(SocketViewModel.socket.RemoteEndPoint.ToString()))
                                    {
                                        if (!string.IsNullOrEmpty(SocketModel.ClientKeys[SocketViewModel.socket.RemoteEndPoint.ToString()].ServerSendKey.PublicKey))
                                        {
                                            break;
                                        }
                                    }
                                    Thread.Sleep(1000);
                                }

                                await socketViewModel.SendRESMessage(MessageClass.Json, getJson, SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString());

                            });
                        }
                        catch(Exception ex)
                        {
                            SendWorryMessage(ex.Message);
                        }
                        
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _LoginCommand;
            }
        }

        public delegate void _ShowWorryMessage(string txt);

        public static _ShowWorryMessage ShowWorryMessage;

        private RelayCommand<Window> _LoadedCommand;

        public RelayCommand<Window> LoadedCommand
        {
            get
            {
                if (_LoadedCommand == null)
                {
                    _LoadedCommand = new RelayCommand<Window>((wd) => {

                        window = wd as LoginWindow;
                        ShowWorryMessage = new _ShowWorryMessage(SendWorryMessage);
                    });
                }
                return _LoadedCommand;
            }
            set { _LoadedCommand = value; }
        }

        public void SendWorryMessage(string txt)
        {
            ShowPage(2);

            WorryMessage = txt;
        }

        public static void ShowPage(int i)
        {
            if (i > 2 || i < 0)
            {
                return;
            }

            window.Dispatcher.Invoke(new Action(() =>
            {
                switch (i)
                {
                    case 0:
                        AnimationBase.Appear(window.First);

                        if (window.Login.Visibility == Visibility.Visible)
                        {
                            AnimationBase.Disappear(window.Login);
                        }

                        if (window.Worry.Visibility == Visibility.Visible)
                        {
                            AnimationBase.Disappear(window.Worry);
                        }
                        break;
                    case 1:
                        AnimationBase.Appear(window.Login);

                        if (window.First.Visibility == Visibility.Visible)
                        {
                            AnimationBase.Disappear(window.First);
                        }

                        if (window.Worry.Visibility == Visibility.Visible)
                        {
                            AnimationBase.Disappear(window.Worry);
                        }
                        break;
                    case 2:
                        AnimationBase.Appear(window.Worry);

                        if (window.First.Visibility == Visibility.Visible)
                        {
                            AnimationBase.Disappear(window.First);
                        }

                        if (window.Login.Visibility == Visibility.Visible)
                        {
                            AnimationBase.Disappear(window.Login);
                        }
                        break;
                }
            }));
        }
    }
}
