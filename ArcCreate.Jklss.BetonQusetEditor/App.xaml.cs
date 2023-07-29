using ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;

namespace ArcCreate.Jklss.BetonQusetEditor
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 初始化一个<see cref="App"/>类型的新实例
        /// </summary>
        public App()
        {
            
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            var socketViewModel = new SocketViewModel();

            await socketViewModel.StarSocketTCP();

            base.OnStartup(e);
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("报错啦！请将报错截图发送给苦逼程序员！\n"+e.Exception.ToString(), "异常");

            e.Handled = true;
        }
    }
}
