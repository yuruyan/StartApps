using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Shared.Model;

/// <summary>
/// AppTaskPO
/// </summary>
[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class AppTaskPO {
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 延迟时间(ms)
    /// </summary>
    public int Delay { get; set; }
    /// <summary>
    /// 项目名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 可执行文件路径
    /// </summary>
    public string Path { get; set; } = string.Empty;
    /// <summary>
    /// 程序参数
    /// </summary>
    public string Args { get; set; } = string.Empty;
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }
    /// <summary>
    /// 以管理员身份运行
    /// </summary>
    [JsonProperty("runAsAdmin")]
    public bool RunAsAdministrator { get; set; }
}
