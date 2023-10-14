using ArcCreate.Jklss.BetonQusetEditor.Base.ClientBase;
using ArcCreate.Jklss.BetonQusetEditor.ViewModel.MainWindows;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.Data;
using ArcCreate.Jklss.Model.MainWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace ArcCreate.Jklss.BetonQusetEditor.Base.DataBase
{
    public class DataCheckBase
    {
        private List<CardViewModel> cards;

        private SaveJsonModel getJsons;

        public DataCheckBase(List<CardViewModel> cards = null, SaveJsonModel getJsons = null)
        {
            this.getJsons = getJsons;
            this.cards = cards;
        }

        public async Task<ReturnModel> CheckCard()
        {
            var result = new ReturnModel();

            var listInfo = new List<DataCheckInfoModel>();

            //卡片的基础检测
            var getFoundationBack = await CheckCardFoundation();

            if (!getFoundationBack.Succese)
            {
                listInfo.AddRange(getFoundationBack.Backs as  List<DataCheckInfoModel>);
            }

            //对话卡片的MainCard归属检测
            var getTalkBack = await CheckTalkCardMain();

            if(!getTalkBack.Succese)
            {
                listInfo.AddRange(getTalkBack.Backs as List<DataCheckInfoModel>);
            }

            //卡片的数据检测
            var getDataBack = await CheckCardData();

            if(!getDataBack.Succese)
            {
                listInfo.AddRange(getDataBack.Backs as List<DataCheckInfoModel>);
            }

            if (listInfo.Count > 0)
            {
                result.SetError($"有{listInfo.Count} 个错误请检查！", listInfo);

                return result;
            }

            result.SetSuccese();

            return result;
        }

        public async Task<ReturnModel> CheckJsons()
        {
            var result = new ReturnModel();

            await Task.Run(() =>
            {

            });

            result.SetSuccese();

            return result;
        }
        
        /// <summary>
        /// 卡片检测--->基本检测
        /// </summary>
        /// <returns></returns>
        private async Task<ReturnModel> CheckCardFoundation()
        {
            var result = new ReturnModel();

            if(cards == null || cards.Count <= 1)
            {
                result.SetSuccese();

                return result;
            }

            var badinfo = new List<DataCheckInfoModel>();

            await Task.Run(() => 
            {
                foreach (var item in cards)
                {
                    //卡片配置名称为空
                    if (string.IsNullOrEmpty(item.ConfigName))
                    {
                        var info = new DataCheckInfoModel()
                        {
                            CardName = item.ConfigName,
                            CardClass = item.Type,
                            CheckInfoLevel = CheckInfoLevel.error,
                            Message = "卡片未填写名称"
                        };

                        badinfo.Add(info);
                    }

                    //卡片位置超出范围
                    if(item.CvTop < 0.00||item.CvTop > 509800 || item.CvLeft <0.00 || item.CvLeft> 1277400)
                    {
                        var info = new DataCheckInfoModel()
                        {
                            CardName = item.ConfigName,
                            CardClass = item.Type,
                            CheckInfoLevel = CheckInfoLevel.worry,
                            Message = "卡片超出画布范围"
                        };

                        badinfo.Add(info);
                    }

                    var getlist = cards.Where(t => t.ConfigName == item.ConfigName&&!string.IsNullOrEmpty(item.ConfigName)).ToList();
                    
                    //卡片名称重叠
                    if (getlist.Count > 1)
                    {
                        var info = new DataCheckInfoModel()
                        {
                            CardName = item.ConfigName,
                            CardClass = item.Type,
                            CheckInfoLevel = CheckInfoLevel.error,
                            Message = $"有{getlist.Count}张卡片名称重叠"
                        };

                        badinfo.Add(info);
                    }
                }
            });

            if(badinfo.Count > 0)
            {
                result.SetError("worry", badinfo);

                return result;
            }

            result.SetSuccese();

            return result;
        }

        /// <summary>
        /// 卡片检测--->MainCard归类检测
        /// </summary>
        /// <returns></returns>
        private async Task<ReturnModel> CheckTalkCardMain()
        {
            var result = new ReturnModel();

            if (cards == null || cards.Count <= 1)
            {
                result.SetSuccese();

                return result;
            }

            var badinfo = new List<DataCheckInfoModel>();

            await Task.Run(async () =>
            {
                var getMainList = cards.Where(t => t.Type == ThumbClass.Subject).ToList();

                for (int i = 0; i < getMainList.Count; i++)
                {
                    for (int j = 0; j < getMainList[i].Right.Count; j++)//避免递归导致的堆溢出问题
                    {
                        if (getMainList[i].Right[j].Type != ThumbClass.NPC)//检查主卡片下的卡片是否为NPC对话
                        {
                            var info = new DataCheckInfoModel()
                            {
                                CardName = getMainList[i].Right[j].ConfigName,
                                CardClass = getMainList[i].Right[j].Type,
                                CheckInfoLevel = CheckInfoLevel.worry,
                                Message = "对话主体卡片的子卡片必须为NPC对话卡片，请删除该卡片"
                            };

                            badinfo.Add(info);
                        }

                        if (getMainList[i].Right[j].MainCard != getMainList[i])//二级卡片错误
                        {
                            var info = new DataCheckInfoModel()
                            {
                                CardName = getMainList[i].Right[j].ConfigName,
                                CardClass = getMainList[i].Right[j].Type,
                                CheckInfoLevel = CheckInfoLevel.error,
                                Message = "子卡片总附属发生错误",
                                Backs = getMainList[i]
                            };

                            badinfo.Add(info);
                        }

                        var getThreeNeedCheckCard = getMainList[i].Right[j].Right.Where(t=>t.Type == ThumbClass.Player||t.Type == ThumbClass.NPC).ToList();

                        if(getThreeNeedCheckCard == null)
                        {
                            continue;
                        }

                        for (int n = 0; n < getThreeNeedCheckCard.Count; n++)//三级卡片的错误查询
                        {
                            var getCheckBack = await CheckMainCard(getMainList[i], getMainList[i].Right[j].Right[n]);

                            if (!getCheckBack.Succese)
                            {
                                badinfo.AddRange(getCheckBack.Backs as List<DataCheckInfoModel>);
                            }
                        }
                    }
                }
            });

            result.SetSuccese();

            return result;
        }

        /// <summary>
        /// 卡片检测--->基础数据检测
        /// </summary>
        /// <returns></returns>
        private async Task<ReturnModel> CheckCardData()
        {
            var result = new ReturnModel();

            if (cards == null || cards.Count <= 1)
            {
                result.SetSuccese();

                return result;
            }

            var badinfo = new List<DataCheckInfoModel>();

            var getConditions = ConditionsCardChanged.saveAllValue;

            var getEvents = EventCardChanged.saveAllValue;

            var getObjectives = ObjectiveCardChanged.saveAllValue;

            var getConversations = ConversationCardChanged.saveAllValue;

            await Task.Run(() =>
            {
                var getNeedCheckPlayerCard = cards.Where(t => t.Type == ThumbClass.Player).ToList();

                var getNeedCheckNPCCard = cards.Where(t => t.Type == ThumbClass.NPC).ToList();

                var getNeedCheckCondtionCard = cards.Where(t=>t.Type == ThumbClass.Conditions).ToList();

                var getNeedCheckEventCard = cards.Where(t => t.Type == ThumbClass.Events).ToList();

                var getNeedCheckobjectiveCard = cards.Where(t => t.Type == ThumbClass.Objectives).ToList();

                var tasks = new List<Task>();

                tasks.Add(Task.Run(() =>
                {
                    foreach (var item in getNeedCheckCondtionCard)
                    {
                        var realItem = item as AnyCardViewModel;

                        if (string.IsNullOrEmpty(realItem.SelectType))
                        {
                            var worryMessage = new DataCheckInfoModel()
                            {
                                CardName = realItem.ConfigName,
                                CardClass = realItem.Type,
                                CheckInfoLevel = CheckInfoLevel.error,
                                Message = "卡片条件类型（主页）必须选择"
                            };

                            badinfo.Add(worryMessage);

                            continue;

                        }

                        if (!getConditions.ContainsKey(realItem))
                        {
                            var worryMessage = new DataCheckInfoModel()
                            {
                                CardName = realItem.ConfigName,
                                CardClass = realItem.Type,
                                CheckInfoLevel = CheckInfoLevel.worry,
                                Message = "卡片存储数据失败，请尝试重新选择条件类型(主页)"
                            };

                            badinfo.Add(worryMessage);

                            continue;
                        }

                        var isHaveData = false;

                        foreach (var i in getConditions[realItem])
                        {
                            foreach (var j in i.Value)
                            {
                                foreach (var m in j.Value)
                                {
                                    foreach (var n in m.Value)
                                    {
                                        if (!string.IsNullOrEmpty(n.Value))
                                        {
                                            isHaveData = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (!isHaveData)
                        {
                            var worryMessage = new DataCheckInfoModel()
                            {
                                CardName = realItem.ConfigName,
                                CardClass = realItem.Type,
                                CheckInfoLevel = CheckInfoLevel.worry,
                                Message = "卡片未填写任何数据，请删除卡片或填写数据"
                            };

                            badinfo.Add(worryMessage);

                            continue;
                        }
                    }
                    
                }));

                tasks.Add(Task.Run(() =>
                {
                    foreach (var item in getNeedCheckEventCard)
                    {
                        var realItem = item as AnyCardViewModel;

                        if (string.IsNullOrEmpty(realItem.SelectType))
                        {
                            var worryMessage = new DataCheckInfoModel()
                            {
                                CardName = realItem.ConfigName,
                                CardClass = realItem.Type,
                                CheckInfoLevel = CheckInfoLevel.error,
                                Message = "卡片事件类型（主页）必须选择"
                            };

                            badinfo.Add(worryMessage);

                            continue;

                        }

                        if (!getEvents.ContainsKey(realItem))
                        {
                            var worryMessage = new DataCheckInfoModel()
                            {
                                CardName = realItem.ConfigName,
                                CardClass = realItem.Type,
                                CheckInfoLevel = CheckInfoLevel.worry,
                                Message = "卡片存储数据失败，请尝试重新选择事件类型(主页)"
                            };

                            badinfo.Add(worryMessage);

                            continue;
                        }

                        var isHaveData = false;

                        foreach (var i in getEvents[realItem])
                        {
                            foreach (var j in i.Value)
                            {
                                foreach (var m in j.Value)
                                {
                                    foreach (var n in m.Value)
                                    {
                                        if (!string.IsNullOrEmpty(n.Value))
                                        {
                                            isHaveData = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (!isHaveData)
                        {
                            var worryMessage = new DataCheckInfoModel()
                            {
                                CardName = realItem.ConfigName,
                                CardClass = realItem.Type,
                                CheckInfoLevel = CheckInfoLevel.worry,
                                Message = "卡片未填写任何数据，请删除卡片或填写数据"
                            };

                            badinfo.Add(worryMessage);

                            continue;
                        }
                    }
                    
                }));

                tasks.Add(Task.Run(() =>
                {
                    foreach (var item in getNeedCheckobjectiveCard)
                    {
                        var realItem = item as AnyCardViewModel;

                        if (string.IsNullOrEmpty(realItem.SelectType))
                        {
                            var worryMessage = new DataCheckInfoModel()
                            {
                                CardName = realItem.ConfigName,
                                CardClass = realItem.Type,
                                CheckInfoLevel = CheckInfoLevel.error,
                                Message = "卡片事件类型（主页）必须选择"
                            };

                            badinfo.Add(worryMessage);

                            continue;

                        }

                        if (!getObjectives.ContainsKey(realItem))
                        {
                            var worryMessage = new DataCheckInfoModel()
                            {
                                CardName = realItem.ConfigName,
                                CardClass = realItem.Type,
                                CheckInfoLevel = CheckInfoLevel.worry,
                                Message = "卡片存储数据失败，请尝试重新选择事件类型(主页)"
                            };

                            badinfo.Add(worryMessage);

                            continue;
                        }

                        var isHaveData = false;

                        foreach (var i in getObjectives[realItem])
                        {
                            foreach (var j in i.Value)
                            {
                                foreach (var m in j.Value)
                                {
                                    foreach (var n in m.Value)
                                    {
                                        if (!string.IsNullOrEmpty(n.Value))
                                        {
                                            isHaveData = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (!isHaveData)
                        {
                            var worryMessage = new DataCheckInfoModel()
                            {
                                CardName = realItem.ConfigName,
                                CardClass = realItem.Type,
                                CheckInfoLevel = CheckInfoLevel.worry,
                                Message = "卡片未填写任何数据，请删除卡片或填写数据"
                            };

                            badinfo.Add(worryMessage);

                            continue;
                        }
                    }
                    
                }));

                tasks.Add(Task.Run(() =>
                {
                    foreach (var item in getNeedCheckPlayerCard)
                    {
                        var realItem = item as AnyCardViewModel;

                        if (!getConversations.ContainsKey(realItem))
                        {
                            var worryMessage = new DataCheckInfoModel()
                            {
                                CardName = realItem.ConfigName,
                                CardClass = realItem.Type,
                                CheckInfoLevel = CheckInfoLevel.worry,
                                Message = "卡片存储数据失败，请尝试删除并重新生成该卡片"
                            };

                            badinfo.Add(worryMessage);

                            continue;
                        }

                        try
                        {
                            var getTalk = getConversations[realItem]["文案: text"]["text"]["第 1 条参数"];

                            if (getTalk.Count <= 0)
                            {
                                var worryMessage = new DataCheckInfoModel()
                                {
                                    CardName = realItem.ConfigName,
                                    CardClass = realItem.Type,
                                    CheckInfoLevel = CheckInfoLevel.error,
                                    Message = "玩家对话卡片请至少保存一条对话！"
                                };

                                badinfo.Add(worryMessage);

                                continue;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(getTalk.FirstOrDefault().Value))
                                {
                                    var worryMessage = new DataCheckInfoModel()
                                    {
                                        CardName = realItem.ConfigName,
                                        CardClass = realItem.Type,
                                        CheckInfoLevel = CheckInfoLevel.error,
                                        Message = "玩家对话卡片首项对话请保存！"
                                    };

                                    badinfo.Add(worryMessage);

                                    continue;
                                }
                            }
                        }
                        catch
                        {
                            var worryMessage = new DataCheckInfoModel()
                            {
                                CardName = realItem.ConfigName,
                                CardClass = realItem.Type,
                                CheckInfoLevel = CheckInfoLevel.worry,
                                Message = "玩家对话卡片数据异常，请尝试删除后重新生成！"
                            };

                            badinfo.Add(worryMessage);

                            continue;
                        }

                        if (realItem.Left.Count <= 0)
                        {
                            var worryMessage = new DataCheckInfoModel()
                            {
                                CardName = realItem.ConfigName,
                                CardClass = realItem.Type,
                                CheckInfoLevel = CheckInfoLevel.worry,
                                Message = "该卡片至少需要一个父级与其相连"
                            };

                            badinfo.Add(worryMessage);

                            continue;
                        }
                    }
                    
                }));

                tasks.Add(Task.Run(() =>
                {
                    foreach (var item in getNeedCheckNPCCard)
                    {
                        var realItem = item as AnyCardViewModel;

                        if (!getConversations.ContainsKey(realItem))
                        {
                            var worryMessage = new DataCheckInfoModel()
                            {
                                CardName = realItem.ConfigName,
                                CardClass = realItem.Type,
                                CheckInfoLevel = CheckInfoLevel.worry,
                                Message = "卡片存储数据失败，请尝试删除并重新生成该卡片"
                            };

                            badinfo.Add(worryMessage);

                            continue;
                        }

                        try
                        {
                            var getTalk = getConversations[realItem]["文案: text"]["text"]["第 1 条参数"];

                            if (getTalk.Count <= 0)
                            {
                                var worryMessage = new DataCheckInfoModel()
                                {
                                    CardName = realItem.ConfigName,
                                    CardClass = realItem.Type,
                                    CheckInfoLevel = CheckInfoLevel.error,
                                    Message = "NPC对话卡片请至少保存一条对话！"
                                };

                                badinfo.Add(worryMessage);

                                continue;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(getTalk.FirstOrDefault().Value))
                                {
                                    var worryMessage = new DataCheckInfoModel()
                                    {
                                        CardName = realItem.ConfigName,
                                        CardClass = realItem.Type,
                                        CheckInfoLevel = CheckInfoLevel.error,
                                        Message = "NPC对话卡片首项对话请保存！"
                                    };

                                    badinfo.Add(worryMessage);

                                    continue;
                                }
                            }
                        }
                        catch
                        {
                            var worryMessage = new DataCheckInfoModel()
                            {
                                CardName = realItem.ConfigName,
                                CardClass = realItem.Type,
                                CheckInfoLevel = CheckInfoLevel.worry,
                                Message = "NPC对话卡片数据异常，请尝试删除后重新生成！"
                            };

                            badinfo.Add(worryMessage);

                            continue;
                        }

                        if (realItem.Left.Count <= 0)
                        {
                            var worryMessage = new DataCheckInfoModel()
                            {
                                CardName = realItem.ConfigName,
                                CardClass = realItem.Type,
                                CheckInfoLevel = CheckInfoLevel.worry,
                                Message = "该卡片至少需要一个父级与其相连"
                            };

                            badinfo.Add(worryMessage);

                            continue;
                        }
                    }
                    
                }));

                while (!(tasks[0].Status ==TaskStatus.RanToCompletion) && 
                !(tasks[1].Status == TaskStatus.RanToCompletion) && 
                !(tasks[2].Status == TaskStatus.RanToCompletion) && 
                !(tasks[3].Status == TaskStatus.RanToCompletion) && 
                !(tasks[4].Status == TaskStatus.RanToCompletion))
                {
                    Thread.Sleep(100);
                }
            });

            if (badinfo.Count > 0)
            {
                result.SetError("worry", badinfo);

                return result;
            }

            result.SetSuccese();

            return result;
        }

        /// <summary>
        /// 递归查询各卡片MainCard是否正确
        /// </summary>
        /// <param name="mainCard"></param>
        /// <param name="checkCard"></param>
        /// <returns></returns>
        private async Task<ReturnModel> CheckMainCard(CardViewModel mainCard,CardViewModel checkCard)
        {
            var result = new ReturnModel();

            var badinfo = new List<DataCheckInfoModel>();

            await Task.Run(async () =>
            {
                if (checkCard.MainCard != mainCard)
                {
                    var info = new DataCheckInfoModel()
                    {
                        CardName = checkCard.ConfigName,
                        CardClass = checkCard.Type,
                        CheckInfoLevel = CheckInfoLevel.error,
                        Message = "子卡片总附属发生错误",
                        Backs = mainCard
                    };

                    badinfo.Add(info);
                }

                if(checkCard.Right != null && checkCard.Right.Count > 0)
                {
                    var getNeedCheckCard = checkCard.Right.Where(t=>t.Type == ThumbClass.Player||t.Type == ThumbClass.NPC).ToList();

                    if(getNeedCheckCard == null)
                    {
                        return;
                    }

                    for (int i = 0; i < getNeedCheckCard.Count; i++)
                    {
                        var getBack = await CheckMainCard(mainCard, getNeedCheckCard[i]);

                        if (!getBack.Succese)
                        {
                            badinfo.AddRange(getBack.Backs as  List<DataCheckInfoModel>);
                        }
                    }
                    
                }

            });

            if(badinfo.Count > 0)
            {
                result.SetError("", badinfo);

                return result;
            }
            
            result.SetSuccese();

            return result;
        }

    }
}
