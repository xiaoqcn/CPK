/// <reference path="jquery-1.3.2.min.js" />

/*
	版权所有 2009-2012 北京法盾科技有限公司
	保留所有权利
	联系信箱：zhaojq@qqdao.org
*/
//删除元素值
Array.prototype.remove = function(val)
{
	for (var i = 0, n = 0; i < this.length; i++)
	{
		if (this[i] != val)
		{
			this[n++] = this[i]
		}
	}
	this.length -= 1
}

//全局对象
var HttpUploaderMgrInstance = null; //上传管理器实例

/*
	2012-11-5 文件管理类
	属性：
		UpFileList
*/
function HttpUploaderMgr()
{
	this.Domain = "http://" + document.location.host;

	HttpUploaderMgrInstance = this;
	this.FileLstMgr = null;
	this.CombinBox = null;
	this.FileFilter = new Array(); //文件过滤器
	this.UploaderListCount = 0; 	//上传项总数
	this.UploaderList = new Object(); //上传项列表
	this.UploadIdList = new Array(); //正在上传的ID列表
	this.CompleteList = new Array(); //已上传完的HttpUploader列表
	
	//初始化路径
	this.InitPath = function()
	{
		this.Config["CabPath"] = this.Domain + this.Config["AppPath"] + this.Config["CabPath"];
		this.Config["PostUrl"] = this.Domain + this.Config["AppPath"] + this.Config["PostUrl"];
	};

	//获取加载字符串。
	this.GetHtml = function () {
	    //加载拖拽控件
	    var acx = "";
	    //acx += '<object id="objFileDroper" classid="clsid:' + this.Config["ClsidDroper"] + '"';
	    //acx += ' codebase="' + this.Config["CabPath"] + '" width="192" height="192" >';
	    //acx += '</object>';
	    //自动安装CAB
	    acx += '<div width="1" height="1">';
	    //文件上传控件
	    acx += '<object id="objHttpUpLoader" classid="clsid:' + this.Config["ClsidUploader"] + '"';
	    acx += ' codebase="' + this.Config["CabPath"] + '" width="1" height="1" ></object>';
	    //文件夹选择控件
	    //acx += '<object id="objHttpUploaderPartition" classid="clsid:' + this.Config["ClsidPartition"] + '"';
	    //acx += ' codebase="' + this.Config["CabPath"] + '" width="1" height="1" ></object>';
	    acx += '</div>';
	    //上传列表项模板
	    acx += '<div class="UploaderItem" id="UploaderTemplate">';
	    acx += '<div class="UploaderItemLeft">';
	    acx += '<div class="FileInfo">';
	    acx += '<div class="FileName top-space">HttpUploader程序开发.pdf</div>';
	    acx += '<div class="FileSize" child="1">100.23MB</div>';
	    acx += '</div>';
	    acx += '<div class="ProcessBorder top-space"><div class="Process"></div></div>';
	    acx += '<div class="PostInf top-space">已上传:15.3MB 速度:20KB/S 剩余时间:10:02:00</div>';
	    acx += '</div>';
	    acx += '<div class="UploaderItemRight">';
	    acx += '<div class="BtnInfo"><span class="Btn">取消</span></div>';
	    acx += '<div class="ProcessNum">35%</div>';
	    acx += '</div>';
	    acx += '</div>';
	    //分隔线
	    acx += '<div class="Line" id="FilePostLine"></div>';
	    //上传列表
	    acx += '<div id="UploaderPanel">';
	    //acx += '<div class="header">上传文件</div>';
	    acx += '<div class="toolbar">';
	    acx += '<input id="btnAddFiles" type="button" value="选择多个文件" />';
	    acx += '<input id="btnAddFolder" type="button" value="选择文件夹" />';
	    //acx += '<input id="btnPasteFile" type="button" value="粘贴文件" />';
	    acx += '</div>';
	    acx += '<div class="content">';
	    acx += '<div id="FilePostLister"></div>';
	    acx += '</div>';
	    acx += '<div class="footer">';
	    acx += '<a href="javascript:void(0)" class="Btn" id="lnkClearComplete">清除已完成文件</a> ';
	    acx += '<a href="javascript:void(0)" class="Btn" id="lnkMgrBtn" >上传</a> ';
	    acx += '<a href="javascript:void(0)" class="Btn" id="lnkTest">测试功能</a> ';
	    acx += '</div>';
	    acx += '</div>';
	    return acx;
	};

	//安全检查，在用户关闭网页时自动停止所有上传任务。
	this.SafeCheck = function () {
	    window.attachEvent("onbeforeunload", function () {
	        if (HttpUploaderMgrInstance.UploadIdList.length > 0) {
	            event.returnValue = "您还有程序正在运行，确定关闭？";
	        }
	    });

	    window.attachEvent("onunload", function () {
	        if (HttpUploaderMgrInstance.UploadIdList.length > 0) {
	            HttpUploaderMgrInstance.StopAll();
	        }
	    });
	};

	this.Load = function () {
	    document.write(this.GetHtml());
	    this.SafeCheck();
	};

	//加载到指定控件
	this.LoadInControl = function(ctlID)
	{
		var obj = document.getElementById(ctlID);
		obj.innerHTML = this.GetHtml();
		this.SafeCheck();
	};
	
	//初始化，一般在window.onload中调用
	this.Init = function () {
	    var mgr = this;
	    this.IsKeyOpened = -1;//设备是否打开

	    this.AVX = document.getElementById("objHttpUpLoader");
	    this.AVX.SetUploadUrl(this.Config["UrlPost"]);
	    this.AVX.SetSelectFileMaxSize(this.Config["FileSizeLimit"]);
	    this.AVX.EnableEncryptFile(this.Config["IsEncrypt"]);
	    //var rv = this.AVX.CPKOpenDevice(1, "11111111");
	    //选中文件
	    HttpUploaderMgrInstance.AVX.attachEvent("OnSelectFile", function (file, size) {
	        mgr.AddFile(file, size);
	    });
	    //文件块上传完成
	    HttpUploaderMgrInstance.AVX.attachEvent("OnHttpUpload", function (File, USize, Error) {
	        HttpUploader_Process(File, USize, Error);
	    });
	    //控件状态
	    HttpUploaderMgrInstance.AVX.attachEvent("OnUploadStatusChange", function (State) {
	        HttpUploader_StateChanged(State);
	    });
	    //需要续传的文件
	    HttpUploaderMgrInstance.AVX.attachEvent("OnGetUnFinishUploadFile", function (File, Size, USize) {
	        HttpUploaderMgrInstance.AddResumeFile(File, Size, USize);
	    });

	    this.UploaderListDiv = document.getElementById("FilePostLister");
	    this.UploaderTemplateDiv = document.getElementById("UploaderTemplate");

	    HttpUploaderMgrInstance.AVX.GetUnFinishUploadFile();

	    //添加多个文件
	    obj = document.getElementById("btnAddFiles");
	    obj.attachEvent("onclick", function () {
	        if (HttpUploaderMgrInstance.IsKeyOpened !== 0) {
	            alert("请输入pin码，打开key");
	            return;
	        }
	        mgr.OpenFileDialog();
	    });
	    //添加文件夹
	    obj = document.getElementById("btnAddFolder");
	    if (obj) obj.attachEvent("onclick", function () {
	        mgr.OpenFolderDialog();
	    });
	    obj = document.getElementById("lnkMgrBtn");
	    if (obj) {
	        obj.attachEvent("onclick", MgrBtnClick);
	    }
	    //清空已完成文件
	    obj = document.getElementById("lnkClearComplete");
	    if (obj) obj.attachEvent("onclick", function () { mgr.ClearComplete(); });
	    //测试
	    obj = document.getElementById("lnkTest");
	    if (obj) obj.attachEvent("onclick", function () {
	        var rv = HttpUploaderMgrInstance.AVX.CPKGetCertId(1);
	        alert(rv);
	    });
	};
	
	//清除已完成文件
	this.ClearComplete = function()
	{
		for(var i = 0 ; i < this.CompleteList.length ; ++i)
		{
			this.Delete(this.CompleteList[i].FileID);
		}
        this.CompleteList.length = 0;
        HttpUploaderMgrInstance.AVX.ClearFinishUploadFile();
	};

	//上传队列是否已满
	this.IsPostQueueFull = function()
	{
		//目前只支持同时上传1个文件
		if (this.UploadIdList.length > 0)
		{
			return true;
		}
		return false;
	};

	//添加一个上传ID
	this.AppendUploadId = function (fid) {
	    this.UploadIdList.push(fid);
	};

	/*
	从当前上传ID列表中删除指定项。
	此函数将会重新构造一个Array
	*/
	this.RemoveUploadId = function(fid)
	{
		if (this.UploadIdList.length < 1) return;
		
		for (var i = 0, l = this.UploadIdList.length; i < l; ++i)
		{
			if (this.UploadIdList[i] == fid)
			{
				this.UploadIdList.remove(fid);
			}
		}
	};

	//停止所有上传项
	this.StopAll = function()
	{
        this.AVX.PauseUpload();
	};

	/*
	添加到上传列表
	参数
	fid 上传项ID
	uploaderItem 新的上传对象
	*/
	this.AppenToUploaderList = function(fid, uploaderItem)
	{
		this.UploaderList[fid] = uploaderItem;
		this.UploaderListCount++;
	};

	/*
	添加到上传列表层
	1.添加到上传列表层
	2.添加分隔线
	参数：
	fid 上传项ID
	uploaderDiv 新的上传信息层
	返回值：
		新添加的分隔线
	*/
	this.AppendToUploaderListDiv = function(fid, uploaderDiv)
	{
		this.UploaderListDiv.appendChild(uploaderDiv);

		var split = "<div class=\"Line\" style=\"display:block;\" id=\"FilePostLine" + fid + "\"></div>";
		this.UploaderListDiv.insertAdjacentHTML("beforeEnd", split);
		var obj = document.getElementById("FilePostLine" + fid);
		return obj;
	};

	//传送当前队列的第一个文件
	this.PostFirst = function () {
	    //上传队列不为空
	    if (this.UploaderListCount > 0) {
	        //禁用取消
	        for (i in this.UploaderList) {
	            this.UploaderList[i].pButton.style.display = "none";
	        }
	        this.AVX.StartUpload();
	    } else {
	        alert("请添加上传文件");
	    }
	};

	/*
	验证文件名是否存在
	参数:
	[0]:文件名称
	*/
	this.Exist = function () {
	    var fn = arguments[0];

	    for (a in this.UploaderList) {
	        if (this.UploaderList[a].LocalFile == fn)
	        {
	        	return true;
	        }
	    }
	    return false;
	};

	/*
	根据ID删除上传任务
	参数:
	fid 上传项ID。唯一标识
	*/
	this.Delete = function (fid) {
	    var obj = this.UploaderList[fid];
	    if (null == obj) return;
	    this.RemoveUploadId(fid);
	    try {
	        HttpUploaderMgrInstance.AVX.DelFileFromUploadList(obj.LocalFile);
	    } catch (e) {

	    }

	    //删除div
	    this.UploaderListDiv.removeChild(obj.div);
	    //删除分隔线
	    this.UploaderListDiv.removeChild(obj.spliter);
	    obj.LocalFile = "";
	};

	/*
		设置文件过滤器
		参数：
			filter 文件类型字符串，使用逗号分隔(exe,jpg,gif)
	*/
	this.SetFileFilter = function(filter)
	{
		this.FileFilter.length = 0;
		this.FileFilter = filter.split(",");
	};

	/*
	判断文件类型是否需要过滤
	根据文件后缀名称来判断。
	*/
	this.NeedFilter = function(fname)
	{
		if (this.FileFilter.length == 0) return false;
		var exArr = fname.split(".");
		var len = exArr.length;
		if (len > 0)
		{
			for (var i = 0, l = this.FileFilter.length; i < l; ++i)
			{
				//忽略大小写
				if (this.FileFilter[i].toLowerCase() == exArr[len - 1].toLowerCase())
				{
					return true;
				}
			}
		}
		return false;
	};
	
	//打开文件选择对话框
	this.OpenFileDialog = function () {
	    HttpUploaderMgrInstance.AVX.OpenSelectFileDlg();

	};
	
	//打开文件夹选择对话框
	this.OpenFolderDialog = function () {
	    HttpUploaderMgrInstance.AVX.OpenSelectPathDlg();
	};

	//粘贴文件
	this.PasteFiles = function()
	{

	};
	
	//检查续传大小是否合法。必须为全数字
	this.IsNumber = function(num)
	{
		var reg = /\D/;
		return reg.test(num);
	};
	
	/*
		添加一个续传文件
		参数：
			filePath 本地文件路径(urlencode编码)。D:\\Soft\\QQ2010.exe
			postedLength 已上传字节。控件将从此位置开始续传数据
			postedPercent 已上传百分比。示例：20%
			md5 文件MD5值。32个字符
			sfid 与服务器对应的fid，必须唯一
	*/
	this.AddResumeFile = function (File, Size, USize)//(filePath, postedLength, postedPercent, md5, sfid)
	{
	    if (Size < 0) {
	        Size = Size >>> 0;
	    }
	    if (USize < 0) {
	        USize = USize >>> 0;
	    }
	    var filePath = File;
	    var postedLength = USize;
	    var postedPercent = (parseFloat(USize) / parseFloat(Size)).toFixed(2) * 100 + "%";
	    var md5 = "";
	    //本地文件名称存在
	    if (this.Exist(filePath)) return;

	    var fileName = filePath.substr(filePath.lastIndexOf("\\") + 1);
	    var fid = this.UploaderListCount;

	    var upFile = new HttpUploader(fid, filePath, this, Size);
	    var newTable = this.UploaderTemplateDiv.cloneNode(true);
	    newTable.style.display = "block";
	    newTable.id = "item" + fid;

	    var divLeft = newTable.children(0);
	    var divRight = newTable.children(1);
	    var objFileName = divLeft.children(0).children(0);
	    objFileName.innerText = fileName;
	    objFileName.title = fileName;
	    var fileSize = divLeft.children(0).children(1);
	    fileSize.innerText = upFile.FileSize;
	    upFile.pProcess = divLeft.children(1).children(0);
	    upFile.pProcess.style.width = postedPercent;
	    upFile.pMsg = divLeft.children(2);
	    upFile.pMsg.innerText = "";
	    upFile.pButton = divRight.children(0).children(0);
	    upFile.pButton.fid = fid;
	    upFile.pButton.domid = "item" + fid;
	    upFile.pButton.lineid = "FilePostLine" + fid;
	    upFile.pButton.attachEvent("onclick", BtnControlClick);
	    upFile.pPercent = divRight.children(1);
	    upFile.pPercent.innerText = postedPercent;
	    upFile.Manager = this;
	    //upFile.ATL.PostedLength = parseInt(postedLength); //设置续传位置
	    //upFile.ATL.MD5 = md5;
	    upFile.MD5 = md5;
	    upFile.fid = fid;
	    //upFile.fid = sfid;

	    //添加到上传列表
	    this.AppenToUploaderList(fid, upFile);
	    //添加到上传列表层
	    upFile.spliter = this.AppendToUploaderListDiv(fid, newTable);
	    upFile.div = newTable;
	    //upFile.Post(); //开始上传
	    upFile.WaitContinue(); //准备

	};
}

