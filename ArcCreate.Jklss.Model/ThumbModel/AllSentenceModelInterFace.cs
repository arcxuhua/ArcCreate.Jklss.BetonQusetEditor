using ArcCreate.Jklss.Model.MainWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Model.ThumbModel
{
    /// <summary>
    /// 语句基本构造器接口
    /// </summary>
    public interface AllSentenceModelInterFace
    {

        /// <summary>
        /// 主命令
        /// </summary>
        string MainClass { get; set; }

        /// <summary>
        /// 第二参数分割符
        /// </summary>
        char TextSplitChar { get; set; }

        /// <summary>
        /// 第二参数步长
        /// </summary>
        int TextNum { get; set; }

        /// <summary>
        /// 第n条参数同时存储分隔符以及步长
        /// </summary>
        List<(char i,int j)> TextSplitWords { get; set; }

        /// <summary>
        /// 是否有参数需要且仅需要 条件、事件、目标 其中一个类型
        /// </summary>
        Dictionary<string,Dictionary<int, ThumbClass>> NeedTpye { get; set; }
        /// <summary>
        /// 多条子命令结构
        /// </summary>
        List<ChildClasses> ChildClasses { get; set; }
    }

    /// <summary>
    /// 子命令存储结构
    /// </summary>
    public class ChildClasses
    {
        /// <summary>
        /// 主命令截止子命令
        /// </summary>
        public string ChildClass { get; set; }

        /// <summary>
        /// 子命令第二参数分割符
        /// </summary>
        public char ChildTextSplitChar { get; set; }

        /// <summary>
        /// 子命令第二参数步长
        /// </summary>
        public int ChildTextNum { get; set; }

        /// <summary>
        /// 子命令第n条参数同时存储分隔符及步长
        /// </summary>
        public List<(char i, int j)> ChildTextSplitWords { get; set; }
    }
}
