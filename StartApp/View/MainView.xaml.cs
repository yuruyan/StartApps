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
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace StartApp.View;

public partial class MainView : System.Windows.Controls.Page {

    private const string ConfigurationPath = "Data.json";
    // 启动应用程序
    private const string StartAppBootPath = "StartAppBoot.exe";
    private const double DelayVisibleThreshold = 520;
    private const double PathVisibleThreshold = 800;
    public static readonly DependencyProperty AppTasksProperty = DependencyProperty.Register("AppTasks", typeof(ObservableCollection<AppTask>), typeof(MainView), new PropertyMetadata());
    public static readonly DependencyProperty IsPathVisibleProperty = DependencyProperty.Register("IsPathVisible", typeof(bool), typeof(MainView), new PropertyMetadata(false));
    public static readonly DependencyProperty IsDelayVisibleProperty = DependencyProperty.Register("IsDelayVisible", typeof(bool), typeof(MainView), new PropertyMetadata(false));
    public static readonly DependencyProperty IsStartedAsAdminProperty = DependencyProperty.Register("IsStartedAsAdmin", typeof(bool), typeof(MainView), new PropertyMetadata(false));
    public static readonly DependencyProperty CopiedTaskIdListProperty = DependencyProperty.Register("CopiedTaskIdList", typeof(ICollection<int>), typeof(MainView), new PropertyMetadata());

    /// <summary>
    /// AppTaskId 集合
    /// </summary>
    private static ISet<int> AppTaskIdSet => new HashSet<int>(new int[] { 0 });
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
    /// <summary>
    /// 是否以管理员身份运行
    /// </summary>
    public bool IsStartedAsAdmin {
        get { return (bool)GetValue(IsStartedAsAdminProperty); }
        set { SetValue(IsStartedAsAdminProperty, value); }
    }
    /// <summary>
    /// 复制的 AppTask id
    /// </summary>
    public ICollection<int> CopiedTaskIdList {
        get { return (ICollection<int>)GetValue(CopiedTaskIdListProperty); }
        set { SetValue(CopiedTaskIdListProperty, value); }
    }

