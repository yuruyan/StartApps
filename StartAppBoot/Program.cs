using Newtonsoft.Json;
using NLog;
using Shared.Model;
using System.Diagnostics;

Logger Logger = LogManager.GetCurrentClassLogger();

// 检查参数
if (args.Length < 1) {
    return;
}
string configurationFile = args[0];
// 文件不存在
if (!File.Exists(configurationFile)) {
    return;
}
var tasks = JsonConvert.DeserializeObject<ICollection<AppTaskPO>>(configurationFile);
// 解析失败
if (tasks is null) {
    return;
}
var runningTasks = new Task[tasks.Count];
int i = 0;
foreach (var item in tasks) {
    runningTasks[i] = Task.Run(() => {
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
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            });
            // 失败
            if (process is null) {
                throw new Exception("Start process failed");
            }
            Logger.Debug(process.StandardOutput.ReadToEnd());
        } catch (Exception error) {
            Logger.Error($"Start process '{item.Name}' failed");
            Logger.Error($"\t{error.Message}");
        }
    });
}
Task.WaitAll(runningTasks);
