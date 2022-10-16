using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Windows;
using AutoMapper;

namespace StartApp.Model;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class AppTask : DependencyObject {
    public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(int), typeof(AppTask), new PropertyMetadata(0));
    public static readonly DependencyProperty DelayProperty = DependencyProperty.Register("Delay", typeof(int), typeof(AppTask), new PropertyMetadata(0));
    public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(AppTask), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(AppTask), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty ArgsProperty = DependencyProperty.Register("Args", typeof(string), typeof(AppTask), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(AppTask), new PropertyMetadata(true));

    public int Id {
        get { return (int)GetValue(IdProperty); }
        set { SetValue(IdProperty, value); }
    }
    public int Delay {
        get { return (int)GetValue(DelayProperty); }
        set { SetValue(DelayProperty, value); }
    }
    public string Name {
        get { return (string)GetValue(NameProperty); }
        set { SetValue(NameProperty, value); }
    }
    public string Path {
        get { return (string)GetValue(PathProperty); }
        set { SetValue(PathProperty, value); }
    }
    public string Args {
        get { return (string)GetValue(ArgsProperty); }
        set { SetValue(ArgsProperty, value); }
    }
    public bool IsEnabled {
        get { return (bool)GetValue(IsEnabledProperty); }
        set { SetValue(IsEnabledProperty, value); }
    }

}

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class AppTaskPO {
    public int Id { get; set; }
    public int Delay { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Args { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}

public class AppTaskProfile : Profile {
    public AppTaskProfile() {
        CreateMap<AppTask, AppTask>();
        CreateMap<AppTaskPO, AppTaskPO>();

        CreateMap<AppTaskPO, AppTask>();
        CreateMap<AppTask, AppTaskPO>();
    }
}