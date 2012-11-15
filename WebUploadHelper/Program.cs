using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WebUploadHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            TestDES();
        }


        /// <summary>
        /// 获取设备序列号
        /// </summary>
        public static void GetKeySn()
        { 
            int snLen = 6;
            byte[] sn = new byte[snLen];
            int rn = CPKWrap.libukey.UKeyGetSn(sn, ref snLen);
            string str = System.Text.Encoding.ASCII.GetString(sn, 0, snLen);
            Console.WriteLine(str);
            Console.ReadLine();
        }

        public static void TestDES()
        {
            string path = @"D:\picture\ky.jpg";
            //int keyLen = 32;
            //byte[] key = new byte[keyLen];
            //int rv = CPKWrap.libukey.UKeyGetRandom(key,ref keyLen);
            string str = "1234567890poiuytrewqasdfghjklmnb";
            var key = System.Text.ASCIIEncoding.ASCII.GetBytes(str);
            var key1 = System.Text.ASCIIEncoding.ASCII.GetBytes(str);
            string path_enc = Encode(path, key);
            Decode(path_enc, key1);
        }

        public static string Encode(string path, byte[] key)
        {
            int rv = -1;
            var path_enc = path + ".enc";
            FileStream fs_read = File.Open(path, FileMode.Open, FileAccess.Read);
            FileStream fs_write = File.Open(path_enc, FileMode.OpenOrCreate, FileAccess.Write,FileShare.Read);

            int dataBufferLen = 128 * 1024;
            byte[] dataBuffer = new byte[dataBufferLen];
            int encodeBufferLen = 128 * 1024;
            byte[] encodeBuffer = new byte[encodeBufferLen];

            int readLen = 0;
            while ((readLen = fs_read.Read(dataBuffer, 0, dataBuffer.Length)) != 0)
            {
                rv = CPKWrap.libcpkapi.Sym_Encrypt("AES-256-CFB", dataBuffer, readLen, key, key.Length, encodeBuffer, ref encodeBufferLen);
                if (rv != 0)
                {
                    return string.Empty;
                }
                fs_write.Write(encodeBuffer, 0, encodeBufferLen);
                fs_write.Flush();
            }
            fs_read.Close();
            fs_write.Close();
            fs_read.Dispose();
            fs_write.Dispose();
            return path_enc;
        }

        public static void Decode(string path_enc, byte[] key)
        {
            int rv = -1;
            var path_enc_rmvb = path_enc + ".jpg";
            FileStream fs_read = File.Open(path_enc, FileMode.Open, FileAccess.Read,FileShare.Read);
            FileStream fs_write = File.Open(path_enc_rmvb, FileMode.OpenOrCreate, FileAccess.Write);

            int encodeBufferLen = 128 * 1024;
            byte[] encodeBuffer = new byte[encodeBufferLen];
            int dataBufferLen = 128* 1024;
            byte[] dataBuffer = new byte[dataBufferLen];

            int readLen = 0;
            while ((readLen = fs_read.Read(encodeBuffer, 0, encodeBuffer.Length)) != 0)
            {
                rv = CPKWrap.libcpkapi.Sym_Decrypt("AES-256-CFB", encodeBuffer, readLen, key, key.Length, dataBuffer, ref dataBufferLen);
                if (rv != 0)
                {
                    break;
                }
                fs_write.Write(dataBuffer, 0, dataBufferLen); 
                fs_write.Flush();
            }
            fs_read.Close();
            fs_write.Close();
            fs_read.Dispose();
            fs_write.Dispose();
        }



        //测试文件上传
        public static void Demo()
        {
            #region 1、读取文件

            string filename = "C程序设计语言.pdf";
            string path = @"D:\456\" + filename;
            
            #endregion

            #region 2、拆分

            byte[] buffer = new byte[4096];
            FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read);
            if (fs == null)
            {
                Console.Write("error");
                fs.Close();
                return;
            }

            int bytesRead = 0;

            var url = "http://localhost:8405/ReceiveFileBlock.ashx";
            var form = "length={0}&range={1}-{2}";
            string result = string.Empty;
            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
            {
                //fs.Read(buffer, 0, bytesRead);
                result = WebHelper.HttpUploadFile(url, string.Format(form, fs.Length, fs.Position-bytesRead, fs.Position-1), "filename", buffer,filename);
                Console.WriteLine(result);
            }
            fs.Close();
            Console.ReadLine();
            #endregion
        }
    }
}
