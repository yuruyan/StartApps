// 如果有管理员权限运行的任务，那么此程序是以管理员身份运行的
using CommonTools.Utils;
using Newtonsoft.Json;
using NLog;
using Shared.Model;
using StartAppBoot;
using System.Diagnostics;

Logger Logger = LogManager.GetCurrentClassLogger();
string DefaultConfigurationFile = "Data.json";
string configurationFile = DefaultConfigurationFile;

// file 参数
if (args.Length > 0) {
    configurationFile = args[0];
}
// 文件不存在
if (!File.Exists(configurationFile)) {
    Logger.Error($"文件 '{configurationFile}' 不存在");
    return;
}
var tasks = JsonConvert.DeserializeObject<IEnumerable<AppTaskPO>>(File.ReadAllText(configurationFile));
// 解析失败
if (tasks is null) {
    return;
}
// 筛选
tasks = tasks.Where(task => task.IsEnabled);
var normalTasks = tasks.Where(task => !task.RunAsAdministrator).ToList();
var adminTasks = tasks.Where(task => task.RunAsAdministrator).ToList();

var normalRunningTasks = new Task[normalTasks.Count];
var adminRunningTasks = new Task[adminTasks.Count];

// 执行
RunTask(normalTasks, normalRunningTasks, false);
RunTask(adminTasks, adminRunningTasks, true);

void RunTask(IEnumerable<AppTaskPO> tasks, Task[] runningTasks, bool runAsAdmin) {
    if (runningTasks.Length == 0) {
        return;
    }
    int i = 0;
    var shellProcessStartInfo = new ProcessStartInfo {
        FileName = "powershell.exe",
        RedirectStandardInput = true,
        CreateNoWindow = true,
    };
    Process? shellProcess = null;
    if (runAsAdmin) {
        shellProcess = Process.Start(shellProcessStartInfo);
    }
    foreach (var item in tasks) {
        runningTasks[i++] = Task.Run(() => {
            // 延迟执行
            if (item.Delay > 0) {
                Thread.Sleep(item.Delay);
            }
            Logger.Debug($"Starting process {item.Name}");
            var taskEscapeQuote = item.Args.Replace("\"", "\\\"");
            if (runAsAdmin) {
                // 启动任务
                var command = $"start \"{item.Path}\"";
                if (!string.IsNullOrEmpty(item.Args)) {
                    command += $" -ArgumentList \"{taskEscapeQuote}\"";
                }
                shellProcess?.StandardInput.WriteLine(command);
            }
            // 非管理员身份
            else {
                var args = string.IsNullOrWhiteSpace(item.Args) ? string.Empty : taskEscapeQuote;
                var arch = TaskUtils.Try(() => AppUtils.GetProgramArchitecture(item.Path));
                var archArg = arch switch {
                    ProgramArchitecture.X86 => "x86",
                    _ => "amd64",
                };
                Process.Start(new ProcessStartInfo {
                    FileName = "runas.exe",
                    CreateNoWindow = true,
                    Arguments = $"/trustlevel:0x20000 /machine:{archArg} \"{item.Path} {args}\""
                });
            }
        });
    }
    Task.WaitAll(runningTasks);
}