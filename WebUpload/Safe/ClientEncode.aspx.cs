using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace WebUpload.Safe
{
    public partial class ClientEncode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonEncode_Click(object sender, EventArgs e)
        {
            //1、上传文件
            #region 
            if (!this.FileUploadControl.HasFile)
            {
                R("上传文件为空");
                return;
            }
            var dir = GetServerDir();
            var path = Path.Combine(dir,FileUploadControl.FileName);
            FileUploadControl.SaveAs(path);

            #endregion

            //2、取密钥
            #region 

            int rnLen = 16;
            byte[] rn = new byte[rnLen];
            int rv = CPKWrap.libukey.UKeyGetRandom(rn, ref rnLen);
            if (rv != 0)
            {
                R("获取随机数失败");
                return;
            }
            string rnStr = Convert.ToBase64String(rn, 0, rnLen, Base64FormattingOptions.None);

            #endregion

            //3、加密文件
            #region 
            FileStream fs = File.OpenRead(path);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();

            int cipherLen = data.Length*2;
            byte[] cipher = new byte[cipherLen];
            rv = CPKWrap.libcpkapi.Sym_Encrypt("AES-128-ECB", data, data.Length, rn, rn.Length, cipher, ref cipherLen);
            if (rv != 0)
            {
                R("加密文件失败");
                return;
            }
            var path_enc = path+".enc";
            fs = File.Open(path_enc, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fs.Write(cipher, 0, cipherLen);
            fs.Close();
            File.Delete(path);

            #endregion

            //4、数字信封
            #region
            
            int envelopeLen = 2048;
            byte[] envelope  =  new byte[envelopeLen];
            rv = CPKWrap.libukey.UKeySM1Encrypt(0, rn, rnLen, envelope, ref envelopeLen);
            if (rv != 0)
            {
                R("产生数字信封失败");
                return;
            }
            string envelopeBase64 = Convert.ToBase64String(envelope,0,envelopeLen, Base64FormattingOptions.None);
            #endregion

            this.TextBoxNote.Text = envelopeBase64;
            this.TextBoxPath.Text = path_enc;
            this.TextBoxRandom.Text = rnStr;
        }

        protected void R(string msg)
        {
            Response.Write(msg);
        }


        protected string GetServerDir()
        {
            return @"F:\FD\CPK\WorkCopy\Bench\Server\";
        }
    }
}