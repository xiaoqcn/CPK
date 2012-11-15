using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace WebUpload.Safe
{
    public partial class ServerDecode : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void R(string msg)
        {
            Response.Write(msg);
        }

        protected void ButtonDecode_Click(object sender, EventArgs e)
        {
            string en = NewNote(this.TextBoxNote_Old.Text);
            if (string.IsNullOrEmpty(en))
            {
                return;
            }
            string path = this.TextBoxPath.Text;
            Decode(en, path);
        }

        protected string NewNote(string oldNote)
        {
            //1、打开旧信封
            #region
            byte[] data = Convert.FromBase64String(oldNote);

            int bufferLen = data.Length * 2;
            byte[] buffer = new byte[bufferLen];
            int rv = CPKWrap.libukey.UKeySM1Decrypt(0, data, data.Length, buffer, ref bufferLen);
            if (rv != 0)
            {
                R("打开信封失败！");
                return string.Empty;
            }
            #endregion
            //2、发送新信封
            #region
            var matrix_path = Server.MapPath("/matrix/cguard.cn.pkm");
            var address = System.Text.Encoding.ASCII.GetBytes("107568");

            var new_enLen = buffer.Length*5;
            byte[] new_en = new byte[new_enLen];
            rv = CPKWrap.libcpkapi.CPK_Encrypt_Data(matrix_path, address, buffer, bufferLen, new_en, ref new_enLen);
            if (rv != 0)
            {
                R("新信封密封失败");
                return string.Empty;
            }
            #endregion

            return Convert.ToBase64String(new_en, 0, new_enLen, Base64FormattingOptions.None);
        }

        protected void Decode(string note,string path)
        {
            System.Security.Cryptography.AesCryptoServiceProvider aes = new System.Security.Cryptography.AesCryptoServiceProvider();
            aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            //1、打开信封
            #region 
            
            int times=0;
            var data = Convert.FromBase64String(note);
            
            int rv = CPKWrap.libukey.UKeyOpen(2, "11111111",out times);
            if(rv!=0)
            {
                R("打开key失败："+times);
                return;
            }
            int rnLen = data.Length*2;
            byte[] rn = new byte[rnLen];
            rv =  CPKWrap.libukey.UKeyDecrypt(0, data, data.Length, rn, ref rnLen);
            if (rv != 0)
            {
                R("打开信封失败");
                return;
            }
            CPKWrap.libukey.UKeyClose();
            #endregion
            //2、解密文件
            FileStream fs = File.OpenRead(path);
            byte[] filedata = new byte[fs.Length];
            fs.Read(filedata, 0, filedata.Length);
            fs.Close();

            int fileClearLen = filedata.Length*2;
            byte[] fileClear = new byte[fileClearLen];
            rv = CPKWrap.libcpkapi.Sym_Decrypt("AES-128-ECB", filedata, filedata.Length, rn, rnLen, fileClear, ref fileClearLen);
            if (rv != 0)
            {
                R("解密失败");
                return;
            }
            var clearPath = path.Substring(0, path.LastIndexOf('.'));
            fs = File.Open(clearPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fs.Write(fileClear, 0, fileClearLen);
            fs.Flush();
            fs.Close();
            File.Delete(path);
        }
    }
}