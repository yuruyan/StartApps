using AutoMapper;
using Newtonsoft.Json.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StartApp.Model;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class AppTask : DependencyObject, ICloneable {
    public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(int), typeof(AppTask), new PropertyMetadata(0));
    public static readonly DependencyProperty DelayProperty = DependencyProperty.Register("Delay", typeof(int), typeof(AppTask), new PropertyMetadata(0));
    public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(AppTask), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(AppTask), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty ArgsProperty = DependencyProperty.Register("Args", typeof(string), typeof(AppTask), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(AppTask), new PropertyMetadata(true));
    public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(AppTask), new PropertyMetadata());
    public static readonly DependencyProperty RunAsAdministratorProperty = DependencyProperty.Register("RunAsAdministrator", typeof(bool), typeof(AppTask), new PropertyMetadata(false));
    public static readonly DependencyProperty IconPathProperty = DependencyProperty.Register("IconPath", typeof(string), typeof(AppTask), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 规定 id 为 -1 时，该对象由 Clone 生成
    /// </summary>
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
    /// <summary>
    /// 图标路径
    /// </summary>
    public string IconPath {
        get { return (string)GetValue(IconPathProperty); }
        set { SetValue(IconPathProperty, value); }
    }
    public ImageSource ImageSource {
        get { return (ImageSource)GetValue(ImageSourceProperty); }
        set { SetValue(ImageSourceProperty, value); }
    }
    [JsonProperty("runAsAdmin")]
    public bool RunAsAdministrator {
        get { return (bool)GetValue(RunAsAdministratorProperty); }
        set { SetValue(RunAsAdministratorProperty, value); }
    }

    /// <summary>
    /// 返回 id 为 -1 的新对象
    /// </summary>
    /// <returns></returns>
    public object Clone() {
        var newTask = Mapper.Instance.Map<AppTask>(this);
        newTask.Id = -1;
        return newTask;
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