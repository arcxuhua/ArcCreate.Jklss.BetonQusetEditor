using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest;
using ArcCreate.Jklss.BetonQusetEditor.Windows;
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
using MessageBox = System.Windows.MessageBox;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest.Data
{
    public class SelectDataWindowViewModel : NotifyBase
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
                NotifyChanged();//当view的值发生改变时通知model值发生了改变
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
                NotifyChanged();//当view的值发生改变时通知model值发生了改变
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
                NotifyChanged();//当view的值发生改变时通知model值发生了改变
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
                NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        private RelayCommand<Window> _LoadedCommand;

        public RelayCommand<Window> LoadedCommand
        {
            get
            {
                if (_LoadedCommand == null)
                {
                    _LoadedCommand = new RelayCommand<Window>(async (wd) =>
                    {
                        (wd as SelectDataWindow).data.SelectedCellsChanged += Data_SelectedCellsChangedAsync;
                        window = wd as SelectDataWindow;
                        wd.IsEnabled = false;

                        var message = new MessageModel()
                        {
                            IsLogin = isLogin,
                            JsonInfo = JsonInfo.GetSaveData,
                            UserName = userName,
                            Message = "",
                        };

                        var jsonMessage = FileService.SaveToJson(message);

                        var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                            SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), token, true);

                        if (getMessage == null || !getMessage.Succese)
                        {
                            return;
                        }

                        var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

                        if (getModel.Token != token)
                        {
                            return;
                        }

                        var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

                        if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.GetSaveData || !getRealMessage.IsLogin)
                        {
                            return;
                        }

                        var getData = FileService.JsonToProp<List<GridData>>(getRealMessage.Message);

                        if (getData == null)
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

        private async void Data_SelectedCellsChangedAsync(object sender, SelectedCellsChangedEventArgs e)
        {
            var dg = sender as DataGrid;
            var dataItem = dg.SelectedItem as GridData;
            if (dataItem == null)
            {
                return;
            }

            var res = MessageBox.Show("导入请选择 [是]\n删除请选择 [否]\n取消操作请 [取消]", "操作指导", MessageBoxButton.YesNoCancel);

            if (res == MessageBoxResult.Yes)
            {
                window.Tag = dg.SelectedItem as GridData;

                window.Close();
            }
            else if (res == MessageBoxResult.No)
            {

                var message = new MessageModel()
                {
                    IsLogin = isLogin,
                    JsonInfo = JsonInfo.DeleteSaveData,
                    UserName = userName,
                    Message = dataItem.Code.ToString(),
                };

                var jsonMessage = FileService.SaveToJson(message);

                var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                    SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), token, true);

                if (getMessage == null || !getMessage.Succese)
                {
                    MessageBox.Show("请求错误请尝试重新请求");
                    return;
                }

                var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

                if (getModel.Token != token)
                {
                    return;
                }

                var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

                if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.DeleteSaveData || !getRealMessage.IsLogin)
                {
                    if (getRealMessage != null)
                    {
                        MessageBox.Show(getRealMessage.Message);
                    }

                    return;
                }
                MessageBox.Show(getRealMessage.Message);
                Data.Remove(dataItem);
                dg.ItemsSource = null;
                dg.ItemsSource = Data;
            }
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
                        System.Windows.Controls.Panel.SetZIndex(window.CreateNewPage, 1);
                        System.Windows.Controls.Panel.SetZIndex(window.CreatePage, 2);
                        System.Windows.Controls.Panel.SetZIndex(window.First, 1);
                        AnimationBase.Appear(window.CreatePage, 1);
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
                        System.Windows.Controls.Panel.SetZIndex(window.CreateNewPage, 2);
                        System.Windows.Controls.Panel.SetZIndex(window.CreatePage, 1);
                        System.Windows.Controls.Panel.SetZIndex(window.First, 1);
                        AnimationBase.Appear(window.CreateNewPage, 1);
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
                        System.Windows.Controls.Panel.SetZIndex(window.CreateNewPage, 1);
                        System.Windows.Controls.Panel.SetZIndex(window.CreatePage, 1);
                        System.Windows.Controls.Panel.SetZIndex(window.First, 2);
                        AnimationBase.Appear(window.First, 1);
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
                        if (System.Windows.Controls.Panel.GetZIndex(window.CreatePage) == 2)
                        {
                            if (string.IsNullOrWhiteSpace(FilePath) || string.IsNullOrWhiteSpace(CreateName))
                            {
                                return;
                            }

                            if (MessageBox.Show("注意：请求云端解析YML配置将会花费您一定的积分\n扣费如下：1.对话文件1个1积分\n2.NPC话语1个1积分\n3.玩家话语1个1积分\n" +
                                "4.条件1个1积分\n5.事件1个1积分\n6.目标1个1积分\n7.日记1个1积分\n8.物品1个1积分\n如果您的积分不足且强行解析，照成的损失将由您自己负责", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
                            }
                        }
                        else
                        {
                            window.Tag = new GridData
                            {
                                Code = -1,
                                CreateDate = DateTime.Now,
                                UpdateData = DateTime.Now,
                                Name = CreateName,
                            };
                            window.Close();
                        }


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
                        OpenFileDialog dialog = new OpenFileDialog();
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

        public CommandBase _GrammarCommand;
        public CommandBase GrammarCommand
        {
            get
            {
                if (_GrammarCommand == null)
                {
                    _GrammarCommand = new CommandBase();
                    _GrammarCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        GrammarModelWindow window = new GrammarModelWindow();

                        window.ShowDialog();
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _GrammarCommand;
            }
        }
    }
}
