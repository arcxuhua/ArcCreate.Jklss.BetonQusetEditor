using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.ThumbModel.CommandModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
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

        public async Task<ReturnModel> SaveToYaml(SaveChird saveChild)
        {
            var result = new ReturnModel();

            if(saveChild == null || saveChild.thumbClass != ThumbClass.Subject)
            {
                throw new ArgumentNullException("传输的thumb不是主对话");
            }



            result.SetSuccese("保存成功");

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
                            }
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
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
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
                        }
                        else
                        {
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：C003 请向开发者报告此代码！");

                                    return result;
                                }

                                //理论情况不允许出现
                            }
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
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
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
                        }

                        csNum++;
                    }
                }
                else
                {
                    var csNum = 0;

                    foreach (var i in item.Value)
                    {
                        if (i.Value == null)
                        {
                            result.SetError("错误！语法构造器错误代码：C002 请向开发者报告此代码！");

                            return result;
                        }

                        if (csNum == 0)
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
                            }
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " "+item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
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
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
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
                            }
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
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
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
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
                    result.SetError("错误！语法构造器错误代码：E001 请向开发者报告此代码！");

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
                            result.SetError("错误！语法构造器错误代码：E002 请向开发者报告此代码！");

                            return result;
                        }

                        if (csNum == 0)//是第1参时
                        {
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：E003 请向开发者报告此代码！");

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
                            }
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：E004 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        if (!getStructure.Tags.Contains(j.Value))
                                        {
                                            result.SetError("错误！语法构造器错误代码：E012 请向开发者报告此代码！");

                                            return result;
                                        }

                                        goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
                        }
                        else
                        {
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：E005 请向开发者报告此代码！");

                                    return result;
                                }

                                //理论情况不允许出现
                            }
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：E006 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
                        }

                        csNum++;
                    }
                }
                else
                {
                    var csNum = 0;

                    foreach (var i in item.Value)
                    {
                        if (i.Value == null)
                        {
                            result.SetError("错误！语法构造器错误代码：E007 请向开发者报告此代码！");

                            return result;
                        }

                        if (csNum == 0)
                        {
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：E008 请向开发者报告此代码！");

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
                            }
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：E009 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
                        }//是第1参时
                        else
                        {
                            //理论情况不允许出现
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：E010 请向开发者报告此代码！");

                                    return result;
                                }
                            }
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：E1011 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
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
                    result.SetError("错误！语法构造器错误代码：E001 请向开发者报告此代码！");

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
                            result.SetError("错误！语法构造器错误代码：E002 请向开发者报告此代码！");

                            return result;
                        }

                        if (csNum == 0)//是第1参时
                        {
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：E003 请向开发者报告此代码！");

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
                            }
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：E004 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        if (!getStructure.Tags.Contains(j.Value))
                                        {
                                            result.SetError("错误！语法构造器错误代码：E012 请向开发者报告此代码！");

                                            return result;
                                        }

                                        goujian = item.Key + " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
                        }
                        else
                        {
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：E005 请向开发者报告此代码！");

                                    return result;
                                }

                                //理论情况不允许出现
                            }
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：E006 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
                        }

                        csNum++;
                    }
                }
                else
                {
                    var csNum = 0;

                    foreach (var i in item.Value)
                    {
                        if (i.Value == null)
                        {
                            result.SetError("错误！语法构造器错误代码：E007 请向开发者报告此代码！");

                            return result;
                        }

                        if (csNum == 0)
                        {
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：E008 请向开发者报告此代码！");

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
                            }
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：E009 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + item.Key + ":" + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
                        }//是第1参时
                        else
                        {
                            //理论情况不允许出现
                            if (getNeedCmd[csNum].j == 0)
                            {
                                if (i.Value.Count != 1 || i.Value.Count != 0)
                                {
                                    result.SetError("错误！语法构造器错误代码：E010 请向开发者报告此代码！");

                                    return result;
                                }
                            }
                            else if (getNeedCmd[csNum].j == -1)
                            {
                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
                            else
                            {
                                if (i.Value.Count != getNeedCmd[csNum].j)
                                {
                                    result.SetError("错误！语法构造器错误代码：E1011 请向开发者报告此代码！");

                                    return result;
                                }

                                var goujian = string.Empty;

                                var nowNum = 0;

                                foreach (var j in i.Value)
                                {
                                    if (nowNum == i.Value.Count - 1)
                                    {
                                        goujian += j.Value + getNeedCmd[csNum].i;
                                    }
                                    else if (nowNum == 0)
                                    {
                                        goujian = " " + j.Value + getNeedCmd[csNum].i;//构建命令头 
                                    }
                                    else
                                    {
                                        goujian += j.Value;
                                    }
                                    nowNum++;
                                }

                                yuju += goujian;
                            }
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

            foreach (var item in info)
            {
                var realCmd = GetRealCmd(item.Key);

                var num = 0;

                var str = string.Empty;

                foreach (var i in item.Value[realCmd]["第 1 条参数"])
                {
                    if(num == item.Value[realCmd]["第 1 条参数"].Count - 1)
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

            result.SetSuccese("Player或Npc构建成功", savedic);

            return result;
        }

        /// <summary>
        /// 获取真实命令
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        protected static string GetRealCmd(string txt)
        {
            var getSqlit = txt.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

            if(getSqlit.Length == 3|| getSqlit.Length == 2)
            {
                return getSqlit[1];
            }
            else
            {
                return txt;
            }
        }
    }
}
