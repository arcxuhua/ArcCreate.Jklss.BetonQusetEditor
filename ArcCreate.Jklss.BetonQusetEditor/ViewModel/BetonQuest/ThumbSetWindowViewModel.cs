﻿using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.Model.ThumbSetWindow;
using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Text.RegularExpressions;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest
{
    public class ThumbSetWindowViewModel : NotifyBase
    {
        public ThumbSetWindowModel model = new ThumbSetWindowModel();

        public bool UseItem
        {
            get
            {
                return model.UseItem;
            }
            set
            {
                model.UseItem = value;

                NotifyChanged();
            }
        }

        public bool IsNegate
        {
            get
            {
                return model.IsNegate;
            }
            set
            {
                model.IsNegate = value;

                NotifyChanged();
            }
        }

        public bool IsEnabel
        {
            get
            {
                return model.IsEnabel;
            }
            set
            {
                model.IsEnabel = value;

                NotifyChanged();
            }
        }

        public string ClassificationsSeleted
        {
            get
            {
                return model.ClassificationsSeleted;
            }
            set
            {
                model.ClassificationsSeleted = value;

                NotifyChanged();
            }
        }

        public string TermsSeleted
        {
            get
            {
                return model.TermsSeleted;
            }
            set
            {
                model.TermsSeleted = value;

                NotifyChanged();
            }
        }

        public string ItemNum
        {
            get
            {
                return model.ItemNum;
            }
            set
            {
                model.ItemNum = value;

                NotifyChanged();
            }
        }

        public List<string> Classifications
        {
            get
            {
                return model.Classifications;
            }
            set
            {
                model.Classifications = value;

                NotifyChanged();
            }
        }

        public List<string> Terms
        {
            get
            {
                return model.Terms;
            }
            set
            {
                model.Terms = value;

                NotifyChanged();
            }
        }

        public Dictionary<string, List<string>> SaveTerms { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CommandBase _SendOk;
        public CommandBase SendOk
        {
            get
            {
                if (_SendOk == null)
                {
                    _SendOk = new CommandBase();
                    _SendOk.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        if (!IsEnabel)
                        {
                            IsNegate = false;
                        }
                        var itemNums = 0;
                        if (UseItem)
                        {
                            if (!Regex.IsMatch(ItemNum, @"^[1-9]\d*"))
                            {
                                return;
                            }
                            else
                            {
                                itemNums = Convert.ToInt32(ItemNum);
                            }
                        }

                        if (string.IsNullOrEmpty(ClassificationsSeleted))
                        {
                            return;
                        }

                        if (string.IsNullOrEmpty(TermsSeleted))
                        {
                            return;
                        }

                        //MainWindowViewModel.saveResult = new MainWindowViewModel.SaveResult()
                        //{
                        //    One = ClassificationsSeleted,
                        //    Two = TermsSeleted,
                        //    Three = IsNegate,
                        //    Four = itemNums
                        //};
                        (obj as Window).Tag = true;
                        (obj as Window).Close();
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _SendOk;
            }
        }

        private RelayCommand<ComboBox> _ComBoxLoadedCommand;

        public RelayCommand<ComboBox> ComBoxLoadedCommand
        {
            get
            {
                if (_ComBoxLoadedCommand == null)
                {
                    _ComBoxLoadedCommand = new RelayCommand<ComboBox>((cb) =>
                    {
                        cb.SelectionChanged += Cb_SelectionChanged; ;
                    });
                }
                return _ComBoxLoadedCommand;
            }
            set { _ComBoxLoadedCommand = value; }
        }

        private void Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Terms = SaveTerms[(sender as ComboBox).SelectedItem.ToString()];
        }
    }
}
