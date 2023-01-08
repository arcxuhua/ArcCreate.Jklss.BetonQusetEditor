using ArcCreate.Jklss.BetonQusetEditor.ViewModel;
using ArcCreate.Jklss.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using ArcCreate.Jklss.Model.ThumbInfoWindow;
using System.Security.Policy;

namespace ArcCreate.Jklss.BetonQusetEditor.Base.FileLoader
{
    public class PlayerLoaderBase
    {
        public Thumb getThumb = null;

        public Dictionary<Thumb, ThumbInfoWindowModel> saveThumbInfoWindowModel = null;

        private TreeView saveTree = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdModels"></param>
        /// <param name="cmd"></param>
        /// <param name="thumb"></param>
        /// <returns></returns>
        public async Task<ReturnModel> ChangeThumb(string cmd, Thumb thumb,TreeView tree)
        {
            var model = new ReturnModel();

            saveTree = tree;

            getThumb = thumb;

            try
            {
                var getRealCmd = string.Empty;

                var fg = TxtSplit(cmd,": ");

                if (fg.Count == 3)
                {
                    getRealCmd = fg[2];
                }
                else if(fg.Count == 2)
                {
                    getRealCmd=fg[1];
                }
                else
                {
                    getRealCmd = cmd;
                }

                var getConditionsCmdEdit_CBox = GetControl("ConditionsCmdEdit_CBox", thumb) as ComboBox;//编辑命令

                var getConditionsCmdparameterEdit_CBox = GetControl("ConditionsCmdparameterEdit_CBox", thumb) as ComboBox;//参数

                var getConditionsCmdProjectEdit_CBox = GetControl("ConditionsCmdProjectEdit_CBox", thumb) as ComboBox;//项

                {
                    getConditionsCmdEdit_CBox.Items.Clear();

                    getConditionsCmdEdit_CBox.Items.Add(getRealCmd);

                    getConditionsCmdEdit_CBox.SelectedItem = getRealCmd;
                }

                {
                    getConditionsCmdparameterEdit_CBox.Items.Clear();

                    getConditionsCmdparameterEdit_CBox.Items.Add("第 1 条参数");

                    getConditionsCmdparameterEdit_CBox.SelectedItem = "第 1 条参数";
                }

                {
                    getConditionsCmdProjectEdit_CBox.Items.Clear();

                    var treeBase = new TreeViewBase();

                    if (saveThumbInfoWindowModel == null)
                    {
                        saveThumbInfoWindowModel = new Dictionary<Thumb, ThumbInfoWindowModel>();
                    }

                    if (saveThumbInfoWindowModel.ContainsKey(thumb))
                    {
                        foreach (var item in saveThumbInfoWindowModel[thumb].TreeItems[getRealCmd]["第 1 条参数"])
                        {
                            await treeBase.AddItemToComBox(thumb, cmd, getRealCmd, "第 1 条参数", item.Key);
                        }
  
                    }
                    else
                    {
                        getConditionsCmdProjectEdit_CBox.Items.Add("第 1 项");

                        getConditionsCmdProjectEdit_CBox.SelectedItem = "第 1 项";

                        await treeBase.AddItemToComBox(thumb, cmd, getRealCmd, "第 1 条参数", "第 1 项");
                    }
                }

                var getRealOne = TxtSplit(cmd, ": ");

                if (getRealOne.Count == 3)
                {
                    cmd = getRealOne[1] + ": " + getRealOne[2];
                }

                if(cmd == "存储对话: pointer")
                {
                    (GetControl("ConditionsAdd_Btn", thumb) as Button).IsEnabled = false;
                    (GetControl("ConditionsRemove_Btn", thumb) as Button).IsEnabled = false;
                }
                else
                {
                    (GetControl("ConditionsAdd_Btn", thumb) as Button).IsEnabled = true;
                    (GetControl("ConditionsRemove_Btn", thumb) as Button).IsEnabled = true;
                }

                getConditionsCmdProjectEdit_CBox.SelectionChanged -= GetConditionsCmdProjectEdit_CBox_SelectionChanged;
                getConditionsCmdProjectEdit_CBox.SelectionChanged += GetConditionsCmdProjectEdit_CBox_SelectionChanged;

                (GetControl("ConditionsAdd_Btn", thumb) as Button).Click -= PlayerLoaderBase_Click;
                (GetControl("ConditionsAdd_Btn", thumb) as Button).Click += PlayerLoaderBase_Click;

                (GetControl("ConditionsRemove_Btn", thumb) as Button).Click -= PlayerLoaderBase_Click1;
                (GetControl("ConditionsRemove_Btn", thumb) as Button).Click += PlayerLoaderBase_Click1;

                model.SetSuccese();

                return model;
            }
            catch
            {
                model.SetError("未知的命令");

                return model;
            }

        }