/*
添加一个文件到上传队列
参数:
fileName 包含完整路径的文件名称。D:\\Soft\\QQ.exe
*/
HttpUploaderMgr.prototype.AddFile = function (filePath,_size) {
    //本地文件名称存在
    if (this.Exist(filePath)) return;
    //此类型为过滤类型
    if (this.NeedFilter(filePath)) return;


    var fileName = filePath.substr(filePath.lastIndexOf("\\") + 1);
    var fid = this.UploaderListCount;

    this.AVX.AddFileToUploadList(filePath);

    var upFile = new HttpUploader(fid, filePath, this,_size);
    var newTable = this.UploaderTemplateDiv.cloneNode(true);
    newTable.style.display = "block";
    newTable.id = "item" + fid;
    var divLeft = newTable.children(0);
    var divRight = newTable.children(1);
    var objFileName = divLeft.children(0).children(0);
    objFileName.innerText = fileName;
    objFileName.title = fileName;
    var fileSize = divLeft.children(0).children(1);
    fileSize.innerText = upFile.FileSize;
    upFile.pProcess = divLeft.children(1).children(0);
    upFile.pMsg = divLeft.children(2);
    upFile.pMsg.innerText = "";
    upFile.pButton = divRight.children(0).children(0);
    upFile.pButton.fid = fid;
    upFile.pButton.domid = "item" + fid;
    upFile.pButton.lineid = "FilePostLine" + fid;
    upFile.pButton.attachEvent("onclick", BtnControlClick);
    upFile.pPercent = divRight.children(1);
    upFile.pPercent.innerText = "0%";
    upFile.Manager = this;

    //添加到上传列表
    this.AppenToUploaderList(fid, upFile);
    //添加到上传列表层
    upFile.spliter = this.AppendToUploaderListDiv(fid, newTable);
    upFile.div = newTable;
    upFile.Ready(); //准备
}

