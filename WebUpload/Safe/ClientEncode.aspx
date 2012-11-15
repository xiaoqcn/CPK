<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientEncode.aspx.cs" Inherits="WebUpload.Safe.ClientEncode" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div>
            <span>文件：</span>
            <span><asp:FileUpload ID="FileUploadControl" runat="server" /></span>
        </div>
        <div>
            <asp:Button ID="ButtonEncode" runat="server" Text="加密保存" 
                onclick="ButtonEncode_Click" />
        </div>
        <br />
        <br />
        <br />
        <div>
            <span>密钥：</span>
            <span><asp:TextBox ID="TextBoxRandom" Width="1024" runat="server"></asp:TextBox></span>
        </div>
        <div>
            <span>信封：</span>
            <span><asp:TextBox ID="TextBoxNote" Width="1024" runat="server"></asp:TextBox></span>
        </div>
        <div>
            <span>路径：</span>
            <span><asp:TextBox ID="TextBoxPath" Width="1024" runat="server"></asp:TextBox></span>
        </div>
    </div>
    </form>
</body>
</html>
