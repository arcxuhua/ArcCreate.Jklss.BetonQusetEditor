using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.BetonQusetEditor.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.ClientWindow
{
    public class RegisterWindowViewModel : NotifyBase
    {
        
        public CommandBase _LoginCommand;
        public CommandBase LoginCommand
        {
            get
            {
                if (_LoginCommand == null)
                {
                    _LoginCommand = new CommandBase();
                    _LoginCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        var window = new LoginWindow();
                        window.Show();
                        (obj as Window).Close();
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _LoginCommand;
            }
        }

        public CommandBase _PayCommand;
        public CommandBase PayCommand
        {
            get
            {
                if (_PayCommand == null)
                {
                    _PayCommand = new CommandBase();
                    _PayCommand.DoExecute = new Action<object>(obj =>//回调函数
                    {
                        var window = new PayWindow();
                        window.Show();
                    });//obj是窗口CommandParameter参数传递的值，此处传递为窗口本体
                }
                return _PayCommand;
            }
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
                        Environment.Exit(0);
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
