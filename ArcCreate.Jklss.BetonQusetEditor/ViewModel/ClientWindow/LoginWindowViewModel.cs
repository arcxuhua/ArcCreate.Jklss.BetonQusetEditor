using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.BetonQusetEditor.Windows;
using ArcCreate.Jklss.Model.ClientModel;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.SocketModel;
using ArcCreate.Jklss.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static ArcCreate.Jklss.Model.SocketModel.SocketModel;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.ClientWindow
{
    public class LoginWindowViewModel : NotifyBase
    {
        private static LoginModel model = new LoginModel();

        public static string computerInfo = MachineNumberService.GetComputerInfo();

        public static SocketViewModel socketViewModel = new SocketViewModel();

        public static LoginWindow window = null;

        private static PasswordBox passwordBox = null;

        private static TextBox passwordTbox = null;

        private RememberPassword rememberPassword = null;

        public CommandBase _EyeCommand;
        public CommandBase EyeCommand
        {
            get
            {
                if (_EyeCommand == null)
                {
                    _EyeCommand = new CommandBase();
                    _EyeCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        if(Eyes == "Eye")
                        {
                            Eyes = "EyeOff";
                            passwordBox.Visibility = Visibility.Hidden;
                            passwordTbox.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            Eyes = "Eye";
                            passwordBox.Visibility = Visibility.Visible;
                            passwordTbox.Visibility = Visibility.Hidden;
                        }
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _EyeCommand;
            }
        }

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

        public string Eyes
        {
            get
            {
                return model.Eyes;
            }
            set
            {
                model.Eyes = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public bool RememberPassword
        {
            get
            {
                return model.RememberPassword;
            }
            set
            {
                model.RememberPassword = value;
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

                        if (!Regex.IsMatch(PassWord, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[%&',;=?$\x22]).{7,15}$"))
                        {
                            SendWorryMessage("错误的密码格式，请输入包含A-Z,a-z,且包含特殊字符的密码");
                            return;
                        }

                        var loginMessage = new UserLoginModel()
                        {
                            UserName = UserName,
                            PassWord = PassWord,
                            ComputerInfo = computerInfo,
                        };

                        var message = new MessageModel()
                        {
                            IsLogin = false,
                            JsonInfo = JsonInfo.Login,
                            UserName = UserName,
                            Message = FileService.SaveToJson(loginMessage),
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
                                Thread.Sleep(3000);
                                while (true)
                                {
                                    if (SocketModel.ClientKeys.ContainsKey(SocketViewModel.socket.RemoteEndPoint.ToString()))
                                    {
                                        if (!string.IsNullOrEmpty(SocketModel.ClientKeys[SocketViewModel.socket.RemoteEndPoint.ToString()].ServerSendKey.PublicKey))
                                        {
                                            if (!string.IsNullOrEmpty(SocketModel.ClientKeys[SocketViewModel.socket.RemoteEndPoint.ToString()].ClientSendKey.PrivetKey))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    Thread.Sleep(1000);
                                }

                                await SocketViewModel.SendRESMessage(MessageClass.Json, getJson, SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString());

                                var getFilePath = Directory.GetCurrentDirectory();

                                if (RememberPassword)
                                {
                                    rememberPassword.UserName = UserName;
                                    rememberPassword.Password = PassWord;
                                    rememberPassword.AutoLogin = true;
                                }
                                else
                                {
                                    rememberPassword.AutoLogin = false;

                                    rememberPassword.UserName = "";

                                    rememberPassword.Password = "";
                                }

                                FileService.ChangeFile(getFilePath + @"\config.json", FileService.SaveToJson(rememberPassword));
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
                    _LoadedCommand = new RelayCommand<Window>(async (wd) => {
                        Eyes = "Eye";
                        window = wd as LoginWindow;
                        ShowWorryMessage = new _ShowWorryMessage(SendWorryMessage);

                        if (SocketViewModel.socket == null)
                        {
                            await socketViewModel.StarSocketTCP();
                        }

                        var getFilePath = Directory.GetCurrentDirectory();

                        if (FileService.IsHaveFile(getFilePath + @"\config.json"))
                        {
                            try
                            {
                                rememberPassword = FileService.JsonToProp<RememberPassword>(FileService.GetFileText(getFilePath + @"\config.json"));
                            }
                            catch
                            {
                                rememberPassword = new RememberPassword()
                                {
                                    Password = "",
                                    UserName = "",
                                    AutoLogin = false,
                                };

                                FileService.ChangeFile(getFilePath + @"\config.json", FileService.SaveToJson(rememberPassword));
                            }
                        }
                        else
                        {
                            rememberPassword = new RememberPassword()
                            {
                                Password = "",
                                UserName = "",
                                AutoLogin = false,
                            };

                            FileService.ChangeFile(getFilePath + @"\config.json", FileService.SaveToJson(rememberPassword));
                        }

                        if (rememberPassword.AutoLogin)
                        {
                            rememberPassword.AutoLogin = true;

                            UserName = rememberPassword.UserName;

                            PassWord = rememberPassword.Password;
                        }
                        else
                        {
                            rememberPassword.AutoLogin = false;

                            rememberPassword.UserName = "";

                            rememberPassword.Password = "";
                        }
                    });
                }
                return _LoadedCommand;
            }
            set { _LoadedCommand = value; }
        }

        private RelayCommand<PasswordBox> _PswLoadedCommand;

        public RelayCommand<PasswordBox> PswLoadedCommand
        {
            get
            {
                if (_PswLoadedCommand == null)
                {
                    _PswLoadedCommand = new RelayCommand<PasswordBox>(async (pswBox) => {

                        pswBox.PasswordChanged += PswBox_PasswordChanged;
                        passwordBox = pswBox;

                        await Task.Run(() =>
                        {
                            while (rememberPassword == null)
                            {
                                Thread.Sleep(100);
                            }
                            
                        });
                        pswBox.Password = rememberPassword.Password;
                        RememberPassword = rememberPassword.AutoLogin;
                    });
                }
                return _PswLoadedCommand;
            }
            set { _PswLoadedCommand = value; }
        }

        private void PswBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            
            var pswBox = sender as PasswordBox;

            if (pswBox.Visibility == Visibility.Visible)
            {
                PassWord = pswBox.Password;
            }

        }

        private RelayCommand<TextBox> _PswTboxLoadedCommand;

        public RelayCommand<TextBox> PswTboxLoadedCommand
        {
            get
            {
                if (_PswTboxLoadedCommand == null)
                {
                    _PswTboxLoadedCommand = new RelayCommand<TextBox>((pswBox) => {

                        pswBox.TextChanged += PswBox_PasswordChanged1;

                        passwordTbox = pswBox;
                    });
                }
                return _PswTboxLoadedCommand;
            }
            set { _PswTboxLoadedCommand = value; }
        }

        private void PswBox_PasswordChanged1(object sender, RoutedEventArgs e)
        {
            var pswBox = sender as TextBox;

            if(pswBox.Visibility == Visibility.Visible)
            {
               PassWord = pswBox.Text;
               passwordBox.Password = PassWord;
            }
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
