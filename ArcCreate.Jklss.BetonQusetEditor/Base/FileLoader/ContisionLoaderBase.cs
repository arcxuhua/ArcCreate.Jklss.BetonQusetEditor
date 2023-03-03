using ArcCreate.Jklss.BetonQusetEditor.ViewModel;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.SocketModel;
using ArcCreate.Jklss.Model.ThumbInfoWindow;
using ArcCreate.Jklss.Model.ThumbModel;
using ArcCreate.Jklss.Model.ThumbModel.CommandModel;
using ArcCreate.Jklss.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static ArcCreate.Jklss.Model.SocketModel.SocketModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ArcCreate.Jklss.BetonQusetEditor.Base.FileLoader
{
    public class ContisionLoaderBase
    {
        public string jsons = string.Empty;

        public Thumb getThumb = null;
               
        public static List<ContisionsCmdModel> savecmdModels = null;

        private TreeView saveTree = null;

        public Dictionary<Thumb, ThumbInfoWindowModel> saveThumbInfoWindowModel = null;

        private MainWindow mainWindow = null;

        private MainWindowModels mainWindowModels = null;

        public ContisionLoaderBase(MainWindow mainWindow, MainWindowModels mainWindowModels)
        {
            this.mainWindow = mainWindow;
            this.mainWindowModels = mainWindowModels;
        }

        /// <summary>
        /// 初始化默认的Json文本
        /// </summary>
        /// <returns></returns>
        public async Task<string> Saver()
        {
            var message = new MessageModel()
            {
                IsLogin = SocketModel.isLogin,
                JsonInfo = JsonInfo.GetConditonModel,
                UserName = SocketModel.userName,
                Message = "",
            };

            var jsonMessage = FileService.SaveToJson(message);

            var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage, 
                SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(),SocketModel.token,true);

            if (getMessage == null || !getMessage.Succese)
            {
                return null;
            }

            var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

            if(getModel.Token != SocketModel.token)
            {
                return null;
            }

            var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

            if(getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.GetConditonModel||!getRealMessage.IsLogin)
            {
                return null;
            }

            try
            {
                return getRealMessage.Message;
            }
            catch
            {
                return null;
            }
            
        }

        /// <summary>
        /// 得到相关实体类
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task<List<ContisionsCmdModel>> Loader()
        {
            if (string.IsNullOrEmpty(jsons))
            {
                MessageBox.Show("您的账户发生了严重的错误，请截图联系管理员！", "错误");
                Environment.Exit(0);
            }

            try
            {
                var models = await Task.Run(() => { 
                    return FileService.JsonToProp<List<ContisionsCmdModel>>(jsons); 
                });

                if (models.Count == 0)
                {
                    return null;
                }
                savecmdModels = models;
                return models;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdModels"></param>
        /// <param name="cmd"></param>
        /// <param name="thumb"></param>
        /// <returns></returns>
        public async Task<ReturnModel> ChangeThumb(List<ContisionsCmdModel> cmdModels, string cmd,Thumb thumb)
        {
            var model = new ReturnModel();

            getThumb = thumb;

            savecmdModels = cmdModels;

            try
            {
                ContisionsCmdModel getModelInfo = null;

                var getRealCMD = TxtSplit(cmd, ": ");

                if (getRealCMD.Count >= 2)
                {
                    getModelInfo = savecmdModels.Find(t => t.MainClass == getRealCMD[getRealCMD.Count - 1]);
                }
                else
                {
                    getModelInfo = savecmdModels.Find(t => t.MainClass == cmd);
                }

                if (getModelInfo == null)
                {
                    model.SetError("该指令还没有对应的模型！");

                    return model;
                }

                (GetControl("Conditions_CBox", thumb) as ComboBox).ToolTip = getModelInfo.MainToolTip;

                var editModel = new List<string>();//参数构造 键为命令

                editModel.Add(getModelInfo.MainClass);

                for (int i = 0; i < getModelInfo.ChildClasses.Count; i++)
                {
                    editModel.Add(getModelInfo.ChildClasses[i].ChildClass);
                }

                var cmdCoBox = GetControl("ConditionsCmdEdit_CBox", thumb) as ComboBox;

                cmdCoBox.Items.Clear();
                foreach (var item in editModel)
                {
                    await Task.Run(() =>
                    {
                        mainWindow.Dispatcher.Invoke(new Action(() => {
                            cmdCoBox.Items.Add(item);
                        }));
                    });
                }
                //添加可编辑命令

                if(getModelInfo.MainClass == "tag"&& mainWindow.IsEnabled)//tag标签的特殊处理
                {
                    var haveInfoMain = mainWindowModels.SaveThumbInfo.ContainsKey(getThumb);

                    if (!haveInfoMain)
                    {
                        mainWindowModels.SaveThumbInfo.Add(getThumb, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>());
                    }

                    var tryOne = mainWindowModels.SaveThumbInfo[getThumb].ContainsKey(cmd);

                    if (!tryOne)
                    {
                        mainWindowModels.SaveThumbInfo[getThumb].Add(cmd, new Dictionary<string, Dictionary<string, Dictionary<string, string>>>());
                    }

                    var tryTwo = mainWindowModels.SaveThumbInfo[getThumb][cmd].ContainsKey(getModelInfo.MainClass);

                    if (!tryTwo)
                    {
                        mainWindowModels.SaveThumbInfo[getThumb][cmd].Add(getModelInfo.MainClass, new Dictionary<string, Dictionary<string, string>>());
                    }

                    var tryThree = mainWindowModels.SaveThumbInfo[getThumb][cmd][getModelInfo.MainClass].ContainsKey("第 1 条参数");

                    if (!tryThree)
                    {
                        mainWindowModels.SaveThumbInfo[getThumb][cmd][getModelInfo.MainClass].Add("第 1 条参数", new Dictionary<string, string>());
                    }

                    var tryFour = mainWindowModels.SaveThumbInfo[getThumb][cmd][getModelInfo.MainClass]["第 1 条参数"].ContainsKey("第 1 项");

                    if (!tryFour)
                    {
                        mainWindowModels.SaveThumbInfo[getThumb][cmd][getModelInfo.MainClass]["第 1 条参数"].Add("第 1 项",null);
                    }

                    mainWindowModels.SaveThumbInfo[getThumb][cmd][getModelInfo.MainClass]["第 1 条参数"]["第 1 项"] = (GetControl("ConditionsConfig_TBox", thumb) as TextBox).Text;
                }

                cmdCoBox.SelectionChanged -= CmdCoBox_SelectionChanged;
                cmdCoBox.SelectionChanged += CmdCoBox_SelectionChanged;

                (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = false;
                (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = false;

                (GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectionChanged -= CceCoBox_SelectionChanged;
                (GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox).SelectionChanged -= ContisionLoaderBase_SelectionChanged;


                if(getModelInfo.MainClass == "tag")
                {
                    model.SetSuccese("","0");

                    return model;
                }
                else
                {
                    model.SetSuccese();

                    return model;
                }
            }
            catch
            {
                model.SetError("未知的命令");

                return model;
            }
            
        }

        private void CmdCoBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var cceCoBox = GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox;

                string ccCoBox = string.Empty;

                if ((sender as ComboBox).Items.Count > 0)
                {
                    var box = (sender as ComboBox).SelectedItem as ComboBoxItem;
                    if (box != null)
                    {
                        ccCoBox = box.Content.ToString();
                    }
                    else
                    {
                        ccCoBox = (sender as ComboBox).SelectedItem.ToString();
                    }
                }

                var cCoBox = ((GetControl("Conditions_CBox", getThumb) as ComboBox).SelectedItem as ComboBoxItem).Content.ToString();

                ContisionsCmdModel getModelInfo = null;

                var getRealCMD = TxtSplit(cCoBox, ": ");

                if (getRealCMD.Count >= 2)
                {
                    getModelInfo = savecmdModels.Find(t => t.MainClass == getRealCMD[getRealCMD.Count - 1]);
                }
                else
                {
                    getModelInfo = savecmdModels.Find(t => t.MainClass == cCoBox);
                }

                cceCoBox.Items.Clear();

                var getSelectIndex = (sender as ComboBox).SelectedIndex;

                if (getSelectIndex == -1)
                {
                    (sender as ComboBox).ToolTip = "请选择命令";
                }
                else
                {
                    try
                    {
                        (sender as ComboBox).ToolTip = getModelInfo.CmdToolTip[getSelectIndex];
                    }
                    catch
                    {
                        (sender as ComboBox).ToolTip = "此命令不做解释";
                    }
                }

                if (ccCoBox == getModelInfo.MainClass)//判断其是否为主命令
                {
                    if (getModelInfo.TextNum != 0)
                    {
                        var num = 1+ getModelInfo.TextSplitWords.Count;

                        for (int i = 0; i < num; i++)
                        {

                            cceCoBox.Items.Add($"第 {i + 1} 条参数");
                        }
                    }
                    else
                    {
                        cceCoBox.Items.Add($"此命令无参数");
                    }
                }
                else
                {
                    foreach (var item in getModelInfo.ChildClasses)
                    {
                        if(ccCoBox == item.ChildClass)//判断其是否为子命令
                        {
                            if (item.ChildTextNum != 0)
                            {
                                int num = 1 + item.ChildTextSplitWords.Count;

                                for (int i = 0; i < num; i++)
                                {
                                    cceCoBox.Items.Add($"第 {i + 1} 条参数");
                                }
                            }
                            else
                            {
                                cceCoBox.Items.Add($"此命令无参数");
                            }

                        }
                    }
                }

                cceCoBox.SelectionChanged -= CceCoBox_SelectionChanged;
                cceCoBox.SelectionChanged += CceCoBox_SelectionChanged;
            }
            catch
            {
                (GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).Items.Clear();
                (GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox).Items.Clear();

                (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = false;
                (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Hidden;

                (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = true;
                (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Visible;

                (GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectionChanged -= CceCoBox_SelectionChanged;
            }
            
        }

        private void CceCoBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var cCoBox = GetControl("Conditions_CBox", getThumb) as ComboBox;

                var cmdCoBox = GetControl("ConditionsCmdEdit_CBox", getThumb) as ComboBox;

                var self = sender as ComboBox;

                var ccpeCoBox = GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox;

                ContisionsCmdModel getModelInfo = null;

                var getRealCMD = TxtSplit((cCoBox.SelectedItem as ComboBoxItem).Content.ToString(), ": ");
                (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Clear();

                (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = false;
                (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Hidden;

                (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = true;
                (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Visible;

                if (getRealCMD.Count >= 2)
                {
                    getModelInfo = savecmdModels.Find(t => t.MainClass == getRealCMD[getRealCMD.Count - 1]);
                }
                else
                {
                    getModelInfo = savecmdModels.Find(t => t.MainClass == (cCoBox.SelectedItem as ComboBoxItem).Content.ToString());
                }

                ccpeCoBox.Items.Clear();

                var getSelectIndex = self.SelectedIndex;

                if (getSelectIndex == -1)
                {
                    self.ToolTip = "请选择参数";
                }
                else
                {
                    try
                    {
                        var cmdIndex = cmdCoBox.SelectedIndex;

                        self.ToolTip = getModelInfo.ParameterToolTip[cmdIndex][getSelectIndex];
                    }
                    catch
                    {
                        self.ToolTip = "此参数不做解释";
                    }
                }

                if (cmdCoBox.SelectedItem == null)
                {
                    return;
                }

                var cmdCoBoxItem = cmdCoBox.SelectedItem as ComboBoxItem;
                var cmdCoBoxItemstr = string.Empty;
                if (cmdCoBoxItem != null)
                {
                    cmdCoBoxItemstr = cmdCoBoxItem.Content.ToString();
                }
                else
                {
                    cmdCoBoxItemstr = cmdCoBox.SelectedItem.ToString();
                }
                if (cmdCoBoxItemstr == getModelInfo.MainClass)//判断其是否为主命令
                {
                    var getIndex = self.SelectedIndex;//找到当前所选中的个数

                    if (getIndex == 0)
                    {
                        if (getModelInfo.TextNum > 0)
                        {
                            (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = false;
                            (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = false;
                            for (int i = 0; i < getModelInfo.TextNum; i++)
                            {
                                ccpeCoBox.Items.Add($"第 {i + 1} 项");
                            }
                        }
                        else if (getModelInfo.TextNum == -1)
                        {
                            (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = true;
                            (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = true;
                            ccpeCoBox.Items.Add($"第 1 项");
                        }
                        else
                        {
                            (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = false;
                            (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = false;

                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = true;
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Visible;

                            (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = false;
                            (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Hidden;

                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("开启");
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("关闭");

                            ccpeCoBox.Items.Add($"第 1 项");
                        }
                    }
                    else
                    {
                        var getncan = getModelInfo.TextSplitWords[getIndex - 1];

                        if (getncan.j > 0)
                        {
                            (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = false;
                            (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = false;
                            for (int i = 0; i < getncan.j; i++)
                            {
                                ccpeCoBox.Items.Add($"第 {i + 1} 项");
                            }
                        }
                        else if (getncan.j == -1)
                        {
                            (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = true;
                            (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = true;
                            ccpeCoBox.Items.Add($"第 1 项");
                        }
                        else
                        {
                            (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = false;
                            (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = false;

                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = true;
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Visible;

                            (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = false;
                            (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Hidden;

                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("开启");
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("关闭");

                            ccpeCoBox.Items.Add($"第 1 项");
                        }
                    }
                }
                else
                {
                    foreach (var item in getModelInfo.ChildClasses)
                    {
                        if (item.ChildClass == (cmdCoBox.SelectedItem as ComboBoxItem).Content.ToString())
                        {
                            var getIndex = self.SelectedIndex;//找到当前所选中的个数

                            if (getIndex == 0)
                            {
                                if (item.ChildTextNum > 0)
                                {
                                    (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = false;
                                    (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = false;
                                    for (int i = 0; i < item.ChildTextNum; i++)
                                    {
                                        ccpeCoBox.Items.Add($"第 {i + 1} 项");
                                    }
                                }
                                else if (item.ChildTextNum == -1)
                                {
                                    (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = true;
                                    (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = true;

                                    ccpeCoBox.Items.Add($"第 1 项");
                                }
                                else
                                {
                                    (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = false;
                                    (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = false;

                                    (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = true;
                                    (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Visible;

                                    (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = false;
                                    (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Hidden;

                                    (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("开启");
                                    (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("关闭");

                                    ccpeCoBox.Items.Add($"第 1 项");
                                }
                            }
                            else
                            {
                                var getncan = item.ChildTextSplitWords[getIndex - 1];

                                if (getncan.j > 0)
                                {
                                    (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = false;
                                    (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = false;
                                    for (int i = 0; i < getncan.j; i++)
                                    {
                                        ccpeCoBox.Items.Add($"第 {i + 1} 项");
                                    }
                                }
                                else if (getncan.j == -1)
                                {
                                    (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = true;
                                    (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = true;
                                    ccpeCoBox.Items.Add($"第 1 项");
                                }
                                else
                                {
                                    (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = false;
                                    (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = false;

                                    (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = true;
                                    (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Visible;

                                    (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = false;
                                    (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Hidden;

                                    (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("开启");
                                    (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("关闭");

                                    ccpeCoBox.Items.Add($"第 1 项");
                                }
                            }
                        }
                    }
                }

                if (mainWindowModels.SaveThumbInfo.ContainsKey(getThumb))
                {
                    var getInfo = mainWindowModels.SaveThumbInfo[getThumb];

                    try
                    {

                        var one = string.Empty;
                        var two = string.Empty;
                        var three = string.Empty;

                        var Conditions_CBoxItem = (GetControl("Conditions_CBox", getThumb) as ComboBox).SelectedItem as ComboBoxItem;
                        var ConditionsCmdEdit_CBoxItem = (GetControl("ConditionsCmdEdit_CBox", getThumb) as ComboBox).SelectedItem as ComboBoxItem;
                        var ConditionsCmdparameterEdit_CBoxItem = (GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectedItem as ComboBoxItem;

                        if (Conditions_CBoxItem != null)
                        {
                            one = Conditions_CBoxItem.Content.ToString();
                        }
                        else
                        {
                            one = (GetControl("Conditions_CBox", getThumb) as ComboBox).SelectedItem.ToString();
                        }

                        if (ConditionsCmdEdit_CBoxItem != null)
                        {
                            two = ConditionsCmdEdit_CBoxItem.Content.ToString();
                        }
                        else
                        {
                            two = (GetControl("ConditionsCmdEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString();
                        }

                        if (ConditionsCmdparameterEdit_CBoxItem != null)
                        {
                            three = ConditionsCmdparameterEdit_CBoxItem.Content.ToString();
                        }
                        else
                        {
                            three = (GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString();
                        }
                        var getNeed = getInfo[one][two][three];

                        foreach (var item in getNeed)
                        {
                            if (!ccpeCoBox.Items.Contains(item.Key))
                            {
                                ccpeCoBox.Items.Add(item.Key);
                            }
                        }
                    }
                    catch
                    {

                    }
                }

                (GetControl("ConditionsAdd_Btn", getThumb) as Button).Click -= ContisionLoaderBase_Click;
                (GetControl("ConditionsAdd_Btn", getThumb) as Button).Click += ContisionLoaderBase_Click;

                (GetControl("ConditionsRemove_Btn", getThumb) as Button).Click -= ContisionLoaderBase_Click1;
                (GetControl("ConditionsRemove_Btn", getThumb) as Button).Click += ContisionLoaderBase_Click1;

                (GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox).SelectionChanged -= ContisionLoaderBase_SelectionChanged;
                (GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox).SelectionChanged += ContisionLoaderBase_SelectionChanged;
            }
            catch
            {
                (GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox).Items.Clear();
                (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = false;
                (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = false;

                (GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox).SelectionChanged -= ContisionLoaderBase_SelectionChanged;
            }
            
        }

        private void ContisionLoaderBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var getConditions_CBox = GetControl("Conditions_CBox", getThumb) as ComboBox;

            var getConditionsCmdEdit_CBox = GetControl("ConditionsCmdEdit_CBox", getThumb) as ComboBox;

            var getConditionsCmdparameterEdit_CBox = GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox;

            var getConditionsCmdProjectEdit_CBox = sender as ComboBox;

            var getConditions_ComboBox = GetControl("Conditions_ComboBox", getThumb) as ComboBox;

            var getConditions_TBox = GetControl("Conditions_TBox", getThumb) as TextBox;

            ContisionsCmdModel getModelInfo = null;

            var getRealCMD = TxtSplit((getConditions_CBox.SelectedItem as ComboBoxItem).Content.ToString(), ": ");

            if (getRealCMD.Count >= 2)
            {
                getModelInfo = savecmdModels.Find(t => t.MainClass == getRealCMD[getRealCMD.Count - 1]);
            }
            else
            {
                getModelInfo = savecmdModels.Find(t => t.MainClass == (getConditions_CBox.SelectedItem as ComboBoxItem).Content.ToString());
            }

            var getSelectIndex = getConditionsCmdProjectEdit_CBox.SelectedIndex;

            if (getSelectIndex == -1)
            {
                getConditionsCmdProjectEdit_CBox.ToolTip = "请选择项";
            }
            else
            {
                try
                {
                    getConditionsCmdProjectEdit_CBox.ToolTip = getModelInfo.TermToolTip[getConditionsCmdEdit_CBox.SelectedIndex]
                        [getConditionsCmdparameterEdit_CBox.SelectedIndex][getSelectIndex];
                }
                catch
                {
                    getConditionsCmdProjectEdit_CBox.ToolTip = "此项不做解释";
                }
            }

            if (!mainWindowModels.SaveThumbInfo.ContainsKey(getThumb))
            {
                return;
            }

            var getInfo = mainWindowModels.SaveThumbInfo[getThumb];

            string one = string.Empty, two = string.Empty, three = string.Empty, four = string.Empty;

            if (getConditions_CBox.SelectedItem != null)
            {
                var getConditions_CBoxItem = getConditions_CBox.SelectedItem as ComboBoxItem;

                if (getConditions_CBoxItem != null)
                {
                    one = getConditions_CBoxItem.Content.ToString();
                }
                else
                {
                    one = getConditions_CBox.SelectedItem.ToString();
                }

            }
            else
            {
                return;
            }

            if (getConditionsCmdEdit_CBox.SelectedItem != null)
            {
                var getConditionsCmdEdit_CBoxItem = getConditionsCmdEdit_CBox.SelectedItem as ComboBoxItem;

                if (getConditionsCmdEdit_CBoxItem != null)
                {
                    two = getConditionsCmdEdit_CBoxItem.Content.ToString();
                }
                else
                {
                    two = getConditionsCmdEdit_CBox.SelectedItem.ToString();
                }


            }
            else
            {
                return;
            }

            if (getConditionsCmdparameterEdit_CBox.SelectedItem != null)
            {
                var getConditionsCmdparameterEdit_CBoxItem = getConditionsCmdparameterEdit_CBox.SelectedItem as ComboBoxItem;

                if (getConditionsCmdparameterEdit_CBoxItem != null)
                {
                    three = getConditionsCmdparameterEdit_CBoxItem.Content.ToString();
                }
                else
                {
                    three = getConditionsCmdparameterEdit_CBox.SelectedItem.ToString();
                }
            }
            else
            {
                return;
            }

            if (getConditionsCmdProjectEdit_CBox.SelectedItem != null)
            {
                var getConditionsCmdProjectEdit_CBoxItem = getConditionsCmdProjectEdit_CBox.SelectedItem as ComboBoxItem;

                if (getConditionsCmdProjectEdit_CBoxItem != null)
                {
                    four = getConditionsCmdProjectEdit_CBoxItem.Content.ToString();
                }
                else
                {
                    four = getConditionsCmdProjectEdit_CBox.SelectedItem.ToString();
                }

            }
            else
            {
                return;
            }

            try
            {
                var velue = getInfo[one][two][three][four];

                if (getConditions_TBox.IsEnabled)
                {
                    getConditions_TBox.Text = velue;
                }
                else
                {
                    getConditions_ComboBox.SelectedItem = velue;
                }
            }
            catch
            {
                var haveOne = getInfo.ContainsKey(one);

                if (!haveOne)
                {
                    var fg = one.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    if (fg.Length == 3)
                    {
                        one = fg[1] + ": " + fg[2];
                    }

                    if (!getInfo.ContainsKey(one))
                    {
                        return;
                    }
                }

                var haveTwo = getInfo[one].ContainsKey(two);

                if (!haveTwo)
                {
                    var fg = two.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    if (fg.Length == 3)
                    {
                        two = fg[1] + ": " + fg[2];
                    }

                    if (!getInfo[one].ContainsKey(two))
                    {
                        return;
                    }
                }

                var haveThree = getInfo[one][two].ContainsKey(three);

                if (!haveThree)
                {
                    var fg = three.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    if (fg.Length == 3)
                    {
                        three = fg[1] + ": " + fg[2];
                    }

                    if (!getInfo[one][two].ContainsKey(three))
                    {
                        return;
                    }
                }

                var haveFour = getInfo[one][two][three].ContainsKey(four);

                if (!haveFour)
                {
                    var fg = four.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    if (fg.Length == 3)
                    {
                        four = fg[1] + ": " + fg[2];
                    }

                    if (!getInfo[one][two][three].ContainsKey(four))
                    {
                        return;
                    }
                }

                var velue = getInfo[one][two][three][four];

                if (getConditions_TBox.IsEnabled)
                {
                    getConditions_TBox.Text = velue;
                }
                else
                {
                    getConditions_ComboBox.SelectedItem = velue;
                }
            }

        }

        private async void ContisionLoaderBase_Click1(object sender, System.Windows.RoutedEventArgs e)
        {
            var self = sender as Button;

            var ccpeCoBox = GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox;

            if (self.IsEnabled && ccpeCoBox.SelectedItem != null)
            {

                var ConditionsEdit_CBoxItem = (GetControl("Conditions_CBox", getThumb) as ComboBox).SelectedItem as ComboBoxItem;

                var cmd = string.Empty;

                if (ConditionsEdit_CBoxItem != null)
                {
                    cmd = ConditionsEdit_CBoxItem.Content.ToString();
                }
                else
                {
                    cmd = (GetControl("Conditions_CBox", getThumb) as ComboBox).SelectedItem.ToString();
                }

                var ConditionsCmdEdit_CBoxItem = (GetControl("ConditionsCmdEdit_CBox", getThumb) as ComboBox).SelectedItem as ComboBoxItem;

                var one = string.Empty;

                if (ConditionsCmdEdit_CBoxItem != null)
                {
                    one = ConditionsCmdEdit_CBoxItem.Content.ToString();
                }
                else
                {
                    one = (GetControl("ConditionsCmdEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString();
                }

                var ConditionsCmdparameterEdit_CBoxItem = (GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectedItem as ComboBoxItem;

                var two = string.Empty;

                if (ConditionsCmdparameterEdit_CBoxItem != null)
                {
                    two = ConditionsCmdparameterEdit_CBoxItem.Content.ToString();
                }
                else
                {
                    two = (GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString();
                }

                var ccpeCoBoxItem = ccpeCoBox.SelectedItem as ComboBoxItem;

                var three = string.Empty;

                if (ccpeCoBoxItem != null)
                {
                    three = ccpeCoBoxItem.Content.ToString();
                }
                else
                {
                    three = ccpeCoBox.SelectedItem.ToString();
                }

                var cs = saveThumbInfoWindowModel[getThumb].TreeItems[one][two].Remove(three);

                if (mainWindowModels.SaveThumbInfo.ContainsKey(getThumb))
                {
                    var getInfo = mainWindowModels.SaveThumbInfo[getThumb];

                    try
                    {
                        var getNeed = getInfo[cmd][one][two];

                        if (getNeed.ContainsKey(three))
                        {
                            getNeed.Remove(three);
                        }
                    }
                    catch
                    {

                    }
                }

                await DeleteTreeItem(one, two, three);

                ccpeCoBox.Items.Remove(ccpeCoBox.SelectedItem);
            }
        }
        private async void ContisionLoaderBase_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var self = sender as Button;

            if (self.IsEnabled)
            {
                var ConditionsCmdEdit_CBoxItem = (GetControl("ConditionsCmdEdit_CBox", getThumb) as ComboBox).SelectedItem as ComboBoxItem;
                var ccpeCoBox = GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox;
                var one = string.Empty;

                if (ConditionsCmdEdit_CBoxItem != null)
                {
                    one = ConditionsCmdEdit_CBoxItem.Content.ToString();
                }
                else
                {
                    one = (GetControl("ConditionsCmdEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString();
                }

                var ConditionsCmdparameterEdit_CBoxItem = (GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectedItem as ComboBoxItem;

                var two = string.Empty;

                if (ConditionsCmdparameterEdit_CBoxItem != null)
                {
                    two = ConditionsCmdparameterEdit_CBoxItem.Content.ToString();
                }
                else
                {
                    two = (GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString();
                }

                saveThumbInfoWindowModel[getThumb].TreeItems[one][two].Add($"第 {ccpeCoBox.Items.Count + 1} 项", false);

                await AddTreeItem(one, two, $"第 {ccpeCoBox.Items.Count + 1} 项");

                ccpeCoBox.Items.Add($"第 {ccpeCoBox.Items.Count + 1} 项");
            }
        }

        /// <summary>
        /// 当选中为条件、事件、目标类型的Thumb时更改树形结构
        /// </summary>
        public async Task<ReturnModel> ChangeTheTree(TreeView tiw, List<ContisionsCmdModel> cmdModels, string cmd)
        {
            var result = new ReturnModel();

            saveTree = tiw;

            ContisionsCmdModel getModelInfo = null;

            var getRealCMD = TxtSplit(cmd, ": ");

            if (getRealCMD.Count == 2)
            {
                getModelInfo = cmdModels.Find(t => t.MainClass == getRealCMD[1]);
            }
            else if (getRealCMD.Count == 3)
            {
                getModelInfo = cmdModels.Find(t => t.MainClass == getRealCMD[2]);
            }
            else
            {
                getModelInfo = cmdModels.Find(t => t.MainClass == cmd);
            }

            var windowModel = new ThumbInfoWindowModel
            {
                TreeItems = new Dictionary<string, Dictionary<string, Dictionary<string, bool>>>()
            };

            try
            {

                if (saveThumbInfoWindowModel == null||!saveThumbInfoWindowModel.ContainsKey(getThumb))//当对象存储区为空或者找不到相关Thumb时创建新的存储
                {
                    windowModel = await CreateThunbInfowModel(getModelInfo);

                    if (saveThumbInfoWindowModel == null)
                    {
                        saveThumbInfoWindowModel = new Dictionary<Thumb, ThumbInfoWindowModel>()
                        {
                            {getThumb, windowModel}
                        };
                    }
                    else
                    {
                        saveThumbInfoWindowModel.Add(getThumb, windowModel);
                    }
                }
                //确保对象存储区始终有值

                if (!IsSame(mainWindowModels.SaveThumbInfo, saveThumbInfoWindowModel, getModelInfo.MainClass))//当数据存储区与对象存储区不相同时
                {
                    saveThumbInfoWindowModel[getThumb] = await CreateThunbInfowModel(getModelInfo);
                }
                CheckData();

                windowModel = saveThumbInfoWindowModel[getThumb];

                var nodes = new List<DefinitionNode>();

                foreach (var item in windowModel.TreeItems)
                {
                    var node = new DefinitionNode();

                    node.Name = item.Key;

                    int num = 0;

                    if (node.Children == null)
                    {
                        node.Children = new List<DefinitionNode>();
                    }
                    
                    foreach (var i in item.Value)
                    {
                        node.Children.Add(new DefinitionNode()
                        {
                            Name = i.Key
                        });

                        if (node.Children[num].Children == null)
                        {
                            node.Children[num].Children = new List<DefinitionNode>();
                        }

                        int now = 0;

                        foreach (var j in i.Value)
                        {
                            if (j.Value)
                            {
                                node.Children[num].Children.Add(new DefinitionNode()
                                {
                                    Name = j.Key + " ------ 已保存"
                                });

                                node.Children[num].Children[now].FontColor = "#1f640a";
                            }
                            else
                            {
                                node.Children[num].Children.Add(new DefinitionNode()
                                {
                                    Name = j.Key + " ------ 未保存"
                                });

                                node.Children[num].Children[now].FontColor = "#f6003c";
                            }

                            now++;
                        }
                        num++;
                    }

                    nodes.Add(node);
                }

                tiw.ItemsSource = nodes;

                result.SetSuccese();
                return result;
            }
            catch
            {
                result.SetError("条件树结构生成失败");
                return result;
            }
        }

        protected async Task<ThumbInfoWindowModel> CreateThunbInfowModel(ContisionsCmdModel getModelInfo)
        {
            var windowModel = new ThumbInfoWindowModel
            {
                TreeItems = new Dictionary<string, Dictionary<string, Dictionary<string, bool>>>()
            };

            await Task.Run(() =>
            {
                windowModel.TreeItems.Add(getModelInfo.MainClass, new Dictionary<string, Dictionary<string, bool>>()
                        {
                            {"第 1 条参数",new Dictionary<string, bool>()},
                        });//主命令

                if (getModelInfo.TextNum > 0)
                {
                    for (int i = 0; i < getModelInfo.TextNum; i++)
                    {
                        windowModel.TreeItems[getModelInfo.MainClass]["第 1 条参数"].Add($"第 {i + 1} 项", false);
                    }
                }
                else if (getModelInfo.TextNum == -1)
                {
                    windowModel.TreeItems[getModelInfo.MainClass]["第 1 条参数"].Add($"第 1 项", false);
                }
                else
                {
                    windowModel.TreeItems.Remove(getModelInfo.MainClass);

                    windowModel.TreeItems.Add(getModelInfo.MainClass, new Dictionary<string, Dictionary<string, bool>>()
                        {
                            {"此命令无参数",new Dictionary<string, bool>()},
                        });//主命令

                    windowModel.TreeItems[getModelInfo.MainClass]["此命令无参数"].Add($"第 1 项", false);
                }


                for (int i = 0; i < getModelInfo.TextSplitWords.Count; i++)
                {
                    windowModel.TreeItems[getModelInfo.MainClass].Add($"第 {i + 2} 条参数", new Dictionary<string, bool>());

                    if (getModelInfo.TextSplitWords[i].j > 0)
                    {
                        for (int j = 0; j < getModelInfo.TextSplitWords[i].j; j++)
                        {
                            windowModel.TreeItems[getModelInfo.MainClass][$"第 {i + 2} 条参数"].Add($"第 {j + 1} 项", false);
                        }
                    }
                    else if (getModelInfo.TextSplitWords[i].j == -1)
                    {
                        windowModel.TreeItems[getModelInfo.MainClass][$"第 {i + 2} 条参数"].Add($"第 1 项", false);
                    }
                    else
                    {
                        windowModel.TreeItems.Remove(getModelInfo.MainClass);

                        windowModel.TreeItems.Add(getModelInfo.MainClass, new Dictionary<string, Dictionary<string, bool>>()
                        {
                            {"此命令无参数",new Dictionary<string, bool>()},
                        });//主命令

                        windowModel.TreeItems[getModelInfo.MainClass]["此命令无参数"].Add($"第 1 项", false);
                    }


                }

                for (int i = 0; i < getModelInfo.ChildClasses.Count; i++)
                {

                    windowModel.TreeItems.Add(getModelInfo.ChildClasses[i].ChildClass, new Dictionary<string, Dictionary<string, bool>>()
                        {
                            {"第 1 条参数",new Dictionary<string, bool>()},
                        });//主命令

                    if (getModelInfo.ChildClasses[i].ChildTextNum > 0)
                    {
                        for (int j = 0; j < getModelInfo.ChildClasses[i].ChildTextNum; j++)
                        {
                            windowModel.TreeItems[getModelInfo.ChildClasses[i].ChildClass]["第 1 条参数"].Add($"第 {j + 1} 项", false);
                        }
                    }
                    else if (getModelInfo.ChildClasses[i].ChildTextNum == -1)
                    {
                        windowModel.TreeItems[getModelInfo.ChildClasses[i].ChildClass]["第 1 条参数"].Add($"第 1 项", false);
                    }
                    else
                    {
                        windowModel.TreeItems.Remove(getModelInfo.MainClass);

                        windowModel.TreeItems.Add(getModelInfo.MainClass, new Dictionary<string, Dictionary<string, bool>>()
                        {
                            {"此命令无参数",new Dictionary<string, bool>()},
                        });//主命令

                        windowModel.TreeItems[getModelInfo.MainClass]["此命令无参数"].Add($"第 1 项", false);
                    }


                    for (int j = 0; j < getModelInfo.ChildClasses[i].ChildTextSplitWords.Count; j++)
                    {
                        windowModel.TreeItems[getModelInfo.ChildClasses[i].ChildClass].Add($"第 {j + 2} 条参数", new Dictionary<string, bool>());

                        if (getModelInfo.ChildClasses[i].ChildTextSplitWords[j].j > 0)
                        {
                            for (int n = 0; n < getModelInfo.ChildClasses[i].ChildTextSplitWords[j].j; n++)
                            {
                                windowModel.TreeItems[getModelInfo.MainClass][$"第 {j + 2} 条参数"].Add($"第 {n + 1} 项", false);
                            }
                        }
                        else if (getModelInfo.ChildClasses[i].ChildTextSplitWords[j].j == -1)
                        {
                            windowModel.TreeItems[getModelInfo.MainClass][$"第 {j + 2} 条参数"].Add($"第 1 项", false);
                        }
                        else
                        {
                            windowModel.TreeItems.Remove(getModelInfo.MainClass);

                            windowModel.TreeItems.Add(getModelInfo.MainClass, new Dictionary<string, Dictionary<string, bool>>()
                            {
                                {"此命令无参数",new Dictionary<string, bool>()},
                            });//主命令

                            windowModel.TreeItems[getModelInfo.MainClass]["此命令无参数"].Add($"第 1 项", false);
                        }
                    }
                }
            });

            return windowModel;
        }

        /// <summary>
        /// 添加TreeItem内容
        /// </summary>
        /// <param name="one">命令</param>
        /// <param name="two">参数</param>
        /// <param name="three">项</param>
        /// <returns></returns>
        private async Task<ReturnModel> AddTreeItem(string one, string two, string three)
        {
            var result = new ReturnModel();

            if (saveTree == null)
            {
                result.SetError();

                return result;
            }

            var tree = saveTree;

            await Task.Run(() =>
            {
                var newList = new List<DefinitionNode>();

                foreach (var item in tree.Items)
                {
                    var getTreeItem = item as DefinitionNode;

                    if (getTreeItem.Name == one)
                    {
                        var getTwo = getTreeItem.Children.Where(t => t.Name == two).First();

                        var newChild = new DefinitionNode()
                        {
                            Name = three + " ------ 未保存",
                            FontColor = "#f6003c"
                        };

                        getTwo.Children.Add(newChild);
                    }

                    newList.Add(getTreeItem);
                }

                tree.Dispatcher.Invoke(new Action(() => {
                    tree.ItemsSource = newList;
                }));

            });

            result.SetSuccese();

            return result;

        }

        /// <summary>
        /// 更改TreeItem内容
        /// </summary>
        /// <param name="one">命令</param>
        /// <param name="two">参数</param>
        /// <param name="three">项</param>
        /// <returns></returns>
        private async Task<ReturnModel> DeleteTreeItem(string one, string two, string three)
        {
            var result = new ReturnModel();

            if (saveTree == null)
            {
                result.SetError();

                return result;
            }

            var tree = saveTree;

            await Task.Run(() =>
            {
                var newList = new List<DefinitionNode>();
                foreach (var item in tree.Items)
                {
                    var getTreeItem = item as DefinitionNode;

                    if (getTreeItem.Name == one)
                    {
                        var getTwo = getTreeItem.Children.Where(t => t.Name == two).First();

                        DefinitionNode getDeleteItem = null;

                        foreach (var child in getTwo.Children)
                        {
                            var getReal = child.Name.Split(new string[] { " ------ " }, StringSplitOptions.RemoveEmptyEntries);

                            if (getReal[0] == three)
                            {
                                getDeleteItem = child;

                                break;
                            }
                        }

                        getTwo.Children.Remove(getDeleteItem);
                    }

                    newList.Add(getTreeItem);
                }

                tree.Dispatcher.Invoke(new Action(() => {

                    tree.ItemsSource = newList;

                }));

            });

            result.SetSuccese();

            return result;

        }

        /// <summary>
        /// 从Thumb中获取控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="thumb"></param>
        /// <returns></returns>
        protected static object GetControl(string name, Thumb thumb)
        {
            return thumb.Template.FindName(name, thumb);
        }

        /// <summary>
        /// 文本分割
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="fg"></param>
        /// <returns></returns>
        protected static List<string> TxtSplit(string txt, string fg)
        {
            var getSqlit = txt.Split(new string[] { fg }, StringSplitOptions.RemoveEmptyEntries);

            var newList = new List<string>(getSqlit);

            return newList;
        }

        /// <summary>
        /// 用于两者之间的判断
        /// </summary>
        /// <param name="bd"></param>
        /// <param name="need"></param>
        /// <returns></returns>
        protected bool IsSame(Dictionary<Thumb, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> bd, Dictionary<Thumb, ThumbInfoWindowModel> need,string cmd)
        {
            try
            {
                if (need == null)
                {
                    return false;
                }

                var getter_one = bd[getThumb];

                var getter_two = need[getThumb].TreeItems;

                var getMain_One = string.Empty;

                var getMain_Two = string.Empty;

                foreach (var item in getter_one)
                {
                    getMain_One = item.Key;
                }

                int num = 0;

                foreach (var item in getter_two)
                {
                    if (num == 0)
                    {
                        getMain_Two = item.Key;
                    }
                    num++;
                }

                var fg_one = TxtSplit(getMain_One, ": ");

                if (fg_one.Count >= 2)
                {
                    getMain_One = fg_one[fg_one.Count - 1];
                }

                var fg_two = TxtSplit(getMain_Two, ": ");

                if (fg_two.Count >= 2)
                {
                    getMain_Two = fg_two[fg_two.Count - 1];
                }

                if(cmd!= getMain_Two)
                {
                    return false;
                }

                if (getMain_One == getMain_Two)
                {


                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                if(!bd.ContainsKey(getThumb)&& need.ContainsKey(getThumb))
                {
                    var getter_two = need[getThumb].TreeItems;

                    var getMain_Two = string.Empty;

                    int num = 0;

                    foreach (var item in getter_two)
                    {
                        if (num == 0)
                        {
                            getMain_Two = item.Key;
                        }
                        num++;
                    }

                    var fg_two = TxtSplit(getMain_Two, ": ");

                    if (fg_two.Count >= 2)
                    {
                        getMain_Two = fg_two[fg_two.Count - 1];
                    }

                    if (cmd != getMain_Two)
                    {
                        return false;
                    }
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 保证数据的统一性
        /// </summary>
        protected async void CheckData()
        {
            if (!mainWindowModels.SaveThumbInfo.ContainsKey(getThumb))
            {
                return;
            }

            if (!saveThumbInfoWindowModel.ContainsKey(getThumb))
            {
                return;
            }

            try
            {
                var dataOne = mainWindowModels.SaveThumbInfo[getThumb][mainWindowModels.SaveThumbInfo[getThumb].Keys.First()];

                var dataTwo = saveThumbInfoWindowModel[getThumb];

                if (dataOne.Keys.First() != dataTwo.TreeItems.Keys.First())
                {
                    return;
                }

                var treeViewBase = new TreeViewBase();

                foreach (var item in dataOne)
                {
                    foreach (var i in item.Value)
                    {
                        foreach (var j in i.Value)
                        {
                            await treeViewBase.AddItemToSaves(getThumb, item.Key, i.Key, j.Key, "", "", true, saveThumbInfoWindowModel, mainWindowModels.SaveThumbInfo, false);
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }
    }
}
