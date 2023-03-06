namespace StartApp;
using MessageBox = System.Windows.MessageBox;

public partial class App : Application {
    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);

        #region 全局错误处理
        Current.DispatcherUnhandledException += GlobalDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += DomainUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;
        #endregion
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
        e.Handled = true;
        if (e.Exception is System.Runtime.InteropServices.COMException comException) {
            if (comException.ErrorCode == -2147221040) {
                return;
            }
        }
        // 其他处理
        if (e.Exception is InvalidOperationException
            || e.Exception is IOException
        ) {
            MessageBox.Show(
                Current.MainWindow,
                e.Exception.Message,
                "错误",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }
}
