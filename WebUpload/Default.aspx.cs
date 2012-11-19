using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUpload
{
    public partial class Default : System.Web.UI.Page
    {
        public System.Text.Encoding encode = System.Text.Encoding.ASCII;

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CPK_Sign_Data"] == null)
            {
                Session["CPK_Sign_Data"] = Session.SessionID;
            }
            
        }

        public void R(string k, string v)
        {
            Response.Write(string.Format("{0}：{1}<br/>", k, v));
        }

        public void T(string t)
        {
            Response.Write(string.Format("<h2>{0}</h2>---------------------------------------<br />", t));
        }

        //验证登录
        public void V(string signDataStr, string signInfoStr)
        {
            T("处理数据");
            R("输入", string.Empty);
            R("被签信息", signDataStr);
            R("签名信息", signInfoStr);
            R("输出", string.Empty);

            var rn = 0;
            var signInfoLen = 512;
            byte[] signInfo = new byte[signInfoLen];
            var signInfoBytes = encode.GetBytes(signInfoStr);
            rn = CPKWrap.libcpkapi.Base64Decode(signInfoStr, signInfoBytes.Length, signInfo, ref signInfoLen);
            if (rn != 0)
            {
                R("Base64解码", CPKWrap.ErrorCode.codeDic[rn]);
                return;
            }
            //signInfo = Convert.FromBase64String(signInfoStr);

            //读取的缓冲区
            int dL = 1024;
            byte[] d = new byte[dL];

            #region 获取矩阵标示
            rn = CPKWrap.libcpkapi.CPK_GetSignMatrixId(signInfo, signInfo.Length, d, ref dL);
            if (rn != 0)
            {
                R("矩阵标示", CPKWrap.ErrorCode.codeDic[rn]);
                return;
            }
            R("矩阵标示", encode.GetString(d, 0, dL));
            #endregion

            dL = 1024;
            #region 获取签名标示
            rn = CPKWrap.libcpkapi.CPK_GetSignerId(signInfo, signInfo.Length, d, ref dL);
            if (rn != 0)
            {
                R("签名标示",CPKWrap.ErrorCode.codeDic[rn]);
                return;
            }
            R("签名标示", encode.GetString(d, 0, dL));
            #endregion

            dL = 1024;
            #region 获取签名时间
            rn = CPKWrap.libcpkapi.CPK_GetSignTime(signInfo, signInfo.Length, d, ref dL);
            if (rn != 0)
            {
                R("签名时间", CPKWrap.ErrorCode.codeDic[rn]);
                return;
            }
            R("签名时间", encode.GetString(d, 0, dL));
            #endregion


            var path = Server.MapPath(@"/matrix/cguard.cn.pkm");
            var signData = encode.GetBytes(signDataStr);//Convert.FromBase64String(signDataStr);//
            //验证签名
            rn = CPKWrap.libcpkapi.CPK_Verify_Data(path, signData, signData.Length, signInfo, signInfoLen);
            if (rn != 0)
            {
                R("验证签名", CPKWrap.ErrorCode.codeDic[rn]);
                return;
            }
            R("验证签名", CPKWrap.ErrorCode.codeDic[rn]);
            Response.Redirect("Upload.aspx");
        }

        //生成签名数据--无中间件时的测试方法
        public string GenSignInfo()
        {
            if (Session["CPK_Sign_Data"] == null)
            {
                Session["CPK_Sign_Data"] = Session.SessionID;
            }
            string SignDataStr = (string)Session["CPK_Sign_Data"];
            byte[] signData = encode.GetBytes(SignDataStr);
            string signDate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss");
            //var signDateByte = encode.GetBytes(signDate);

            T("当前SessionID");
            R("SessionID", Session.SessionID);

            T("原始信息");
            R("被签数据", SignDataStr);
            R("签名时间", signDate);

            //if (IsPostBack)
            //{
            //    return string.Empty;
            //}
            var rn = 0;

            #region 打开设备

            int times = 0;
            string pin = "11111111";

            rn = CPKWrap.libukey.UKeyOpen(0x02, pin, out times);
            if (rn != 0)
            {
                R(CPKWrap.ErrorCode.codeDic[rn], "剩余次数" + times);
                return string.Empty;
            }
            #endregion

            #region 签名


            int cL = 1024;
            byte[] c = new byte[cL];
            rn = CPKWrap.libukey.UKeySign(0x00, signData, signData.Length, signDate, c, ref cL);
            if (rn != 0)
            {
                R(CPKWrap.ErrorCode.codeDic[rn], string.Empty);
                return string.Empty;
            }

            var signInfo = Convert.ToBase64String(c, 0, cL, Base64FormattingOptions.None);
            R("签名信息", signInfo);
            //this.SignInfo.Width = 1200;
            //this.SignInfo.Text = signInfo;

            #endregion

            #region 关闭设备
            rn = CPKWrap.libukey.UKeyClose();
            #endregion
            return signInfo;
        }

        protected void Logon_Click(object sender, EventArgs e)
        {
            var signInfo =  Request["signInfo"];//GenSignInfo();//
            signInfo = signInfo.Replace("\r\n", "");
            if (string.IsNullOrWhiteSpace(signInfo))
            {
                return;
            }
            R("客户端被签信息", Request["signData"]);
            var SignData = (string)Session["CPK_Sign_Data"];//Session.SessionID;//
            V(SignData, signInfo);
        }
    }
}