using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
//using VenueRtcCLI;
using VenueRtcWpf.Command;
using VenueRtcWpf.Resources.Controls.CButton;

namespace VenueRtcWpf
{
    /// <summary>
    /// SideUserList.xaml 的交互逻辑
    /// </summary>
    public partial class SideUserList : UserControl, IDockControl
    {

        public string ClassName { get => this.GetType().Name; }
        public bool IsDocked { get => (bool)GetValue(DockControl.IsDockedProperty); set => SetValue(DockControl.IsDockedProperty, value); }
        

        ObservableCollection<UserInfoItem> itemList = new ObservableCollection<UserInfoItem>();
        //List<VenueUserHubCLI> currentUserHubs = new List<VenueUserHubCLI>();
        bool changingFromCode = false;
        bool isAssistant = true;

        SolidColorBrush[] colorBrushs = new SolidColorBrush[]{
            new SolidColorBrush(Color.FromRgb(0x39, 0x39, 0x4F)), 
            new SolidColorBrush(Color.FromRgb(0xFF, 0x8E, 0x57)), 
            new SolidColorBrush(Color.FromRgb(0x40, 0x9B, 0xFF)),
            new SolidColorBrush(Color.FromRgb(0xFF, 0x5D, 0x71))
        };
        SolidColorBrush brush_63a0ff = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#63a0ff"));


        public SideUserList()
        {
            InitializeComponent();

            userList.ItemsSource = itemList;
            IsDocked = true;

            this.CommandBindings.Add(new CommandBinding(MainWindowCommand.ToggleAllVoice, (ss, ee) => {
                
                var sb = ee.OriginalSource as StateButton;
                if (sb.CurrentState.Equals("全体静音"))
                {
                    try
                    {
                        //VenueRTC.Instance.setAllVoiceEnable(false);
                    }
                    catch (Exception ex)
                    {
                        App.LogError(ex);
                    }
                    sb.CurrentState = "取消全体静音";
                }
                else
                {
                    try { 
                    //VenueRTC.Instance.setAllVoiceEnable(true);
                    }
                    catch (Exception ex)
                    {
                        App.LogError(ex);
                    }
                    sb.CurrentState = "全体静音";
                }
            }));
            this.CommandBindings.Add(new CommandBinding(MainWindowCommand.ToggleVoice, (ss, ee) => {

                var sb = ee.OriginalSource as StateButton;
                if (sb.CurrentState.Equals("静音"))
                {
                    //VenueUserHubCLI userHub = getExistHub(sb.DataContext as UserInfoItem);
                    //if (userHub != null)
                    //{
                    //    try { 
                    //        VenueRTC.Instance.setVoiceEnable(userHub, false);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        App.LogError(ex);
                    //    }
                    //}
                    //sb.CurrentState = "解除静音";
                }
                else
                {
                    //VenueUserHubCLI userHub = getExistHub(sb.DataContext as UserInfoItem);
                    //if (userHub != null)
                    //{
                    //    try { 
                    //        VenueRTC.Instance.setVoiceEnable(userHub, true);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        App.LogError(ex);
                    //    }
                    //}
                    //sb.CurrentState = "静音";
                }
            }));
            this.CommandBindings.Add(new CommandBinding(MainWindowCommand.ToggleCam, (ss, ee) => {

                var sb = ee.OriginalSource as StateButton;

                if (sb.CurrentState.Equals("禁用视频"))
                {
                    try
                    {
                        //VenueUserHubCLI userHub = getExistHub(sb.DataContext as UserInfoItem);
                        //VenueRTC.Instance.setVideoEnable(userHub, false);
                    }
                    catch (Exception ex)
                    {
                        App.LogError(ex);
                    }
                    //sb.CurrentState = "开启视频";
                }
                else
                {
                    try
                    {
                        //VenueUserHubCLI userHub = getExistHub(sb.DataContext as UserInfoItem);
                        //VenueRTC.Instance.setVideoEnable(userHub, true);
                    }
                    catch (Exception ex)
                    {
                        App.LogError(ex);
                    }
                    //sb.CurrentState = "禁用视频";
                }
            }));
        }

