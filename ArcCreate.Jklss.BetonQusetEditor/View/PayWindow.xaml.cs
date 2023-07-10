using ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest.ClientWindow;
using System.Windows;
using System.Windows.Input;

namespace ArcCreate.Jklss.BetonQusetEditor.Windows
{
    /// <summary>
    /// PayWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PayWindow : Window
    {
        public PayWindow()
        {
            InitializeComponent();

            this.DataContext = new PayWindowViewModel(this);
        }

        private void Move_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)

            {

                this.DragMove();

            }
        }
    }
}
