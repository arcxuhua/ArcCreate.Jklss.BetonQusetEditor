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
        public static Dictionary<MessageClass, Dictionary<int, string>> waitBackDic = new Dictionary<MessageClass, Dictionary<int, string>>();

        public static string token = string.Empty;

        public static bool isLogin = false;

        public static string password = string.Empty;

        public static string userName = string.Empty;

        public static Dictionary<string, Socket> saveSocketClient = new Dictionary<string, Socket>();

        public static Dictionary<string, SaveClientKeys> ClientKeys = new Dictionary<string, SaveClientKeys>();

        public static Dictionary<string, MessageMode> cliendSend = new Dictionary<string, MessageMode>();
    }

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
        public string Token { get; set; }
        public int Key { get; set; }
    }

    public enum JsonInfo
    {
        Login,
        Register,
        Message,
        GetConditonModel,
        GetEventModel,
        GetObjectiveModel,
        ConditionAnalysis,
        EventAnalysis,
        ObjectiveAnalysis,
        PlayerAndNpcAnalysis,
        SaveToYaml,
        SaveToJson,
        GetSaveData,
        DeleteSaveData,
    }

    public class MessageModel
    {
        public bool IsLogin { get; set; }

        public JsonInfo JsonInfo { get; set; }

        public string UserName { get; set; }

        public string Message { get; set; }

        public string Other { get; set; }
    }

    public class config
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// 下载地址
        /// </summary>
        public string update_path { get; set; }
    }
}
