<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileList.aspx.cs" Inherits="WebUpload.FileList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript">
        function Change() {
            var fdCtrl = document.getElementById("FaDnCtrl");
            var env = document.getElementById("TextBoxEnvelope").value;
            var id = "107570"; //根据服务器设置
            var rv= fdCtrl.CPKOpenDevice(1, "11111111");
            var r = fdCtrl.CPKExchKeyEnvo(env, id);
            if (!r) {
                r == '';
            }
            document.getElementById("TextBoxEnvelope").value = r;
        }
    </script>
</head>
<body>
    <object classid="clsid:A03577BF-AF00-4AB2-AAE1-27435AE01FF4" width="1" height="1" id="FaDnCtrl"></object>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label2" runat="server" Text="信封："></asp:Label>
        <asp:TextBox ID="TextBoxEnvelope" ClientIDMode="Static" runat="server"></asp:TextBox><br />
        <asp:Label ID="Label1" runat="server" Text="文件："></asp:Label>
        <asp:TextBox ID="TextBoxPath" runat="server"></asp:TextBox>
        <asp:Button ID="ButtonDe" runat="server" OnClientClick="Change()" Text="解密" />
    </div>
    </form>
</body>
</html>
