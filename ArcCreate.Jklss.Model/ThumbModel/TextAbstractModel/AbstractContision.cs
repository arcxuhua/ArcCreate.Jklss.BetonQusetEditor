using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.ThumbModel;

namespace ArcCreate.Jklss.Model.ThumbModel.TextAbstractModel
{
    /// <summary>
    /// Contisions语法构造器模型
    /// </summary>
    public abstract class AbstractContision : AllSentenceModelInterFace
    {
        public abstract string MainClass { get; set; }

        public abstract char TextSplitChar { get; set; }

        public abstract int TextNum { get; set; }
        /// <summary>
        /// 是否为条件性命令，开启将识别参数中的(!)符号
        /// </summary>
        public abstract bool isContisionCmd { get; set; }

        public abstract List<(char i, int j)> TextSplitWords { get; set; }

        public abstract List<ChildClasses> ChildClasses { get; set; }
        public abstract Dictionary<string, Dictionary<int, ThumbClass>> NeedTpye { get; set; }
        public abstract string MainToolTip { get; set; }
        public abstract List<string> CmdToolTip { get; set; }
        public abstract Dictionary<int, Dictionary<int, string>> ParameterToolTip { get; set; }
        public abstract Dictionary<int, Dictionary<int, Dictionary<int, string>>> TermToolTip { get; set; }
    }
}
