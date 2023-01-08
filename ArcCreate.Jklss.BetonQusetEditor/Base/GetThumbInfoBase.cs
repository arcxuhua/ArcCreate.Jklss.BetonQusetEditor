using ArcCreate.Jklss.BetonQusetEditor.ViewModel;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.ThumbModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static ArcCreate.Jklss.Model.MainWindow.MainWindowModels;

namespace ArcCreate.Jklss.BetonQusetEditor.Base
{
    public class GetThumbInfoBase
    {
        private Dictionary<ThumbClass, ThumbCanFather> bidui = new Dictionary<ThumbClass, ThumbCanFather>();
        public GetThumbInfoBase()
        {
            bidui.Add(ThumbClass.Conditions, new ThumbCanFather()
            {
                FatherCan = new Dictionary<ThumbClass, bool>
                {
                    { ThumbClass.Conditions, false },
                    { ThumbClass.Events, true },
                    {ThumbClass.Items,false },
                    {ThumbClass.Journal,false },
                    {ThumbClass.NPC,true },
                    {ThumbClass.Objectives,false },
                    {ThumbClass.Player,true },
                    {ThumbClass.Subject,false }
                },

                ChirldCan = new Dictionary<ThumbClass, bool>
                {
                    { ThumbClass.Conditions, false },
                    { ThumbClass.Events, false },
                    {ThumbClass.Items,false },
                    {ThumbClass.Journal,false },
                    {ThumbClass.NPC,false },
                    {ThumbClass.Objectives,false },
                    {ThumbClass.Player,false },
                    {ThumbClass.Subject,false }
                },

                MustChirld = new Dictionary<ThumbClass, string>
                {
                    { ThumbClass.Conditions, "or||and"},
                    { ThumbClass.Events, "" },
                    {ThumbClass.Items,"item||armor||hand" },
                    {ThumbClass.Journal,"journal||" },
                    {ThumbClass.NPC,"" },
                    {ThumbClass.Objectives,"objective||" },
                    {ThumbClass.Player,"" },
                    {ThumbClass.Subject,"" }
                }
            });

            bidui.Add(ThumbClass.Events, new ThumbCanFather()
            {
                FatherCan = new Dictionary<ThumbClass, bool>
                {
                    { ThumbClass.Conditions, false },
                    { ThumbClass.Events, false },
                    {ThumbClass.Items,false },
                    {ThumbClass.Journal,false },
                    {ThumbClass.NPC,true },
                    {ThumbClass.Objectives,true },
                    {ThumbClass.Player,true },
                    {ThumbClass.Subject,false }
                },

                ChirldCan = new Dictionary<ThumbClass, bool>
                {
                    { ThumbClass.Conditions, true },
                    { ThumbClass.Events, false },
                    {ThumbClass.Items,false },
                    {ThumbClass.Journal,false },
                    {ThumbClass.NPC,false },
                    {ThumbClass.Objectives,false },
                    {ThumbClass.Player,false },
                    {ThumbClass.Subject,false }
                },

                MustChirld = new Dictionary<ThumbClass, string>
                {
                    { ThumbClass.Conditions, "tag||"},
                    { ThumbClass.Events, "folder||" },
                    {ThumbClass.Items,"give||take" },
                    {ThumbClass.Journal,"journal||" },
                    {ThumbClass.NPC,"" },
                    {ThumbClass.Objectives,"objective||cancel" },
                    {ThumbClass.Player,"" },
                    {ThumbClass.Subject,"conversation||" }
                }
            });

            bidui.Add(ThumbClass.Journal, new ThumbCanFather()
            {
                FatherCan = new Dictionary<ThumbClass, bool>
                {
                    { ThumbClass.Conditions, false },
                    { ThumbClass.Events, false },
                    {ThumbClass.Items,false },
                    {ThumbClass.Journal,false },
                    {ThumbClass.NPC,false },
                    {ThumbClass.Objectives,false },
                    {ThumbClass.Player,false },
                    {ThumbClass.Subject,false }
                },

                ChirldCan = new Dictionary<ThumbClass, bool>
                {
                    { ThumbClass.Conditions, false },
                    { ThumbClass.Events, false },
                    {ThumbClass.Items,false },
                    {ThumbClass.Journal,false },
                    {ThumbClass.NPC,false },
                    {ThumbClass.Objectives,false },
                    {ThumbClass.Player,false },
                    {ThumbClass.Subject,false }
                },


                MustChirld = new Dictionary<ThumbClass, string>
                {
                    { ThumbClass.Conditions, ""},
                    { ThumbClass.Events, "" },
                    {ThumbClass.Items,"" },
                    {ThumbClass.Journal,"" },
                    {ThumbClass.NPC,"" },
                    {ThumbClass.Objectives,"" },
                    {ThumbClass.Player,"" },
                    {ThumbClass.Subject,"" }
                }
            });

            bidui.Add(ThumbClass.Objectives, new ThumbCanFather()
            {
                FatherCan = new Dictionary<ThumbClass, bool>
                {
                    { ThumbClass.Conditions, false },
                    { ThumbClass.Events, false },
                    {ThumbClass.Items,false },
                    {ThumbClass.Journal,false },
                    {ThumbClass.NPC,false },
                    {ThumbClass.Objectives,false },
                    {ThumbClass.Player,false },
                    {ThumbClass.Subject,false }
                },

                ChirldCan = new Dictionary<ThumbClass, bool>
                {
                    { ThumbClass.Conditions, true },
                    { ThumbClass.Events, true },
                    {ThumbClass.Items,false },
                    {ThumbClass.Journal,false },
                    {ThumbClass.NPC,false },
                    {ThumbClass.Objectives,false },
                    {ThumbClass.Player,false },
                    {ThumbClass.Subject,false }
                },


                MustChirld = new Dictionary<ThumbClass, string>
                {
                    { ThumbClass.Conditions, ""},
                    { ThumbClass.Events, "" },
                    {ThumbClass.Items,"" },
                    {ThumbClass.Journal,"" },
                    {ThumbClass.NPC,"" },
                    {ThumbClass.Objectives,"" },
                    {ThumbClass.Player,"" },
                    {ThumbClass.Subject,"" }
                }
            });

            bidui.Add(ThumbClass.Subject, new ThumbCanFather()
            {
                FatherCan = new Dictionary<ThumbClass, bool>
                {
                    { ThumbClass.Conditions, false },
                    { ThumbClass.Events, false },
                    {ThumbClass.Items,false },
                    {ThumbClass.Journal,false },
                    {ThumbClass.NPC,false },
                    {ThumbClass.Objectives,false },
                    {ThumbClass.Player,false },
                    {ThumbClass.Subject,false }
                },

                ChirldCan = new Dictionary<ThumbClass, bool>
                {
                    { ThumbClass.Conditions, false },
                    { ThumbClass.Events, false },
                    {ThumbClass.Items,false },
                    {ThumbClass.Journal,false },
                    {ThumbClass.NPC,true },
                    {ThumbClass.Objectives,false },
                    {ThumbClass.Player,false },
                    {ThumbClass.Subject,false }
                },


                MustChirld = new Dictionary<ThumbClass, string>
                {
                    { ThumbClass.Conditions, ""},
                    { ThumbClass.Events, "" },
                    {ThumbClass.Items,"" },
                    {ThumbClass.Journal,"" },
                    {ThumbClass.NPC,"" },
                    {ThumbClass.Objectives,"" },
                    {ThumbClass.Player,"" },
                    {ThumbClass.Subject,"" }
                }
            });

            bidui.Add(ThumbClass.NPC, new ThumbCanFather()
            {
                FatherCan = new Dictionary<ThumbClass, bool>
                {
                    { ThumbClass.Conditions, false },
                    { ThumbClass.Events, false },
                    {ThumbClass.Items,false },
                    {ThumbClass.Journal,false },
                    {ThumbClass.NPC,false },
                    {ThumbClass.Objectives,false },
                    {ThumbClass.Player,true },
                    {ThumbClass.Subject,true }
                },

                ChirldCan = new Dictionary<ThumbClass, bool>
                {
                    { ThumbClass.Conditions, true },
                    { ThumbClass.Events, true },
                    {ThumbClass.Items,false },
                    {ThumbClass.Journal,false },
                    {ThumbClass.NPC,false },
                    {ThumbClass.Objectives,false },
                    {ThumbClass.Player,true },
                    {ThumbClass.Subject,false }
                },


                MustChirld = new Dictionary<ThumbClass, string>
                {
                    { ThumbClass.Conditions, ""},
                    { ThumbClass.Events, "" },
                    {ThumbClass.Items,"" },
                    {ThumbClass.Journal,"" },
                    {ThumbClass.NPC,"" },
                    {ThumbClass.Objectives,"" },
                    {ThumbClass.Player,"" },
                    {ThumbClass.Subject,"" }
                }
            });

            bidui.Add(ThumbClass.Player, new ThumbCanFather()
            {
                FatherCan = new Dictionary<ThumbClass, bool>
                {
                    { ThumbClass.Conditions, false },
                    { ThumbClass.Events, false },
                    {ThumbClass.Items,false },
                    {ThumbClass.Journal,false },
                    {ThumbClass.NPC,true },
                    {ThumbClass.Objectives,false },
                    {ThumbClass.Player,false },
                    {ThumbClass.Subject,false }
                },

                ChirldCan = new Dictionary<ThumbClass, bool>
                {
                    { ThumbClass.Conditions, true },
                    { ThumbClass.Events, true },
                    {ThumbClass.Items,false },
                    {ThumbClass.Journal,false },
                    {ThumbClass.NPC,true },
                    {ThumbClass.Objectives,false },
                    {ThumbClass.Player,false },
                    {ThumbClass.Subject,false }
                },


                MustChirld = new Dictionary<ThumbClass, string>
                {
                    { ThumbClass.Conditions, ""},
                    { ThumbClass.Events, "" },
                    {ThumbClass.Items,"" },
                    {ThumbClass.Journal,"" },
                    {ThumbClass.NPC,"" },
                    {ThumbClass.Objectives,"" },
                    {ThumbClass.Player,"" },
                    {ThumbClass.Subject,"" }
                }
            });

            bidui.Add(ThumbClass.Items, new ThumbCanFather()
            {
                FatherCan = new Dictionary<ThumbClass, bool>
                {
                    { ThumbClass.Conditions, false },
                    { ThumbClass.Events, false },
                    {ThumbClass.Items,false },
                    {ThumbClass.Journal,false },
                    {ThumbClass.NPC,false },
                    {ThumbClass.Objectives,false },
                    {ThumbClass.Player,false },
                    {ThumbClass.Subject,false }
                },

                ChirldCan = new Dictionary<ThumbClass, bool>
                {
                    { ThumbClass.Conditions, false },
                    { ThumbClass.Events, false },
                    {ThumbClass.Items,false },
                    {ThumbClass.Journal,false },
                    {ThumbClass.NPC,false },
                    {ThumbClass.Objectives,false },
                    {ThumbClass.Player,false },
                    {ThumbClass.Subject,false }
                },


                MustChirld = new Dictionary<ThumbClass, string>
                {
                    { ThumbClass.Conditions, ""},
                    { ThumbClass.Events, "" },
                    {ThumbClass.Items,"" },
                    {ThumbClass.Journal,"" },
                    {ThumbClass.NPC,"" },
                    {ThumbClass.Objectives,"" },
                    {ThumbClass.Player,"" },
                    {ThumbClass.Subject,"" }
                }
            });
        }

