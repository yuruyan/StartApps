namespace StartApp;

public partial class App : Application {
    private static readonly Logger Logger;
    private const string NlogFileName = "nlog.config";
    public static string AppDirectory { get; }

    static App() {
        #region 创建 Nlog
        Environment.CurrentDirectory = Path.GetDirectoryName(Environment.ProcessPath)!;
        // 创建 nlog
        if (!File.Exists(NlogFileName)) {
            File.WriteAllText(NlogFileName, ResourceHelper.Nlog);
        }
        Logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region 创建 APPDirectory
        AppDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            nameof(StartApp)
        );
        // 创建 StartApp 文件夹
        try {
            Directory.CreateDirectory(AppDirectory);
        } catch (Exception error) {
            Logger.Fatal(error);
            System.Windows.MessageBox.Show($"创建文件夹StartApp失败", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(-1);
            return;
        }
        #endregion
    }

    public App() {
        new SplashScreen("favicon.png").Show(true);
    }

    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);

        #region 全局错误处理
        Current.DispatcherUnhandledException += GlobalDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += DomainUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;
        #endregion

        new MainWindow().Show();
    }

    private void TaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e) {
        Logger.Warn(e.Exception);
        e.SetObserved();
    }

    private void DomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
        if (e.ExceptionObject is Exception exception) {
            Logger.Fatal(exception);
            Environment.Exit(-1);
        }
    }

    private void GlobalDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
        Logger.Fatal(e.Exception);
        Environment.Exit(-1);
    }
}
