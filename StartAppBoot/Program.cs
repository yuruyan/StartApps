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
ICollection<AppTaskPO>? tasks = JsonConvert.DeserializeObject<ICollection<AppTaskPO>>(File.ReadAllText(configurationFile));
// 解析失败
if (tasks is null) {
    return;
}
// 筛选
tasks = tasks.Where(task => task.IsEnabled).ToList();
var runningTasks = new Task[tasks.Count];
int i = 0;
// 执行
foreach (var item in tasks) {
    runningTasks[i++] = Task.Run(() => {
        // 延迟执行
        if (item.Delay > 0) {
            Thread.Sleep(item.Delay);
        }
        try {
            Logger.Debug($"Starting process {item.Name}");
            // 启动任务
            var process = Process.Start(new ProcessStartInfo {
                FileName = item.Path,
                Arguments = item.Args,
            });
            // 失败
            if (process is null) {
                throw new Exception("Start process failed");
            }
        } catch (Exception error) {
            Logger.Error($"Start process '{item.Name}' failed");
            Logger.Error($"\t{error.Message}");
        }
    });
}
Task.WaitAll(runningTasks);
