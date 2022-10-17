using Microsoft.Win32;
using ModernWpf.Controls;
using StartApp.Model;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace StartApp.View;

public partial class TaskDialog : ContentDialog {
    private const double MinContentWidth = 300;
    private const double MaxContentWidth = 450;

    public static readonly DependencyProperty AppTaskProperty = DependencyProperty.Register("AppTask", typeof(AppTask), typeof(TaskDialog), new PropertyMetadata());
    public static readonly DependencyProperty HeaderTagProperty = DependencyProperty.Register("HeaderTag", typeof(string), typeof(TaskDialog), new PropertyMetadata());
    public static readonly DependencyProperty InputTagProperty = DependencyProperty.Register("InputTag", typeof(string), typeof(TaskDialog), new PropertyMetadata());
    public static readonly DependencyProperty InputItemPanelTagProperty = DependencyProperty.Register("InputItemPanelTag", typeof(string), typeof(TaskDialog), new PropertyMetadata());

    public AppTask AppTask {
        get { return (AppTask)GetValue(AppTaskProperty); }
        set { SetValue(AppTaskProperty, value); }
    }
    public string HeaderTag {
        get { return (string)GetValue(HeaderTagProperty); }
        set { SetValue(HeaderTagProperty, value); }
    }
    public string InputTag {
        get { return (string)GetValue(InputTagProperty); }
        set { SetValue(InputTagProperty, value); }
    }
    public string InputItemPanelTag {
        get { return (string)GetValue(InputItemPanelTagProperty); }
        set { SetValue(InputItemPanelTagProperty, value); }
    }

    public TaskDialog() {
        HeaderTag = "HeaderNormal";
        InputTag = "InputNormal";
        InputItemPanelTag = "InputItemPanelNormal";
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
    }
}