function MgrBtnClick() {
    var obj = event.srcElement; //<a>
    switch (obj.innerText) {
        case "上传":
            HttpUploaderMgrInstance.PostFirst();
            break;
        case "暂停":
            HttpUploaderMgrInstance.StopAll();
            break;
    }
}

//单击控制按钮
function BtnControlClick()
{
	var obj = event.srcElement; //<a fid=""></a>
	var objup = HttpUploaderMgrInstance.UploaderList[obj.fid];

	switch (obj.innerText)
	{
		case "取消":
				HttpUploaderMgrInstance.Delete(objup.FileID);
                break;
	}
	return false;
}

var HttpUploaderErrorCode = {
	"0": "连接服务器错误"
	, "1": "发送数据错误"
	, "2": "接收数据错误"
	, "3": "未设置本地文件"
	, "4": "本地文件不存在"
	, "5": "打开本地文件错误"
	, "6": "不能读取本地文件"
	, "7": "公司未授权"
	, "8": "未设置IP"
	, "9": "域名未授权"
	, "10": "文件大小超过限制"//默认为2G
	//md5
	, "200": "无打打开文件"
	, "201": "文件大小为0"
};

var HttpUploaderState = {
	Ready: 0,
	Posting: 1,
	Stop: 2,
	Error: 3,
	GetNewID: 4,
	Complete: 5,
	WaitContinueUpload: 6,
	None: 7,
	Waiting: 8
};

