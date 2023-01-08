using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace ArcCreate.Jklss.Model.ThumbInfoWindow
{
    public class ThumbInfoWindowModel
    {
        public TreeViewItem ThumbInfo { get; set; }

        public Dictionary<string, Dictionary<string, Dictionary<string,bool>>> TreeItems { get; set; }
    }

    public class DefinitionNode
    {
        public string Name { get; set; }

        public string FontColor { get; set; } = "#FF000000";

        public IList<DefinitionNode> Children { get; set; }
    }
}
