using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace WebUploadHelper
{
    public class WebHelper
    {
        /// <summary> 
        /// 模拟POST请求：上传图片文件 
        /// </summary> 
        /// <param name="url">提交的地址</param> 
        /// <param name="poststr">发送的文本串   比如：user=eking&pass=123456  </param> 
        /// <param name="fileformname">文本域的名称  比如：name="file"，那么fileformname=file  </param> 
        /// <param name="filepath">上传的文件路径  比如： c:\12.jpg </param> 
        /// <param name="refre">头部的跳转地址</param> 
        /// <returns></returns> 
        public static string HttpUploadFile(string url, string poststr, string fileformname, byte[] fileContent, string filename)
        {
            // 这个可以是改变的，也可以是下面这个固定的字符串 
            string boundary = "----------7d930d1a850658";

            // 创建request对象 
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.ContentType = "multipart/form-data;boundary=" + boundary;
            webrequest.Method = "POST";
            //webrequest.Headers.Add("Cookie: " + cookie);
            webrequest.Referer = url;

            // 构造发送数据
            StringBuilder sb = new StringBuilder();

            // 文本域的数据，将user=eking&pass=123456  格式的文本域拆分 ，然后构造 
            foreach (string c in poststr.Split('&'))
            {
                string[] item = c.Split('=');
                if (item.Length != 2)
                {
                    break;
                }
                string name = item[0];
                string value = item[1];
                sb.Append("--" + boundary);
                sb.Append("\r\n");
                sb.Append("Content-Disposition:form-data; name=\"" + name + "\"");
                sb.Append("\r\n\r\n");
                sb.Append(value);
                sb.Append("\r\n");
            }

            // 文件域的数据
            sb.Append("--" + boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition:form-data; name=\"" + fileformname + "\";filename=\"" + filename + "\"");
            sb.Append("\r\n");

            sb.Append("Content-Type:application/octet-stream");
            sb.Append("image/jpg");
            sb.Append("\r\n\r\n");

            string postHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

            //构造尾部数据 
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            //FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            //long length = postHeaderBytes.Length + fileStream.Length + boundaryBytes.Length;
            long length = postHeaderBytes.Length + fileContent.Length + boundaryBytes.Length;
            webrequest.ContentLength = length;

            Stream requestStream = webrequest.GetRequestStream();

            // 输入头部数据 
            requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

            // 输入文件流数据 
            //byte[] buffer = new Byte[checked((uint)Math.Min(4096, (int)fileStream.Length))];
            //int bytesRead = 0;
            //while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            //    requestStream.Write(buffer, 0, bytesRead);
            requestStream.Write(fileContent, 0, fileContent.Length);
            // 输入尾部数据 
            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
            WebResponse responce = webrequest.GetResponse();
            Stream s = responce.GetResponseStream();
            StreamReader sr = new StreamReader(s);

            //fileStream.Close();
            //fileStream.Dispose();
            // 返回数据流(源码) 
            return sr.ReadToEnd();
        }
    }
}
