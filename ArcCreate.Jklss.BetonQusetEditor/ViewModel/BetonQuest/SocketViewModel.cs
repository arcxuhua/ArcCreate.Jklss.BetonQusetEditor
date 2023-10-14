using ArcCreate.Jklss.Model.SocketModel;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Services;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System;
using System.Threading;
using ArcCreate.Jklss.BetonQusetEditor.View;
using ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest.ClientWindow;
using ArcCreate.Jklss.BetonQusetEditor.View.ShowTool;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest
{
    public class SocketViewModel
    {
        public static Socket socket;

        public static ScoketService socketService = new ScoketService();

        public static string version = "4.0.4.5";

        /// <summary>
        /// 开启Socket通讯
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnModel> StarSocketTCP()
        {
            var result = new ReturnModel();

            await Task.Run(() =>
            {
                socket = socketService.AsynConnect();

                ScoketService.GetMessage += ScoketService_GetMessage;
            });

            result.SetSuccese("");

            return result;
        }

        private async void ScoketService_GetMessage(MessageMode message, string token = "")
        {
            if (message == null)
            {
                return;
            }
            if (message.Class == MessageClass.Json)
            {
                var getMessage = Encoding.UTF8.GetString(message.Message);

                MessageModel model = null;

                try
                {
                    model = FileService.JsonToProp<MessageModel>(getMessage);
                }
                catch
                {
                    return;
                }

                if (model == null)
                {
                    return;
                }

                if (model.JsonInfo == JsonInfo.Login)
                {
                    if (!model.IsLogin)
                    {

                        if (model.Other == "1")
                        {
                            LoginWindowViewModel.ShowWorryMessage(model.Message, true);
                        }
                        else
                        {
                            LoginWindowViewModel.ShowWorryMessage(model.Message);
                        }


                        return;
                    }

                    SocketModel.token = token;
                    SocketModel.isLogin = model.IsLogin;
                    SocketModel.userName = model.UserName;

                    if (!string.IsNullOrEmpty(model.Other))
                    {
                        MessageBox.Show($"今日的赠送已经到账哦~\n今日获得: {model.Other} 点积分");
                    }


                    LoginWindowViewModel.window.Dispatcher.Invoke(new Action(() =>
                    {
                        ShowToolMainWindow window = new ShowToolMainWindow();
                        window.Show();
                        
                        LoginWindowViewModel.window.Close();
                    }));

                }

                if (model.JsonInfo == JsonInfo.Register)
                {
                    if (model.IsLogin)
                    {
                        MessageBox.Show(model.Message);
                        LoginWindowViewModel.window.Dispatcher.Invoke(new Action(() =>
                        {
                            var window = new EmailWindow();
                            window.Show();

                            RegisterWindowViewModel.window.Close();
                        }));
                    }
                    else
                    {
                        RegisterWindowViewModel.ShowWorryMessage(model.Message);
                    }

                }


            }
            else if (message.Class == MessageClass.File)
            {

            }
            else if (message.Class == MessageClass.Image)
            {

            }
            else if (message.Class == MessageClass.Heart)
            {

            }
            else if (message.Class == MessageClass.SendKey)
            {
                var clientKeys = FileService.CreateKey(KeyType.XML, KeySize.BIG);

                if (SocketModel.ClientKeys.ContainsKey(message.Ip))
                {
                    SocketModel.ClientKeys[message.Ip].ServerSendKey = new RESKeysModel()
                    {
                        PublicKey = Encoding.UTF8.GetString(message.Message)
                    };
                    SocketModel.ClientKeys[message.Ip].ClientSendKey = clientKeys;
                }
                else
                {
                    SocketModel.ClientKeys.Add(message.Ip, new SaveClientKeys()
                    {
                        Ip = message.Ip,
                        ServerSendKey = new RESKeysModel()
                        {
                            PublicKey = Encoding.UTF8.GetString(message.Message)
                        },
                        ClientSendKey = clientKeys
                    });
                }
                await SendRESMessage(MessageClass.SendKey, clientKeys.PublicKey, socket.LocalEndPoint.ToString(), message.Ip);
            }
            else if (message.Class == MessageClass.Version)
            {
                var getMessage = FileService.JsonToProp<config>(Encoding.UTF8.GetString(message.Message));

                if (getMessage.version == version)
                {
                    return;
                }

                var updateExePath = Directory.GetCurrentDirectory() + @"\update.exe";

                try
                {
                    ProcessStartInfo versionUpdatePrp = new ProcessStartInfo(updateExePath, getMessage.update_path);

                    Process newProcess = new Process();
                    newProcess.StartInfo = versionUpdatePrp;
                    newProcess.Start();

                    Environment.Exit(0);
                }
                catch
                {
                    LoginWindowViewModel.ShowWorryMessage("正在准备更新程序");
                    LoginWindowViewModel.window.Dispatcher.Invoke(new Action(() =>
                    {
                        LoginWindowViewModel.window.IsEnabled = false;
                    }));
                    

                    var downloader = new DownLoadController();

                    await Task.Run(async () =>
                    {
                        downloader.AddDownLoad(new DownloadInfo() { Url = "https://www.jklss.cn/BqEditer/update.exe", path = updateExePath });

                        await downloader.DownloadProgress(1);

                        while (!downloader.GetEndDownload()) Thread.Sleep(3000);

                        Thread.Sleep(3000);
                    });

                    ProcessStartInfo versionUpdatePrp = new ProcessStartInfo(updateExePath, getMessage.update_path);

                    Process newProcess = new Process();
                    newProcess.StartInfo = versionUpdatePrp;
                    newProcess.Start();

                    Environment.Exit(0);
                }

            }
            else
            {
                return;
            }


            lock (SocketModel.cliendSend)
            {
                if (SocketModel.cliendSend.ContainsKey(message.Ip))
                {
                    SocketModel.cliendSend[message.Ip] = message;
                }
                else
                {
                    SocketModel.cliendSend.Add(message.Ip, message);
                }
            }
        }

        /// <summary>
        /// 向客户端发送消息（不加密）仅心跳包使用
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async Task<ReturnModel> SendMessage(string message, string ip)
        {
            var result = new ReturnModel();

            await Task.Run(() =>
            {
                socketService.AsyncSend(SocketModel.saveSocketClient[ip], message);
            });

            result.SetSuccese("");

            return result;
        }

        private static int id = 10000;

        /// <summary>
        /// 向客户端发送消息（RES加密）可等待返回值
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static async Task<ReturnModel> SendRESMessage(MessageClass messageClass, string message, string sendip, string keyIp, string token = "", bool waitBack = false)
        {
            var result = new ReturnModel();

            var data = Encoding.UTF8.GetBytes(message);

            if (SocketModel.waitBackDic.ContainsKey(messageClass))
            {
                while(!SocketModel.waitBackDic[messageClass].TryAdd(id, string.Empty))
                {
                    id++;
                }

            }
            else
            {
                SocketModel.waitBackDic.Add(messageClass, new Dictionary<int, string>
                {
                    {id,string.Empty }
                });
            }

            var sendModel = new MessageMode()
            {
                Key = id,
                Token = token,
                Class = messageClass,
                Ip = sendip,
                Message = data,
                Length = 1,
                NowLength = 1,
            };

            var send = FileService.SaveToJson(sendModel);

            var publicKey = SocketModel.ClientKeys[keyIp].ServerSendKey.PublicKey;

            if (string.IsNullOrEmpty(publicKey))
            {
                result.SetError("未找到加密公匙");

                return result;
            }

            var getJM = FileService.PublicKeyEncrypt(publicKey, send);

            socketService.AsyncSend(SocketModel.saveSocketClient[keyIp], getJM);

            if (waitBack)
            {
                var value = await WaitBack(messageClass, id);

                id++;
                return value;

            }

            result.SetSuccese("");

            return result;
        }

        private static async Task<ReturnModel> WaitBack(MessageClass messageClass, int id)
        {
            var result = new ReturnModel();

            await Task.Run(() =>
            {
                var waitTimes = 0;

                while (waitTimes<10)
                {
                    Thread.Sleep(300);//延迟50毫秒
                    if (!string.IsNullOrEmpty(SocketModel.waitBackDic[messageClass][id]))
                    {
                        break;
                    }

                    waitTimes++;
                }
            });

            if (!string.IsNullOrEmpty(SocketModel.waitBackDic[messageClass][id]))
            {
                result.SetSuccese("ok", SocketModel.waitBackDic[messageClass][id]);
            }
            else
            {
                result.SetError("请求超时！请尝试重新发送请求");
            }


            return result;
        }

        /// <summary>
        /// 发送消息的二次封装（建议使用）返回值
        /// </summary>
        /// <param name="messageClass">消息类型</param>
        /// <param name="message">消息内容</param>
        /// <param name="times">失败后再次发送次数默认5次</param>
        /// <returns></returns>
        public static async Task<ReturnModel> EazySendRESMessage(MessageModel message,int times = 5, MessageClass messageClass = MessageClass.Json)
        {
            var result = new ReturnModel();

            var jsonMessage = FileService.SaveToJson(message);

            var getMessage = await SocketViewModel.SendRESMessage(messageClass, jsonMessage,
                SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);//取返回值

            var sendTimes = 0;

            while ((getMessage == null || !getMessage.Succese)&& (sendTimes < times))
            {
                getMessage = await SocketViewModel.SendRESMessage(messageClass, jsonMessage,
                SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);//取返回值

                sendTimes++;
            }

            if (getMessage == null || !getMessage.Succese)
            {
                return getMessage;
            }

            var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

            if (getModel.Token != SocketModel.token)
            {
                result.SetError("您的Token异常，请重新登录客户端！");

                return result;
            }

            var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

            if (getRealMessage == null || getRealMessage.JsonInfo != message.JsonInfo || !getRealMessage.IsLogin)
            {
                if(getRealMessage != null)
                {
                    result.SetError(getRealMessage.Message);

                    return result;
                }

                result.SetError("解析服务端返回值错误，请尝试重新请求");

                return result;
            }

            try
            {
                result.SetSuccese("ok", getRealMessage);

                return result;
            }
            catch
            {
                result.SetError("出乎意料的报错！");

                return result;
            }
        }
    }
}
