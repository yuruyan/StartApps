using ModernWpf.Controls;
using StartApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StartApp.View;

public partial class TaskDialog : ContentDialog {

    public static readonly DependencyProperty AppTaskProperty = DependencyProperty.Register("AppTask", typeof(AppTask), typeof(TaskDialog), new PropertyMetadata());
    public static readonly DependencyProperty HeaderTagProperty = DependencyProperty.Register("HeaderTag", typeof(string), typeof(TaskDialog), new PropertyMetadata());
    public static readonly DependencyProperty InputTagProperty = DependencyProperty.Register("InputTag", typeof(string), typeof(TaskDialog), new PropertyMetadata());
    public static readonly DependencyProperty InputItemPanelTagProperty = DependencyProperty.Register("InputItemPanelTag", typeof(string), typeof(TaskDialog), new PropertyMetadata());

    public AppTask AppTask {
        get { return (AppTask)GetValue(AppTaskProperty); }
        set { SetValue(AppTaskProperty, value); }
    }
    public string HeaderTag {
        get { return (string)GetValue(HeaderTagProperty); }
        set { SetValue(HeaderTagProperty, value); }
    }
    public string InputTag {
        get { return (string)GetValue(InputTagProperty); }
        set { SetValue(InputTagProperty, value); }
    }
    public string InputItemPanelTag {
        get { return (string)GetValue(InputItemPanelTagProperty); }
        set { SetValue(InputItemPanelTagProperty, value); }
    }

    public TaskDialog() {
        HeaderTag = "HeaderNormal";
        InputTag = "InputNormal";
        InputItemPanelTag = "InputItemPanelNormal";
        InitializeComponent();
    }

    /// <summary>
    /// 阻止 Enter 提交
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PanelKeyDownHandler(object sender, KeyEventArgs e) {
        if (e.Key == Key.Enter) {
            e.Handled = true;
        }
    }
}
