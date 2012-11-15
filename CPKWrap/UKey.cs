using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPKWrap
{
    public class UKey
    {
        #region 辅助函数
        public static string Byte2String(byte[] ba)
        {
            try
            {
                return System.Text.Encoding.UTF8.GetString(ba);
            }
            catch
            { }
            return null;
        }

        public static byte[] String2Byte(string s)
        {
            try
            {
                return System.Text.Encoding.UTF8.GetBytes(s);
            }
            catch { }
            return null;
        }

        #endregion

        public static int Open(byte pinType, string pin, out int times)
        {
            //byte[] pin_b = String2Byte(pin);

            return libukey.UKeyOpen((byte)pinType, pin, out times);
        }

        public static string GetSn()
        {
            //StringBuilder sn = new StringBuilder();
            
            //int snLen = 16;
            //byte[] sn = new byte[snLen];
            //int i = libukey.UKeyGetSn(sn, ref snLen);
            //if (i == 0 && snLen > 0)
            //{
            //    return Byte2String(sn);
            //}
            //else
            //{
            //    return string.Empty;
            //}
            return string.Empty;
        }

        public static string GetCertId(byte keyId)
        {
            //StringBuilder certId = new StringBuilder();
            int certIdLen = 16;
            byte[] certId = new byte[certIdLen];
            int i = libukey.UKeyGetCertId(keyId, certId, ref certIdLen);
            if (i == 0 && certIdLen > 0)
            {
                return Byte2String(certId);
            }
            else
            {
                return string.Empty;
            }
        }

        public static int Close()
        {
            return libukey.UKeyClose();
        }
    }
}
