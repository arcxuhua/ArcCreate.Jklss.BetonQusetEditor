using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.ThumbInfoWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static System.Net.Mime.MediaTypeNames;

namespace ArcCreate.Jklss.BetonQusetEditor.Base
{
    public class TreeViewBase
    {
        /// <summary>
        /// 添加或修改元素到Tree
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="three"></param>
        /// <returns></returns>
        public async Task<ReturnModel> AddItemToTreeView(TreeView saveTree, string one, string two, string three,bool isSave)
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

                var isHave = false;

                foreach (var item in tree.Items)
                {
                    var getTreeItem = item as DefinitionNode;

                    if (getTreeItem.Name == one)
                    {

                        if(!getTreeItem.Children.Where(t => t.Name == two).Any())
                        {
                            return;
                        }

                        var getTwo = getTreeItem.Children.Where(t => t.Name == two).First();

                        var newChild = new DefinitionNode();

                        if (isSave)
                        {
                            newChild.Name = three + " ------ 已保存";

                            newChild.FontColor = "#1f640a";
                        }
                        else
                        {
                            newChild.Name = three + " ------ 未保存";

                            newChild.FontColor = "#f6003c";
                        }

                        var getSave = getTwo.Children.Where(t => t.Name == three + " ------ 已保存" || t.Name == three + " ------ 未保存").Any();

                        if (getSave)
                        {
                            getTwo.Children.Where(t => t.Name == three + " ------ 已保存" || t.Name == three + " ------ 未保存").First().Name=newChild.Name;
                            getTwo.Children.Where(t => t.Name == three + " ------ 已保存" || t.Name == three + " ------ 未保存").First().FontColor = newChild.FontColor;
                        }
                        else
                        {
                            getTwo.Children.Add(newChild);
                        }

                        isHave = true;
                    }

                    newList.Add(getTreeItem);
                }

                if (!isHave)
                {
                    if (!newList.Where(t => t.Name == one).Any())
                    {
                        newList.Add(new DefinitionNode()
                        {
                            Name = one,
                            Children = new List<DefinitionNode>()
                        });
                    }

                    var getone = newList.Where(t => t.Name == one).First();

                    if (!getone.Children.Where(t => t.Name == two).Any())
                    {
                        getone.Children.Add(new DefinitionNode()
                        {
                            Name = two,

                            Children = new List<DefinitionNode>()
                        });
                    }

                    var gettwo = getone.Children.Where(t => t.Name == two).First();

                    var newChild = new DefinitionNode();

                    if (isSave)
                    {
                        newChild.Name = three + " ------ 已保存";

                        newChild.FontColor = "#1f640a";
                    }
                    else
                    {
                        newChild.Name = three + " ------ 未保存";

                        newChild.FontColor = "#f6003c";
                    }

                    var getSave = gettwo.Children.Where(t => t.Name == three + " ------ 已保存" || t.Name == three + " ------ 未保存").Any();

                    if (getSave)
                    {
                        gettwo.Children.Where(t => t.Name == three + " ------ 已保存" || t.Name == three + " ------ 未保存").First().Name = newChild.Name;
                        gettwo.Children.Where(t => t.Name == three + " ------ 已保存" || t.Name == three + " ------ 未保存").First().FontColor = newChild.FontColor;
                    }
                    else
                    {
                        gettwo.Children.Add(newChild);
                    }


                }

