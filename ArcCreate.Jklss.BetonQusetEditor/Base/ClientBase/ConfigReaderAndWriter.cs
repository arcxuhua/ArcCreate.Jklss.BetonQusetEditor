using ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest;
using ArcCreate.Jklss.BetonQusetEditor.ViewModel.MainWindows;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.SocketModel;
using ArcCreate.Jklss.Model.ThumbModel;
using ArcCreate.Jklss.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ArcCreate.Jklss.BetonQusetEditor.Base.ClientBase
{
    public class ConfigReaderAndWriter
    {
        private Dictionary<AnyCardViewModel, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> conditionSave;

        private Dictionary<AnyCardViewModel, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> eventSave;

        private Dictionary<AnyCardViewModel, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> objectiveSave;

        private Dictionary<AnyCardViewModel, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> conversationSave;

        private Dictionary<AnyCardViewModel, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> allSave = 
            new Dictionary<AnyCardViewModel, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>();

        private List<CardViewModel> allCard = new List<CardViewModel>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="conditionSave"></param>
        /// <param name="eventSave"></param>
        /// <param name="objectiveSave"></param>
        /// <param name="conversationSave"></param>
        /// <param name="mainCard"></param>
        public ConfigReaderAndWriter(Dictionary<AnyCardViewModel, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> conditionSave,
            Dictionary<AnyCardViewModel, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> eventSave,
            Dictionary<AnyCardViewModel, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> objectiveSave,
            Dictionary<AnyCardViewModel, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> conversationSave,
            ObservableCollection<CardViewModel> allCard)
        {
            var delCondtion = new List<(AnyCardViewModel a, string b,string c,string d,string e)>();

            foreach (var item in conditionSave)//审查有没有空的项，有就删除
            {
                foreach (var i in item.Value)
                {
                    foreach (var j in i.Value)
                    {
                        foreach (var m in j.Value)
                        {
                            foreach (var n in m.Value)
                            {
                                if (string.IsNullOrWhiteSpace(n.Value))
                                {
                                    delCondtion.Add((item.Key, i.Key, j.Key, m.Key, n.Key));

                                }
                            }
                        }
                    }
                }
            }

            foreach (var item in delCondtion)
            {
                conditionSave[item.a][item.b][item.c][item.d].Remove(item.e);
            }

            delCondtion.Clear();

            foreach (var item in eventSave)//审查有没有空的项，有就删除
            {
                foreach (var i in item.Value)
                {
                    foreach (var j in i.Value)
                    {
                        foreach (var m in j.Value)
                        {
                            foreach (var n in m.Value)
                            {
                                if (string.IsNullOrWhiteSpace(n.Value))
                                {
                                    delCondtion.Add((item.Key, i.Key, j.Key, m.Key, n.Key));

                                }
                            }
                        }
                    }
                }
            }

            foreach (var item in delCondtion)
            {
                eventSave[item.a][item.b][item.c][item.d].Remove(item.e);
            }

            delCondtion.Clear();

            foreach (var item in objectiveSave)//审查有没有空的项，有就删除
            {
                foreach (var i in item.Value)
                {
                    foreach (var j in i.Value)
                    {
                        foreach (var m in j.Value)
                        {
                            foreach (var n in m.Value)
                            {
                                if (string.IsNullOrWhiteSpace(n.Value))
                                {
                                    delCondtion.Add((item.Key, i.Key, j.Key, m.Key, n.Key));

                                }
                            }
                        }
                    }
                }
            }

            foreach (var item in delCondtion)
            {
                objectiveSave[item.a][item.b][item.c][item.d].Remove(item.e);
            }

            delCondtion.Clear();

            foreach (var item in conversationSave)//审查有没有空的项，有就删除
            {
                foreach (var i in item.Value)
                {
                    foreach (var j in i.Value)
                    {
                        foreach (var m in j.Value)
                        {
                            foreach (var n in m.Value)
                            {
                                if (string.IsNullOrWhiteSpace(n.Value))
                                {
                                    delCondtion.Add((item.Key, i.Key, j.Key, m.Key, n.Key));

                                }
                            }
                        }
                    }
                }
            }

            foreach (var item in delCondtion)
            {
                conversationSave[item.a][item.b][item.c][item.d].Remove(item.e);
            }

            delCondtion.Clear();

            foreach (var item in allCard)
            {
                this.allCard.Add(item);
            }

            this.conversationSave = conversationSave;

            this.objectiveSave = objectiveSave;

            this.eventSave = eventSave;

            this.conditionSave = conditionSave;

            foreach (var item in this.conditionSave)
            {
                this.allSave.Add(item.Key,item.Value);
            }

            foreach (var item in this.eventSave)
            {
                this.allSave.Add(item.Key, item.Value);
            }

            foreach (var item in this.objectiveSave)
            {
                this.allSave.Add(item.Key, item.Value);
            }

            foreach (var item in this.conversationSave)
            {
                this.allSave.Add(item.Key, item.Value);
            }

            
        }

        /// <summary>
        /// 新版本的创建Json
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnModel> SaveToJson(string filePath, string txt, int id = -1, bool isShowMessage = false)
        {
            var result = new ReturnModel();

            var saveMainInfo = new List<ThumbsModels>();

            var saveAllChildInfo = new List<SaveChilds>();

            try
            {
                if (allCard.Where(t => t.Type == ThumbClass.Subject).Any())
                {
                    var getMainThumb = allCard.Where(t => t.Type == ThumbClass.Subject&&!t.IsLine&&!t.IsDraw).ToList();

                    await Task.Run(() =>
                    {
                        foreach (var item in getMainThumb)
                        {
                            var getRealCard = item as SubjectCardViewModel;

                            saveMainInfo.Add(new ThumbsModels(getRealCard.ConfigName, getRealCard.NPC_ID,getRealCard.ItemContent));

                            var childInfo = new SaveChilds()
                            {
                                Saver = getRealCard.ConfigName,
                                Main = getRealCard.ConfigName,
                            };

                            childInfo.CanFather = false;
                            childInfo.thumbClass = item.Type;

                            foreach (var i in item.Right)
                            {
                                childInfo.Children.Add(i.ConfigName);
                            }

                            foreach (var i in item.Left)
                            {
                                childInfo.Fathers.Add(i.ConfigName);
                            }
                            saveAllChildInfo.Add(childInfo);
                        }
                    });
                }
            }
            catch
            {
                result.SetError("构造器错误，错误码:JS001");

                return result;
            }

            var mainJson = FileService.SaveToJson(saveMainInfo);//转换为Json

            var allChildInfo = FileService.SaveToJson(saveAllChildInfo);//转换为Json

            var saveNPCEOInfo = new List<ThumbInfoModel>();

            try
            {
                await Task.Run(() =>
                {
                    foreach (var item in allSave)
                    {
                        var name = item.Key.ConfigName;

                        var getMainName = item.Key.MainCard.ConfigName;

                        saveNPCEOInfo.Add(new ThumbInfoModel() { Main = getMainName, Name = name, thumbClass = item.Key.Type, data = item.Value });
                    }

                    //将没有归类的也放入数据
                    var getThumb = allCard.Where(t => t.Type != ThumbClass.Subject && t.Type != ThumbClass.Journal && t.Type != ThumbClass.Items).ToList();

                    foreach (var item in getThumb)
                    {
                        if (!allSave.ContainsKey(item as AnyCardViewModel))
                        {
                            var name = item.ConfigName;

                            saveNPCEOInfo.Add(new ThumbInfoModel() { Name = name, thumbClass = item.Type });
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                result.SetError("构造器错误，错误码:JS002");

                return result;
            }

            var npceoJson = FileService.SaveToJson(saveNPCEOInfo);//转换为Json

            var saveJournalInfo = new List<ThumbsModels>();

            try
            {
                if (allCard.Where(t => t.Type == ThumbClass.Journal).Any())
                {
                    var getMainThumb = allCard.Where(t => t.Type == ThumbClass.Journal).ToList();

                    await Task.Run(() =>
                    {
                        foreach (var item in getMainThumb)
                        {
                            saveJournalInfo.Add(new ThumbsModels(item.ConfigName,"",item.ItemContent));
                        }
                    });
                }
            }
            catch
            {
                result.SetError("构造器错误，错误码:JS003");

                return result;
            }

            var journalJson = FileService.SaveToJson(saveJournalInfo);//转换为Json

            var saveItemsInfo = new List<ThumbsModels>();

            try
            {
                if (allCard.Where(t => t.Type == ThumbClass.Items).Any())
                {
                    var getMainThumb = allCard.Where(t => t.Type == ThumbClass.Items).ToList();

                    await Task.Run(() =>
                    {
                        foreach (var item in getMainThumb)
                        {
                            saveItemsInfo.Add(new ThumbsModels(item.ConfigName,"",item.ItemContent));
                        }
                    });
                }
            }
            catch
            {
                result.SetError("构造器错误，错误码:JS004");

                return result;
            }

            var itemsJson = FileService.SaveToJson(saveItemsInfo);//转换为Json

            var dic = new List<ThumbCoordinateModel>();

            try
            {
                foreach (var item in allCard.Where(t=>!t.IsDraw&&!t.IsLine).ToList())
                {
                    var getX = item.CvLeft;

                    var getY = item.CvTop;

                    dic.Add(new ThumbCoordinateModel() { Name = item.ConfigName, thumbClass = item.Type, X = getX, Y = getY });
                }
            }
            catch
            {
                result.SetError("构造器错误，错误码:JS005");

                return result;
            }

            var coordinateJson = FileService.SaveToJson(dic);//转换为Json

            var newToolList = new List<HelpToolModel>();

            try
            {
                await Task.Run(() =>
                {
                    foreach (var item in allCard.Where(t => !t.IsDraw && !t.IsLine).ToList())
                    {
                        newToolList.Add(new HelpToolModel
                        {
                            Class = item.Type,
                            Name = item.ConfigName,
                            Tool = item.HelpInfo,
                        });
                    }
                });


            }
            catch
            {
                result.SetError("构造器错误，错误码:JS006");

                return result;
            }

            var toolListJson = FileService.SaveToJson(newToolList);

            var path = new SaveJsonModel()
            {
                id = id,
                text = txt,
                Main = mainJson,
                Maininfo = allChildInfo,
                Data = npceoJson,
                Jdata = journalJson,
                Idata = itemsJson,
                Coordinate = coordinateJson,
                filepath = filePath,
                HelpTool = toolListJson,
            };

            if (isShowMessage)
            {
                var points = saveMainInfo.Count + saveNPCEOInfo.Count + saveJournalInfo.Count + saveItemsInfo.Count;

                if (MessageBox.Show($"你确定要花费积分：{points} 来生成YML文件？\n如果是请选择是，这将是不可逆的操作！", "警告", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    result.SetError();

                    return result;
                }
            }

            result.SetSuccese("配置生成成功", path);

            return result;
        }

        /// <summary>
        /// Condition语法解析器
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public async Task<ReturnModel> ConditionAnalysis(string txt)
        {
            var result = new ReturnModel();

            if (string.IsNullOrEmpty(txt))
            {
                result.SetError("");

                return result;
            }

            var message = new MessageModel()
            {
                IsLogin = SocketModel.isLogin,
                JsonInfo = JsonInfo.ConditionAnalysis,
                UserName = SocketModel.userName,
                Message = txt,
            };

            var jsonMessage = FileService.SaveToJson(message);

            var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);

            if (getMessage == null || !getMessage.Succese)
            {
                result.SetError("");

                return result;
            }

            var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

            if (getModel.Token != SocketModel.token)
            {
                result.SetError("");

                return result;
            }

            var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

            if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.ConditionAnalysis || !getRealMessage.IsLogin)
            {
                result.SetError("");

                return result;
            }

            result.SetSuccese("", FileService.JsonToProp<Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>(getRealMessage.Message));

            return result;
        }

        /// <summary>
        /// Event语法解析器
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public async Task<ReturnModel> EventAnalysis(string txt)
        {
            var result = new ReturnModel();
            if (string.IsNullOrEmpty(txt))
            {
                result.SetError("");

                return result;
            }
            var message = new MessageModel()
            {
                IsLogin = SocketModel.isLogin,
                JsonInfo = JsonInfo.EventAnalysis,
                UserName = SocketModel.userName,
                Message = txt,
            };

            var jsonMessage = FileService.SaveToJson(message);

            var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);

            if (getMessage == null || !getMessage.Succese)
            {
                result.SetError("");

                return result;
            }

            var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

            if (getModel.Token != SocketModel.token)
            {
                result.SetError("");

                return result;
            }

            var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

            if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.EventAnalysis || !getRealMessage.IsLogin)
            {
                result.SetError("");

                return result;
            }

            result.SetSuccese("", FileService.JsonToProp<Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>(getRealMessage.Message));

            return result;
        }

        /// <summary>
        /// Objective语法解析器
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public async Task<ReturnModel> ObjectiveAnalysis(string txt)
        {
            var result = new ReturnModel();
            if (string.IsNullOrEmpty(txt))
            {
                result.SetError("");

                return result;
            }
            var message = new MessageModel()
            {
                IsLogin = SocketModel.isLogin,
                JsonInfo = JsonInfo.ObjectiveAnalysis,
                UserName = SocketModel.userName,
                Message = txt,
            };

            var jsonMessage = FileService.SaveToJson(message);

            var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);

            if (getMessage == null || !getMessage.Succese)
            {
                result.SetError("");

                return result;
            }

            var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

            if (getModel.Token != SocketModel.token)
            {
                result.SetError("");

                return result;
            }

            var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

            if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.ObjectiveAnalysis || !getRealMessage.IsLogin)
            {
                result.SetError("");

                return result;
            }

            result.SetSuccese("", FileService.JsonToProp<Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>(getRealMessage.Message));

            return result;
        }

        /// <summary>
        /// Player与Npc语法解析器
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public async Task<ReturnModel> PlayerAndNpcAnalysis(string txt)
        {
            var result = new ReturnModel();
            if (string.IsNullOrEmpty(txt))
            {
                result.SetError("");

                return result;
            }
            var message = new MessageModel()
            {
                IsLogin = SocketModel.isLogin,
                JsonInfo = JsonInfo.PlayerAndNpcAnalysis,
                UserName = SocketModel.userName,
                Message = txt,
            };

            var jsonMessage = FileService.SaveToJson(message);

            var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage,
                SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(), SocketModel.token, true);

            if (getMessage == null || !getMessage.Succese)
            {
                result.SetError("");

                return result;
            }

            var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

            if (getModel.Token != SocketModel.token)
            {
                result.SetError("");

                return result;
            }

            var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

            if (getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.PlayerAndNpcAnalysis || !getRealMessage.IsLogin)
            {
                result.SetError("");

                return result;
            }

            result.SetSuccese("", FileService.JsonToProp<Dictionary<string, Dictionary<string, string>>>(getRealMessage.Message));

            return result;
        }

        /// <summary>
        /// 输出为Yaml
        /// </summary>
        /// <param name="getBacks"></param>
        /// <returns></returns>
        public async Task<ReturnModel> InputToYaml(YamlSaver getBacks,string filePath)
        {
            var result = new ReturnModel();

            try
            {
                var createAllTalk = new List<string>();

                await Task.Run(() =>
                {
                    var disPath = FileService.GetFileDirectory(filePath);

                    foreach (var item in getBacks.Conversations)
                    {
                        FileService.ChangeFile(disPath + @"\conversations\" + item.Key, item.Value); //输出对话
                    }
                    FileService.ChangeFile(filePath, getBacks.Main); //输出Main

                    FileService.ChangeFile(disPath + @"\conditions.yml", getBacks.Conditions); //输出条件

                    FileService.ChangeFile(disPath + @"\events.yml", getBacks.Events); //输出事件

                    FileService.ChangeFile(disPath + @"\items.yml", getBacks.Items); //输出物品

                    FileService.ChangeFile(disPath + @"\journal.yml", getBacks.Journal); //输出日记

                    FileService.ChangeFile(disPath + @"\objectives.yml", getBacks.Objectives); //输出目标
                });
                result.SetSuccese("输出至本地成功");
            }
            catch
            {
                result.SetError("输出至本地错误");
            }

            return result;
        }
    }

    public class ThumbInfoModel
    {
        public string Name { get; set; }

        public string Main { get; set; }

        public ThumbClass thumbClass { get; set; }

        public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> data { get; set; }
    }

    public class ThumbCoordinateModel
    {
        public string Name { get; set; }

        public ThumbClass thumbClass { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
    }

    public class SaveChilds
    {
        public string Saver { get; set; }

        public List<string> Children { get; set; } = new List<string>();

        public List<string> Fathers { get; set; } = new List<string>();

        public bool CanFather { get; set; }

        public string Main { get; set; }

        public ThumbClass thumbClass { get; set; }
    }

    public class HelpToolModel
    {
        public string Name { get; set; }

        public ThumbClass Class { get; set; }

        public string Tool { get; set; }
    }
}
