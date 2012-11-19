function Submit() {
    document.getElementById("signInfo").value = "";
    var pin = document.getElementById("pin").value;
    document.getElementById("pin").value = "";
    var signData = document.getElementById("signData").value;
    var signInfo = CPK_SIGN(pin, signData);
    if (!signInfo) {
        signInfo = '';
    }
    document.getElementById("signInfo").value = signInfo;
}

window.onload = function () {
    var cpkCtrl = GetCPKCtrl();
    var _sid = cpkCtrl.CPKGetDeviceSn(1); //1-CPKey,2-TFCard
    if (!_sid) {
        return;
    }
    document.getElementById("sid").value = _sid;
}

function CPK_SIGN(pin, signData) {
    //返回值
    var rtn = null;
    //ActiveX
    var cpkCtrl = GetCPKCtrl();
    if (!cpkCtrl) {
        return;
    }

    if (arguments.length < 2) {
        throw new Error('参数错误！');
        return;
    }

    try {
        rtn = cpkCtrl.CPKOpenDevice(1, pin); //1-CPKey,2-TFCard
    }
    catch (e) {
        alert("需要安装客户端才能登录本系统：打开失败");
        return;
    }
    if (rtn !== 0) {
        alert("打开设备失败:" + rtn);
        return;
    }

    try {
        rtn = cpkCtrl.CPKSignData(signData);
    } catch (e) {
        alert('需要安装客户端才能登录本系统：签名失败');
        return;
    }
    if (!rtn) {
        alert("签名失败:" + rtn);
        return;
    }
    //控件：返回签名结果

    return rtn;
}

function GetCPKCtrl() {
    return document.getElementById("FaDnCtrl");
}

