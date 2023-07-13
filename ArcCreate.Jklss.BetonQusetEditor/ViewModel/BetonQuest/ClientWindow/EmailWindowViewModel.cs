using ArcCreate.Jklss.BetonQusetEditor.View;
using ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest;
using ArcCreate.Jklss.BetonQusetEditor.Windows;
using ArcCreate.Jklss.Model.SocketModel;
using ArcCreate.Jklss.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.ClientWindow
{
    public partial class EmailWindowViewModel:ObservableObject
    {
        private EmailWindow window;
        public EmailWindowViewModel(EmailWindow window)
        {
            this.window = window;
        }

        [ObservableProperty]
        private string _WorryMessage = string.Empty;

        [RelayCommand()]
        private void Close()
        {
            this.window.Close();
        }

        [RelayCommand()]
        private void Narrow()
        {
            this.window.WindowState = WindowState.Minimized;
        }

        [RelayCommand()]
        private async Task Back()
        {
            var message = new MessageModel()
            {
                IsLogin = SocketModel.isLogin,
                JsonInfo = JsonInfo.UsePath,
                UserName = SocketModel.userName,
                Message = this.WorryMessage,
                Path = "Code"
            };

            var getResult = await SocketViewModel.EazySendRESMessage(message);
            
            if (getResult.Succese)
            {
                MessageBox.Show((getResult.Backs as MessageModel).Message);

                LoginWindow window = new LoginWindow();

                window.Show();

                this.window.Close();
                return;
            }
            else
            {
                MessageBox.Show(getResult.Text);
                return;
            }
        }
    }
}
