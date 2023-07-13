using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.BetonQusetEditor.Windows;
using ArcCreate.Jklss.Model.ClientModel;
using ArcCreate.Jklss.Model.SocketModel;
using ArcCreate.Jklss.Services;
using GalaSoft.MvvmLight.Command;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using ComboBox = System.Windows.Controls.ComboBox;
using Color = System.Drawing.Color;
using System.Threading;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest.ClientWindow
{
    public class PayWindowViewModel : NotifyBase
    {
        private PayWindow window;

        public PayWindowViewModel(PayWindow payWindow)
        {
            window = payWindow;
        }

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

        public string PayMessage
        {
            get
            {
                return model.PayMessage;
            }
            set
            {
                model.PayMessage = value;
                NotifyChanged();//当view的值发生改变时通知model值发生了改变
            }
        }

        public string PayMessageColor
        {
            get
            {
                return model.PayMessageColor;
            }
            set
            {
                model.PayMessageColor = value;
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
                    _SelectionChanged = new RelayCommand<ComboBox>(async (cb) =>
                    {

                        var getSel = cb.SelectedIndex;
                        
                        var message = new MessageModel()
                        {
                            IsLogin = SocketModel.isLogin,
                            JsonInfo = JsonInfo.UsePath,
                            UserName = SocketModel.userName,
                            Message = getSel.ToString(),
                            Path = "BuyPointVIPBuy"
                        };

                        var getResult = await SocketViewModel.EazySendRESMessage(message);

                        if (!getResult.Succese)
                        {
                            return;
                        }

                        var outTradeNo = string.Empty;

                        try
                        {
                            var fg = (getResult.Backs as MessageModel).Message.Split('|');

                            var payaddress = fg[0];

                            outTradeNo = fg[1];

                            QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
                            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payaddress, QRCodeGenerator.ECCLevel.L);
                            QRCode qrcode = new QRCode(qrCodeData);
                            var image = qrcode.GetGraphic(5, Color.Black, Color.White, null, 15, 6, false);
                            window.bitmap_Img.Source = ChangeBitmapToImageSource(image);
                        }
                        catch
                        {
                            return;
                        }
                        await CheckPay(outTradeNo);

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

        /// <summary>
        /// 从bitmap转换成ImageSource
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static ImageSource ChangeBitmapToImageSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new System.ComponentModel.Win32Exception();
            }
            return wpfBitmap;
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        private async Task CheckPay(string outTradeNo)
        {
            bool isSuccess = false;

            await Task.Run(async () =>
            {
                while (!isSuccess)
                {
                    Thread.Sleep(1000);

                    var newmessage = new MessageModel()
                    {
                        IsLogin = SocketModel.isLogin,
                        JsonInfo = JsonInfo.UsePath,
                        UserName = SocketModel.userName,
                        Message = outTradeNo,
                        Path = "CheckPointVIPBuy"
                    };

                    var getResult = await SocketViewModel.EazySendRESMessage(newmessage);

                    if (!getResult.Succese)
                    {
                        PayMessageColor = "Red";
                        PayMessage = getResult.Text;
                        continue;
                    }
                    else
                    {
                        PayMessageColor = "Green";
                        PayMessage = (getResult.Backs as MessageModel).Message;

                        isSuccess = true;
                    }
                }
            });
        }
    }
}