        private void MicBtn_CheckChange(object sender, RoutedEventArgs e)
        {
            if (changingFromCode)
                return;

            ImageToggleButton button = sender as ImageToggleButton;
            UserInfoItem infoItem = button.DataContext as UserInfoItem;

            if(infoItem != null)
            {
                //VenueUserHubCLI userHub = getExistHub(infoItem);
                //if(userHub != null)
                //{
                //    try
                //    {
                //        VenueRTC.Instance.setVoiceEnable(userHub, !button.IsChecked.Value);
                //    }
                //    catch (Exception ex)
                //    {
                //        App.LogError(ex);
                //    }
                //}
            }
        }

        private void CameraBtn_CheckChange(object sender, RoutedEventArgs e)
        {
            if (changingFromCode)
                return;

            ImageToggleButton button = sender as ImageToggleButton;
            UserInfoItem infoItem = button.DataContext as UserInfoItem;

            if (infoItem != null)
            {
                //VenueUserHubCLI userHub = getExistHub(infoItem);
                //if (userHub != null)
                //{
                //    try
                //    {
                //        VenueRTC.Instance.setVideoEnable(userHub, !button.IsChecked.Value);
                //    }
                //    catch (Exception ex)
                //    {
                //        App.LogError(ex);
                //    }
                //}
            }
        }

