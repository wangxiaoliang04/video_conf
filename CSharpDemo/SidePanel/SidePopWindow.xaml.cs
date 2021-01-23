using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using VenueRtcWpf.Command;
using VenueRtcWpf.Resources.Controls.CButton;
using VenueRtcWpf.Resources.Controls.StyleableWindow;

namespace VenueRtcWpf
{
    /// <summary>
    /// SidePopWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SidePopWindow : Window
    {
        WeakReference wr_mainwindow = new WeakReference(null);
        private Visibility togglebuttonVisibility;

        public MainWindow MainWindow { get => (MainWindow)( wr_mainwindow.Target); set => wr_mainwindow.Target = value; }


        public SidePopWindow(string title, Visibility togglebuttonVisibility)
        {
            InitializeComponent();
            this.Title = title;
            this.togglebuttonVisibility = togglebuttonVisibility;
        }
        

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                base.OnClosing(e);
                var vm = this.MainWindow.DataContext as MainWindowViewModel;
                if ((this.Content as IDockControl).ClassName == nameof(SideUserList) && !(this.Content as IDockControl).IsDocked)
                {
                    vm.ToggleUserState = "打开用户列表";

                }
                else if ((this.Content as IDockControl).ClassName == nameof(SideChatView) && !(this.Content as IDockControl).IsDocked)
                {
                    vm.ToggleChatState = "打开聊天消息";
                }
            }
            catch { }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //var rwc = this.WindowButtonCommands as WindowButtonCommandsEx;
            //rwc.ToggleDock.Click += (ss, ee) =>
            //{
            //    (this.MainWindow as MainWindow).ToggleSideDock(this.Content as IDockControl);
            //};
        }
    }
}
