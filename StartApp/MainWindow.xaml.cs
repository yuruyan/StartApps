using StartApp.View;
using System.Windows;

namespace StartApp;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        // 初始化
        CommonUITools.Widget.MessageBox.PanelChildren = MessageBoxPanel.Children;
        CommonUITools.Widget.NotificationBox.PanelChildren = NotificationPanel.Children;
        ContentFrame.Content = new MainView();
    }
}
