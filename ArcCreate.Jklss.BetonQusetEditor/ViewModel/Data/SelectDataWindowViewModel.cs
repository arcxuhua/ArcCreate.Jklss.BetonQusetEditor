using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.BetonQusetEditor.Windows.Data;
using ArcCreate.Jklss.Model.Data;
using ArcCreate.Jklss.Model.SocketModel;
using ArcCreate.Jklss.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using static ArcCreate.Jklss.Model.SocketModel.SocketModel;
using DataGrid = System.Windows.Controls.DataGrid;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.Data
{
    public class SelectDataWindowViewModel:NotifyBase
    {
        private SelectDataWindowModel model = new SelectDataWindowModel();

        private SelectDataWindow window = null;

        public string FilePath
        {
            get
            {
                return model.FilePath;
            }
            set
            {
                model.FilePath = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public string CreateName
        {
            get
            {
                return model.CreateName;
            }
            set
            {
                model.CreateName = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public string SearchText
        {
            get
            {
                return model.SearchText;
            }
            set
            {
                model.SearchText = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public List<GridData> Data
        {
            get
            {
                return model.Data;
            }
            set
            {
                model.Data = value;
                this.NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        private RelayCommand<Window> _LoadedCommand;

        public RelayCommand<Window> LoadedCommand
        {
            get
            {
                if (_LoadedCommand == null)
                {
                    _LoadedCommand = new RelayCommand<Window>(async (wd) => {
                        (wd as SelectDataWindow).data.SelectedCellsChanged += Data_SelectedCellsChanged;
                        window = wd as SelectDataWindow;
                        wd.IsEnabled = false;

                        var message = new MessageModel()
                        {
                            IsLogin = SocketModel.isLogin,
                            JsonInfo = JsonInfo.GetSaveData,
                            UserName = SocketModel.userName,
                            Message = "",
                        };

                        var jsonMessage = FileService.SaveToJson(message);

                        var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                            SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);

                        if (getMessage == null || !getMessage.Succese)
                        {
                            return;
                        }

                        var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

                        if (getModel.Token != SocketModel.token)
                        {
                            return;
                        }

                        var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

                        if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.GetSaveData || !getRealMessage.IsLogin)
                        {
                            return;
                        }

                        var getData = FileService.JsonToProp<List<GridData>>(getRealMessage.Message);

                        if(getData == null)
                        {
                            return;
                        }

                        Data = getData;

                        wd.IsEnabled = true;
                    });
                }
                return _LoadedCommand;
            }
            set { _LoadedCommand = value; }
        }

        private void Data_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var dg = sender as DataGrid;

            window.Tag = dg.SelectedItem as GridData;

            window.Close();
        }

        public CommandBase _SearchCommand;
        public CommandBase SearchCommand
        {
            get
            {
                if (_SearchCommand == null)
                {
                    _SearchCommand = new CommandBase();
                    _SearchCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _SearchCommand;
            }
        }

        public CommandBase _CreateCommand;
        public CommandBase CreateCommand
        {
            get
            {
                if (_CreateCommand == null)
                {
                    _CreateCommand = new CommandBase();
                    _CreateCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        if (window.CreatePage.Visibility == Visibility.Hidden)
                        {
                            AnimationBase.Appear(window.CreatePage, 1);
                        }
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreateCommand;
            }
        }

        public CommandBase _ReadCommand;
        public CommandBase ReadCommand
        {
            get
            {
                if (_ReadCommand == null)
                {
                    _ReadCommand = new CommandBase();
                    _ReadCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {

                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _ReadCommand;
            }
        }

        public CommandBase _MainCommand;
        public CommandBase MainCommand
        {
            get
            {
                if (_MainCommand == null)
                {
                    _MainCommand = new CommandBase();
                    _MainCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        if(window.CreatePage.Visibility == Visibility.Visible)
                        {
                            AnimationBase.Disappear(window.CreatePage,1);
                        }
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _MainCommand;
            }
        }

        public CommandBase _CreateNewCommand;
        public CommandBase CreateNewCommand
        {
            get
            {
                if (_CreateNewCommand == null)
                {
                    _CreateNewCommand = new CommandBase();
                    _CreateNewCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        window.Tag = new GridData
                        {
                            Code = -1,
                            CreateDate = DateTime.Now,
                            UpdateData = DateTime.Now,
                            Name = CreateName,
                            FilePath = FilePath
                        };
                        window.Close();
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CreateNewCommand;
            }
        }

        public CommandBase _SelectFilePathCommand;
        public CommandBase SelectFilePathCommand
        {
            get
            {
                if (_SelectFilePathCommand == null)
                {
                    _SelectFilePathCommand = new CommandBase();
                    _SelectFilePathCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
                        dialog.Multiselect = false;//该值确定是否可以选择多个文件
                        dialog.Title = "请选择Main.yml";
                        dialog.Filter = "入口文件|main.yml";
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            FilePath = dialog.FileName;

                        }
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _SelectFilePathCommand;
            }
        }
    }
}
