using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace WebUpload
{
    /// <summary>
    /// ReceiveFileBlock 的摘要说明
    /// </summary>
    public class ReceiveFileBlock : IHttpHandler
    {
        public string GetPath(HttpContext context,string fileName)
        {
            //var fid = context.Request["fid"];
            var dir = context.Server.MapPath("/Files/");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            return dir + fileName;
        }

        public static string matrix = "/matrix/cguard.cn.pkm"; 

        public void ProcessRequest(HttpContext context)
        {
            log(context);
            var postfile = context.Request.Files["filename"];
            var lenth = context.Request["length"];
            var range = context.Request["range"];
            if (postfile == null)
            {
                context.Response.Write("无上传文件,表单name:filename");
                return;
            }
            if (string.IsNullOrEmpty(lenth))
            {
                context.Response.Write("表单name:length为空");
                return;
            }
            if (string.IsNullOrEmpty(range))
            {
                context.Response.Write("表单name:range为空");
                return;
            }
            Save(context);
            string[] ranges = range.Split('-');
            var start = long.Parse(ranges[0]);
            var end = long.Parse(ranges[1]);
            //var count = end -start+1;
            if (postfile.InputStream.Length <= 0)
            {
                return;
            }
            byte[] byteArr = new byte[(int)postfile.InputStream.Length];
            postfile.InputStream.Read(byteArr, 0, byteArr.Length);

            var path = GetPath(context,postfile.FileName);
            try
            {
                FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs.Seek(start, SeekOrigin.Begin);
                fs.Write(byteArr, 0, byteArr.Length);
                fs.Flush();
                fs.Close();
                if (end == long.Parse(lenth))
                { 
                    var e_sign = context.Request["e-sign"];
                    e_sign = e_sign.Replace("\r\n","");
                    var e_signByte = Convert.FromBase64String(e_sign);
                    if (CPKWrap.libcpkapi.CPK_Verify_File(context.Server.MapPath(matrix), path, e_signByte, e_signByte.Length) != 0)
                    {
                        context.Response.StatusCode = 501;//验证签名失败;文件损坏，重传
                        context.Response.Write("验证签名失败;文件损坏，重传");
                        return;
                    }
                }
            }
            catch {
                context.Response.StatusCode = 500;
                context.Response.Write("写文件失败！");
                return;
            }
            context.Response.ContentType = "text/plain";
            var r = string.Format("时间：{0}\t文件：{1}\t长度：{2}\tRange：{3}", DateTime.Now.ToString("yyyy-MM-ddHH:mm:ss"), postfile.FileName, lenth, range);
            context.Response.Write(r);
        }

        public void log(HttpContext context)
        {
            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
            
            strBuilder.AppendFormat("\r\n\r\n{0}:{1}\r\n","时间", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strBuilder.AppendLine("----------------------------------------");
            string fmt = "【表单】{0}\t:{1}\r\n";
            try {
                for(int i=0;i<context.Request.Files.Count;i++)
                {
                    HttpPostedFile hpf = context.Request.Files[i];
                    strBuilder.AppendFormat("【文件】{0}\t:\t名称:{1}\t长度:{2}\t\r\n", context.Request.Files.GetKey(i), hpf.FileName, hpf.ContentLength);
                }
                for (int i = 0; i < context.Request.Form.Count; i++)
                {
                    strBuilder.AppendFormat(fmt,context.Request.Form.GetKey(i),context.Request.Form[i]);
                }
                
                File.AppendAllText(GetPath(context, "log.txt"), strBuilder.ToString());
            }
            catch { }
        }

        public void Save(HttpContext context)
        {
            try
            {
                HttpPostedFile hpf = context.Request.Files["filename"];
                var length = context.Request["length"];
                var range = context.Request["range"];
                var fid = context.Request["fid"];
                var crc = context.Request["crc"];
                var sign = context.Request["sign"];
                var envelop = context.Request["envelop"];
                var path = GetPath(context, hpf.FileName);
                
                string database = context.Server.MapPath("/App_Data/Demo.db");
                var connStrBuilder = SQLiteHelper.GetConnection(database);
                var sql = "select * from FileInfo where fname='{0}'";
                sql = string.Format(sql, path);
                System.Data.DataTable dt =SQLiteHelper.ExecuteTable(sql, connStrBuilder);
                if (dt.Rows.Count>0)
                {
                    return;
                }

                sql = "insert into FileInfo(id,fid,fname,length,crc,sign,envelop,uploadtime) values(null,'{0}','{1}',{2},'{3}','{4}','{5}','{6}')";
                sql = string.Format(sql, fid,path , length, crc, sign, envelop, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                SQLiteHelper.ExecuteNonQuery(sql, connStrBuilder);
            }
            catch { }
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