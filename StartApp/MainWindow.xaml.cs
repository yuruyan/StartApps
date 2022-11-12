using StartApp.View;
using System.Windows;

namespace StartApp;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        CommonUITools.App.RegisterWidgetPage(this);
        // 初始化
        ContentFrame.Content = new MainView();
    }
}
