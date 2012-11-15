<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebUpload.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>·¨¶Ü-CPKµÇÂ¼</title>
    <script src="js/Default.aspx.js" type="text/javascript"></script>
</head>
<body>
    <object classid="clsid:A03577BF-AF00-4AB2-AAE1-27435AE01FF4" width="1" height="1" id="FaDnCtrl"></object>
    <form id="form1" runat="server">
        <label for="sid">CPKºÅ£º</label>
        <input type="text" readonly="readonly" id="sid" />
        <br />
        <label for="pin">PINÂë£º</label>
        <input type="password" id="pin" />
        <br />
        <input type="hidden" id="signData" name="signData" value="<%=Session.SessionID %>" />
        <input type="hidden" id="signInfo" name="signInfo" />
        <asp:Button ID="Logon" Text="µÇÂ¼" OnClientClick="Submit()" runat="server" 
            onclick="Logon_Click"/>
    </form>
</body>
</html>
