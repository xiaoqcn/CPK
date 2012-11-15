using System.Runtime.InteropServices;
using System.Text;

namespace CPKWrap
{
    public class libukey
    {
        public const string libukeyPath = "libukey.dll";//@".\DllLib\libukey.dll";//

        //[DllImport(libukeyPath)]
        #region 设备管理
        /// <summary>
        /// 打开设备
        /// 【描述】通过验证PIN，确定用户的合法性，并将CPK Key的状态设置为打开，并修改为相应的安全状态。
        /// 【备注】打开设备后关闭设备前，安全状态保持。
        /// </summary>
        /// <param name="pinType">[in] PIN的类型，1为超级用户，2为普通用户</param>
        /// <param name="pin">[in] PIN码，长度为8~16个字节</param>
        /// <param name="times">[out] PIN剩余可重试次数，如果输入PIN不正确，则PIN对应的错误计数器减1，当剩余可重试次数为0时，CPK Key对应的PIN被锁定。</param>
        /// <returns>函数成功返回0，否则返回错误号。</returns>
        [DllImport(libukeyPath)]
        public static extern int UKeyOpen(byte pinType, string pin, out int times);

        /// <summary>
        /// 获取设备序列号
        /// 【描述】如果输入的缓冲区大小不足以存放序列号，则返回缓冲区内在溢出错误，其中snLen指示所需的大小。
        /// </summary>
        /// <param name="sn">[out] 设备序列号缓冲区</param>
        /// <param name="snLen">[in,out] 输入缓冲区大小，返回序列号的实际长度</param>
        /// <returns>函数成功返回0，否则返回错误号。</returns>
        [DllImport(libukeyPath,CharSet=CharSet.Ansi,SetLastError=false,CallingConvention=CallingConvention.Cdecl)]
        public static extern int UKeyGetSn([Out, MarshalAs(UnmanagedType.LPArray)]byte[] sn, ref int snLen);


        /// <summary>
        /// 关闭设备
        /// 【描述】关闭CPK Key，系统将断开与CPK Key连接，并将CPK Key的状态设置为关闭，清除安全状态寄存器。
        /// 【备注】在访问结束时调用此函数。
        /// </summary>
        /// <returns>函数成功返回0，否则返回错误号。</returns>
        [DllImport(libukeyPath)]
        public static extern int UKeyClose();

        #endregion

        #region 访问控制
        
        #endregion

        #region 文件管理
        
        #endregion

        #region 密码服务
        
        /// <summary>
        /// 取随机数
        /// 【描述】从密钥设备中得到指定长度的随机数，随机数的长度为4~32个字节
        /// </summary>
        /// <param name="rand">[out] 返回的随机数缓冲区</param>
        /// <param name="randLen">[in,out] 输入缓冲区的大小或期望返回的随机数的长度，输出随机数的实际大小</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libukeyPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int UKeyGetRandom([Out, MarshalAs(UnmanagedType.LPArray)]byte[] rand, ref int randLen);

        /// <summary>
        /// 得到证书标识
        /// 【描述】得ID证书的标识，即用于计算用户密钥的用户标识
        /// </summary>
        /// <param name="keyId">[in] 密钥序号</param>
        /// <param name="certId">[out] 密钥标识</param>
        /// <param name="certIdLen">[in,out] 输入为缓冲区大小，输出为密钥标识长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libukeyPath)]
        public static extern int UKeyGetCertId(byte keyId, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] certId, ref int certIdLen);

