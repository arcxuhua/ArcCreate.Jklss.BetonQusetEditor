using ArcCreate.Jklss.BetonQusetEditor.Base.FileLoader;
using ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest;
using ArcCreate.Jklss.BetonQusetEditor.Windows;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.SocketModel;
using ArcCreate.Jklss.Model.ThumbModel;
using ArcCreate.Jklss.Model.ThumbModel.CommandModel;
using ArcCreate.Jklss.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel
{
    public partial class GrammerModelViewModel: ObservableObject
    {
        public GrammerModelViewModel(GrammarModelWindow window)
        {
            this.window = window;

            var one = new GrammerListViewModel("0");

            _addList = new AddList(addToList);

            _removeList = new RemoveList(removeToList);

            _getModelId = new GetModelId(GetModelsID);

            _changeEditer = new ChangeEditer(ChangeEditers);

            this.Parameter.Add(one);

            GrammerSelect = one;

            window.SelectModel_Cbox.SelectionChanged += SelectModel_Cbox_SelectionChanged;

            window.SelectType_Cbox.SelectionChanged += SelectType_Cbox_SelectionChanged;

        }

        #region 属性与字段

        #region 委托
        private delegate void AddList(GrammerListViewModel listViewModel);
        private static AddList _addList;

        private delegate void RemoveList(GrammerListViewModel listViewModel);
        private static RemoveList _removeList;


        private delegate int GetModelId();
        private static GetModelId _getModelId;

        private delegate void ChangeEditer(EditerModelAbstract editerModel, GrammerListViewModel listViewModel);
        private static ChangeEditer _changeEditer;

        #endregion

        private GrammarModelWindow window;

        [ObservableProperty]
        private ObservableCollection<GrammerListViewModel> _Parameter = new ObservableCollection<GrammerListViewModel>();

        [ObservableProperty]
        private int _SelectType = -1;

        [ObservableProperty]
        private string _ModelSelect = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _ModelItems = new ObservableCollection<string>();

        [ObservableProperty]
        private GrammerListViewModel _GrammerSelect = null;

        [ObservableProperty]
        private int _GrammerSelectIndex = -1;

        [ObservableProperty]
        private EditerModelAbstract _Editer = new ContidonsMainClassViewModel() { Type = "" };

        private static Dictionary<GrammerListViewModel, EditerModelAbstract> saveAllEditer = new Dictionary<GrammerListViewModel, EditerModelAbstract>();

        private static int nowID = 0; 
        #endregion

        #region 命令
        [RelayCommand()]
        private void Close(object obj)
        {
            var window = obj as Window;

            window.Close();
        }

        [RelayCommand()]
        private async Task SelectTypeSelectionChanged(ComboBox cbox)
        {
            await Task.Run(() =>
            {
                saveAllEditer.Clear();

                Editer = new ContidonsMainClassViewModel() { Type = "" };
            });
            
        }

        [RelayCommand()]
        private async Task SelectModelSelectionChanged(ComboBox cbox)
        {
            if (string.IsNullOrWhiteSpace(ModelSelect))
            {
                return;
            }

            if(SelectType == 0)
            {
                var getModel = ContisionLoaderBase.savecmdModels.Where(t => t.MainClass == ModelSelect).FirstOrDefault();

                await InputGrammarModel(getModel, ThumbClass.Conditions);
            }

            if (SelectType == 1)
            {
                var getModel = EventLoaderBase.savecmdModels.Where(t => t.MainClass == ModelSelect).FirstOrDefault();

                await InputGrammarModel(getModel, ThumbClass.Events);
            }

            if (SelectType == 2)
            {
                var getModel = ObjectiveLoaderBase.savecmdModels.Where(t => t.MainClass == ModelSelect).FirstOrDefault();

                await InputGrammarModel(getModel, ThumbClass.Objectives);
            }

        }

        [RelayCommand()]
        private async Task AddGrammarModelToList(ComboBox cbox)
        {
            if (MessageBox.Show("你确定将你所编辑的模型添加进你的语法模型器中?", "警告", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            var getBack = await CreateGrammerModel();

            MessageBox.Show(getBack.Text);

        }

        [RelayCommand()]
        private async Task DeleteGrammarModelToList(ComboBox cbox)
        {
            if (string.IsNullOrWhiteSpace(ModelSelect))
            {
                return;
            }

            if (MessageBox.Show("你确定将你选择的语法模型从语法模型器中移除?", "警告", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            if (ModelItems.Contains(ModelSelect))
            {
                

                if(SelectType == 0)
                {
                    var findModel = ContisionLoaderBase.savecmdModels.Where(t => t.MainClass == ModelSelect).FirstOrDefault();

                    if (findModel == null)
                    {
                        MessageBox.Show("您的语法模型中并不拥有该语法！");
                        return;
                    }

                    var message = new MessageModel()
                    {
                        IsLogin = SocketModel.isLogin,
                        JsonInfo = JsonInfo.RemoveGrammerModel,
                        UserName = SocketModel.userName,
                        Message = ModelSelect,
                        Other = "1"
                    };
                    var jsonMessage = FileService.SaveToJson(message);

                    var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                        SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);

                    if (getMessage == null || !getMessage.Succese)
                    {
                        MessageBox.Show($"请求云端失败 {getMessage.Text}");
                        return;
                    }

                    var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

                    if (getModel.Token != SocketModel.token)
                    {
                        MessageBox.Show($"请求云端失败 {getMessage.Text}");
                        return;
                    }

                    var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

                    if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.RemoveGrammerModel || !getRealMessage.IsLogin)
                    {
                        MessageBox.Show($"请求云端失败 {getRealMessage.Message}");
                        return;
                    }

                    try
                    {
                        MessageBox.Show($"请求云端成功：{getRealMessage.Message}");
                    }
                    catch
                    {
                        MessageBox.Show($"请求云端失败 获取返回值错误");
                        return;
                    }

                    ContisionLoaderBase.savecmdModels.Remove(findModel);

                    ModelItems.Remove(ModelSelect);
                }

                if (SelectType == 1)
                {
                    var findModel = EventLoaderBase.savecmdModels.Where(t => t.MainClass == ModelSelect).FirstOrDefault();

                    if (findModel == null)
                    {
                        MessageBox.Show("您的语法模型中并不拥有该语法！");
                        return;
                    }

                    var message = new MessageModel()
                    {
                        IsLogin = SocketModel.isLogin,
                        JsonInfo = JsonInfo.RemoveGrammerModel,
                        UserName = SocketModel.userName,
                        Message = ModelSelect,
                        Other = "2"
                    };
                    var jsonMessage = FileService.SaveToJson(message);

                    var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                        SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);

                    if (getMessage == null || !getMessage.Succese)
                    {
                        MessageBox.Show($"请求云端失败 {getMessage.Text}");
                        return;
                    }

                    var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

                    if (getModel.Token != SocketModel.token)
                    {
                        MessageBox.Show($"请求云端失败 {getMessage.Text}");
                        return;
                    }

                    var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

                    if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.RemoveGrammerModel || !getRealMessage.IsLogin)
                    {
                        MessageBox.Show($"请求云端失败 {getRealMessage.Message}");
                        return;
                    }

                    try
                    {
                        MessageBox.Show($"请求云端成功：{getRealMessage.Message}");
                    }
                    catch
                    {
                        MessageBox.Show($"请求云端失败 获取返回值错误");
                        return;
                    }

                    EventLoaderBase.savecmdModels.Remove(findModel);

                    ModelItems.Remove(ModelSelect);
                }

                if (SelectType == 2)
                {
                    var findModel = ObjectiveLoaderBase.savecmdModels.Where(t => t.MainClass == ModelSelect).FirstOrDefault();

                    if (findModel == null)
                    {
                        MessageBox.Show("您的语法模型中并不拥有该语法！");
                        return;
                    }

                    var message = new MessageModel()
                    {
                        IsLogin = SocketModel.isLogin,
                        JsonInfo = JsonInfo.RemoveGrammerModel,
                        UserName = SocketModel.userName,
                        Message = ModelSelect,
                        Other = "3"
                    };
                    var jsonMessage = FileService.SaveToJson(message);

                    var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                        SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);

                    if (getMessage == null || !getMessage.Succese)
                    {
                        MessageBox.Show($"请求云端失败 {getMessage.Text}");
                        return;
                    }

                    var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

                    if (getModel.Token != SocketModel.token)
                    {
                        MessageBox.Show($"请求云端失败 {getMessage.Text}");
                        return;
                    }

                    var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

                    if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.RemoveGrammerModel || !getRealMessage.IsLogin)
                    {
                        MessageBox.Show($"请求云端失败 {getRealMessage.Message}");
                        return;
                    }

                    try
                    {
                        MessageBox.Show($"请求云端成功：{getRealMessage.Message}");
                    }
                    catch
                    {
                        MessageBox.Show($"请求云端失败 获取返回值错误");
                        return;
                    }

                    ObjectiveLoaderBase.savecmdModels.Remove(findModel);

                    ModelItems.Remove(ModelSelect);
                }
            }
        }

        [RelayCommand()]
        private async Task SaveGrammarModel(ComboBox cbox)
        {
            if(!this.Parameter.Where(t => t.Type == "0").Any())
            {
                MessageBox.Show("未找到存储的主命令模型");
                return;
            }
            var mainModel = this.Parameter.Where(t => t.Type == "0").FirstOrDefault();

            if (!saveAllEditer.ContainsKey(mainModel))
            {
                MessageBox.Show("未找到存储的主命令模型的编辑模型");
                return;
            }

            if (SelectType == 0)
            {
                var getMainModelEditer = saveAllEditer[mainModel] as ContidonsMainClassViewModel;

                if (ModelItems.Contains(getMainModelEditer.CmdName))
                {
                    if(MessageBox.Show("语法模型中含有该命令，您确定替换该命令的语法模型？","警告",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                       var model =  ContisionLoaderBase.savecmdModels.Where(t => t.MainClass == getMainModelEditer.CmdName).FirstOrDefault();

                        ContisionLoaderBase.savecmdModels.Remove(model);

                        var getBack = await CreateGrammerModel();

                        MessageBox.Show(getBack.Text);

                        return;
                    }
                }
            }

            if (SelectType == 1)
            {
                var getMainModelEditer = saveAllEditer[mainModel] as EventsMainClassViewModel;

                if (ModelItems.Contains(getMainModelEditer.CmdName))
                {
                    if (MessageBox.Show("语法模型中含有该命令，您确定替换该命令的语法模型？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        var model = EventLoaderBase.savecmdModels.Where(t => t.MainClass == getMainModelEditer.CmdName).FirstOrDefault();

                        EventLoaderBase.savecmdModels.Remove(model);

                        var getBack = await CreateGrammerModel();

                        MessageBox.Show(getBack.Text);

                        return;
                    }
                }
            }

            if (SelectType == 2)
            {
                var getMainModelEditer = saveAllEditer[mainModel] as ObjectivesMainClassViewModel;

                if (ModelItems.Contains(getMainModelEditer.CmdName))
                {
                    if (MessageBox.Show("语法模型中含有该命令，您确定替换该命令的语法模型？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        var model = ObjectiveLoaderBase.savecmdModels.Where(t => t.MainClass == getMainModelEditer.CmdName).FirstOrDefault();

                        ObjectiveLoaderBase.savecmdModels.Remove(model);

                        var getBack = await CreateGrammerModel();

                        MessageBox.Show(getBack.Text);

                        return;
                    }
                }
            }

            MessageBox.Show("请使用添加按钮来添加新的语法模型");
        }

        #endregion

        #region 事件

        private void SelectType_Cbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbox = sender as ComboBox;

            SelectType = cbox.SelectedIndex;

            ModelItems.Clear();

            if (SelectType == 0)
            {
                foreach (var item in ContisionLoaderBase.savecmdModels)
                {
                    ModelItems.Add(item.MainClass);
                }
            }
            else if(SelectType == 1)
            {
                foreach (var item in EventLoaderBase.savecmdModels)
                {
                    ModelItems.Add(item.MainClass);
                }
            }
            else if(SelectType == 2)
            {
                foreach (var item in ObjectiveLoaderBase.savecmdModels)
                {
                    ModelItems.Add(item.MainClass);
                }
            }
        }

        private void SelectModel_Cbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbox = sender as ComboBox;
            
        }

        #endregion

        #region 具体方法

        private async Task<ReturnModel> InputGrammarModel<T>(T model,ThumbClass thumbClass)
        {
            var result = new ReturnModel();

            this.Parameter.Clear();
            saveAllEditer.Clear();

            var one = new GrammerListViewModel("0");

            this.Parameter.Add(one);

            if (thumbClass == ThumbClass.Conditions)
            {
                var back = await InputConditionsModel(model as ContisionsCmdModel, one);

                return back;
            }

            if(thumbClass == ThumbClass.Events)
            {
                var back = await InputEventsModel(model as EventCmdModel,one);
                return back;
            }

            if(thumbClass == ThumbClass.Objectives)
            {
                var back = await InputObjectivesModel(model as ObjectiveCmdModel,one);
                return back;
            }

            result.SetError();

            return result;
        }

        private async Task<ReturnModel> InputConditionsModel(ContisionsCmdModel contisionsCmdModel, GrammerListViewModel main)
        {
            var result = new ReturnModel();

            #region 视图的创建

            var getMainModelNeedParNums = contisionsCmdModel.TextSplitWords.Count;

            for (int i = 0; i < getMainModelNeedParNums; i++)
            {
                main.Parameter.Add(new GrammerListViewModel("2"));
            }

            for (int i = 0; i < contisionsCmdModel.ChildClasses.Count; i++)
            {
                var childCmdViewModel = new GrammerListViewModel("3");

                this.Parameter.Add(childCmdViewModel);

                for (int j = 0; j < contisionsCmdModel.ChildClasses[i].ChildTextSplitWords.Count; j++)
                {
                    childCmdViewModel.Parameter.Add(new GrammerListViewModel("2"));
                }
            }

            #endregion

            #region 数据的处理与绑定

            #region 主命令数据处理

            var newGrammerMainClassModel = new ContidonsMainClassViewModel();

            newGrammerMainClassModel.CmdName = contisionsCmdModel.MainClass;

            newGrammerMainClassModel.HelpInfoMessages = new ObservableCollection<ContidonsMainClassViewModel.SaveHelpInfoModel>();

            var maintool_fg = contisionsCmdModel.MainToolTip.Split("\n", StringSplitOptions.RemoveEmptyEntries);

            await Task.Run(() =>
            {
                for (int i = 0; i < maintool_fg.Length; i++)
                {
                    newGrammerMainClassModel.HelpInfoMessages.Add(new ContidonsMainClassViewModel.SaveHelpInfoModel { HelpInfo = maintool_fg[i] });
                }
            });

            newGrammerMainClassModel.IsUseCondition = contisionsCmdModel.isContisionCmd;

            saveAllEditer.Add(main, newGrammerMainClassModel);

            #endregion

            #region 主命令下第一参数的数据处理

            var newGrammerMainFirstPar = new ContidonsParameterFirstViewModel();

            await Task.Run(() =>
            {
                if (contisionsCmdModel.ParameterToolTip.ContainsKey(0))
                {
                    if (contisionsCmdModel.ParameterToolTip.ContainsKey(0))
                    {
                        var mainpartool_fg = contisionsCmdModel.ParameterToolTip[0][0].Split("\n", StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < mainpartool_fg.Length; i++)
                        {
                            newGrammerMainFirstPar.ParameterHelpInfoMessages.Add(new ContidonsParameterFirstViewModel.SaveParameterHelpInfo { ParameterHelpInfo = mainpartool_fg[i] });
                        }
                    }
                }
            });

            await Task.Run(() =>
            {
                if (contisionsCmdModel.TermToolTip.ContainsKey(0))
                {
                    if (contisionsCmdModel.TermToolTip[0].ContainsKey(0))
                    {
                        var dic = contisionsCmdModel.TermToolTip[0][0];

                        foreach (var item in dic)
                        {
                            var csfg = item.Value.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                            for (int i = 0; i < csfg.Length; i++)
                            {
                                newGrammerMainFirstPar.NapeHelpInfoMessages.Add(new ContidonsParameterFirstViewModel.SaveNapeHelpInfo
                                {
                                    NapeHelpInfoMessage = csfg[i],

                                    NapeHelpInfoMessageNum = $"第 {item.Key + 1} 项"
                                });
                            }


                        }
                    }
                }
            });

            newGrammerMainFirstPar.ParameterSpiltChar = contisionsCmdModel.TextSplitChar.ToString();

            newGrammerMainFirstPar.ParameterSpiltStep = contisionsCmdModel.TextNum.ToString();

            if (contisionsCmdModel.NeedTpye !=null && contisionsCmdModel.NeedTpye.ContainsKey(contisionsCmdModel.MainClass))
            {
                if (contisionsCmdModel.NeedTpye[contisionsCmdModel.MainClass].ContainsKey(0))
                {
                    switch (contisionsCmdModel.NeedTpye[contisionsCmdModel.MainClass][0])
                    {
                        case ThumbClass.Subject:
                            newGrammerMainFirstPar.NeedCardSelectIndex = 0;
                            break;
                        case ThumbClass.Player:
                            newGrammerMainFirstPar.NeedCardSelectIndex = 1;
                            break;
                        case ThumbClass.NPC:
                            newGrammerMainFirstPar.NeedCardSelectIndex = 2;
                            break;
                        case ThumbClass.Conditions:
                            newGrammerMainFirstPar.NeedCardSelectIndex = 3;
                            break;
                        case ThumbClass.Events:
                            newGrammerMainFirstPar.NeedCardSelectIndex = 4;
                            break;
                        case ThumbClass.Objectives:
                            newGrammerMainFirstPar.NeedCardSelectIndex = 5;
                            break;
                        case ThumbClass.Journal:
                            newGrammerMainFirstPar.NeedCardSelectIndex = 6;
                            break;
                        case ThumbClass.Items:
                            newGrammerMainFirstPar.NeedCardSelectIndex = 7;
                            break;
                    }
                }
            }

            saveAllEditer.Add(main.Parameter.Where(t => t.Type == "1").FirstOrDefault(), newGrammerMainFirstPar);

            #endregion

            #region 主命令下其他参数的数据处理

            var getMainParNums = contisionsCmdModel.TextSplitWords.Count;

            var getModel = main.Parameter.Where(t => t.Type == "2").ToList();

            for (int i = 0; i < getMainParNums; i++)
            {
                var newGrammerMainPar = new ContidonsParameterFirstViewModel();

                if (contisionsCmdModel.ParameterToolTip.ContainsKey(0))
                {
                    var key = i + 1;
                    if (contisionsCmdModel.ParameterToolTip[0].ContainsKey(key))
                    {
                        var mainpartool_fg = contisionsCmdModel.ParameterToolTip[0][key].Split("\n", StringSplitOptions.RemoveEmptyEntries);

                        for (int j = 0; j < mainpartool_fg.Length; j++)
                        {
                            newGrammerMainPar.ParameterHelpInfoMessages.Add(new ContidonsParameterFirstViewModel.SaveParameterHelpInfo { ParameterHelpInfo = mainpartool_fg[j] });
                        }
                    }
                }

                if (contisionsCmdModel.TermToolTip.ContainsKey(0))
                {
                    if (contisionsCmdModel.TermToolTip[0].ContainsKey(getMainParNums + 1))
                    {
                        var dic = contisionsCmdModel.TermToolTip[0][getMainParNums + 1];

                        foreach (var item in dic)
                        {
                            var csfg = item.Value.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                            for (int j = 0; j < csfg.Length; j++)
                            {
                                newGrammerMainPar.NapeHelpInfoMessages.Add(new ContidonsParameterFirstViewModel.SaveNapeHelpInfo
                                {
                                    NapeHelpInfoMessage = csfg[j],

                                    NapeHelpInfoMessageNum = $"第 {item.Key + 1} 项"
                                });
                            }
                        }
                    }
                }

                newGrammerMainPar.ParameterSpiltChar = contisionsCmdModel.TextSplitWords[i].i.ToString();

                newGrammerMainPar.ParameterSpiltStep = contisionsCmdModel.TextSplitWords[i].j.ToString();

                if (contisionsCmdModel.NeedTpye != null && contisionsCmdModel.NeedTpye.ContainsKey(contisionsCmdModel.MainClass))
                {
                    if (contisionsCmdModel.NeedTpye[contisionsCmdModel.MainClass].ContainsKey(getMainParNums + 1))
                    {
                        switch (contisionsCmdModel.NeedTpye[contisionsCmdModel.MainClass][getMainParNums + 1])
                        {
                            case ThumbClass.Subject:
                                newGrammerMainPar.NeedCardSelectIndex = 0;
                                break;
                            case ThumbClass.Player:
                                newGrammerMainPar.NeedCardSelectIndex = 1;
                                break;
                            case ThumbClass.NPC:
                                newGrammerMainPar.NeedCardSelectIndex = 2;
                                break;
                            case ThumbClass.Conditions:
                                newGrammerMainPar.NeedCardSelectIndex = 3;
                                break;
                            case ThumbClass.Events:
                                newGrammerMainPar.NeedCardSelectIndex = 4;
                                break;
                            case ThumbClass.Objectives:
                                newGrammerMainPar.NeedCardSelectIndex = 5;
                                break;
                            case ThumbClass.Journal:
                                newGrammerMainPar.NeedCardSelectIndex = 6;
                                break;
                            case ThumbClass.Items:
                                newGrammerMainPar.NeedCardSelectIndex = 7;
                                break;
                        }
                    }
                }

                if (i < getModel.Count)
                {
                    saveAllEditer.Add(getModel[i], newGrammerMainPar);
                }
                else
                {
                    result.SetError("加载错误！");
                    return result;
                }
            }

            #endregion

            #region 子命令的数据处理

            var getModels = this.Parameter.Where(t => t.Type == "3").ToList();

            for (int i = 0; i < contisionsCmdModel.ChildClasses.Count; i++)
            {
                var newGrammerChirdCmdModel = new ContidonsChirldClassViewModel();

                newGrammerChirdCmdModel.ChirdCmdName = contisionsCmdModel.ChildClasses[i].ChildClass;

                await Task.Run(() =>
                {
                    if (contisionsCmdModel.CmdToolTip.Count > i)
                    {
                        var chirdtool_fg = contisionsCmdModel.CmdToolTip[i].Split("\n", StringSplitOptions.RemoveEmptyEntries);

                        for (int j = 0; j < chirdtool_fg.Length; j++)
                        {
                            newGrammerChirdCmdModel.ChirdCmdHelpInfoMessages.Add(new ContidonsChirldClassViewModel.SaveHelpInfoModel { ChirdCmdHelpInfo = chirdtool_fg[j] });
                        }
                    }
                });

                if(i< getModels.Count)
                {
                    saveAllEditer.Add(getModels[i], newGrammerChirdCmdModel);
                }
                else
                {
                    result.SetError("加载错误！");

                    return result;
                }

                #region 子命令第一参数的处理

                var getCcmdFirstModel = getModels[i].Parameter.Where(t => t.Type == "1").FirstOrDefault();

                var newGrammerChirdFirstPar = new ContidonsParameterFirstViewModel();

                await Task.Run(() =>
                {
                    if (contisionsCmdModel.ParameterToolTip.ContainsKey(i+1))
                    {
                        if (contisionsCmdModel.ParameterToolTip.ContainsKey(0))
                        {
                            var mainpartool_fg = contisionsCmdModel.ParameterToolTip[i+1][0].Split("\n", StringSplitOptions.RemoveEmptyEntries);

                            for (int j = 0; j < mainpartool_fg.Length; j++)
                            {
                                newGrammerChirdFirstPar.ParameterHelpInfoMessages.Add(new ContidonsParameterFirstViewModel.SaveParameterHelpInfo { ParameterHelpInfo = mainpartool_fg[j] });
                            }
                        }
                    }
                });

                await Task.Run(() =>
                {
                    if (contisionsCmdModel.TermToolTip.ContainsKey(i+1))
                    {
                        if (contisionsCmdModel.TermToolTip[i + 1].ContainsKey(0))
                        {
                            var dic = contisionsCmdModel.TermToolTip[i + 1][0];

                            foreach (var item in dic)
                            {
                                var csfg = item.Value.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                                for (int j = 0; j < csfg.Length; j++)
                                {
                                    newGrammerChirdFirstPar.NapeHelpInfoMessages.Add(new ContidonsParameterFirstViewModel.SaveNapeHelpInfo
                                    {
                                        NapeHelpInfoMessage = csfg[j],

                                        NapeHelpInfoMessageNum = $"第 {item.Key + 1} 项"
                                    });
                                }


                            }
                        }
                    }
                });

                newGrammerChirdFirstPar.ParameterSpiltChar = contisionsCmdModel.ChildClasses[i].ChildTextSplitChar.ToString();

                newGrammerChirdFirstPar.ParameterSpiltStep = contisionsCmdModel.ChildClasses[i].ChildTextNum.ToString();

                if (contisionsCmdModel.NeedTpye !=null && contisionsCmdModel.NeedTpye.ContainsKey(contisionsCmdModel.ChildClasses[i].ChildClass))
                {
                    if (contisionsCmdModel.NeedTpye[contisionsCmdModel.ChildClasses[i].ChildClass].ContainsKey(0))
                    {
                        switch (contisionsCmdModel.NeedTpye[contisionsCmdModel.ChildClasses[i].ChildClass][0])
                        {
                            case ThumbClass.Subject:
                                newGrammerChirdFirstPar.NeedCardSelectIndex = 0;
                                break;
                            case ThumbClass.Player:
                                newGrammerChirdFirstPar.NeedCardSelectIndex = 1;
                                break;
                            case ThumbClass.NPC:
                                newGrammerChirdFirstPar.NeedCardSelectIndex = 2;
                                break;
                            case ThumbClass.Conditions:
                                newGrammerChirdFirstPar.NeedCardSelectIndex = 3;
                                break;
                            case ThumbClass.Events:
                                newGrammerChirdFirstPar.NeedCardSelectIndex = 4;
                                break;
                            case ThumbClass.Objectives:
                                newGrammerChirdFirstPar.NeedCardSelectIndex = 5;
                                break;
                            case ThumbClass.Journal:
                                newGrammerChirdFirstPar.NeedCardSelectIndex = 6;
                                break;
                            case ThumbClass.Items:
                                newGrammerChirdFirstPar.NeedCardSelectIndex = 7;
                                break;
                        }
                    }
                }

                saveAllEditer.Add(getCcmdFirstModel, newGrammerChirdFirstPar);

                #endregion

                #region 子命令其他参数的数据处理

                //并不允许的情况，这里不做内容处理

                #endregion



            }



            #endregion

            #endregion

            result.SetSuccese("载入成功！");

            return result;
        }

        private async Task<ReturnModel> InputEventsModel(EventCmdModel contisionsCmdModel, GrammerListViewModel main)
        {
            var result = new ReturnModel();

            #region 视图的创建

            var getMainModelNeedParNums = contisionsCmdModel.TextSplitWords.Count;

            for (int i = 0; i < getMainModelNeedParNums; i++)
            {
                main.Parameter.Add(new GrammerListViewModel("2"));
            }

            for (int i = 0; i < contisionsCmdModel.ChildClasses.Count; i++)
            {
                var childCmdViewModel = new GrammerListViewModel("3");

                this.Parameter.Add(childCmdViewModel);

                for (int j = 0; j < contisionsCmdModel.ChildClasses[i].ChildTextSplitWords.Count; j++)
                {
                    childCmdViewModel.Parameter.Add(new GrammerListViewModel("2"));
                }
            }

            #endregion

            #region 数据的处理与绑定

            #region 主命令数据处理

            var newGrammerMainClassModel = new EventsMainClassViewModel();

            newGrammerMainClassModel.CmdName = contisionsCmdModel.MainClass;

            newGrammerMainClassModel.HelpInfoMessages = new ObservableCollection<EventsMainClassViewModel.SaveHelpInfoModel>();

            var maintool_fg = contisionsCmdModel.MainToolTip.Split("\n", StringSplitOptions.RemoveEmptyEntries);

            await Task.Run(() =>
            {
                for (int i = 0; i < maintool_fg.Length; i++)
                {
                    newGrammerMainClassModel.HelpInfoMessages.Add(new EventsMainClassViewModel.SaveHelpInfoModel { HelpInfo = maintool_fg[i] });
                }
            });


            saveAllEditer.Add(main, newGrammerMainClassModel);

            #endregion

            #region 主命令下第一参数的数据处理

            var newGrammerMainFirstPar = new EventsParameterFirstViewModel();

            newGrammerMainFirstPar.ParameterOnlyOne = contisionsCmdModel.IsNotSplitChar;

            await Task.Run(() =>
            {
                if (contisionsCmdModel.ParameterToolTip.ContainsKey(0))
                {
                    if (contisionsCmdModel.ParameterToolTip.ContainsKey(0))
                    {
                        var mainpartool_fg = contisionsCmdModel.ParameterToolTip[0][0].Split("\n", StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < mainpartool_fg.Length; i++)
                        {
                            newGrammerMainFirstPar.ParameterHelpInfoMessages.Add(new EventsParameterFirstViewModel.SaveParameterHelpInfo { ParameterHelpInfo = mainpartool_fg[i] });
                        }
                    }
                }
            });

            await Task.Run(() =>
            {
                if (contisionsCmdModel.TermToolTip.ContainsKey(0))
                {
                    if (contisionsCmdModel.TermToolTip[0].ContainsKey(0))
                    {
                        var dic = contisionsCmdModel.TermToolTip[0][0];

                        foreach (var item in dic)
                        {
                            var csfg = item.Value.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                            for (int i = 0; i < csfg.Length; i++)
                            {
                                newGrammerMainFirstPar.NapeHelpInfoMessages.Add(new EventsParameterFirstViewModel.SaveNapeHelpInfo
                                {
                                    NapeHelpInfoMessageSave = csfg[i],

                                    NapeHelpInfoMessageNum = $"第 {item.Key + 1} 项"
                                });
                            }


                        }
                    }
                }
            });

            newGrammerMainFirstPar.ParameterSpiltChar = contisionsCmdModel.TextSplitChar.ToString();

            newGrammerMainFirstPar.ParameterSpiltNum = contisionsCmdModel.TextNum.ToString();

            if (contisionsCmdModel.NeedTpye != null && contisionsCmdModel.NeedTpye.ContainsKey(contisionsCmdModel.MainClass))
            {
                if (contisionsCmdModel.NeedTpye[contisionsCmdModel.MainClass].ContainsKey(0))
                {
                    switch (contisionsCmdModel.NeedTpye[contisionsCmdModel.MainClass][0])
                    {
                        case ThumbClass.Subject:
                            newGrammerMainFirstPar.NapeNumsSelectNow = 0;
                            break;
                        case ThumbClass.Player:
                            newGrammerMainFirstPar.NapeNumsSelectNow = 1;
                            break;
                        case ThumbClass.NPC:
                            newGrammerMainFirstPar.NapeNumsSelectNow = 2;
                            break;
                        case ThumbClass.Conditions:
                            newGrammerMainFirstPar.NapeNumsSelectNow = 3;
                            break;
                        case ThumbClass.Events:
                            newGrammerMainFirstPar.NapeNumsSelectNow = 4;
                            break;
                        case ThumbClass.Objectives:
                            newGrammerMainFirstPar.NapeNumsSelectNow = 5;
                            break;
                        case ThumbClass.Journal:
                            newGrammerMainFirstPar.NapeNumsSelectNow = 6;
                            break;
                        case ThumbClass.Items:
                            newGrammerMainFirstPar.NapeNumsSelectNow = 7;
                            break;
                    }
                }
            }

            foreach (var item in contisionsCmdModel.Tags)
            {
                newGrammerMainFirstPar.ParameterNapeNeedTypes.Add(item);
            }
            saveAllEditer.Add(main.Parameter.Where(t => t.Type == "1").FirstOrDefault(), newGrammerMainFirstPar);

            #endregion

            #region 主命令下其他参数的数据处理

            var getMainParNums = contisionsCmdModel.TextSplitWords.Count;

            var getModel = main.Parameter.Where(t => t.Type == "2").ToList();

            for (int i = 0; i < getMainParNums; i++)
            {
                var newGrammerMainPar = new EventsParameterViewModel();

                if (contisionsCmdModel.ParameterToolTip.ContainsKey(0))
                {
                    var key = i + 1;
                    if (contisionsCmdModel.ParameterToolTip[0].ContainsKey(key))
                    {
                        var mainpartool_fg = contisionsCmdModel.ParameterToolTip[0][key].Split("\n", StringSplitOptions.RemoveEmptyEntries);

                        for (int j = 0; j < mainpartool_fg.Length; j++)
                        {
                            newGrammerMainPar.ParameterHelpInfoMessages.Add(new EventsParameterViewModel.SaveParameterHelpInfo { ParameterHelpInfo = mainpartool_fg[j] });
                        }
                    }
                }

                if (contisionsCmdModel.TermToolTip.ContainsKey(0))
                {
                    if (contisionsCmdModel.TermToolTip[0].ContainsKey(getMainParNums + 1))
                    {
                        var dic = contisionsCmdModel.TermToolTip[0][getMainParNums + 1];

                        foreach (var item in dic)
                        {
                            var csfg = item.Value.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                            for (int j = 0; j < csfg.Length; j++)
                            {
                                newGrammerMainPar.NapeHelpInfoMessages.Add(new EventsParameterViewModel.SaveNapeHelpInfo
                                {
                                    NapeHelpInfoMessage = csfg[j],

                                    NapeHelpInfoMessageNum = $"第 {item.Key + 1} 项"
                                });
                            }
                        }
                    }
                }

                newGrammerMainPar.ParameterSpiltChar = contisionsCmdModel.TextSplitWords[i].i.ToString();

                newGrammerMainPar.ParameterSpiltStep = contisionsCmdModel.TextSplitWords[i].j.ToString();

                if (contisionsCmdModel.NeedTpye != null && contisionsCmdModel.NeedTpye.ContainsKey(contisionsCmdModel.MainClass))
                {
                    if (contisionsCmdModel.NeedTpye[contisionsCmdModel.MainClass].ContainsKey(getMainParNums + 1))
                    {
                        switch (contisionsCmdModel.NeedTpye[contisionsCmdModel.MainClass][getMainParNums + 1])
                        {
                            case ThumbClass.Subject:
                                newGrammerMainPar.NeedCardSelectIndex = 0;
                                break;
                            case ThumbClass.Player:
                                newGrammerMainPar.NeedCardSelectIndex = 1;
                                break;
                            case ThumbClass.NPC:
                                newGrammerMainPar.NeedCardSelectIndex = 2;
                                break;
                            case ThumbClass.Conditions:
                                newGrammerMainPar.NeedCardSelectIndex = 3;
                                break;
                            case ThumbClass.Events:
                                newGrammerMainPar.NeedCardSelectIndex = 4;
                                break;
                            case ThumbClass.Objectives:
                                newGrammerMainPar.NeedCardSelectIndex = 5;
                                break;
                            case ThumbClass.Journal:
                                newGrammerMainPar.NeedCardSelectIndex = 6;
                                break;
                            case ThumbClass.Items:
                                newGrammerMainPar.NeedCardSelectIndex = 7;
                                break;
                        }
                    }
                }

                if (i < getModel.Count)
                {
                    saveAllEditer.Add(getModel[i], newGrammerMainPar);
                }
                else
                {
                    result.SetError("加载错误！");
                    return result;
                }
            }

            #endregion

            #region 子命令的数据处理

            var getModels = this.Parameter.Where(t => t.Type == "3").ToList();

            for (int i = 0; i < contisionsCmdModel.ChildClasses.Count; i++)
            {
                var newGrammerChirdCmdModel = new EventsChirldClassViewModel();

                newGrammerChirdCmdModel.ChirdCmdName = contisionsCmdModel.ChildClasses[i].ChildClass;

                await Task.Run(() =>
                {
                    if (contisionsCmdModel.CmdToolTip.Count > i)
                    {
                        var chirdtool_fg = contisionsCmdModel.CmdToolTip[i].Split("\n", StringSplitOptions.RemoveEmptyEntries);

                        for (int j = 0; j < chirdtool_fg.Length; j++)
                        {
                            newGrammerChirdCmdModel.ChirdCmdHelpInfoMessages.Add(new EventsChirldClassViewModel.SaveHelpInfoModel { ChirdCmdHelpInfo = chirdtool_fg[j] });
                        }
                    }
                });

                if (i < getModels.Count)
                {
                    saveAllEditer.Add(getModels[i], newGrammerChirdCmdModel);
                }
                else
                {
                    result.SetError("加载错误！");

                    return result;
                }

                #region 子命令第一参数的处理

                var getCcmdFirstModel = getModels[i].Parameter.Where(t => t.Type == "1").FirstOrDefault();

                var newGrammerChirdFirstPar = new EventsParameterFirstViewModel();

                await Task.Run(() =>
                {
                    if (contisionsCmdModel.ParameterToolTip.ContainsKey(i + 1))
                    {
                        if (contisionsCmdModel.ParameterToolTip.ContainsKey(0))
                        {
                            var mainpartool_fg = contisionsCmdModel.ParameterToolTip[i + 1][0].Split("\n", StringSplitOptions.RemoveEmptyEntries);

                            for (int j = 0; j < mainpartool_fg.Length; j++)
                            {
                                newGrammerChirdFirstPar.ParameterHelpInfoMessages.Add(new EventsParameterFirstViewModel.SaveParameterHelpInfo { ParameterHelpInfo = mainpartool_fg[j] });
                            }
                        }
                    }
                });

                await Task.Run(() =>
                {
                    if (contisionsCmdModel.TermToolTip.ContainsKey(i + 1))
                    {
                        if (contisionsCmdModel.TermToolTip[i + 1].ContainsKey(0))
                        {
                            var dic = contisionsCmdModel.TermToolTip[i + 1][0];

                            foreach (var item in dic)
                            {
                                var csfg = item.Value.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                                for (int j = 0; j < csfg.Length; j++)
                                {
                                    newGrammerChirdFirstPar.NapeHelpInfoMessages.Add(new EventsParameterFirstViewModel.SaveNapeHelpInfo
                                    {
                                        NapeHelpInfoMessageSave = csfg[j],

                                        NapeHelpInfoMessageNum = $"第 {item.Key + 1} 项"
                                    });
                                }


                            }
                        }
                    }
                });

                newGrammerChirdFirstPar.ParameterSpiltChar = contisionsCmdModel.ChildClasses[i].ChildTextSplitChar.ToString();

                newGrammerChirdFirstPar.ParameterSpiltNum = contisionsCmdModel.ChildClasses[i].ChildTextNum.ToString();

                if (contisionsCmdModel.NeedTpye != null && contisionsCmdModel.NeedTpye.ContainsKey(contisionsCmdModel.ChildClasses[i].ChildClass))
                {
                    if (contisionsCmdModel.NeedTpye[contisionsCmdModel.ChildClasses[i].ChildClass].ContainsKey(0))
                    {
                        switch (contisionsCmdModel.NeedTpye[contisionsCmdModel.ChildClasses[i].ChildClass][0])
                        {
                            case ThumbClass.Subject:
                                newGrammerChirdFirstPar.NapeNumsSelectNow = 0;
                                break;
                            case ThumbClass.Player:
                                newGrammerChirdFirstPar.NapeNumsSelectNow = 1;
                                break;
                            case ThumbClass.NPC:
                                newGrammerChirdFirstPar.NapeNumsSelectNow = 2;
                                break;
                            case ThumbClass.Conditions:
                                newGrammerChirdFirstPar.NapeNumsSelectNow = 3;
                                break;
                            case ThumbClass.Events:
                                newGrammerChirdFirstPar.NapeNumsSelectNow = 4;
                                break;
                            case ThumbClass.Objectives:
                                newGrammerChirdFirstPar.NapeNumsSelectNow = 5;
                                break;
                            case ThumbClass.Journal:
                                newGrammerChirdFirstPar.NapeNumsSelectNow = 6;
                                break;
                            case ThumbClass.Items:
                                newGrammerChirdFirstPar.NapeNumsSelectNow = 7;
                                break;
                        }
                    }
                }

                saveAllEditer.Add(getCcmdFirstModel, newGrammerChirdFirstPar);

                #endregion

                #region 子命令其他参数的数据处理

                //并不允许的情况，这里不做内容处理

                #endregion



            }



            #endregion

            #endregion

            result.SetSuccese("载入成功！");

            return result;
        }

        private async Task<ReturnModel> InputObjectivesModel(ObjectiveCmdModel contisionsCmdModel, GrammerListViewModel main)
        {
            var result = new ReturnModel();

            #region 视图的创建

            var getMainModelNeedParNums = contisionsCmdModel.TextSplitWords.Count;

            for (int i = 0; i < getMainModelNeedParNums; i++)
            {
                main.Parameter.Add(new GrammerListViewModel("2"));
            }

            for (int i = 0; i < contisionsCmdModel.ChildClasses.Count; i++)
            {
                var childCmdViewModel = new GrammerListViewModel("3");

                this.Parameter.Add(childCmdViewModel);

                for (int j = 0; j < contisionsCmdModel.ChildClasses[i].ChildTextSplitWords.Count; j++)
                {
                    childCmdViewModel.Parameter.Add(new GrammerListViewModel("2"));
                }
            }

            #endregion

            #region 数据的处理与绑定

            #region 主命令数据处理

            var newGrammerMainClassModel = new ObjectivesMainClassViewModel();

            newGrammerMainClassModel.CmdName = contisionsCmdModel.MainClass;

            newGrammerMainClassModel.HelpInfoMessages = new ObservableCollection<ObjectivesMainClassViewModel.SaveHelpInfoModel>();

            var maintool_fg = contisionsCmdModel.MainToolTip.Split("\n", StringSplitOptions.RemoveEmptyEntries);

            await Task.Run(() =>
            {
                for (int i = 0; i < maintool_fg.Length; i++)
                {
                    newGrammerMainClassModel.HelpInfoMessages.Add(new ObjectivesMainClassViewModel.SaveHelpInfoModel { HelpInfo = maintool_fg[i] });
                }
            });


            saveAllEditer.Add(main, newGrammerMainClassModel);

            #endregion

            #region 主命令下第一参数的数据处理

            var newGrammerMainFirstPar = new ObjectivesParameterFirstViewModel();

            newGrammerMainFirstPar.ChirdTags = new ObservableCollection<string>(contisionsCmdModel.ChildTags);

            await Task.Run(() =>
            {
                if (contisionsCmdModel.ParameterToolTip.ContainsKey(0))
                {
                    if (contisionsCmdModel.ParameterToolTip.ContainsKey(0))
                    {
                        var mainpartool_fg = contisionsCmdModel.ParameterToolTip[0][0].Split("\n", StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < mainpartool_fg.Length; i++)
                        {
                            newGrammerMainFirstPar.ParameterHelpInfoMessages.Add(new ObjectivesParameterFirstViewModel.SaveParameterHelpInfo { ParameterHelpInfo = mainpartool_fg[i] });
                        }
                    }
                }
            });

            await Task.Run(() =>
            {
                if (contisionsCmdModel.TermToolTip.ContainsKey(0))
                {
                    if (contisionsCmdModel.TermToolTip[0].ContainsKey(0))
                    {
                        var dic = contisionsCmdModel.TermToolTip[0][0];

                        foreach (var item in dic)
                        {
                            var csfg = item.Value.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                            for (int i = 0; i < csfg.Length; i++)
                            {
                                newGrammerMainFirstPar.NapeHelpInfoMessages.Add(new ObjectivesParameterFirstViewModel.SaveNapeHelpInfo
                                {
                                    NapeHelpInfoMessageSave = csfg[i],

                                    NapeHelpInfoMessageNum = $"第 {item.Key + 1} 项"
                                });
                            }


                        }
                    }
                }
            });

            newGrammerMainFirstPar.ParameterSpiltChar = contisionsCmdModel.TextSplitChar.ToString();

            newGrammerMainFirstPar.ParameterSpiltNum = contisionsCmdModel.TextNum.ToString();

            if (contisionsCmdModel.NeedTpye != null && contisionsCmdModel.NeedTpye.ContainsKey(contisionsCmdModel.MainClass))
            {
                if (contisionsCmdModel.NeedTpye[contisionsCmdModel.MainClass].ContainsKey(0))
                {
                    switch (contisionsCmdModel.NeedTpye[contisionsCmdModel.MainClass][0])
                    {
                        case ThumbClass.Subject:
                            newGrammerMainFirstPar.NapeNumsSelectNow = 0;
                            break;
                        case ThumbClass.Player:
                            newGrammerMainFirstPar.NapeNumsSelectNow = 1;
                            break;
                        case ThumbClass.NPC:
                            newGrammerMainFirstPar.NapeNumsSelectNow = 2;
                            break;
                        case ThumbClass.Conditions:
                            newGrammerMainFirstPar.NapeNumsSelectNow = 3;
                            break;
                        case ThumbClass.Events:
                            newGrammerMainFirstPar.NapeNumsSelectNow = 4;
                            break;
                        case ThumbClass.Objectives:
                            newGrammerMainFirstPar.NapeNumsSelectNow = 5;
                            break;
                        case ThumbClass.Journal:
                            newGrammerMainFirstPar.NapeNumsSelectNow = 6;
                            break;
                        case ThumbClass.Items:
                            newGrammerMainFirstPar.NapeNumsSelectNow = 7;
                            break;
                    }
                }
            }

            foreach (var item in contisionsCmdModel.Tags)
            {
                newGrammerMainFirstPar.ParameterNapeNeedTypes.Add(item);
            }
            saveAllEditer.Add(main.Parameter.Where(t => t.Type == "1").FirstOrDefault(), newGrammerMainFirstPar);

            #endregion

            #region 主命令下其他参数的数据处理

            var getMainParNums = contisionsCmdModel.TextSplitWords.Count;

            var getModel = main.Parameter.Where(t => t.Type == "2").ToList();

            for (int i = 0; i < getMainParNums; i++)
            {
                var newGrammerMainPar = new ObjectivesParameterViewModel();

                if (contisionsCmdModel.ParameterToolTip.ContainsKey(0))
                {
                    var key = i + 1;
                    if (contisionsCmdModel.ParameterToolTip[0].ContainsKey(key))
                    {
                        var mainpartool_fg = contisionsCmdModel.ParameterToolTip[0][key].Split("\n", StringSplitOptions.RemoveEmptyEntries);

                        for (int j = 0; j < mainpartool_fg.Length; j++)
                        {
                            newGrammerMainPar.ParameterHelpInfoMessages.Add(new ObjectivesParameterViewModel.SaveParameterHelpInfo { ParameterHelpInfo = mainpartool_fg[j] });
                        }
                    }
                }

                if (contisionsCmdModel.TermToolTip.ContainsKey(0))
                {
                    if (contisionsCmdModel.TermToolTip[0].ContainsKey(getMainParNums + 1))
                    {
                        var dic = contisionsCmdModel.TermToolTip[0][getMainParNums + 1];

                        foreach (var item in dic)
                        {
                            var csfg = item.Value.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                            for (int j = 0; j < csfg.Length; j++)
                            {
                                newGrammerMainPar.NapeHelpInfoMessages.Add(new ObjectivesParameterViewModel.SaveNapeHelpInfo
                                {
                                    NapeHelpInfoMessage = csfg[j],

                                    NapeHelpInfoMessageNum = $"第 {item.Key + 1} 项"
                                });
                            }
                        }
                    }
                }

                newGrammerMainPar.ParameterSpiltChar = contisionsCmdModel.TextSplitWords[i].i.ToString();

                newGrammerMainPar.ParameterSpiltStep = contisionsCmdModel.TextSplitWords[i].j.ToString();

                if (contisionsCmdModel.NeedTpye != null && contisionsCmdModel.NeedTpye.ContainsKey(contisionsCmdModel.MainClass))
                {
                    if (contisionsCmdModel.NeedTpye[contisionsCmdModel.MainClass].ContainsKey(getMainParNums + 1))
                    {
                        switch (contisionsCmdModel.NeedTpye[contisionsCmdModel.MainClass][getMainParNums + 1])
                        {
                            case ThumbClass.Subject:
                                newGrammerMainPar.NeedCardSelectIndex = 0;
                                break;
                            case ThumbClass.Player:
                                newGrammerMainPar.NeedCardSelectIndex = 1;
                                break;
                            case ThumbClass.NPC:
                                newGrammerMainPar.NeedCardSelectIndex = 2;
                                break;
                            case ThumbClass.Conditions:
                                newGrammerMainPar.NeedCardSelectIndex = 3;
                                break;
                            case ThumbClass.Events:
                                newGrammerMainPar.NeedCardSelectIndex = 4;
                                break;
                            case ThumbClass.Objectives:
                                newGrammerMainPar.NeedCardSelectIndex = 5;
                                break;
                            case ThumbClass.Journal:
                                newGrammerMainPar.NeedCardSelectIndex = 6;
                                break;
                            case ThumbClass.Items:
                                newGrammerMainPar.NeedCardSelectIndex = 7;
                                break;
                        }
                    }
                }

                if (i < getModel.Count)
                {
                    saveAllEditer.Add(getModel[i], newGrammerMainPar);
                }
                else
                {
                    result.SetError("加载错误！");
                    return result;
                }
            }

            #endregion

            #region 子命令的数据处理

            var getModels = this.Parameter.Where(t => t.Type == "3").ToList();

            for (int i = 0; i < contisionsCmdModel.ChildClasses.Count; i++)
            {
                var newGrammerChirdCmdModel = new ObjectivesChirldClassViewModel();

                newGrammerChirdCmdModel.ChirdCmdName = contisionsCmdModel.ChildClasses[i].ChildClass;

                await Task.Run(() =>
                {
                    if (contisionsCmdModel.CmdToolTip.Count > i)
                    {
                        var chirdtool_fg = contisionsCmdModel.CmdToolTip[i].Split("\n", StringSplitOptions.RemoveEmptyEntries);

                        for (int j = 0; j < chirdtool_fg.Length; j++)
                        {
                            newGrammerChirdCmdModel.ChirdCmdHelpInfoMessages.Add(new ObjectivesChirldClassViewModel.SaveHelpInfoModel { ChirdCmdHelpInfo = chirdtool_fg[j] });
                        }
                    }
                });

                if (i < getModels.Count)
                {
                    saveAllEditer.Add(getModels[i], newGrammerChirdCmdModel);
                }
                else
                {
                    result.SetError("加载错误！");

                    return result;
                }

                #region 子命令第一参数的处理

                var getCcmdFirstModel = getModels[i].Parameter.Where(t => t.Type == "1").FirstOrDefault();

                var newGrammerChirdFirstPar = new ObjectivesParameterFirstViewModel();

                await Task.Run(() =>
                {
                    if (contisionsCmdModel.ParameterToolTip.ContainsKey(i + 1))
                    {
                        if (contisionsCmdModel.ParameterToolTip.ContainsKey(0))
                        {
                            var mainpartool_fg = contisionsCmdModel.ParameterToolTip[i + 1][0].Split("\n", StringSplitOptions.RemoveEmptyEntries);

                            for (int j = 0; j < mainpartool_fg.Length; j++)
                            {
                                newGrammerChirdFirstPar.ParameterHelpInfoMessages.Add(new ObjectivesParameterFirstViewModel.SaveParameterHelpInfo { ParameterHelpInfo = mainpartool_fg[j] });
                            }
                        }
                    }
                });

                await Task.Run(() =>
                {
                    if (contisionsCmdModel.TermToolTip.ContainsKey(i + 1))
                    {
                        if (contisionsCmdModel.TermToolTip[i + 1].ContainsKey(0))
                        {
                            var dic = contisionsCmdModel.TermToolTip[i + 1][0];

                            foreach (var item in dic)
                            {
                                var csfg = item.Value.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                                for (int j = 0; j < csfg.Length; j++)
                                {
                                    newGrammerChirdFirstPar.NapeHelpInfoMessages.Add(new ObjectivesParameterFirstViewModel.SaveNapeHelpInfo
                                    {
                                        NapeHelpInfoMessageSave = csfg[j],

                                        NapeHelpInfoMessageNum = $"第 {item.Key + 1} 项"
                                    });
                                }


                            }
                        }
                    }
                });

                newGrammerChirdFirstPar.ParameterSpiltChar = contisionsCmdModel.ChildClasses[i].ChildTextSplitChar.ToString();

                newGrammerChirdFirstPar.ParameterSpiltNum = contisionsCmdModel.ChildClasses[i].ChildTextNum.ToString();

                if (contisionsCmdModel.NeedTpye != null && contisionsCmdModel.NeedTpye.ContainsKey(contisionsCmdModel.ChildClasses[i].ChildClass))
                {
                    if (contisionsCmdModel.NeedTpye[contisionsCmdModel.ChildClasses[i].ChildClass].ContainsKey(0))
                    {
                        switch (contisionsCmdModel.NeedTpye[contisionsCmdModel.ChildClasses[i].ChildClass][0])
                        {
                            case ThumbClass.Subject:
                                newGrammerChirdFirstPar.NapeNumsSelectNow = 0;
                                break;
                            case ThumbClass.Player:
                                newGrammerChirdFirstPar.NapeNumsSelectNow = 1;
                                break;
                            case ThumbClass.NPC:
                                newGrammerChirdFirstPar.NapeNumsSelectNow = 2;
                                break;
                            case ThumbClass.Conditions:
                                newGrammerChirdFirstPar.NapeNumsSelectNow = 3;
                                break;
                            case ThumbClass.Events:
                                newGrammerChirdFirstPar.NapeNumsSelectNow = 4;
                                break;
                            case ThumbClass.Objectives:
                                newGrammerChirdFirstPar.NapeNumsSelectNow = 5;
                                break;
                            case ThumbClass.Journal:
                                newGrammerChirdFirstPar.NapeNumsSelectNow = 6;
                                break;
                            case ThumbClass.Items:
                                newGrammerChirdFirstPar.NapeNumsSelectNow = 7;
                                break;
                        }
                    }
                }

                saveAllEditer.Add(getCcmdFirstModel, newGrammerChirdFirstPar);

                #endregion

                #region 子命令其他参数的数据处理

                //并不允许的情况，这里不做内容处理

                #endregion

            }

            #endregion

            #endregion

            result.SetSuccese("载入成功！");

            return result;
        }

        private async Task<ReturnModel> CreateGrammerModel()
        {
            var result = new ReturnModel();

            if (SelectType == 0)
            {
                var getBack = await ConditionModelCreate(Parameter);

                result = getBack;

                if (getBack.Succese)
                {
                    var val = getBack.Backs as ContisionsCmdModel;

                    ContisionLoaderBase.savecmdModels.Add(val);
                    ModelItems.Add(val.MainClass);

                    var message = new MessageModel()
                    {
                        IsLogin = SocketModel.isLogin,
                        JsonInfo = JsonInfo.AddGrammerModel,
                        UserName = SocketModel.userName,
                        Message = FileService.SaveToJson(val),
                        Other = "1"
                    };
                    var jsonMessage = FileService.SaveToJson(message);

                    var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                        SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);

                    if (getMessage == null || !getMessage.Succese)
                    {
                        result.SetError($"请求云端失败 {getMessage.Text}");
                        return result;
                    }

                    var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

                    if (getModel.Token != SocketModel.token)
                    {
                        result.SetError($"请求云端失败 {getMessage.Text}");
                        return result;
                    }

                    var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

                    if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.AddGrammerModel || !getRealMessage.IsLogin)
                    {
                        result.SetError($"请求云端失败 {getRealMessage.Message}");
                        return result;
                    }

                    try
                    {
                        result.SetSuccese($"请求云端成功：{getRealMessage.Message}");
                        return result;
                    }
                    catch
                    {
                        result.SetError($"请求云端失败 获取返回值错误");
                        return result;
                    }
                }

                return result;

            }

            if (SelectType == 1)
            {
                var getBack = await EventModelCreate(Parameter);

                result = getBack;

                if (getBack.Succese)
                {
                    var val = getBack.Backs as EventCmdModel;

                    EventLoaderBase.savecmdModels.Add(val);
                    ModelItems.Add(val.MainClass);

                    var message = new MessageModel()
                    {
                        IsLogin = SocketModel.isLogin,
                        JsonInfo = JsonInfo.AddGrammerModel,
                        UserName = SocketModel.userName,
                        Message = FileService.SaveToJson(val),
                        Other = "2"
                    };
                    var jsonMessage = FileService.SaveToJson(message);

                    var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                        SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);

                    if (getMessage == null || !getMessage.Succese)
                    {
                        result.SetError($"请求云端失败 {getMessage.Text}");
                        return result;
                    }

                    var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

                    if (getModel.Token != SocketModel.token)
                    {
                        result.SetError($"请求云端失败 {getMessage.Text}");
                        return result;
                    }

                    var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

                    if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.AddGrammerModel || !getRealMessage.IsLogin)
                    {
                        result.SetError($"请求云端失败 {getRealMessage.Message}");
                        return result;
                    }

                    try
                    {
                        result.SetSuccese($"请求云端成功：{getRealMessage.Message}");
                        return result;
                    }
                    catch
                    {
                        result.SetError($"请求云端失败 获取返回值错误");
                        return result;
                    }
                }

                return result;
            }

            if (SelectType == 2)
            {
                var getBack = await ObjectiveModelCreate(Parameter);

                result = getBack;

                if (getBack.Succese)
                {
                    var val = getBack.Backs as ObjectiveCmdModel;

                    ObjectiveLoaderBase.savecmdModels.Add(val);
                    ModelItems.Add(val.MainClass);

                    var message = new MessageModel()
                    {
                        IsLogin = SocketModel.isLogin,
                        JsonInfo = JsonInfo.AddGrammerModel,
                        UserName = SocketModel.userName,
                        Message = FileService.SaveToJson(val),
                        Other = "3"
                    };
                    var jsonMessage = FileService.SaveToJson(message);

                    var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                        SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);

                    if (getMessage == null || !getMessage.Succese)
                    {
                        result.SetError($"请求云端失败 {getMessage.Text}");
                        return result;
                    }

                    var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

                    if (getModel.Token != SocketModel.token)
                    {
                        result.SetError($"请求云端失败 {getMessage.Text}");
                        return result;
                    }

                    var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

                    if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.AddGrammerModel || !getRealMessage.IsLogin)
                    {
                        result.SetError($"请求云端失败 {getRealMessage.Message}");
                        return result;
                    }

                    try
                    {
                        result.SetSuccese($"请求云端成功：{getRealMessage.Message}");
                        return result;
                    }
                    catch
                    {
                        result.SetError($"请求云端失败 获取返回值错误");
                        return result;
                    }
                }

                return result;
            }

            result.SetSuccese();

            return result;
        }

        private async Task<ReturnModel> ConditionModelCreate(ObservableCollection<GrammerListViewModel> listViewModel)
        {
            var result = new ReturnModel();

            if(!listViewModel.Where(t => t.Type == "0").Any())
            {
                result.SetError("未找到主命令");

                return result;
            }

            #region 主命令的存储

            var getMain = listViewModel.Where(t => t.Type == "0").FirstOrDefault();

            if(!saveAllEditer.TryGetValue(getMain, out var editer))
            {
                result.SetError("未找到主命令的存储模型");

                return result;
            }

            var getMainModel = editer as ContidonsMainClassViewModel;

            if (string.IsNullOrWhiteSpace(getMainModel.CmdName))
            {
                result.SetError("命令不能是空的字符串");

                return result;
            }

            var newMainHelp = string.Empty;

            await Task.Run(() =>
            {
                foreach (var item in getMainModel.HelpInfoMessages)
                {
                    newMainHelp += item.HelpInfo + "\n";
                }
            });

            #endregion

            var newGrammarModel = new ContisionsCmdModel()
            {
                MainClass = getMainModel.CmdName,
                isContisionCmd = getMainModel.IsUseCondition,
                MainToolTip = newMainHelp,
                CmdToolTip = new List<string>
                {
                    newMainHelp
                }
            };

            #region 主命令下的第一参数存储

            if (!getMain.Parameter.Where(t => t.Type == "1").Any())
            {
                result.SetError("命令不能没有主参数");

                return result;
            }

            var getMainListFirstPar = getMain.Parameter.Where(t => t.Type == "1").FirstOrDefault();

            if (!saveAllEditer.TryGetValue(getMainListFirstPar, out var editerModel))
            {
                result.SetError("主命令下的主参数存储为空");

                return result;
            }

            var mainFirstPar = editerModel as ContidonsParameterFirstViewModel;

            newGrammarModel.TextSplitChar = mainFirstPar.ParameterSpiltChar[0];

            newGrammarModel.TextNum = Convert.ToInt32(mainFirstPar.ParameterSpiltStep);

            var mainFirstParHelp = string.Empty;

            foreach (var item in mainFirstPar.ParameterHelpInfoMessages)
            {
                mainFirstParHelp += item.ParameterHelpInfo + "\n";
            }

            newGrammarModel.ParameterToolTip.Add(0, new Dictionary<int, string> { { 0, mainFirstParHelp } });

            var mainFirstNapesHelp = new Dictionary<int, string>();

            foreach (var item in mainFirstPar.NapeHelpInfoMessages)
            {
                try
                {
                    var num = Convert.ToInt32(item.NapeHelpInfoMessageNum.Split(' ')[1])-1;
                    if (mainFirstNapesHelp.ContainsKey(num))
                    {
                        mainFirstNapesHelp[num] += item.NapeHelpInfoMessage + "\n";
                    }
                    else
                    {
                        mainFirstNapesHelp.Add(num, item.NapeHelpInfoMessage + "\n");
                    }
                }
                catch
                {
                    result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                    return result;
                }
            }

            newGrammarModel.TermToolTip.Add(0, new Dictionary<int, Dictionary<int, string>>
            {
                {0,mainFirstNapesHelp }
            });

            if (mainFirstPar.NeedCardSelectIndex != -1)
            {
                switch (mainFirstPar.NeedCardSelectIndex)
                {
                    case 0:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Subject}
                        });
                        break;
                    case 1:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Player}
                        });
                        break;
                    case 2:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.NPC}
                        });
                        break;
                    case 3:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Conditions}
                        });
                        break;
                    case 4:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Events}
                        });
                        break;
                    case 5:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Objectives}
                        });
                        break;
                    case 6:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Journal}
                        });
                        break;
                    case 7:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Items}
                        });
                        break;
                }
            }

            #endregion

            #region 主命令下其他参数的存储

            var getMainListPars = getMain.Parameter.Where(t => t.Type == "2").ToList();
            var nowChirldNum = 0;
            foreach (var item in getMainListPars)
            {
                nowChirldNum++;
                if (!saveAllEditer.TryGetValue(item, out var chirdEditer))
                {
                    continue;
                }

                var realChirdModel = chirdEditer as ContidonsParameterFirstViewModel;

                var cstep = 0;

                try
                {
                    cstep = Convert.ToInt32(realChirdModel.ParameterSpiltStep);
                }
                catch
                {
                    result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                    return result;
                }

                newGrammarModel.TextSplitWords.Add((realChirdModel.ParameterSpiltChar[0], cstep));

                if (realChirdModel.NeedCardSelectIndex != -1)
                {
                    switch (realChirdModel.NeedCardSelectIndex)
                    {
                        case 0:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Subject);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Subject}
                                });
                            }
                            
                            break;
                        case 1:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Player);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Player}
                                });
                            }
                            break;
                        case 2:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.NPC);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.NPC}
                                });
                            }
                            break;
                        case 3:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Conditions);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Conditions}
                                });
                            }
                            break;
                        case 4:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Events);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Events}
                                });
                            }
                            break;
                        case 5:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Objectives);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Objectives}
                                });
                            }
                            break;
                        case 6:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Journal);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Journal}
                                });
                            }
                            break;
                        case 7:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Items);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Items}
                                });
                            }
                            break;
                    }
                }

                var chirldParHelp = string.Empty;

                foreach (var helps in realChirdModel.ParameterHelpInfoMessages)
                {
                    chirldParHelp += helps.ParameterHelpInfo + "\n";
                }

                if (newGrammarModel.ParameterToolTip.ContainsKey(0))
                {
                    newGrammarModel.ParameterToolTip[0].Add(nowChirldNum, mainFirstParHelp);
                }
                else
                {
                    newGrammarModel.ParameterToolTip.Add(0, new Dictionary<int, string> { { nowChirldNum, mainFirstParHelp } });
                }
                
                var mainParNapesHelp = new Dictionary<int, string>();

                foreach (var helps in realChirdModel.NapeHelpInfoMessages)
                {
                    try
                    {
                        var num = Convert.ToInt32(helps.NapeHelpInfoMessageNum.Split(' ')[1]) - 1;
                        if (mainParNapesHelp.ContainsKey(num))
                        {
                            mainParNapesHelp[num] += helps.NapeHelpInfoMessage + "\n";
                        }
                        else
                        {
                            mainParNapesHelp.Add(num, helps.NapeHelpInfoMessage + "\n");
                        }
                    }
                    catch
                    {
                        result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                        return result;
                    }
                }

                if (newGrammarModel.TermToolTip.ContainsKey(0))
                {
                    newGrammarModel.TermToolTip[0].Add(nowChirldNum, mainParNapesHelp);
                }
                else
                {
                    newGrammarModel.TermToolTip.Add(0, new Dictionary<int, Dictionary<int, string>>
                {
                    {nowChirldNum,mainParNapesHelp }
                });
                }
                
            }

            #endregion

            #region 子命令与参数存储

            var getChirldCmd = listViewModel.Where(t => t.Type == "3").ToList();

            var nowForNum = 0;

            foreach (var item in getChirldCmd)
            {
                if (item == null)
                {
                    continue;
                }

                var newChirldCmdModel = new ChildClasses();

                if (!saveAllEditer.TryGetValue(item, out var CcmdEditer))
                {
                    continue;
                }

                nowForNum++;

                var realCcmdEditer = CcmdEditer as ContidonsChirldClassViewModel;

                newChirldCmdModel.ChildClass = realCcmdEditer.ChirdCmdName;

                var realCcmdHelp = string.Empty;

                foreach (var helps in realCcmdEditer.ChirdCmdHelpInfoMessages)
                {
                    realCcmdHelp += helps.ChirdCmdHelpInfo + "\n";
                }

                newGrammarModel.CmdToolTip.Add(realCcmdHelp);

                #region 子命令第一参数存储
                

                if (!item.Parameter.Where(t => t.Type == "1").Any())
                {
                    result.SetError("命令不能没有主参数");

                    return result;
                }

                var getCcmdFirstPar = item.Parameter.Where(t => t.Type == "1").FirstOrDefault();

                if (getCcmdFirstPar == null)
                {
                    result.SetError("命令的主参数没有保存");

                    return result;
                }

                EditerModelAbstract cCmdFirstParEditer = null;

                if (saveAllEditer.ContainsKey(getCcmdFirstPar))
                {
                    cCmdFirstParEditer = saveAllEditer[getCcmdFirstPar];
                }
                else
                {
                    result.SetError("命令的主参数没有保存");

                    return result;
                }

                if(cCmdFirstParEditer == null)
                {
                    result.SetError("命令的主参数保存出错");

                    return result;
                }

                var getRealcCmdFirstParEditer = cCmdFirstParEditer as ContidonsParameterFirstViewModel;

                newChirldCmdModel.ChildTextSplitChar = getRealcCmdFirstParEditer.ParameterSpiltChar[0];

                try
                {
                    newChirldCmdModel.ChildTextNum = Convert.ToInt32(getRealcCmdFirstParEditer.ParameterSpiltStep);
                }
                catch
                {
                    result.SetError("命令的主参数步长出现了未知字符！请检查后重新保存");

                    return result;
                }

                var newRealCcmdFisrtParHelp = string.Empty;

                foreach (var helps in getRealcCmdFirstParEditer.ParameterHelpInfoMessages)
                {
                    newRealCcmdFisrtParHelp += helps.ParameterHelpInfo + "\n";
                }

                if (newGrammarModel.ParameterToolTip.ContainsKey(nowForNum))
                {
                    newGrammarModel.ParameterToolTip[nowForNum].Add(0, newRealCcmdFisrtParHelp);
                }
                else
                {
                    newGrammarModel.ParameterToolTip.Add(nowForNum, new Dictionary<int, string>
                    {
                        {0,newRealCcmdFisrtParHelp }
                    });
                }

                

                var newRealCcmdFisrtNapeHelp = new Dictionary<int, string>();

                foreach (var helps in getRealcCmdFirstParEditer.NapeHelpInfoMessages)
                {
                    try
                    {
                        var num = Convert.ToInt32(helps.NapeHelpInfoMessageNum.Split(' ')[1]) - 1;
                        if (newRealCcmdFisrtNapeHelp.ContainsKey(num))
                        {
                            newRealCcmdFisrtNapeHelp[num] += helps.NapeHelpInfoMessage + "\n";
                        }
                        else
                        {
                            newRealCcmdFisrtNapeHelp.Add(num, helps.NapeHelpInfoMessage + "\n");
                        }
                    }
                    catch
                    {
                        result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                        return result;
                    }
                }

                if (newGrammarModel.TermToolTip.ContainsKey(nowForNum))
                {
                    newGrammarModel.TermToolTip[nowForNum].Add(0, newRealCcmdFisrtNapeHelp);
                }
                else
                {
                    newGrammarModel.TermToolTip.Add(nowForNum, new Dictionary<int, Dictionary<int, string>>
                {
                    {0,newRealCcmdFisrtNapeHelp }
                });
                }

                

                if (getRealcCmdFirstParEditer.NeedCardSelectIndex != -1)
                {
                    switch (getRealcCmdFirstParEditer.NeedCardSelectIndex)
                    {
                        case 0:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Subject}
                        });
                            break;
                        case 1:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Player}
                        });
                            break;
                        case 2:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.NPC}
                        });
                            break;
                        case 3:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Conditions}
                        });
                            break;
                        case 4:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Events}
                        });
                            break;
                        case 5:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Objectives}
                        });
                            break;
                        case 6:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Journal}
                        });
                            break;
                        case 7:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Items}
                        });
                            break;
                    }
                }
                #endregion

                #region 子命令其他参数存储

                var getCcmdAnyPar = item.Parameter.Where(t => t.Type == "2").ToList();

                var nowCcmdNum = 0;

                foreach (var pars in getCcmdAnyPar)
                {
                    nowCcmdNum++;

                    EditerModelAbstract cCmdAnyParEditer = null;

                    if (saveAllEditer.ContainsKey(pars))
                    {
                        cCmdAnyParEditer = saveAllEditer[pars];
                    }
                    else
                    {
                        continue;
                    }

                    if (cCmdAnyParEditer == null)
                    {
                        continue;
                    }

                    var realCcmdAnyParModel = cCmdAnyParEditer as ContidonsParameterFirstViewModel;

                    var cCstep = 0;

                    try
                    {
                        cCstep = Convert.ToInt32(realCcmdAnyParModel.ParameterSpiltStep);
                    }
                    catch
                    {
                        result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                        return result;
                    }

                    if (newChirldCmdModel.ChildTextSplitWords == null)
                    {
                        newChirldCmdModel.ChildTextSplitWords = new List<(char i, int j)>();
                    }

                    newChirldCmdModel.ChildTextSplitWords.Add((realCcmdAnyParModel.ParameterSpiltChar[0], cCstep));

                    if (realCcmdAnyParModel.NeedCardSelectIndex != -1)
                    {
                        switch (realCcmdAnyParModel.NeedCardSelectIndex)
                        {
                            case 0:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Subject);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Subject}
                                    });
                                }
                                break;
                            case 1:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Player);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Player}
                                    });
                                }
                                break;
                            case 2:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.NPC);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.NPC}
                                    });
                                }
                                break;
                            case 3:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Conditions);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Conditions}
                                    });
                                }
                                break;
                            case 4:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Events);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Events}
                                    });
                                }
                                break;
                            case 5:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Objectives);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Objectives}
                                    });
                                }
                                break;
                            case 6:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Journal);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Journal}
                                    });
                                }
                                break;
                            case 7:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Items);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Items}
                                    });
                                }
                                break;
                        }
                    }

                    var chirldParHelp = string.Empty;

                    foreach (var helps in realCcmdAnyParModel.ParameterHelpInfoMessages)
                    {
                        chirldParHelp += helps.ParameterHelpInfo + "\n";
                    }

                    if (newGrammarModel.ParameterToolTip.ContainsKey(nowForNum))
                    {
                        newGrammarModel.ParameterToolTip[nowForNum].Add(nowCcmdNum, mainFirstParHelp);
                    }
                    else
                    {
                        newGrammarModel.ParameterToolTip.Add(nowForNum, new Dictionary<int, string> { { nowCcmdNum, mainFirstParHelp } });
                    }

                    

                    var mainParNapesHelp = new Dictionary<int, string>();

                    foreach (var helps in realCcmdAnyParModel.NapeHelpInfoMessages)
                    {
                        try
                        {
                            var num = Convert.ToInt32(helps.NapeHelpInfoMessageNum.Split(' ')[1]) - 1;
                            if (mainParNapesHelp.ContainsKey(num))
                            {
                                mainParNapesHelp[num] += helps.NapeHelpInfoMessage + "\n";
                            }
                            else
                            {
                                mainParNapesHelp.Add(num, helps.NapeHelpInfoMessage + "\n");
                            }
                        }
                        catch
                        {
                            result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                            return result;
                        }
                    }

                    if (newGrammarModel.TermToolTip.ContainsKey(nowForNum))
                    {
                        newGrammarModel.TermToolTip[nowForNum].Add(nowCcmdNum, mainParNapesHelp);
                    }
                    else
                    {
                        newGrammarModel.TermToolTip.Add(nowForNum, new Dictionary<int, Dictionary<int, string>>
                        {
                            {nowCcmdNum,mainParNapesHelp }
                        });
                    }

                    
                }


                #endregion

                newGrammarModel.ChildClasses.Add(newChirldCmdModel);
            }

            #endregion


            result.SetSuccese("生成成功",newGrammarModel);

            return result;
        }

        private async Task<ReturnModel> EventModelCreate(ObservableCollection<GrammerListViewModel> listViewModel)
        {
            var result = new ReturnModel();

            if (!listViewModel.Where(t => t.Type == "0").Any())
            {
                result.SetError("未找到主命令");

                return result;
            }

            #region 主命令的存储

            var getMain = listViewModel.Where(t => t.Type == "0").FirstOrDefault();

            if (!saveAllEditer.TryGetValue(getMain, out var editer))
            {
                result.SetError("未找到主命令的存储模型");

                return result;
            }

            var getMainModel = editer as EventsMainClassViewModel;

            if (string.IsNullOrWhiteSpace(getMainModel.CmdName))
            {
                result.SetError("命令不能是空的字符串");

                return result;
            }

            var newMainHelp = string.Empty;

            await Task.Run(() =>
            {
                foreach (var item in getMainModel.HelpInfoMessages)
                {
                    newMainHelp += item.HelpInfo + "\n";
                }
            });

            #endregion

            var newGrammarModel = new EventCmdModel()
            {
                MainClass = getMainModel.CmdName,
                MainToolTip = newMainHelp,
                CmdToolTip = new List<string>
                {
                    newMainHelp
                }
            };

            #region 主命令下的第一参数存储

            if (!getMain.Parameter.Where(t => t.Type == "1").Any())
            {
                result.SetError("命令不能没有主参数");

                return result;
            }

            var getMainListFirstPar = getMain.Parameter.Where(t => t.Type == "1").FirstOrDefault();

            if (!saveAllEditer.TryGetValue(getMainListFirstPar, out var editerModel))
            {
                result.SetError("主命令下的主参数存储为空");

                return result;
            }

            var mainFirstPar = editerModel as EventsParameterFirstViewModel;

            newGrammarModel.TextSplitChar = mainFirstPar.ParameterSpiltChar[0];

            newGrammarModel.TextNum = Convert.ToInt32(mainFirstPar.ParameterSpiltNum);

            var mainFirstParHelp = string.Empty;

            foreach (var item in mainFirstPar.ParameterHelpInfoMessages)
            {
                mainFirstParHelp += item.ParameterHelpInfo + "\n";
            }

            newGrammarModel.ParameterToolTip.Add(0, new Dictionary<int, string> { { 0, mainFirstParHelp } });

            var mainFirstNapesHelp = new Dictionary<int, string>();

            foreach (var item in mainFirstPar.NapeHelpInfoMessages)
            {
                try
                {
                    var num = Convert.ToInt32(item.NapeHelpInfoMessageNum.Split(' ')[1]) - 1;
                    if (mainFirstNapesHelp.ContainsKey(num))
                    {
                        mainFirstNapesHelp[num] += item.NapeHelpInfoMessageSave + "\n";
                    }
                    else
                    {
                        mainFirstNapesHelp.Add(num, item.NapeHelpInfoMessageSave + "\n");
                    }
                }
                catch
                {
                    result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                    return result;
                }
            }

            newGrammarModel.TermToolTip.Add(0, new Dictionary<int, Dictionary<int, string>>
            {
                {0,mainFirstNapesHelp }
            });

            if (mainFirstPar.NapeNumsSelectNow != -1)
            {
                switch (mainFirstPar.NapeNumsSelectNow)
                {
                    case 0:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Subject}
                        });
                        break;
                    case 1:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Player}
                        });
                        break;
                    case 2:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.NPC}
                        });
                        break;
                    case 3:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Conditions}
                        });
                        break;
                    case 4:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Events}
                        });
                        break;
                    case 5:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Objectives}
                        });
                        break;
                    case 6:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Journal}
                        });
                        break;
                    case 7:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Items}
                        });
                        break;
                }
            }

            newGrammarModel.IsNotSplitChar = mainFirstPar.ParameterOnlyOne;

            newGrammarModel.Tags = new List<string>(mainFirstPar.ParameterNapeNeedTypes);

            #endregion

            #region 主命令下其他参数的存储

            var getMainListPars = getMain.Parameter.Where(t => t.Type == "2").ToList();
            var nowChirldNum = 0;
            foreach (var item in getMainListPars)
            {
                nowChirldNum++;
                if (!saveAllEditer.TryGetValue(item, out var chirdEditer))
                {
                    continue;
                }

                var realChirdModel = chirdEditer as EventsParameterViewModel;

                var cstep = 0;

                try
                {
                    cstep = Convert.ToInt32(realChirdModel.ParameterSpiltStep);
                }
                catch
                {
                    result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                    return result;
                }

                newGrammarModel.TextSplitWords.Add((realChirdModel.ParameterSpiltChar[0], cstep));

                if (realChirdModel.NeedCardSelectIndex != -1)
                {
                    switch (realChirdModel.NeedCardSelectIndex)
                    {
                        case 0:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Subject);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Subject}
                                });
                            }

                            break;
                        case 1:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Player);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Player}
                                });
                            }
                            break;
                        case 2:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.NPC);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.NPC}
                                });
                            }
                            break;
                        case 3:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Conditions);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Conditions}
                                });
                            }
                            break;
                        case 4:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Events);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Events}
                                });
                            }
                            break;
                        case 5:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Objectives);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Objectives}
                                });
                            }
                            break;
                        case 6:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Journal);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Journal}
                                });
                            }
                            break;
                        case 7:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Items);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Items}
                                });
                            }
                            break;
                    }
                }

                var chirldParHelp = string.Empty;

                foreach (var helps in realChirdModel.ParameterHelpInfoMessages)
                {
                    chirldParHelp += helps.ParameterHelpInfo + "\n";
                }

                if (newGrammarModel.ParameterToolTip.ContainsKey(0))
                {
                    newGrammarModel.ParameterToolTip[0].Add(nowChirldNum, mainFirstParHelp);
                }
                else
                {
                    newGrammarModel.ParameterToolTip.Add(0, new Dictionary<int, string> { { nowChirldNum, mainFirstParHelp } });
                }

                var mainParNapesHelp = new Dictionary<int, string>();

                foreach (var helps in realChirdModel.NapeHelpInfoMessages)
                {
                    try
                    {
                        var num = Convert.ToInt32(helps.NapeHelpInfoMessageNum.Split(' ')[1]) - 1;
                        if (mainParNapesHelp.ContainsKey(num))
                        {
                            mainParNapesHelp[num] += helps.NapeHelpInfoMessage + "\n";
                        }
                        else
                        {
                            mainParNapesHelp.Add(num, helps.NapeHelpInfoMessage + "\n");
                        }
                    }
                    catch
                    {
                        result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                        return result;
                    }
                }

                if (newGrammarModel.TermToolTip.ContainsKey(0))
                {
                    newGrammarModel.TermToolTip[0].Add(nowChirldNum, mainParNapesHelp);
                }
                else
                {
                    newGrammarModel.TermToolTip.Add(0, new Dictionary<int, Dictionary<int, string>>
                {
                    {nowChirldNum,mainParNapesHelp }
                });
                }

            }

            #endregion

            #region 子命令与参数存储

            var getChirldCmd = listViewModel.Where(t => t.Type == "3").ToList();

            var nowForNum = 0;

            foreach (var item in getChirldCmd)
            {
                if (item == null)
                {
                    continue;
                }

                var newChirldCmdModel = new ChildClasses();

                if (!saveAllEditer.TryGetValue(item, out var CcmdEditer))
                {
                    continue;
                }

                nowForNum++;

                var realCcmdEditer = CcmdEditer as EventsChirldClassViewModel;

                newChirldCmdModel.ChildClass = realCcmdEditer.ChirdCmdName;

                var realCcmdHelp = string.Empty;

                foreach (var helps in realCcmdEditer.ChirdCmdHelpInfoMessages)
                {
                    realCcmdHelp += helps.ChirdCmdHelpInfo + "\n";
                }

                newGrammarModel.CmdToolTip.Add(realCcmdHelp);

                #region 子命令第一参数存储


                if (!item.Parameter.Where(t => t.Type == "1").Any())
                {
                    result.SetError("命令不能没有主参数");

                    return result;
                }

                var getCcmdFirstPar = item.Parameter.Where(t => t.Type == "1").FirstOrDefault();

                if (getCcmdFirstPar == null)
                {
                    result.SetError("命令的主参数没有保存");

                    return result;
                }

                EditerModelAbstract cCmdFirstParEditer = null;

                if (saveAllEditer.ContainsKey(getCcmdFirstPar))
                {
                    cCmdFirstParEditer = saveAllEditer[getCcmdFirstPar];
                }
                else
                {
                    result.SetError("命令的主参数没有保存");

                    return result;
                }

                if (cCmdFirstParEditer == null)
                {
                    result.SetError("命令的主参数保存出错");

                    return result;
                }

                var getRealcCmdFirstParEditer = cCmdFirstParEditer as EventsParameterFirstViewModel;

                newChirldCmdModel.ChildTextSplitChar = getRealcCmdFirstParEditer.ParameterSpiltChar[0];

                try
                {
                    newChirldCmdModel.ChildTextNum = Convert.ToInt32(getRealcCmdFirstParEditer.ParameterSpiltNum);
                }
                catch
                {
                    result.SetError("命令的主参数步长出现了未知字符！请检查后重新保存");

                    return result;
                }

                var newRealCcmdFisrtParHelp = string.Empty;

                foreach (var helps in getRealcCmdFirstParEditer.ParameterHelpInfoMessages)
                {
                    newRealCcmdFisrtParHelp += helps.ParameterHelpInfo + "\n";
                }

                if (newGrammarModel.ParameterToolTip.ContainsKey(nowForNum))
                {
                    newGrammarModel.ParameterToolTip[nowForNum].Add(0, newRealCcmdFisrtParHelp);
                }
                else
                {
                    newGrammarModel.ParameterToolTip.Add(nowForNum, new Dictionary<int, string>
                    {
                        {0,newRealCcmdFisrtParHelp }
                    });
                }



                var newRealCcmdFisrtNapeHelp = new Dictionary<int, string>();

                foreach (var helps in getRealcCmdFirstParEditer.NapeHelpInfoMessages)
                {
                    try
                    {
                        var num = Convert.ToInt32(helps.NapeHelpInfoMessageNum.Split(' ')[1]) - 1;
                        if (newRealCcmdFisrtNapeHelp.ContainsKey(num))
                        {
                            newRealCcmdFisrtNapeHelp[num] += helps.NapeHelpInfoMessageSave + "\n";
                        }
                        else
                        {
                            newRealCcmdFisrtNapeHelp.Add(num, helps.NapeHelpInfoMessageSave + "\n");
                        }
                    }
                    catch
                    {
                        result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                        return result;
                    }
                }

                if (newGrammarModel.TermToolTip.ContainsKey(nowForNum))
                {
                    newGrammarModel.TermToolTip[nowForNum].Add(0, newRealCcmdFisrtNapeHelp);
                }
                else
                {
                    newGrammarModel.TermToolTip.Add(nowForNum, new Dictionary<int, Dictionary<int, string>>
                {
                    {0,newRealCcmdFisrtNapeHelp }
                });
                }



                if (getRealcCmdFirstParEditer.NapeNumsSelectNow != -1)
                {
                    switch (getRealcCmdFirstParEditer.NapeNumsSelectNow)
                    {
                        case 0:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Subject}
                        });
                            break;
                        case 1:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Player}
                        });
                            break;
                        case 2:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.NPC}
                        });
                            break;
                        case 3:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Conditions}
                        });
                            break;
                        case 4:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Events}
                        });
                            break;
                        case 5:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Objectives}
                        });
                            break;
                        case 6:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Journal}
                        });
                            break;
                        case 7:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Items}
                        });
                            break;
                    }
                }
                #endregion

                #region 子命令其他参数存储

                var getCcmdAnyPar = item.Parameter.Where(t => t.Type == "2").ToList();

                var nowCcmdNum = 0;

                foreach (var pars in getCcmdAnyPar)
                {
                    nowCcmdNum++;

                    EditerModelAbstract cCmdAnyParEditer = null;

                    if (saveAllEditer.ContainsKey(pars))
                    {
                        cCmdAnyParEditer = saveAllEditer[pars];
                    }
                    else
                    {
                        continue;
                    }

                    if (cCmdAnyParEditer == null)
                    {
                        continue;
                    }

                    var realCcmdAnyParModel = cCmdAnyParEditer as EventsParameterViewModel;

                    var cCstep = 0;

                    try
                    {
                        cCstep = Convert.ToInt32(realCcmdAnyParModel.ParameterSpiltStep);
                    }
                    catch
                    {
                        result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                        return result;
                    }

                    if (newChirldCmdModel.ChildTextSplitWords == null)
                    {
                        newChirldCmdModel.ChildTextSplitWords = new List<(char i, int j)>();
                    }

                    newChirldCmdModel.ChildTextSplitWords.Add((realCcmdAnyParModel.ParameterSpiltChar[0], cCstep));

                    if (realCcmdAnyParModel.NeedCardSelectIndex != -1)
                    {
                        switch (realCcmdAnyParModel.NeedCardSelectIndex)
                        {
                            case 0:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Subject);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Subject}
                                    });
                                }
                                break;
                            case 1:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Player);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Player}
                                    });
                                }
                                break;
                            case 2:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.NPC);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.NPC}
                                    });
                                }
                                break;
                            case 3:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Conditions);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Conditions}
                                    });
                                }
                                break;
                            case 4:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Events);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Events}
                                    });
                                }
                                break;
                            case 5:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Objectives);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Objectives}
                                    });
                                }
                                break;
                            case 6:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Journal);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Journal}
                                    });
                                }
                                break;
                            case 7:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Items);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Items}
                                    });
                                }
                                break;
                        }
                    }

                    var chirldParHelp = string.Empty;

                    foreach (var helps in realCcmdAnyParModel.ParameterHelpInfoMessages)
                    {
                        chirldParHelp += helps.ParameterHelpInfo + "\n";
                    }

                    if (newGrammarModel.ParameterToolTip.ContainsKey(nowForNum))
                    {
                        newGrammarModel.ParameterToolTip[nowForNum].Add(nowCcmdNum, mainFirstParHelp);
                    }
                    else
                    {
                        newGrammarModel.ParameterToolTip.Add(nowForNum, new Dictionary<int, string> { { nowCcmdNum, mainFirstParHelp } });
                    }



                    var mainParNapesHelp = new Dictionary<int, string>();

                    foreach (var helps in realCcmdAnyParModel.NapeHelpInfoMessages)
                    {
                        try
                        {
                            var num = Convert.ToInt32(helps.NapeHelpInfoMessageNum.Split(' ')[1]) - 1;
                            if (mainParNapesHelp.ContainsKey(num))
                            {
                                mainParNapesHelp[num] += helps.NapeHelpInfoMessage + "\n";
                            }
                            else
                            {
                                mainParNapesHelp.Add(num, helps.NapeHelpInfoMessage + "\n");
                            }
                        }
                        catch
                        {
                            result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                            return result;
                        }
                    }

                    if (newGrammarModel.TermToolTip.ContainsKey(nowForNum))
                    {
                        newGrammarModel.TermToolTip[nowForNum].Add(nowCcmdNum, mainParNapesHelp);
                    }
                    else
                    {
                        newGrammarModel.TermToolTip.Add(nowForNum, new Dictionary<int, Dictionary<int, string>>
                        {
                            {nowCcmdNum,mainParNapesHelp }
                        });
                    }


                }


                #endregion

                newGrammarModel.ChildClasses.Add(newChirldCmdModel);
            }

            #endregion


            result.SetSuccese("生成成功", newGrammarModel);

            return result;
        }

        private async Task<ReturnModel> ObjectiveModelCreate(ObservableCollection<GrammerListViewModel> listViewModel)
        {
            var result = new ReturnModel();

            if (!listViewModel.Where(t => t.Type == "0").Any())
            {
                result.SetError("未找到主命令");

                return result;
            }

            #region 主命令的存储

            var getMain = listViewModel.Where(t => t.Type == "0").FirstOrDefault();

            if (!saveAllEditer.TryGetValue(getMain, out var editer))
            {
                result.SetError("未找到主命令的存储模型");

                return result;
            }

            var getMainModel = editer as ObjectivesMainClassViewModel;

            if (string.IsNullOrWhiteSpace(getMainModel.CmdName))
            {
                result.SetError("命令不能是空的字符串");

                return result;
            }

            var newMainHelp = string.Empty;

            await Task.Run(() =>
            {
                foreach (var item in getMainModel.HelpInfoMessages)
                {
                    newMainHelp += item.HelpInfo + "\n";
                }
            });

            #endregion

            var newGrammarModel = new ObjectiveCmdModel()
            {
                MainClass = getMainModel.CmdName,
                MainToolTip = newMainHelp,
                CmdToolTip = new List<string>
                {
                    newMainHelp
                }
            };

            #region 主命令下的第一参数存储

            if (!getMain.Parameter.Where(t => t.Type == "1").Any())
            {
                result.SetError("命令不能没有主参数");

                return result;
            }

            var getMainListFirstPar = getMain.Parameter.Where(t => t.Type == "1").FirstOrDefault();

            if (!saveAllEditer.TryGetValue(getMainListFirstPar, out var editerModel))
            {
                result.SetError("主命令下的主参数存储为空");

                return result;
            }

            var mainFirstPar = editerModel as ObjectivesParameterFirstViewModel;

            newGrammarModel.TextSplitChar = mainFirstPar.ParameterSpiltChar[0];

            newGrammarModel.TextNum = Convert.ToInt32(mainFirstPar.ParameterSpiltNum);

            var mainFirstParHelp = string.Empty;

            foreach (var item in mainFirstPar.ParameterHelpInfoMessages)
            {
                mainFirstParHelp += item.ParameterHelpInfo + "\n";
            }

            newGrammarModel.ParameterToolTip.Add(0, new Dictionary<int, string> { { 0, mainFirstParHelp } });

            var mainFirstNapesHelp = new Dictionary<int, string>();

            foreach (var item in mainFirstPar.NapeHelpInfoMessages)
            {
                try
                {
                    var num = Convert.ToInt32(item.NapeHelpInfoMessageNum.Split(' ')[1]) - 1;
                    if (mainFirstNapesHelp.ContainsKey(num))
                    {
                        mainFirstNapesHelp[num] += item.NapeHelpInfoMessageSave + "\n";
                    }
                    else
                    {
                        mainFirstNapesHelp.Add(num, item.NapeHelpInfoMessageSave + "\n");
                    }
                }
                catch
                {
                    result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                    return result;
                }
            }

            newGrammarModel.TermToolTip.Add(0, new Dictionary<int, Dictionary<int, string>>
            {
                {0,mainFirstNapesHelp }
            });

            if (mainFirstPar.NapeNumsSelectNow != -1)
            {
                switch (mainFirstPar.NapeNumsSelectNow)
                {
                    case 0:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Subject}
                        });
                        break;
                    case 1:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Player}
                        });
                        break;
                    case 2:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.NPC}
                        });
                        break;
                    case 3:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Conditions}
                        });
                        break;
                    case 4:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Events}
                        });
                        break;
                    case 5:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Objectives}
                        });
                        break;
                    case 6:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Journal}
                        });
                        break;
                    case 7:
                        newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Items}
                        });
                        break;
                }
            }

            newGrammarModel.ChildTags = new List<string>(mainFirstPar.ChirdTags);

            newGrammarModel.Tags = new List<string>(mainFirstPar.ParameterNapeNeedTypes);

            #endregion

            #region 主命令下其他参数的存储

            var getMainListPars = getMain.Parameter.Where(t => t.Type == "2").ToList();
            var nowChirldNum = 0;
            foreach (var item in getMainListPars)
            {
                nowChirldNum++;
                if (!saveAllEditer.TryGetValue(item, out var chirdEditer))
                {
                    continue;
                }

                var realChirdModel = chirdEditer as ObjectivesParameterViewModel;

                var cstep = 0;

                try
                {
                    cstep = Convert.ToInt32(realChirdModel.ParameterSpiltStep);
                }
                catch
                {
                    result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                    return result;
                }

                newGrammarModel.TextSplitWords.Add((realChirdModel.ParameterSpiltChar[0], cstep));

                if (realChirdModel.NeedCardSelectIndex != -1)
                {
                    switch (realChirdModel.NeedCardSelectIndex)
                    {
                        case 0:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Subject);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Subject}
                                });
                            }

                            break;
                        case 1:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Player);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Player}
                                });
                            }
                            break;
                        case 2:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.NPC);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.NPC}
                                });
                            }
                            break;
                        case 3:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Conditions);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Conditions}
                                });
                            }
                            break;
                        case 4:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Events);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Events}
                                });
                            }
                            break;
                        case 5:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Objectives);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Objectives}
                                });
                            }
                            break;
                        case 6:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Journal);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Journal}
                                });
                            }
                            break;
                        case 7:
                            if (newGrammarModel.NeedTpye.ContainsKey(getMainModel.CmdName))
                            {
                                newGrammarModel.NeedTpye[getMainModel.CmdName].Add(nowChirldNum, ThumbClass.Items);
                            }
                            else
                            {
                                newGrammarModel.NeedTpye.Add(getMainModel.CmdName, new Dictionary<int, ThumbClass>
                                {
                                    {nowChirldNum, ThumbClass.Items}
                                });
                            }
                            break;
                    }
                }

                var chirldParHelp = string.Empty;

                foreach (var helps in realChirdModel.ParameterHelpInfoMessages)
                {
                    chirldParHelp += helps.ParameterHelpInfo + "\n";
                }

                if (newGrammarModel.ParameterToolTip.ContainsKey(0))
                {
                    newGrammarModel.ParameterToolTip[0].Add(nowChirldNum, mainFirstParHelp);
                }
                else
                {
                    newGrammarModel.ParameterToolTip.Add(0, new Dictionary<int, string> { { nowChirldNum, mainFirstParHelp } });
                }

                var mainParNapesHelp = new Dictionary<int, string>();

                foreach (var helps in realChirdModel.NapeHelpInfoMessages)
                {
                    try
                    {
                        var num = Convert.ToInt32(helps.NapeHelpInfoMessageNum.Split(' ')[1]) - 1;
                        if (mainParNapesHelp.ContainsKey(num))
                        {
                            mainParNapesHelp[num] += helps.NapeHelpInfoMessage + "\n";
                        }
                        else
                        {
                            mainParNapesHelp.Add(num, helps.NapeHelpInfoMessage + "\n");
                        }
                    }
                    catch
                    {
                        result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                        return result;
                    }
                }

                if (newGrammarModel.TermToolTip.ContainsKey(0))
                {
                    newGrammarModel.TermToolTip[0].Add(nowChirldNum, mainParNapesHelp);
                }
                else
                {
                    newGrammarModel.TermToolTip.Add(0, new Dictionary<int, Dictionary<int, string>>
                {
                    {nowChirldNum,mainParNapesHelp }
                });
                }

            }

            #endregion

            #region 子命令与参数存储

            var getChirldCmd = listViewModel.Where(t => t.Type == "3").ToList();

            var nowForNum = 0;

            foreach (var item in getChirldCmd)
            {
                if (item == null)
                {
                    continue;
                }

                var newChirldCmdModel = new ChildClasses();

                if (!saveAllEditer.TryGetValue(item, out var CcmdEditer))
                {
                    continue;
                }

                nowForNum++;

                var realCcmdEditer = CcmdEditer as ObjectivesChirldClassViewModel;

                newChirldCmdModel.ChildClass = realCcmdEditer.ChirdCmdName;

                var realCcmdHelp = string.Empty;

                foreach (var helps in realCcmdEditer.ChirdCmdHelpInfoMessages)
                {
                    realCcmdHelp += helps.ChirdCmdHelpInfo + "\n";
                }

                newGrammarModel.CmdToolTip.Add(realCcmdHelp);

                #region 子命令第一参数存储


                if (!item.Parameter.Where(t => t.Type == "1").Any())
                {
                    result.SetError("命令不能没有主参数");

                    return result;
                }

                var getCcmdFirstPar = item.Parameter.Where(t => t.Type == "1").FirstOrDefault();

                if (getCcmdFirstPar == null)
                {
                    result.SetError("命令的主参数没有保存");

                    return result;
                }

                EditerModelAbstract cCmdFirstParEditer = null;

                if (saveAllEditer.ContainsKey(getCcmdFirstPar))
                {
                    cCmdFirstParEditer = saveAllEditer[getCcmdFirstPar];
                }
                else
                {
                    result.SetError("命令的主参数没有保存");

                    return result;
                }

                if (cCmdFirstParEditer == null)
                {
                    result.SetError("命令的主参数保存出错");

                    return result;
                }

                var getRealcCmdFirstParEditer = cCmdFirstParEditer as ObjectivesParameterFirstViewModel;

                newChirldCmdModel.ChildTextSplitChar = getRealcCmdFirstParEditer.ParameterSpiltChar[0];

                try
                {
                    newChirldCmdModel.ChildTextNum = Convert.ToInt32(getRealcCmdFirstParEditer.ParameterSpiltNum);
                }
                catch
                {
                    result.SetError("命令的主参数步长出现了未知字符！请检查后重新保存");

                    return result;
                }

                var newRealCcmdFisrtParHelp = string.Empty;

                foreach (var helps in getRealcCmdFirstParEditer.ParameterHelpInfoMessages)
                {
                    newRealCcmdFisrtParHelp += helps.ParameterHelpInfo + "\n";
                }

                if (newGrammarModel.ParameterToolTip.ContainsKey(nowForNum))
                {
                    newGrammarModel.ParameterToolTip[nowForNum].Add(0, newRealCcmdFisrtParHelp);
                }
                else
                {
                    newGrammarModel.ParameterToolTip.Add(nowForNum, new Dictionary<int, string>
                    {
                        {0,newRealCcmdFisrtParHelp }
                    });
                }



                var newRealCcmdFisrtNapeHelp = new Dictionary<int, string>();

                foreach (var helps in getRealcCmdFirstParEditer.NapeHelpInfoMessages)
                {
                    try
                    {
                        var num = Convert.ToInt32(helps.NapeHelpInfoMessageNum.Split(' ')[1]) - 1;
                        if (newRealCcmdFisrtNapeHelp.ContainsKey(num))
                        {
                            newRealCcmdFisrtNapeHelp[num] += helps.NapeHelpInfoMessageSave + "\n";
                        }
                        else
                        {
                            newRealCcmdFisrtNapeHelp.Add(num, helps.NapeHelpInfoMessageSave + "\n");
                        }
                    }
                    catch
                    {
                        result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                        return result;
                    }
                }

                if (newGrammarModel.TermToolTip.ContainsKey(nowForNum))
                {
                    newGrammarModel.TermToolTip[nowForNum].Add(0, newRealCcmdFisrtNapeHelp);
                }
                else
                {
                    newGrammarModel.TermToolTip.Add(nowForNum, new Dictionary<int, Dictionary<int, string>>
                {
                    {0,newRealCcmdFisrtNapeHelp }
                });
                }



                if (getRealcCmdFirstParEditer.NapeNumsSelectNow != -1)
                {
                    switch (getRealcCmdFirstParEditer.NapeNumsSelectNow)
                    {
                        case 0:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Subject}
                        });
                            break;
                        case 1:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Player}
                        });
                            break;
                        case 2:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.NPC}
                        });
                            break;
                        case 3:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Conditions}
                        });
                            break;
                        case 4:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Events}
                        });
                            break;
                        case 5:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Objectives}
                        });
                            break;
                        case 6:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Journal}
                        });
                            break;
                        case 7:
                            newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                        {
                            {0, ThumbClass.Items}
                        });
                            break;
                    }
                }
                #endregion

                #region 子命令其他参数存储

                var getCcmdAnyPar = item.Parameter.Where(t => t.Type == "2").ToList();

                var nowCcmdNum = 0;

                foreach (var pars in getCcmdAnyPar)
                {
                    nowCcmdNum++;

                    EditerModelAbstract cCmdAnyParEditer = null;

                    if (saveAllEditer.ContainsKey(pars))
                    {
                        cCmdAnyParEditer = saveAllEditer[pars];
                    }
                    else
                    {
                        continue;
                    }

                    if (cCmdAnyParEditer == null)
                    {
                        continue;
                    }

                    var realCcmdAnyParModel = cCmdAnyParEditer as ObjectivesParameterViewModel;

                    var cCstep = 0;

                    try
                    {
                        cCstep = Convert.ToInt32(realCcmdAnyParModel.ParameterSpiltStep);
                    }
                    catch
                    {
                        result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                        return result;
                    }

                    if (newChirldCmdModel.ChildTextSplitWords == null)
                    {
                        newChirldCmdModel.ChildTextSplitWords = new List<(char i, int j)>();
                    }

                    newChirldCmdModel.ChildTextSplitWords.Add((realCcmdAnyParModel.ParameterSpiltChar[0], cCstep));

                    if (realCcmdAnyParModel.NeedCardSelectIndex != -1)
                    {
                        switch (realCcmdAnyParModel.NeedCardSelectIndex)
                        {
                            case 0:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Subject);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Subject}
                                    });
                                }
                                break;
                            case 1:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Player);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Player}
                                    });
                                }
                                break;
                            case 2:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.NPC);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.NPC}
                                    });
                                }
                                break;
                            case 3:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Conditions);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Conditions}
                                    });
                                }
                                break;
                            case 4:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Events);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Events}
                                    });
                                }
                                break;
                            case 5:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Objectives);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Objectives}
                                    });
                                }
                                break;
                            case 6:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Journal);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Journal}
                                    });
                                }
                                break;
                            case 7:
                                if (newGrammarModel.NeedTpye.ContainsKey(realCcmdEditer.ChirdCmdName))
                                {
                                    newGrammarModel.NeedTpye[realCcmdEditer.ChirdCmdName].Add(nowCcmdNum, ThumbClass.Items);
                                }
                                else
                                {
                                    newGrammarModel.NeedTpye.Add(realCcmdEditer.ChirdCmdName, new Dictionary<int, ThumbClass>
                                    {
                                        {nowCcmdNum, ThumbClass.Items}
                                    });
                                }
                                break;
                        }
                    }

                    var chirldParHelp = string.Empty;

                    foreach (var helps in realCcmdAnyParModel.ParameterHelpInfoMessages)
                    {
                        chirldParHelp += helps.ParameterHelpInfo + "\n";
                    }

                    if (newGrammarModel.ParameterToolTip.ContainsKey(nowForNum))
                    {
                        newGrammarModel.ParameterToolTip[nowForNum].Add(nowCcmdNum, mainFirstParHelp);
                    }
                    else
                    {
                        newGrammarModel.ParameterToolTip.Add(nowForNum, new Dictionary<int, string> { { nowCcmdNum, mainFirstParHelp } });
                    }



                    var mainParNapesHelp = new Dictionary<int, string>();

                    foreach (var helps in realCcmdAnyParModel.NapeHelpInfoMessages)
                    {
                        try
                        {
                            var num = Convert.ToInt32(helps.NapeHelpInfoMessageNum.Split(' ')[1]) - 1;
                            if (mainParNapesHelp.ContainsKey(num))
                            {
                                mainParNapesHelp[num] += helps.NapeHelpInfoMessage + "\n";
                            }
                            else
                            {
                                mainParNapesHelp.Add(num, helps.NapeHelpInfoMessage + "\n");
                            }
                        }
                        catch
                        {
                            result.SetError("有关项的存储出现了未知字符请不要随意修改项的内容");

                            return result;
                        }
                    }

                    if (newGrammarModel.TermToolTip.ContainsKey(nowForNum))
                    {
                        newGrammarModel.TermToolTip[nowForNum].Add(nowCcmdNum, mainParNapesHelp);
                    }
                    else
                    {
                        newGrammarModel.TermToolTip.Add(nowForNum, new Dictionary<int, Dictionary<int, string>>
                        {
                            {nowCcmdNum,mainParNapesHelp }
                        });
                    }


                }


                #endregion

                newGrammarModel.ChildClasses.Add(newChirldCmdModel);
            }

            #endregion

            result.SetSuccese();

            return result;
        }

        private void addToList(GrammerListViewModel listViewModel)
        {
            this.Parameter.Add(listViewModel);
        }

        private void removeToList(GrammerListViewModel listViewModel)
        {
            this.Parameter.Remove(listViewModel);
        }

        private int GetModelsID()
        {
            return SelectType;
        }

        private void ChangeEditers(EditerModelAbstract editerModel, GrammerListViewModel listViewModel)
        {
            if (saveAllEditer.ContainsKey(listViewModel))
            {
                this.Editer = saveAllEditer[listViewModel];
            }
            else
            {
                saveAllEditer.Add(listViewModel, editerModel);

                this.Editer = editerModel;
            }

            
        }

        #endregion

        public partial class GrammerListViewModel : ObservableObject
        {
            public GrammerListViewModel(string type)
            {
                this.Type = type;


                if(this.Type == "3"|| this.Type == "0")
                {
                    this.Parameter.Add(new GrammerListViewModel("1") { father = this});
                }
                GrammerSelect = this;

                Uid = nowID;
                nowID++;
            }

            #region 属性与字段

            public int Uid { get; set; }

            private GrammerListViewModel father;

            [ObservableProperty]
            private string _Type = "";

            [ObservableProperty]
            private GrammerListViewModel _GrammerSelect;

            [ObservableProperty]
            private int _GrammerSelectIndex = -1;

            [ObservableProperty]
            private ObservableCollection<GrammerListViewModel> _Parameter = new ObservableCollection<GrammerListViewModel>();

            #endregion

            #region 命令

            [RelayCommand()]
            private void AddControl()
            {
                if (Type == "0" || Type == "3")
                {
                    var one = new GrammerListViewModel("3");
                    GrammerModelViewModel._addList(one);
                }
                if(Type == "1" || Type == "2")
                {
                    father.Parameter.Add(new GrammerListViewModel("2") { father = father});
                }
            }

            [RelayCommand()]
            private void DeleteControl()
            {
                if (Type == "0" || Type == "3")
                {
                    GrammerModelViewModel._removeList(this);
                }
                if (Type == "1" || Type == "2")
                {
                    father.Parameter.Remove(this);
                }
            }

            [RelayCommand()]
            private void SelectIt()
            {
                var getModelID = GrammerModelViewModel._getModelId();
                
                if (Type == "0")
                {
                    if (getModelID == 0)
                    {
                        var model = new ContidonsMainClassViewModel();
                        GrammerModelViewModel._changeEditer(model,this);
                    }
                    else if (getModelID==1)
                    {
                        var model = new EventsMainClassViewModel();
                        GrammerModelViewModel._changeEditer(model, this);
                    }
                    else if (getModelID == 2)
                    {
                        var model = new ObjectivesMainClassViewModel();
                        GrammerModelViewModel._changeEditer(model, this);
                    }
                }
                else if(Type == "1")
                {
                    if (getModelID == 0)
                    {
                        var model = new ContidonsParameterFirstViewModel();
                        GrammerModelViewModel._changeEditer(model, this);
                    }
                    else if (getModelID == 1)
                    {
                        var model = new EventsParameterFirstViewModel();
                        GrammerModelViewModel._changeEditer(model, this);
                    }
                    else if (getModelID == 2)
                    {
                        var model = new ObjectivesParameterFirstViewModel();
                        GrammerModelViewModel._changeEditer(model, this);
                    }
                }
                else if(Type == "2")
                {
                    if (getModelID == 0)
                    {
                        var model = new ContidonsParameterFirstViewModel();
                        GrammerModelViewModel._changeEditer(model, this);
                    }
                    else if (getModelID == 1)
                    {
                        var model = new EventsParameterViewModel();
                        GrammerModelViewModel._changeEditer(model, this);
                    }
                    else if (getModelID == 2)
                    {
                        var model = new ObjectivesParameterViewModel();
                        GrammerModelViewModel._changeEditer(model, this);
                    }
                }
                else if(Type == "3") 
                {
                    if (getModelID == 0)
                    {
                        var model = new ContidonsChirldClassViewModel();
                        GrammerModelViewModel._changeEditer(model, this);
                    }
                    else if (getModelID == 1)
                    {
                        var model = new EventsChirldClassViewModel();
                        GrammerModelViewModel._changeEditer(model, this);
                    }
                    else if (getModelID == 2)
                    {
                        var model = new ObjectivesChirldClassViewModel();
                        GrammerModelViewModel._changeEditer(model, this);
                    }
                }
            }

            #endregion
        }

        #region 接口与抽象类

        public interface IEditerModel
        {
            public string Type { get; set; }
        }

        public abstract partial class EditerModelAbstract : ObservableObject, IEditerModel
        {
            [ObservableProperty]
            private string _Type = "0";
        }

        #endregion

        #region 具体实现类

        public partial class ContidonsMainClassViewModel : EditerModelAbstract
        {
            [ObservableProperty]
            private string _Type = "Contidons_MainClass";

            [ObservableProperty]
            private string _CmdName = string.Empty;

            [ObservableProperty]
            private bool _IsUseCondition = false;

            [ObservableProperty]
            private ObservableCollection<SaveHelpInfoModel> _HelpInfoMessages = new ObservableCollection<SaveHelpInfoModel>();

            [ObservableProperty]
            private string _HelpInfoMessage = string.Empty;

            [RelayCommand()]
            private async Task SaveThis(Grid grid)
            {

            }

            [RelayCommand()]
            private async Task AddHelpInfoMessage(DataGrid grid)
            {
                if (string.IsNullOrWhiteSpace(HelpInfoMessage))
                {
                    return;
                }

                HelpInfoMessages.Add(new SaveHelpInfoModel { HelpInfo = HelpInfoMessage });

            }

            [RelayCommand()]
            private async Task DeleteHelpInfoMessage(DataGrid grid)
            {
                if(grid.SelectedItems.Count==0|| grid.SelectedItems.Count > 1)
                {
                    return;
                }

                var getSelect = grid.SelectedItems[0] as SaveHelpInfoModel;

                if (getSelect == null)
                {
                    return;
                }

                HelpInfoMessages.Remove(getSelect);

            }

            public class SaveHelpInfoModel
            {
                public string HelpInfo { get; set; }
            }

        }

        public partial class ContidonsParameterFirstViewModel : EditerModelAbstract
        {
            [ObservableProperty]
            private string _Type = "Contidons_ParameterFirst";

            [ObservableProperty]
            private string _ParameterSpiltChar = string.Empty;

            [ObservableProperty]
            private string _ParameterSpiltStep = string.Empty;

            [ObservableProperty]
            private ObservableCollection<SaveParameterHelpInfo> _ParameterHelpInfoMessages = new ObservableCollection<SaveParameterHelpInfo>();
            
            [ObservableProperty]
            private string _ParameterHelpInfoMessage = string.Empty;
            
            [ObservableProperty]
            private ObservableCollection<SaveNapeHelpInfo> _NapeHelpInfoMessages = new ObservableCollection<SaveNapeHelpInfo>();
            
            [ObservableProperty]
            private string _NapeHelpInfoMessage = string.Empty;
            
            [ObservableProperty]
            private ObservableCollection<string> _NapeNums = new ObservableCollection<string>();
            
            [ObservableProperty]
            private string _NapeSelectNum = string.Empty;
            
            [ObservableProperty]
            private int _NeedCardSelectIndex = -1;

            [RelayCommand()]
            private async Task ParameterSpiltStepChanged(TextBox textBox)
            {
                NapeNums.Clear();
                if (!Regex.IsMatch(ParameterSpiltStep, @"^[0-9]|-[1]*$"))
                {
                    ParameterSpiltStep = "0";
                    return;
                }

                try
                {
                    var getInt = Convert.ToInt32(ParameterSpiltStep);

                    if (getInt > 10)
                    {
                        ParameterSpiltStep = "10";

                        return;
                    }
                    else if(getInt < 0)
                    {
                        ParameterSpiltStep = "-1";

                        return;
                    }

                    for (int i = 1; i <= getInt; i++)
                    {
                        NapeNums.Add($"第 {i} 项");
                    }
                }
                catch
                {

                }
                
            }

            [RelayCommand()]
            private async Task AddNapeHelpInfoMessage(DataGrid grid)
            {
                if (string.IsNullOrWhiteSpace(NapeHelpInfoMessage)||string.IsNullOrWhiteSpace(NapeSelectNum))
                {
                    return;
                }

                NapeHelpInfoMessages.Add(new SaveNapeHelpInfo
                {
                    NapeHelpInfoMessage = NapeHelpInfoMessage,
                    NapeHelpInfoMessageNum = NapeSelectNum
                });
            }

            [RelayCommand()]
            private async Task DeleteNapeHelpInfoMessage(DataGrid grid)
            {
                if (grid.SelectedItems.Count == 0 || grid.SelectedItems.Count > 1)
                {
                    return;
                }

                var getSelect = grid.SelectedItems[0] as SaveNapeHelpInfo;

                if (getSelect == null)
                {
                    return;
                }

                NapeHelpInfoMessages.Remove(getSelect);

            }

            [RelayCommand()]
            private async Task AddParameterHelpInfoMessage(DataGrid grid)
            {
                if (string.IsNullOrWhiteSpace(ParameterHelpInfoMessage))
                {
                    return;
                }

                ParameterHelpInfoMessages.Add(new SaveParameterHelpInfo
                {
                    ParameterHelpInfo = ParameterHelpInfoMessage
                });
            }

            [RelayCommand()]
            private async Task DeleteParameterHelpInfoMessage(DataGrid grid)
            {
                if (grid.SelectedItems.Count == 0 || grid.SelectedItems.Count > 1)
                {
                    return;
                }

                var getSelect = grid.SelectedItems[0] as SaveParameterHelpInfo;

                if (getSelect == null)
                {
                    return;
                }

                ParameterHelpInfoMessages.Remove(getSelect);
            }

            [RelayCommand()]
            private async Task SaveThis(Grid grid)
            {

            }

            public class SaveParameterHelpInfo
            {
                public string ParameterHelpInfo { get; set; } = string.Empty;
            }

            public class SaveNapeHelpInfo
            {
                public string NapeHelpInfoMessageNum { get; set; }

                public string NapeHelpInfoMessage { get; set; }
            }
        }

        public partial class ContidonsChirldClassViewModel : EditerModelAbstract
        {
            [ObservableProperty]
            private string _Type = "Contidons_ChirldClass";

            [ObservableProperty]
            private string _ChirdCmdName = string.Empty;

            [ObservableProperty]
            private ObservableCollection<SaveHelpInfoModel> _ChirdCmdHelpInfoMessages = new ObservableCollection<SaveHelpInfoModel>();

            [ObservableProperty]
            private string _ChirdCmdHelpInfoMessage = string.Empty;

            [RelayCommand()]
            private async Task SaveThis(Grid grid)
            {

            }

            [RelayCommand()]
            private async Task AddChirdCmdHelpInfoMessage(DataGrid grid)
            {
                if (string.IsNullOrWhiteSpace(ChirdCmdHelpInfoMessage))
                {
                    return;
                }

                ChirdCmdHelpInfoMessages.Add(new SaveHelpInfoModel { ChirdCmdHelpInfo = ChirdCmdHelpInfoMessage });
            }

            [RelayCommand()]
            private async Task DeleteChirdCmdHelpInfoMessage(DataGrid grid)
            {
                if (grid.SelectedItems.Count == 0 || grid.SelectedItems.Count > 1)
                {
                    return;
                }

                var getSelect = grid.SelectedItems[0] as SaveHelpInfoModel;

                if (getSelect == null)
                {
                    return;
                }

                ChirdCmdHelpInfoMessages.Remove(getSelect);
            }

            public class SaveHelpInfoModel
            {
                public string ChirdCmdHelpInfo { get; set; } = string.Empty;
            }
        }

        public partial class EventsMainClassViewModel : EditerModelAbstract
        {
            [ObservableProperty]
            private string _Type = "Events_MainClass";

            [ObservableProperty]
            private string _CmdName = string.Empty;

            [ObservableProperty]
            private ObservableCollection<SaveHelpInfoModel> _HelpInfoMessages = new ObservableCollection<SaveHelpInfoModel>();

            [ObservableProperty]
            private string _HelpInfoMessage = string.Empty;

            [RelayCommand()]
            private async Task SaveThis(Grid grid)
            {

            }

            [RelayCommand()]
            private async Task AddHelpInfoMessage(DataGrid grid)
            {
                if (string.IsNullOrWhiteSpace(HelpInfoMessage))
                {
                    return;
                }

                HelpInfoMessages.Add(new SaveHelpInfoModel { HelpInfo = HelpInfoMessage });

            }

            [RelayCommand()]
            private async Task DeleteHelpInfoMessage(DataGrid grid)
            {
                if (grid.SelectedItems.Count == 0 || grid.SelectedItems.Count > 1)
                {
                    return;
                }

                var getSelect = grid.SelectedItems[0] as SaveHelpInfoModel;

                if (getSelect == null)
                {
                    return;
                }

                HelpInfoMessages.Remove(getSelect);

            }

            public class SaveHelpInfoModel
            {
                public string HelpInfo { get; set; } = string.Empty;
            }
        }

        public partial class EventsParameterFirstViewModel : EditerModelAbstract
        {
            [ObservableProperty]
            private string _Type = "Events_ParameterFirst";

            [ObservableProperty]
            private string _ParameterSpiltChar = string.Empty;

            [ObservableProperty]
            private bool _ParameterOnlyOne = false;

            [ObservableProperty]
            private string _ParameterSpiltNum = string.Empty;

            [ObservableProperty]
            private ObservableCollection<SaveParameterHelpInfo> _ParameterHelpInfoMessages = new ObservableCollection<SaveParameterHelpInfo>();

            [ObservableProperty]
            private string _ParameterHelpInfoMessage = string.Empty;

            [ObservableProperty]
            private ObservableCollection<SaveNapeHelpInfo> _NapeHelpInfoMessages = new ObservableCollection<SaveNapeHelpInfo>();

            [ObservableProperty]
            private string _NapeHelpInfoMessage = string.Empty;

            [ObservableProperty]
            private ObservableCollection<string> _NapeNums = new ObservableCollection<string>();

            [ObservableProperty]
            private string _NapeNumsSelectItem = string.Empty;

            [ObservableProperty]
            private int _NapeNumsSelectNow = -1;

            [ObservableProperty]
            private ObservableCollection<string> _ParameterNapeNeedTypes = new ObservableCollection<string>();

            [ObservableProperty]
            private string _ParameterNapeNeedTypeSelectItem = string.Empty;

            [ObservableProperty]
            private string _ParameterNapeNeedType = string.Empty;

            [RelayCommand()]
            private async Task ParameterSpiltStepChanged(TextBox textBox)
            {
                NapeNums.Clear();
                if (!Regex.IsMatch(ParameterSpiltNum, @"^[0-9]|-[1]*$"))
                {
                    ParameterSpiltNum = "0";
                    return;
                }

                try
                {
                    var getInt = Convert.ToInt32(ParameterSpiltNum);

                    if (getInt > 10)
                    {
                        ParameterSpiltNum = "10";

                        return;
                    }

                    for (int i = 1; i <= getInt; i++)
                    {
                        NapeNums.Add($"第 {i} 项");
                    }
                }
                catch
                {

                }

            }

            [RelayCommand()]
            private async Task AddParameterNapeNeedType(ComboBox cbox)
            {
                if (string.IsNullOrWhiteSpace(ParameterNapeNeedType))
                {
                    return;
                }

                ParameterNapeNeedTypes.Add(ParameterNapeNeedType);
            }

            [RelayCommand()]
            private async Task DeleteParameterNapeNeedType(ComboBox cbox)
            {
                if (string.IsNullOrWhiteSpace(ParameterNapeNeedTypeSelectItem))
                {
                    return;
                }

                ParameterNapeNeedTypes.Remove(ParameterNapeNeedTypeSelectItem);
            }

            [RelayCommand()]
            private async Task AddParameterHelpInfo(DataGrid grid)
            {
                if (string.IsNullOrWhiteSpace(ParameterHelpInfoMessage))
                {
                    return;
                }

                ParameterHelpInfoMessages.Add(new SaveParameterHelpInfo
                {
                    ParameterHelpInfo = ParameterHelpInfoMessage
                });
            }

            [RelayCommand()]
            private async Task DeleteParameterHelpInfo(DataGrid grid)
            {
                if (grid.SelectedItems.Count == 0 || grid.SelectedItems.Count > 1)
                {
                    return;
                }

                var getSelect = grid.SelectedItems[0] as SaveParameterHelpInfo;

                if (getSelect == null)
                {
                    return;
                }

                ParameterHelpInfoMessages.Remove(getSelect);
            }

            [RelayCommand()]
            private async Task AddNapeHelpInfoMessage(DataGrid grid)
            {
                if (string.IsNullOrWhiteSpace(NapeHelpInfoMessage) || string.IsNullOrWhiteSpace(NapeNumsSelectItem))
                {
                    return;
                }

                NapeHelpInfoMessages.Add(new SaveNapeHelpInfo
                {
                    NapeHelpInfoMessageSave = NapeHelpInfoMessage,
                    NapeHelpInfoMessageNum = NapeNumsSelectItem
                });
            }

            [RelayCommand()]
            private async Task DeleteNapeHelpInfoMessage(DataGrid grid)
            {
                if (grid.SelectedItems.Count == 0 || grid.SelectedItems.Count > 1)
                {
                    return;
                }

                var getSelect = grid.SelectedItems[0] as SaveNapeHelpInfo;

                if (getSelect == null)
                {
                    return;
                }

                NapeHelpInfoMessages.Remove(getSelect);
            }

            [RelayCommand()]
            private async Task SaveThis(Grid grid)
            {

            }

            public class SaveParameterHelpInfo
            {
                public string ParameterHelpInfo { get; set; } = string.Empty;
            }

            public class SaveNapeHelpInfo
            {
                public string NapeHelpInfoMessageNum { get; set; }

                public string NapeHelpInfoMessageSave { get; set; }
            }
        }

        public partial class EventsParameterViewModel : EditerModelAbstract
        {
            [ObservableProperty]
            private string _Type = "Events_Parameter";

            [ObservableProperty]
            private string _ParameterSpiltChar = string.Empty;

            [ObservableProperty]
            private string _ParameterSpiltStep = string.Empty;

            [ObservableProperty]
            private ObservableCollection<SaveParameterHelpInfo> _ParameterHelpInfoMessages = new ObservableCollection<SaveParameterHelpInfo>();

            [ObservableProperty]
            private string _ParameterHelpInfoMessage = string.Empty;

            [ObservableProperty]
            private ObservableCollection<SaveNapeHelpInfo> _NapeHelpInfoMessages = new ObservableCollection<SaveNapeHelpInfo>();

            [ObservableProperty]
            private string _NapeHelpInfoMessage = string.Empty;

            [ObservableProperty]
            private ObservableCollection<string> _NapeNums = new ObservableCollection<string>();

            [ObservableProperty]
            private string _NapeSelectNum = string.Empty;

            [ObservableProperty]
            private int _NeedCardSelectIndex = -1;

            [RelayCommand()]
            private async Task ParameterSpiltStepChanged(TextBox textBox)
            {
                NapeNums.Clear();
                if (!Regex.IsMatch(ParameterSpiltStep, @"^[0-9]|-[1]*$"))
                {
                    ParameterSpiltStep = "0";
                    return;
                }

                try
                {
                    var getInt = Convert.ToInt32(ParameterSpiltStep);

                    if (getInt > 10)
                    {
                        ParameterSpiltStep = "10";

                        return;
                    }

                    for (int i = 1; i <= getInt; i++)
                    {
                        NapeNums.Add($"第 {i} 项");
                    }
                }
                catch
                {

                }

            }

            [RelayCommand()]
            private async Task AddNapeHelpInfoMessage(DataGrid grid)
            {
                if (string.IsNullOrWhiteSpace(NapeHelpInfoMessage) || string.IsNullOrWhiteSpace(NapeSelectNum))
                {
                    return;
                }

                NapeHelpInfoMessages.Add(new SaveNapeHelpInfo
                {
                    NapeHelpInfoMessage = NapeHelpInfoMessage,
                    NapeHelpInfoMessageNum = NapeSelectNum
                });
            }

            [RelayCommand()]
            private async Task DeleteNapeHelpInfoMessage(DataGrid grid)
            {
                if (grid.SelectedItems.Count == 0 || grid.SelectedItems.Count > 1)
                {
                    return;
                }

                var getSelect = grid.SelectedItems[0] as SaveNapeHelpInfo;

                if (getSelect == null)
                {
                    return;
                }

                NapeHelpInfoMessages.Remove(getSelect);

            }

            [RelayCommand()]
            private async Task AddParameterHelpInfoMessage(DataGrid grid)
            {
                if (string.IsNullOrWhiteSpace(ParameterHelpInfoMessage))
                {
                    return;
                }

                ParameterHelpInfoMessages.Add(new SaveParameterHelpInfo
                {
                    ParameterHelpInfo = ParameterHelpInfoMessage
                });
            }

            [RelayCommand()]
            private async Task DeleteParameterHelpInfoMessage(DataGrid grid)
            {
                if (grid.SelectedItems.Count == 0 || grid.SelectedItems.Count > 1)
                {
                    return;
                }

                var getSelect = grid.SelectedItems[0] as SaveParameterHelpInfo;

                if (getSelect == null)
                {
                    return;
                }

                ParameterHelpInfoMessages.Remove(getSelect);
            }

            [RelayCommand()]
            private async Task SaveThis(Grid grid)
            {

            }

            public class SaveParameterHelpInfo
            {
                public string ParameterHelpInfo { get; set; } = string.Empty;
            }

            public class SaveNapeHelpInfo
            {
                public string NapeHelpInfoMessageNum { get; set; }

                public string NapeHelpInfoMessage { get; set; }
            }
        }

        public partial class EventsChirldClassViewModel : EditerModelAbstract
        {
            [ObservableProperty]
            private string _Type = "Events_ChirldClass";

            [ObservableProperty]
            private string _ChirdCmdName = string.Empty;

            [ObservableProperty]
            private ObservableCollection<SaveHelpInfoModel> _ChirdCmdHelpInfoMessages = new ObservableCollection<SaveHelpInfoModel>();

            [ObservableProperty]
            private string _ChirdCmdHelpInfoMessage = string.Empty;

            [RelayCommand()]
            private async Task SaveThis(Grid grid)
            {

            }

            [RelayCommand()]
            private async Task AddChirdCmdHelpInfoMessage(DataGrid grid)
            {
                if (string.IsNullOrWhiteSpace(ChirdCmdHelpInfoMessage))
                {
                    return;
                }

                ChirdCmdHelpInfoMessages.Add(new SaveHelpInfoModel { ChirdCmdHelpInfo = ChirdCmdHelpInfoMessage });
            }

            [RelayCommand()]
            private async Task DeleteChirdCmdHelpInfoMessage(DataGrid grid)
            {
                if (grid.SelectedItems.Count == 0 || grid.SelectedItems.Count > 1)
                {
                    return;
                }

                var getSelect = grid.SelectedItems[0] as SaveHelpInfoModel;

                if (getSelect == null)
                {
                    return;
                }

                ChirdCmdHelpInfoMessages.Remove(getSelect);
            }

            public class SaveHelpInfoModel
            {
                public string ChirdCmdHelpInfo { get; set; } = string.Empty;
            }
        }

        public partial class ObjectivesMainClassViewModel : EditerModelAbstract
        {
            [ObservableProperty]
            private string _Type = "Objectives_MainClass";

            [ObservableProperty]
            private string _CmdName = string.Empty;

            [ObservableProperty]
            private ObservableCollection<SaveHelpInfoModel> _HelpInfoMessages = new ObservableCollection<SaveHelpInfoModel>();

            [ObservableProperty]
            private string _HelpInfoMessage = string.Empty;

            [RelayCommand()]
            private async Task SaveThis(Grid grid)
            {

            }

            [RelayCommand()]
            private async Task AddHelpInfoMessage(DataGrid grid)
            {
                if (string.IsNullOrWhiteSpace(HelpInfoMessage))
                {
                    return;
                }

                HelpInfoMessages.Add(new SaveHelpInfoModel { HelpInfo = HelpInfoMessage });

            }

            [RelayCommand()]
            private async Task DeleteHelpInfoMessage(DataGrid grid)
            {
                if (grid.SelectedItems.Count == 0 || grid.SelectedItems.Count > 1)
                {
                    return;
                }

                var getSelect = grid.SelectedItems[0] as SaveHelpInfoModel;

                if (getSelect == null)
                {
                    return;
                }

                HelpInfoMessages.Remove(getSelect);

            }

            public class SaveHelpInfoModel
            {
                public string HelpInfo { get; set; } = string.Empty;
            }
        }

        public partial class ObjectivesParameterFirstViewModel : EditerModelAbstract
        {
            [ObservableProperty]
            private string _Type = "Objectives_ParameterFirst";

            [ObservableProperty]
            private ObservableCollection<string> _ChirdTags = new ObservableCollection<string>();

            [ObservableProperty]
            private string _ParameterSpiltChar = string.Empty;

            [ObservableProperty]
            private string _ParameterSpiltNum = string.Empty;

            [ObservableProperty]
            private ObservableCollection<SaveParameterHelpInfo> _ParameterHelpInfoMessages = new ObservableCollection<SaveParameterHelpInfo>();

            [ObservableProperty]
            private string _ParameterHelpInfoMessage = string.Empty;

            [ObservableProperty]
            private ObservableCollection<SaveNapeHelpInfo> _NapeHelpInfoMessages = new ObservableCollection<SaveNapeHelpInfo>();

            [ObservableProperty]
            private string _NapeHelpInfoMessage = string.Empty;

            [ObservableProperty]
            private ObservableCollection<string> _NapeNums = new ObservableCollection<string>();

            [ObservableProperty]
            private string _NapeNumsSelectItem = string.Empty;

            [ObservableProperty]
            private int _NapeNumsSelectNow = -1;

            [ObservableProperty]
            private ObservableCollection<string> _ParameterNapeNeedTypes = new ObservableCollection<string>();

            [ObservableProperty]
            private string _ParameterNapeNeedTypeSelectItem = string.Empty;

            [ObservableProperty]
            private string _ParameterNapeNeedType = string.Empty;

            [RelayCommand()]
            private async Task ParameterSpiltStepChanged(TextBox textBox)
            {
                NapeNums.Clear();
                if (!Regex.IsMatch(ParameterSpiltNum, @"^[0-9]|-[1]*$"))
                {
                    ParameterSpiltNum = "0";
                    return;
                }

                try
                {
                    var getInt = Convert.ToInt32(ParameterSpiltNum);

                    if (getInt > 10)
                    {
                        ParameterSpiltNum = "10";

                        return;
                    }

                    for (int i = 1; i <= getInt; i++)
                    {
                        NapeNums.Add($"第 {i} 项");
                    }
                }
                catch
                {

                }

            }

            [RelayCommand()]
            private async Task AddParameterNapeNeedType(ComboBox cbox)
            {
                if (string.IsNullOrWhiteSpace(ParameterNapeNeedType))
                {
                    return;
                }

                ParameterNapeNeedTypes.Add(ParameterNapeNeedType);
            }

            [RelayCommand()]
            private async Task DeleteParameterNapeNeedType(ComboBox cbox)
            {
                if (string.IsNullOrWhiteSpace(ParameterNapeNeedTypeSelectItem))
                {
                    return;
                }

                ParameterNapeNeedTypes.Remove(ParameterNapeNeedTypeSelectItem);
            }

            [RelayCommand()]
            private async Task AddParameterHelpInfo(DataGrid grid)
            {
                if (string.IsNullOrWhiteSpace(ParameterHelpInfoMessage))
                {
                    return;
                }

                ParameterHelpInfoMessages.Add(new SaveParameterHelpInfo
                {
                    ParameterHelpInfo = ParameterHelpInfoMessage
                });
            }

            [RelayCommand()]
            private async Task DeleteParameterHelpInfo(DataGrid grid)
            {
                if (grid.SelectedItems.Count == 0 || grid.SelectedItems.Count > 1)
                {
                    return;
                }

                var getSelect = grid.SelectedItems[0] as SaveParameterHelpInfo;

                if (getSelect == null)
                {
                    return;
                }

                ParameterHelpInfoMessages.Remove(getSelect);
            }

            [RelayCommand()]
            private async Task AddNapeHelpInfoMessage(DataGrid grid)
            {
                if (string.IsNullOrWhiteSpace(NapeHelpInfoMessage) || string.IsNullOrWhiteSpace(NapeNumsSelectItem))
                {
                    return;
                }

                NapeHelpInfoMessages.Add(new SaveNapeHelpInfo
                {
                    NapeHelpInfoMessageSave = NapeHelpInfoMessage,
                    NapeHelpInfoMessageNum = NapeNumsSelectItem
                });
            }

            [RelayCommand()]
            private async Task DeleteNapeHelpInfoMessage(DataGrid grid)
            {
                if (grid.SelectedItems.Count == 0 || grid.SelectedItems.Count > 1)
                {
                    return;
                }

                var getSelect = grid.SelectedItems[0] as SaveNapeHelpInfo;

                if (getSelect == null)
                {
                    return;
                }

                NapeHelpInfoMessages.Remove(getSelect);
            }

            [RelayCommand()]
            private async Task SaveThis(Grid grid)
            {

            }

            public class SaveParameterHelpInfo
            {
                public string ParameterHelpInfo { get; set; } = string.Empty;
            }

            public class SaveNapeHelpInfo
            {
                public string NapeHelpInfoMessageNum { get; set; }

                public string NapeHelpInfoMessageSave { get; set; }
            }
        }

        public partial class ObjectivesParameterViewModel : EditerModelAbstract
        {
            [ObservableProperty]
            private string _Type = "Objectives_Parameter";

            [ObservableProperty]
            private string _ParameterSpiltChar = string.Empty;

            [ObservableProperty]
            private string _ParameterSpiltStep = string.Empty;

            [ObservableProperty]
            private ObservableCollection<SaveParameterHelpInfo> _ParameterHelpInfoMessages = new ObservableCollection<SaveParameterHelpInfo>();

            [ObservableProperty]
            private string _ParameterHelpInfoMessage = string.Empty;

            [ObservableProperty]
            private ObservableCollection<SaveNapeHelpInfo> _NapeHelpInfoMessages = new ObservableCollection<SaveNapeHelpInfo>();

            [ObservableProperty]
            private string _NapeHelpInfoMessage = string.Empty;

            [ObservableProperty]
            private ObservableCollection<string> _NapeNums = new ObservableCollection<string>();

            [ObservableProperty]
            private string _NapeSelectNum = string.Empty;

            [ObservableProperty]
            private int _NeedCardSelectIndex = -1;

            [RelayCommand()]
            private async Task ParameterSpiltStepChanged(TextBox textBox)
            {
                NapeNums.Clear();
                if (!Regex.IsMatch(ParameterSpiltStep, @"^[0-9]|-[1]*$"))
                {
                    ParameterSpiltStep = "0";
                    return;
                }

                try
                {
                    var getInt = Convert.ToInt32(ParameterSpiltStep);

                    if (getInt > 10)
                    {
                        ParameterSpiltStep = "10";

                        return;
                    }

                    for (int i = 1; i <= getInt; i++)
                    {
                        NapeNums.Add($"第 {i} 项");
                    }
                }
                catch
                {

                }

            }

            [RelayCommand()]
            private async Task AddNapeHelpInfoMessage(DataGrid grid)
            {
                if (string.IsNullOrWhiteSpace(NapeHelpInfoMessage) || string.IsNullOrWhiteSpace(NapeSelectNum))
                {
                    return;
                }

                NapeHelpInfoMessages.Add(new SaveNapeHelpInfo
                {
                    NapeHelpInfoMessage = NapeHelpInfoMessage,
                    NapeHelpInfoMessageNum = NapeSelectNum
                });
            }

            [RelayCommand()]
            private async Task DeleteNapeHelpInfoMessage(DataGrid grid)
            {
                if (grid.SelectedItems.Count == 0 || grid.SelectedItems.Count > 1)
                {
                    return;
                }

                var getSelect = grid.SelectedItems[0] as SaveNapeHelpInfo;

                if (getSelect == null)
                {
                    return;
                }

                NapeHelpInfoMessages.Remove(getSelect);

            }

            [RelayCommand()]
            private async Task AddParameterHelpInfoMessage(DataGrid grid)
            {
                if (string.IsNullOrWhiteSpace(ParameterHelpInfoMessage))
                {
                    return;
                }

                ParameterHelpInfoMessages.Add(new SaveParameterHelpInfo
                {
                    ParameterHelpInfo = ParameterHelpInfoMessage
                });
            }

            [RelayCommand()]
            private async Task DeleteParameterHelpInfoMessage(DataGrid grid)
            {
                if (grid.SelectedItems.Count == 0 || grid.SelectedItems.Count > 1)
                {
                    return;
                }

                var getSelect = grid.SelectedItems[0] as SaveParameterHelpInfo;

                if (getSelect == null)
                {
                    return;
                }

                ParameterHelpInfoMessages.Remove(getSelect);
            }

            [RelayCommand()]
            private async Task SaveThis(Grid grid)
            {

            }

            public class SaveParameterHelpInfo
            {
                public string ParameterHelpInfo { get; set; } = string.Empty;
            }

            public class SaveNapeHelpInfo
            {
                public string NapeHelpInfoMessageNum { get; set; }

                public string NapeHelpInfoMessage { get; set; }
            }
        }

        public partial class ObjectivesChirldClassViewModel : EditerModelAbstract
        {
            [ObservableProperty]
            private string _Type = "Objectives_ChirldClass";

            [ObservableProperty]
            private string _ChirdCmdName = string.Empty;

            [ObservableProperty]
            private ObservableCollection<SaveHelpInfoModel> _ChirdCmdHelpInfoMessages = new ObservableCollection<SaveHelpInfoModel>();

            [ObservableProperty]
            private string _ChirdCmdHelpInfoMessage = string.Empty;

            [RelayCommand()]
            private async Task SaveThis(Grid grid)
            {

            }

            [RelayCommand()]
            private async Task AddChirdCmdHelpInfoMessage(DataGrid grid)
            {
                if (string.IsNullOrWhiteSpace(ChirdCmdHelpInfoMessage))
                {
                    return;
                }

                ChirdCmdHelpInfoMessages.Add(new SaveHelpInfoModel { ChirdCmdHelpInfo = ChirdCmdHelpInfoMessage });
            }

            [RelayCommand()]
            private async Task DeleteChirdCmdHelpInfoMessage(DataGrid grid)
            {
                if (grid.SelectedItems.Count == 0 || grid.SelectedItems.Count > 1)
                {
                    return;
                }

                var getSelect = grid.SelectedItems[0] as SaveHelpInfoModel;

                if (getSelect == null)
                {
                    return;
                }

                ChirdCmdHelpInfoMessages.Remove(getSelect);
            }

            public class SaveHelpInfoModel
            {
                public string ChirdCmdHelpInfo { get; set; } = string.Empty;
            }
        }

        #endregion
    }
}