var ControlState = {
    UPLOAD_FILE_STOP    :0,
	UPLOAD_FILE_RUN     :1,
	UPLOAD_FILE_PAUSE   :2
}

//文件上传对象
function HttpUploader(fileID, filePath, mgr,_size)
{
	this.Manager = mgr; //上传管理器指针
	this.FileLstMgr = mgr.FileLstMgr;//文件列表管理器
	this.Config = mgr.Config;
	this.Fields = mgr.Fields;
	this.ActiveX = mgr.ActiveX;
	this.State = HttpUploaderState.None;
	this.MD5 = "";
	this.FileName = filePath.substr(filePath.lastIndexOf("\\") + 1);
	this.LocalFile = filePath;
	this.FileID = fileID;
	this.FileSize = CalSize(_size); //this.ATL.FileSize;//格式化后的文件大小 50MB
	this.FileLength = _size; // this.ATL.FileLength;//以字节为单位的字符串
	this.PathLocal = encodeURIComponent(filePath); //URL编码后的本地路径
	this.PathRelat = ""; //文件在服务器中的相对地址。示例：http://www.ncmem.con/upload/201204/03/QQ2012.exe
	this.fid = 0; //与服务器数据库对应的fid
	this.uid = this.Fields["uid"];

	
	//准备
	this.Ready = function()
	{
		this.pMsg.innerText = "等待上传";
		this.State = HttpUploaderState.Ready;
    };
    this.WaitContinue = function () {
        this.pMsg.innerText = "等待续传";
        this.State = HttpUploaderState.WaitContinueUpload;
    }
	
}

