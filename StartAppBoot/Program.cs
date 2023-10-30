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
var appTasks = JsonConvert.DeserializeObject<IList<AppTaskPO>>(
    File.ReadAllText(configurationFile)
);
// 解析失败
if (appTasks is null) {
    return;
}
// 筛选
appTasks = appTasks.Where(task => task.IsEnabled).ToList();
var runningTasks = new Task[appTasks.Count];

int i = 0;
foreach (var item in appTasks) {
    runningTasks[i++] = Task.Run(() => {
        // 延迟执行
        if (item.Delay > 0) {
            Thread.Sleep(item.Delay);
        }
        Logger.Debug($"Starting process {item.Name}");
        try {
            Process.Start(new ProcessStartInfo {
                FileName = item.Path,
                Arguments = item.Args,
                UseShellExecute = true,
            });
        } catch (Exception error) {
            Logger.Error(error);
        }
    });
}
Task.WaitAll(runningTasks);
