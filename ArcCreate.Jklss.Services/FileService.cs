using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Xml;
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
        /// 获取文件地址中的文件名
        /// </summary>
        /// <returns></returns>
        public static string GetFilePathToFileName(string path)
        {
            var folder = new FileInfo(path);

            var fg = folder.Name.Split(new string[] { "." }, System.StringSplitOptions.RemoveEmptyEntries);

            return fg[0];
        }

        /// <summary>
        /// 获取文件夹下所有文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> GetDisAllFile(string path)
        {
            var list = new List<string>();

            var folder = new DirectoryInfo(path);

            foreach (var item in folder.GetFiles("*.yml"))
            {
                list.Add(item.FullName);
            }

            return list;
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

        public static T YamlToProp<T>(string yaml_path)
        {
            FileInfo fi = new FileInfo(yaml_path);

            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            StreamReader yamlReader = File.OpenText(yaml_path);
            Deserializer yamlDeserializer = new Deserializer();

            //读取持久化对象  
            T info = yamlDeserializer.Deserialize<T>(yamlReader);
            yamlReader.Close();

            return info;
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
