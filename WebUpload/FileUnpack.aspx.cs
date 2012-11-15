using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUpload
{
    public partial class FileUnpack : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int times = 0;
            CPKWrap.libukey.UKeyOpen(2, "11111111", out times);
        }

        protected void ButtonMing_Click(object sender, EventArgs e)
        {
            int times = 0;
            CPKWrap.libukey.UKeyOpen(2, "11111111", out times);

            string id = "107568";
            string matrix = Server.MapPath("/matrix/cguard.cn.pkm");
            string signInfo = string.Empty;
            string envelope = string.Empty;
            this.TextBoxMi.Text = FilePack.Pack(id, matrix, this.TextBoxMing.Text, out signInfo, out envelope);
            this.TextBoxEnvelope.Text = envelope;
        }

        protected void ButtonMi_Click(object sender, EventArgs e)
        {
            this.TextBoxMing.Text =FilePack.UnPack(this.TextBoxMi.Text, this.TextBoxEnvelope.Text);
        }

        protected void ButtonChange_Click(object sender, EventArgs e)
        {
            int times = 0;
            CPKWrap.libukey.UKeyOpen(2, "11111111", out times);
            this.TextBoxEnvelope.Text = FilePack.GenServerEnvelope("107570", Server.MapPath("/matrix/cguard.cn.pkm"), this.TextBoxEnvelope.Text);
        }
    }
}