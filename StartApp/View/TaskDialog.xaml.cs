using Microsoft.Win32;

namespace StartApp.View;

public partial class TaskDialog : BaseDialog {
    public static readonly DependencyProperty AppTaskProperty = DependencyProperty.Register("AppTask", typeof(AppTask), typeof(TaskDialog), new PropertyMetadata());

    public AppTask AppTask {
        get { return (AppTask)GetValue(AppTaskProperty); }
        set { SetValue(AppTaskProperty, value); }
    }

    public TaskDialog() : base(true) {
        InitializeComponent();
        this.EnableAutoResize(
            (double)Resources["RegexListItemsControlMinWidth"]
        );
    }

    /// <summary>
    /// 阻止 Enter 提交
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PanelKeyDownHandler(object sender, KeyEventArgs e) {
        if (e.Key == Key.Enter) {
            e.Handled = true;
        }
    }

    /// <summary>
    /// 修改任务路径
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenTaskDirectoryMouseUpHandler(object sender, MouseButtonEventArgs e) {
        e.Handled = true;
        var dialog = new OpenFileDialog {
            Filter = "可执行程序|*.exe|全部文件|*.*",
        };
        // 设置文件路径
        if (File.Exists(AppTask.Path)) {
            dialog.InitialDirectory = new FileInfo(AppTask.Path).DirectoryName;
            dialog.FileName = Path.GetFileName(AppTask.Path);
        }
        if (dialog.ShowDialog() != true) {
            return;
        }
        AppTask.Path = dialog.FileName;
        // 自动填充 Name
        if (string.IsNullOrEmpty(AppTask.Name)) {
            AppTask.Name = Path.GetFileNameWithoutExtension(AppTask.Path);
        }
    }

    /// <summary>
    /// 当清除输入框时，设置 Delay 为 0
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void DelayValueChangedHandler(NumberBox sender, NumberBoxValueChangedEventArgs args) {
        if (double.IsNaN(sender.Value)) {
            AppTask.Delay = 0;
        }
    }

    /// <summary>
    /// 打开图标选择对话框
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenIconSelectionDialogMouseUpHandler(object sender, MouseButtonEventArgs e) {
        e.Handled = true;
        var dialog = new OpenFileDialog {
            Filter = "图标文件|*.ico;*.png;*.jpg",
        };
        if (dialog.ShowDialog() != true) {
            return;
        }
        AppTask.IconPath = dialog.FileName;
    }
}
