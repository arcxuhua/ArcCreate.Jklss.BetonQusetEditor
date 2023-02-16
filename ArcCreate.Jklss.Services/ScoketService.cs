using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static ArcCreate.Jklss.Model.SocketModel.SocketModel;
using System.Threading;
using static ArcCreate.Jklss.Services.ScoketService;
using ArcCreate.Jklss.Model.SocketModel;
using System.Windows;
using System.Runtime.CompilerServices;

namespace ArcCreate.Jklss.Services
{
    public class ScoketService
    {
        public delegate void _GetMessage(MessageMode message,string token = "");

        /// <summary>
        /// 接受到消息
        /// </summary>
        public static event _GetMessage GetMessage;

        public static Socket socket = null;

        /// <summary>  
        /// 连接到服务器  
        /// </summary>  
        public Socket AsynConnect()
        {
            var host = Dns.GetHostAddresses("jklss.cn");
            //端口及IP  
            IPEndPoint ipe = new IPEndPoint(host[0], 6012);

            //IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6012);

            //创建套接字  
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //开始连接到服务器  

            client.BeginConnect(ipe, asyncResult =>
            {
                try
                {
                    client.EndConnect(asyncResult);
                    //接受消息  
                    AsynRecive(client);

                    socket = client;
                    saveSocketClient.Add(client.RemoteEndPoint.ToString(), client);//存储对话
                    HeartSend();
                }
                catch
                {
                    MessageBox.Show("服务器正在维护请等待维护完成！", "服务器维护中");

                    Environment.Exit(0);

                }

            }, null);

            return client;
        }

        /// <summary>  
        /// 发送消息  
        /// </summary>  
        /// <param name="socket"></param>  
        /// <param name="message"></param>  
        public void AsyncSend(Socket socket, string message)
        {
            if (socket == null || message == string.Empty) return;
            //编码  
            var data = addBytes(Encoding.UTF8.GetBytes(message), Encoding.UTF8.GetBytes("$$"));

            try
            {
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
                {
                    //完成发送消息  
                    int length = socket.EndSend(asyncResult);
                }, null);
            }
            catch (Exception ex)
            {
                
            }
        }

        public static Dictionary<int, Dictionary<int, byte[]>> fb = new Dictionary<int, Dictionary<int, byte[]>>();

        private static string saveNeed = string.Empty;

