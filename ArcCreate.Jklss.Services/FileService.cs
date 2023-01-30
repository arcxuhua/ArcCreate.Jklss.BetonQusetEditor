using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using YamlDotNet.Serialization;
using static ArcCreate.Jklss.Model.SocketModel.SocketModel;

namespace ArcCreate.Jklss.Services
{
    /// <summary>
    /// 密钥类型
    /// </summary>
    public enum KeyType
    {
        /// <summary>
        /// xml类型
        /// </summary>
        XML,

        /// <summary>
        /// pks8类型
        /// </summary>
        PKS8
    }

    /// <summary>
    /// 密钥尺寸(一般都是1024位的)
    /// </summary>
    public enum KeySize
    {
        SMALL = 1024,
        BIG = 2048
    }
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

        /// <summary>
        /// 生成公钥与私钥方法
        /// </summary>
        /// <returns></returns>
        public static RESKeysModel CreateKey(KeyType keyType, KeySize keySize)
        {
            try
            {
                var sKeys = new RESKeysModel();
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider((int)keySize);
                switch (keyType)
                {
                    case KeyType.XML:
                        {
                            //私钥
                            sKeys.PrivetKey = rsa.ToXmlString(true);
                            //公钥
                            sKeys.PublicKey = rsa.ToXmlString(false);
                        }
                        break;
                    default:
                        break;
                }
                return sKeys;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 最大加密长度
        /// </summary>
        private const int MAX_ENCRYPT_BLOCK = 245;

        /// <summary>
        /// 最大解密长度
        /// </summary>
        private const int MAX_DECRYPT_BLOCK = 256;

        /// <summary>
        /// 用私钥给数据进行RSA解密
        /// </summary>
        /// <param name="xmlPrivateKey"></param>
        /// <param name="strEncryptString"></param>
        /// <returns></returns>
        public static string PrivateKeyDecrypt(string xmlPrivateKey, string strEncryptString)
        {
            //加载私钥
            RSACryptoServiceProvider privateRsa = new RSACryptoServiceProvider();
            privateRsa.FromXmlString(xmlPrivateKey);

            //转换密钥
            AsymmetricCipherKeyPair keyPair = DotNetUtilities.GetKeyPair(privateRsa);
            IBufferedCipher c = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding"); //使用RSA/ECB/PKCS1Padding格式

            c.Init(false, keyPair.Private);//第一个参数为true表示加密，为false表示解密；第二个参数表示密钥
            byte[] dataToEncrypt = Convert.FromBase64String(strEncryptString);

            byte[] cache;
            int time = 0;//次数
            int inputLen = dataToEncrypt.Length;
            int offSet = 0;

            MemoryStream outStream = new MemoryStream();
            while (inputLen - offSet > 0)
            {
                if (inputLen - offSet > MAX_DECRYPT_BLOCK)
                {
                    cache = c.DoFinal(dataToEncrypt, offSet, MAX_DECRYPT_BLOCK);
                }
                else
                {
                    cache = c.DoFinal(dataToEncrypt, offSet, inputLen - offSet);
                }
                //写入
                outStream.Write(cache, 0, cache.Length);

                time++;
                offSet = time * MAX_DECRYPT_BLOCK;
            }

            byte[] resData = outStream.ToArray();


            string strDec = Encoding.UTF8.GetString(resData);
            return strDec;
        }

        /// <summary>
        /// 用公钥给数据进行RSA加密
        /// </summary>
        /// <param name="xmlPublicKey"> 公钥(XML格式字符串) </param>
        /// <param name="strDecryptString"> 要加密的数据 </param>
        /// <returns> 解密后的数据 </returns>
        public static string PublicKeyEncrypt(string xmlPublicKey, string strDecryptString)
        {
            //加载公钥
            RSACryptoServiceProvider publicRsa = new RSACryptoServiceProvider();
            publicRsa.FromXmlString(xmlPublicKey);
            RSAParameters rp = publicRsa.ExportParameters(false);

            //转换密钥
            AsymmetricKeyParameter pbk = DotNetUtilities.GetRsaPublicKey(rp);

            IBufferedCipher c = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");
            //第一个参数为true表示加密，为false表示解密；第二个参数表示密钥
            c.Init(true, pbk);


            byte[] DataToDecrypt = Encoding.UTF8.GetBytes(strDecryptString);//获取字节

            byte[] cache;
            int time = 0;//次数
            int inputLen = DataToDecrypt.Length;
            int offSet = 0;
            MemoryStream outStream = new MemoryStream();
            while (inputLen - offSet > 0)
            {
                if (inputLen - offSet > MAX_ENCRYPT_BLOCK)
                {
                    cache = c.DoFinal(DataToDecrypt, offSet, MAX_ENCRYPT_BLOCK);
                }
                else
                {
                    cache = c.DoFinal(DataToDecrypt, offSet, inputLen - offSet);
                }
                //写入
                outStream.Write(cache, 0, cache.Length);

                time++;
                offSet = time * MAX_ENCRYPT_BLOCK;
            }
            byte[] resData = outStream.ToArray();

            string strBase64 = Convert.ToBase64String(resData);
            outStream.Close();
            return strBase64;
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="str">需签名的数据</param>
        /// <returns>签名后的值</returns>
        public static string Sign(string str, string privateKey, SignAlgType signAlgType)
        {
            //根据需要加签时的哈希算法转化成对应的hash字符节
            byte[] bt = Encoding.GetEncoding("utf-8").GetBytes(str);
            byte[] rgbHash = null;
            switch (signAlgType)
            {
                case SignAlgType.SHA256:
                    {
                        SHA256CryptoServiceProvider csp = new SHA256CryptoServiceProvider();
                        rgbHash = csp.ComputeHash(bt);
                    }
                    break;
                case SignAlgType.MD5:
                    {
                        MD5CryptoServiceProvider csp = new MD5CryptoServiceProvider();
                        rgbHash = csp.ComputeHash(bt);
                    }
                    break;
                case SignAlgType.SHA1:
                    {
                        SHA1 csp = new SHA1CryptoServiceProvider();
                        rgbHash = csp.ComputeHash(bt);
                    }
                    break;
                default:
                    break;
            }
            RSACryptoServiceProvider key = new RSACryptoServiceProvider();
            key.FromXmlString(privateKey);
            RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(key);
            formatter.SetHashAlgorithm(signAlgType.ToString());//此处是你需要加签的hash算法，需要和上边你计算的hash值的算法一致，不然会报错。
            byte[] inArray = formatter.CreateSignature(rgbHash);
            return Convert.ToBase64String(inArray);
        }

        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="str">待验证的字符串</param>
        /// <param name="sign">加签之后的字符串</param>
        /// <returns>签名是否符合</returns>
        public static bool Verify(string str, string sign, string publicKey, SignAlgType signAlgType)
        {

            byte[] bt = Encoding.GetEncoding("utf-8").GetBytes(str);
            byte[] rgbHash = null;
            switch (signAlgType)
            {
                case SignAlgType.SHA256:
                    {
                        SHA256CryptoServiceProvider csp = new SHA256CryptoServiceProvider();
                        rgbHash = csp.ComputeHash(bt);
                    }
                    break;
                case SignAlgType.MD5:
                    {
                        MD5CryptoServiceProvider csp = new MD5CryptoServiceProvider();
                        rgbHash = csp.ComputeHash(bt);
                    }
                    break;
                case SignAlgType.SHA1:
                    {
                        SHA1 csp = new SHA1CryptoServiceProvider();
                        rgbHash = csp.ComputeHash(bt);
                    }
                    break;
                default:
                    break;
            }
            RSACryptoServiceProvider key = new RSACryptoServiceProvider();
            key.FromXmlString(publicKey);
            RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(key);
            deformatter.SetHashAlgorithm(signAlgType.ToString());
            byte[] rgbSignature = Convert.FromBase64String(sign);
            if (deformatter.VerifySignature(rgbHash, rgbSignature))
                return true;
            return false;
        }

        /// <summary>
        /// 签名算法类型
        /// </summary>
        public enum SignAlgType
        {
            /// <summary>
            /// sha256
            /// </summary>
            SHA256,

            /// <summary>
            /// md5
            /// </summary>
            MD5,

            /// <summary>
            /// sha1
            /// </summary>
            SHA1
        }
    }
}
