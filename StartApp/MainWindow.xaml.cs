namespace StartApp;

public partial class MainWindow : BaseWindow {
    public MainWindow() {
        InitializeComponent();
        this.SetLoadedOnceEventHandler((_, _) => {
            WindowHelper.ApplyOptimalThemeStyle(this);
            // 初始化
            ContentFrame.Content = new MainView();
        });
    }
}
