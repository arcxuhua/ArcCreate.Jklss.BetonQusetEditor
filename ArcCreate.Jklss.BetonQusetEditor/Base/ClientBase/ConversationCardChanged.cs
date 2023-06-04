using ArcCreate.Jklss.BetonQusetEditor.ViewModel.MainWindows;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.ThumbModel.CommandModel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.BetonQusetEditor.Base.ClientBase
{
    public class ConversationCardChanged
    {
        private AnyCardViewModel cardInfo;

        public static Dictionary<AnyCardViewModel, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> saveAllValue = new Dictionary<AnyCardViewModel, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>();

        public ConversationCardChanged(AnyCardViewModel cardInfo)
        {
            this.cardInfo = cardInfo;

            if (!saveAllValue.TryGetValue(cardInfo, out var vl))
            {
                saveAllValue.Add(cardInfo, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>()
                {
                    {"文案: text",new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                    {
                        {"text",new Dictionary<string, Dictionary<string, string>>
                        {
                            {"第 1 条参数",new Dictionary<string, string>
                            {
                                {"第 1 项","" }
                            } }
                        } }
                    } },
                    {"触发条件: conditions",new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                    {
                        {"conditions",new Dictionary<string, Dictionary<string, string>>
                        {
                            {"第 1 条参数",new Dictionary<string, string>
                            {
                                {"第 1 项","" }
                            } }
                        } }
                    } },
                    {"触发事件: events",new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                    {
                        {"events",new Dictionary<string, Dictionary<string, string>>
                        {
                            {"第 1 条参数",new Dictionary<string, string>
                            {
                                {"第 1 项","" }
                            } }
                        } }
                    } },
                    {"存储对话: pointer",new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                    {
                        {"pointer",new Dictionary<string, Dictionary<string, string>>
                        {
                            {"第 1 条参数",new Dictionary<string, string>
                            {
                                {"第 1 项","" }
                            } }
                        } }
                    } }
                });
            }

        }

        public async Task<ReturnModel> TypeChanged()
        {
            var result = new ReturnModel();

            cardInfo.AllCmd.Clear();

            cardInfo.AllParameter.Clear();

            cardInfo.AllItem.Clear();

            cardInfo.ItemAddIsEnable = false;

            cardInfo.ItemRemoveIsEnable = false;

            cardInfo.Item_Items.Clear();

            cardInfo.ItemCoBoxVisibility = System.Windows.Visibility.Hidden;

            cardInfo.ItemCoBoxIsEnable = false;

            cardInfo.ItemContent = string.Empty;

            cardInfo.AllCmd.Add(GetRealCmd(cardInfo.SelectType));

            result.SetSuccese();

            return result;
        }

        public async Task<ReturnModel> CmdChanged()
        {
            var result = new ReturnModel();

            cardInfo.AllParameter.Clear();

            cardInfo.AllItem.Clear();

            cardInfo.ItemAddIsEnable = false;

            cardInfo.ItemRemoveIsEnable = false;

            cardInfo.Item_Items.Clear();

            cardInfo.ItemCoBoxVisibility = System.Windows.Visibility.Hidden;

            cardInfo.ItemCoBoxIsEnable = false;

            cardInfo.ItemContent = string.Empty;

            cardInfo.AllParameter.Add("第 1 条参数");

            result.SetSuccese();

            return result;
        }

        public async Task<ReturnModel> ParameterChanged()
        {
            var result = new ReturnModel();

            cardInfo.AllItem.Clear();

            cardInfo.ItemAddIsEnable = false;

            cardInfo.ItemRemoveIsEnable = false;

            cardInfo.Item_Items.Clear();

            cardInfo.ItemCoBoxVisibility = System.Windows.Visibility.Hidden;

            cardInfo.ItemCoBoxIsEnable = false;

            cardInfo.ItemContent = string.Empty;

            var type = cardInfo.SelectType;

            var cmd = cardInfo.SelectCmd;

            var par = cardInfo.SelectParameter;

            try
            {
                if (saveAllValue[cardInfo][type][cmd][par].Count > 0)
                {
                    foreach (var item in saveAllValue[cardInfo][type][cmd][par])
                    {
                        cardInfo.AllItem.Add(item.Key);
                    }
                }
            else
                {
                    cardInfo.AllItem.Add("第 1 项");
                }
            }
            catch
            {
                result.SetError();

                return result;
            }
            
            result.SetSuccese();

            return result;
        }

        public async Task<ReturnModel> ItemChanged()
        {
            var result = new ReturnModel();

            if(cardInfo.Type == Model.MainWindow.ThumbClass.Player && cardInfo.SelectType == "存储对话: pointer")
            {
                cardInfo.ItemAddIsEnable = false;

                cardInfo.ItemRemoveIsEnable = false;
            }
            else if(cardInfo.Type == Model.MainWindow.ThumbClass.NPC && cardInfo.SelectType == "存储对话: pointer")
            {
                cardInfo.ItemAddIsEnable = true;

                cardInfo.ItemRemoveIsEnable = true;
            }
            else
            {
                cardInfo.ItemAddIsEnable = true;

                cardInfo.ItemRemoveIsEnable = true;
            }

            cardInfo.Item_Items.Clear();

            cardInfo.ItemCoBoxVisibility = System.Windows.Visibility.Hidden;

            cardInfo.ItemCoBoxIsEnable = false;

            cardInfo.ItemContent = string.Empty;

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

                cardInfo.ItemContent = getItemInfo;

                
            }
            catch
            {
                result.SetError();

                return result;
            }

            result.SetSuccese();

            return result;
        }

        public async Task<ReturnModel> ItemAdd()
        {
            var result = new ReturnModel();

            if (cardInfo.Type == Model.MainWindow.ThumbClass.Player && cardInfo.SelectType == "存储对话: pointer")
            {
                result.SetError("玩家对话的节点下只能拥有一个NPC对话");

                return result;
            }

            var type = cardInfo.SelectType;

            var cmd = cardInfo.SelectCmd;

            var par = cardInfo.SelectParameter;

            var item = cardInfo.SelectItem;

            var realCmd = GetRealCmd(type);

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
                result.SetError("添加失败");

                return result;
            }
        }

        public async Task<ReturnModel> ItemRemove()
        {
            var result = new ReturnModel();

            if (cardInfo.Type == Model.MainWindow.ThumbClass.Player && cardInfo.SelectType == "存储对话: pointer")
            {
                result.SetError("玩家对话的节点下的NPC对话不能删除");

                return result;
            }

            var type = cardInfo.SelectType;

            var cmd = cardInfo.SelectCmd;

            var par = cardInfo.SelectParameter;

            var item = cardInfo.SelectItem;

            var realCmd = GetRealCmd(type);

            if (saveAllValue[cardInfo][type][cmd][par].Count < 2)
            {
                result.SetSuccese("仅剩一个项不允许被删除！");

                return result;
            }

            saveAllValue[cardInfo][type][cmd][par].Remove(item);

            cardInfo.AllItem.Remove(item);

            result.SetSuccese("删除成功！");

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
