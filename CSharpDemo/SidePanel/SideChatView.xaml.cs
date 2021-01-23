using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VenueRtcWpf.IM;

namespace VenueRtcWpf
{
    /// <summary>
    /// SideChatView.xaml 的交互逻辑
    /// </summary>
    public partial class SideChatView : UserControl, IDockControl
    {

        public string ClassName { get => this.GetType().Name; }
        
        public bool IsDocked { get => (bool)GetValue(DockControl.IsDockedProperty); set => SetValue(DockControl.IsDockedProperty, value); }


        ObservableCollection<MessageItem> itemList = new ObservableCollection<MessageItem>();
        Delegate scroll_handler;

        IMChatWrapper iMChatWrapper;
        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;
        

        public SideChatView()
        {
            InitializeComponent();
       
            iMChatWrapper = new IMChatTcWrapper(itemList);
            iMChatWrapper.onMessageReceive += IMChatWrapper_onMessageReceive;

            messageList.ItemsSource = itemList;
            scroll_handler = new ScrollChangedEventHandler(MessageList_ScrollChanged);
            messageList.AddHandler(ScrollViewer.ScrollChangedEvent, scroll_handler);
            
            //VenueRtcCLI.VenueRTC.Instance.onJoinRoom += Instance_onJoinRoom;
            IsDocked = true;
        }
        ~SideChatView()
        {
            //VenueRtcCLI.VenueRTC.Instance.onJoinRoom -= Instance_onJoinRoom;
            iMChatWrapper.onMessageReceive -= IMChatWrapper_onMessageReceive;
        }
        private void Instance_onJoinRoom(int errorCode, string message)
        {
            if (errorCode == 200)
            {
                //VenueRtcCLI.VenueRTC.Instance.onJoinRoom -= Instance_onJoinRoom;
                if (_syncContext != SynchronizationContext.Current)
                {
                    _syncContext.Post(o =>
                    {
                        InitChatSDK();
                    }, null);
                    return;
                }
            }
        }

        public async void InitChatSDK()
        {
            await iMChatWrapper.InitChatSDK();

            if (itemList.Count > 0)
            {
                messageList.ScrollIntoView(itemList[itemList.Count - 1]);
            }
        }
        bool isMessageReceiveHasBeenCalled = false;
        private void IMChatWrapper_onMessageReceive(MessageItem newItem, bool forceScroll)
        {
            Dispatcher.InvokeAsync(() =>
            {
                var vm = ((App.Current.MainWindow as MainWindow).DataContext as MainWindowViewModel);
                if (!vm.IsChatNotice && vm.ToggleChatState == "打开聊天消息" && isMessageReceiveHasBeenCalled)
                {
                    vm.IsChatNotice = true;
                    if (!vm.ChatNoticeCount.HasValue)
                    {
                        vm.ChatNoticeCount = 0;
                    }
                }
                if (vm.IsChatNotice)
                {
                    vm.ChatNoticeCount++;
                }
                var scrollViewer = GetFirstChildOfType<ScrollViewer>(messageList);
                if (scrollViewer != null && scrollViewer.VerticalOffset < scrollViewer.ExtentHeight - scrollViewer.ViewportHeight - 20
                    && !forceScroll)
                {
                    newMsgBtn.Visibility = Visibility.Visible;
                }
                else if (newItem.type == 1)
                {
                    messageList.ScrollIntoView(newItem);
                }
                if (newItem.type == 2)
                {
                    nickName.Text = newItem.nickName;
                    bgGrid.Opacity = 1d;
                    bgGrid.BeginAnimation(FrameworkElement.OpacityProperty, null);

                    DoubleAnimation da0 = new DoubleAnimation();
                    da0.From = 1d;
                    da0.To = 0d;
                    da0.BeginTime = TimeSpan.FromSeconds(3);
                    da0.Duration = new Duration(TimeSpan.FromSeconds(0.3));
                    //center_msg.BeginAnimation(FrameworkElement.OpacityProperty, null);
                    bgGrid.BeginAnimation(FrameworkElement.OpacityProperty, da0);
                }
                isMessageReceiveHasBeenCalled = true;
            });
        }

        private async void MessageList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = GetFirstChildOfType<ScrollViewer>(messageList);
            if (e.VerticalChange != 0 && e.VerticalOffset == 0)
            {
                double oldExtentHeight = e.ExtentHeight;

                int getCount = await iMChatWrapper.getHistoryMessage();

                //if (getCount < IMChatLeanWrapper.MessagePerPage)
                //    messageList.RemoveHandler(ScrollViewer.ScrollChangedEvent, scroll_handler);
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight - oldExtentHeight);
            }
            else if(newMsgBtn.Visibility == Visibility.Visible &&
                scrollViewer.VerticalOffset > scrollViewer.ExtentHeight - scrollViewer.ViewportHeight - 20)
            {
                newMsgBtn.Visibility = Visibility.Hidden;
            }
        }

        private void TextInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textInput.Text.Length > 0)
                btnSend.IsEnabled = true;
            else
                btnSend.IsEnabled = false;
        }

        private void TextInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (textInput.Text.Length > 0)
                    Send_Click(this, new RoutedEventArgs());
            }
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textInput.Text)) return;
            MessageItem newItem = null;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    string key = nameof(iMChatWrapper.SendMessage);
                    App app = App.Current as App;
                    app.KTokenSource[key] = new CancellationTokenSource();
                    app.KTokenSourceCounter[key] = 0;

                    Task.Run(() =>
                    {
                        App.StartWaiting(key);
                        if (!(App.Current as App).KTokenSource[key].IsCancellationRequested)
                        {
                            (App.Current as App).KTokenSource[key].Cancel();
                            (App.Current as App).KTokenSource.Remove(key);
                            Dispatcher.Invoke(() =>
                            {
                                App.LogTimeOut(null, key);
                            });
                            return;
                        }
                    });
                    string textInput_Text = "";
                    Dispatcher.Invoke(() => {
                        textInput_Text = textInput.Text;
                    });
                   newItem = iMChatWrapper.SendMessage(textInput_Text);
                   if((App.Current as App).KTokenSource.ContainsKey(key))
                    (App.Current as App).KTokenSource[key]?.Cancel(); 

                }
                catch (Exception ex) { Dispatcher.InvokeAsync(() => App.LogError(ex)); }
                

            }).ContinueWith(t=> {
                Dispatcher.InvokeAsync(() => {
                    messageList.ScrollIntoView(newItem);
                    textInput.Text = "";
                    textInput.Focus();
                    newMsgBtn.Visibility = Visibility.Hidden;
                    e.Handled = true;
                });
            });
            
        }

        private void NewMssage_Click(object sender, RoutedEventArgs e)
        {
            newMsgBtn.Visibility = Visibility.Hidden;
            if (itemList.Count > 0)
            {
                messageList.ScrollIntoView(itemList[itemList.Count - 1]);
            }
        }

        #region GetFirstChildOfType

        private static T GetFirstChildOfType<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            if (dependencyObject == null)
            {
                return null;
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);

                var result = (child as T) ?? GetFirstChildOfType<T>(child);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        #endregion

        private void FlowDocumentScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scv = (sender as FrameworkElement).GetVisualAncestor<ScrollViewer>();
                
            if (scv == null) return;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void BtnDock_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsOpen = !(sender as Button).ContextMenu.IsOpen;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            (App.Current.MainWindow as MainWindow).ToggleSideDock(this as IDockControl);
        }
    }
}
