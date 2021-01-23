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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VenueRtcCLI;

namespace VenueRtcWpf
{
    /// <summary>
    /// SidePanel.xaml 的交互逻辑
    /// </summary>
    public partial class SidePanel : UserControl
    {
        SideUserList userList = new SideUserList();
        SideChatView chatView = new SideChatView();
        SidePopWindow chatPopWindow, userPopWindow;
        bool isShown = false;

        public SidePanel()
        {
            InitializeComponent();

            userContent.Children.Add(userList);
            chatContent.Children.Add(chatView);
            //mainGrid.RowDefinitions[2].Height = new GridLength(0);
            //mainGrid.RowDefinitions[3].Height = new GridLength(0);
        }

        public bool IsShown { get { return isShown; } }

        public void InitChat()
        {
            if (chatView != null)
                chatView.InitChatSDK();
        }

        public void UpdateUserInfoList(List<VenueUserHubCLI> venueUserHubs)
        {
            if (userList != null)
                userList.UpdateUserInfoList(venueUserHubs);

            int userCount = 0;

            foreach (VenueUserHubCLI userHub in venueUserHubs)
            {
                if (userHub.isScreenShare)
                    continue;
                userCount++;
            }

            userTitle.Text = "参会者(" + userCount + ")";
        }

        public void SwitchShow(bool show = true, bool forceShowUsers = false, bool forceShowChat = false)
        {
            if(forceShowUsers && usersHideToggle.IsChecked.Value)
            {
                usersHideToggle.IsChecked = false;
                if (isShown)
                    return;
            }
            if (forceShowChat && chatHideToggle.IsChecked.Value)
            {
                chatHideToggle.IsChecked = false;
                if (isShown)
                    return;
            }

            DoubleAnimation anim;
            if (show && !isShown)
            {
                anim = new DoubleAnimation(-this.ActualWidth, 0, new Duration(TimeSpan.FromSeconds(0.3f)));
                anim.DecelerationRatio = 0.8;
                isShown = true;
            }
            else
            {
                anim = new DoubleAnimation(0, -this.ActualWidth, new Duration(TimeSpan.FromSeconds(0.3f)));
                anim.AccelerationRatio = 0.8;
                isShown = false;
            }
            this.BeginAnimation(Canvas.RightProperty, anim);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            SwitchShow(false);
        }

        private void HideUserList_Click(object sender, RoutedEventArgs e)
        {
            ImageToggleButton toggleButton = sender as ImageToggleButton;

            if (toggleButton.IsChecked.Value)
                mainGrid.RowDefinitions[1].Height = new GridLength(0);
            else
                mainGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);

        }

        private void HideChatList_Click(object sender, RoutedEventArgs e)
        {
            ImageToggleButton toggleButton = sender as ImageToggleButton;

            if (toggleButton.IsChecked.Value)
                mainGrid.RowDefinitions[3].Height = new GridLength(0);
            else
                mainGrid.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);
        }

        public void SwitchUserWindow()
        {
            if (userPopWindow == null)
            {
                userPopWindow = new SidePopWindow("参会者");
                userPopWindow.Closing += UserPopWindow_Closing;
                
                userPopWindow.Show();

                userContent.Children.Remove(userList);
                userPopWindow.layoutContent.Children.Add(userList);

                mainGrid.RowDefinitions[0].Height = new GridLength(0);
                mainGrid.RowDefinitions[1].Height = new GridLength(0);

                if (chatPopWindow != null)
                    chatPopWindow.Close();
            }
            else
            {
                userPopWindow.Close();
            }
        }

        private void UserWin_Click(object sender, RoutedEventArgs e)
        {
            if (userPopWindow == null)
                SwitchUserWindow();
        }

        private void UserPopWindow_Closing(object sender, EventArgs e)
        {
            userPopWindow.layoutContent.Children.Remove(userList);
            userContent.Children.Add(userList);

            userPopWindow = null;
            mainGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);
            if (usersHideToggle.IsChecked.Value)
                mainGrid.RowDefinitions[1].Height = new GridLength(0);
            else
                mainGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
        }

        public void SwitchChatWindow()
        {
            if (chatPopWindow == null)
            {
                chatPopWindow = new SidePopWindow("聊天");
                chatPopWindow.Closing += ChatPopWindow_Closing;

                //chatPopWindow.Owner = Window.GetWindow(this);
                chatPopWindow.Show();

                chatContent.Children.Remove(chatView);
                chatPopWindow.layoutContent.Children.Add(chatView);

                mainGrid.RowDefinitions[2].Height = new GridLength(0);
                mainGrid.RowDefinitions[3].Height = new GridLength(0);

                if (userPopWindow != null)
                    userPopWindow.Close();
            }
            else
            {
                chatPopWindow.Close();
            }
        }

        private void ChatWin_Click(object sender, RoutedEventArgs e)
        {
            if (chatPopWindow == null)
                SwitchChatWindow();
        }

        private void ChatPopWindow_Closing(object sender, EventArgs e)
        {
            chatPopWindow.layoutContent.Children.Remove(chatView);
            chatContent.Children.Add(chatView);

            chatPopWindow = null;
            mainGrid.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Auto);
            if (chatHideToggle.IsChecked.Value)
                mainGrid.RowDefinitions[3].Height = new GridLength(0);
            else
                mainGrid.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);
        }
    }
}
