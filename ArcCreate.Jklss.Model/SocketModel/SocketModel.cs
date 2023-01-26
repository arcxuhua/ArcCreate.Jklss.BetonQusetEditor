using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Model.SocketModel
{
    public class SocketModel
    {
        public static Dictionary<string, Socket> saveSocketClient = new Dictionary<string, Socket>();

        public static Dictionary<string, SaveClientKeys> ClientKeys = new Dictionary<string, SaveClientKeys>();

        public static Dictionary<string, MessageMode> cliendSend = new Dictionary<string, MessageMode>();

        public class SaveClientKeys
        {
            public string Ip { get; set; }

            public RESKeysModel ServerSendKey { get; set; }

            public RESKeysModel ClientSendKey { get; set; }
        }

        public class RESKeysModel
        {
            public string PublicKey { get; set; }

            public string PrivetKey { get; set; }
        }

        public class MessageMode : IMessageModel
        {
            public MessageClass Class { get; set; }
            public string Ip { get; set; } = string.Empty;
            public byte[] Message { get; set; } = new byte[0];
            public uint Length { get; set; }
            public uint NowLength { get; set; }
        }

        public class MessageModel
        {
            public bool IsLogin { get; set; }

            public string UserName { get; set; }

            public string Message { get; set; }
        }
    }
}
