using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace ArcCreate.Jklss.Model.MainWindow
{
    public class MainWindowModels
    {
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
}
