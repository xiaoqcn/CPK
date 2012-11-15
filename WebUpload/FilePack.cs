using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace WebUpload
{
    public class FilePack
    {
        #region 路径映射

        public static string GetMatrixPath(HttpServerUtility server,string relatePath)
        {
            return server.MapPath(relatePath);
        }

        /// <summary>
        /// 获取明文文件路径对应的密文文件路径
        /// </summary>
        /// <param name="unpackpath">明文文件路径</param>
        /// <returns></returns>
        public static string GetPackPath(string unpackpath)
        {
            return unpackpath + ".enc";  
        }

        /// <summary>
        /// 获取密文文件对应的明文文件路径
        /// </summary>
        /// <param name="packpath">密文文件</param>
        /// <returns></returns>
        public static string GetUnPackPath(string packpath)
        {
            return packpath + ".txt";
            if (string.IsNullOrEmpty(packpath))
            {
                return string.Empty;
            }
            if (!packpath.EndsWith(".enc"))
            {
                return string.Empty;
            }
            int dot = packpath.LastIndexOf('.');
            if (dot < 0)
            {
                return string.Empty;
            }
            return packpath.Substring(0, dot);
        }

        #endregion

        #region 对称加解密


        public static string Encode(string path, byte[] key)
        {
            int rv = -1;
            var path_enc = GetPackPath(path);
            FileStream fs_read = File.Open(path, FileMode.Open, FileAccess.Read);
            FileStream fs_write = File.Open(path_enc, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

            int dataBufferLen = 128 * 1024;
            byte[] dataBuffer = new byte[dataBufferLen];
            int encodeBufferLen = 128 * 1024+16;
            byte[] encodeBuffer = new byte[encodeBufferLen];

            int readLen = 0;
            while ((readLen = fs_read.Read(dataBuffer, 0, dataBuffer.Length)) != 0)
            {
                rv = CPKWrap.libcpkapi.Sym_Encrypt("AES-256-CBC", dataBuffer, readLen, key, key.Length, encodeBuffer, ref encodeBufferLen);
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

        public static string Decode(string path_enc, byte[] key)
        {
            int rv = -1;
            var path = GetUnPackPath(path_enc);
            FileStream fs_read = File.Open(path_enc, FileMode.Open, FileAccess.Read, FileShare.Read);
            FileStream fs_write = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);

            int encodeBufferLen = 128 * 1024+16;
            byte[] encodeBuffer = new byte[encodeBufferLen];
            int dataBufferLen = 128 * 1024;
            byte[] dataBuffer = new byte[dataBufferLen];

            int readLen = 0;
            while ((readLen = fs_read.Read(encodeBuffer, 0, encodeBuffer.Length)) != 0)
            {
                rv = CPKWrap.libcpkapi.Sym_Decrypt("AES-256-CBC", encodeBuffer, readLen, key, key.Length, dataBuffer, ref dataBufferLen);
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
            return path;
        }

        #endregion

        #region 数字信封
        /// <summary>
        /// 产生数字信封--不用key，需要指定key号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="matrixpath"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GenEnvelope(string id,string matrixpath, string dataStr)
        {
            var address = System.Text.Encoding.ASCII.GetBytes(id);
            var data = Convert.FromBase64String(dataStr);
            
            int bufferLen = 512;
            byte[] buffer = new byte[bufferLen];
            int rv = CPKWrap.libcpkapi.CPK_Encrypt_Data(matrixpath,address,data,data.Length,buffer,ref bufferLen);
            if (rv != 0)
            {
                return string.Empty;
            }
            return Convert.ToBase64String(buffer,0,bufferLen,Base64FormattingOptions.None);
        }

        /// <summary>
        /// 打开数字信封-需要打开key
        /// </summary>
        /// <param name="envelope">数字信封-base64（无换行）</param>
        /// <returns>信封内容（base64无换行）</returns>
        public static string OpenEnvelope(string envelope)
        {
            byte[] envelopeBuffer = Convert.FromBase64String(envelope);

            int bufferLen = 512;
            byte[] buffer = new byte[bufferLen];
            int rv =CPKWrap.libukey.UKeyDecrypt(0x00, envelopeBuffer, envelopeBuffer.Length, buffer, ref bufferLen);
            if (rv != 0)
            {
                return string.Empty;
            }
            return Convert.ToBase64String(buffer, 0, bufferLen, Base64FormattingOptions.None);
        }

        #endregion

        /// <summary>
        /// 打包文件
        /// </summary>
        /// <param name="id">数字信封接收人</param>
        /// <param name="matrixpath">使用的矩阵</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="signInfo">加密前签名</param>
        /// <param name="envelope">数字信封</param>
        /// <returns>加密后文件路径</returns>
        public static string Pack(string id,string matrixpath,string filePath, out string signInfo, out string envelope)
        {
            string result = string.Empty;

            #region 产生对称密钥

            int rnLen = 32;
            byte[] key = new byte[rnLen];
            int rv = CPKWrap.libukey.UKeyGetRandom(key, ref rnLen);
            if (rv != 0)
            {
                signInfo = string.Empty;
                envelope = string.Empty;
                return result;
            }
            string keyStr = Convert.ToBase64String(key, 0, rnLen, Base64FormattingOptions.None);
            Log("base64", keyStr);
            Log("加密路径", filePath);

            #endregion

            #region 加密文件
            result = Encode(filePath, key);
            if (string.IsNullOrEmpty(result))
            {
                signInfo = string.Empty;
                envelope = string.Empty;
                return result;
            }
            #endregion

            int bufferLen = 512;
            byte[] buffer = new byte[bufferLen];
            #region 原文件签名
            rv = CPKWrap.libukey.UKeySignFile(0x00, filePath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), buffer, ref bufferLen);
            if (rv != 0)
            {
                signInfo = string.Empty;
                envelope = string.Empty;
                return result;
            }
            signInfo = Convert.ToBase64String(buffer, 0, bufferLen, Base64FormattingOptions.None);
            signInfo = string.Empty;
            #endregion

            #region 数字信封
            envelope = GenEnvelope(id, matrixpath, keyStr);
            if (string.IsNullOrEmpty(envelope))
            {
                signInfo = string.Empty;
                envelope = string.Empty;
                return result;
            }

            #endregion
            return result;
        }

        /// <summary>
        /// 产生服务器的数字信封
        /// </summary>
        /// <param name="id"></param>
        /// <param name="matrixpath"></param>
        /// <param name="ClientEnvelope"></param>
        /// <returns></returns>
        public static string GenServerEnvelope(string id,string matrixpath, string ClientEnvelope)
        {
            string key =OpenEnvelope(ClientEnvelope);
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }
            return GenEnvelope(id, matrixpath, key);
        }

        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="filePath">密文文件路径</param>
        /// <param name="envelope">数字信封</param>
        /// <returns>明文文件路径</returns>
        public static string UnPack(string filePath,string envelope)
        {
            #region 解开信封-拿到密钥

            string keyStr = OpenEnvelope(envelope);
            if (string.IsNullOrEmpty(keyStr))
            {
                return string.Empty;
            }
            byte[] key = Convert.FromBase64String(keyStr);
            #endregion

            #region 解密文件
            Log("Base64",keyStr);
            Log("密文路径",filePath);
            return Decode(filePath, key);

            #endregion
        }

        public static void Log(string k,string v)
        {
            string fmt = string.Format("{0}:{1}\r\n", k, v);
            File.AppendAllText(@"D:\123.txt", fmt);
        }
    }
}