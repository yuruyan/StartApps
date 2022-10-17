using CommonUITools.Utils;
using CommonUITools.View;
using ModernWpf.Controls;
using Newtonsoft.Json;
using Shared.Model;
using StartApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace StartApp.View;

public partial class MainView : System.Windows.Controls.Page {

    private const string ConfigurationPath = "Data.json";
    // 启动应用程序
    private const string StartAppBoootPath = "StartAppBoot.exe";
    private const double DelayVisibleThreshold = 520;
    private const double PathVisibleThreshold = 800;
    public static readonly DependencyProperty AppTasksProperty = DependencyProperty.Register("AppTasks", typeof(ObservableCollection<AppTask>), typeof(MainView), new PropertyMetadata());
    public static readonly DependencyProperty IsPathVisibleProperty = DependencyProperty.Register("IsPathVisible", typeof(bool), typeof(MainView), new PropertyMetadata(false));
    public static readonly DependencyProperty IsDelayVisibleProperty = DependencyProperty.Register("IsDelayVisible", typeof(bool), typeof(MainView), new PropertyMetadata(false));

    private readonly TaskDialog TaskDialog = new();
    public ObservableCollection<AppTask> AppTasks {
        get { return (ObservableCollection<AppTask>)GetValue(AppTasksProperty); }
        set { SetValue(AppTasksProperty, value); }
    }
    /// <summary>
    /// 路径是否可见
    /// </summary>
    public bool IsPathVisible {
        get { return (bool)GetValue(IsPathVisibleProperty); }
        set { SetValue(IsPathVisibleProperty, value); }
    }
    /// <summary>
    /// 延迟是否可见
    /// </summary>
    public bool IsDelayVisible {
        get { return (bool)GetValue(IsDelayVisibleProperty); }
        set { SetValue(IsDelayVisibleProperty, value); }
    }

    public MainView() {
        AppTasks = new();
        InitializeComponent();
        LoadConfigurationAsync();
        App.Current.MainWindow.SizeChanged += (s, e) => {
            double width = e.NewSize.Width;
            IsDelayVisible = width > DelayVisibleThreshold;
            IsPathVisible = width > PathVisibleThreshold;
        };
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    private async void LoadConfigurationAsync() {
        var appTasks = JsonConvert.DeserializeObject<IList<AppTaskPO>>(await File.ReadAllTextAsync(ConfigurationPath));
        if (appTasks is null) {
            return;
        }
        // 读取配置，添加到列表
        foreach (var item in Mapper.Instance.Map<IEnumerable<AppTask>>(appTasks)) {
            AppTasks.Add(item);
        }
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    /// <returns></returns>
    private Task UpdateConfigurationAsync() {
        return File.WriteAllTextAsync(
            ConfigurationPath,
            JsonConvert.SerializeObject(Mapper.Instance.Map<IEnumerable<AppTaskPO>>(AppTasks))
        );
    }

    /// <summary>
    /// 添加任务
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void AddTaskClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (TaskDialog.IsVisible) {
            return;
        }
        TaskDialog.AppTask = new();
        // 确认添加
        if (await TaskDialog.ShowAsync() != ContentDialogResult.Primary) {
            return;
        }
        var taskCopy = Mapper.Instance.Map<AppTask>(TaskDialog.AppTask);
        // 合法性检查
        if (!IsAppTaskValid(taskCopy)) {
            MessageBox.Error("路径不能为空");
            return;
        }
        // 补全 Name
        if (string.IsNullOrEmpty(taskCopy.Name)) {
            taskCopy.Name = Path.GetFileNameWithoutExtension(taskCopy.Path);
        }
        AppTasks.Add(taskCopy);
        UpdateConfigurationAsync();
    }

    /// <summary>
    /// 检查 AppTask 是否合法
    /// </summary>
    /// <param name="appTask"></param>
    /// <returns></returns>
    private bool IsAppTaskValid(AppTask appTask) {
        return !string.IsNullOrEmpty(appTask.Path);
    }

    /// <summary>
    /// 开始运行
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StartRunningAllTasksClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (AppTasks.Count == 0) {
            return;
        }
        // 检查文件
        if (!File.Exists(StartAppBoootPath)) {
            MessageBox.Error($"{StartAppBoootPath} 丢失");
            return;
        }
        if (!File.Exists(ConfigurationPath)) {
            MessageBox.Error($"{ConfigurationPath} 丢失");
            return;
        }
        var process = CommonUtils.Try(() => Process.Start(StartAppBoootPath, ConfigurationPath));
        if (process == null) {
            MessageBox.Error($"启动程序 {StartAppBoootPath} 失败");
        }
    }

    /// <summary>
    /// 运行单个任务
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RunTaskClickHandler(object sender, RoutedEventArgs e) {
        if (sender is FrameworkElement element && element.DataContext is AppTask task) {
            try {
                Process.Start(task.Path, task.Args);
            } catch {
                MessageBox.Error("启动失败");
            }
        }
    }

    /// <summary>
    /// 移除任务
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void RemoveAppTaskClickHandler(object sender, RoutedEventArgs e) {
        if (sender is FrameworkElement element && element.DataContext is AppTask task) {
            WarningDialog warningDialog = WarningDialog.Shared;
            if (warningDialog.IsVisible) {
                return;
            }
            warningDialog.DetailText = $"是否要删除 '{task.Name}' ？";
            if (await warningDialog.ShowAsync() != ContentDialogResult.Primary) {
                return;
            }
            AppTasks.Remove(task);
            UpdateConfigurationAsync();
        }
    }

    /// <summary>
    /// 修改任务
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ModifyAppTaskClickHandler(object sender, RoutedEventArgs e) {
        if (sender is FrameworkElement element && element.DataContext is AppTask task) {
            if (TaskDialog.IsVisible) {
                return;
            }
            TaskDialog.AppTask = Mapper.Instance.Map<AppTask>(task);
            if (await TaskDialog.ShowAsync() != ContentDialogResult.Primary) {
                return;
            }
            Mapper.Instance.Map(TaskDialog.AppTask, task);
            UpdateConfigurationAsync();
        }
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ToggledHandler(object sender, RoutedEventArgs e) => UpdateConfigurationAsync();

    /// <summary>
    /// 打开文件所在位置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenDirectoryClickHandler(object sender, RoutedEventArgs e) {
        if (sender is FrameworkElement element && element.DataContext is AppTask task) {
            UIUtils.OpenFileInDirectoryAsync(task.Path);
        }
    }
}
