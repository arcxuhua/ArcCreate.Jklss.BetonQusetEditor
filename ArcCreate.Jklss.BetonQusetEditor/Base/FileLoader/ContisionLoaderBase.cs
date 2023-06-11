using ArcCreate.Jklss.BetonQusetEditor.ViewModel.BetonQuest;
using ArcCreate.Jklss.Model;
using ArcCreate.Jklss.Model.MainWindow;
using ArcCreate.Jklss.Model.SocketModel;
using ArcCreate.Jklss.Model.ThumbInfoWindow;
using ArcCreate.Jklss.Model.ThumbModel.CommandModel;
using ArcCreate.Jklss.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ArcCreate.Jklss.BetonQusetEditor.Base.FileLoader
{
    public class ContisionLoaderBase
    {
        public string jsons = string.Empty;

        public Thumb getThumb = null;
               
        public static List<ContisionsCmdModel> savecmdModels = null;

        public Dictionary<Thumb, ThumbInfoWindowModel> saveThumbInfoWindowModel = null;

        /// <summary>
        /// 初始化默认的Json文本
        /// </summary>
        /// <returns></returns>
        public async Task<string> Saver()
        {
            var message = new MessageModel()
            {
                IsLogin = SocketModel.isLogin,
                JsonInfo = JsonInfo.GetConditonModel,
                UserName = SocketModel.userName,
                Message = "",
            };

            var jsonMessage = FileService.SaveToJson(message);

            var getMessage = await SocketViewModel.SendRESMessage(MessageClass.Json, jsonMessage, 
                SocketViewModel.socket.LocalEndPoint.ToString(), SocketViewModel.socket.RemoteEndPoint.ToString(),SocketModel.token,true);

            if (getMessage == null || !getMessage.Succese)
            {
                return null;
            }

            var getModel = FileService.JsonToProp<MessageMode>(getMessage.Backs as string);

            if(getModel.Token != SocketModel.token)
            {
                return null;
            }

            var getRealMessage = FileService.JsonToProp<MessageModel>(Encoding.UTF8.GetString(getModel.Message));

            if(getRealMessage == null || getRealMessage.JsonInfo != JsonInfo.GetConditonModel||!getRealMessage.IsLogin)
            {
                return null;
            }

            try
            {
                return getRealMessage.Message;
            }
            catch
            {
                return null;
            }
            
        }

        /// <summary>
        /// 得到相关实体类
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task<List<ContisionsCmdModel>> Loader()
        {
            if (string.IsNullOrEmpty(jsons))
            {
                MessageBox.Show("您的账户发生了严重的错误，请截图联系管理员！", "错误");
                Environment.Exit(0);
            }

            try
            {
                var models = await Task.Run(() => { 
                    return FileService.JsonToProp<List<ContisionsCmdModel>>(jsons); 
                });

                if (models.Count == 0)
                {
                    return null;
                }
                savecmdModels = models;
                return models;
            }
            catch
            {
                return null;
            }
        }
    }
}
