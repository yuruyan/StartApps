using System;
using System.Drawing;

namespace StartApp.Util;

public static class Utils {
    /// <summary>
    /// 获取可执行程序图标
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Bitmap GetExeBitmap(string path) {
        uint _nIcons = PInvokeHelpers.PrivateExtractIcons(path, 0, 0, 0, null, null, 0, 0);
        IntPtr[] phicon = new IntPtr[_nIcons];
        uint[] piconid = new uint[_nIcons];
        _ = PInvokeHelpers.PrivateExtractIcons(path, 0, 128, 128, phicon, piconid, _nIcons, 0);
        Icon icon = Icon.FromHandle(phicon[0]);
        PInvokeHelpers.DestroyIcon(icon.Handle);
        Bitmap bitmap = icon.ToBitmap();
        return bitmap;
    }
}
