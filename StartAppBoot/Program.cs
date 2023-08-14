// 假设此程序以管理员身份运行
using Newtonsoft.Json;
using NLog;
using Shared.Model;
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
    };
    // 非管理员身份
    if (!runAsAdmin) {
        shellProcessStartInfo.FileName = "explorer.exe";
        shellProcessStartInfo.Arguments = $"powershell.exe";
    }
    var shellProcess = Process.Start(shellProcessStartInfo);
    foreach (var item in tasks) {
        runningTasks[i++] = Task.Run(() => {
            // 延迟执行
            if (item.Delay > 0) {
                Thread.Sleep(item.Delay);
            }
            Logger.Debug($"Starting process {item.Name}");
            // 启动任务
            var command = $"start \"{item.Path}\"";
            if (!string.IsNullOrEmpty(item.Args)) {
                command = command + $" -ArgumentList \"{item.Args.Replace("\"", "\\\"")}\"";
            }
            shellProcess?.StandardInput.WriteLine(command);
        });
    }
    Task.WaitAll(runningTasks);
}