    public MainView() {
        AppTasks = new();
        CopiedTaskIdList = new List<int>();
        InitializeComponent();
        LoadConfigurationAsync();
        #region 设置 IsStartedAsAdmin
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new(identity);
        IsStartedAsAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
        #endregion
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
            AppTasks.Add(CheckAndSetTaskId(item));
        }
    }

    /// <summary>
    /// 检查和设置 AppTask.Id
    /// </summary>
    /// <param name="task"></param>
    /// <returns>同 task</returns>
    private AppTask CheckAndSetTaskId(AppTask task) {
        if (AppTaskIdSet.Contains(task.Id)) {
            task.Id = GenerateUniqueTaskId();
        }
        AppTaskIdSet.Add(task.Id);
        return task;
    }

    /// <summary>
    /// 生成唯一 AppTask.Id
    /// </summary>
    /// <returns></returns>
    private int GenerateUniqueTaskId() {
        while (true) {
            int id = Random.Shared.Next();
            if (!AppTaskIdSet.Contains(id)) {
                return id;
            }
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
        AppTasks.Add(CheckAndSetTaskId(taskCopy));
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
        // 检查
        if (!CheckRunningTask()) {
            return;
        }
        var process = CommonUtils.Try(() => RunStartAppBoot(ConfigurationPath));
        if (process == null) {
            MessageBox.Error($"启动程序 {StartAppBootPath} 失败");
        }
    }

    /// <summary>
    /// 检查开始运行任务条件，with Notification
    /// </summary>
    /// <returns></returns>
    private bool CheckRunningTask() {
        if (AppTasks.Count == 0) {
            return false;
        }
        // 检查文件
        if (!File.Exists(StartAppBootPath)) {
            MessageBox.Error($"{StartAppBootPath} 丢失");
            return false;
        }
        if (!File.Exists(ConfigurationPath)) {
            MessageBox.Error($"{ConfigurationPath} 丢失");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 以管理员身份运行
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StartRunningAllTasksAsAdminClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 检查
        if (!CheckRunningTask()) {
            return;
        }
        var process = CommonUtils.Try(() => RunStartAppBoot(ConfigurationPath, true));
        // 失败
        if (process == null) {
            MessageBox.Error($"启动程序 {StartAppBootPath} 失败");
        }
    }

    /// <summary>
    /// 立即运行选中项任务
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RunTaskClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        foreach (AppTask task in AppTaskListBox.SelectedItems) {
            try {
                Process.Start(task.Path, task.Args);
            } catch {
                MessageBox.Error($"'{task.Name}' 启动失败");
            }
        }
    }

    /// <summary>
    /// 启动 StartAppBoot，with Notification
    /// </summary>
    /// <param name="args">运行参数，不检查此</param>
    /// <param name="runAsAdmin">以管理员身份启动</param>
    /// <returns></returns>
    private Process? RunStartAppBoot(string args, bool runAsAdmin = false) {
        // 不存在
        if (!File.Exists(StartAppBootPath)) {
            MessageBox.Error($"{StartAppBootPath} 丢失");
            return null;
        }
        // 普通模式
        if (!runAsAdmin) {
            return Process.Start(StartAppBootPath, ConfigurationPath);
        }
        // 管理员身份
        return Process.Start(new ProcessStartInfo {
            FileName = StartAppBootPath,
            Arguments = args,
            UseShellExecute = true,
            Verb = "RunAs"
        });
    }

    /// <summary>
    /// 以管理员身份运行选中项
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void RunTaskAsAdminClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var selectedTasks = AppTaskListBox.SelectedItems;
        // 意外情况
        if (selectedTasks.Count == 0) {
            return;
        }
        #region 写入临时文件
        var tasksCopy = new AppTask[selectedTasks.Count];
        selectedTasks.CopyTo(tasksCopy, 0);
        var poList = Mapper.Instance.Map<IEnumerable<AppTaskPO>>(tasksCopy);
        // 立即运行设置
        foreach (AppTaskPO po in poList) {
            po.Delay = 0;
            po.IsEnabled = true;
        }
        string jsonData = JsonConvert.SerializeObject(poList);
        string tempConfigFile = "tempConfig.json";
        // 写入临时文件
        await File.WriteAllTextAsync(tempConfigFile, jsonData);
        #endregion
        // 启动 StartAppBoot
        var process = CommonUtils.Try(() => RunStartAppBoot(tempConfigFile, true));
        // 失败
        if (process == null) {
            MessageBox.Error($"启动程序 {StartAppBootPath} 失败");
        }
    }

    /// <summary>
    /// 移除任务
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void RemoveAppTaskClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        WarningDialog warningDialog = WarningDialog.Shared;
        if (warningDialog.IsVisible) {
            return;
        }
        var selectedTasks = AppTaskListBox.SelectedItems;
        if (selectedTasks.Count == 0) {
            return;
        }
        var detailText = $"是否删除这 {selectedTasks.Count} 项？";
        // 单个 Item
        if (selectedTasks.Count == 1) {
            detailText = $"是否要删除 '{CommonUtils.NullCheck(selectedTasks[0] as AppTask).Name}' ？";
        }
        warningDialog.DetailText = detailText;
        // show dialog
        if (await warningDialog.ShowAsync() != ContentDialogResult.Primary) {
            return;
        }
        // 移除
        var tasksCopy = new AppTask[selectedTasks.Count];
        selectedTasks.CopyTo(tasksCopy, 0);
        foreach (var item in tasksCopy) {
            AppTasks.Remove(item);
        }
        MessageBox.Success("删除成功");
        UpdateConfigurationAsync();
    }

    /// <summary>
    /// 修改任务
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ModifyAppTaskClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
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
    private void ToggledHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        UpdateConfigurationAsync();
    }

    /// <summary>
    /// 打开文件所在位置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenDirectoryClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        foreach (AppTask task in AppTaskListBox.SelectedItems) {
            UIUtils.OpenFileInDirectoryAsync(task.Path);
        }
    }

    /// <summary>
    /// 切换为 Enabled
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EnableTaskClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        foreach (AppTask item in AppTaskListBox.SelectedItems) {
            item.IsEnabled = true;
        }
    }

    /// <summary>
    /// 切换为 Disabled
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DisableTaskClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        foreach (AppTask item in AppTaskListBox.SelectedItems) {
            item.IsEnabled = false;
        }
    }

    /// <summary>
    /// 设置 EnableTaskMenuItem Visibility
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EnableTaskMenuItemLoaded(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender is FrameworkElement element) {
            // 多个 Item
            if (AppTaskListBox.SelectedItems.Count > 1) {
                element.Visibility = Visibility.Visible;
                return;
            }
            bool isEnabled = CommonUtils.NullCheck(AppTaskListBox.SelectedItem as AppTask).IsEnabled;
            element.Visibility = isEnabled ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    /// <summary>
    /// 设置 DisableTaskMenuItem Visibility
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DisableTaskMenuItemLoaded(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender is FrameworkElement element) {
            // 多个 Item
            if (AppTaskListBox.SelectedItems.Count > 1) {
                element.Visibility = Visibility.Visible;
                return;
            }
            bool isEnabled = CommonUtils.NullCheck(AppTaskListBox.SelectedItem as AppTask).IsEnabled;
            element.Visibility = !isEnabled ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    /// <summary>
    /// 复制选中项
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyAppTaskClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        CopiedTaskIdList = new List<int>(AppTaskListBox.SelectedItems.Count);
        foreach (AppTask item in AppTaskListBox.SelectedItems) {
            CopiedTaskIdList.Add(item.Id);
        }
    }

    /// <summary>
    /// 粘贴选中项
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PasteAppTaskClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (CopiedTaskIdList.Count < 1) {
            return;
        }
        foreach (var id in CopiedTaskIdList) {
            var targetTask = AppTasks.FirstOrDefault(t => t.Id == id);
            // 可能已经删除
            if (targetTask is null) {
                continue;
            }
            AppTasks.Add(CheckAndSetTaskId(
                Mapper.Instance.Map<AppTask>(targetTask)
            ));
        }
    }

    /// <summary>
    /// 粘贴选中项 LoadedHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PasteAppTaskMenuItemLoadedHandler(object sender, RoutedEventArgs e) {
        if (sender is FrameworkElement element) {
            element.Visibility = CopiedTaskIdList.Count < 1 ? Visibility.Collapsed : Visibility.Visible;
        }
    }

}
