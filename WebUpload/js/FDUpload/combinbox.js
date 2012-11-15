/*
	组合框
	说明：需要配合JQuery使用
	更新记录：
		2012-4-2 创建
*/
function CombinBoxMgr()
{
	this.Config = {
		"EncodeType"		: "GB2312"
		, "CompanyLicensed"	: "武汉命运科技有限公司"
		, "FileFilter"		: "*"//文件类型。所有类型：*。自定义类型：jpg,bmp,png,gif,rar,zip,7z,doc
		, "FileSizeLimit"	: "0"//自定义允许上传的文件大小，以字节为单位。0表示不限制。
		, "FilesLimit"		: 0//文件选择数限制。0表示不限制
		, "AllowMultiSelect": 1//多选开关。1:开启多选。0:关闭多选
		, "RangeSize"		: 131072//文件块大小，以字节为单位。必须为64KB的倍数。推荐大小：128KB。
		, "AppPath"			: ""//网站虚拟目录名称。子文件夹 web
		, "CabPath"			: ""//"http://www.ncmem.com/products/http-uploader5//HttpUploader.cab#version=2,7,47,55490"
		, "UrlCreate"		: "http://localhost:1591/demoAccess/db/ajax_create_fid.aspx"
		, "UrlPost"         : "http://localhost:8405/ReceiveFileBlock.ashx"
		, "UrlProcess"		: "http://localhost:1591/demoAccess/db/ajax_process.aspx"
		, "UrlComplete"		: "http://localhost:1591/demoAccess/db/ajax_complete.aspx"
		, "UrlList"			: "http://localhost:1591/demoAccess/db/ajax_list.aspx"
		, "UrlDel"			: "http://localhost:1591/demoAccess/db/ajax_del.aspx"
		, "ClsidDroper"     : "A03577BF-AF00-4AB2-AAE1-27435AE01FF4"
		, "ClsidUploader"   : "A03577BF-AF00-4AB2-AAE1-27435AE01FF4"
		, "ClsidPartition"  : "A03577BF-AF00-4AB2-AAE1-27435AE01FF4"
        , "IsEncrypt"       :   true
	};
	
	this.ActiveX = {
	    "Uploader"      : "FdWebCtrl.FadnWebCtrl"//HKEY_LOCAL_MACHINE\SOFTWARE\Classes\FdWebCtrl.FadnWebCtrl
		, "Partition"   : "FdWebCtrl.FadnWebCtrl"
	};
	
	//附加参数
	this.Fields = {
		"UserName": "test"
		, "UserPass": "test"
		,"uid":0
		,"fid":0
	};

	this.upMgr = new HttpUploaderMgr(); //文件上传管理器
	//this.flMgr = new FileListerMgr(); //文件列表管理器
	this.upMgr.FileLstMgr = this.flMgr;
	this.upMgr.CombinBox = this;
	this.upMgr.Config = this.Config;
	this.upMgr.ActiveX = this.ActiveX;
	this.upMgr.Fields = this.Fields;
	
//	this.flMgr.UploaderMgr = this.upMgr;
//	this.flMgr.CombinBox = this;
//	this.flMgr.Config = this.Config;
//	this.flMgr.Fields = this.Fields;
	
	//获取加载代码
	this.GetHtml = function()
	{
		var html = '<div class="combinBox">';
		html += '<ul id="cbHeader" class="cbHeader">';
		html += '<li id="liPnlUploader" class="hover">上传新文件</li>';
		//html += '<li id="liPnlFiles" >文件列表</li>';
		html += '</ul>';
		html += '<div class="cbBody" id="cbBody">';
		html += '<ul name="cbItem" class="block"><li id="liUploadPanel"></li></ul>';
		//html += '<ul name="cbItem" class="cbItem"><li id="liListerPanel"></li></ul>';
		html += '</div>';
		html += '</div>';
		return html;
	};
	
	//加载到指定的控件中
	this.LoadInControl = function(ctlID)
	{
		var obj = document.getElementById(ctlID);
		obj.innerHTML = this.GetHtml();
	};
	
	//打开上传面板
	this.OpenPnlUpload = function()
	{
		$("#liPnlUploader").click();
	};
	//打开文件列表面板
//	this.OpenPnlFiles = function()
//	{
//		$("#liPnlFiles").click();
//	};
	
	//初始化
	this.Init = function()
	{
		this.upMgr.LoadInControl("liUploadPanel");
		this.upMgr.Init();
//		this.flMgr.LoadInControl("liListerPanel");
//		this.flMgr.Init();
//		this.flMgr.LoadData();

		$("#cbHeader li").each(function(n)
		{
			if (this.className == "hover")
			{
				cbItemLast = this;
			}

			$(this).click(function()
			{
				$("ul[name='cbItem']").each(function(i)
				{
					this.style.display = i == n ? "block" : "none"; /*确定主区域显示哪一个对象*/
				});
				if (cbItemLast) cbItemLast.className = "";

				if (this.className == "hover")
				{
					this.className = "";
				}
				else
				{
					this.className = "hover";
				}
				cbItemLast = this;
			});
		});
	};
}