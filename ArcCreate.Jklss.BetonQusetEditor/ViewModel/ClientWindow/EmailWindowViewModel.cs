using ArcCreate.Jklss.BetonQusetEditor.View;
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

            var jsonMessage = FileService.SaveToJson(message);

            var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(),waitBack:true);

            if (getMessage == null || !getMessage.Succese)
            {
                MessageBox.Show(getMessage.Text);
                return;
            }

            var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);


            var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

            if (getRealMessage == null || getRealMessage.Path!="Code" || !getRealMessage.IsLogin)
            {
                MessageBox.Show(getRealMessage.Message);
                return;
            }

            MessageBox.Show(getRealMessage.Message);

            LoginWindow window = new LoginWindow();

            window.Show();

            this.window.Close();

        }
    }
}
