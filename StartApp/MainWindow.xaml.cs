using CommonUITools.Controls;

namespace StartApp;

public partial class MainWindow : BaseWindow {
    public MainWindow() {
        InitializeComponent();
        // 初始化
        ContentFrame.Content = new MainView();
    }
}
