using ArcCreate.Jklss.Update.Service;
using Downloader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArcCreate.Jklss.Update
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string updatePath = "";
        public MainWindow()
        {
            InitializeComponent();

            UpdateFile();
        }

        private async Task UpdateFile() 
        {
            await Task.Run(async () =>
            {
                
                if (App.Args.Length == 0)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this.Close();
                        Environment.Exit(0);
                    }));

                }

                updatePath = App.Args[0];

                if (string.IsNullOrEmpty(updatePath))
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this.Close();
                        Environment.Exit(0);
                    }));
                }
                this.Dispatcher.Invoke(new Action(() =>
                {
                    MessageTBlock.Text = "下载中。。。";
                }));
                
                var downloader = new DownLoadController();

                var updateExePath = Directory.GetCurrentDirectory() + @"\update.zip";

                downloader.AddDownLoad(new DownloadInfo() { Url = updatePath, path = updateExePath });

                await downloader.DownloadProgress(1);

                while (!downloader.GetEndDownload()) Thread.Sleep(3000);
                this.Dispatcher.Invoke(new Action(() =>
                {
                    MessageTBlock.Text = "解压中。。。";
                }));
                Thread.Sleep(10000);
                try
                {
                    ZipService.UnZipFile(updateExePath);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                this.Dispatcher.Invoke(new Action(() =>
                {
                    MessageTBlock.Text = "删除下载文件。。。";
                }));
                Thread.Sleep(1000);
                try
                {
                    File.Delete(updateExePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                ProcessStartInfo versionUpdatePrp = new ProcessStartInfo(Directory.GetCurrentDirectory() + @"\ArcCreate BQ编辑器.exe");

                Process newProcess = new Process();
                newProcess.StartInfo = versionUpdatePrp;
                newProcess.Start();
                this.Dispatcher.Invoke(new Action(() =>
                {
                    MessageTBlock.Text = "更新完成！";
                }));
                
                Thread.Sleep(3000);
                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.Close();
                    Environment.Exit(0);
                }));
            });
            
        } 
    }
}
