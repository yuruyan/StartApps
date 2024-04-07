﻿using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace StartApp.Widget;

public partial class AppTaskItem : UserControl {
    public static readonly DependencyProperty AppTaskProperty = DependencyProperty.Register("AppTask", typeof(AppTask), typeof(AppTaskItem), new PropertyMetadata());
    public static readonly DependencyProperty IsPathVisibleProperty = DependencyProperty.Register("IsPathVisible", typeof(bool), typeof(AppTaskItem), new PropertyMetadata(false));
    public static readonly DependencyProperty IsDelayVisibleProperty = DependencyProperty.Register("IsDelayVisible", typeof(bool), typeof(AppTaskItem), new PropertyMetadata(false));

    public event EventHandler<RoutedEventArgs>? Toggled;

    public AppTask AppTask {
        get { return (AppTask)GetValue(AppTaskProperty); }
        set { SetValue(AppTaskProperty, value); }
    }
    /// <summary>
    /// 路径是否可见
    /// </summary>
    public bool IsPathVisible {
        get { return (bool)GetValue(IsPathVisibleProperty); }
        set { SetValue(IsPathVisibleProperty, value); }
    }
    /// <summary>
    /// 延迟是否可见
    /// </summary>
    public bool IsDelayVisible {
        get { return (bool)GetValue(IsDelayVisibleProperty); }
        set { SetValue(IsDelayVisibleProperty, value); }
    }
    private string Path = string.Empty;
    private string IconPath = string.Empty;

    public AppTaskItem() {
        InitializeComponent();
    }

    /// <summary>
    /// 重新加载图标
    /// </summary>
    public void ReloadIcon() => AppTaskItemLoadedHandler(this, new RoutedEventArgs());

    /// <summary>
    /// 切换
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ToggledHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Toggled?.Invoke(sender, e);
    }

    /// <summary>
    /// 加载图标
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void AppTaskItemLoadedHandler(object sender, RoutedEventArgs e) {
        if (!ShouldReloadIcon()) {
            return;
        }
        var task = this.AppTask;
        var iconPath = task.IconPath;
        // 加载图标
        if (File.Exists(iconPath)) {
            var iconStream = await task.Dispatcher.InvokeAsync(() => TaskUtils.Try(iconPath.GetImageSource));
            if (iconStream is not null) {
                task.ImageSource = iconStream;
                return;
            }
        }
        var tempPath = task.Path;
        // 从可执行文件加载图标
        var stream = await Task.Run(() => Utils.GetExeBitmap(tempPath));
        if (stream == null) {
            return;
        }
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = stream;
        bitmapImage.EndInit();
        task.ImageSource = bitmapImage;
    }

    /// <summary>
    /// 是否需要重新加载图标, 路径发生变化就需要重新加载
    /// </summary>
    /// <returns></returns>
    private bool ShouldReloadIcon() {
        if (Path != AppTask.Path || IconPath != AppTask.IconPath) {
            Path = AppTask.Path;
            IconPath = AppTask.IconPath;
            return true;
        }
        return false;
    }
}

/// <summary>
/// 程序图标
/// </summary>
public class IconConverter : IValueConverter {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">AppTask.Path</param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        // 不合法
        if (value == null || value is not string path || !File.Exists(path)) {
            return null!;
        }
        Stream? stream = Utils.GetExeBitmap(path);
        if (stream == null) {
            return null!;
        }
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = stream;
        bitmapImage.EndInit();
        return bitmapImage;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 时间单位
/// </summary>
public class TimeUnitConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        double mills = System.Convert.ToDouble(value);
        return mills switch {
            0 => 0,
            < 1000 => $"{mills} ms",
            < ConstantUtils.OneMinuteMillisecond => $"{mills / 1000:#.##} s",
            < ConstantUtils.OneHourMillisecond => $"{mills / ConstantUtils.OneMinuteMillisecond:#.##} m",
            < ConstantUtils.OneDayMillisecond => $"{mills / ConstantUtils.OneHourMillisecond:#.##} h",
            _ => $"{mills / ConstantUtils.OneDayMillisecond:#.##}",
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Path ColumnDefinition
/// </summary>
public class PathColumnConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (System.Convert.ToBoolean(value)) {
            return new GridLength(2, GridUnitType.Star);
        }
        return new GridLength(0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Delay ColumnDefinition
/// </summary>
public class DelayColumnConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (System.Convert.ToBoolean(value)) {
            return new GridLength(80);
        }
        return new GridLength(0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// delay 为 0 时也隐藏
/// </summary>
public class DelayMultiConverter : IMultiValueConverter {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
        if (values.Length < 2) {
            return Visibility.Visible;
        }
        bool isVisible = System.Convert.ToBoolean(values[0]);
        if (values[1] is int delay) {
            return !isVisible || delay == 0 ? Visibility.Hidden : Visibility.Visible;
        }
        return Visibility.Visible;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}