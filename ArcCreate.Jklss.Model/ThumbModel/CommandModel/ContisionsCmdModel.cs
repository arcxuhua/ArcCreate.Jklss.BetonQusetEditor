using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.ThumbModel.TextAbstractModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Model.ThumbModel.CommandModel
{
    public class ContisionsCmdModel : AbstractContision
    {
        public override string MainClass { get; set; }
        public override char TextSplitChar { get; set; }
        public override int TextNum { get; set; }
        public override bool isContisionCmd { get; set; }
        public override List<(char i, int j)> TextSplitWords { get; set; }
        public override List<ChildClasses> ChildClasses { get; set; }
        public override Dictionary<string, Dictionary<int, ThumbClass>> NeedTpye { get; set; }
    }
}
