using ArcCreate.Jklss.BetonQusetEditor.ViewModel;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.Data;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.SocketModel;
using ArcCreate.Jklss.Model.ThumbModel;
using ArcCreate.Jklss.Model.ThumbModel.CommandModel;
using ArcCreate.Jklss.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using static ArcCreate.Jklss.Model.SocketModel.SocketModel;

namespace ArcCreate.Jklss.BetonQusetEditor.Base
{
    public class SaveAndReadYamlBase
    {
        private string filePath = null;

        private Dictionary<Thumb, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> saveInfo = null;

        private List<SaveChird> saveThumbs = null;

        private List<ContisionsCmdModel> contisionProp = null;//Contitions语法构造器模型

        private List<EventCmdModel> eventProp = null;

        private List<ObjectiveCmdModel> objectiveProp = null;

        private Dictionary<Thumb, string> saveHelpTool;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="objectiveProp"></param>
        /// <param name="eventProp"></param>
        /// <param name="contisionProp"></param>
        /// <param name="saveThumbs"></param>
        /// <param name="saveInfo"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SaveAndReadYamlBase(string filePath, List<ObjectiveCmdModel> objectiveProp, 
            List<EventCmdModel> eventProp, List<ContisionsCmdModel> contisionProp, List<SaveChird> saveThumbs,
            Dictionary<Thumb, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> saveInfo, Dictionary<Thumb, string> saveHelpTool)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("文件地址为空");
            }

            if (objectiveProp == null)
            {
                throw new ArgumentNullException("objective的语法构造器模型为null");
            }

            if (eventProp == null)
            {
                throw new ArgumentNullException("evevt的语法构造器模型为null");
            }

            if (contisionProp == null)
            {
                throw new ArgumentNullException("conditions的语法构造器模型为null");
            }

            if (saveThumbs == null)
            {
                throw new ArgumentNullException("Thumb存储器为null");
            }

            if (saveInfo == null)
            {
                throw new ArgumentNullException("数据存储器为null");
            }

            this.filePath = filePath;

            this.objectiveProp = objectiveProp;

            this.eventProp = eventProp;

            this.contisionProp = contisionProp;

            this.saveThumbs = saveThumbs;

            this.saveInfo = saveInfo;

            this.saveHelpTool = saveHelpTool;
        }

        /// <summary>
        /// 保存为Json[不需要迁移]
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnModel> SaveToJson(string txt ,int id = -1)
        {
            var result = new ReturnModel();

            var saveMainInfo = new List<ThumbsModels>();

            var saveAllChildInfo = new List<SaveChilds>();

            try
            {
                if (MainWindowModels.saveThumbs.Where(t => t.thumbClass == ThumbClass.Subject).Any())
                {
                    var getMainThumb = MainWindowModels.saveThumbs.Where(t => t.thumbClass == ThumbClass.Subject).ToList();

                    await Task.Run(() =>
                    {
                        foreach (var item in getMainThumb)
                        {
                            var getInfo = GetThumbInfoBase.GetThumbInfo(item);

                            saveMainInfo.Add(getInfo);

                            var childInfo = new SaveChilds();

                            GetControl<TextBox>("ShowNpcName_TBox", item.Saver).Dispatcher.Invoke(new Action(() =>
                            {
                                childInfo.Saver = GetControl<TextBox>("ShowNpcName_TBox", item.Saver).Text;
                                childInfo.Main = childInfo.Saver;
                            }));

                            childInfo.CanFather = item.CanFather;
                            childInfo.thumbClass = item.thumbClass;

                            foreach (var i in item.Children)
                            {
                                i.Dispatcher.Invoke(new Action(() =>
                                {
                                    childInfo.Children.Add(GetControl<TextBox>("ConditionsConfig_TBox", i).Text);
                                }));
                            }

                            foreach (var i in item.Fathers)
                            {
                                i.Dispatcher.Invoke(new Action(() =>
                                {
                                    childInfo.Fathers.Add(GetControl<TextBox>("ConditionsConfig_TBox", i).Text);
                                }));
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
                await Task.Run(async () =>
                {
                    foreach (var item in saveInfo)
                    {
                        var name = string.Empty;

                        item.Key.Dispatcher.Invoke(new Action(() =>
                        {
                            name = GetControl<TextBox>("ConditionsConfig_TBox", item.Key).Text;
                        }));

                        var saver = await FindSaveThumbInfo(item.Key);

                        var getMainName = string.Empty;

                        item.Key.Dispatcher.Invoke(new Action(() =>
                        {
                            if (saver.Main == null)
                            {
                                return;
                            }
                            getMainName = (GetControl<TextBox>("MainName_TBox", saver.Main)).Text;
                        }));

                        saveNPCEOInfo.Add(new ThumbInfoModel() { Main = getMainName, Name = name,thumbClass = saver.thumbClass ,data = item.Value });
                    }

                    //将没有归类的也放入数据
                    var getThumb = MainWindowModels.saveThumbs.Where(t => t.thumbClass != ThumbClass.Subject && t.thumbClass != ThumbClass.Journal && t.thumbClass != ThumbClass.Items).ToList();

                    foreach (var item in getThumb)
                    {
                        if (!saveInfo.ContainsKey(item.Saver))
                        {
                            var name = string.Empty;

                            item.Saver.Dispatcher.Invoke(new Action(() =>
                            {
                                name = GetControl<TextBox>("ConditionsConfig_TBox", item.Saver).Text;
                            }));

                            saveNPCEOInfo.Add(new ThumbInfoModel() { Name = name, thumbClass = item.thumbClass });
                        }
                    }
                });
            }
            catch(Exception ex)
            {
                result.SetError("构造器错误，错误码:JS002");

                return result;
            }
            
            var npceoJson = FileService.SaveToJson(saveNPCEOInfo);//转换为Json

            var saveJournalInfo = new List<ThumbsModels>();

            try
            {
                if (MainWindowModels.saveThumbs.Where(t => t.thumbClass == ThumbClass.Journal).Any())
                {
                    var getMainThumb = MainWindowModels.saveThumbs.Where(t => t.thumbClass == ThumbClass.Journal).ToList();

                    await Task.Run(() =>
                    {
                        foreach (var item in getMainThumb)
                        {
                            var getInfo = GetThumbInfoBase.GetThumbInfo(item);

                            saveJournalInfo.Add(getInfo);
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
                if (MainWindowModels.saveThumbs.Where(t => t.thumbClass == ThumbClass.Items).Any())
                {
                    var getMainThumb = MainWindowModels.saveThumbs.Where(t => t.thumbClass == ThumbClass.Items).ToList();

                    await Task.Run(() =>
                    {
                        foreach (var item in getMainThumb)
                        {
                            var getInfo = GetThumbInfoBase.GetThumbInfo(item);

                            saveItemsInfo.Add(getInfo);
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
                foreach (var item in MainWindowModels.saveThumbs)
                {
                    var getX = Canvas.GetLeft(item.Saver);

                    var getY = Canvas.GetTop(item.Saver);

                    switch (item.thumbClass)
                    {
                        case ThumbClass.Subject:
                            dic.Add(new ThumbCoordinateModel() { Name = GetControl<TextBox>("ShowNpcName_TBox", item.Saver).Text, thumbClass = item.thumbClass ,X = getX,Y= getY});
                            break;
                        case ThumbClass.Journal:
                            dic.Add(new ThumbCoordinateModel() { Name = GetControl<TextBox>("JournalConfig_TBox", item.Saver).Text ,thumbClass = item.thumbClass, X = getX, Y = getY });
                            break;
                        case ThumbClass.Items:
                            dic.Add(new ThumbCoordinateModel() { Name = GetControl<TextBox>("ItemsConfig_TBox", item.Saver).Text ,thumbClass = item.thumbClass, X = getX, Y = getY });
                            break;
                        default:
                            dic.Add(new ThumbCoordinateModel() { Name = GetControl<TextBox>("ConditionsConfig_TBox", item.Saver).Text ,thumbClass = item.thumbClass, X = getX, Y = getY });
                            break;
                    }
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
                await Task.Run(async () =>
                {
                    foreach (var item in saveHelpTool)
                    {
                        var thumbInfo = await FindSaveThumbInfo(item.Key);

                        if (thumbInfo == null)
                        {
                            continue;
                        }

                        if (thumbInfo.thumbClass == ThumbClass.Subject)
                        {
                            GetControl<TextBox>("ShowNpcName_TBox", thumbInfo.Saver).Dispatcher.Invoke(new Action(() =>
                            {
                                newToolList.Add(new HelpToolModel
                                {
                                    Class = thumbInfo.thumbClass,
                                    Name = GetControl<TextBox>("ShowNpcName_TBox", thumbInfo.Saver).Text,
                                    Tool = item.Value,
                                }) ;
                            }));
                        }
                        else if(thumbInfo.thumbClass == ThumbClass.Items|| thumbInfo.thumbClass == ThumbClass.Journal)
                        {
                            var getInfo = GetThumbInfoBase.GetThumbInfo(await FindSaveThumbInfo(item.Key));

                            newToolList.Add(new HelpToolModel
                            {
                                Class = thumbInfo.thumbClass,
                                Name = getInfo.Config,
                                Tool = item.Value,
                            });

                        }
                        else
                        {
                            GetControl<TextBox>("ConditionsConfig_TBox", thumbInfo.Saver).Dispatcher.Invoke(new Action(() =>
                            {
                                newToolList.Add(new HelpToolModel
                                {
                                    Class = thumbInfo.thumbClass,
                                    Name = GetControl<TextBox>("ConditionsConfig_TBox", thumbInfo.Saver).Text,
                                    Tool = item.Value,
                                });
                            }));
                        }
                    }
                });

                
            }
            catch
            {

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
        public async Task<ReturnModel> InputToYaml(YamlSaver getBacks)
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
        /// <summary>
        /// 获取真实命令
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        protected static string GetRealCmd(string txt,string fg=": ")
        {
            var getSqlit = txt.Split(new string[] { fg }, StringSplitOptions.RemoveEmptyEntries);

            if(getSqlit.Length == 3|| getSqlit.Length == 2)
            {
                return getSqlit[getSqlit.Length-1];
            }
            else
            {
                return txt;
            }
        }

        /// <summary>
        /// 从Thumb中获取控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="thumb"></param>
        /// <returns></returns>
        protected static T GetControl<T>(string name, Thumb thumb)
        {
            return (T)thumb.Template.FindName(name, thumb);
        }

        /// <summary>
        /// 查询被存储在SaveChirld中的信息
        /// </summary>
        /// <returns></returns>
        private async Task<SaveChird> FindSaveThumbInfo(Thumb thumb)
        {
            SaveChird save = null;

            await Task.Run(() =>
            {
                if(saveThumbs.Where(t => t.Saver == thumb).Any())
                {
                    save = saveThumbs.Where(t => t.Saver == thumb).FirstOrDefault();
                }
            });

            return save;
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
