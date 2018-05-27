/// <summary>
/// @file   EncryptHelper.cs
///	@brief  数据的加密与解密、HASH相关的帮助类。
/// @author	DothanTech 胡殿兴
/// 
/// Copyright(C) 2011~2015, DothanTech. All rights reserved.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DothanTech.Helpers
{
    /// <summary>
    /// 数据的加密与解密、HASH相关的帮助类。
    /// </summary>
    public class EncryptHelper
    {
        /// <summary>
        /// 生成字符串的 Base64 编码。
        /// </summary>
        public static string Base64Encode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// 生成二进制数组的 Base64 编码。
        /// </summary>
        public static string Base64Encode(byte[] value)
        {
            if (value == null || value.Length <= 0)
                return null;

            return Convert.ToBase64String(value);
        }

        /// <summary>
        /// 从 Base64 编码中解码出字符串，出错的情况下返回 null。
        /// </summary>
        public static string Base64Decode(string code)
        {
            if (string.IsNullOrEmpty(code))
                return code;

            try
            {
                return Encoding.UTF8.GetString(System.Convert.FromBase64String(code));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 从 Base64 编码中解码出字符串，出错的情况下返回 null。
        /// </summary>
        public static byte[] Base64DecodeB(string code)
        {
            if (string.IsNullOrEmpty(code))
                return null;

            try
            {
                return System.Convert.FromBase64String(code);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 返回字符串 MD5 Hash 之后的结果字符串（32个小写16进制字符串）。
        /// </summary>
        public static string MD5Encode(string value)
        {
            if (value == null)
                return null;

            return MD5Encode(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// 返回二进制数组 MD5 Hash 之后的结果字符串（32个小写16进制字符串）。
        /// </summary>
        public static string MD5Encode(byte[] value)
        {
            if (value == null)
                return null;

            return MD5.GetMD5(value);
        }
    }
}
