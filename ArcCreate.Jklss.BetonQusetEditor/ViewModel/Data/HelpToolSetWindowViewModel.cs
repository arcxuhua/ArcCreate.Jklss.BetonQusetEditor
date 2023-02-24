using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.Data
{
    public partial class HelpToolSetWindowViewModel : ObservableObject
    {
        public HelpToolSetWindowViewModel(string tool)
        {
            HelpMessage = tool;
        }

        [ObservableProperty]
        private string _HelpMessage = "";


        [RelayCommand()]
        private void Close(object obj)
        {
            var window = obj as Window;

            window.Tag = HelpMessage;

            window.Close();
        }
    }
}
