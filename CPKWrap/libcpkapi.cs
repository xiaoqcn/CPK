using System.Runtime.InteropServices;
using System.Text;

namespace CPKWrap
{
    public class libcpkapi
    {
        public const string libcpkapiPath = "libcpkapi.dll"; //@".\Lib\libcpkapi.dll";//"libcpkapi.dll";//
        
        #region 数字签名

        /// <summary>
        /// 验证签名（数据）
        /// </summary>
        /// <param name="pubmatrix">[in] 公钥矩阵文件名（含路径，支持相对路径）</param>
        /// <param name="signedData">[in] 需要验证的被签名数据</param>
        /// <param name="signedDataLen">[in] 需要验证的数据长度</param>
        /// <param name="signInfo">[in] 签名信息</param>
        /// <param name="signInfoLen">[in] 签名信息的长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int CPK_Verify_Data(string pubmatrix, byte[] signedData, int signedDataLen, byte[] signInfo, int signInfoLen);

        /// <summary>
        /// 验证签名（数据）
        /// </summary>
        /// <param name="pubmatrix">[in] 公钥矩阵文件名（含路径，支持相对路径）</param>
        /// <param name="signedData">[in] 需要验证的被签名数据</param>
        /// <param name="signedDataLen">[in] 需要验证的数据长度</param>
        /// <param name="signInfo">[in] 签名信息</param>
        /// <param name="signInfoLen">[in] 签名信息的长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int CPK_Verify_Data(string pubmatrix, string signedData, int signedDataLen, byte[] signInfo, int signInfoLen);

        /// <summary>
        /// 验证签名（摘要）
        /// </summary>
        /// <param name="pubmatrix">[in] 公钥矩阵文件名(含路径，支持相对路径)</param>
        /// <param name="digest">[in] 摘要数据</param>
        /// <param name="digestLen">[in] 摘要数据的长度</param>
        /// <param name="signInfo">[in] 签名信息</param>
        /// <param name="signInfoLen">[in] 签名信息的长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int CPK_Verify_Digest(string pubmatrix, byte[] digest, int digestLen, byte[] signInfo, int signInfoLen);
	    
        /// <summary>
        /// 验证文件签名
        /// </summary>
        /// <param name="pubmatrix">[in] 公钥矩阵文件名(含路径，支持相对路径)</param>
        /// <param name="filename">[in] 被签名的文件名(含路径)</param>
        /// <param name="signInfo">[in] 签名信息</param>
        /// <param name="signInfoLen">[in] 签名信息的长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int CPK_Verify_File(string pubmatrix, string filename, byte[] signInfo, int signInfoLen);

        /// <summary>
        /// 得到签名标识
        /// </summary>
        /// <param name="signInfo">[in] 签名信息</param>
        /// <param name="signInfoLen">[in] 签名信息长度</param>
        /// <param name="signerId">[out] 签名者标识缓冲区</param>
        /// <param name="signerIdLen">[in,out] 输入的是缓冲区的长度，输出是返回的签名者标识的实际长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int CPK_GetSignerId(byte[] signInfo, int signInfoLen, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] signerId, ref int signerIdLen);
        
        /// <summary>
        /// 得到签名时间
        /// </summary>
        /// <param name="signInfo">[in] 签名信息</param>
        /// <param name="signInfoLen">[in] 签名信息长度</param>
        /// <param name="signerId">[out] 签名时间缓冲区</param>
        /// <param name="signTimeLen">[in,out] 输入的是缓冲区的长度，输出是返回的签名时间的实际长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int CPK_GetSignTime(byte[] signInfo, int signInfoLen,[Out, MarshalAs(UnmanagedType.LPArray)]byte[] signerId, ref int signTimeLen);

        /// <summary>
        /// 得到签名矩阵标识
        /// </summary>
        /// <param name="signInfo">[in] 签名信息</param>
        /// <param name="signInfoLen">[in] 签名信息长度</param>
        /// <param name="matrixId">[out] 签名作用域标识缓冲区</param>
        /// <param name="matrixIdLen">[in,out] 输入的是缓冲区的长度，输出是返回的签名者作用域标识的实际长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int CPK_GetSignMatrixId(byte[] signInfo, int signInfoLen, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] matrixId, ref int matrixIdLen);
        
        #endregion

        #region 密钥交换

