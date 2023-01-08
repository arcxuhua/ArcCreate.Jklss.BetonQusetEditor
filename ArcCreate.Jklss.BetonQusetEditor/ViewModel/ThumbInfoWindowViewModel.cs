using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.ThumbInfoWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static ArcCreate.Jklss.Model.MainWindow.MainWindowModels;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel
{
    public class ThumbInfoWindowViewModel : NotifyBase
    {
        public static ThumbInfoWindowModel model { get; set; } = new ThumbInfoWindowModel();
        public TreeViewItem ThumbInfo
        {
            get
            {
                return model.ThumbInfo;
            }
            set
            {
                model.ThumbInfo = value;

                this.NotifyChanged();
            }
        }
    }
}
