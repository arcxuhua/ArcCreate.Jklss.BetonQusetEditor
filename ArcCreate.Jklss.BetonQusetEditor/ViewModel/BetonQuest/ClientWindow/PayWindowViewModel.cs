using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.Model.ClientModel;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest.ClientWindow
{
    public class PayWindowViewModel : NotifyBase
    {
        private static PayModel model = new PayModel();
        public string ImageFile
        {
            get
            {
                if (string.IsNullOrEmpty(model.ImageFile))
                {
                    return "/img/pay/99.jpg";
                }
                return model.ImageFile;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    model.ImageFile = "/img/pay/99.jpg";
                }
                model.ImageFile = value;
                NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        private RelayCommand<ComboBox> _SelectionChanged;

        public RelayCommand<ComboBox> SelectionChanged
        {
            get
            {
                if (_SelectionChanged == null)
                {
                    _SelectionChanged = new RelayCommand<ComboBox>((cb) =>
                    {
                        var getSel = cb.SelectedIndex;

                        switch (getSel)
                        {
                            case 0:
                                ImageFile = "/img/pay/10.jpg";
                                break;
                            case 1:
                                ImageFile = "/img/pay/20.jpg";
                                break;
                            case 2:
                                ImageFile = "/img/pay/99.jpg";
                                break;
                            case 3:
                                ImageFile = "/img/pay/199.jpg";
                                break;
                            case 4:
                                ImageFile = "/img/pay/99.jpg";
                                break;
                            case 5:
                                ImageFile = "/img/pay/199.jpg";
                                break;
                            case 6:
                                ImageFile = "/img/pay/299.jpg";
                                break;
                        }
                    });
                }
                return _SelectionChanged;
            }
            set { _SelectionChanged = value; }
        }

        public CommandBase _CloseCommand;
        public CommandBase CloseCommand
        {
            get
            {
                if (_CloseCommand == null)
                {
                    _CloseCommand = new CommandBase();
                    _CloseCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        (obj as Window).Close();
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _CloseCommand;
            }
        }

        public CommandBase _NarrowCommand;
        public CommandBase NarrowCommand
        {
            get
            {
                if (_NarrowCommand == null)
                {
                    _NarrowCommand = new CommandBase();
                    _NarrowCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        (obj as Window).WindowState = WindowState.Minimized;
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _NarrowCommand;
            }
        }
    }
}
