using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUpload.Safe
{
    /// <summary>
    /// ServerAddress 的摘要说明
    /// </summary>
    public class ServerAddress : IHttpHandler
    {

        public void RW(HttpContext context,string msg)
        { 
            context.Response.ContentType = "text/plain";
            context.Response.Write(msg);
        }

        public void ProcessRequest(HttpContext context)
        {
            int snLen = 16;
            byte[] sn = new byte[snLen];

            try
            {
                int rv = CPKWrap.libukey.UKeyGetSn(sn, ref snLen);
                if (rv != 0)
                {
                    RW(context, "获取服务器序列号失败！");
                    return;
                }
                string snStr = System.Text.Encoding.ASCII.GetString(sn, 0, snLen);
                RW(context, snStr);
            }
            catch
            {
                RW(context, "获取服务器序列号异常！");
            }

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