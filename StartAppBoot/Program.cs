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
    int i = 0;
    foreach (var item in tasks) {
        runningTasks[i++] = Task.Run(() => {
            // 延迟执行
            if (item.Delay > 0) {
                Thread.Sleep(item.Delay);
            }
            try {
                Logger.Debug($"Starting process {item.Name}");
                // 启动任务
                var filename = item.Path;
                var args = item.Args;
                // 非管理员身份
                if (!runAsAdmin) {
                    filename = "explorer.exe";
                    args = $"{item.Path} {item.Args}";
                }
                var process = Process.Start(new ProcessStartInfo {
                    FileName = filename,
                    Arguments = args,
                    UseShellExecute = true
                }) ?? throw new Exception("Start process failed");
            } catch (Exception error) {
                Logger.Error($"Start process '{item.Name}' failed");
                Logger.Error($"\t{error.Message}");
            }
        });
    }
    Task.WaitAll(runningTasks);
}