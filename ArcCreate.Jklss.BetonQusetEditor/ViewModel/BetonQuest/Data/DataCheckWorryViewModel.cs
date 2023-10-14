using ArcCreate.Jklss.BetonQusetEditor.ViewModel.MainWindows;
using ArcCreate.Jklss.Model.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest.Data
{
    public partial class DataCheckWorryViewModel : ObservableObject, INotifyPropertyChanged
    {
        [ObservableProperty]
        private ObservableCollection<DataCheckInfoModel> _Data = new ObservableCollection<DataCheckInfoModel>();

        [RelayCommand()]
        public void SelectedWorry(DataGrid dataGrid)
        {
            var getItem = dataGrid.SelectedItem;

            if (getItem == null) 
            {
                return;
            }

            var realItem = getItem as DataCheckInfoModel;

            ClientWindowViewModel.FindCardDel(realItem.CardName, realItem.CardClass);
        }

        [RelayCommand()]
        public void Close(Window window)
        {
            window.Close();
        }

        public void ChangeWorryDataDo(List<DataCheckInfoModel> data)
        {
            Data.Clear();

            foreach (var item in data)
            {
                Data.Add(item);
            }
        }
    }

}
