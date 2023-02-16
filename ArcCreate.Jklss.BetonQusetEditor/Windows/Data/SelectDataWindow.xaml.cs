using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArcCreate.Jklss.BetonQusetEditor.Windows.Data
{
    /// <summary>
    /// SelectDataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelectDataWindow : Window
    {
        public SelectDataWindow()
        {
            InitializeComponent();
        }

        private new void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
