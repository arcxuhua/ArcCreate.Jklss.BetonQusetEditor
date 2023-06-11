using ArcCreate.Jklss.BetonQusetEditor.ViewModel.ShowTool;
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

namespace ArcCreate.Jklss.BetonQusetEditor.View.ShowTool
{
    /// <summary>
    /// ShowToolMainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShowToolMainWindow : Window
    {
        public ShowToolMainWindow()
        {
            InitializeComponent();

            this.DataContext = new ShowToolMainViewModel(this);
        }
    }
}
