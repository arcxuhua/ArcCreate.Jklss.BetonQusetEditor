using ArcCreate.Jklss.BetonQusetEditor.ViewModel;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.ThumbInfoWindow;
using ArcCreate.Jklss.Model.ThumbModel;
using ArcCreate.Jklss.Model.ThumbModel.CommandModel;
using ArcCreate.Jklss.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ArcCreate.Jklss.BetonQusetEditor.Base.FileLoader
{
    public class ContisionLoaderBase
    {
        private string jsons = string.Empty;

        public Thumb getThumb = null;
               
        private List<ContisionsCmdModel> savecmdModels = null;

        private TreeView saveTree = null;

        public Dictionary<Thumb, ThumbInfoWindowModel> saveThumbInfoWindowModel = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        public ContisionLoaderBase()
        {
            var path = System.IO.Directory.GetCurrentDirectory()+@"\Loader\contitionModel.json";

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

        /// <summary>
        /// 初始化默认的Json文本
        /// </summary>
        /// <returns></returns>
        private string Saver()
        {
            var model = new List<ContisionsCmdModel>()
                {
                    new ContisionsCmdModel()
                    {
                        MainClass="item",//主命令

                        TextSplitChar=',',//第二参数分割符号

                        TextNum=-1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="这个条件将检测玩家背包中是否拥有指定物品. \n你可以指定一列物品,如果你只指定了名称，插件将假设你指定的是一个物品.",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"项为[物品名]" }
                            } }
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {

                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令

                        NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                        {

                        }//参数需求
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="hand",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="这个条件将检测玩家是否手持指定物品。[无法检测物品的数量]",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"项为[物品名]" }
                            } }
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {

                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令

                        NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                        {

                        }//参数需求
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="or",//主命令

                        TextSplitChar=',',//第二参数分割符号

                        TextNum=-1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = true,//是否为条件性参数

                        MainToolTip="或门会检测子集中的条件是否满足，只要满足一条条件，则或门将会激活[支持 ！取反标识]",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"项为[条件名][不建议您手动添加]" }
                            } }
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {

                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令

                        NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                        {
                            { "or",new Dictionary<int, ThumbClass>(){
                                {0,ThumbClass.Conditions },
                            }},
                        }//参数需求
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="and",//主命令

                        TextSplitChar=',',//第二参数分割符号

                        TextNum=-1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = true,//是否为条件性参数

                        MainToolTip="与门会检测子集中的条件是否满足，全部条件满足后，则与门将会激活[支持 ！取反标识]",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"项为[条件名][不建议您手动添加]" }
                            } }
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {

                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令

                        NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                        {
                            { "and",new Dictionary<int, ThumbClass>(){
                                {0,ThumbClass.Conditions },
                            }},
                        }//参数需求
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="location",//主命令

                        TextSplitChar=';',//第二参数分割符号

                        TextNum=5,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="当玩家在指定地点（X,Y,Z,世界名,模糊度）上那么该条件则满足",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"指定地点（X,Y,Z,世界名,模糊度）" }
                            } }
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"X轴坐标" },
                                    {1,"Y轴坐标" },
                                    {2,"Z轴坐标" },
                                    {3,"世界名称" },
                                    {4,"模糊度[范围，在XYZ坐标以模糊度为半径的圆内]" },
                                } 
                                }
                            } 
                            }
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令

                        NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                        {

                        }//参数需求
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="health",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="玩家最低要有多少生命值才能满足条件",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"生命值，0意味着玩家死亡" }
                            } }
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"生命值[可带小数点]" },
                                }
                                }
                            }
                            }
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="experience",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="这个条件将在玩家拥有指定等级的时候满足 (默认的MC经验)。\n注意这里是完整的等级而非经验点。请填写整数！",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"等级(默认的MC经验)" }
                            } }
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"等级[请填写整数]" },
                                }
                                }
                            }
                            }
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="permission",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="玩家为了达成条件必须要满足指定权限",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"权限节点[例如 essentials.tpa]" }
                            } }
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"权限节点" },
                                }
                                }
                            }
                            }
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="point",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="需要玩家至少拥有足够的指定类的点数。",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"类别名称 (string型字符串)" }
                            } },
                            {1,new Dictionary<int, string>
                            {
                                {0,"点数 (int型整数)" }
                            } }
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"类别名称" },
                                }
                                },
                                {1,new Dictionary<int, string>
                                {
                                    {0,"点数" },
                                }
                                }
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {
                            ('X',1)
                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="tag",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="要求玩家拥有指定标签才能达成条件。这是在创建对话时最有利的工具之一。",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"标签内容[字符串]" }
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"标签内容" },
                                }
                                }
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="armor",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="这个条件要求玩家穿戴指定物品。",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"物品名称[字符串]" }
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"物品名称" },
                                }
                                }
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="effect",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="为了满足这种条件，玩家必须具有指定的药水效果",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"药水效果[字符串]" }
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"药水效果" },
                                }
                                }
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="time",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = true,//是否为条件性参数

                        MainToolTip="这里必须指定一个特定的时间 (MC时间)，\n当玩家所在的世界的时间为指定时间范围内时，条件达成",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"24小时制,需要指定两个值，用短划线分隔[例:2-23]" }
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"小时-小时[区间]" },
                                }
                                }
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="weather",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = true,//是否为条件性参数

                        MainToolTip="为了满足条件，天气必须为指定天气",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"有三个可选项 sun（晴天）, rain（雨天） 和 storm （暴风雨天）" }
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"请输入 sun（晴天）、 rain（雨天）、storm （暴风雨天）" },
                                }
                                }
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="height",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="此条件要求玩家低于特定的Y轴高度。",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"表示Y轴的数字" }
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"Y轴" },
                                }
                                }
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="rating",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="这个需要玩家穿戴盔甲并拥有特定的护甲值（护甲图标）",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"护甲值[整数]" }
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"护甲值" },
                                }
                                }
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="random",//主命令

                        TextSplitChar='-',//第二参数分割符号

                        TextNum=2,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="这个条件用于取随机值。",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"第一项除以第二项的几率" }
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"整数[除数]" },
                                    {1,"整数[被除数]" },
                                }
                                }
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="sneak",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=0,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="这个条件将检测玩家是否潜行。[此命令无参数仅选择是否开启]",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {

                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {

                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="journal",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="这个条件要求玩家的日记中记载有指定的条目",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"任务日记的名称[不建议您手动输入]" }
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"任务日记的名称" },
                                }
                                }
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令

                        NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                        {
                            { "journal",new Dictionary<int, ThumbClass>(){
                                {0,ThumbClass.Journal },
                            }},
                        }//参数需求
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="testforblock",//主命令

                        TextSplitChar=';',//第二参数分割符号

                        TextNum=4,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="这个条件要求在指定位置的方块是指定方块。",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"坐标[X,Y,Z,世界]" },
                                {1,"方块类型[字符]" }
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"坐标" },
                                }
                                },
                                {1,new Dictionary<int, string>
                                {
                                    {0,"方块类型" },
                                }
                                }
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {
                            ('X',1)
                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="empty",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="需要玩家剩余多少个空背包格",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"空背包格数量[整数]" },
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"空背包格数量" },
                                }
                                },
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="party",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = true,//是否为条件性参数

                        MainToolTip="组队非常简单。\n" +
                        "简单到你难以理解其他的冗杂的任务系统。\n" +
                        "基本上，只有你需要使用队伍，系统才会创建队伍。\n" +
                        "队伍直接通过条件/事件定义（队伍事件和队伍条件，可以在下面的列表中看到）。\n" +
                        "在这样的指令字符串中，第一个参数是数字范围。\n" +
                        "它定义了将要查找队员的半径。第二个是条件列表。\n" +
                        "只有符合条件的玩家才会被认为是队伍的成员。\n" +
                        "对于玩家来说，这是最直观的，因为他们不需要做任何事情，\n" +
                        "没有任何命令，没有GUI，只需要“正在做一样的任务”或者“拥有一样的物品”\n" +
                        " 你的选择意味着你的队伍。",

                        CmdToolTip= new List<string>
                        {
                            "不做解释",
                            "队伍中每个玩家必须满足的条件的列表[选填]",
                            "只要队伍中有一个玩家满足指定条件[选填]",
                            "要求队伍中至少要有多少个指定玩家[选填]",
                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"范围半径[整数]" },
                                {1,"激活此语句必须的条件列表（感觉跟every参数相同）" },
                            } },
                            {1,new Dictionary<int, string>
                            {
                                {0,"队伍中每个玩家必须满足以下所有条件" },
                            } },
                            {2,new Dictionary<int, string>
                            {
                                {0,"队伍中只要有一名玩家满足以下所有条件" },
                            } },
                            {3,new Dictionary<int, string>
                            {
                                {0,"至少要有多少个玩家" },
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"范围半径" },
                                }
                                },
                            }
                            },
                            {3,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"玩家数量" },
                                }
                                },
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {
                            (',',-1)
                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {
                            new ChildClasses()
                            {
                                ChildClass = "every",
                                ChildTextSplitChar = ',',
                                ChildTextNum=-1,
                                ChildTextSplitWords = new List<(char i, int j)>()
                                {

                                },
                            },
                            new ChildClasses()
                            {
                                ChildClass = "any",
                                ChildTextSplitChar = ',',
                                ChildTextNum=-1,
                                ChildTextSplitWords = new List<(char i, int j)>()
                                {

                                },
                            },
                            new ChildClasses()
                            {
                                ChildClass = "count",
                                ChildTextSplitChar = 'X',
                                ChildTextNum=1,
                                ChildTextSplitWords = new List<(char i, int j)>()
                                {

                                },
                            },
                        },//子命令

                        NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                        {
                            { "party",new Dictionary<int, ThumbClass>(){
                                {1,ThumbClass.Conditions },
                            }},
                            { "every",new Dictionary<int, ThumbClass>(){
                                {0,ThumbClass.Conditions },
                            }},
                            { "any",new Dictionary<int, ThumbClass>(){
                                {0,ThumbClass.Conditions },
                            }},
                        }//参数需求
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="monsters",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum=-1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="指定区域中有指定数量（或更多）的指定怪物时满足条件",

                        CmdToolTip= new List<string>
                        {
                            "不做解释",
                            "指定怪物名称[mm怪物插件名称适用][选填]"
                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"怪物及数量[例: ZOMBIE:2]" },
                                {1,"坐标[X,Y,Z,世界名,模糊度]" },
                            } },
                            {1,new Dictionary<int, string>
                            {
                                {0,"怪物名称[例: Deamon]" },
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {1,new Dictionary<int, string>
                                {
                                    {0,"X轴" },
                                    {1,"Y轴" },
                                    {2,"Z轴" },
                                    {3,"世界名" },
                                    {4,"模糊度[范围半径]" },
                                }
                                },
                            }
                            },
                            {1,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"怪物名称" },
                                }
                                },
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {
                            (';',5),
                        },//多参数下是否有其他分割符
                         
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
                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="objective",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum = 1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="当玩家拥有指定目标时达成条件",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"目标名称[不建议您手动输入]" },
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"目标名称" },
                                }
                                },
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                        NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                        {
                            { "objective",new Dictionary<int, ThumbClass>(){
                                {0,ThumbClass.Objectives },
                            }},
                        }//参数需求
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="check",//主命令

                        TextSplitChar='^',//第二参数分割符号

                        TextNum = -1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="和与门条件有相似之处，这个条件也要求玩家满足列出的全部条件。\n" +
                        "不同的是，与门条件指定的是条件名称，而这儿直接指定指令字符串。",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"条件列表[如果您不懂的话建议您使用 与门:and 条件]" },
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {

                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {

                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="chestitem",//主命令

                        TextSplitChar=';',//第二参数分割符号

                        TextNum = 4,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="检测指定的坐标的箱子中是否拥有指定物品",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"坐标[X,Y,Z,世界名]" },
                                {1,"物品名称及数量[例: emerald:5,sword]" },
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"X轴" },
                                    {1,"Y轴" },
                                    {2,"Z轴" },
                                    {3,"世界名" },
                                }
                                },
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {
                            (',',-1)
                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
                    },
                    new ContisionsCmdModel()
                    {
                        MainClass="score",//主命令

                        TextSplitChar='X',//第二参数分割符号

                        TextNum = 1,//第二参数步长（-1为不限制步长）

                        isContisionCmd = false,//是否为条件性参数

                        MainToolTip="这个条件用于检测玩家的计分板。",

                        CmdToolTip= new List<string>
                        {

                        },

                        ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"积分版项目名[字符串]" },
                                {1,"数量[整数]" },
                            } },
                        },

                        TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                        {
                            {0,new Dictionary<int, Dictionary<int, string>>
                            {
                                {0,new Dictionary<int, string>
                                {
                                    {0,"积分版项目名" },
                                }
                                },
                                {1,new Dictionary<int, string>
                                {
                                    {0,"数量" },
                                }
                                },
                            }
                            },
                        },

                        TextSplitWords = new List<(char i, int j)>()
                        {
                            ('X',1),
                        },//多参数下是否有其他分割符
                         
                        ChildClasses = new List<ChildClasses>()
                        {

                        },//子命令
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
        public async Task<List<ContisionsCmdModel>> Loader()
        {
            try
            {
                var models = await Task.Run(() => { 
                    return FileService.JsonToProp<List<ContisionsCmdModel>>(jsons); 
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdModels"></param>
        /// <param name="cmd"></param>
        /// <param name="thumb"></param>
        /// <returns></returns>
        public async Task<ReturnModel> ChangeThumb(List<ContisionsCmdModel> cmdModels, string cmd,Thumb thumb)
        {
            var model = new ReturnModel();

            getThumb = thumb;

            savecmdModels = cmdModels;

            try
            {
                ContisionsCmdModel getModelInfo = null;

                var getRealCMD = TxtSplit(cmd, ": ");

                if (getRealCMD.Count >= 2)
                {
                    getModelInfo = savecmdModels.Find(t => t.MainClass == getRealCMD[getRealCMD.Count - 1]);
                }
                else
                {
                    getModelInfo = savecmdModels.Find(t => t.MainClass == cmd);
                }

                (GetControl("Conditions_CBox", thumb) as ComboBox).ToolTip = getModelInfo.MainToolTip;

                var editModel = new List<string>();//参数构造 键为命令

                editModel.Add(getModelInfo.MainClass);

                for (int i = 0; i < getModelInfo.ChildClasses.Count; i++)
                {
                    editModel.Add(getModelInfo.ChildClasses[i].ChildClass);
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

                (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = false;
                (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = false;

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

                ContisionsCmdModel getModelInfo = null;

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

                var getSelectIndex = (sender as ComboBox).SelectedIndex;

                if (getSelectIndex == -1)
                {
                    (sender as ComboBox).ToolTip = "请选择命令";
                }
                else
                {
                    try
                    {
                        (sender as ComboBox).ToolTip = getModelInfo.CmdToolTip[getSelectIndex];
                    }
                    catch
                    {
                        (sender as ComboBox).ToolTip = "此命令不做解释";
                    }
                }

                if (ccCoBox == getModelInfo.MainClass)//判断其是否为主命令
                {
                    if (getModelInfo.TextNum != 0)
                    {
                        var num = 1+ getModelInfo.TextSplitWords.Count;

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
                        if(ccCoBox == item.ChildClass)//判断其是否为子命令
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

                (GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectionChanged -= CceCoBox_SelectionChanged;
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

                ContisionsCmdModel getModelInfo = null;

                var getRealCMD = TxtSplit(cCoBox.SelectedItem.ToString(), ": ");
                (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Clear();

                (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = false;
                (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Hidden;

                (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = true;
                (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Visible;

                if (getRealCMD.Count >= 2)
                {
                    getModelInfo = savecmdModels.Find(t => t.MainClass == getRealCMD[getRealCMD.Count - 1]);
                }
                else
                {
                    getModelInfo = savecmdModels.Find(t => t.MainClass == cCoBox.SelectedItem.ToString());
                }

                ccpeCoBox.Items.Clear();

                var getSelectIndex = self.SelectedIndex;

                if (getSelectIndex == -1)
                {
                    self.ToolTip = "请选择参数";
                }
                else
                {
                    try
                    {
                        var cmdIndex = cmdCoBox.SelectedIndex;

                        self.ToolTip = getModelInfo.ParameterToolTip[cmdIndex][getSelectIndex];
                    }
                    catch
                    {
                        self.ToolTip = "此参数不做解释";
                    }
                }

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

                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = true;
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Visible;

                            (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = false;
                            (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Hidden;

                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("开启");
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("关闭");

                            ccpeCoBox.Items.Add($"第 1 项");
                        }
                    }
                    else
                    {
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

                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = true;
                            (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Visible;

                            (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = false;
                            (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Hidden;

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

                                    (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = true;
                                    (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Visible;

                                    (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = false;
                                    (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Hidden;

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

                                    (GetControl("Conditions_ComboBox", getThumb) as ComboBox).IsEnabled = true;
                                    (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Visibility = System.Windows.Visibility.Visible;

                                    (GetControl("Conditions_TBox", getThumb) as TextBox).IsEnabled = false;
                                    (GetControl("Conditions_TBox", getThumb) as TextBox).Visibility = System.Windows.Visibility.Hidden;

                                    (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("开启");
                                    (GetControl("Conditions_ComboBox", getThumb) as ComboBox).Items.Add("关闭");

                                    ccpeCoBox.Items.Add($"第 1 项");
                                }
                            }
                        }
                    }
                }

                if (MainWindowViewModel.mainWindowModels.SaveThumbInfo.ContainsKey(getThumb))
                {
                    var getInfo = MainWindowViewModel.mainWindowModels.SaveThumbInfo[getThumb];

                    try
                    {

                        var one = (GetControl("Conditions_CBox", getThumb) as ComboBox).SelectedItem.ToString();

                        var two = (GetControl("ConditionsCmdEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString();

                        var three = (GetControl("ConditionsCmdparameterEdit_CBox", getThumb) as ComboBox).SelectedItem.ToString();

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

                (GetControl("ConditionsRemove_Btn", getThumb) as Button).Click -= ContisionLoaderBase_Click1;
                (GetControl("ConditionsRemove_Btn", getThumb) as Button).Click += ContisionLoaderBase_Click1;

                (GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox).SelectionChanged -= ContisionLoaderBase_SelectionChanged;
                (GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox).SelectionChanged += ContisionLoaderBase_SelectionChanged;
            }
            catch
            {
                (GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox).Items.Clear();
                (GetControl("ConditionsAdd_Btn", getThumb) as Button).IsEnabled = false;
                (GetControl("ConditionsRemove_Btn", getThumb) as Button).IsEnabled = false;

                (GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox).SelectionChanged -= ContisionLoaderBase_SelectionChanged;
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

            ContisionsCmdModel getModelInfo = null;

            var getRealCMD = TxtSplit(getConditions_CBox.SelectedItem.ToString(), ": ");

            if (getRealCMD.Count >= 2)
            {
                getModelInfo = savecmdModels.Find(t => t.MainClass == getRealCMD[getRealCMD.Count - 1]);
            }
            else
            {
                getModelInfo = savecmdModels.Find(t => t.MainClass == getConditions_CBox.SelectedItem.ToString());
            }

            var getSelectIndex = getConditionsCmdProjectEdit_CBox.SelectedIndex;

            if (getSelectIndex == -1)
            {
                getConditionsCmdProjectEdit_CBox.ToolTip = "请选择项";
            }
            else
            {
                try
                {
                    getConditionsCmdProjectEdit_CBox.ToolTip = getModelInfo.TermToolTip[getConditionsCmdEdit_CBox.SelectedIndex]
                        [getConditionsCmdparameterEdit_CBox.SelectedIndex][getSelectIndex];
                }
                catch
                {
                    getConditionsCmdProjectEdit_CBox.ToolTip = "此项不做解释";
                }
            }

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
                var haveOne = getInfo.ContainsKey(one);

                if (!haveOne)
                {
                    var fg = one.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    if(fg.Length == 3)
                    {
                        one = fg[1] + ": " + fg[2];
                    }

                    if (!getInfo.ContainsKey(one))
                    {
                        return;
                    }
                }

                var haveTwo = getInfo[one].ContainsKey(two);

                if (!haveTwo)
                {
                    var fg = two.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    if (fg.Length == 3)
                    {
                        two = fg[1] + ": " + fg[2];
                    }

                    if (!getInfo[one].ContainsKey(two))
                    {
                        return;
                    }
                }

                var haveThree = getInfo[one][two].ContainsKey(three);

                if (!haveThree)
                {
                    var fg = three.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    if (fg.Length == 3)
                    {
                        three = fg[1] + ": " + fg[2];
                    }

                    if (!getInfo[one][two].ContainsKey(three))
                    {
                        return;
                    }
                }

                var haveFour = getInfo[one][two][three].ContainsKey(four);

                if (!haveFour)
                {
                    var fg = four.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    if (fg.Length == 3)
                    {
                        four = fg[1] + ": " + fg[2];
                    }

                    if (!getInfo[one][two][three].ContainsKey(four))
                    {
                        return;
                    }
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
            
        }

        private async void ContisionLoaderBase_Click1(object sender, System.Windows.RoutedEventArgs e)
        {
            var self = sender as Button;

            var ccpeCoBox = GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox;

            if (self.IsEnabled &&ccpeCoBox.SelectedItem != null)
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
        public async Task<ReturnModel> ChangeTheTree(TreeView tiw, List<ContisionsCmdModel> cmdModels, string cmd)
        {
            var result = new ReturnModel();

            saveTree = tiw;

            ContisionsCmdModel getModelInfo = null;

            var getRealCMD = TxtSplit(cmd, ": ");

            if (getRealCMD.Count == 2)
            {
                getModelInfo = cmdModels.Find(t => t.MainClass == getRealCMD[1]);
            }
            else if (getRealCMD.Count == 3)
            {
                getModelInfo = cmdModels.Find(t => t.MainClass == getRealCMD[2]);
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

                if (saveThumbInfoWindowModel == null||!saveThumbInfoWindowModel.ContainsKey(getThumb))//当对象存储区为空或者找不到相关Thumb时创建新的存储
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
                CheckData();

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
                result.SetError("条件树结构生成失败");
                return result;
            }
        }

        protected async Task<ThumbInfoWindowModel> CreateThunbInfowModel(ContisionsCmdModel getModelInfo)
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
                        windowModel.TreeItems.Remove(getModelInfo.MainClass);

                        windowModel.TreeItems.Add(getModelInfo.MainClass, new Dictionary<string, Dictionary<string, bool>>()
                        {
                            {"此命令无参数",new Dictionary<string, bool>()},
                        });//主命令

                        windowModel.TreeItems[getModelInfo.MainClass]["此命令无参数"].Add($"第 1 项", false);
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
                        windowModel.TreeItems.Remove(getModelInfo.MainClass);

                        windowModel.TreeItems.Add(getModelInfo.MainClass, new Dictionary<string, Dictionary<string, bool>>()
                        {
                            {"此命令无参数",new Dictionary<string, bool>()},
                        });//主命令

                        windowModel.TreeItems[getModelInfo.MainClass]["此命令无参数"].Add($"第 1 项", false);
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
                            windowModel.TreeItems.Remove(getModelInfo.MainClass);

                            windowModel.TreeItems.Add(getModelInfo.MainClass, new Dictionary<string, Dictionary<string, bool>>()
                            {
                                {"此命令无参数",new Dictionary<string, bool>()},
                            });//主命令

                            windowModel.TreeItems[getModelInfo.MainClass]["此命令无参数"].Add($"第 1 项", false);
                        }
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

        /// <summary>
        /// 用于两者之间的判断
        /// </summary>
        /// <param name="bd"></param>
        /// <param name="need"></param>
        /// <returns></returns>
        protected bool IsSame(Dictionary<Thumb, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> bd, Dictionary<Thumb, ThumbInfoWindowModel> need,string cmd)
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

                if(cmd!= getMain_Two)
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
                if(!bd.ContainsKey(getThumb)&& need.ContainsKey(getThumb))
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
        /// 保证数据的统一性
        /// </summary>
        protected async void CheckData()
        {
            if (!MainWindowViewModel.mainWindowModels.SaveThumbInfo.ContainsKey(getThumb))
            {
                return;
            }

            if (!saveThumbInfoWindowModel.ContainsKey(getThumb))
            {
                return;
            }

            try
            {
                var dataOne = MainWindowViewModel.mainWindowModels.SaveThumbInfo[getThumb][MainWindowViewModel.mainWindowModels.SaveThumbInfo[getThumb].Keys.First()];

                var dataTwo = saveThumbInfoWindowModel[getThumb];

                if (dataOne.Keys.First() != dataTwo.TreeItems.Keys.First())
                {
                    return;
                }

                var treeViewBase = new TreeViewBase();

                foreach (var item in dataOne)
                {
                    foreach (var i in item.Value)
                    {
                        foreach (var j in i.Value)
                        {
                            await treeViewBase.AddItemToSaves(getThumb, item.Key, i.Key, j.Key, "", "", true, saveThumbInfoWindowModel, MainWindowViewModel.mainWindowModels.SaveThumbInfo, false);
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }
    }
}