        private async void PlayerLoaderBase_Click1(object sender, System.Windows.RoutedEventArgs e)
        {
            var self = sender as Button;

            var ccpeCoBox = GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox;

            if (self.IsEnabled && ccpeCoBox.SelectedItem != null)
            {
                var one = (GetControl("ConditionsCmdEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString();

                var two = (GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString();

                var three = ccpeCoBox.SelectedItem.ToString();

                saveThumbInfoWindowModel[getThumb].TreeItems[one][two].Remove(three);

                if (MainWindowViewModel.mainWindowModels.SaveThumbInfo.ContainsKey(getThumb))
                {
                    var getInfo = MainWindowViewModel.mainWindowModels.SaveThumbInfo[getThumb];

                    try
                    {
                        var getNeed = getInfo[(GetControl("Conditions_CBox", getThumb) as ComboBox).SelectedItem.ToString()]
                            [(GetControl("ConditionsCmdEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString()]
                            [(GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString()];

                        if (getNeed.ContainsKey(three))
                        {
                            getNeed.Remove(three);
                        }
                    }
                    catch
                    {

                    }
                }
                var treeBase = new TreeViewBase();
                await treeBase.DeleteToTreeView(saveTree,one, two, three);

                ccpeCoBox.Items.Remove(ccpeCoBox.SelectedItem);
            }
        }

        private async void PlayerLoaderBase_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var self = sender as Button;

            if (self.IsEnabled)
            {
                var ccpeCoBox = GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox;

                var one = (GetControl("ConditionsCmdEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString();

                var two = (GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString();

                var cs1 = saveThumbInfoWindowModel.ContainsKey(getThumb);

                var cs2 = saveThumbInfoWindowModel[getThumb].TreeItems.ContainsKey(one);

                var cs3 = saveThumbInfoWindowModel[getThumb].TreeItems[one].ContainsKey(two);

                saveThumbInfoWindowModel[getThumb].TreeItems[one][two].Add($"第 {ccpeCoBox.Items.Count + 1} 项", false);

                var treeBase = new TreeViewBase();

                await treeBase.AddItemToTreeView(saveTree,one, two, $"第 {ccpeCoBox.Items.Count + 1} 项",false);

                ccpeCoBox.Items.Add($"第 {ccpeCoBox.Items.Count + 1} 项");
            }
        }

        private void GetConditionsCmdProjectEdit_CBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!MainWindowViewModel.mainWindowModels.SaveThumbInfo.ContainsKey(getThumb))
            {
                return;
            }

            var getConditions_CBox = GetControl("Conditions_CBox", getThumb) as ComboBox;

            var getConditionsCmdEdit_CBox = GetControl("ConditionsCmdEdit_CBox", getThumb) as ComboBox;

            var getConditionsCmdparameterEdit_CBox = GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox;

            var getConditionsCmdProjectEdit_CBox = sender as ComboBox;

            var getConditions_ComboBox = GetControl("Conditions_ComboBox", getThumb) as ComboBox;

            var getConditions_TBox = GetControl("Conditions_TBox", getThumb) as TextBox;

            var getInfo = MainWindowViewModel.mainWindowModels.SaveThumbInfo[getThumb];

            string one = string.Empty, two = string.Empty, three = string.Empty, four = string.Empty;

            if (getConditions_CBox.SelectedItem != null)
            {
                one = getConditions_CBox.SelectedItem.ToString();
            }
            else
            {
                return;
            }

            if (getConditionsCmdEdit_CBox.SelectedItem != null)
            {
                two = getConditionsCmdEdit_CBox.SelectedItem.ToString();
            }
            else
            {
                return;
            }

            if (getConditionsCmdparameterEdit_CBox.SelectedItem != null)
            {
                three = getConditionsCmdparameterEdit_CBox.SelectedItem.ToString();
            }
            else
            {
                return;
            }

            if (getConditionsCmdProjectEdit_CBox.SelectedItem != null)
            {
                four = getConditionsCmdProjectEdit_CBox.SelectedItem.ToString();
            }
            else
            {
                return;
            }

            try
            {
                var getRealOne = TxtSplit(one, ": ");

                if (getRealOne.Count == 3)
                {
                    one = getRealOne[1] + ": " + getRealOne[2];
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
            catch
            {
                return;
            }
        }

        public async Task<ReturnModel> ChangeTheTree(TreeView tiw)
        {
            var result = new ReturnModel();

            var windowModel = new ThumbInfoWindowModel
            {
                TreeItems = new Dictionary<string, Dictionary<string, Dictionary<string, bool>>>()
            };

            if (saveThumbInfoWindowModel == null || !saveThumbInfoWindowModel.ContainsKey(getThumb))//当对象存储区为空或者找不到相关Thumb时创建新的存储
            {
                windowModel.TreeItems.Add("text", new Dictionary<string, Dictionary<string, bool>>()
                {
                    {"第 1 条参数",new Dictionary<string, bool>()
                        {
                            {"第 1 项",false }
                        }
                    }
                });

                windowModel.TreeItems.Add("conditions", new Dictionary<string, Dictionary<string, bool>>()
                {
                    {"第 1 条参数",new Dictionary<string, bool>()
                        {
                            {"第 1 项",false }
                        }
                    }
                });

                windowModel.TreeItems.Add("events", new Dictionary<string, Dictionary<string, bool>>()
                {
                    {"第 1 条参数",new Dictionary<string, bool>()
                        {
                            {"第 1 项",false }
                        }
                    }
                });

                windowModel.TreeItems.Add("pointer", new Dictionary<string, Dictionary<string, bool>>()
                {
                    {"第 1 条参数",new Dictionary<string, bool>()
                        {
                            {"第 1 项",false }
                        }
                    }
                });

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
            else
            {
                windowModel = saveThumbInfoWindowModel[getThumb];
            }
            //确保对象存储区始终有值

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
    }
}
