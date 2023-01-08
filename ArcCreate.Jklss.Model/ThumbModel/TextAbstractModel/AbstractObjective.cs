using ArcCreate.Jklss.Model.MainWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Model.ThumbModel.TextAbstractModel
{
    public abstract class AbstractObjective : AllSentenceModelInterFace
    {
        public abstract string MainClass { get; set; }

        public abstract char TextSplitChar { get; set; }

        public abstract int TextNum { get; set; }
        /// <summary>
        /// 第二参数固有形式
        /// </summary>
        public abstract List<string> Tags { get; set; }

        /// <summary>
        /// 固有子命令
        /// </summary>
        public abstract List<string> ChildTags { get; set; }

        public abstract List<(char i, int j)> TextSplitWords { get; set; }

        public abstract List<ChildClasses> ChildClasses { get; set; }
        public abstract Dictionary<string, Dictionary<int, ThumbClass>> NeedTpye { get; set; }
    }
}
