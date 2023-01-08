using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.ThumbModel.TextAbstractModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Model.ThumbModel.CommandModel
{
    public class ObjectiveCmdModel : AbstractObjective
    {
        public override string MainClass { get; set; }
        public override char TextSplitChar { get; set; }
        public override int TextNum { get; set; }
        public override List<string> Tags { get; set; }
        public override List<string> ChildTags { get; set; }
        public override List<(char i, int j)> TextSplitWords { get; set; }
        public override List<ChildClasses> ChildClasses { get; set; }
        public override Dictionary<string, Dictionary<int, ThumbClass>> NeedTpye { get; set; }
    }
}
