using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Model.SocketModel
{
    public enum MessageClass
    {
        Json,
        File,
        Image,
        Heart,
        SendKey,
        Version
    }
    public interface IMessageModel
    {
        /// <summary>
        /// 数据包属性
        /// </summary>
        MessageClass Class { get; set; }

        /// <summary>
        /// 发送者IP
        /// </summary>
        string Ip { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        byte[] Message { get; set; }

        /// <summary>
        /// 数据包总长度
        /// </summary>
        uint Length { get; set; }

        /// <summary>
        /// 当前数据包
        /// </summary>
        uint NowLength { get; set; }
    }
}
