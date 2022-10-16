using StartApp.Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace StartApp.Widget;

public partial class AppTaskItem : UserControl {
    public static readonly DependencyProperty AppTaskProperty = DependencyProperty.Register("AppTask", typeof(AppTask), typeof(AppTaskItem), new PropertyMetadata());
    public event EventHandler<RoutedEventArgs>? Toggled;

    public AppTask AppTask {
        get { return (AppTask)GetValue(AppTaskProperty); }
        set { SetValue(AppTaskProperty, value); }
    }

    public AppTaskItem() {
        InitializeComponent();
    }

    /// <summary>
    /// 切换
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ToggledHandler(object sender, RoutedEventArgs e) {
        Toggled?.Invoke(sender, e);
    }
}
