<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileUnpack.aspx.cs" Inherits="WebUpload.FileUnpack" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>解压文件</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="LabelEnvelope" runat="server" Text="数字信封"></asp:Label>
        <asp:TextBox ID="TextBoxEnvelope" runat="server"></asp:TextBox>
    </div>
    <hr />
    <div><!--加密-->
        <asp:Label ID="LabelMing" runat="server" Text="明文文件路径"></asp:Label>
        <asp:TextBox ID="TextBoxMing" runat="server"></asp:TextBox>
        <asp:Button ID="ButtonMing" runat="server" Text="加密" 
            onclick="ButtonMing_Click" />
    </div>
    <div>
        <asp:Button ID="ButtonChange" runat="server" Text="换信封" 
            onclick="ButtonChange_Click" />
    </div>
    <div><!--解密-->
        <asp:Label ID="LabelMi" runat="server" Text="密文文件路径"></asp:Label>
        <asp:TextBox ID="TextBoxMi" runat="server"></asp:TextBox>
        <asp:Button ID="ButtonMi" runat="server" Text="解密" 
            onclick="ButtonMi_Click" />
    </div>
    </form>
</body>
</html>
