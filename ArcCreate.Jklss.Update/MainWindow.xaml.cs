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

            if(App.Args.Length == 0)
            {
                Environment.Exit(0);
            }

            updatePath = App.Args[0];

            if (string.IsNullOrEmpty(updatePath))
            {
                Environment.Exit(0);
            }

            UpdateFile().Start();
        }

        private async Task UpdateFile() 
        {
            var downloader = new DownLoadController();

            var updateExePath = Directory.GetCurrentDirectory() + @"\update.zip";

            downloader.AddDownLoad(new DownloadInfo() { Url = updatePath, path = updateExePath });

            await downloader.DownloadProgress(1);

            await Task.Run(() => { while (!downloader.GetEndDownload()) Thread.Sleep(3000); });

            ZipFile.ExtractToDirectory(updateExePath, Directory.GetCurrentDirectory());//解压

            ProcessStartInfo versionUpdatePrp = new ProcessStartInfo(Directory.GetCurrentDirectory() + @"\ArcCreate BQ编辑器.exe");

            Process newProcess = new Process();
            newProcess.StartInfo = versionUpdatePrp;
            newProcess.Start();

            Environment.Exit(0);
        } 
    }
}
