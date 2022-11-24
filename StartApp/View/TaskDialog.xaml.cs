using Microsoft.Win32;
using ModernWpf.Controls;
using StartApp.Model;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace StartApp.View;

public partial class TaskDialog : ContentDialog {
    private const double MinContentWidth = 300;
    private const double MaxContentWidth = 600;

    public static readonly DependencyProperty AppTaskProperty = DependencyProperty.Register("AppTask", typeof(AppTask), typeof(TaskDialog), new PropertyMetadata());

    public AppTask AppTask {
        get { return (AppTask)GetValue(AppTaskProperty); }
        set { SetValue(AppTaskProperty, value); }
    }

    public TaskDialog() {
        InitializeComponent();
        InitContentScrollViewer();
    }

    /// <summary>
    /// 初始化 ContentScrollViewer
    /// </summary>
    private void InitContentScrollViewer() {
        // 随窗口大小变化，改变 ContentScrollViewer 宽度
        ContentScrollViewer.Width = MinContentWidth;
        App.Current.MainWindow.SizeChanged += (s, e) => {
            double newWidth = e.NewSize.Width / 1.5;
            ContentScrollViewer.Width = newWidth switch {
                < MinContentWidth => MinContentWidth,
                > MaxContentWidth => MaxContentWidth,
                _ => newWidth
            };
        };
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
}
