using CommonTools;
using CommonTools.Utils;
using Microsoft.Extensions.Logging;
using Shared.Model;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

var Logger = SharedLogging.FileLogger;
string DefaultConfigurationFile = "Data.json";
string configurationFile = DefaultConfigurationFile;

void Run() {
    // file 参数
    if (args.Length > 0) {
        configurationFile = args[0];
    }
    // 文件不存在
    if (!File.Exists(configurationFile)) {
        Logger.LogError("File '{configurationFile}' does not exist", configurationFile);
        return;
    }
    var appTasks = TaskUtils.Try(() => JsonSerializer.Deserialize(
        File.ReadAllText(configurationFile), SourceGenerationContext.Default.ListAppTaskPO
    ));
    // 解析失败
    if (appTasks is null) {
        Logger.LogError("Parse file '{configurationFile}' failed", configurationFile);
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
            Logger.LogInformation("Starting process '{Name}'", item.Name);
            try {
                Process.Start(new ProcessStartInfo {
                    FileName = item.Path,
                    Arguments = item.Args,
                    UseShellExecute = true,
                });
            } catch (Exception error) {
                Logger.LogError(error, "Failed to start process '{Name}'", item.Name);
            }
        });
    }
    Task.WaitAll(runningTasks);
}

try {
    Run();
} catch (Exception error) {
    Logger.LogError(error, "Program terminated unexpectedly");
} finally {
    SharedLogging.Dispose();
}
