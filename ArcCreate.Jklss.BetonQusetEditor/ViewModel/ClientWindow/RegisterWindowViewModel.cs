using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.BetonQusetEditor.Windows;
using ArcCreate.Jklss.Model.ClientModel;
using ArcCreate.Jklss.Model.SocketModel;
using ArcCreate.Jklss.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static ArcCreate.Jklss.BetonQusetEditor.ViewModel.ClientWindow.LoginWindowViewModel;
using static ArcCreate.Jklss.Model.SocketModel.SocketModel;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.ClientWindow
{
    public class RegisterWindowViewModel : NotifyBase
    {
        private static RegisterModel model = new RegisterModel();

        public static RegisterWindow window = null;

        #region 属性
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

        public string Activation
        {
            get
            {
                return model.Activation;
            }
            set
            {
                model.Activation = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public bool RegisterEnabled
        {
            get
            {
                return model.RegisterEnabled;
            }
            set
            {
                model.RegisterEnabled = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public string UserNameCheckIco
        {
            get
            {
                return model.UserNameCheckIco;
            }
            set
            {
                model.UserNameCheckIco = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public string UserNameChecked
        {
            get
            {
                return model.UserNameChecked;
            }
            set
            {
                model.UserNameChecked = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public string UserNameWorry
        {
            get
            {
                return model.UserNameWorry;
            }
            set
            {
                model.UserNameWorry = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public string PasswordCheckIco
        {
            get
            {
                return model.PasswordCheckIco;
            }
            set
            {
                model.PasswordCheckIco = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public string PasswordChecked
        {
            get
            {
                return model.PasswordChecked;
            }
            set
            {
                model.PasswordChecked = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public string PasswordWorry
        {
            get
            {
                return model.PasswordWorry;
            }
            set
            {
                model.PasswordWorry = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public string ActivationCheckIco
        {
            get
            {
                return model.ActivationCheckIco;
            }
            set
            {
                model.ActivationCheckIco = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public string ActivationChecked
        {
            get
            {
                return model.ActivationChecked;
            }
            set
            {
                model.ActivationChecked = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public string ActivationWorry
        {
            get
            {
                return model.ActivationWorry;
            }
            set
            {
                model.ActivationWorry = value;
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

        #endregion

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
                        AnimationBase.Disappear(obj as Window);

                        await Task.Run(() =>
                        {
                            while ((obj as Window).Visibility == Visibility.Visible)
                            {
                                Thread.Sleep(100);
                            }
                        });

                        var window = new LoginWindow();

                        AnimationBase.Appear(window,duration:1);

                        (obj as Window).Close();
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _LoginCommand;
            }
        }

        public CommandBase _PayCommand;
        public CommandBase PayCommand
        {
            get
            {
                if (_PayCommand == null)
                {
                    _PayCommand = new CommandBase();
                    _PayCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        var window = new PayWindow();
                        window.Show();
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _PayCommand;
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

        public CommandBase _RegisterCommand;
        public CommandBase RegisterCommand
        {
            get
            {
                if (_RegisterCommand == null)
                {
                    _RegisterCommand = new CommandBase();
                    _RegisterCommand.DoExecute = new Action<object>(async obj =>//回调函数
                    {
                        if (string.IsNullOrEmpty(UserName))
                        {
                            return;
                        }

                        if (string.IsNullOrEmpty(PassWord))
                        {
                            return;
                        }
                        if (!Regex.IsMatch(UserName, @"^(\w)+(\.\w+)*@(\w)+((\.\w{2,3}){1,3})$"))
                        {
                            return;
                        }

                        if (!Regex.IsMatch(PassWord, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[%&',;=?$\x22]).{8,16}$"))
                        {
                            return;
                        }

                        if (Activation.Length != 44)
                        {
                            return;
                        }
                        window.First.IsEnabled = false;
                        var registerModel = new UserRegisterModel()
                        {
                            UserName = UserName,
                            PassWord = PassWord,
                            Activation = Activation,
                            ComputerInfo = LoginWindowViewModel.computerInfo,
                            BackMessage = "注册流程"
                        };

                        var message = new MessageModel()
                        {
                            IsLogin = false,
                            JsonInfo = JsonInfo.Register,
                            UserName = UserName,
                            Message = FileService.SaveToJson(registerModel),
                        };

                        var getJson = FileService.SaveToJson(message);

                        try
                        {
                            if (SocketViewModel.socket == null)
                            {
                                await LoginWindowViewModel.socketViewModel.StarSocketTCP();
                            }

                            await Task.Run(async () =>
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

                                await LoginWindowViewModel.socketViewModel.SendRESMessage(MessageClass.Json, getJson, SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString());

                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            window.First.IsEnabled = true;
                        }

                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _RegisterCommand;
            }
        }

        private RelayCommand<TextBox> _UserNameTboxLoadedCommand;

        public RelayCommand<TextBox> UserNameTboxLoadedCommand
        {
            get
            {
                if (_UserNameTboxLoadedCommand == null)
                {
                    _UserNameTboxLoadedCommand = new RelayCommand<TextBox>((tBox) => {

                        tBox.TextChanged += TBox_TextChanged;
                    });
                }
                return _UserNameTboxLoadedCommand;
            }
            set { _UserNameTboxLoadedCommand = value; }
        }

        private void TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserName = (sender as TextBox).Text;
            if (!Regex.IsMatch(UserName, @"^(\w)+(\.\w+)*@(\w)+((\.\w{2,3}){1,3})$"))
            {
                UserNameCheckIco = "AlertCircleOutline";
                UserNameChecked = "Red";
                UserNameWorry = "请填写正确\n的邮箱";
            }
            else
            {
                UserNameCheckIco = "CheckCircleOutline";
                UserNameChecked = "Green";
                UserNameWorry = "";
            }
        }

        private RelayCommand<TextBox> _PasswordTboxLoadedCommand;

        public RelayCommand<TextBox> PasswordTboxLoadedCommand
        {
            get
            {
                if (_PasswordTboxLoadedCommand == null)
                {
                    _PasswordTboxLoadedCommand = new RelayCommand<TextBox>((tBox) => {

                        tBox.TextChanged += TBox_TextChanged1;
                    });
                }
                return _PasswordTboxLoadedCommand;
            }
            set { _PasswordTboxLoadedCommand = value; }
        }

        private void TBox_TextChanged1(object sender, TextChangedEventArgs e)
        {
            PassWord = (sender as TextBox).Text;
            if (!Regex.IsMatch(PassWord, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[%&',;=?$\x22]).{8,16}$"))
            {
                PasswordCheckIco = "AlertCircleOutline";
                PasswordChecked = "Red";
                PasswordWorry = "密码需包含：\n大小写字母\n0-9的数字\n特殊字符";
            }
            else
            {
                PasswordCheckIco = "CheckCircleOutline";
                PasswordChecked = "Green";
                PasswordWorry = "";
            }
        }

        private RelayCommand<TextBox> _ActivationTboxLoadedCommand;

        public RelayCommand<TextBox> ActivationTboxLoadedCommand
        {
            get
            {
                if (_ActivationTboxLoadedCommand == null)
                {
                    _ActivationTboxLoadedCommand = new RelayCommand<TextBox>((tBox) => {

                        tBox.TextChanged += TBox_TextChanged2;
                    });
                }
                return _ActivationTboxLoadedCommand;
            }
            set { _ActivationTboxLoadedCommand = value; }
        }

        private void TBox_TextChanged2(object sender, TextChangedEventArgs e)
        {
            Activation = (sender as TextBox).Text;

            if (Activation.Length != 44)
            {
                ActivationCheckIco = "AlertCircleOutline";
                ActivationChecked = "Red";
                ActivationWorry = "激活码验证失败";
            }
            else
            {
                ActivationCheckIco = "CheckCircleOutline";
                ActivationChecked = "Green";
                ActivationWorry = "";
            }
        }

        private RelayCommand<Window> _LoadedCommand;

        public RelayCommand<Window> LoadedCommand
        {
            get
            {
                if (_LoadedCommand == null)
                {
                    _LoadedCommand = new RelayCommand<Window>((wd) => {
                        
                        window = wd as RegisterWindow;

                        ShowWorryMessage = new _ShowWorryMessage(SendWorryMessage);
                    });
                }
                return _LoadedCommand;
            }
            set { _LoadedCommand = value; }
        }

        public delegate void _ShowWorryMessage(string txt);

        public static _ShowWorryMessage ShowWorryMessage;

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

                        if (window.Worry.Visibility == Visibility.Visible)
                        {
                            AnimationBase.Disappear(window.Worry);
                        }
                        break;
                    case 1:
                        AnimationBase.Appear(window.Worry);

                        if (window.First.Visibility == Visibility.Visible)
                        {
                            AnimationBase.Disappear(window.First);
                        }
                        break;
                }
            }));
        }

        public void SendWorryMessage(string txt)
        {
            ShowPage(1);

            WorryMessage = txt;

            window.Dispatcher.Invoke(new Action(() =>
            {
                window.First.IsEnabled = true;
            }));
            
        }
    }
}
