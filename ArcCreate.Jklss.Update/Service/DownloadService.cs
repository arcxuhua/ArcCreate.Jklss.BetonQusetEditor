using Downloader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Update.Service
{
    public class DownLoadService
    {
        DownloadService _downloader;
        public DownloadIntermation intermation = new DownloadIntermation();
        public DownLoadService()
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 5;
            var downloadOpt = new DownloadConfiguration()
            {
                BufferBlockSize = 10240, // usually, hosts support max to 8000 bytes, default values is 8000
                ChunkCount = 1, // file parts to download, default value is 1
                MaximumBytesPerSecond = 10240 * 10240, // download speed limited to 1MB/s, default values is zero or unlimited
                MaxTryAgainOnFailover = int.MaxValue, // the maximum number of times to fail
                OnTheFlyDownload = false, // caching in-memory or not? default values is true
                ParallelDownload = false, // download parts of file as parallel or not. Default value is false
                TempDirectory = "C:\\temp", // Set the temp path for buffering chunk files, the default path is Path.GetTempPath()
                Timeout = 1000, // timeout (millisecond) per stream block reader, default values is 1000
                RequestConfiguration = // config and customize request headers
                {
                    Accept = "*/*",
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    CookieContainer =  new CookieContainer(), // Add your cookies
                    Headers = new WebHeaderCollection(), // Add your custom headers
                    KeepAlive = false,
                    ProtocolVersion = HttpVersion.Version11, // Default value is HTTP 1.1
                    UseDefaultCredentials = false,
                    UserAgent = $"DownloaderSample/{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}"
                }
            };
            _downloader = new DownloadService(downloadOpt);
            // Provide any information about download progress, like progress percentage of sum of chunks, total speed, average speed, total received bytes and received bytes array to live streaming.
            _downloader.DownloadProgressChanged += OnDownloadProgressChanged;
            // Download completed event that can include occurred errors or cancelled or download completed successfully.
            _downloader.DownloadFileCompleted += OnDownloadFileCompleted;
        }

        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                intermation.Status = e.Error.Message;
                intermation.StatusId = -1;
            }
        }

        private void OnDownloadProgressChanged(object sender, Downloader.DownloadProgressChangedEventArgs e)
        {
            intermation.Speed = Math.Round(e.BytesPerSecondSpeed / 512 / 1024, 1);
            intermation.Progress = (int)e.ProgressPercentage;
        }

        public async void StartDownload()
        {
            await _downloader.DownloadFileTaskAsync(Url, FilePath);
            intermation.Status = "下载中";
            intermation.StatusId = 1;
        }
        string Url, FilePath;
        public void BuildDownload(string Url, string FilePath)
        {
            this.Url = Url;
            this.FilePath = FilePath;
            intermation.StatusId = 0;
        }
    }
    public class DownloadIntermation
    {

        public string Status { get; internal set; }
        public int StatusId { get; internal set; }
        public double Speed { get; internal set; }
        public int Progress { get; internal set; }
        public string FileName { get; internal set; }
    }

    public class DownLoadController
    {
        public List<DownloadInfo> redownload = new List<DownloadInfo>();
        public int EndDownload = 0;
        public int DuckEndDownload = 0;
        public double Speed = 0;
        public double Progress = 0;

        //public event DownloadProgressChangedEvent DownloadProgressChanged;
        //public delegate void DownloadProgressChangedEvent(DownloadInfoMation Log);

        int ADindex = 0;
        private DownloadInfo AssignedDownload()
        {
            if (ADindex == redownload.Count) return null;
            ADindex++;
            return redownload[ADindex - 1];
        }

        /// <summary>
        /// 添加下载队列
        /// </summary>
        /// <param name="info"></param>
        public void AddDownLoad(DownloadInfo info)
        {
            this.redownload.Add(info);
        }

        /// <summary>
        /// 启动下载线程
        /// </summary>
        /// <param name="threadNum">线程数</param>
        public async Task DownloadProgress(int threadNum)
        {
            List<DownLoadService> files = new List<DownLoadService>();

            for (int i = 0; i < threadNum; i++)
            {
                DownloadInfo download = AssignedDownload();
                if (download != null)
                {
                    DownLoadService fileDownloader = new DownLoadService();
                    fileDownloader.BuildDownload(download.Url, download.path);
                    fileDownloader.StartDownload();
                    files.Add(fileDownloader);
                }
            }
            if (files.Count == 0)
            {
                Progress = 100;
                DuckEndDownload = redownload.Count;
                return;
            }
            await Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    int end = 0;
                    for (int i = 0; i < files.Count; i++)
                    {
                        Speed += files[i].intermation.Speed;
                        if (files[i].intermation.StatusId == -1)//下载进程错误
                        {
                            end++;
                        }
                        if (files[i].intermation.Progress == 100)//下载完成
                        {
                            end++;
                        }
                    }

                    if (end >= files.Count)
                    {
                        DuckEndDownload += files.Count;
                        EndDownload += files.Count;
                        Speed = 1;
                        Progress += 0.5 / redownload.Count * files.Count;

                        DownloadProgress(threadNum);//递归
                        return;
                    }
                    Thread.Sleep(200);
                }
            });
            Thread thread = new Thread(Process_OutputDataReceived);
            thread.Start();
        }
        public bool GetEndDownload()
        {
            return DuckEndDownload == redownload.Count ? true : false;
        }
        private void Process_OutputDataReceived()
        {
            DownloadInfoMation intermation = new DownloadInfoMation();
            while (!this.GetEndDownload())
            {
                intermation.FinishFile = this.EndDownload;
                intermation.AllFile = redownload.Count;
                intermation.Progress = this.Progress;
                intermation.Speed = this.Speed;
                //DownloadProgressChanged(intermation);
                Thread.Sleep(1000);
            }
            intermation.FinishFile = this.EndDownload;
            intermation.AllFile = redownload.Count;
            intermation.Progress = 100;
            intermation.Speed = 2;
            //DownloadProgressChanged(intermation);
        }
    }
    public class DownloadInfo
    {
        /// <summary>
        /// 下载网址
        /// </summary>
        public string Url { get; internal set; }
        /// <summary>
        /// 下载路径
        /// </summary>
        public string path { get; internal set; }
        internal string name { get; set; }
        internal string mainClass { get; set; }
    }

    public class DownloadInfoMation
    {
        /// <summary>
        /// 下载速度
        /// </summary>
        public double Speed { get; internal set; }
        /// <summary>
        /// 进度
        /// </summary>
        public double Progress { get; internal set; }
        /// <summary>
        /// 完成文件
        /// </summary>
        public int FinishFile { get; internal set; }
        /// <summary>
        /// 所有文件
        /// </summary>
        public int AllFile { get; internal set; }
    }
}
