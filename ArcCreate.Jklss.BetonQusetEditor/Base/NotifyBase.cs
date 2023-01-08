using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace ArcCreate.Jklss.BetonQusetEditor.Base
{
    public class NotifyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyChanged([CallerMemberName] string propName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));//委托方法
            }
        }
    }
}
