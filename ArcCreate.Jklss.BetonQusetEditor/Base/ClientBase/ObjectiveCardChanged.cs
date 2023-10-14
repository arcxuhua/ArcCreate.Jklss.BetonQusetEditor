using ArcCreate.Jklss.BetonQusetEditor.ViewModel.MainWindows;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.ThumbModel.CommandModel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.BetonQusetEditor.Base.ClientBase
{
    public class ObjectiveCardChanged//暂时不做接口处理
    {
        private List<ObjectiveCmdModel> savecmdModels;

        private AnyCardViewModel cardInfo;

        public static Dictionary<AnyCardViewModel, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> saveAllValue = new Dictionary<AnyCardViewModel, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>();
        public ObjectiveCardChanged(List<ObjectiveCmdModel> savecmdModels, AnyCardViewModel cardInfo)
        {
            this.savecmdModels = savecmdModels;

            this.cardInfo = cardInfo;
        }

        public async Task<ReturnModel> TypeChanged()
        {
            var result = new ReturnModel();

            if (!saveAllValue.TryGetValue(cardInfo, out var vl))
            {
                saveAllValue.Add(cardInfo, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>());
            }
            else
            {
                saveAllValue.Remove(cardInfo);
                saveAllValue.Add(cardInfo, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>());
            }

            var getRealCmd = GetRealCmd(cardInfo.SelectType);

            if (!savecmdModels.Where(t => t.MainClass == getRealCmd).Any())
            {
                result.SetError($"错误：您的语法模型中没有该命令[{getRealCmd}]！你可以自定义该语法或购买他人的语法");

                return result;
            }

            var getModel = savecmdModels.Where(t => t.MainClass == getRealCmd).FirstOrDefault();

            //相关参数的添加
            await Task.Run(() =>
            {
                //主命令的添加
                saveAllValue[cardInfo].Add(cardInfo.SelectType,
                    new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                    {
                        {getRealCmd,new Dictionary<string, Dictionary<string, string>>() }
                    });

                //主命令下主参数的添加
                saveAllValue[cardInfo][cardInfo.SelectType][getRealCmd].Add("第 1 条参数",
                    new Dictionary<string, string>());

                if (getModel.TextNum == -1)//无限项时
                {
                    saveAllValue[cardInfo][cardInfo.SelectType][getRealCmd]["第 1 条参数"].Add("第 1 项", "");
                }
                else if (getModel.TextNum == 0)//没有项时
                {
                    saveAllValue[cardInfo][cardInfo.SelectType][getRealCmd]["第 1 条参数"].Add("开启/关闭", "关闭");
                }
                else//有限项时
                {
                    for (int i = 0; i < getModel.TextNum; i++)
                    {
                        saveAllValue[cardInfo][cardInfo.SelectType][getRealCmd]["第 1 条参数"].Add($"第 {i + 1} 项", "");
                    }
                }

                //主命令下其他参数的添加
                for (int i = 0; i < getModel.TextSplitWords.Count; i++)
                {
                    saveAllValue[cardInfo][cardInfo.SelectType][getRealCmd].Add($"第 {i + 2} 条参数",
                    new Dictionary<string, string>());

                    if (getModel.TextSplitWords[i].j == -1)//无限项时
                    {
                        saveAllValue[cardInfo][cardInfo.SelectType][getRealCmd][$"第 {i + 2} 条参数"].Add("第 1 项", "");
                    }
                    else if (getModel.TextSplitWords[i].j == 0)//没有项时,理论不存在该情况
                    {
                        saveAllValue[cardInfo][cardInfo.SelectType][getRealCmd][$"第 {i + 2} 条参数"].Add("开启/关闭", "关闭");
                    }
                    else//有限项时
                    {
                        for (int j = 0; j < getModel.TextSplitWords[i].j; j++)
                        {
                            saveAllValue[cardInfo][cardInfo.SelectType][getRealCmd][$"第 {i + 2} 条参数"].Add($"第 {j + 1} 项", "");
                        }
                    }
                }

                //子命令的添加
                for (int i = 0; i < getModel.ChildClasses.Count; i++)
                {
                    saveAllValue[cardInfo][cardInfo.SelectType].Add(getModel.ChildClasses[i].ChildClass,
                        new Dictionary<string, Dictionary<string, string>>());

                    //子命令下主参数的添加

                    saveAllValue[cardInfo][cardInfo.SelectType][getModel.ChildClasses[i].ChildClass].Add("第 1 条参数", new Dictionary<string, string>());

                    if (getModel.ChildClasses[i].ChildTextNum == -1)//无限项时
                    {
                        saveAllValue[cardInfo][cardInfo.SelectType][getModel.ChildClasses[i].ChildClass][$"第 1 条参数"].Add("第 1 项", "");
                    }
                    else if (getModel.ChildClasses[i].ChildTextNum == 0)//没有项时,理论不存在该情况
                    {
                        saveAllValue[cardInfo][cardInfo.SelectType][getModel.ChildClasses[i].ChildClass][$"第 1 条参数"].Add("开启/关闭", "关闭");
                    }
                    else//有限项时
                    {
                        for (int j = 0; j < getModel.ChildClasses[i].ChildTextNum; j++)
                        {
                            saveAllValue[cardInfo][cardInfo.SelectType][getModel.ChildClasses[i].ChildClass][$"第 1 条参数"].Add($"第 {j + 1} 项", "");
                        }
                    }

                    //子命令下其他参数的添加，理论不存在
                }

                //固有子命令的添加
                for (int i = 0; i < getModel.ChildTags.Count; i++)
                {
                    saveAllValue[cardInfo][cardInfo.SelectType].Add(getModel.ChildTags[i], new Dictionary<string, Dictionary<string, string>>());
                    switch (getModel.ChildTags[i])
                    {
                        case "conditions":
                            saveAllValue[cardInfo]
                            [cardInfo.SelectType]
                            [getModel.ChildTags[i]]
                            .Add("第 1 条参数", new Dictionary<string, string>() { { "第 1 项", "" } });
                            break;
                        case "events":
                            saveAllValue[cardInfo]
                            [cardInfo.SelectType]
                            [getModel.ChildTags[i]]
                            .Add("第 1 条参数", new Dictionary<string, string>() { { "第 1 项", "" } });
                            break;
                        case "notify":
                            saveAllValue[cardInfo]
                            [cardInfo.SelectType]
                            [getModel.ChildTags[i]]
                            .Add("第 1 条参数", new Dictionary<string, string>() { { "开启/关闭", "关闭" } });
                            break;
                        case "ignoreCase":
                            saveAllValue[cardInfo]
                            [cardInfo.SelectType]
                            [getModel.ChildTags[i]]
                            .Add("第 1 条参数", new Dictionary<string, string>() { { "开启/关闭", "关闭" } });
                            break;
                        case "cancel":
                            saveAllValue[cardInfo]
                            [cardInfo.SelectType]
                            [getModel.ChildTags[i]]
                            .Add("第 1 条参数", new Dictionary<string, string>() { { "开启/关闭", "关闭" } });
                            break;
                    }
                }
            });

            //视图的改变
            cardInfo.AllCmd.Clear();

            cardInfo.AllParameter.Clear();

            cardInfo.AllItem.Clear();

            cardInfo.ItemAddIsEnable = false;

            cardInfo.ItemRemoveIsEnable = false;

            cardInfo.Item_Items.Clear();

            cardInfo.ItemCoBoxVisibility = System.Windows.Visibility.Hidden;

            cardInfo.ItemCoBoxIsEnable = false;

            cardInfo.ItemContent = string.Empty;

            foreach (var item in saveAllValue[cardInfo].Values)
            {
                foreach (var i in item)
                {
                    cardInfo.AllCmd.Add(i.Key);
                }
            }

            #region 帮助提示

            cardInfo.TypeHelp = getModel.MainToolTip;

            cardInfo.ShurtcutIdea.Clear();

            if(getModel.NeedTpye != null)
            {
                foreach (var item in getModel.NeedTpye)
                {
                    foreach (var i in item.Value)
                    {
                        switch (i.Value)
                        {
                            case ThumbClass.Subject:

                                cardInfo.ShurtcutIdea.Add(new ShurtcutIdeaBtnViewModel
                                {
                                    CardName = $"创建对话主体卡片[{item.Key} 中的第 {i.Key + 1} 条参数]",
                                    ThumbClassName = i.Value,
                                });

                                break;

                            case ThumbClass.Player:

                                cardInfo.ShurtcutIdea.Add(new ShurtcutIdeaBtnViewModel
                                {
                                    CardName = $"创建玩家对话卡片[{item.Key} 中的第 {i.Key + 1} 条参数]",
                                    ThumbClassName = i.Value,
                                });

                                break;

                            case ThumbClass.NPC:

                                cardInfo.ShurtcutIdea.Add(new ShurtcutIdeaBtnViewModel
                                {
                                    CardName = $"创建NPC对话卡片[{item.Key} 中的第 {i.Key + 1} 条参数]",
                                    ThumbClassName = i.Value,
                                });

                                break;

                            case ThumbClass.Conditions:

                                cardInfo.ShurtcutIdea.Add(new ShurtcutIdeaBtnViewModel
                                {
                                    CardName = $"创建条件卡片[{item.Key} 中的第 {i.Key + 1} 条参数]",
                                    ThumbClassName = i.Value,
                                });

                                break;

                            case ThumbClass.Events:

                                cardInfo.ShurtcutIdea.Add(new ShurtcutIdeaBtnViewModel
                                {
                                    CardName = $"创建事件卡片[{item.Key} 中的第 {i.Key + 1} 条参数]",
                                    ThumbClassName = i.Value,
                                });

                                break;

                            case ThumbClass.Objectives:

                                cardInfo.ShurtcutIdea.Add(new ShurtcutIdeaBtnViewModel
                                {
                                    CardName = $"创建目标卡片[{item.Key} 中的第 {i.Key + 1} 条参数]",
                                    ThumbClassName = i.Value,
                                });

                                break;

                            case ThumbClass.Journal:

                                cardInfo.ShurtcutIdea.Add(new ShurtcutIdeaBtnViewModel
                                {
                                    CardName = $"创建日记卡片[{item.Key} 中的第 {i.Key + 1} 条参数]",
                                    ThumbClassName = i.Value,
                                });

                                break;

                            case ThumbClass.Items:

                                cardInfo.ShurtcutIdea.Add(new ShurtcutIdeaBtnViewModel
                                {
                                    CardName = $"创建物品卡片[{item.Key} 中的第 {i.Key + 1} 条参数]",
                                    ThumbClassName = i.Value,
                                });

                                break;
                        }
                    }
                }
            }

            #endregion

            result.SetSuccese();

            return result;
        }

        public async Task<ReturnModel> CmdChanged()
        {
            var result = new ReturnModel();

            try
            {
                if (!saveAllValue.TryGetValue(cardInfo, out var info))
                {
                    result.SetError();

                    return result;
                }

                if (!info.TryGetValue(cardInfo.SelectType, out var getTypeInfo))
                {
                    result.SetError();

                    return result;
                }

                if (!getTypeInfo.TryGetValue(cardInfo.SelectCmd, out var getCmdInfo))
                {
                    result.SetError();

                    return result;
                }

                cardInfo.AllParameter.Clear();

                foreach (var item in getCmdInfo)
                {
                    cardInfo.AllParameter.Add(item.Key);
                }

                cardInfo.AllItem.Clear();

                cardInfo.ItemAddIsEnable = false;

                cardInfo.ItemRemoveIsEnable = false;

                cardInfo.Item_Items.Clear();

                cardInfo.ItemCoBoxVisibility = System.Windows.Visibility.Hidden;

                cardInfo.ItemCoBoxIsEnable = false;

                cardInfo.ItemContent = string.Empty;

                #region 帮助提示

                var getRealCmd = GetRealCmd(cardInfo.SelectType);

                if (!savecmdModels.Where(t => t.MainClass == getRealCmd).Any())
                {
                    result.SetError($"错误：您的语法模型中没有该命令[{getRealCmd}]！你可以自定义该语法或购买他人的语法");

                    return result;
                }
                var getModel = savecmdModels.Where(t => t.MainClass == getRealCmd).FirstOrDefault();

                var getSelecteCmdNum = cardInfo.AllCmd.IndexOf(cardInfo.SelectCmd);

                if (getModel.CmdToolTip.Count > getSelecteCmdNum)
                {
                    cardInfo.CmdHelp = getModel.CmdToolTip[getSelecteCmdNum];
                }
                #endregion

                result.SetSuccese();

                return result;
            }
            catch
            {
                result.SetError();

                return result;
            }

        }

        public async Task<ReturnModel> ParameterChanged()
        {
            var result = new ReturnModel();

            try
            {
                if (!saveAllValue.TryGetValue(cardInfo, out var info))
                {
                    result.SetError();

                    return result;
                }

                if (!info.TryGetValue(cardInfo.SelectType, out var getTypeInfo))
                {
                    result.SetError();

                    return result;
                }

                if (!getTypeInfo.TryGetValue(cardInfo.SelectCmd, out var getCmdInfo))
                {
                    result.SetError();

                    return result;
                }

                if (!getCmdInfo.TryGetValue(cardInfo.SelectParameter, out var getParameterInfo))
                {
                    result.SetError();

                    return result;
                }

                cardInfo.AllItem.Clear();

                foreach (var item in getParameterInfo)
                {
                    cardInfo.AllItem.Add(item.Key);
                }

                var getTF = await Task.Run(() =>
                {
                    if (!savecmdModels.Where(t => t.MainClass == GetRealCmd(cardInfo.SelectType)).Any())
                    {
                        return false;
                    }

                    var getModel = savecmdModels.Where(t => t.MainClass == GetRealCmd(cardInfo.SelectType)).FirstOrDefault();

                    if (cardInfo.SelectCmd == GetRealCmd(cardInfo.SelectType))//选中的是主命令
                    {
                        var getParNum = cardInfo.SelectParameter.Split(' ');
                        if (getParNum.Length != 3)
                        {
                            return false;
                        }

                        if (getParNum[1] == "1")//选中的是主参数
                        {
                            if (getModel.TextNum == -1)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (getModel.TextSplitWords.Count > Convert.ToInt32(getParNum[1])-2 && getModel.TextSplitWords[Convert.ToInt32(getParNum[1])-2].j == -1)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        switch (cardInfo.SelectCmd)
                        {
                            case "conditions":
                                return true;
                            case "events":
                                return true;
                        }

                        if(getModel.ChildClasses.Where(t => t.ChildClass == cardInfo.SelectCmd).Any())
                        {
                            var getChild = getModel.ChildClasses.Where(t => t.ChildClass == cardInfo.SelectCmd).FirstOrDefault();

                            if (getChild.ChildTextNum == -1)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return false;
                        }
                        
                    }

                    return false;
                });

                cardInfo.ItemAddIsEnable = getTF;

                cardInfo.ItemRemoveIsEnable = getTF;

                cardInfo.Item_Items.Clear();

                cardInfo.ItemCoBoxVisibility = System.Windows.Visibility.Hidden;

                cardInfo.ItemCoBoxIsEnable = false;

                cardInfo.ItemContent = string.Empty;

                #region 帮助提示

                var getRealCmd = GetRealCmd(cardInfo.SelectType);

                if (!savecmdModels.Where(t => t.MainClass == getRealCmd).Any())
                {
                    result.SetError($"错误：您的语法模型中没有该命令[{getRealCmd}]！你可以自定义该语法或购买他人的语法");

                    return result;
                }
                var getModel = savecmdModels.Where(t => t.MainClass == getRealCmd).FirstOrDefault();

                var getSelecteCmdNum = cardInfo.AllCmd.IndexOf(cardInfo.SelectCmd);

                var getSelecteParNum = cardInfo.AllParameter.IndexOf(cardInfo.SelectParameter);

                try
                {
                    cardInfo.ParameterHelp = getModel.ParameterToolTip[getSelecteCmdNum][getSelecteParNum];
                }
                catch
                {

                }
                #endregion

                result.SetSuccese();

                return result;
            }
            catch
            {
                result.SetError();
                return result;
            }
        }

        public async Task<ReturnModel> ItemChanged()
        {
            var result = new ReturnModel();

            try
            {
                if (!saveAllValue.TryGetValue(cardInfo, out var info))
                {
                    result.SetError();

                    return result;
                }

                if (!info.TryGetValue(cardInfo.SelectType, out var getTypeInfo))
                {
                    result.SetError();

                    return result;
                }

                if (!getTypeInfo.TryGetValue(cardInfo.SelectCmd, out var getCmdInfo))
                {
                    result.SetError();

                    return result;
                }

                if (!getCmdInfo.TryGetValue(cardInfo.SelectParameter, out var getParameterInfo))
                {
                    result.SetError();

                    return result;
                }

                if (!getParameterInfo.TryGetValue(cardInfo.SelectItem, out var getItemInfo))
                {
                    result.SetError();

                    return result;
                }

                if (!savecmdModels.Where(t => t.MainClass == GetRealCmd(cardInfo.SelectType)).Any())
                {
                    result.SetError();

                    return result;
                }

                var getModel = savecmdModels.Where(t => t.MainClass == GetRealCmd(cardInfo.SelectType)).FirstOrDefault();

                var getTF = await Task.Run(() =>
                {
                    
                    if(cardInfo.SelectItem == "开启/关闭")//是否为无参数
                    {
                        return true;
                    }

                    if (cardInfo.SelectCmd == GetRealCmd(cardInfo.SelectType))//选中的是主命令
                    {
                        var getParNum = cardInfo.SelectParameter.Split(' ');
                        if (getParNum.Length != 3)
                        {
                            return false;
                        }

                        if (getParNum[1] == "1")//选中的是主参数
                        {
                            if (getModel.Tags.Count > 0)
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                });

                if (getTF)
                {
                    cardInfo.ItemCoBoxIsEnable = getTF;

                    cardInfo.ItemCoBoxVisibility = System.Windows.Visibility.Visible;

                }

                if (cardInfo.ItemCoBoxIsEnable && cardInfo.ItemCoBoxVisibility == System.Windows.Visibility.Visible)
                {
                    cardInfo.Item_Items.Clear();

                    if (cardInfo.SelectItem == "开启/关闭")
                    {
                        cardInfo.Item_Items.Add("开启");
                        cardInfo.Item_Items.Add("关闭");
                    }
                    else
                    {
                        for (int i = 0; i < getModel.Tags.Count; i++)
                        {
                            cardInfo.Item_Items.Add(getModel.Tags[i]);
                        }
                    }
                    

                    cardInfo.ItemSelectContent = getItemInfo;
                }
                else
                {
                    cardInfo.ItemContent = getItemInfo;
                }

                #region 帮助提示

                var getSelecteCmdNum = cardInfo.AllCmd.IndexOf(cardInfo.SelectCmd);

                var getSelecteParNum = cardInfo.AllParameter.IndexOf(cardInfo.SelectParameter);

                var getSelecteItemNum = cardInfo.AllItem.IndexOf(cardInfo.SelectItem);

                try
                {
                    cardInfo.ItemHelp = getModel.TermToolTip[getSelecteCmdNum][getSelecteParNum][getSelecteItemNum];
                }
                catch
                {

                }
                #endregion

                result.SetSuccese();

                return result;
            }
            catch
            {
                result.SetError();

                return result;
            }
        }

        public async Task<ReturnModel> ItemAdd()
        {
            var result = new ReturnModel();

            var type = cardInfo.SelectType;

            var cmd = cardInfo.SelectCmd;

            var par = cardInfo.SelectParameter;

            var item = cardInfo.SelectItem;

            var realCmd = GetRealCmd(type);

            if (!savecmdModels.Where(t => t.MainClass == realCmd).Any())
            {
                result.SetError("错误，您的语法模型中没有相关模型！");

                return result;
            }

            var getModel = savecmdModels.Where(t => t.MainClass == realCmd).FirstOrDefault();

            if (!getModel.ChildClasses.Where(t => t.ChildClass == cmd).Any()&&!getModel.ChildTags.Contains(cmd))
            {
                var getParNum = par.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (getParNum.Length < 3)
                {
                    result.SetError("错误，您的语法模型中没有相关模型！");

                    return result;
                }

                var getIntParNum = Convert.ToInt32(getParNum[1]);

                if (getIntParNum == 1)//主参数
                {
                    if (getModel.TextNum == -1)
                    {
                        try
                        {
                            var getnowNum = saveAllValue[cardInfo][type][cmd][par];

                            var nowNum = 0;

                            foreach (var i in getnowNum)
                            {
                                var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                if (fg.Length != 3)
                                {
                                    result.SetError("添加错误！");

                                    return result;
                                }

                                var num = Convert.ToInt32(fg[1]);

                                if (num > nowNum)
                                {
                                    nowNum = num;
                                }
                            }

                            saveAllValue[cardInfo][type][cmd][par].Add($"第 {nowNum + 1} 项", "");

                            cardInfo.AllItem.Add($"第 {nowNum + 1} 项");

                            result.SetSuccese("添加成功");

                            return result;
                        }
                        catch
                        {
                            result.SetError("添加错误！");

                            return result;
                        }
                    }
                    else
                    {
                        result.SetError("该参数不允许添加其他项！");

                        return result;
                    }
                }
                else
                {
                    try
                    {
                        var realNum = getIntParNum - 1;

                        var getFg = getModel.TextSplitWords[realNum];

                        if (getFg.j != -1)
                        {
                            result.SetError("该参数不允许添加其他项！");

                            return result;
                        }

                        var getnowNum = saveAllValue[cardInfo][type][cmd][par];

                        var nowNum = 0;

                        foreach (var i in getnowNum)
                        {
                            var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                            if (fg.Length != 3)
                            {
                                result.SetError("添加错误！");

                                return result;
                            }

                            var num = Convert.ToInt32(fg[1]);

                            if (num > nowNum)
                            {
                                nowNum = num;
                            }
                        }

                        saveAllValue[cardInfo][type][cmd][par].Add($"第 {nowNum + 1} 项", "");

                        cardInfo.AllItem.Add($"第 {nowNum + 1} 项");

                        result.SetSuccese("添加成功");

                        return result;
                    }
                    catch
                    {
                        result.SetError("该参数不允许添加其他项！");

                        return result;
                    }
                }
            }//主命令情况
            else if (getModel.ChildTags.Contains(cmd))
            {
                switch (cmd)
                {
                    case "conditions":
                        var getnowNum = saveAllValue[cardInfo][type][cmd][par];

                        var nowNum = 0;

                        foreach (var i in getnowNum)
                        {
                            var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                            if (fg.Length != 3)
                            {
                                result.SetError("添加错误！");

                                return result;
                            }

                            var num = Convert.ToInt32(fg[1]);

                            if (num > nowNum)
                            {
                                nowNum = num;
                            }
                        }

                        saveAllValue[cardInfo][type][cmd][par].Add($"第 {nowNum + 1} 项", "");

                        cardInfo.AllItem.Add($"第 {nowNum + 1} 项");
                        result.SetSuccese("添加成功");

                        return result;
                    case "events":
                        getnowNum = saveAllValue[cardInfo][type][cmd][par];

                        nowNum = 0;

                        foreach (var i in getnowNum)
                        {
                            var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                            if (fg.Length != 3)
                            {
                                result.SetError("添加错误！");

                                return result;
                            }

                            var num = Convert.ToInt32(fg[1]);

                            if (num > nowNum)
                            {
                                nowNum = num;
                            }
                        }

                        saveAllValue[cardInfo][type][cmd][par].Add($"第 {nowNum + 1} 项", "");

                        cardInfo.AllItem.Add($"第 {nowNum + 1} 项");
                        result.SetSuccese("添加成功");

                        return result;
                    case "notify":
                        result.SetError("该参数不允许添加其他项！");

                        return result;
                    case "ignoreCase":
                        result.SetError("该参数不允许添加其他项！");

                        return result;
                    case "cancel":
                        result.SetError("该参数不允许添加其他项！");

                        return result;
                }
            }
            else
            {
                var getchildModel = getModel.ChildClasses.Where(t => t.ChildClass == cmd).FirstOrDefault();

                //子参数只能存在一个主参数的情况
                var getParNum = par.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (getParNum.Length < 3)
                {
                    result.SetError("错误，您的语法模型中没有相关模型！");

                    return result;
                }

                var getIntParNum = Convert.ToInt32(getParNum[1]);

                if (getIntParNum != 1)
                {
                    result.SetError("错误，您的语法模型出现了错误，子命令不允许出现子参数！");

                    return result;
                }

                if (getchildModel.ChildTextNum != -1)
                {
                    result.SetError("该参数不允许添加其他项！");

                    return result;
                }

                try
                {
                    var getnowNum = saveAllValue[cardInfo][type][cmd][par];

                    var nowNum = 0;

                    foreach (var i in getnowNum)
                    {
                        var fg = i.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        if (fg.Length != 3)
                        {
                            result.SetError("添加错误！");

                            return result;
                        }

                        var num = Convert.ToInt32(fg[1]);

                        if (num > nowNum)
                        {
                            nowNum = num;
                        }
                    }

                    saveAllValue[cardInfo][type][cmd][par].Add($"第 {nowNum + 1} 项", "");

                    cardInfo.AllItem.Add($"第 {nowNum + 1} 项");

                    result.SetSuccese("添加成功");

                    return result;
                }
                catch
                {
                    result.SetError("添加错误！");

                    return result;
                }
            }

            result.SetSuccese();

            return result;
        }

        public async Task<ReturnModel> ItemRemove()
        {
            var result = new ReturnModel();

            var type = cardInfo.SelectType;

            var cmd = cardInfo.SelectCmd;

            var par = cardInfo.SelectParameter;

            var item = cardInfo.SelectItem;

            var realCmd = GetRealCmd(type);

            if (!savecmdModels.Where(t => t.MainClass == realCmd).Any())
            {
                result.SetError("错误，您的语法模型中没有相关模型！");

                return result;
            }

            var getModel = savecmdModels.Where(t => t.MainClass == realCmd).FirstOrDefault();

            if (!getModel.ChildClasses.Where(t => t.ChildClass == cmd).Any() && !getModel.ChildTags.Contains(cmd))
            {
                var getParNum = par.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (getParNum.Length < 3)
                {
                    result.SetError("错误，您的语法模型中没有相关模型！");

                    return result;
                }

                var getIntParNum = Convert.ToInt32(getParNum[1]);

                if (getIntParNum == 1)//主参数
                {
                    if (getModel.TextNum == -1)
                    {
                        try
                        {
                            if (saveAllValue[cardInfo][type][cmd][par].TryGetValue(item, out var value))
                            {
                                if (saveAllValue[cardInfo][type][cmd][par].Count < 2)
                                {
                                    result.SetError("仅剩一个项不允许被删除！");

                                    return result;
                                }
                                saveAllValue[cardInfo][type][cmd][par].Remove(item);

                                cardInfo.AllItem.Remove(item);

                                result.SetSuccese("删除成功");

                                return result;
                            }
                            else
                            {
                                result.SetError("该项不存在！");

                                return result;
                            }
                        }
                        catch
                        {
                            result.SetError("删除错误！");

                            return result;
                        }
                    }
                    else
                    {
                        result.SetError("该参数不允许删除项！");

                        return result;
                    }
                }
                else
                {
                    try
                    {
                        var realNum = getIntParNum - 1;

                        var getFg = getModel.TextSplitWords[realNum];

                        if (getFg.j != -1)
                        {
                            result.SetError("该参数不允许删除项！");

                            return result;
                        }

                        if (saveAllValue[cardInfo][type][cmd][par].TryGetValue(item, out var value))
                        {
                            if (saveAllValue[cardInfo][type][cmd][par].Count < 2)
                            {
                                result.SetError("仅剩一个项不允许被删除！");

                                return result;
                            }
                            saveAllValue[cardInfo][type][cmd][par].Remove(item);

                            cardInfo.AllItem.Remove(item);

                            result.SetSuccese("删除成功");

                            return result;
                        }
                        else
                        {
                            result.SetError("该项不存在！");

                            return result;
                        }
                    }
                    catch
                    {
                        result.SetError("该参数不允许删除项！");

                        return result;
                    }
                }
            }
            else if (getModel.ChildTags.Contains(cmd))
            {
                switch (cmd)
                {
                    case "conditions":
                        if (saveAllValue[cardInfo][type][cmd][par].Count < 2)
                        {
                            result.SetError("仅剩一个项不允许被删除！");

                            return result;
                        }
                        saveAllValue[cardInfo][type][cmd][par].Remove(item);

                        cardInfo.AllItem.Remove(item);

                        result.SetSuccese("删除成功");

                        return result;
                    case "events":
                        if (saveAllValue[cardInfo][type][cmd][par].Count < 2)
                        {
                            result.SetError("仅剩一个项不允许被删除！");

                            return result;
                        }
                        saveAllValue[cardInfo][type][cmd][par].Remove(item);

                        cardInfo.AllItem.Remove(item);

                        result.SetSuccese("删除成功");

                        return result;
                    case "notify":
                        result.SetError("该参数不允许删除项！");

                        return result;
                    case "ignoreCase":
                        result.SetError("该参数不允许删除项！");

                        return result;
                    case "cancel":
                        result.SetError("该参数不允许删除项！");

                        return result;
                }
            }
            else
            {
                var getchildModel = getModel.ChildClasses.Where(t => t.ChildClass == cmd).FirstOrDefault();

                //子参数只能存在一个主参数的情况
                var getParNum = par.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (getParNum.Length < 3)
                {
                    result.SetError("错误，您的语法模型中没有相关模型！");

                    return result;
                }

                var getIntParNum = Convert.ToInt32(getParNum[1]);

                if (getIntParNum != 1)
                {
                    result.SetError("错误，您的语法模型出现了错误，子命令不允许出现子参数！");

                    return result;
                }

                if (getchildModel.ChildTextNum != -1)
                {
                    result.SetError("该参数不允许添加其他项！");

                    return result;
                }

                try
                {
                    if (saveAllValue[cardInfo][type][cmd][par].TryGetValue(item, out var value))
                    {
                        if (saveAllValue[cardInfo][type][cmd][par].Count < 2)
                        {
                            result.SetError("仅剩一个项不允许被删除！");

                            return result;
                        }
                        saveAllValue[cardInfo][type][cmd][par].Remove(item);

                        cardInfo.AllItem.Remove(item);

                        result.SetSuccese("删除成功");

                        return result;
                    }
                    else
                    {
                        result.SetError("该项不存在！");

                        return result;
                    }
                }
                catch
                {
                    result.SetError("删除错误！");

                    return result;
                }
            }

            result.SetError("这是我也不知道什么的情况,如果出现了麻烦报告一下");
            return result;
        }

        private string GetRealCmd(string str)
        {
            var sp = str.Split(": ", StringSplitOptions.RemoveEmptyEntries);

            if (sp.Length == 2)
            {
                return sp[1];
            }
            else
            {
                return str;
            }
        }
    }
}
