using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest;
using ArcCreate.Jklss.BetonQusetEditor.Windows;
using ArcCreate.Jklss.BetonQusetEditor.Windows.Market;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.Market;
using ArcCreate.Jklss.Model.SocketModel;
using ArcCreate.Jklss.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.Market
{
    public partial class MarketWindowViewModel:ObservableObject
    {
        #region 构造函数

        public MarketWindowViewModel(MarketWindow window)
        {
             this.window = window;

            
        }

        

        #endregion

        #region 字段与属性

        private MarketWindow window;

        [ObservableProperty]
        private string _UserImage = "/ArcCreate.png";

        [ObservableProperty]
        private string _UserName = string.Empty;

        [ObservableProperty]
        private string _UserPoints = string.Empty;

        [ObservableProperty]
        private string _UserType = "/img/vip/normal.png";

        [ObservableProperty]
        private string _UserAccets = string.Empty;

        [ObservableProperty]
        private string _MessagesShowText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<BqMarketInfoDataViewModel> _BqMarketInfoData = new ObservableCollection<BqMarketInfoDataViewModel>();

        private int nowPage = 0;

        private Dictionary<int,List<BqMarketInfoDataViewModel>> saveCommditys = new Dictionary<int, List<BqMarketInfoDataViewModel>>();

        #endregion

        #region 命令
        private bool isDoing = false;
        private async void DataMarketScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange < 0)
            {
                return;
            }

            if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
            {
                if (isDoing == true)
                {
                    return;
                }

                isDoing = true;

                var getNums = (BqMarketInfoData.Count/8)+1;

                if (BqMarketInfoData.Count % 8 != 0)
                {
                    getNums++;
                }

                var getBack = await GetMarketCommodity(window.MarketTable.SelectedIndex, new PageList { page = getNums, size = 8 });

                if (!getBack.Succese)
                {
                    ShowMessage(getBack.Text);
                    isDoing = false;
                    return;
                }

                if (string.IsNullOrWhiteSpace(getBack.Backs.ToString().TrimStart('[').TrimEnd(']')))
                {
                    isDoing = false;
                    return;
                }

                var jarray = FileService.JsonToProp<List<arccreate_market_commodity>>(getBack.Backs.ToString());

                foreach (var item in jarray)
                {
                    var newShowModel = new BqMarketInfoDataViewModel();

                    newShowModel.BqMarketInfoImage = "/img/title/normal.png";

                    newShowModel.BqMarketInfoName = item.name;

                    newShowModel.BqMarketInfoUserName = "作者ID：" + item.userid;

                    newShowModel.BqMarketInfoNeedPoint = "购买积分:" + item.need_points;

                    newShowModel.BqMarketInfoStar = item.star;

                    newShowModel.BqMarketInfoIntroduction = "简介：" + item.introduction;

                    this.BqMarketInfoData.Add(newShowModel);
                    saveCommditys[nowPage].Add(newShowModel);
                }

                isDoing = false;
            }

        }

        [RelayCommand()]
        private async Task MarketGridSelectionChanged(TabControl tabControl)
        {
            if(nowPage == tabControl.SelectedIndex)
            {
                return;
            }

            this.BqMarketInfoData.Clear();

            nowPage = tabControl.SelectedIndex;

            if (saveCommditys.ContainsKey(nowPage))
            {
                this.BqMarketInfoData = new ObservableCollection<BqMarketInfoDataViewModel>(saveCommditys[nowPage]);
            }
            else
            {
                saveCommditys.Add(nowPage, new List<BqMarketInfoDataViewModel>());

                var getBack = await GetMarketCommodity(tabControl.SelectedIndex, new PageList { page = 1, size = 16 });

                if (!getBack.Succese)
                {
                    ShowMessage(getBack.Text);

                    return;
                }

                if (string.IsNullOrWhiteSpace(getBack.Backs.ToString().TrimStart('[').TrimEnd(']')))
                {
                    ShowMessage("这里空空如也等待您的上传");

                    return;
                }

                var jarray = FileService.JsonToProp<List<arccreate_market_commodity>>(getBack.Backs.ToString());

                foreach (var item in jarray)
                {
                    var newShowModel = new BqMarketInfoDataViewModel();

                    newShowModel.BqMarketInfoImage = "/img/title/normal.png";

                    newShowModel.BqMarketInfoName = item.name;

                    newShowModel.BqMarketInfoUserName = "作者ID：" + item.userid;

                    newShowModel.BqMarketInfoNeedPoint = "购买积分:" + item.need_points;

                    newShowModel.BqMarketInfoStar = item.star;

                    newShowModel.BqMarketInfoIntroduction = "简介：" + item.introduction;

                    this.BqMarketInfoData.Add(newShowModel);
                    saveCommditys[nowPage].Add(newShowModel);
                }
            }
            

        }

        [RelayCommand()]
        private async void Loaded()
        {
            window.DataMarketScroll.ScrollChanged += DataMarketScroll_ScrollChanged;
            await Task.Run(async () =>
            {
                var getBack = await GetUserInfo();

                if (!getBack.Succese)
                {
                    ShowMessage(getBack.Text);

                    window.Close();
                }

                var getUserModel = getBack.Backs as UserInfo;

                if (getUserModel == null)
                {
                    ShowMessage("获取用户信息失败");

                    window.Close();
                }

                if (string.IsNullOrEmpty(getUserModel.UserImage))
                {
                    this.UserImage = "/ArcCreate.png";
                }

                if (string.IsNullOrEmpty(getUserModel.UserName))
                {
                    this.UserName = "昵称：无";
                }

                this.UserPoints = "积分：" + getUserModel.UserPoints.ToString();

                if (getUserModel.UserAccets.Length > 12)
                {
                    this.UserAccets = "[ " + getUserModel.UserAccets.Substring(0, 9) +"..." + " ]";
                }
                else
                {
                    this.UserAccets = "[ " + getUserModel.UserAccets + " ]";
                }
                

                if (getUserModel.UserGroup == 2)
                {
                    this.UserType = $"/img/vip/vip{getUserModel.UserVip}.png";
                }
                else if(getUserModel.UserGroup == 1)
                {
                    this.UserType = $"/img/vip/normal.png";
                }
                else if(getUserModel.UserGroup == 0)
                {
                    this.UserType = $"/img/vip/admin.png";
                }
            });
        }

        [RelayCommand()]
        private void Close()
        {
            window.Close();
        }

        [RelayCommand()]
        private void Narrow()
        {
            window.WindowState = WindowState.Minimized;
        }

        [RelayCommand()]
        private void Minimize()
        {
            if (window.WindowState == WindowState.Maximized)
            {
                window.WindowState = WindowState.Normal;
            }
            else if (window.WindowState == WindowState.Normal)
            {
                window.WindowState = WindowState.Maximized;
            }
        }

        [RelayCommand()]
        private async Task EditUserInfo()
        {

        }

        [RelayCommand()]
        private async Task Pay()
        {
            PayWindow pay = new PayWindow();

            pay.Show();
        }

        [RelayCommand()]
        private async Task SelfUpdate()
        {

        }

        [RelayCommand()]
        private async Task BetonQuestMarket()
        {

        }

        #endregion

        #region 具体方法

        private async Task<ReturnModel> GetUserInfo()
        {
            var result = new ReturnModel();

            var message = new MessageModel()
            {
                IsLogin = SocketModel.isLogin,
                UserName = SocketModel.userName,
                JsonInfo = JsonInfo.UsePath,
                Message = "",
                Path = "GetUserInfo"
            };

            var getResult = await SocketViewModel.EazySendRESMessage(message);

            if (!getResult.Succese)
            {
                result.SetError(getResult.Text);
                return result;
            }

            try
            {
                result.SetSuccese("获取信息成功",FileService.JsonToProp<UserInfo>((getResult.Backs as MessageModel).Message));
                return result;
            }
            catch
            {
                result.SetError("获取信息失败");
                return result;
            }
        }


        private async Task<ReturnModel> GetMarketCommodity(int type, PageList page)
        {
            var result = new ReturnModel();

            var message = new MessageModel()
            {
                IsLogin = SocketModel.isLogin,
                UserName = SocketModel.userName,
                JsonInfo = JsonInfo.UsePath,
                Message = FileService.SaveToJson(page),
                Path = "GetMarketCommodity",
                Other = type.ToString(),
            };

            var getResult = await SocketViewModel.EazySendRESMessage(message);

            if (!getResult.Succese)
            {
                result.SetError(getResult.Text);
                return result;
            }

            try
            {
                result.SetSuccese("获取信息成功", (getResult.Backs as MessageModel).Message);
                return result;
            }
            catch
            {
                result.SetError("这里空空如也，等待您的上传");
                return result;
            }
        }

        private async void ShowMessage(string txt)
        {
            this.MessagesShowText = txt;
            AnimationBase.Appear(window.Message);

            await Task.Run(() =>
            {
                Thread.Sleep(3000);
            });

            AnimationBase.Disappear(window.Message);
        }
        #endregion


    }

    public partial class BqMarketInfoDataViewModel : ObservableObject
    {
        #region 字段与属性

        [ObservableProperty]
        private string _BqMarketInfoImage = "/ArcCreate.png";

        [ObservableProperty]
        private string _BqMarketInfoName = string.Empty;

        [ObservableProperty]
        private string _BqMarketInfoUserName = string.Empty;

        [ObservableProperty]
        private string _BqMarketInfoNeedPoint = string.Empty;

        [ObservableProperty]
        private int _BqMarketInfoStar = 0;

        [ObservableProperty]
        private string _BqMarketInfoIntroduction = string.Empty;


        #endregion

        #region 命令

        [RelayCommand()]
        private async Task BqMarketBuy()
        {

        }

        #endregion

        #region 具体方法

        #endregion
    }
}
