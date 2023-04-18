using CommonUITools.Controls;
using CommonUITools.Model;
using CommonUITools.Themes;

namespace StartApp;

public partial class MainWindow : BaseWindow {
    public MainWindow() {
        InitializeComponent();
        UpdateTheme(SystemColorsHelper.CurrentSystemTheme);
        SystemColorsHelper.SystemThemeChanged += (_, theme) => UpdateTheme(theme);
        // 初始化
        ContentFrame.Content = new MainView();
    }

    private void UpdateTheme(ThemeMode theme) {
        if (theme == ThemeMode.Light) {
            ThemeManager.SwitchToLightTheme();
        } else {
            ThemeManager.SwitchToDarkTheme();
        }
    }
}