        private string[] GetMoreMust(string text)
        {
            var geter = text.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

            return geter;
        }

        /// <summary>
        /// 获取两个Thumb能否归类为父子关系
        /// </summary>
        /// <param name="father"></param>
        /// <param name="chirld"></param>
        /// <returns></returns>
        public async Task<ReturnModel> GetThisCanFather(SaveChird father,SaveChird chirld)
        {
            var back = new ReturnModel();

            //var getChirdThumbInfo = await GetThumbInfo(chirld);

            if (father.thumbClass == ThumbClass.Conditions)
            {
                var getModel = MainWindowViewModel.contisionProp.Where(t => t.MainClass == GetRealCmd(father.Saver)).First();

                if (getModel.NeedTpye.Count == 0)
                {
                    back.SetError();

                    return back;
                }

                foreach (var item in getModel.NeedTpye)
                {
                    if(item.Value.Where(t => t.Value == chirld.thumbClass).Any())
                    {
                        back.SetSuccese("mustok");
                        return back;
                    }
                }
            }
            else if(father.thumbClass == ThumbClass.Events)
            {
                var getModel = MainWindowViewModel.eventProp.Where(t => t.MainClass == GetRealCmd(father.Saver)).First();

                if (getModel.NeedTpye.Count == 0)
                {
                    back.SetError();

                    return back;
                }

                foreach (var item in getModel.NeedTpye)
                {
                    if (item.Value.Where(t => t.Value == chirld.thumbClass).Any())
                    {
                        back.SetSuccese("mustok");
                        return back;
                    }
                }
            }
            else if(father.thumbClass == ThumbClass.Objectives)
            {
                var getModel = MainWindowViewModel.objectiveProp.Where(t => t.MainClass == GetRealCmd(father.Saver)).First();

                if (getModel.NeedTpye.Count == 0)
                {
                    back.SetError();

                    return back;
                }

                foreach (var item in getModel.NeedTpye)
                {
                    if (item.Value.Where(t => t.Value == chirld.thumbClass).Any())
                    {
                        back.SetSuccese("mustok");
                        return back;
                    }
                }
            }
            else
            {
                var getFatherThumbInfo = GetThumbInfo(father);

                if (getFatherThumbInfo == null)
                {
                    back.SetError();
                    return back;
                }

                var FatherMust = bidui[father.thumbClass].MustChirld;

                var needMust = GetMoreMust(FatherMust[chirld.thumbClass]);//获取有关父级的必须子集类型

                if (needMust.Length > 0)
                {
                    for (int i = 0; i < needMust.Length; i++)
                    {
                        if (needMust[i] == getFatherThumbInfo.Type)//确定子集元素是父级元素的必须子集
                        {
                            back.SetSuccese("mustok");
                            return back;
                        }
                    }
                }
            }

            var fatherNeed = bidui[father.thumbClass].ChirldCan;

            var chirldNeed = bidui[chirld.thumbClass].FatherCan;

            if (fatherNeed[chirld.thumbClass])
            {
                if (father.thumbClass == ThumbClass.Subject)//特殊情况 对话主体首选仅能一个对话
                {
                    if (father.Children.Count >= 1)
                    {
                        back.SetError("");
                        return back;
                    }
                }

                if(father.thumbClass == ThumbClass.Player && chirld.thumbClass == ThumbClass.NPC)//玩家对话不允许有多个Npc对话
                {
                    if (father.Children.Count >= 1)
                    {
                        back.SetError("");
                        return back;
                    }
                }

                if (chirldNeed[father.thumbClass])
                {
                    back.SetSuccese("ok");
                    return back;
                }
            }

            back.SetError();
            return back;
        }

