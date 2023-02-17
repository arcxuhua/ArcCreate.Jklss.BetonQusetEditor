using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ArcCreate.Jklss.Model.MainWindow
{
    public class MainWindowModels
    {
        public List<ThumbClass> SearchListType { get; set; }
        public ThumbClass SearchType { get; set; }
        public string LoadingMessage { get; set; }
        public Visibility LoadingShow { get; set; } = Visibility.Hidden;
        public string SearchText { get; set; }
        public bool IsFindFile { get; set; } = false;
        public bool isHaveSubjcet { get; set; } = false;
        public string Message { get; set; }
        public string MainFilePath { get; set; }
        public string ThumbNums { get; set; }
        public string ThumbChecks { get; set; }

        public Dictionary<Thumb,Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string,string>>>>> SaveThumbInfo { get; set; } = new Dictionary<Thumb, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>();

        public static List<SaveChird> saveThumbs = new List<SaveChird>();
        public class SaveChird
        {
            public Thumb Saver { get; set; }

            public List<Thumb> Children { get; set; }

            public List<Thumb> Fathers { get; set; }

            public bool CanFather { get; set; }

            public Thumb Main { get; set; }

            public ThumbClass thumbClass { get; set; }
        }
    }

    public enum ThumbClass
    {
        /// <summary>
        /// 主体
        /// </summary>
        Subject,
        /// <summary>
        /// NPC对话
        /// </summary>
        NPC,
        /// <summary>
        /// 玩家对话
        /// </summary>
        Player,
        /// <summary>
        /// 条件
        /// </summary>
        Conditions,
        /// <summary>
        /// 事件
        /// </summary>
        Events,
        /// <summary>
        /// 物品
        /// </summary>
        Items,
        /// <summary>
        /// 日记
        /// </summary>
        Journal,
        /// <summary>
        /// 目标
        /// </summary>
        Objectives
    }

    public class SaveJsonModel
    {
        public int id { get;set; }
        public string text { get; set; }
        public string filepath { get; set; }
        public string Main { get; set; }

        public string Maininfo { get; set; }

        public string Data { get; set; }

        public string Jdata { get; set; }

        public string Idata { get; set; }

        public string Coordinate { get; set; }
    }

    public class YamlSaver
    {
        public string Main { get; set; }

        public Dictionary<string, string> Conversations { get; set; } = new Dictionary<string, string>();

        public string Conditions { get; set; }

        public string Events { get; set; }

        public string Items { get; set; }

        public string Journal { get; set; }

        public string Objectives { get; set; }
    }

    public partial class savedatum
    {
        /// <summary>
        /// 存储数据id
        /// </summary>
        public int cid { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public int uid { get; set; }

        /// <summary>
        /// 数据说明
        /// </summary>
        public string explain { get; set; }

        /// <summary>
        /// 本地存储地址
        /// </summary>
        public string filepath { get; set; }

        /// <summary>
        /// 卡片坐标
        /// </summary>
        public string coordinate { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public string data { get; set; }

        /// <summary>
        /// 物品数据
        /// </summary>
        public string itemsdata { get; set; }

        /// <summary>
        /// 日记数据
        /// </summary>
        public string journaldata { get; set; }

        /// <summary>
        /// main数据
        /// </summary>
        public string main { get; set; }

        /// <summary>
        /// main其他数据
        /// </summary>
        public string maininfo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_date { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime update_date { get; set; }
    }
}
