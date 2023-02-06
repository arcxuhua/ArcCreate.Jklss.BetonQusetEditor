using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Update.Service
{
    public class ZipService
    {
        /// <summary>
        /// 解压ZIP
        /// </summary>
        /// <param name="zipFilePath"></param>
        public static void UnZipFile(string zipFilePath)
        {
            if (!File.Exists(zipFilePath))
            {
                return;
            }
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = System.IO.Path.GetDirectoryName(zipFilePath);
                    string fileName = System.IO.Path.GetFileName(theEntry.Name);

                    // create directory
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if (fileName != String.Empty)
                    {
                        string filepath = directoryName + @"\" + fileName;
                        using (FileStream streamWriter = File.Create(filepath))
                        {
                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