//上传错误
function HttpUploader_Error(obj)
{
	obj.pMsg.innerText = HttpUploaderErrorCode[obj.ATL.ErrorCode];
	//文件大小超过限制,文件大小为0
	if (10 == obj.ATL.ErrorCode
		|| 201 == obj.ATL.ErrorCode)
	{
		obj.pButton.innerText = "取消";
	}
	else
	{
		obj.pButton.innerText = "续传";
	}
	obj.State = HttpUploaderState.Error;
	//从上传列表中删除
	obj.Manager.RemoveUploadId(obj.FileID);
	obj.PostNext();
}

//上传完成，向服务器传送信息
function HttpUploader_Complete(obj) {
    
	obj.pButton.style.display = "none";
	obj.pProcess.style.width = "100%";
	obj.pPercent.innerText = "100%";
	obj.pMsg.innerText = "上传完成";
	obj.Manager.CompleteList.push(obj);
	obj.State = HttpUploaderState.Complete;
	//从上传列表中删除
	obj.Manager.RemoveUploadId(obj.FileID);
}

//传输进度。频率为每秒调用一次
function HttpUploader_Process(File,Size,USize)//obj, speed, postedLength, percent, times)
{
    if (Size < 0) {
        Size = Size >>> 0;
    }
    if (USize < 0) {
        USize = USize >>> 0;
    }
    var percent = (parseFloat(USize) / parseFloat(Size)).toFixed(2) * 100 + "%";
    //var speed = 0;
    //var times = 0;
    var obj = null;
    for (a in HttpUploaderMgrInstance.UploaderList) {
        if (HttpUploaderMgrInstance.UploaderList[a].LocalFile == File) {
            obj = HttpUploaderMgrInstance.UploaderList[a];
        }
    }
    HttpUploaderMgrInstance.AppendUploadId(obj.FileID);
    if (!obj) {
        return;
    }
    obj.pPercent.innerText = percent;
	obj.pProcess.style.width = percent;
	var str = "已上传:" + CalSize(USize);// + " 速度:" + speed + "/S 剩余时间:" + times;
	obj.pMsg.innerText = str;
	if (obj.FileLength == USize) {
	    HttpUploader_Complete(obj);
    }
}

/*
	HUS_Leisure			=0	//空闲
	,HUS_Uploading		=1	//上传中 
	,HUS_Stop  			=2	//停止 
	,HUS_UploadComplete	=3	//传输完毕 
	,HUS_Error 			=4	//错误 
	,HUS_Connected 		=5	//服务器已连接
	,HUS_Md5Working		=6	//MD5计算中
	,HUS_Md5Complete	=7	//MD5计算完毕
*/
function HttpUploader_StateChanged(state) {
    var obj = document.getElementById("lnkMgrBtn");
	switch(state)
	{
	    case ControlState.UPLOAD_FILE_STOP:
	        obj.innerText = "上传";
	        break;
	    case ControlState.UPLOAD_FILE_RUN:
	        obj.innerText = "暂停";
	        break;
	    case ControlState.UPLOAD_FILE_PAUSE:
	        obj.innerText = "上传";
			break;
	}
}

function CalSize(size) {
    if (size < 0) {
        size = size >>> 0;
    }
    var str = size;
    if (size > 1048576) {
        str = (parseFloat(size) / 1048576).toFixed(2) + "M";
        return str;
    }
    if (size > 1024) {
        str = (parseFloat(size) / 1024).toFixed(2) + "K";
        return str;
    }
    return str + "B";
}