        /// <summary>
        /// CPK加密
        /// 【描述】产生随机数作为会话密钥对指定的数据进行AES对称加密，再利用CPK密钥交换协议对会话密钥采用CPK非对称加密产生数字信封，并将数字信封与加密后的数据打包。
        /// </summary>
        /// <param name="pubmatrix">[in] 公钥矩阵文件名</param>
        /// <param name="addresseeId">[in] 收件人标识</param>
        /// <param name="data">[in] 需要被加密的数据</param>
        /// <param name="dataLen">[in] 需要被加密的数据的长度</param>
        /// <param name="szEnvelope">[out] 加密后的数据</param>
        /// <param name="envelopeLen">[in,out] 输入的是缓冲区的长度，输出是返回的加密后的数据的长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int CPK_Encrypt_Data(string pubmatrix, byte[] addresseeId, byte[] data, int dataLen, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] szEnvelope, ref int envelopeLen);

        /// <summary>
        /// 得到加密标识
        /// 【描述】从加密数据包中解析得到加密时所使用的标识
        /// 【备注】加密标识也就是解密所对应的标识，即收件人标识，只有此标识的人才能解密该加密数据包。
        /// </summary>
        /// <param name="szEnvelope">[in] 加密数据包数据</param>
        /// <param name="envelopeLen">[in] 加密数据包数据的长度</param>
        /// <param name="addresseeId">[out] 加密标识缓冲区</param>
        /// <param name="addresseeIdLen">[in,out] 输入的是缓冲区的长度，输出是返回的加密标识的实际长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int CPK_GetEncId(byte[] szEnvelope, int envelopeLen, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] addresseeId, ref int addresseeIdLen);

        /// <summary>
        /// 得到矩阵标识
        /// 【描述】从加密数据包中得到加密所用的矩阵标识
        /// 【备注】加密矩阵标识是解密所对应的矩阵标识，即收件人所在的矩阵标识。
        /// </summary>
        /// <param name="szEnvelope">[in] 加密数据包数据</param>
        /// <param name="envelopeLen">[in] 加密数据包数据的长度</param>
        /// <param name="matrixId">[out] 矩阵标识缓冲区</param>
        /// <param name="matrixIdLen">[in,out] 输入的是缓冲区的长度，输出是返回的矩阵标识的实际长度</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int CPK_GetEncMatrixId(byte[] szEnvelope, int envelopeLen, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] matrixId, ref int matrixIdLen);

        #endregion

        #region 对称密码

        /// <summary>
        /// 对称加密
        /// 【描述】对称加密，支持主流对称加密算法，如AES和DES算法系列
        /// 【备注】
        ///     本函数支持的对称加密算法有：
        ///     (1) AES算法 AES-128-ECB AES-128-CBC AES-128-OFB AES-128-CFB AES-192-ECB AES-192-CBC AES-192-OFB AES-192-CFB AES-256-ECB AES-256-CBC AES-256-OFB AES-256-CFB AES-128-CFB1 AES-192-CFB1 AES-256-CFB1 AES-128-CFB8 AES-192-CFB8 AES-256-CFB8
        ///     (2) DES算法 DES-ECB DES-CBC DES-OFB DES-CFB DES-EDE DES-EDE-CBC DES-EDE-CFB DES-EDE-OFB DES-CFB1 DES-CFB8
        ///     (3) 3DES算法 DES-EDE3 DES-EDE3-CFB DES-EDE3-OFB DES-EDE3-CFB1 DES-EDE3-CFB8 DES-EDE3-CBC
        /// </summary>
        /// <param name="szalg">[in] 加密算法名称，详见备注</param>
        /// <param name="data">[in] 加密源数据</param>
        /// <param name="dataLen">[in] 加密源数据的长度</param>
        /// <param name="symkey">[in] 对称加密密钥</param>
        /// <param name="keyLen">[in] 对称密钥的长度</param>
        /// <param name="cipher">[out] 输出加密后密文的数据缓冲区</param>
        /// <param name="cipherLen">[in,out] 输入缓冲区的大小，输出密文数据的实际大小</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int Sym_Encrypt(string szalg, byte[] data, int dataLen, byte[] symkey, int keyLen, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] cipher, ref int cipherLen);