        /// <summary>
        /// 获取Thumb内部中的相关控件信息
        /// </summary>
        /// <param name="getthumbInfo"></param>
        /// <returns></returns>
        public static ThumbsModels GetThumbInfo(SaveChird getthumbInfo)
        {
            ThumbsModels needModel = null;
            switch (getthumbInfo.thumbClass)
            {
                case ThumbClass.Subject:
                    var main_ConfigBox = getthumbInfo.Saver.Template.FindName("MainName_TBox", getthumbInfo.Saver) as TextBox;
                    var main_NpcBox = getthumbInfo.Saver.Template.FindName("NpcName_TBox", getthumbInfo.Saver) as TextBox;
                    var main_NpcNameBox = getthumbInfo.Saver.Template.FindName("ShowNpcName_TBox", getthumbInfo.Saver) as TextBox;

                    needModel = new ThumbsModels(main_ConfigBox.Text, main_NpcBox.Text, main_NpcNameBox.Text);

                    break;
                case ThumbClass.NPC:
                    var npc_ConfigBox = getthumbInfo.Saver.Template.FindName("ConditionsConfig_TBox", getthumbInfo.Saver) as TextBox;
                    var npc_TextBox = getthumbInfo.Saver.Template.FindName("Conditions_TBox", getthumbInfo.Saver) as TextBox;

                    needModel = new ThumbsModels(npc_ConfigBox.Text, "", npc_TextBox.Text);

                    break;
                case ThumbClass.Player:
                    var player_ConfigBox = getthumbInfo.Saver.Template.FindName("ConditionsConfig_TBox", getthumbInfo.Saver) as TextBox;
                    var palyer_TextBox = getthumbInfo.Saver.Template.FindName("Conditions_TBox", getthumbInfo.Saver) as TextBox;

                    needModel = new ThumbsModels(player_ConfigBox.Text, "", palyer_TextBox.Text);

                    break;
                case ThumbClass.Conditions:
                    var conditions_ConfigBox = getthumbInfo.Saver.Template.FindName("ConditionsConfig_TBox", getthumbInfo.Saver) as TextBox;
                    var conditions_TBox = getthumbInfo.Saver.Template.FindName("Conditions_TBox", getthumbInfo.Saver) as TextBox;
                    var conditions_CBox = getthumbInfo.Saver.Template.FindName("Conditions_CBox", getthumbInfo.Saver) as ComboBox;

                    try
                    {
                        var fg = conditions_CBox.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries)[2];
                        needModel = new ThumbsModels(conditions_ConfigBox.Text, fg, conditions_TBox.Text);
                    }
                    catch
                    {
                        needModel = new ThumbsModels(conditions_ConfigBox.Text, conditions_CBox.Text, conditions_TBox.Text);
                    }
                   
                    
                    break;
                case ThumbClass.Events:
                    var event_ConfigBox = getthumbInfo.Saver.Template.FindName("ConditionsConfig_TBox", getthumbInfo.Saver) as TextBox;
                    var event_TBox = getthumbInfo.Saver.Template.FindName("Conditions_TBox", getthumbInfo.Saver) as TextBox;
                    var event_CBox = getthumbInfo.Saver.Template.FindName("Conditions_CBox", getthumbInfo.Saver) as ComboBox;

                    try
                    {
                        var event_fg = event_CBox.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries)[2];
                        needModel = new ThumbsModels(event_ConfigBox.Text, event_fg, event_TBox.Text);
                    }
                    catch
                    {
                        needModel = new ThumbsModels(event_ConfigBox.Text, event_CBox.Text, event_TBox.Text);
                    }

