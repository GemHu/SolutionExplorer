/// <summary>
/// @file   ViBlur.cs
///	@brief  提供字符串混淆用的函数。
/// @author	DothanTech 刘伟宏
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
    /// 字符串混淆用的类。能将 A-Z, a-z, 0-9, _ . 这些字符根据指定的种子密码进行混淆。
    /// </summary>
    public class ViBlur
    {
        /// <summary>
        /// 得到指定种子的混淆用助手用对象。如果指定种子为空字符串，则返回 baseBlur；如果 baseBlur 还为空，则返回 NoBlur。
        /// </summary>
        public static ViBlur GetBlur(ViBlur baseBlur, string key)
        {
            if (baseBlur == null)
                return null;

            if (string.IsNullOrEmpty(key))
                return baseBlur ?? NoBlur;

            return new ViBlur(key);
        }

        /// <summary>
        /// 没有混淆功能（字符串原样输出）的助手对象。
        /// </summary>
        public static readonly ViBlur NoBlur = new ViBlur();

        /// <summary>
        /// 没有混淆功能（字符串原样输出）同时禁止后续的所有混淆功能的助手对象。
        /// </summary>
        public static readonly ViBlur Disable = null;

        /// <summary>
        /// 对字符串进行混淆。
        /// </summary>
        public static string Encode(ViBlur blur, string str)
        {
            if (blur == Disable)
                return str;
            if (blur == NoBlur)
                return str;

            return blur.Encode(str);
        }

        /// <summary>
        /// 对字符串进行去混淆（解码）。
        /// </summary>
        public static string Decode(ViBlur blur, string str)
        {
            if (blur == Disable)
                return str;
            if (blur == NoBlur)
                return str;

            return blur.Decode(str);
        }

        /// <summary>
        /// 构建根据指定种子进行混淆的字符串混淆对象。
        /// </summary>
        protected ViBlur(int key)
        {
            string remainChars = sBlurChars;

            // 整复杂一点的混淆种子
            if (key < 0) key = -key;
            if (key < sBlurChars.Length)
                key += 0x12344321;

            // 配对所有字符对
            while (remainChars.Length >= 2)
            {
                int pos = (key % (remainChars.Length - 1)) + 1;
                mBlurMap[remainChars[0]] = remainChars[pos];
                mBlurMap[remainChars[pos]] = remainChars[0];

                remainChars = remainChars.Remove(pos, 1);
                remainChars = remainChars.Remove(0, 1);
            }
        }

        /// <summary>
        /// 构建根据指定种子进行混淆的字符串混淆对象。
        /// </summary>
        protected ViBlur(string key)
            : this(key.GetHashCode())
        {
        }

        /// <summary>
        /// 对字符串进行混淆。
        /// </summary>
        protected string Encode(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (mBlurMap.Count <= 0)
                return str;

            StringBuilder sb = new StringBuilder(str.Length);
            for (int i = 0; i < str.Length; ++i)
            {
                char ch = str[i];
                if (mBlurMap.ContainsKey(ch))
                    sb.Append(mBlurMap[ch]);
                else
                    sb.Append(ch);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 对字符串进行去混淆（解码）。
        /// </summary>
        protected string Decode(string str)
        {
            return Encode(str);
        }

        /// <summary>
        /// 主要是对象自己用来创建没有混淆功能的静态对象。
        /// </summary>
        protected ViBlur()
        {
        }

        protected readonly Dictionary<char, char> mBlurMap = new Dictionary<char, char>(sBlurChars.Length);

        protected const string sBlurChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_.";
    }

    public static class XmlWriterBlurExtension
    {
        /// <summary>
        /// 将混淆后的字符串值，写入 Xml Value 的辅助函数。
        /// </summary>
        public static void WriteValue(this System.Xml.XmlWriter This, ViBlur blur, string value)
        {
            This.WriteValue(value.BlurEncode(blur));
        }

        /// <summary>
        /// 将混淆后的字符串值，写入 Xml Attribute 的辅助函数。
        /// </summary>
        public static void WriteAttributeString(this System.Xml.XmlWriter This, ViBlur blur, string localName, string value)
        {
            This.WriteAttributeString(localName, value.BlurEncode(blur));
        }
    }

    public static class XmlReaderBlurExtension
    {
        /// <summary>
        /// 从混淆的 Xml Value 值中，解码出混淆之前的字符串的辅助函数。
        /// </summary>
        public static bool ReadValue(this System.Xml.XmlReader This, ViBlur blur, out string value)
        {
            if (This.Read())
            {
                value = This.Value.BlurDecode(blur);
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// 从混淆的 Xml Attribute 值中，解码出混淆之前的字符串的辅助函数。
        /// </summary>
        public static string GetAttribute(this System.Xml.XmlReader This, ViBlur blur, string name)
        {
            return This.GetAttribute(name).BlurDecode(blur);
        }
    }

    public static class StringBlurExtension
    {
        public static string BlurEncode(this string This, ViBlur blur)
        {
            return ViBlur.Decode(blur, This);
        }

        public static string BlurDecode(this string This, ViBlur blur)
        {
            return ViBlur.Encode(blur, This);
        }
    }
}