                tree.Dispatcher.Invoke(new Action(() => {
                    tree.ItemsSource = newList;
                }));

            });

            result.SetSuccese();

            return result;
        }

        /// <summary>
        /// 删除Tree中的某个元素
        /// </summary>
        /// <param name="saveTree"></param>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="three"></param>
        /// <returns></returns>
        public async Task<ReturnModel> DeleteToTreeView(TreeView saveTree, string one, string two, string three)
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
        /// 添加或修改元素到数据存储
        /// </summary>
        /// <param name="thumb"></param>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="three"></param>
        /// <param name="cmd"></param>
        /// <param name="txt"></param>
        /// <param name="isSave"></param>
        /// <param name="saveThumbInfoWindowModel"></param>
        /// <param name="saveThumbInfo"></param>
        /// <returns></returns>
        public async Task<ReturnModel> AddItemToSaves(Thumb thumb,string one, string two, string three,string cmd,string txt,bool isSave,
            Dictionary<Thumb, ThumbInfoWindowModel> saveThumbInfoWindowModel,
            Dictionary<Thumb, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> saveThumbInfo)
        {
            var result = new ReturnModel();

            if (!saveThumbInfoWindowModel.ContainsKey(thumb))
            {
                result.SetError();

                return result;
            }

            if (!saveThumbInfo.ContainsKey(thumb))
            {
                result.SetError();

                return result;
            }

            //对于树结构数据的添加
            {
                await Task.Run(() =>
                {
                    var getSaves = saveThumbInfoWindowModel[thumb].TreeItems;

                    if (!getSaves.ContainsKey(one))
                    {
                        getSaves.Add(one, new Dictionary<string, Dictionary<string, bool>>());
                    }

                    if (!getSaves[one].ContainsKey(two))
                    {
                        getSaves[one].Add(two, new Dictionary<string, bool>());
                    }

                    if (getSaves[one][two].ContainsKey(three))
                    {

                        getSaves[one][two][three] = isSave;

                    }
                    else
                    {
                        getSaves[one][two].Add(three, isSave);

                    }

                    saveThumbInfoWindowModel[thumb].TreeItems = getSaves;
                });
            }

            //对于数据存储的添加
            {
                await Task.Run(() =>
                {
                    var getSaves = saveThumbInfo[thumb];

                    if (!getSaves.ContainsKey(cmd))
                    {
                        getSaves.Add(cmd, new Dictionary<string, Dictionary<string, Dictionary<string, string>>>());
                    }

                    if (!getSaves[cmd].ContainsKey(one))
                    {
                        getSaves[cmd].Add(one, new Dictionary<string, Dictionary<string, string>>());
                    }

                    if (!getSaves[cmd][one].ContainsKey(two))
                    {
                        getSaves[cmd][one].Add(two, new Dictionary<string, string>());
                    }

                    if (!getSaves[cmd][one][two].ContainsKey(three))
                    {
                        getSaves[cmd][one][two].Add(three, txt);
                    }
                    else
                    {
                        getSaves[cmd][one][two][three] = txt;
                    }

                    saveThumbInfo[thumb] = getSaves;
                });
            }
            result.SetSuccese();

            return result;
        }

        /// <summary>
        /// 删除数据存储中的某个元素
        /// </summary>
        /// <param name="thumb"></param>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="three"></param>
        /// <param name="cmd"></param>
        /// <param name="saveThumbInfoWindowModel"></param>
        /// <param name="saveThumbInfo"></param>
        /// <returns></returns>
        public async Task<ReturnModel> DeleteItemToSaves(Thumb thumb, string one, string two, string three, string cmd,
            Dictionary<Thumb, ThumbInfoWindowModel> saveThumbInfoWindowModel,
            Dictionary<Thumb, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> saveThumbInfo)
        {
            var result = new ReturnModel();

            if (!saveThumbInfoWindowModel.ContainsKey(thumb))
            {
                result.SetError();

                return result;
            }

            if (!saveThumbInfo.ContainsKey(thumb))
            {
                result.SetError();

                return result;
            }

            //对于树结构数据的删除
            {
                await Task.Run(() =>
                {
                    var getSaves = saveThumbInfoWindowModel[thumb].TreeItems;

                    if (!getSaves.ContainsKey(one))
                    {
                        result.SetError();
                    }

                    if (!getSaves[one].ContainsKey(two))
                    {
                        result.SetError();
                    }

                    if (!getSaves[one][two].ContainsKey(three))
                    {
                        result.SetError();
                    }

                    getSaves[one][two].Remove(three);

                    saveThumbInfoWindowModel[thumb].TreeItems = getSaves;
                });
            }

            //对于数据存储的删除
            {
                await Task.Run(() =>
                {
                    var getSaves = saveThumbInfo[thumb];

                    if (!getSaves.ContainsKey(cmd))
                    {
                        result.SetError();
                    }

                    if (!getSaves[cmd].ContainsKey(one))
                    {
                        result.SetError();
                    }

                    if (!getSaves[cmd][one].ContainsKey(two))
                    {
                        result.SetError();
                    }

                    if (!getSaves[cmd][one][two].ContainsKey(three))
                    {
                        result.SetError();
                    }

                    getSaves[cmd][one][two].Remove(three);

                    saveThumbInfo[thumb] = getSaves;
                });
            }

            return result;
        }

        /// <summary>
        /// 添加或修改元素到ComboBox
        /// </summary>
        /// <param name="thumb"></param>
        /// <param name="cmd"></param>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="three"></param>
        /// <returns></returns>
        public async Task<ReturnModel> AddItemToComBox(Thumb thumb, string cmd, string one ,string two ,string three)
        {
            var result = new ReturnModel();

            var getConditions_CBox = GetControl("Conditions_CBox",thumb) as ComboBox;

            if (getConditions_CBox.SelectedItem == null)
            {
                result.SetError();

                return result;
            }

            var getConditionsCmdEdit_CBox = GetControl("ConditionsCmdEdit_CBox", thumb) as ComboBox;

            if (getConditionsCmdEdit_CBox.SelectedItem == null)
            {
                result.SetError();

                return result;
            }

            var getConditionsCmdparameterEdit_CBox = GetControl("ConditionsCmdparameterEdit_CBox", thumb) as ComboBox;

            if (getConditionsCmdparameterEdit_CBox.SelectedItem == null)
            {
                result.SetError();

                return result;
            }

            var getConditionsCmdProjectEdit_CBox = GetControl("ConditionsCmdProjectEdit_CBox", thumb) as ComboBox;

            if(getConditions_CBox.SelectedItem.ToString()!= cmd)
            {
                result.SetError();

                return result;
            }

            if(getConditionsCmdEdit_CBox.SelectedItem.ToString() != one)
            {
                result.SetError();

                return result;
            }

            if (getConditionsCmdparameterEdit_CBox.SelectedItem.ToString() != two)
            {
                result.SetError();

                return result;
            }

            result = await Task.Run(() =>
            {
                foreach (var item in getConditionsCmdProjectEdit_CBox.Items)
                {
                    if (item.ToString() == three)
                    {
                        result.SetError();

                        return result;
                    }
                }

                result.SetSuccese();

                return result;
            });

            if (result.Succese)
            {
                getConditionsCmdProjectEdit_CBox.Items.Add(three);

                result.SetSuccese();

                return result;
            }
            else
            {
                result.SetError();

                return result;
            }
        }

        /// <summary>
        /// 删除ComboBox中的某个元素
        /// </summary>
        /// <param name="thumb"></param>
        /// <param name="cmd"></param>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="three"></param>
        /// <returns></returns>
        public async Task<ReturnModel> DeleteItemToComBox(Thumb thumb, string cmd, string one, string two, string three)
        {
            var result = new ReturnModel();

            var getConditions_CBox = GetControl("Conditions_CBox", thumb) as ComboBox;

            if (getConditions_CBox.SelectedItem == null)
            {
                result.SetError();

                return result;
            }

            var getConditionsCmdEdit_CBox = GetControl("ConditionsCmdEdit_CBox", thumb) as ComboBox;

            if (getConditionsCmdEdit_CBox.SelectedItem == null)
            {
                result.SetError();

                return result;
            }

            var getConditionsCmdparameterEdit_CBox = GetControl("ConditionsCmdparameterEdit_CBox", thumb) as ComboBox;

            if (getConditionsCmdparameterEdit_CBox.SelectedItem == null)
            {
                result.SetError();

                return result;
            }

            var getConditionsCmdProjectEdit_CBox = GetControl("ConditionsCmdparameterEdit_CBox", thumb) as ComboBox;

            if (getConditions_CBox.SelectedItem.ToString() != cmd)
            {
                result.SetError();

                return result;
            }

            if (getConditionsCmdEdit_CBox.SelectedItem.ToString() != one)
            {
                result.SetError();

                return result;
            }

            if (getConditionsCmdparameterEdit_CBox.SelectedItem.ToString() != two)
            {
                result.SetError();

                return result;
            }

            result = await Task.Run(() =>
            {
                foreach (var item in getConditionsCmdProjectEdit_CBox.Items)
                {
                    if (item.ToString() == three)
                    {
                        getConditionsCmdProjectEdit_CBox.Items.Remove(item);

                        result.SetSuccese();

                        return result;
                    }
                }

                result.SetError();

                return result;
            });

            return result;
        }

        /// <summary>
        /// 从Thumb中获取控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="thumb"></param>
        /// <returns></returns>
        protected object GetControl(string name, Thumb thumb)
        {
            return thumb.Template.FindName(name, thumb);
        }
    }
}