                    break;
                case ThumbClass.Objectives:
                    var objectives_ConfigBox = getthumbInfo.Saver.Template.FindName("ConditionsConfig_TBox", getthumbInfo.Saver) as TextBox;
                    var objectives_TBox = getthumbInfo.Saver.Template.FindName("Conditions_TBox", getthumbInfo.Saver) as TextBox;
                    var objectives_CBox = getthumbInfo.Saver.Template.FindName("Conditions_CBox", getthumbInfo.Saver) as ComboBox;

                    try
                    {
                        var objectives_fg = objectives_CBox.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries)[2];

                        needModel = new ThumbsModels(objectives_ConfigBox.Text, objectives_fg, objectives_TBox.Text);
                    }
                    catch
                    {
                        needModel = new ThumbsModels(objectives_ConfigBox.Text, objectives_CBox.Text, objectives_TBox.Text);
                    }
                                        
                    break;
                case ThumbClass.Journal:
                    var journal_ConfigBox = getthumbInfo.Saver.Template.FindName("JournalConfig_TBox", getthumbInfo.Saver) as TextBox;
                    var journal_TextBox = getthumbInfo.Saver.Template.FindName("Journal_TBox", getthumbInfo.Saver) as TextBox;

                    needModel = new ThumbsModels(journal_ConfigBox.Text, "", journal_TextBox.Text);
                    break;
                case ThumbClass.Items:
                    var items_ConfigBox = getthumbInfo.Saver.Template.FindName("ItemsConfig_TBox", getthumbInfo.Saver) as TextBox;
                    var items_TextBox = getthumbInfo.Saver.Template.FindName("Items_TBox", getthumbInfo.Saver) as TextBox;

