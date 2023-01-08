using Newtonsoft.Json;
using System.IO;
using YamlDotNet.Serialization;

namespace ArcCreate.Jklss.Services
{
    public class FileService
    {
        /// <summary>
        /// 获取文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsHaveFile(string path)
        {
            if (File.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 将实体类转为Yml的string文本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="allConfig"></param>
        /// <returns></returns>
        public static string SaveToYaml<T>(T allConfig)
        {
            var serializer = new SerializerBuilder().Build();
            
            var yaml = serializer.Serialize(allConfig);

            return yaml;
        }

        /// <summary>
        /// Json序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="allconfig"></param>
        /// <returns></returns>
        public static string SaveToJson<T>(T allconfig)
        {
            return JsonConvert.SerializeObject(allconfig);
        }

        /// <summary>
        /// Json反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonToProp<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 获取文件目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileDirectory(string path)
        {
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// 输出文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static bool ChangeFile(string path,string txt)
        {
            try
            {
                var disPath = Path.GetDirectoryName(path);
                Directory.CreateDirectory(disPath);
                File.WriteAllText(path, txt);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取文件内容
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileText(string path)
        {
            try
            {
                var txt = File.ReadAllText(path);

                return txt;
            }
            catch
            {
                return "";
            }
        }
    }
}
