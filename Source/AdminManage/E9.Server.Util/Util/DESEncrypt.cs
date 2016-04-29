using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NM.Util
{
    public class DESEncrypt
    {
        public static string _Key = "abcdefghgkpengkivenw";   // 加密解密种子

        public static byte[] ConvertToByteArray(string sJson)
        {
            return Encoding.UTF8.GetBytes((sJson));
        }

        public static string ConvertToString(byte[] b)
        {
            return (Encoding.UTF8.GetString(b, 0, b.Length));
        }

        /// <summary>
        /// 加密
        /// </summary>
        public static string Encrypt(string sText)
        {
            return Encrypt(_Key, sText);
        }

        /// <summary>
        /// 加密
        /// </summary>
        public static string Encrypt(string sKey, string sText)
        {
            if (string.IsNullOrEmpty(sKey) || string.IsNullOrEmpty(sText))
                throw new Exception("Encrypt: 字符串不能为空!");

            int iKeyLen = 0, iKeyPos = 0, iOffset = 0, iSrcPos = 0, iSrcAsc = 0, iRange = 0;
            string sDest = "";    //加密后的字符串
            char cTmp;

            iKeyLen = sKey.Trim().Length;

            if (iKeyLen == 0)
                sKey = _Key;    //如果参数为空值把key默认的种子

            iKeyPos = 0;
            iRange = 255;

            Random rdObj = new Random();

            iOffset = rdObj.Next(0, iRange);    //将0-256范围内取随机数

            sDest = iOffset.ToString("X2");   //将随机数转换成16进制数

            for (iSrcPos = 0; iSrcPos < sText.Trim().Length; iSrcPos++)
            {

                cTmp = sText[iSrcPos];
                iSrcAsc = ((short)cTmp + iOffset) % 255;

                if (iKeyPos < iKeyLen)
                    iKeyPos = iKeyPos + 1;
                else
                    iKeyPos = 1;

                cTmp = sKey[iSrcPos];
                iSrcAsc = iSrcAsc ^ (short)cTmp;

                sDest = sDest + iSrcAsc.ToString("X2");

                iOffset = iSrcAsc;
            }
            return sDest;
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static string Decrypt(string sText)
        {
            return Decrypt(_Key, sText);
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static string Decrypt(string sKey, string sText)
        {
            if (string.IsNullOrEmpty(sKey) || string.IsNullOrEmpty(sText))
                throw new Exception("Decrypt: 字符串不能为空!");

            int iKeyLen, iKeyPos, iOffset, iSrcPos, iSrcAsc, iTmpSrcAsc;
            string strDest = "";

            iKeyLen = sKey.Trim().Length;

            if (iKeyLen == 0)
            {
                sKey = _Key;
                iKeyLen = sKey.Length;
            }

            iKeyPos = 0;

            iOffset = Int32.Parse(sText.Trim().Substring(0, 2), System.Globalization.NumberStyles.HexNumber);

            iSrcPos = 2;

            do
            {
                iSrcAsc = Int32.Parse(sText.Trim().Substring(iSrcPos, 2), System.Globalization.NumberStyles.HexNumber);

                if (iKeyPos < iKeyLen)
                    iKeyPos = iKeyPos + 1;
                else
                    iKeyPos = 1;

                iTmpSrcAsc = iSrcAsc ^ (short)sKey[iKeyPos - 1];

                if (iTmpSrcAsc <= iOffset)
                    iTmpSrcAsc = 255 + iTmpSrcAsc - iOffset;
                else
                    iTmpSrcAsc = iTmpSrcAsc - iOffset;

                strDest = strDest + (char)iTmpSrcAsc;

                iOffset = iSrcAsc;
                iSrcPos = iSrcPos + 2;
            }
            while (iSrcPos < sText.Length);

            return strDest;
        }

        public static string NewGuid()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0).ToString();
        }
    }
}
