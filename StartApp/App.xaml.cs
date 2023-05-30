namespace StartApp;
using MessageBox = System.Windows.MessageBox;

public partial class App : Application {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);

        #region 全局错误处理
        Current.DispatcherUnhandledException += GlobalDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += DomainUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;
        #endregion

        new SplashScreen("favicon.png").Show(true);
        new MainWindow().Show();
    }

    private void TaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e) {
        MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void DomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
        if (e.ExceptionObject is Exception exception) {
            Shutdown();
        }
    }

    private void GlobalDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
        Logger.Fatal(e.Exception);
        Environment.Exit(-1);
    }
}
