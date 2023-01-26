using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.BetonQusetEditor.Windows;
using ArcCreate.Jklss.Model.ClientModel;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.SocketModel;
using ArcCreate.Jklss.Services;
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
                        if (string.IsNullOrEmpty(UserName))
                        {
                            return;
                        }

                        if (string.IsNullOrEmpty(PassWord))
                        {
                            return;
                        }

                        if(!Regex.IsMatch(UserName, @"^(\w)+(\.\w+)*@(\w)+((\.\w{2,3}){1,3})$"))
                        {
                            return;
                        }

                        if (!Regex.IsMatch(PassWord, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^%&',;=?$\x22]).{8,15}$"))
                        {
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
                            MessageBox.Show(ex.Message);
                        }
                        
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _LoginCommand;
            }
        }
    }
}
