using ArcCreate.Jklss.Model.SocketModel;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Services;
using System.Text;
using System.Threading.Tasks;
using static ArcCreate.Jklss.Model.SocketModel.SocketModel;
using System.Net.Sockets;
using System.Windows;
using ArcCreate.Jklss.BetonQusetEditor.ViewModel.ClientWindow;
using Newtonsoft.Json.Linq;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel
{
    public class SocketViewModel
    {
        public static Socket socket;

        public static ScoketService socketService = new ScoketService();

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

        private async void ScoketService_GetMessage(MessageMode message)
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

                if(model.JsonInfo == JsonInfo.Login)
                {
                    if (!model.IsLogin)
                    {
                        LoginWindowViewModel.ShowWorryMessage(model.Message);

                        return;
                    }
                    LoginWindowViewModel.window.Dispatcher.Invoke(new System.Action(() =>
                    {
                        MainWindow window = new MainWindow();
                        window.Show();
                        LoginWindowViewModel.window.Close();
                    }));
                    
                }

                if(model.JsonInfo == JsonInfo.Register)
                {
                    MessageBox.Show(model.Message);
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
            else if(message.Class == MessageClass.SendKey)
            {
                var clientKeys = FileService.CreateKey(KeyType.XML,KeySize.BIG);

                if (ClientKeys.ContainsKey(message.Ip))
                {
                    ClientKeys[message.Ip].ServerSendKey = new RESKeysModel()
                    {
                        PublicKey = Encoding.UTF8.GetString(message.Message)
                    };
                    ClientKeys[message.Ip].ClientSendKey = clientKeys;
                }
                else
                {
                    ClientKeys.Add(message.Ip, new SaveClientKeys()
                    {
                        Ip = message.Ip,
                        ServerSendKey = new RESKeysModel()
                        {
                            PublicKey = Encoding.UTF8.GetString(message.Message)
                        },
                        ClientSendKey = clientKeys
                    });
                }
                await SendRESMessage(MessageClass.SendKey, clientKeys.PublicKey, socket.LocalEndPoint.ToString(),message.Ip);
            }
            else
            {
                return;
            }
            lock (cliendSend)
            {
                if (cliendSend.ContainsKey(message.Ip))
                {
                    cliendSend[message.Ip] = message;
                }
                else
                {
                    cliendSend.Add(message.Ip, message);
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
                socketService.AsyncSend(saveSocketClient[ip], message);
            });

            result.SetSuccese("");

            return result;
        }

        /// <summary>
        /// 向客户端发送消息（RES加密）
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async Task<ReturnModel> SendRESMessage(MessageClass messageClass,string message, string sendip,string keyIp)
        {
            var result = new ReturnModel();

            var data = Encoding.UTF8.GetBytes(message);

            var sendModel = new MessageMode()
            {
                Class = messageClass,
                Ip = sendip,
                Message = data,
                Length = 1,
                NowLength = 1,
            };

            var send = FileService.SaveToJson(sendModel);

            var publicKey = ClientKeys[keyIp].ServerSendKey.PublicKey;

            if (string.IsNullOrEmpty(publicKey))
            {
                result.SetError("未找到加密公匙");

                return result;
            }

            await Task.Run(() =>
            {
                var getJM = FileService.PublicKeyEncrypt(publicKey, send);

                socketService.AsyncSend(saveSocketClient[keyIp], getJM);
            });

            result.SetSuccese("");

            return result;
        }
    }
}
