using System.Diagnostics;
using System.Text.RegularExpressions;

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
    private const string SigCheckFilename = "Tools/sigcheck.exe";
    /// <summary>
    /// 获取程序架构
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <returns></returns>
    public static ProgramArchitecture GetProgramArchitecture(string path) {
        var proc = Process.Start(new ProcessStartInfo {
            FileName = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), SigCheckFilename),
            RedirectStandardOutput = true,
            Arguments = $"-nobanner \"{path}\"",
        })!;
        proc.WaitForExit();
        var output = proc.StandardOutput.ReadToEnd();
        //         MachineType:    64-bit
        var match = Regex.Match(output, @"\s+MachineType:\s+(\d{2})-bit");
        if (!match.Success) {
            return ProgramArchitecture.Unknown;
        }
        return match.Groups[1].Value switch {
            "32" => ProgramArchitecture.X86,
            "64" => ProgramArchitecture.X64,
            _ => ProgramArchitecture.Unknown,
        };
    }
}
