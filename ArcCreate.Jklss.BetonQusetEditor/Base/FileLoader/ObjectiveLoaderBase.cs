﻿using ArcCreate.Jklss.BetonQusetEditor.ViewModel;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.ThumbInfoWindow;
using ArcCreate.Jklss.Model.ThumbModel;
using ArcCreate.Jklss.Model.ThumbModel.CommandModel;
using ArcCreate.Jklss.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ArcCreate.Jklss.BetonQusetEditor.Base.FileLoader
{
    public class ObjectiveLoaderBase
    {
        private string jsons = string.Empty;

        public Thumb getThumb = null;

        private List<ObjectiveCmdModel> savecmdModels = null;

        private TreeView saveTree = null;

        public Dictionary<Thumb, ThumbInfoWindowModel> saveThumbInfoWindowModel = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ObjectiveLoaderBase()
        {
            var path = System.IO.Directory.GetCurrentDirectory() + @"\Loader\objectiveModel.json";

            var json = FileService.GetFileText(path);

            if (string.IsNullOrEmpty(json))
            {
                jsons = Saver();

                FileService.ChangeFile(path, jsons);
            }
            else
            {
                jsons = json;
            }
        }

        private string Saver()
        {
            var model = new List<ObjectiveCmdModel>()
            {
                new ObjectiveCmdModel()
                {
                    MainClass = "location",//主命令

                    TextSplitChar=';',//第二参数分割符

                    TextNum = 5,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {

                    },//第n条参数分割符

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "condition",
                        "events",
                        "notify"
                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {

                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "condition",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Conditions },
                        }},
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "block",//主命令

                    TextSplitChar='X',//第二参数分割符

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        ('X',1)
                    },//第n条参数分割符

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "events",
                        "notify"
                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {

                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "mobkill",//主命令

                    TextSplitChar='X',//第二参数分割符

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        ('X',1)
                    },//第n条参数分割符

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "condition",
                        "events",
                        "notify"
                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {
                        new ChildClasses()
                        {
                            ChildClass = "name",
                            ChildTextNum = 1,
                            ChildTextSplitChar = 'X',
                            ChildTextSplitWords = new List<(char i, int j)>()
                            {

                            },
                        },
                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "condition",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Conditions },
                        }},
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "action",//主命令

                    TextSplitChar='X',//第二参数分割符

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        ('X',1)
                    },//第n条参数分割符

                    Tags = new List<string>()
                    {
                        "right",
                        "left",
                        "any"
                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "condition",
                        "events",
                        "notify",
                        "cancel"
                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {
                        new ChildClasses()
                        {
                            ChildClass = "loc",
                            ChildTextSplitChar = ';',
                            ChildTextNum= 5,
                            ChildTextSplitWords = new List<(char i, int j)>()
                            {

                            },
                        },
                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "condition",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Conditions },
                        }},
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "die",//主命令

                    TextSplitChar='X',//第二参数分割符

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        
                    },//第n条参数分割符

                    Tags = new List<string>()
                    {
                        "cancel",
                        ""
                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "condition",
                        "events",
                        "notify",
                        
                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {
                        new ChildClasses()
                        {
                            ChildClass = "respawn",
                            ChildTextSplitChar = ';',
                            ChildTextNum = 6,
                            ChildTextSplitWords = new List<(char i, int j)>()
                            {

                            },
                        },
                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "condition",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Conditions },
                        }},
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "craft",//主命令

                    TextSplitChar='X',//第二参数分割符

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        ('X',1)
                    },//第n条参数分割符

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "events",
                        "notify",

                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {
                        
                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "smelt",//主命令

                    TextSplitChar='X',//第二参数分割符

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        ('X',1)
                    },//第n条参数分割符

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "events",
                        "notify",

                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {

                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "tame",//主命令

                    TextSplitChar='X',//第二参数分割符

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        ('X',1)
                    },//第n条参数分割符

                    Tags = new List<string>()
                    {
                        "WOLF",
                        "OCELOT",
                        "HORSE",
                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "events",
                        "notify",

                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {

                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "delay",//主命令

                    TextSplitChar='X',//第二参数分割符

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        
                    },//第n条参数分割符

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "events",
                        "notify",

                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {

                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "arrow",//主命令

                    TextSplitChar=';',//第二参数分割符

                    TextNum = 5,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        
                    },//第n条参数分割符

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "conditions",
                        "events",
                        "notify",

                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {

                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "condition",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Conditions },
                        }},
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "experience",//主命令

                    TextSplitChar='X',//第二参数分割符

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        
                    },//第n条参数分割符

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "events",
                        "notify",

                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {

                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "step",//主命令

                    TextSplitChar=';',//第二参数分割符

                    TextNum = 4,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {

                    },//第n条参数分割符

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "events",
                        "notify",

                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {

                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "logout",//主命令

                    TextSplitChar='X',//第二参数分割符

                    TextNum = 0,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {

                    },//第n条参数分割符

                    Tags = new List<string>()
                    {
                        
                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "events",
                        "notify",

                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {

                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "password",//主命令

                    TextSplitChar='X',//第二参数分割符

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        
                    },//第n条参数分割符

                    Tags = new List<string>()
                    {
                        
                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "events",
                        "notify",
                        "ignoreCase",
                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {

                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "fish",//主命令

                    TextSplitChar='X',//第二参数分割符

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        ('X',1)
                    },//第n条参数分割符

                    Tags = new List<string>()
                    {
                        
                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "events",
                        "notify",
                        
                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {

                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "shear",//主命令

                    TextSplitChar='X',//第二参数分割符

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        
                    },//第n条参数分割符

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "events",
                        "notify",

                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {
                        new ChildClasses()
                        {
                            ChildClass = "name",
                            ChildTextSplitChar = 'X',
                            ChildTextNum = 1,
                            ChildTextSplitWords = new List<(char i, int j)>()
                            {

                            },
                        },
                        new ChildClasses()
                        {
                            ChildClass = "color",
                            ChildTextSplitChar = 'X',
                            ChildTextNum = 1,
                            ChildTextSplitWords = new List<(char i, int j)>()
                            {

                            },
                        },
                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "enchant",//主命令

                    TextSplitChar='X',//第二参数分割符

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        ('X',1)
                    },//第n条参数分割符

                    Tags = new List<string>()
                    {
                        
                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "events",
                        "notify",

                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {
                        
                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "chestput",//主命令

                    TextSplitChar=';',//第二参数分割符

                    TextNum = 4,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        (',',-1)
                    },//第n条参数分割符

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "events",
                        "notify",

                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {

                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new ObjectiveCmdModel()
                {
                    MainClass = "potion",//主命令

                    TextSplitChar='X',//第二参数分割符

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        ('X',1)
                    },//第n条参数分割符

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    ChildTags = new List<string>()
                    {
                        "events",
                        "notify",

                    },//固有子命令（专用contitions、Event）

                    ChildClasses = new List<ChildClasses>()
                    {
                        new ChildClasses()
                        {
                            ChildClass = "effects",
                            ChildTextSplitChar = ',',
                            ChildTextNum = -1,
                            ChildTextSplitWords = new List<(char i, int j)>()
                            {

                            },
                        },
                    },//子命令结构

                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "events",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
            };


            try
            {
                return FileService.SaveToJson(model);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 得到相关实体类
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task<List<ObjectiveCmdModel>> Loader()
        {
            try
            {
                var models = await Task.Run(() => {
                    return FileService.JsonToProp<List<ObjectiveCmdModel>>(jsons);
                });

                if (models.Count == 0)
                {
                    return null;
                }

                return models;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ReturnModel> ChangeThumb( List<ObjectiveCmdModel> cmdModels, string cmd, Thumb thumb)
        {
            var model = new ReturnModel();

            getThumb = thumb;

            savecmdModels = cmdModels;

            try
            {
                ObjectiveCmdModel getModelInfo = null;

                var getRealCMD = TxtSplit(cmd, ": ");

                if (getRealCMD.Count >= 2)
                {
                    getModelInfo = savecmdModels.Find(t => t.MainClass == getRealCMD[getRealCMD.Count - 1]);
                }
                else
                {
                    getModelInfo = savecmdModels.Find(t => t.MainClass == cmd);
                }

                var editModel = new List<string>();

                editModel.Add(getModelInfo.MainClass);

                for (int i = 0; i < getModelInfo.ChildClasses.Count; i++)
                {
                    editModel.Add(getModelInfo.ChildClasses[i].ChildClass);
                }

                foreach (var item in getModelInfo.ChildTags)
                {
                    editModel.Add(item);
                }

                var cmdCoBox = GetControl("ConditionsCmdEdit_CBox", thumb) as ComboBox;

                cmdCoBox.Items.Clear();

                foreach (var item in editModel)
                {
                    await Task.Run(() =>
                    {
                        MainWindowViewModel.mainWindow.Dispatcher.Invoke(new Action(() => {
                            cmdCoBox.Items.Add(item);
                        }));
                    });
                }
                //添加可编辑命令

                cmdCoBox.SelectionChanged -= CmdCoBox_SelectionChanged;
                cmdCoBox.SelectionChanged += CmdCoBox_SelectionChanged;

                (GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectionChanged -= CceCoBox_SelectionChanged;
                (GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox).SelectionChanged -= ContisionLoaderBase_SelectionChanged;

                model.SetSuccese();

                return model;
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
                    ccCoBox = (sender as ComboBox).SelectedItem.ToString();
                }

                var cCoBox = (GetControl("Conditions_CBox", getThumb) as ComboBox).SelectedItem.ToString();

                ObjectiveCmdModel getModelInfo = null;

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

                if (ccCoBox == getModelInfo.MainClass)//判断其是否为主命令
                {
                    if (getModelInfo.TextNum != 0)
                    {
                        var num = 1 + getModelInfo.TextSplitWords.Count;

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
                        if (ccCoBox == item.ChildClass)//判断其是否为子命令
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

                    foreach (var item in getModelInfo.ChildTags)
                    {
                        if (ccCoBox == item)//判断其是否为固有子命令
                        {
                            switch (item)
                            {
                                case "condition":
                                    cceCoBox.Items.Add($"第 1 条参数");
                                    break;
                                case "events":
                                    cceCoBox.Items.Add($"第 1 条参数");
                                    break;
                                case "notify":
                                    cceCoBox.Items.Add($"此命令无参数");
                                    break;
                                case "ignoreCase":
                                    cceCoBox.Items.Add($"此命令无参数");
                                    break;
                                case "cancel":
                                    cceCoBox.Items.Add($"此命令无参数");
                                    break;
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

                ObjectiveCmdModel getModelInfo = null;

                var getRealCMD = TxtSplit(cCoBox.SelectedItem.ToString(), ": ");

                if (getRealCMD.Count >= 2)
                {
                    getModelInfo = savecmdModels.Find(t => t.MainClass == getRealCMD[getRealCMD.Count - 1]);
                }
                else
                {
                    getModelInfo = savecmdModels.Find(t => t.MainClass == cCoBox.SelectedItem.ToString());
                }

                ccpeCoBox.Items.Clear();
                (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Clear();

                if (cmdCoBox.SelectedItem == null)
                {
                    return;
                }

                if (cmdCoBox.SelectedItem.ToString() == getModelInfo.MainClass)//判断其是否为主命令
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

                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("开启");
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("关闭");

                            ccpeCoBox.Items.Add($"第 1 项");
                        }

                        if (getModelInfo.Tags.Count > 0)
                        {
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = true;
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Visible;

                            (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = false;
                            (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Hidden;

                            for (int i = 0; i < getModelInfo.Tags.Count; i++)
                            {
                                (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add(getModelInfo.Tags[i]);
                            }
                        }
                    }
                    else
                    {
                        (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = false;
                        (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Hidden;

                        (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = true;
                        (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Visible;

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
                        if (item.ChildClass == cmdCoBox.SelectedItem.ToString())
                        {
                            var getIndex = self.SelectedIndex;//找到当前所选中的个数

                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = false;
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Hidden;

                            (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = true;
                            (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Visible;

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

                                    (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("开启");
                                    (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("关闭");

                                    ccpeCoBox.Items.Add($"第 1 项");
                                }
                            }
                        }
                    }

                    switch (cmdCoBox.SelectedItem.ToString())
                    {
                        case "condition":
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = false;
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Hidden;

                            (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = true;
                            (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Visible;

                            (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = true;
                            (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = true;

                            ccpeCoBox.Items.Add($"第 1 项");
                            break;
                        case "events":
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = false;
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Hidden;

                            (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = true;
                            (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Visible;

                            (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = true;
                            (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = true;

                            ccpeCoBox.Items.Add($"第 1 项");
                            break;
                        case "notify":
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = true;
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Visible;

                            (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = false;
                            (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Hidden;

                            (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = false;
                            (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = false;

                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("开启");
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("关闭");
                            ccpeCoBox.Items.Add($"第 1 项");
                            break;
                        case "ignoreCase":
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = true;
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Visible;

                            (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = false;
                            (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Hidden;

                            (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = false;
                            (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = false;

                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("开启");
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("关闭");

                            ccpeCoBox.Items.Add($"第 1 项");
                            break;
                        case "cancel":
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = true;
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Visible;

                            (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = false;
                            (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = false;

                            (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = false;
                            (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Hidden;

                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("开启");
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("关闭");

                            ccpeCoBox.Items.Add($"第 1 项");
                            break;
                    }
                }

                if (MainWindowViewModel.mainWindowModels.SaveThumbInfo.ContainsKey(getThumb))
                {
                    var getInfo = MainWindowViewModel.mainWindowModels.SaveThumbInfo[getThumb];

                    try
                    {
                        var getNeed = getInfo[(GetControl("Conditions_CBox", getThumb) as ComboBox).SelectedItem.ToString()]
                            [(GetControl("ConditionsCmdEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString()]
                            [(GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString()];

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

                (GetControl("ConditionsRemove_Btn", getThumb) as Button).Click += ContisionLoaderBase_Click1;
                (GetControl("ConditionsRemove_Btn", getThumb) as Button).Click += ContisionLoaderBase_Click1;

                (GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox).SelectionChanged -= ContisionLoaderBase_SelectionChanged;
                (GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox).SelectionChanged += ContisionLoaderBase_SelectionChanged;
            }
            catch
            {
                (GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox).Items.Clear();
                (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = false;
                (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = false;

                (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = false;
                (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Hidden;

                (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = true;
                (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Visible;
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

            if (!MainWindowViewModel.mainWindowModels.SaveThumbInfo.ContainsKey(getThumb))
            {
                return;
            }

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

        private async void ContisionLoaderBase_Click1(object sender, System.Windows.RoutedEventArgs e)
        {
            var self = sender as Button;

            var ccpeCoBox = GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox;

            if (self.IsEnabled && ccpeCoBox.SelectedItem != null)
            {

                var one = (GetControl("ConditionsCmdEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString();

                var two = (GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString();

                var three = ccpeCoBox.SelectedItem.ToString();

                var cs = saveThumbInfoWindowModel[getThumb].TreeItems[one][two].Remove(three);

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

                await DeleteTreeItem(one, two, three);

                ccpeCoBox.Items.Remove(ccpeCoBox.SelectedItem);
            }
        }

        private async void ContisionLoaderBase_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var self = sender as Button;

            if (self.IsEnabled)
            {
                var ccpeCoBox = GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox;

                var one = (GetControl("ConditionsCmdEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString();

                var two = (GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString();

                saveThumbInfoWindowModel[getThumb].TreeItems[one][two].Add($"第 {ccpeCoBox.Items.Count + 1} 项", false);

                await AddTreeItem(one, two, $"第 {ccpeCoBox.Items.Count + 1} 项");

                ccpeCoBox.Items.Add($"第 {ccpeCoBox.Items.Count + 1} 项");
            }
        }

        /// <summary>
        /// 当选中为条件、事件、目标类型的Thumb时更改树形结构
        /// </summary>
        public async Task<ReturnModel> ChangeTheTree(TreeView tiw, List<ObjectiveCmdModel> cmdModels, string cmd)
        {
            var result = new ReturnModel();

            saveTree = tiw;

            ObjectiveCmdModel getModelInfo = null;

            var getRealCMD = TxtSplit(cmd, ": ");

            if (getRealCMD.Count >= 2)
            {
                getModelInfo = cmdModels.Find(t => t.MainClass == getRealCMD[getRealCMD.Count - 1]);
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
                if (saveThumbInfoWindowModel == null || !saveThumbInfoWindowModel.ContainsKey(getThumb))//当对象存储区为空或者找不到相关Thumb时创建新的存储
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

                if (!IsSame(MainWindowViewModel.mainWindowModels.SaveThumbInfo, saveThumbInfoWindowModel, getModelInfo.MainClass))//当数据存储区与对象存储区不相同时
                {
                    saveThumbInfoWindowModel[getThumb] = await CreateThunbInfowModel(getModelInfo);
                }
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
                result.SetError("出大事了兄弟");
                return result;
            }
        }

        protected async Task<ThumbInfoWindowModel> CreateThunbInfowModel(ObjectiveCmdModel getModelInfo)
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
                        windowModel.TreeItems[getModelInfo.MainClass][$"第 {i + 2} 条参数"].Add($"此命令无参数", false);
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
                        windowModel.TreeItems[getModelInfo.ChildClasses[i].ChildClass]["此命令无参数"].Add($"第 1 项", false);
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
                            windowModel.TreeItems[getModelInfo.MainClass][$"第 {j + 2} 条参数"].Add("此命令无参数", false);
                        }
                    }
                }

                foreach (var item in getModelInfo.ChildTags)
                {

                    switch (item)
                    {
                        case "condition":
                            windowModel.TreeItems.Add(item, new Dictionary<string, Dictionary<string, bool>>()
                            {
                                {"第 1 条参数",new Dictionary<string, bool>()},
                            });//主命令

                            windowModel.TreeItems[item]["第 1 条参数"].Add($"第 1 项", false);
                            break;
                        case "events":
                            windowModel.TreeItems.Add(item, new Dictionary<string, Dictionary<string, bool>>()
                            {
                                {"第 1 条参数",new Dictionary<string, bool>()},
                            });//主命令

                            windowModel.TreeItems[item]["第 1 条参数"].Add($"第 1 项", false);
                            break;
                        case "notify":
                            windowModel.TreeItems.Add(item, new Dictionary<string, Dictionary<string, bool>>()
                            {
                                {"此命令无参数",new Dictionary<string, bool>()},
                            });//主命令
                            windowModel.TreeItems[item]["此命令无参数"].Add($"第 1 项", false);
                            break;
                        case "ignoreCase":
                            windowModel.TreeItems.Add(item, new Dictionary<string, Dictionary<string, bool>>()
                            {
                                {"此命令无参数",new Dictionary<string, bool>()},
                            });//主命令
                            windowModel.TreeItems[item]["此命令无参数"].Add($"第 1 项", false);
                            break;
                        case "cancel":
                            windowModel.TreeItems.Add(item, new Dictionary<string, Dictionary<string, bool>>()
                            {
                                {"此命令无参数",new Dictionary<string, bool>()},
                            });//主命令
                            windowModel.TreeItems[item]["此命令无参数"].Add($"第 1 项", false);
                            break;
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
        /// 用于两者之间的判断
        /// </summary>
        /// <param name="bd"></param>
        /// <param name="need"></param>
        /// <returns></returns>
        protected bool IsSame(Dictionary<Thumb, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> bd, Dictionary<Thumb, ThumbInfoWindowModel> need, string cmd)
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

                if (cmd != getMain_Two)
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
                if (!bd.ContainsKey(getThumb) && need.ContainsKey(getThumb))
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