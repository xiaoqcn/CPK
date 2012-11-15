using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace WebUpload.OP
{
    /// <summary>
    /// UploadFile 的摘要说明
    /// </summary>
    public class UploadFile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            #region 1、读取文件

            //string filename = "20110901265.jpg";
            //string path = @"D:\picture\Pictures\" + filename;
            
            string path = context.Request["filename"];
            string filename = path.Substring(path.LastIndexOf('\\')+1);
            string len = context.Request["filelength"];
            string pos = context.Request["pos"];
            int p = int.Parse(pos);

            string format = "completed:{0},PostedLength:{1}";

            #endregion

            #region 2、拆分

            byte[] buffer = new byte[4096];
            FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read);
            if (fs == null)
            {
                context.Response.Write("{" + string.Format(format, "true", fs.Length) + "}");
                fs.Close();
                return;
            }

            int bytesRead = 0;

            var host = context.Request.Url.Host;
            var port = context.Request.Url.Port;
            var url = string.Format("http://{0}:{1}/ReceiveFileBlock.ashx",host,port);
            var form = "length={0}&range={1}-{2}";
            string result = string.Empty;
            fs.Seek(p, SeekOrigin.Begin);
            if (p >= fs.Length)
            {
                context.Response.Write("{"+ string.Format(format,"true",fs.Length)+"}");
                fs.Close();
                return;
            }
            bytesRead = fs.Read(buffer, 0, buffer.Length);
            result = WebHelper.HttpUploadFile(url, string.Format(form, fs.Length, fs.Position - bytesRead, fs.Position - 1), "filename", buffer, filename);
            var r = fs.Position;
            fs.Close(); 
            context.Response.Write("{" + string.Format(format, "false", r) + "}");
            #endregion
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}