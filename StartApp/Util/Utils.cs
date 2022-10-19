using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace StartApp.Util;

public static class Utils {
    /// <summary>
    /// 获取可执行程序图标
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Bitmap GetExeBitmap(string path) {
        uint _nIcons = PInvoke.PrivateExtractIcons(path, 0, 0, 0, null, null, 0, 0);
        IntPtr[] phicon = new IntPtr[_nIcons];
        uint[] piconid = new uint[_nIcons];
        _ = PInvoke.PrivateExtractIcons(path, 0, 128, 128, phicon, piconid, _nIcons, 0);
        Icon icon = Icon.FromHandle(phicon[0]);
        Bitmap bitmap = icon.ToBitmap();
        return bitmap;
    }
}

public static class PInvoke {
    [DllImport("User32.dll", CharSet = CharSet.Unicode)]
    internal static extern uint PrivateExtractIcons(
        string szFileName,
        // szFileName 要从中提取图标的文件的路径和名称。
        int nIconIndex,
        // nIconIndex 要提取的第一个图标的从零开始的索引。例如，如果此值为零，则该函数会提取指定文件中的第一个图标。
        int cxIcon,
        // cxIcon 想要的水平图标大小。
        int cyIcon,
        // cyIcon 想要的垂直图标大小。
        IntPtr[] phicon,
        // phicon 指向返回的图标句柄数组的指针。
        uint[] piconid,
        // piconid 指向最适合当前显示设备的图标的返回资源标识符的指针。
        uint nIcons,
        // nIcons 要从文件中提取的图标数。此参数仅在从 .exe 和 .dll 文件中提取时有效。
        uint flags
    // flags 指定控制此功能的标志。
    );
}
