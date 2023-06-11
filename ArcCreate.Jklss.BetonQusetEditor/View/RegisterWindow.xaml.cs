using ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest.ClientWindow;
using System.Windows;
using System.Windows.Input;

namespace ArcCreate.Jklss.BetonQusetEditor.Windows
{
    /// <summary>
    /// RegisterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            this.DataContext = new RegisterWindowViewModel();
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