                    needModel = new ThumbsModels(items_ConfigBox.Text, "", items_TextBox.Text);
                    break;
                default: break;
            }

            return needModel;
        }

        public static ReturnModel ChangeThumbInfo(Thumb thumb,ThumbClass thumbClass,string config="",string type = "",string txt="")
        {
            var back = new ReturnModel();

            try
            {
                switch (thumbClass)
                {
                    case ThumbClass.Subject:
                        ChangeInfoTbox(thumb, "MainName_TBox", config);
                        ChangeInfoTbox(thumb, "ShowNpcName_TBox", txt);
                        ChangeInfoTbox(thumb, "NpcName_TBox", type);
                        break;
                    case ThumbClass.NPC:
                        ChangeInfoTbox(thumb, "NpcConfig_TBox", config);
                        ChangeInfoTbox(thumb, "NpcTxt_TBox", txt);

                        break;
                    case ThumbClass.Player:
                        ChangeInfoTbox(thumb, "PlayerConfig_TBox", config);
                        ChangeInfoTbox(thumb, "PlayerTxt_TBox", txt);

                        break;
                    case ThumbClass.Conditions:
                        ChangeInfoTbox(thumb, "ConditionsConfig_TBox", config);
                        ChangeInfoTbox(thumb, "Conditions_TBox", txt);
                        ChangeInfoCbox(thumb, "Conditions_CBox", type);
                        break;
                    case ThumbClass.Events:
                        ChangeInfoTbox(thumb, "EventsConfig_TBox", config);
                        ChangeInfoTbox(thumb, "Events_TBox", txt);
                        ChangeInfoCbox(thumb, "Events_CBox", type);
                        break;
                    case ThumbClass.Objectives:
                        ChangeInfoTbox(thumb, "ObjectivesConfig_TBox", config);
                        ChangeInfoTbox(thumb, "Objectives_TBox", txt);
                        ChangeInfoCbox(thumb, "Objectives_CBox", type);
                        break;
                    case ThumbClass.Journal:
                        ChangeInfoTbox(thumb, "JournalConfig_TBox", config);
                        ChangeInfoTbox(thumb, "Journal_TBox", txt);
                        break;
                    case ThumbClass.Items:
                        ChangeInfoTbox(thumb, "ItemsConfig_TBox", config);
                        ChangeInfoTbox(thumb, "Items_TBox", txt);
                        break;
                    default: break;
                }
                back.SetSuccese();

                return back;
            }
            catch(Exception ex)
            {
                back.SetError(ex.Message);

                return back;
            }
            
        }

        private static void ChangeInfoTbox(Thumb thumb, string tbox,string txt)
        {
            var textbox = thumb.Template.FindName(tbox, thumb) as TextBox;

            textbox.Text = txt;
        }

        private static void ChangeInfoCbox(Thumb thumb, string cbox,string txt)
        {
            var combobox = thumb.Template.FindName(cbox, thumb) as ComboBox;

            var num = -1;

            for (int i = 0; i < combobox.Items.Count; i++)
            {
                var fg = combobox.Items[i].ToString()
                    .Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries)[2];

                if (fg == txt)
                { 
                    num = i;
                    break;
                }
            }

            if (num > -1)
            {
                combobox.SelectedIndex = num;
            }
        }

        private string GetRealCmd(Thumb thumb)
        {
            var conditions_CBox = thumb.Template.FindName("Conditions_CBox", thumb) as ComboBox;

            if (conditions_CBox.SelectedItem == null)
            {
                return string.Empty;
            }

            var fg = TxtSplit(conditions_CBox.SelectedItem.ToString(), ": ");

            if (fg.Count == 3)
            {
                return fg[2];
            }
            else if(fg.Count == 2)
            {
                return fg[1];
            }
            else
            {
                return conditions_CBox.SelectedItem.ToString();
            }
        }

        private class ThumbCanFather
        {
            public Dictionary<ThumbClass,bool> FatherCan { get; set; }

            public Dictionary<ThumbClass,bool> ChirldCan { get; set; }

            public Dictionary<ThumbClass, string> MustChirld { get; set; }
        }
       
        /// <summary>
        /// 文本分割
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="fg"></param>
        /// <returns></returns>
        protected static List<string> TxtSplit(string txt,string fg)
        {
            var getSqlit = txt.Split(new string[] { fg }, StringSplitOptions.RemoveEmptyEntries);

            var newList = new List<string>(getSqlit);

            return newList;
        }

        public class ReadersProp
        {
            public string Checked { get; set; }

            public string Command { get; set; }

            public List<string> Chilrdren { get; set; }
        }
    }
}