        /// <summary>
        /// 得到作用域标识
        /// 【描述】获得CPK Key中ID证书所在域的标识
        /// 【备注】需先经过用户PIN认证
        /// </summary>
        /// <param name="keyId">[in] 密钥序号</param>
        /// <param name="matrixId">[out] 作用域标识</param>
        /// <param name="matrixIdLen">[in,out] 输入为缓冲区大小，输出为作用域标识长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libukeyPath)]
        public static extern int UKeyGetMatrixId(byte keyId, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] matrixId, ref int matrixIdLen);

        /// <summary>
        /// CPK数字签名
        /// 【描述】对数据进行签名
        /// 【备注】需要先经过用户PIN认证
        /// </summary>
        /// <param name="keyId">[in] 密钥序号，取值范围0x00~0x0F，具体值由密钥存放位置而定</param>
        /// <param name="data">[in] 需要签名的数据</param>
        /// <param name="dataLen">[in] 需要签名的数据的长度</param>
        /// <param name="signTime">[in] 签名时间(时间戳)，建议格式为“YYYY-MM-DD hh:mm:ss”</param>
        /// <param name="signVal">[out] 返回的签名信息</param>
        /// <param name="signValLen">[in,out] 输入的是缓冲区的长度，输出是返回的签名信息的长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libukeyPath)]
        public static extern int UKeySign(byte keyId, byte[] data, int dataLen, string signTime, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] signVal, ref int signValLen);
        
        /// <summary>
        /// CPK数字签名
        /// 【描述】对数据进行签名
        /// 【备注】需要先经过用户PIN认证
        /// </summary>
        /// <param name="keyId">[in] 密钥序号，取值范围0x00~0x0F，具体值由密钥存放位置而定</param>
        /// <param name="data">[in] 需要签名的数据</param>
        /// <param name="dataLen">[in] 需要签名的数据的长度</param>
        /// <param name="signTime">[in] 签名时间(时间戳)，建议格式为“YYYY-MM-DD hh:mm:ss”</param>
        /// <param name="signVal">[out] 返回的签名信息</param>
        /// <param name="signValLen">[in,out] 输入的是缓冲区的长度，输出是返回的签名信息的长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libukeyPath)]
        public static extern int UKeySign(byte keyId, byte[] data, int dataLen, byte[] signTime, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] signVal, ref int signValLen);

        /// <summary>
        /// CPK文件签名
        /// 【描述】对指定的文件进行签名
        /// 【备注】需要先经过用户PIN认证
        /// </summary>
        /// <param name="keyId">[in] 密钥序号，取值范围0x00~0x0F，具体值由密钥存放位置而定</param>
        /// <param name="filename">[in] 需要签名的文件</param>
        /// <param name="signTime">[in] 签名时间(时间戳)，建议格式为“YYYY-MM-DD hh:mm:ss”</param>
        /// <param name="signVal">[out] 返回的签名信息</param>
        /// <param name="signValLen">[in,out] 输入的是缓冲区的长度，输出是返回的签名信息的长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libukeyPath)]
        public static extern int UKeySignFile(byte keyId, string filename, string signTime, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] signVal, ref int signValLen);

        /// <summary>
        /// CPK数据解密
        /// 【备注】需要先经过用户PIN认证
        /// </summary>
        /// <param name="keyId">[in] 密钥序号</param>
        /// <param name="ciphertext">[in] 需要解密的数据</param>
        /// <param name="ciphertextLen">[in] 需要解密的数据的长度</param>
        /// <param name="cleartext">[out] 返回的解密后数据</param>
        /// <param name="cleartextLen">[in,out] 输入的是缓冲区的长度，输出是返回的解密后数据的长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libukeyPath)]
        public static extern int UKeyDecrypt(byte keyId, byte[] ciphertext, int ciphertextLen, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] cleartext, ref int cleartextLen);

        /// <summary>
        /// SM1加密函数
        /// 【备注】对于不支持SM1算法的芯片，采用3DES算法。由于受通信速度影响，本函数仅适用于对被加密数据内容少，保密性要求高的情形。
        /// </summary>
        /// <param name="P1">
        /// [in] 密钥的类型 
        /// 00 使用密钥文件中的密钥
        /// 01 使用数字信封中的密钥，这种方式需要在加密前先调用产生数字信封函数并将数字信封中的会话密钥放入密钥缓冲区
        /// </param>
        /// <param name="inBuf">[in] 输入的被加密数据</param>
        /// <param name="inLen">[in] 被加密数据的长度，单次长度不超过192</param>
        /// <param name="outBuf">[out] 输出加密后的数据密文</param>
        /// <param name="outLen">[in,out] 输入密文缓冲区长度，输出密文数据的实际长度</param>
        /// <returns>成功返回0，否则返回错误号</returns>
        [DllImport(libukeyPath)]
        public static extern int UKeySM1Encrypt(byte P1, byte[] inBuf, int inLen, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] outBuf, ref int outLen);

        /// <summary>
        /// SM1解密函数
        /// 【备注】对于不支持SM1算法的芯片，采用3DES算法。由于受通信速度影响，本函数仅适用于对被解密数据内容少，保密性要求高的情形。
        /// </summary>
        /// <param name="P1">[in] 密钥的类型 
        ///         00 使用密钥文件中的密钥 
        ///         01 使用数字信封中的密钥，这种方式需要在解密前先调用打开数字信封函数并将数字信封中的会话密钥放入密钥缓冲区</param>
        /// <param name="inBuf">[in] 输入要解密的密文数据</param>
        /// <param name="inLen">[in] 要解密数据的长度，单次长度不超过192</param>
        /// <param name="outBuf">[out] 输出解密后的数据明文</param>
        /// <param name="outLen">[in,out] 输入明文缓冲区长度，输出明文数据的实际长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libukeyPath)]
        public static extern int UKeySM1Decrypt(byte P1, byte[] inBuf, int inLen, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] outBuf, ref int outLen);

        /// <summary>
        /// 计算SHA1数字摘要
        /// </summary>
        /// <param name="inBuf">[in] 输入用于计算SHA1数字摘要的数据</param>
        /// <param name="inLen">[in] 用于计算数字摘要的数据长度</param>
        /// <param name="outBuf">[out] 输出数字摘要缓冲区</param>
        /// <param name="outLen">[in,out] 输入摘要数据缓冲区长度，输出摘要数据的实际长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libukeyPath)]
        public static extern int UKeySHA1(byte[] inBuf, int inLen, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] outBuf, ref int outLen);

        #endregion
    }
}
