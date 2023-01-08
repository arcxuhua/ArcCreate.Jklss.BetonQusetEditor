using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace ArcCreate.Jklss.Model.ThumbModel
{
    public class ConversationsModel
    {
        /// <summary>
        /// 对话框名称
        /// </summary>
        public string quester { get; set; }
        /// <summary>
        /// 首选对话
        /// </summary>
        public string first { get; set; }
        /// <summary>
        /// 不清楚填false就对了
        /// </summary>
        public bool stop { get; set; }
        /// <summary>
        /// NPC说的话
        /// </summary>
        public Dictionary<string, AllTalk> NPC_options { get; set; } = new Dictionary<string, AllTalk>();
        /// <summary>
        /// 玩家说的话
        /// </summary>
        public Dictionary<string, AllTalk> player_options { get; set; } = new Dictionary<string, AllTalk>();
    }

    public class AllTalk
    {
        /// <summary>
        /// 对话文本
        /// </summary>
        public List<string> text { get; set; }
        /// <summary>
        /// 需要条件
        /// </summary>
        public string conditions { get; set; }
        /// <summary>
        /// 选项
        /// </summary>
        public string pointer { get; set; }
        /// <summary>
        /// 对话后触发事件
        /// </summary>
        public string events { get; set; }
    }

    public class MainConfigModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> npcs { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        public Cancel cancel { get; set; }

    }

    public class Cancel
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, Wood> wood { get; set; }

    }

    public class Wood
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string conditions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string objectives { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string tags { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string journal { get; set; }

    }

    public class AllConfigModel
    {
        public MainConfigModel mainConfigModel { get; set; }

        public Dictionary<Thumb, ConversationsModel> allTalk { get; set; } = new Dictionary<Thumb, ConversationsModel>();

        public string conditions { get; set; }

        public string events { get; set; }

        public string objcetives { get; set; }

        public string journal { get; set; }

        public string items { get; set; }
    }
}
