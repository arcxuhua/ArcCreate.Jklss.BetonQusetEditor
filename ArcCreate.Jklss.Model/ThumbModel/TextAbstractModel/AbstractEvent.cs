using ArcCreate.Jklss.Model.MainWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Model.ThumbModel.TextAbstractModel
{
    /// <summary>
    /// Evets语法构造器模型
    /// </summary>
    public abstract class AbstractEvent : AllSentenceModelInterFace
    {
        public abstract string MainClass { get; set; }

        public abstract char TextSplitChar { get; set; }

        public abstract int TextNum { get; set; }
        /// <summary>
        /// 第二参数固有形式
        /// </summary>
        public abstract List<string> Tags { get; set; }

        /// <summary>
        /// 第二参数是否不区分空格（即该命令仅有一个参数）
        /// </summary>
        public abstract bool IsNotSplitChar { get; set; }

        public abstract List<(char i, int j)> TextSplitWords { get; set; }

        public abstract List<ChildClasses> ChildClasses { get; set; }
        public abstract Dictionary<string, Dictionary<int, ThumbClass>> NeedTpye { get; set; }
    }
}
