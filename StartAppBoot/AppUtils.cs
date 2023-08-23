using Vanara.PInvoke;

namespace StartAppBoot;

/// <summary>
/// 程序架构
/// </summary>
public enum ProgramArchitecture {
    /// <summary>
    /// 未知
    /// </summary>
    Unknown,
    /// <summary>
    /// x86架构
    /// </summary>
    X86,
    /// <summary>
    /// x64架构
    /// </summary>
    X64,
}

/// <summary>
/// AppUtils
/// </summary>
public static class AppUtils {
    /// <summary>
    /// 获取程序架构
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <returns></returns>
    public static ProgramArchitecture GetProgramArchitecture(string path) {
        var isExe = Kernel32.GetBinaryType(path, out var type);
        if (!isExe) {
            return ProgramArchitecture.Unknown;
        }
        return type switch {
            Kernel32.SCS.SCS_32BIT_BINARY => ProgramArchitecture.X86,
            Kernel32.SCS.SCS_64BIT_BINARY => ProgramArchitecture.X64,
            _ => ProgramArchitecture.Unknown,
        };
    }
}