        /// <summary>
        /// 对称解密
        /// 【描述】对称解密，支持主流对称加密算法，如AES和DES算法系列
        /// 【备注】
        ///     本函数支持的对称加密算法有：
        ///     (1) AES算法 AES-128-ECB AES-128-CBC AES-128-OFB AES-128-CFB AES-192-ECB AES-192-CBC AES-192-OFB AES-192-CFB AES-256-ECB AES-256-CBC AES-256-OFB AES-256-CFB AES-128-CFB1 AES-192-CFB1 AES-256-CFB1 AES-128-CFB8 AES-192-CFB8 AES-256-CFB8
        ///     (2) DES算法 DES-ECB DES-CBC DES-OFB DES-CFB DES-EDE DES-EDE-CBC DES-EDE-CFB DES-EDE-OFB DES-CFB1 DES-CFB8
        ///     (3) 3DES算法 DES-EDE3 DES-EDE3-CFB DES-EDE3-OFB DES-EDE3-CFB1 DES-EDE3-CFB8 DES-EDE3-CBC
        /// </summary>
        /// <param name="szalg">[in] 加密算法名称，详见备注</param>
        /// <param name="cipher">[in] 需要解密的数据密文</param>
        /// <param name="cipherLen">[in] 数据密文的长度</param>
        /// <param name="symkey">[in] 对称加密密钥</param>
        /// <param name="keyLen">[in] 对称密钥的长度</param>
        /// <param name="data">[out] 输出解密后的数据明文缓冲区</param>
        /// <param name="dataLen">[in,out] 输入缓冲区的大小，输出明文数据的实际大小</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int Sym_Decrypt(string szalg, byte[] cipher, int cipherLen, byte[] symkey, int keyLen, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] data, ref int dataLen);

        #endregion

        #region 数字摘要

        /// <summary>
        /// 数字摘要
        /// 【描述】计算数字摘要，可支持MD和SHA两大算法体系
        /// 【备注】
        ///     本函数支持的数字摘要算法有：
        ///     (1) MD算法 MD2 MD4 MD5 
        ///     (2) SHA算法 SHA1 SHA224 SHA256 SHA384 SHA512
        /// </summary>
        /// <param name="szalg">[in] 摘要算法名称，详见备注</param>
        /// <param name="data">[in] 用于计算数字摘要的源数据</param>
        /// <param name="dataLen">[in] 源数据长度</param>
        /// <param name="md">[out] 输出数字摘要的缓冲区</param>
        /// <param name="mdLen">[in,out] 输入缓冲区的大小，输出数字摘要数据的实际大小</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int Hash(string szalg, byte[] data, int dataLen, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] md, ref int mdLen);

        #endregion

        #region Base64函数

        /// <summary>
        /// Base64编码
        /// 【备注】二进制数据有时在显示或传输时困难，一般将其转化为可显示字符，通常的方法是将其源数据作编码，Base64是最常用的编码。 Base64编码后数据长度增加，其长度满足dstLen = (srcLen/3 + 1)*4，如果数据较长，需要考虑换行符所占用的长度。
        /// </summary>
        /// <param name="pSrc">[in] 源数据</param>
        /// <param name="srcLen">[in] 源数据的长度</param>
        /// <param name="pDst">[out] 编码后的数据</param>
        /// <param name="dstLen">[in,out] 输入缓冲区的大小，输出编码后数据的实际大小</param>
        /// <param name="maxLineLen">[in] 最大行字符数，编码后的数据超过此长度将自动换行</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int Base64Encode(byte[] pSrc, int srcLen, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] pDst, ref int dstLen, int maxLineLen);

        /// <summary>
        /// Base64编码
        /// 【备注】二进制数据有时在显示或传输时困难，一般将其转化为可显示字符，通常的方法是将其源数据作编码，Base64是最常用的编码。 Base64编码后数据长度增加，其长度满足dstLen = (srcLen/3 + 1)*4，如果数据较长，需要考虑换行符所占用的长度。
        /// </summary>
        /// <param name="pSrc">[in] 源数据</param>
        /// <param name="srcLen">[in] 源数据的长度</param>
        /// <param name="pDst">[out] 编码后的数据</param>
        /// <param name="dstLen">[in,out] 输入缓冲区的大小，输出编码后数据的实际大小</param>
        /// <param name="maxLineLen">[in] 最大行字符数，编码后的数据超过此长度将自动换行</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int Base64Encode(byte[] pSrc, int srcLen, StringBuilder pDst, ref int dstLen, int maxLineLen);

        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="pSrc">[in] 源数据</param>
        /// <param name="srcLen">[in] 源数据的长度</param>
        /// <param name="pDst">[out] 解码后的数据</param>
        /// <param name="dstLen">[in,out] 输入缓冲区的大小，输出解码后数据的实际大小</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int Base64Decode(byte[] pSrc, int srcLen, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] pDst, ref int dstLen);

        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="pSrc">[in] 源数据</param>
        /// <param name="srcLen">[in] 源数据的长度</param>
        /// <param name="pDst">[out] 解码后的数据</param>
        /// <param name="dstLen">[in,out] 输入缓冲区的大小，输出解码后数据的实际大小</param>
        /// <returns>成功返回0，否则返回错误号。</returns>
        [DllImport(libcpkapiPath)]
        public static extern int Base64Decode(string pSrc, int srcLen, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] pDst, ref int dstLen);

        #endregion
    }
}
