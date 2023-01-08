using System;
using System.Windows.Input;

namespace ArcCreate.Jklss.BetonQusetEditor.Base
{
    public class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;//此事件为继承ICommand接口下的事件

        public bool CanExecute(object parameter)//用于确认命令是否处于执行状态，如果为ture则会触发Execute方法
        {
            return true;
        }

        public void Execute(object parameter)//执行命令
        {
            DoExecute?.Invoke(parameter);
        }
        public Action<object> DoExecute { get; set; }
    }
}
