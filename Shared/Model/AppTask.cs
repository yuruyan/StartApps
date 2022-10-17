using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Shared.Model;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class AppTaskPO {
    public int Id { get; set; }
    public int Delay { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Args { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}
