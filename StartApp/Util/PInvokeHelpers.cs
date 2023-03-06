using System.Runtime.InteropServices;

namespace StartApp.Util;

internal static class PInvokeHelpers {
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern bool DestroyIcon(IntPtr handle);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
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