        /// <summary>  
        /// 接收消息  
        /// </summary>  
        /// <param name="socket"></param>  
        public void AsynRecive(Socket socket)
        {
            byte[] data = new byte[1024 * 1024];
            try
            {
                //开始接收数据  
                socket.BeginReceive(data, 0, data.Length, SocketFlags.None,
                asyncResult =>
                {
                    try
                    {
                        if (!socket.Connected)
                        {
                            return;
                        }

                        var length = socket.EndReceive(asyncResult);

                        var str = Encoding.UTF8.GetString(data, 0, length);
                        lock (str)
                        {
                            if (!ClientKeys.ContainsKey(socket.RemoteEndPoint.ToString()))
                            {
                                MessageMode sendMessage;
                                try
                                {
                                    var fg = str.Split(new string[] { "$$" }, StringSplitOptions.RemoveEmptyEntries);
                                    sendMessage = FileService.JsonToProp<MessageMode>(fg[0]);
                                    sendMessage.Ip = socket.RemoteEndPoint.ToString();
                                }
                                catch
                                {
                                    AsynRecive(socket);
                                    return;
                                }
                                GetMessage(sendMessage);
                            }
                            else
                            {
                                var keyModel = ClientKeys[socket.RemoteEndPoint.ToString()];

                                if (keyModel.ClientSendKey == null)
                                {
                                    AsynRecive(socket);
                                    return;
                                }

                                try
                                {
                                    var fgf = str.Substring(str.Length - 2, 2);

                                    var fg = str.Split(new string[] { "$$" }, StringSplitOptions.RemoveEmptyEntries);

                                    var getMessageModel = FileService.JsonToProp<MessageMode>(fg[0]);

                                    getMessageModel.Ip = socket.RemoteEndPoint.ToString();

                                    if (getMessageModel.Class != MessageClass.SendKey && getMessageModel.Class != MessageClass.Heart && getMessageModel.Class != MessageClass.Version)
                                    {
                                        AsynRecive(socket);
                                        return;
                                    }

                                    GetMessage(getMessageModel);
                                }
                                catch
                                {
                                    
                                    try
                                    {
                                        var fgf = str.Substring(str.Length - 2, 2);

                                        var fg = str.Split(new string[] { "$$" }, StringSplitOptions.RemoveEmptyEntries);

                                        for (int i = 0; i < fg.Length; i++)
                                        {
                                            if (i == 0)
                                            {
                                                fg[i] = saveNeed + fg[i];

                                                saveNeed = string.Empty;
                                            }
                                            
                                            
                                            MessageMode getMessageModels = null;

                                            try
                                            {
                                                var jm = FileService.PrivateKeyDecrypt(keyModel.ClientSendKey.PrivetKey, fg[i]);
                                                getMessageModels = FileService.JsonToProp<MessageMode>(jm);

                                                try
                                                {
                                                    SocketModel.waitBackDic[getMessageModels.Class][getMessageModels.Key] = jm;
                                                }
                                                catch
                                                {
                                                    getMessageModels.Ip = socket.RemoteEndPoint.ToString();

                                                    GetMessage(getMessageModels, getMessageModels.Token);
                                                }

                                            }
                                            catch
                                            {
                                                saveNeed = fg[i];

                                                AsynRecive(socket);
                                                return;
                                            }

                                            //if (getMessageModels.Length != 1)
                                            //{
                                            //    if (fb.TryGetValue(getMessageModels.Key, out Dictionary<int, byte[]> b))
                                            //    {
                                            //        fb[getMessageModels.Key].Add(Convert.ToInt32(getMessageModels.NowLength), getMessageModels.Message);

                                            //        if (fb[getMessageModels.Key].Count == getMessageModels.Length)
                                            //        {
                                            //            byte[] wzdata = new byte[0];

                                            //            for (int j = 1; j <= fb[getMessageModels.Key].Count; j++)
                                            //            {
                                            //                wzdata = addBytes(wzdata, fb[getMessageModels.Key][j]);
                                            //            }

                                            //            getMessageModels.Message = wzdata;

                                            //            try
                                            //            {
                                            //                SocketModel.waitBackDic[getMessageModels.Class][getMessageModels.Key] = FileService.SaveToYaml(getMessageModels);
                                            //                continue;
                                            //            }
                                            //            catch
                                            //            {
                                            //                getMessageModels.Ip = socket.RemoteEndPoint.ToString();

                                            //                GetMessage(getMessageModels, getMessageModels.Token);
                                            //                continue;
                                            //            }
                                            //        }
                                            //        else
                                            //        {
                                            //            continue;
                                            //        }
                                            //    }
                                            //    else
                                            //    {
                                            //        fb.Add(getMessageModels.Key, new Dictionary<int, byte[]>
                                            //                {
                                            //                    { Convert.ToInt32(getMessageModels.NowLength),getMessageModels.Message }
                                            //                });
                                            //        continue;
                                            //    }

                                            //}

                                            
                                        }
                                    }
                                    catch
                                    {
                                        AsynRecive(socket);
                                        return;
                                    }
                                }
                            }
                        }
                        AsynRecive(socket);
                    }
                    catch
                    {
                        MessageBox.Show("您已被服务器强制中断连接！","警告");
                        Environment.Exit(0);
                    }
                    
                }, null);
            }
            catch (Exception ex)
            {
                
            }
        }
        public static byte[] addBytes(byte[] data1, byte[] data2)
        {
            byte[] data3 = new byte[data1.Length + data2.Length];
            data1.CopyTo(data3, 0);
            data2.CopyTo(data3, data1.Length);
            return data3;
        }
        public async void HeartSend()
        {
            var sendModel = new MessageMode()
            {
                Class = Model.SocketModel.MessageClass.Heart,
                
                Ip = socket.LocalEndPoint.ToString(),
                Length =1,
                NowLength= 1,
            };

            var toJson = FileService.SaveToJson(sendModel);

            await Task.Run(() =>
            {
                while (true)
                {
                    AsyncSend(socket, toJson);
                    Thread.Sleep(1000);
                }
            });
        }

    }
}
