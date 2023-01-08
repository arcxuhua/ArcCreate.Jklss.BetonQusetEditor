using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
            //注册全局事件
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            const string msg = "程序异常";
            try
            {
                if (args.ExceptionObject is Exception && Dispatcher != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        Exception ex = (Exception)args.ExceptionObject;
                        HandleException(msg, ex);
                    });
                }
            }
            catch (Exception ex)
            {
                HandleException(msg, ex);
            }
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            const string msg = "程序异常";
            try
            {
                HandleException(msg, args.Exception);
                args.Handled = true;
            }
            catch (Exception ex)
            {
                HandleException(msg, ex);
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs args)
        {
            const string msg = "程序异常";
            try
            {
                HandleException(msg, args.Exception);
                args.SetObserved();
            }
            catch (Exception ex)
            {
                HandleException(msg, ex);
            }
        }

        private void HandleException(string msg, Exception ex)
        {
            Exception innerEx = ex;
            while (innerEx != null)
            {
                innerEx = innerEx.InnerException;
            }
            MessageBox.Show($"错误消息：{msg}+\r\n+报错内容：{innerEx.Message}", $"错误提示", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
