﻿using System;
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

namespace ArcCreate.Jklss.Services
{
    public class ScoketService
    {
        public delegate void _GetMessage(MessageMode message);

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
            //端口及IP  
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6012);
            //创建套接字  
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //开始连接到服务器  
            client.BeginConnect(ipe, asyncResult =>
            {
                client.EndConnect(asyncResult);
                //接受消息  
                AsynRecive(client);

                socket = client;
                saveSocketClient.Add(client.RemoteEndPoint.ToString(), client);//存储对话
                HeartSend();
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
            var data = Encoding.UTF8.GetBytes(message);
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

        /// <summary>  
        /// 接收消息  
        /// </summary>  
        /// <param name="socket"></param>  
        public void AsynRecive(Socket socket)
        {
            byte[] data = new byte[1024 * 50];
            try
            {
                //开始接收数据  
                socket.BeginReceive(data, 0, data.Length, SocketFlags.None,
                asyncResult =>
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
                                sendMessage = FileService.JsonToProp<MessageMode>(str);
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
                                var getMessageModel = FileService.JsonToProp<MessageMode>(str);

                                if (getMessageModel.Class != MessageClass.SendKey && getMessageModel.Class != MessageClass.Heart)
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
                                    var jm = FileService.PrivateKeyDecrypt(keyModel.ClientSendKey.PrivetKey, str);

                                    var getMessageModel = FileService.JsonToProp<MessageMode>(jm);

                                    GetMessage(getMessageModel);
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
                }, null);
            }
            catch (Exception ex)
            {
                
            }
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
