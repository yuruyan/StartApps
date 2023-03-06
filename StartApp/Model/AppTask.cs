using AutoMapper;
using Newtonsoft.Json.Serialization;

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

public class AppTaskProfile : Profile {
    public AppTaskProfile() {
        CreateMap<AppTask, AppTask>();
        CreateMap<AppTaskPO, AppTaskPO>();

        CreateMap<AppTaskPO, AppTask>();
        CreateMap<AppTask, AppTaskPO>();
    }
}