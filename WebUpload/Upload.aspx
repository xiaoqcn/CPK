<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="WebUpload.Upload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>上传页面</title>
    <link href="js/FDUpload/css/HttpUploader.css" rel="stylesheet" type="text/css" />
    <script src="js/FDUpload/combinbox.js" type="text/javascript"></script>
    <script src="js/FDUpload/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="js/FDUpload/FileLister.js" type="text/javascript"></script>
    <script src="js/FDUpload/HttpUploader.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        var cbItemLast = null;
        var cbMgr = new CombinBoxMgr();

        $(document).ready(function () {
            cbMgr.LoadInControl("FilePanel");
            cbMgr.Init();
            HttpUploaderMgrInstance.IsKeyOpened = HttpUploaderMgrInstance.AVX.CPKOpenDevice(1, "11111111");
            HttpUploaderMgrInstance.IsKeyOpened = 0;
        });

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="FilePanel"></div>
    </form>
</body>
</html>
