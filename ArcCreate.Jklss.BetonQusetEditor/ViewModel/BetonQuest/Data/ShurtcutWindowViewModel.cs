using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest.Data
{
    public partial class ShurtcutWindowViewModel : ObservableObject
    {
        public ShurtcutWindowViewModel(Window window)
        {
            this.window = window;

            DeleteEditerDel = new _DeleteEditerDel(DeleteEditer);
        }

        public Window window = null;

        [ObservableProperty]
        private ObservableCollection<EditerViewModel> _Items = new ObservableCollection<EditerViewModel>();

        [ObservableProperty]
        private string _RichText = string.Empty;

        [ObservableProperty]
        private bool _IsUseOnlyOne = true;

        [ObservableProperty]
        private string _NeedPay = "生成所需积分: 0";

        [ObservableProperty]
        private bool _CanCreate = false;

        public delegate void _DeleteEditerDel(EditerViewModel editerViewModel);

        public static _DeleteEditerDel DeleteEditerDel;
        /// <summary>
        /// 生成
        /// </summary>
        [RelayCommand()]
        private async Task Create()
        {
            if(MessageBox.Show($"{NeedPay},如果您勾选了启用单次对话需要再加 2 积分\n确定请点击 是","注意",MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            window.Tag = Items;

            window.Close();
        }

        /// <summary>
        /// 添加文案
        /// </summary>
        /// <param name="richTextBox"></param>
        [RelayCommand()]
        private void AddText()
        {
            var editerModel = new EditerViewModel() 
            {
                RichText = RichText
            };

            if(Items.Count % 2 == 0)
            {
                editerModel.Type = EditerEnum.NPC;
            }
            else
            {
                editerModel.Type = EditerEnum.Player;
            }

            Items.Add(editerModel);

            if (Items.Count > 2)
            {
                if (Items.Count % 2 == 0)
                {
                    CanCreate = true;
                }
                else
                {
                    CanCreate = false;
                }
            }

            NeedPay = $"生成所需积分: {Items.Count}";
        }

        [RelayCommand()]
        private void Close()
        {
            window.Close();
        }

        private void DeleteEditer(EditerViewModel editerViewModel)
        {
            var getIndex = Items.IndexOf(editerViewModel);

            if(editerViewModel.Type == EditerEnum.NPC)
            {
                if(Items.Count> getIndex + 1)
                {
                    Items.RemoveAt(getIndex + 1);
                    
                }

                Items.RemoveAt(getIndex);
            }
            else
            {
                Items.RemoveAt(getIndex);

                try
                {
                    Items.RemoveAt(getIndex - 1);
                }
                catch
                {

                }
                
            }
        }
    }

    public partial class EditerViewModel : ObservableObject
    {
        [ObservableProperty]
        private EditerEnum _Type = EditerEnum.NPC;

        [ObservableProperty]
        private string _RichText = string.Empty;

        [RelayCommand()]
        private void Editer()
        {
            ShurtcutWindowViewModel.DeleteEditerDel(this);
        }
    }

    public enum EditerEnum
    {
        NPC,
        Player
    }
}
