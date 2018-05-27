using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DothanTech.Helpers
{
    public class FileIcon
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        /// <summary>
        /// 返回系统设置的图标
        /// </summary>
        /// <param name="pszPath">文件路径 如果为""  返回文件夹的</param>
        /// <param name="dwFileAttributes">0</param>
        /// <param name="psfi">结构体</param>
        /// <param name="cbSizeFileInfo">结构体大小</param>
        /// <param name="uFlags">枚举类型</param>
        /// <returns>-1失败</returns>
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref   SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        public enum SHGFI
        {
            SHGFI_ICON = 0x100,
            SHGFI_USEFILEATTRIBUTES = 0x10,
            SHGFI_DISPLAYNAME = 0x200,  // Gets the Display name
            SHGFI_TYPENAME = 0x400,     // Gets the type name
            SHGFI_LARGEICON = 0x0,     // Large icon
            SHGFI_SMALLICON = 0x1      // Small icon
        }

        /// <summary>
        /// 获取文件图标
        /// </summary>
        /// <param name="p_Path">文件全路径</param>
        /// <returns>图标</returns>
        public static Icon GetFileIcon(string p_Path, bool smallIcon = false)
        {
            SHFILEINFO _SHFILEINFO = new SHFILEINFO();
            uint flag = (uint)(SHGFI.SHGFI_ICON | SHGFI.SHGFI_USEFILEATTRIBUTES | (smallIcon ? SHGFI.SHGFI_SMALLICON : SHGFI.SHGFI_LARGEICON));
            IntPtr _IconIntPtr = SHGetFileInfo(p_Path, 0, ref _SHFILEINFO, (uint)Marshal.SizeOf(_SHFILEINFO), flag);
            if (_IconIntPtr.Equals(IntPtr.Zero)) return null;
            Icon _Icon = System.Drawing.Icon.FromHandle(_SHFILEINFO.hIcon);
            return _Icon;
        }
        /// <summary>
        /// 获取文件夹图标 
        /// </summary>
        /// <returns>图标</returns>
        public static Icon GetDirectoryIcon(bool smallIcon = false)
        {
            SHFILEINFO _SHFILEINFO = new SHFILEINFO();
            uint flag = (uint)(SHGFI.SHGFI_ICON | (smallIcon ? SHGFI.SHGFI_SMALLICON : SHGFI.SHGFI_LARGEICON));
            IntPtr _IconIntPtr = SHGetFileInfo(@"", 0, ref _SHFILEINFO, (uint)Marshal.SizeOf(_SHFILEINFO), flag);
            if (_IconIntPtr.Equals(IntPtr.Zero)) return null;
            Icon _Icon = System.Drawing.Icon.FromHandle(_SHFILEINFO.hIcon);
            return _Icon;
        }

        /// <summary>
        /// 获取给定文件或文件夹所关联图标所对应的 ImageSource；
        /// </summary>
        /// <param name="file">目标文件或文件夹</param>
        /// <param name="smallIcon">需要关联的图标是小图标或者大图标</param>
        /// <returns>文件所关联Icon所对应的数据源</returns>
        public static ImageSource GetImageSource(String file, bool smallIcon)
        {
            try
            {
                Icon icon = Directory.Exists(file) ? FileIcon.GetDirectoryIcon(smallIcon) : FileIcon.GetFileIcon(file, smallIcon);
                return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, new Int32Rect(0, 0, icon.Width, icon.Height), BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception ee)
            {
                Trace.WriteLine("### [" + ee.Source + "] Exception: " + ee.Message);
                Trace.WriteLine("### " + ee.StackTrace);
                return null;
            }
        }
    }
}