        private void AllMuteBtn_Click(object sender, RoutedEventArgs e)
        {
            //foreach (VenueUserHubCLI userHub in currentUserHubs)
            //{
            //    VenueRTC.Instance.setVoiceEnable(userHub, false);
            //}
            try
            {
                //if (muteBtn.IsChecked.Value)
                //    VenueRTC.Instance.setAllVoiceEnable(false);
                //else
                //    VenueRTC.Instance.setAllVoiceEnable(true);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }

        //public void UpdateUserInfoList(List<VenueUserHubCLI> venueUserHubs)
        //{
        //    isAssistant = (VenueRTC.Instance.getRole() == "assistant");

        //    if (isAssistant)
        //    {
        //        muteBtn.Visibility = Visibility.Collapsed;
        //        btn_toggle_voice.Visibility = Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        btn_toggle_voice.Visibility = Visibility.Visible;
        //    }

        //    currentUserHubs = venueUserHubs;
        //    foreach (UserInfoItem infoItem in itemList.ToList())
        //    {
        //        if (getExistHub(infoItem) == null)
        //            itemList.Remove(infoItem);
        //    }
        //    var vm = DataContext as MainWindowViewModel;
        //    foreach (VenueUserHubCLI userHub in venueUserHubs)
        //    {
        //        if (userHub.isScreenShare)
        //            continue;
        //        changingFromCode = true;

        //        UserInfoItem newItem = getExistInfo(userHub);
        //        if (newItem == null)
        //        {
        //            newItem = createNewInfoItem(userHub);
        //            itemList.Add(newItem);
        //        }
        //        else
        //        {
        //            newItem.audioDisabled = !userHub.audioEnable;
        //            newItem.videoDisabled = !userHub.videoEnable;
                    
        //        }
        //        newItem.IsUserSelf = newItem.SessionId == vm.SessionId;
        //        changingFromCode = false;
        //    }
        //}
        
        //private VenueUserHubCLI getExistHub(UserInfoItem infoItem)
        //{
        //    foreach (VenueUserHubCLI userHub in currentUserHubs)
        //        if (userHub.sessionId == infoItem.SessionId && userHub.mediaType == infoItem.MediaType)
        //            return userHub;

        //    return null;
        //}

        //private UserInfoItem getExistInfo(VenueUserHubCLI userHub)
        //{
        //    foreach (UserInfoItem infoItem in itemList)
        //        if (userHub.sessionId == infoItem.SessionId && userHub.mediaType == infoItem.MediaType)
        //            return infoItem;

        //    return null;
        //}

        //private UserInfoItem createNewInfoItem(VenueUserHubCLI userHub)
        //{
        //    UserInfoItem newItem = new UserInfoItem();
        //    //if (userHub.isAnchor)
        //    //    newItem.NameColor = new SolidColorBrush(Color.FromArgb(0xff, 0, 0x7A, 0xff));
        //    //else
        //        newItem.NameColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#39394F")); ;
        //    newItem.SessionId = userHub.sessionId;
        //    newItem.MediaType = userHub.mediaType;
        //    newItem.Nickname = userHub.nickname;
        //    newItem.Avatar = createDefaultAvatar(newItem.Nickname);
        //    newItem.audioDisabled = !userHub.audioEnable;
        //    newItem.videoDisabled = !userHub.videoEnable;
        //    newItem.AnchorEnable = !isAssistant;
        //    newItem.IsAnchor = userHub.isAnchor;
        //    //newItem.IsUserSelf = userHub.sessionId
        //    return newItem;
        //}

        int color_index = 0;
        private ImageSource createDefaultAvatar(string nickname)
        {
            DrawingGroup drawing = new DrawingGroup();
            GeometryDrawing ellipseDrawing = new GeometryDrawing(
                   brush_63a0ff,//colorBrushs[color_index],
                    null,
                    new EllipseGeometry(new Point(15, 15), 15, 15));
            drawing.Children.Add(ellipseDrawing);

            try
            {
                Typeface typeface = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                GlyphTypeface glyphTypeface;
                if (!typeface.TryGetGlyphTypeface(out glyphTypeface))
                    throw new InvalidOperationException("No glyphtypeface found");
                ushort glyphIndex = glyphTypeface.CharacterToGlyphMap[GetCodeChar(nickname)];
                ushort[] glyphIndexes = new ushort[] { glyphIndex };
                double width = glyphTypeface.AdvanceWidths[glyphIndex] * 18;
                double[] advanceWidths = new double[] { width };
                Point origin = new Point(15 - width / 2, 21);
                GlyphRun glyphRun = new GlyphRun(glyphTypeface, 0, false, 18,
                    glyphIndexes, origin, advanceWidths, null, null, null, null,
                    null, null);
                GlyphRunDrawing geometryDrawing = new GlyphRunDrawing(Brushes.White, glyphRun);
                drawing.Children.Add(geometryDrawing);
            }
            catch
            {
            }


            DrawingImage drawingImageSource = new DrawingImage(drawing);
            
            color_index = (color_index + 1) % colorBrushs.Length;

            return drawingImageSource;
        }

        /// <summary>
        /// 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母
        /// </summary>
        /// <param name='CnChar'>单个汉字</param>
        /// <returns>单个大写字母</returns>
        internal static char GetCodeChar(string CnChar)
        {
            #region GetCodeChar
            long iCnChar;
            byte[] ZW = System.Text.Encoding.Default.GetBytes(CnChar);
            //如果是字母，则直接返回
            if (ZW[0] <= 127)
            {
                return CnChar.ToUpper().ElementAt<char>(0);
            }
            else
            {
                // get the array of byte from the single char
                int i1 = (short)(ZW[0]);
                int i2 = (short)(ZW[1]);
                iCnChar = i1 * 256 + i2;
            }
            // iCnChar match the constant
            if ((iCnChar >= 45217) && (iCnChar <= 45252))
            {
                return 'A';
            }
            else if ((iCnChar >= 45253) && (iCnChar <= 45760))
            {
                return 'B';
            }
            else if ((iCnChar >= 45761) && (iCnChar <= 46317))
            {
                return 'C';
            }
            else if ((iCnChar >= 46318) && (iCnChar <= 46825))
            {
                return 'D';
            }
            else if ((iCnChar >= 46826) && (iCnChar <= 47009))
            {
                return 'E';
            }
            else if ((iCnChar >= 47010) && (iCnChar <= 47296))
            {
                return 'F';
            }
            else if ((iCnChar >= 47297) && (iCnChar <= 47613))
            {
                return 'G';
            }
            else if ((iCnChar >= 47614) && (iCnChar <= 48118))
            {
                return 'H';
            }
            else if ((iCnChar >= 48119) && (iCnChar <= 49061))
            {
                return 'J';
            }
            else if ((iCnChar >= 49062) && (iCnChar <= 49323))
            {
                return 'K';
            }
            else if ((iCnChar >= 49324) && (iCnChar <= 49895))
            {
                return 'L';
            }
            else if ((iCnChar >= 49896) && (iCnChar <= 50370))
            {
                return 'M';
            }
            else if ((iCnChar >= 50371) && (iCnChar <= 50613))
            {
                return 'N';
            }
            else if ((iCnChar >= 50614) && (iCnChar <= 50621))
            {
                return 'O';
            }
            else if ((iCnChar >= 50622) && (iCnChar <= 50905))
            {
                return 'P';
            }
            else if ((iCnChar >= 50906) && (iCnChar <= 51386))
            {
                return 'Q';
            }
            else if ((iCnChar >= 51387) && (iCnChar <= 51445))
            {
                return 'R';
            }
            else if ((iCnChar >= 51446) && (iCnChar <= 52217))
            {
                return 'S';
            }
            else if ((iCnChar >= 52218) && (iCnChar <= 52697))
            {
                return 'T';
            }
            else if ((iCnChar >= 52698) && (iCnChar <= 52979))
            {
                return 'W';
            }
            else if ((iCnChar >= 52980) && (iCnChar <= 53640))
            {
                return 'X';
            }
            else if ((iCnChar >= 53689) && (iCnChar <= 54480))
            {
                return 'Y';
            }
            else if ((iCnChar >= 54481) && (iCnChar <= 55289))
            {
                return 'Z';
            }
            else
                return ('?');
            #endregion
        }

        #region UserInfoItem
        public class UserInfoItem : INotifyPropertyChanged
        {
            public string SessionId { get; set; }
            public string MediaType { get; set; }

            public SolidColorBrush NameColor { get; set; }

            public string Nickname { get; set; }

            public ImageSource Avatar { get; set; }

            public bool AnchorEnable { get; set; }

            private bool isAnchor;

            private bool stateVoice;

            private bool stateAudio;
            
            bool _audioDisabled;
            public bool audioDisabled { get { return _audioDisabled; }
                set
                {
                    if (_audioDisabled != value)
                    {
                        _audioDisabled = value; OnPropertyChanged("AudioDisabled");
                        OnPropertyChanged(nameof(StateVoice));
                    }
                } }

            bool _videoDisabled;
            public bool videoDisabled { get { return _videoDisabled; }
                set
                {
                    if (_videoDisabled != value)
                    {
                        _videoDisabled = value; OnPropertyChanged("VideoDisabled");
                        OnPropertyChanged(nameof(StateVedio));
                    }
                } }

            public string StateVoice { get => _audioDisabled?  "解除静音" : "静音"; }
            public string StateVedio { get => _videoDisabled ? "开启视频" : "禁用视频"; }
            public bool IsAnchor
            {
                get { return isAnchor; }
                set
                {
                    isAnchor = value;
                    OnPropertyChanged(nameof(IsAnchor));
                }
            }
            private bool isUserSelf ;
                public bool IsUserSelf
            {
                get { return isUserSelf; }
                set
                {
                    isUserSelf = value;
                    OnPropertyChanged(nameof(IsUserSelf));
                }
            }
            protected internal virtual void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            public event PropertyChangedEventHandler PropertyChanged;
        }
        #endregion

        private void StateButton_Loaded(object sender, RoutedEventArgs e)
        {
            //var sb = sender as StateButton;
            //string cs = (string)sb.GetValue(StateButton.CurrentStateProperty);
            //var p = sb.StateControl.Parameters.FirstOrDefault(x => x.TheState == cs);
            //sb.SetValue(StateButton.DefaultParameterProperty, p);
            //sb.SetValue(StateButton.CommandProperty, p.TheCommand);
            //sb.SetValue(StateButton.ToolTipProperty, cs);
            //sb.SetValue(StateButton.CurrentStateProperty, "unknow");
            //sb.SetValue(StateButton.CurrentStateProperty, cs);
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
