using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.ThumbModel.TextAbstractModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Model.ThumbModel.CommandModel
{
    public class EventCmdModel : AbstractEvent
    {
        public override string MainClass { get; set; }
        public override char TextSplitChar { get; set; }
        public override int TextNum { get; set; }
        public override List<string> Tags { get; set; } = new List<string>();
        public override bool IsNotSplitChar { get; set; }
        public override List<(char i, int j)> TextSplitWords { get; set; } = new List<(char i, int j)>();
        public override List<ChildClasses> ChildClasses { get; set; } = new List<ChildClasses>();
        public override Dictionary<string, Dictionary<int, ThumbClass>> NeedTpye { get; set; } = new Dictionary<string, Dictionary<int, ThumbClass>>();
        public override string MainToolTip { get; set; }
        public override List<string> CmdToolTip { get; set; } = new List<string>();
        public override Dictionary<int, Dictionary<int, string>> ParameterToolTip { get; set; } = new Dictionary<int, Dictionary<int, string>>();
        public override Dictionary<int, Dictionary<int, Dictionary<int, string>>> TermToolTip { get; set; } = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>();
    }
}
