using ArcCreate.Jklss.BetonQusetEditor.ViewModel;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.ThumbInfoWindow;
using ArcCreate.Jklss.Model.ThumbModel;
using ArcCreate.Jklss.Model.ThumbModel.CommandModel;
using ArcCreate.Jklss.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ArcCreate.Jklss.BetonQusetEditor.Base.FileLoader
{
    public class EventLoaderBase
    {
        private string jsons = string.Empty;

        public Thumb getThumb = null;

        private List<EventCmdModel> savecmdModels = null;

        private TreeView saveTree = null;

        public Dictionary<Thumb, ThumbInfoWindowModel> saveThumbInfoWindowModel = null;

        public EventLoaderBase()
        {
            var path = System.IO.Directory.GetCurrentDirectory() + @"\Loader\eventModel.json";

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
            var model = new List<EventCmdModel>()
            {
                new EventCmdModel()
                {
                    MainClass = "message",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = -1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="简单地向玩家发送一条消息。",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"消息列表[字符串]\n你可以使用颜色代码与%%标签" }
                        } }
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {

                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {

                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    },
                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        
                    }//参数需求
                },
                new EventCmdModel()
                {
                    MainClass = "command",//主命令

                    TextSplitChar = '|',//第二参数分割符号

                    TextNum = -1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="从控制台运行指定指令，不需要带斜杠（/）",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"命令列表[字符串]\n你可以使用%%标签" }
                        } }
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {

                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {

                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "teleport",//主命令

                    TextSplitChar = ';',//第二参数分割符号

                    TextNum = 6,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="将玩家传送到指定地点，可以选择玩家传送完毕后面对的方向。",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"位置[X,Y,Z,世界名,水平旋转度数,垂直旋转度数]" }
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
                                    {4,"玩家 身体 水平旋转度数" },
                                    {5,"玩家 头部 垂直旋转度数\n[建议0-90范围内]" },
                                }
                                }
                            }
                            }
                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {

                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "point",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = false,//是否仅有第二参数

                    MainToolTip="给予指定玩家在指定类中的指定点数。",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"指定类[字符串]" },
                            {1,"数量[整数]\n支持 加、减、乘、除，相对于原点数" }
                        } }
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {
                        {0,new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"指定类" },
                            }
                            },
                            {1,new Dictionary<int, string>
                            {
                                {0,"数量" },
                            }
                            },
                        }
                        }
                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        ('X',1),
                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "tag",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {
                        "add",
                        "del"
                    },//第二参数固有形式

                    IsNotSplitChar = false,//是否仅有第二参数

                    MainToolTip="这个事件可以 添加/删减 玩家的tag标签",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"操作[选项]" },
                            {1,"标签名[字符串][不建议您手动填写]" }
                        } }
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {
                        {0,new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"添加 | 删除" },
                            }
                            },
                        }
                        }
                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        (',',-1),
                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    },
                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "tag",new Dictionary<int, ThumbClass>(){
                                {1,ThumbClass.Conditions },
                        }},
                    }//参数需求
                },
                new EventCmdModel()
                {
                    MainClass = "objective",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {
                        "start",
                        "delete",
                        "complete"
                    },//第二参数固有形式

                    IsNotSplitChar = false,//是否仅有第二参数

                    MainToolTip="指定目标让玩家执行。",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"操作[选项]" },
                            {1,"目标名[字符串][不建议您手动填写]" }
                        } }
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {
                        {0,new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"使玩家开始 | 使玩家删除[不激活目标的事件] | 使玩家完成[激活目标的事件]" },
                            }
                            },
                        }
                        }
                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        ('X',1)
                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    },
                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "objective",new Dictionary<int, ThumbClass>(){
                            {1,ThumbClass.Objectives },
                        }},
                    }//参数需求
                },
                new EventCmdModel()
                {
                    MainClass = "journal",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {
                        "add",
                        "del",
                        "update"
                    },//第二参数固有形式

                    IsNotSplitChar = false,//是否仅有第二参数

                    MainToolTip="",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"操作[选项]" },
                            {1,"任务笔记名[字符串][不建议您手动填写]" }
                        } }

                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {
                        {0,new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"1.添加 \n 2.删除 \n 3.刷新玩家的任务笔记\n[如果选择此项请不要填写 第二条参数]\n[不常用]" },
                            }
                            },
                        }
                        }
                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        ('X',1)
                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    },
                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "journal",new Dictionary<int, ThumbClass>(){
                            {1,ThumbClass.Journal },
                        }},
                    }//参数需求
                },
                new EventCmdModel()
                {
                    MainClass = "lightning",//主命令

                    TextSplitChar = ';',//第二参数分割符号

                    TextNum = 4,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="在目标地点召唤闪电",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"指定地点[X,Y,Z,世界名]" }
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
                            }
                            }
                        }
                        }
                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {

                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "explosion",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = false,//是否仅有第二参数

                    MainToolTip="制造一场爆炸。",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"是否起火[请填写0或1，0为不起火,1为起火]" },
                            {1,"是否爆炸[请填写0或1，0为不爆炸,1为爆炸]" },
                            {2,"爆炸强度[可填写带小数的数字]\n" +
                            "[TNT 的强度是 4, 凋零出生时的强度是 7]\n" +
                            "[别调太高，会崩服的!!!]"
                            },
                            {3,"指定地点[X,Y,Z,世界名]" },
                        } }
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {
                        {0,new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"是否起火" },
                            }
                            },
                            {1,new Dictionary<int, string>
                            {
                                {0,"是否爆炸" },
                            }
                            },
                            {2,new Dictionary<int, string>
                            {
                                {0,"爆炸强度" },
                            }
                            },
                            {3,new Dictionary<int, string>
                            {
                                {0,"X轴坐标" },
                                {1,"Y轴坐标" },
                                {2,"Z轴坐标" },
                                {3,"世界名称" },
                            }
                            },
                        }
                        }
                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        ('X',1),
                        ('X',1),
                        (';',4),
                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "give",//主命令

                    TextSplitChar = ',',//第二参数分割符号

                    TextNum = -1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="给予玩家指定的物品。",

                    CmdToolTip= new List<string>
                    {
                        "",
                        "是否通知"
                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"物品名称及数量[例: emerald:5]" },
                        } }
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {

                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        
                    },

                    ChildClasses = new List<ChildClasses>()
                    {
                        new ChildClasses()
                        {
                            ChildClass = "notify",
                            ChildTextSplitChar = 'X',
                            ChildTextNum = 0,
                            ChildTextSplitWords = new List<(char i, int j)>()
                            {

                            },
                        },
                    }
                },
                new EventCmdModel()
                {
                    MainClass = "take",//主命令

                    TextSplitChar = ',',//第二参数分割符号

                    TextNum = -1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {
                        "",
                        "是否通知"
                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="从玩家背包/任务背包中移除指定物品",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"物品名称及数量[例: emerald:5]" },
                        } }
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {

                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {

                    },

                    ChildClasses = new List<ChildClasses>()
                    {
                        new ChildClasses()
                        {
                            ChildClass = "notify",
                            ChildTextSplitChar = 'X',
                            ChildTextNum = 0,
                            ChildTextSplitWords = new List<(char i, int j)>()
                            {

                            },
                        },
                    }
                },
                new EventCmdModel()
                {
                    MainClass = "effect",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = false,//是否仅有第二参数

                    MainToolTip="给玩家增加一个指定的药水效果。",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"药水类型[字符串]" },
                            {1,"药水效果持续的时间[以秒为单位]" },
                            {2,"效果等级[1代表I级]" },
                            {3,"不产生粒子效果[如果你想启用此项请在下方填写 --ambient]" },
                        } }
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {
                        {0,new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"药水类型" },
                            }
                            },
                            {1,new Dictionary<int, string>
                            {
                                {0,"药水效果持续的时间" },
                            }
                            },
                            {2,new Dictionary<int, string>
                            {
                                {0,"效果等级" },
                            }
                            },
                            {3,new Dictionary<int, string>
                            {
                                {0,"不产生粒子效果[请填写 --ambient]" },
                            }
                            },
                        }
                        }
                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        ('X',1),
                        ('X',1),
                        ('X',1),
                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "conversation",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="让玩家开始一个对话",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"对话主体的名称[不建议您手动输入]" }
                        } },
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {
                        {0,new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"对话主体的名称" },
                            }
                            }
                        }
                        },
                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        
                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    },
                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "conversation",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Subject },
                        }},
                    }//参数需求
                },
                new EventCmdModel()
                {
                    MainClass = "kill",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 0,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {
                        ""
                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="干掉玩家，没别的",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"可选择保存或不保存，不影响" }
                        } },
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {

                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {

                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "spawn",//主命令

                    TextSplitChar = ';',//第二参数分割符号

                    TextNum = 4,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {
                        
                    },//第二参数固有形式

                    IsNotSplitChar = false,//是否仅有第二参数

                    MainToolTip="在指定位置召唤指定类型的怪物",

                    CmdToolTip= new List<string>
                    {
                        "不做解释",
                        "怪物的名字"
                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {1,"怪物类型[例: ZOMBIE]" },
                            {0,"坐标[X,Y,Z,世界名]" },
                            {2,"怪物数量[整数]" },
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
                            {0,new Dictionary<int, string>
                            {
                                {0,"X轴" },
                                {1,"Y轴" },
                                {2,"Z轴" },
                                {3,"世界名" },
                            }
                            },
                            {1,new Dictionary<int, string>
                            {
                                {0,"怪物类型" },
                            }
                            },
                            {2,new Dictionary<int, string>
                            {
                                {0,"怪物数量" },
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
                        ('X',1),
                        ('X',1),
                    },

                    ChildClasses = new List<ChildClasses>()
                    {
                        new ChildClasses()
                        {
                            ChildClass = "name",
                            ChildTextSplitChar = 'X',
                            ChildTextNum = 1,//第二参数步长（-1为不限制步长）
                            ChildTextSplitWords = new List<(char i, int j)>()
                            {

                            },
                        }
                    }
                },
                new EventCmdModel()
                {
                    MainClass = "time",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="将时间跳转到某个时刻，或将时间向前推动几个小时。",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"时间[24小时制]" },
                        } },
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {
                        {0,new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"时间[24小时制]\n[你可以使用 +或- 来表示增加或减少的时间，可使用小数]\n" +
                                "[如果你不使用任何符号，那么将会把世界时间跳转到指定时间]\n" +
                                "[例: +6 则在原来的时间上增加6小时]\n" +
                                "[例: 6 则跳转到6点]" },
                            }
                            },
                        }
                        },
                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {

                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                 new EventCmdModel()
                {
                    MainClass = "weather",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {
                        "sun",
                        "rain",
                        "storm"
                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="改变天气",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"天气[选项]" },
                        } },
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {
                        {0,new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"1.晴天 \n 2.雨天 \n3.暴雨天"},
                            }
                            },
                        }
                        },
                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {

                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "folder",//主命令

                    TextSplitChar = ',',//第二参数分割符号

                    TextNum = -1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="这就像一个能够容纳多个事件的容器，你可以用它来让你的代码更加整洁。\n[这条语句其实对于编辑器来讲没啥必要]",

                    CmdToolTip= new List<string>
                    {
                        "不做解释",
                        "延迟执行",
                        "随机执行数量"
                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"事件集合[不建议您手动输入]" },
                        } },
                        {1,new Dictionary<int, string>
                        {
                            {0,"延迟时间[秒为单位]" },
                        } },
                        {2,new Dictionary<int, string>
                        {
                            {0,"将从事件中随机选取几个启动[选填]" },
                        } },
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {
                        {1,new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"延迟时间"},
                            }
                            },
                        }
                        },
                        {2,new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"将从事件中随机选取几个启动"},
                            }
                            },
                        }
                        },
                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {

                    },

                    ChildClasses = new List<ChildClasses>()
                    {
                        new ChildClasses()
                        {
                            ChildClass = "delay",
                            ChildTextSplitChar = 'X',
                            ChildTextNum = 1,
                            ChildTextSplitWords = new List<(char i, int j)>()
                            {

                            },
                        },
                        new ChildClasses()
                        {
                            ChildClass = "random",
                            ChildTextSplitChar = 'X',
                            ChildTextNum = 1,
                            ChildTextSplitWords = new List<(char i, int j)>()
                            {

                            },
                        }
                    },
                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "folder",new Dictionary<int, ThumbClass>(){
                            {0,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new EventCmdModel()
                {
                    MainClass = "setblock",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = false,//是否仅有第二参数

                    MainToolTip="在指定坐标以指定材质放置方块",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"方块名称[字符串]" },
                            {1,"坐标[X,Y,Z,世界名]" },
                        } },
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {
                        {0,new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"方块名称" },
                            }
                            },
                            {1,new Dictionary<int, string>
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
                        (';',4)
                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "damage",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="对玩家造成指定伤害",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"伤害量[整数]" },
                        } },
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {
                        {0,new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"伤害量" },
                            }
                            },
                        }
                        },
                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        
                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "party",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = false,//是否仅有第二参数

                    MainToolTip="为队伍里的全部玩家运行事件",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"范围半径[整数]" },
                            {1,"激活此事件必须的条件列表[不建议您手动输入]" },
                            {2,"激活的事件列表[不建议您手动输入]" },
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
                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        (',',-1),
                        (',',-1)
                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    },
                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        { "party",new Dictionary<int, ThumbClass>(){
                            {1,ThumbClass.Conditions },
                            {2,ThumbClass.Events },
                        }},
                    }//参数需求
                },
                new EventCmdModel()
                {
                    MainClass = "clear",//主命令

                    TextSplitChar = ',',//第二参数分割符号

                    TextNum = -1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = false,//是否仅有第二参数

                    MainToolTip="这个事件将清理指定区域的全部怪物。",

                    CmdToolTip= new List<string>
                    {
                        "不做解释",
                        "指定怪物名称[mm怪物插件名称适用][选填]"
                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"怪物类型[例: ZOMBIE]" },
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
                        (';',5)
                    },

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
                    }
                },
                new EventCmdModel()
                {
                    MainClass = "run",//主命令

                    TextSplitChar = '^',//第二参数分割符号

                    TextNum = -1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="与folder（多事件组）非常相似的一个事件，不同的是，它将直接指定多个事件\n[如果您不清楚此事件用法请使用 多事件数组:folder]",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"事件集合[填写详细的内容]\n[例: tag add xxx]" },
                        } },
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {

                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {

                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "givejournal",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 0,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="给玩家一本日记，就像/j能做到的一样。\n[此事件无参数]",

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

                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "sudo",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="以玩家的身份执行命令",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"命令集合[填写详细的内容，不需要反斜杠(/)]\n[例: spawn]" },
                        } },
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {

                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {

                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "chestgive",//主命令

                    TextSplitChar = ';',//第二参数分割符号

                    TextNum = 4,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = false,//是否仅有第二参数

                    MainToolTip="在指定坐标的箱子中放入物品。",

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
                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "chesttake",//主命令

                    TextSplitChar = ';',//第二参数分割符号

                    TextNum = 4,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = false,//是否仅有第二参数

                    MainToolTip="在指定坐标的箱子中移除物品。",

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
                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "chestclear",//主命令

                    TextSplitChar = ';',//第二参数分割符号

                    TextNum = 4,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="这个事件会清理指定坐标的箱子的全部物品。",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"坐标[X,Y,Z,世界名]" },
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
                        
                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "compass",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {
                        "add",
                        "del"
                    },//第二参数固有形式

                    IsNotSplitChar = false,//是否仅有第二参数

                    MainToolTip="当你执行这个命令，玩家可以选择位置为他指南针的目标。\n玩家需要打开任务背包然后点击指南针的图标来选择。",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"操作[选项]" },
                            {1,"目标点名称[Main.yml文件中定义]" },
                        } },
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {
                        {0,new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"添加 | 删除" },
                            }
                            },
                            {1,new Dictionary<int, string>
                            {
                                {0,"目标点名称" },
                            }
                            },
                        }
                        },
                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        ('X',1)
                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
                },
                new EventCmdModel()
                {
                    MainClass = "cancel",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = true,//是否仅有第二参数

                    MainToolTip="删除在main.yml中定义的cancel项",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"删除项名称[Main.yml文件中定义]" },
                        } },
                    },

                    TermToolTip = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>
                    {
                        {0,new Dictionary<int, Dictionary<int, string>>
                        {
                            {0,new Dictionary<int, string>
                            {
                                {0,"删除项名称" },
                            }
                            },
                        }
                        },
                    },

                    TextSplitWords = new List<(char i, int j)>()
                    {
                        
                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    },
                    NeedTpye = new Dictionary<string, Dictionary<int, ThumbClass>>()
                    {
                        
                    }//参数需求
                },
                new EventCmdModel()
                {
                    MainClass = "score",//主命令

                    TextSplitChar = 'X',//第二参数分割符号

                    TextNum = 1,//第二参数步长（-1为不限制步长）

                    Tags = new List<string>()
                    {

                    },//第二参数固有形式

                    IsNotSplitChar = false,//是否仅有第二参数

                    MainToolTip="这个事件将对计分板的点数进行更改操作",

                    CmdToolTip= new List<string>
                    {

                    },

                    ParameterToolTip= new Dictionary<int, Dictionary<int, string>>
                    {
                        {0,new Dictionary<int, string>
                        {
                            {0,"积分版项目名[字符串]" },
                            {1,"数量[整数]\n[可使用符号 +（加），-（减），*（乘），/（除）来操作，如果您不使用符号将会替换分数]" },
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
                        ('X',1)
                    },

                    ChildClasses = new List<ChildClasses>()
                    {

                    }
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
        public async Task<List<EventCmdModel>> Loader()
        {
            try
            {
                var models = await Task.Run(() => {
                    return FileService.JsonToProp<List<EventCmdModel>>(jsons);
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

        public async Task<ReturnModel> ChangeThumb(List<EventCmdModel> cmdModels, string cmd, Thumb thumb)
        {
            var model = new ReturnModel();

            getThumb = thumb;

            savecmdModels = cmdModels;

            try
            {
                EventCmdModel getModelInfo = null;

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

                EventCmdModel getModelInfo = null;

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

                (GetControl("ConditionsCmdProjectEdit_CBox", getThumb) as ComboBox).SelectionChanged -= ContisionLoaderBase_SelectionChanged;
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

                EventCmdModel getModelInfo = null;

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

            EventCmdModel getModelInfo = null;

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

                    if (fg.Length == 3)
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
        public async Task<ReturnModel> ChangeTheTree(TreeView tiw, List<EventCmdModel> cmdModels, string cmd)
        {
            var result = new ReturnModel();

            saveTree = tiw;

            EventCmdModel getModelInfo = null;

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
                result.SetError("事件树结构生成失败");
                return result;
            }
        }

        protected async Task<ThumbInfoWindowModel> CreateThunbInfowModel(EventCmdModel getModelInfo)
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
                        windowModel.TreeItems[getModelInfo.MainClass][$"此命令无参数"].Add($"第 1 项", false);
                    }
                }

                for (int i = 0; i < getModelInfo.ChildClasses.Count; i++)
                {
                    if (getModelInfo.ChildClasses[i].ChildTextNum > 0)
                    {
                        windowModel.TreeItems.Add(getModelInfo.ChildClasses[i].ChildClass, new Dictionary<string, Dictionary<string, bool>>()
                        {
                            {"第 1 条参数",new Dictionary<string, bool>()},
                        });//主命令
                        for (int j = 0; j < getModelInfo.ChildClasses[i].ChildTextNum; j++)
                        {
                            windowModel.TreeItems[getModelInfo.ChildClasses[i].ChildClass]["第 1 条参数"].Add($"第 {j + 1} 项", false);
                        }
                    }
                    else if (getModelInfo.ChildClasses[i].ChildTextNum == -1)
                    {
                        windowModel.TreeItems.Add(getModelInfo.ChildClasses[i].ChildClass, new Dictionary<string, Dictionary<string, bool>>()
                        {
                            {"第 1 条参数",new Dictionary<string, bool>()},
                        });//主命令
                        windowModel.TreeItems[getModelInfo.ChildClasses[i].ChildClass]["第 1 条参数"].Add($"第 1 项", false);
                    }
                    else
                    {
                        windowModel.TreeItems.Add(getModelInfo.ChildClasses[i].ChildClass, new Dictionary<string, Dictionary<string, bool>>()
                        {
                            {"此命令无参数",new Dictionary<string, bool>()},
                        });//主命令
                        windowModel.TreeItems[getModelInfo.ChildClasses[i].ChildClass][$"此命令无参数"].Add($"第 1 项", false);
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
                            windowModel.TreeItems[getModelInfo.MainClass][$"此命令无参数"].Add($"第 1 项", false);
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
