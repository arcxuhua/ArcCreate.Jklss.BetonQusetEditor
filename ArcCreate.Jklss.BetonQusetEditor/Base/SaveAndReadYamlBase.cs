using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.ThumbModel;
using ArcCreate.Jklss.Model.ThumbModel.CommandModel;
using ArcCreate.Jklss.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using static ArcCreate.Jklss.Model.MainWindow.MainWindowModels;

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
            Dictionary<Thumb, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> saveInfo)
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
        }

        /// <summary>
        /// 存储并输出Yaml
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnModel> SaveToYaml()
        {
            var result = new ReturnModel();

            var configModel = new AllConfigModel();//全局存储

            var mainModel = new MainConfigModel();//main文件存储

            var createJson = new Dictionary<Thumb, ConversationsModel>();//所有对话主题存储

            await Task.Run(() =>
            {
                foreach (var item in saveThumbs.Where(t => t.thumbClass == ThumbClass.Subject).ToList())
                {
                    var create = new ConversationsModel();
                    GetControl<TextBox>("ShowNpcName_TBox", item.Saver).Dispatcher.Invoke(new Action(async () =>
                    {
                        create.quester = GetControl<TextBox>("ShowNpcName_TBox", item.Saver).Text;

                        create.first = GetControl<TextBox>("ConditionsConfig_TBox", (await FindSaveThumbInfo(item.Children.FirstOrDefault())).Saver).Text;

                        create.stop = false;

                        createJson.Add(item.Saver, create);

                        mainModel.npcs.Add(GetControl<TextBox>("NpcName_TBox", item.Saver).Text, GetControl<TextBox>("MainName_TBox", item.Saver).Text);//第一项是NPC名称，第二项是配置名称
                    }));

                }
            });//构建所有对话主题及Main文件并存储

            await Task.Run(async () =>
            {
                foreach (var item in saveThumbs.Where(t=>t.thumbClass==ThumbClass.NPC).ToList())
                {
                    var newTalk = new AllTalk();

                    if (!saveInfo.ContainsKey(item.Saver))
                    {
                        result.SetError("错误！请将Npc内容填写完整");
                        return;
                    }

                    var backs = await PlayerAndNpcStructure(saveInfo[item.Saver]);//构建语句

                    if (!backs.Succese)
                    {
                        result.SetError(backs.Text);
                        return;
                    }

                    var saves = backs.Backs as Dictionary<string, string>;

                    if (!saves.ContainsKey("text"))
                    {
                        result.SetError("请至少保存一条对话");
                        return;
                    }

                    var name = string.Empty;

                    GetControl<TextBox>("ConditionsConfig_TBox", item.Saver).Dispatcher.Invoke(new Action(() => 
                    {
                        name = GetControl<TextBox>("ConditionsConfig_TBox", item.Saver).Text;
                    }));

                    newTalk.text = new List<string>(saves["text"].Split(','));

                    if (saves.ContainsKey("conditions"))
                    {
                        newTalk.conditions = saves["conditions"];
                    }

                    if (saves.ContainsKey("events"))
                    {
                        newTalk.events = saves["events"];
                    }

                    if (saves.ContainsKey("pointer"))
                    {
                        newTalk.pointer = saves["pointer"];
                    }

                    createJson[item.Main].NPC_options.Add(name, newTalk);

                    result.SetSuccese();
                }
            });//存储NPC

            if (!result.Succese)
            {
                return result;
            }

            await Task.Run(async () =>
            {
                foreach (var item in saveThumbs.Where(t => t.thumbClass == ThumbClass.Player).ToList())
                {
                    var newTalk = new AllTalk();

                    if (!saveInfo.ContainsKey(item.Saver))
                    {
                        result.SetError("错误！请将Npc内容填写完整");
                        return;
                    }

                    var backs = await PlayerAndNpcStructure(saveInfo[item.Saver]);//构建语句

                    if (!backs.Succese)
                    {
                        result.SetError(backs.Text);
                        return;
                    }

                    var saves = backs.Backs as Dictionary<string, string>;

                    if (!saves.ContainsKey("text"))
                    {
                        result.SetError("请至少保存一条对话");
                        return;
                    }

                    var name = string.Empty;

                    GetControl<TextBox>("ConditionsConfig_TBox", item.Saver).Dispatcher.Invoke(new Action(() =>
                    {
                        name = GetControl<TextBox>("ConditionsConfig_TBox", item.Saver).Text;
                    }));

                    newTalk.text = new List<string>(saves["text"].Split(','));

                    if (saves.ContainsKey("conditions"))
                    {
                        newTalk.conditions = saves["conditions"];
                    }

                    if (saves.ContainsKey("events"))
                    {
                        newTalk.events = saves["events"];
                    }

                    if (saves.ContainsKey("pointer"))
                    {
                        newTalk.pointer = saves["pointer"];
                    }

                    createJson[item.Main].player_options.Add(name, newTalk);

                    result.SetSuccese();
                }
            });//存储Player

            if (!result.Succese)
            {
                return result;
            }

            await Task.Run(async () =>
            {
                var allconditions = string.Empty;

                foreach (var item in saveThumbs.Where(t => t.thumbClass == ThumbClass.Conditions).ToList())
                {
                    if (!saveInfo.ContainsKey(item.Saver))
                    {
                        result.SetError("错误！请将其内容填写完整");
                        return;
                    }
                    var name = string.Empty;

                    GetControl<TextBox>("ConditionsConfig_TBox", item.Saver).Dispatcher.Invoke(new Action(() =>
                    {
                        name = GetControl<TextBox>("ConditionsConfig_TBox", item.Saver).Text;
                    }));

                    var backs = await ConditonStructure(saveInfo[item.Saver]);

                    if (!backs.Succese)
                    {
                        result.SetError();
                        return;
                    }

                    allconditions += name+ ": '" + backs.Backs.ToString()+ "'\r\n";
                }

                var allevents = string.Empty;

                foreach (var item in saveThumbs.Where(t => t.thumbClass == ThumbClass.Events).ToList())
                {
                    if (!saveInfo.ContainsKey(item.Saver))
                    {
                        result.SetError("错误！请将其内容填写完整");
                        return;
                    }

                    var name = string.Empty;

                    GetControl<TextBox>("ConditionsConfig_TBox", item.Saver).Dispatcher.Invoke(new Action(() =>
                    {
                        name = GetControl<TextBox>("ConditionsConfig_TBox", item.Saver).Text;
                    }));

                    var backs = await EventStructure(saveInfo[item.Saver]);

                    if (!backs.Succese)
                    {
                        result.SetError();
                        return;
                    }

                    allevents += name + ": '" + backs.Backs.ToString() + "'\r\n";
                }

                var allobjectives = string.Empty;

                foreach (var item in saveThumbs.Where(t => t.thumbClass == ThumbClass.Objectives).ToList())
                {
                    if (!saveInfo.ContainsKey(item.Saver))
                    {
                        result.SetError("错误！请将其内容填写完整");

                        return;
                    }

                    var name = string.Empty;

                    GetControl<TextBox>("ConditionsConfig_TBox", item.Saver).Dispatcher.Invoke(new Action(() =>
                    {
                        name = GetControl<TextBox>("ConditionsConfig_TBox", item.Saver).Text;
                    }));

                    var backs = await ObjectiveStructure(saveInfo[item.Saver]);

                    if (!backs.Succese)
                    {
                        result.SetError();

                        return;
                    }

                    allobjectives += name + ": '" + backs.Backs.ToString() + "'\r\n";
                }

                configModel.conditions = allconditions;
                configModel.events = allevents;
                configModel.objcetives = allobjectives;

                result.SetSuccese();
            });//存储CEO三大类

            if (!result.Succese)
            {
                return result;
            }

            await Task.Run(() => 
            {
                var allJournal = string.Empty;

                var allItems = string.Empty;
                foreach (var item in saveThumbs.Where(t => t.thumbClass == ThumbClass.Journal).ToList())
                {
                    var getInfo = GetThumbInfoBase.GetThumbInfo(item);

                    var fgTalk = getInfo.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    var makeTalk = string.Empty;

                    for (int i = 0; i < fgTalk.Length; i++)
                    {
                        makeTalk += "  " + fgTalk[i] + "\r\n";
                    }

                    allJournal += getInfo.Config + ": |\r\n"
                        + makeTalk
                        + "\r\n";
                }

                foreach (var item in saveThumbs.Where(t => t.thumbClass == ThumbClass.Items).ToList())
                {
                    var getInfo = GetThumbInfoBase.GetThumbInfo(item);

                    allItems += getInfo.Config + ":"
                        + getInfo.Text
                        + "\r\n";
                }

                configModel.journal = allJournal;

                configModel.items = allItems;

                result.SetSuccese();
            });//存储JI两类

            if (!result.Succese)
            {
                return result;
            }

            configModel.mainConfigModel = mainModel;

            configModel.allTalk = createJson;

            result = await InputToYaml(configModel);

            return result;
        }

        /// <summary>
        /// Condition语法构造器
        /// </summary>
        /// <param name="info">存储数据</param>
        /// <returns></returns>
        private async Task<ReturnModel> ConditonStructure(Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> info)
        {
            var result = new ReturnModel();

            var realcmd  = GetRealCmd(info.Keys.First());

            var getStructure = contisionProp.Where(t=>t.MainClass == realcmd).FirstOrDefault();//获取此命令的语法模型

            var needCmd = new Dictionary<string, List<(char i, int j)>>()//获取所有需要构建的命令
            {
                { getStructure.MainClass,new List<(char i,int j)>()}
            };

            needCmd[getStructure.MainClass].Add((getStructure.TextSplitChar, getStructure.TextNum));

            needCmd[getStructure.MainClass].AddRange(getStructure.TextSplitWords);

            await Task.Run(() =>
            {
                for (int i = 0; i < getStructure.ChildClasses.Count; i++)
                {
                    needCmd.Add(getStructure.ChildClasses[i].ChildClass, new List<(char i, int j)>
                {
                    (getStructure.ChildClasses[i].ChildTextSplitChar, getStructure.ChildClasses[i].ChildTextNum)
                });

                    needCmd[getStructure.ChildClasses[i].ChildClass].AddRange(getStructure.ChildClasses[i].ChildTextSplitWords);
                }
            });

            var yuju = string.Empty;

            var itemNum = 0;

            foreach (var item in info[info.Keys.First()])
            {
                if (!needCmd.ContainsKey(item.Key))
                {
                    result.SetError("错误！语法构造器错误代码：C001 请向开发者报告此代码！");

                    return result;
                }
                var getNeedCmd = needCmd[item.Key];

                if (itemNum == 0)//是主命令时
                {
                    var csNum = 0;

                    foreach (var i in item.Value)
                    {
                        if (i.Value == null)
                        {
                            result.SetError("错误！语法构造器错误代码：C002 请向开发者报告此代码！");

                            return result;
                        }

                        if (csNum == 0)//是第1参时
                        {
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：C003 请向开发者报告此代码！");

                                    return result;
                                }

                                if (i.Value.Count == 1)
                                {
                                    var kq = i.Value.Values.First();

                                    if (kq == "开启")
                                    {
                                        yuju += item.Key;
                                    }
                                }
                            }//没有项时
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = item.Key + " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if(nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = item.Key + " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            } 
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = item.Key + " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value ;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//无限项时
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：C005 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = item.Key + " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = item.Key + " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = item.Key + " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//有限项时
                        }
                        else//第n参数时
                        {
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：C003 请向开发者报告此代码！");

                                    return result;
                                }
                            }//没有项时，理论情况不允许出现
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//无限项时
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：C005 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//有限项时
                        }
                        csNum++;
                    }
                }
                else//是子命令时
                {
                    var csNum = 0;

                    foreach (var i in item.Value)
                    {
                        if (i.Value == null)
                        {
                            result.SetError("错误！语法构造器错误代码：C002 请向开发者报告此代码！");

                            return result;
                        }

                        if (csNum == 0)//子命令第一参数
                        {
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：C003 请向开发者报告此代码！");

                                    return result;
                                }

                                if (i.Value.Count == 1)
                                {
                                    var kq = i.Value.Values.First();

                                    if (kq == "开启")
                                    {
                                        yuju += " "+item.Key;
                                    }
                                }
                            }//没有项时
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " "+item.Key + ":" + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//无限项时
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：C005 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//有限项时
                        }//是第1参时
                        else
                        {
                            //理论情况不允许出现
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：C003 请向开发者报告此代码！");

                                    return result;
                                }

                                if (i.Value.Count == 1)
                                {
                                    var kq = i.Value.Values.First();

                                    if (kq == "开启")
                                    {
                                        yuju += " " + item.Key;
                                    }
                                }
                            }//没有项时
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//无限项时
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：C005 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }
                                yuju += goujian;
                            }//有限项时
                        }//理论情况不允许出现子命令多参数

                        csNum++;
                    }
                }
                itemNum++;
            }

            result.SetSuccese("Conditions语句构建成功",yuju);

            return result;
        }

        /// <summary>
        /// Event语法构造器
        /// </summary>
        /// <param name="info">存储数据</param>
        /// <returns></returns>
        private async Task<ReturnModel> EventStructure(Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> info)
        {
            var result = new ReturnModel();

            var realcmd = GetRealCmd(info.Keys.First());

            var getStructure = eventProp.Where(t => t.MainClass == realcmd).FirstOrDefault();//获取此命令的语法模型

            var needCmd = new Dictionary<string, List<(char i, int j)>>()//获取所有需要构建的命令
            {
                { getStructure.MainClass,new List<(char i,int j)>()}
            };

            needCmd[getStructure.MainClass].Add((getStructure.TextSplitChar, getStructure.TextNum));

            needCmd[getStructure.MainClass].AddRange(getStructure.TextSplitWords);

            await Task.Run(() =>
            {
                for (int i = 0; i < getStructure.ChildClasses.Count; i++)
                {
                    needCmd.Add(getStructure.ChildClasses[i].ChildClass, new List<(char i, int j)>
                {
                    (getStructure.ChildClasses[i].ChildTextSplitChar, getStructure.ChildClasses[i].ChildTextNum)
                });

                    needCmd[getStructure.ChildClasses[i].ChildClass].AddRange(getStructure.ChildClasses[i].ChildTextSplitWords);
                }
            });

            var yuju = string.Empty;

            var itemNum = 0;

            foreach (var item in info[info.Keys.First()])
            {
                if (!needCmd.ContainsKey(item.Key))
                {
                    result.SetError("错误！语法构造器错误代码：C001 请向开发者报告此代码！");

                    return result;
                }
                var getNeedCmd = needCmd[item.Key];

                if (itemNum == 0)//是主命令时
                {
                    var csNum = 0;

                    foreach (var i in item.Value)
                    {
                        if (i.Value == null)
                        {
                            result.SetError("错误！语法构造器错误代码：C002 请向开发者报告此代码！");

                            return result;
                        }

                        if (csNum == 0)//是第1参时
                        {
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：C003 请向开发者报告此代码！");

                                    return result;
                                }

                                if (i.Value.Count == 1)
                                {
                                    var kq = i.Value.Values.First();

                                    if (kq == "开启")
                                    {
                                        yuju += item.Key;
                                    }
                                }
                            }//没有项时
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = item.Key + " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = item.Key + " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = item.Key + " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//无限项时
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：C005 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (!getStructure.Tags.Contains(j.Value))
                                    {
                                        result.SetError("错误！语法构造器错误代码：E012 请向开发者报告此代码！");

                                        return result;
                                    }

                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = item.Key + " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = item.Key + " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = item.Key + " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//有限项时
                        }
                        else//第n参数时
                        {
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：C003 请向开发者报告此代码！");

                                    return result;
                                }
                            }//没有项时，理论情况不允许出现
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//无限项时
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：C005 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//有限项时
                        }
                        csNum++;
                    }
                }
                else//是子命令时
                {
                    var csNum = 0;

                    foreach (var i in item.Value)
                    {
                        if (i.Value == null)
                        {
                            result.SetError("错误！语法构造器错误代码：C002 请向开发者报告此代码！");

                            return result;
                        }

                        if (csNum == 0)//子命令第一参数
                        {
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：C003 请向开发者报告此代码！");

                                    return result;
                                }

                                if (i.Value.Count == 1)
                                {
                                    var kq = i.Value.Values.First();

                                    if (kq == "开启")
                                    {
                                        yuju += " " + item.Key;
                                    }
                                }
                            }//没有项时
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//无限项时
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：C005 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//有限项时
                        }//是第1参时
                        else
                        {
                            //理论情况不允许出现
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：C003 请向开发者报告此代码！");

                                    return result;
                                }

                                if (i.Value.Count == 1)
                                {
                                    var kq = i.Value.Values.First();

                                    if (kq == "开启")
                                    {
                                        yuju += " " + item.Key;
                                    }
                                }
                            }//没有项时
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//无限项时
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：C005 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }
                                yuju += goujian;
                            }//有限项时
                        }//理论情况不允许出现子命令多参数

                        csNum++;
                    }
                }
                itemNum++;
            }

            result.SetSuccese("Event语句构建成功", yuju);

            return result;
        }

        /// <summary>
        /// Objective语法构造器
        /// </summary>
        /// <param name="info">存储数据</param>
        /// <returns></returns>
        private async Task<ReturnModel> ObjectiveStructure(Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> info)
        {
            var result = new ReturnModel();

            var realcmd = GetRealCmd(info.Keys.First());

            var getStructure = objectiveProp.Where(t => t.MainClass == realcmd).FirstOrDefault();//获取此命令的语法模型

            var needCmd = new Dictionary<string, List<(char i, int j)>>()//获取所有需要构建的命令
            {
                { getStructure.MainClass,new List<(char i,int j)>()}
            };

            needCmd[getStructure.MainClass].Add((getStructure.TextSplitChar, getStructure.TextNum));

            needCmd[getStructure.MainClass].AddRange(getStructure.TextSplitWords);

            await Task.Run(() =>
            {
                for (int i = 0; i < getStructure.ChildClasses.Count; i++)
                {
                    needCmd.Add(getStructure.ChildClasses[i].ChildClass, new List<(char i, int j)>
                    {
                        (getStructure.ChildClasses[i].ChildTextSplitChar, getStructure.ChildClasses[i].ChildTextNum)
                    });

                    needCmd[getStructure.ChildClasses[i].ChildClass].AddRange(getStructure.ChildClasses[i].ChildTextSplitWords);
                }

                for (int i = 0; i < getStructure.ChildTags.Count; i++)
                {
                    switch (getStructure.ChildTags[i])
                    {
                        case "condition":
                            needCmd.Add(getStructure.ChildTags[i], new List<(char i, int j)>
                            {
                                (',', -1)
                            });
                            break;
                        case "events":
                            needCmd.Add(getStructure.ChildTags[i], new List<(char i, int j)>
                            {
                                (',', -1)
                            });
                            break;
                        case "notify":
                            needCmd.Add(getStructure.ChildTags[i], new List<(char i, int j)>
                            {
                                ('X', 0)
                            });
                            break;
                        case "ignoreCase":
                            needCmd.Add(getStructure.ChildTags[i], new List<(char i, int j)>
                            {
                                ('X', 0)
                            });
                            break;
                        case "cancel":
                            needCmd.Add(getStructure.ChildTags[i], new List<(char i, int j)>
                            {
                                ('X', 0)
                            });
                            break;
                    }
                }
            });

            var yuju = string.Empty;

            var itemNum = 0;

            foreach (var item in info[info.Keys.First()])
            {
                if (!needCmd.ContainsKey(item.Key))
                {
                    result.SetError("错误！语法构造器错误代码：C001 请向开发者报告此代码！");

                    return result;
                }
                var getNeedCmd = needCmd[item.Key];

                if (itemNum == 0)//是主命令时
                {
                    var csNum = 0;

                    foreach (var i in item.Value)
                    {
                        if (i.Value == null)
                        {
                            result.SetError("错误！语法构造器错误代码：C002 请向开发者报告此代码！");

                            return result;
                        }

                        if (csNum == 0)//是第1参时
                        {
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：C003 请向开发者报告此代码！");

                                    return result;
                                }

                                if (i.Value.Count == 1)
                                {
                                    var kq = i.Value.Values.First();

                                    if (kq == "开启")
                                    {
                                        yuju += item.Key;
                                    }
                                }
                            }//没有项时
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = item.Key + " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = item.Key + " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = item.Key + " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//无限项时
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：C005 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (!getStructure.Tags.Contains(j.Value))
                                    {
                                        result.SetError("错误！语法构造器错误代码：E012 请向开发者报告此代码！");

                                        return result;
                                    }

                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = item.Key + " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = item.Key + " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = item.Key + " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//有限项时
                        }
                        else//第n参数时
                        {
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：C003 请向开发者报告此代码！");

                                    return result;
                                }
                            }//没有项时，理论情况不允许出现
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//无限项时
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：C005 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//有限项时
                        }
                        csNum++;
                    }
                }
                else//是子命令时
                {
                    var csNum = 0;

                    foreach (var i in item.Value)
                    {
                        if (i.Value == null)
                        {
                            result.SetError("错误！语法构造器错误代码：C002 请向开发者报告此代码！");

                            return result;
                        }

                        if (csNum == 0)//子命令第一参数
                        {
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：C003 请向开发者报告此代码！");

                                    return result;
                                }

                                if (i.Value.Count == 1)
                                {
                                    var kq = i.Value.Values.First();

                                    if (kq == "开启")
                                    {
                                        yuju += " " + item.Key;
                                    }
                                }
                            }//没有项时
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//无限项时
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：C005 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + item.Key + ":" + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//有限项时
                        }//是第1参时
                        else
                        {
                            //理论情况不允许出现
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：C003 请向开发者报告此代码！");

                                    return result;
                                }

                                if (i.Value.Count == 1)
                                {
                                    var kq = i.Value.Values.First();

                                    if (kq == "开启")
                                    {
                                        yuju += " " + item.Key;
                                    }
                                }
                            }//没有项时
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }

                                yuju += goujian;
                            }//无限项时
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：C005 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        if (nowNum == 0)
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian = " " + j.Value;//构建命令头 
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian = " " + j.Value;//构建命令头 
                                                }
                                                else
                                                {
                                                    goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (getNeedCmd[csNum].i == 'X')
                                            {
                                                goujian += j.Value;
                                            }
                                            else
                                            {
                                                if (nowNum == i.Value.Count - 1)
                                                {
                                                    goujian += j.Value;
                                                }
                                                else
                                                {
                                                    goujian += j.Value + getNeedCmd[csNum].i;
                                                }
                                            }
                                        }
                                    }//最后一个时候
                                    else if (nowNum == 0)
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian = " " + j.Value;//构建命令头 
                                        }
                                        else
                                        {
                                            goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                        }
                                    }//第一个的时候
                                    else
                                    {
                                        if (getNeedCmd[csNum].i == 'X')
                                        {
                                            goujian += j.Value;
                                        }
                                        else
                                        {
                                            goujian += j.Value + getNeedCmd[csNum].i;
                                        }

                                    }//中间的时候
                                    nowNum++;
                                }
                                yuju += goujian;
                            }//有限项时
                        }//理论情况不允许出现子命令多参数

                        csNum++;
                    }
                }
                itemNum++;
            }

            result.SetSuccese("Event语句构建成功", yuju);

            return result;
        }

        /// <summary>
        /// Player与Npc语法构造器
        /// </summary>
        /// <param name="info">存储数据</param>
        /// <returns></returns>
        private async Task<ReturnModel> PlayerAndNpcStructure(Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> info)
        {
            var result = new ReturnModel();

            var savedic = new Dictionary<string, string>();

            await Task.Run(() =>
            {
                foreach (var item in info)
                {
                    var realCmd = GetRealCmd(item.Key);

                    var num = 0;

                    var str = string.Empty;

                    foreach (var i in item.Value[realCmd]["第 1 条参数"])
                    {
                        if (num == item.Value[realCmd]["第 1 条参数"].Count - 1)
                        {
                            str += i.Value;
                        }
                        else
                        {
                            str += i.Value + ",";
                        }
                        num++;
                    }

                    savedic.Add(realCmd, str);
                }
            });

            result.SetSuccese("Player或Npc构建成功", savedic);

            return result;
        }

        /// <summary>
        /// 输出为Yaml
        /// </summary>
        /// <param name="getBacks"></param>
        /// <returns></returns>
        private async Task<ReturnModel> InputToYaml(AllConfigModel getBacks)
        {
            var result = new ReturnModel();

            try
            {
                var createAllTalk = new List<string>();

                await Task.Run(() =>
                {
                    foreach (var item in getBacks.allTalk.Values)
                    {
                        createAllTalk.Add(FileService.SaveToYaml(item));
                    }

                    var createMain = FileService.SaveToYaml(getBacks.mainConfigModel);

                    var disPath = FileService.GetFileDirectory(filePath);

                    FileService.ChangeFile(filePath, createMain); //输出Main

                    for (int i = 0; i < createAllTalk.Count; i++)
                    {
                        var s = 0;

                        var getter = string.Empty;

                        foreach (var item in getBacks.mainConfigModel.npcs)
                        {
                            if (s == i)
                            {
                                getter = item.Value;
                            }
                            s++;
                        }
                        FileService.ChangeFile(disPath + @"\conversations\" + getter + ".yml", createAllTalk[i]); //输出对话
                    }

                    FileService.ChangeFile(disPath + @"\conditions.yml", getBacks.conditions); //输出条件

                    FileService.ChangeFile(disPath + @"\events.yml", getBacks.events); //输出事件

                    FileService.ChangeFile(disPath + @"\items.yml", getBacks.items); //输出物品

                    FileService.ChangeFile(disPath + @"\journal.yml", getBacks.journal); //输出日记

                    FileService.ChangeFile(disPath + @"\objectives.yml", getBacks.objcetives); //输出目标
                });
                result.SetSuccese("输出成功");
            }
            catch
            {
                result.SetError("输出错误");
            }

            return result;
        }
        /// <summary>
        /// 获取真实命令
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        protected static string GetRealCmd(string txt)
        {
            var getSqlit = txt.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

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
                foreach (var item in saveThumbs)
                {
                    if (item.Saver == thumb)
                    {
                        save = item;
                        break;
                    }
                }
            });

            return save;
        }
    }
}
