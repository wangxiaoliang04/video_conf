using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using VenueRtcCLI;

namespace VenueRtcWpf.IM
{
    class IMChatTcWrapper : IMChatWrapper
    {
        public const int MessagePerPage = 20;

        private ObservableCollection<MessageItem> itemList;
        public event OnMessageReceiveEvent onMessageReceive;

        private SemaphoreSlim historySignal = new SemaphoreSlim(0, 1);
        int getTextMessageCount = 0;

        ImageSource anchorTag, assistTag;
        System.Windows.Media.Brush selfFore, adimFore, normalFore, joinBack, anchorFore, assistantFore;
        string userid, roomid;

        public IMChatTcWrapper(ObservableCollection<MessageItem> itemList)
        {
            this.itemList = itemList;

            anchorTag = new BitmapImage(new Uri("/VenueRtc;component/image/tag_anchor.png", UriKind.Relative));
            assistTag = new BitmapImage(new Uri("/VenueRtc;component/image/tag_assist.png", UriKind.Relative));

            selfFore = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x54, 0x01));
            anchorFore = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B23"));
            adimFore = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF716F"));
            assistantFore = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#469EFF"));
            normalFore = new SolidColorBrush(Color.FromArgb(0xFF, 0x80, 0x80, 0x80));
            joinBack = new SolidColorBrush(Color.FromArgb(0xFF, 0xEA, 0xF6, 0xFF));
        }
       
        public async Task InitChatSDK()
        {
            string nickname = string.Empty;
            Task.Run(() => {
                try
                {
                    //var userid = SettingManager.ReadUserUUID();//VenueRtcCLI.VenueRTC.Instance.getUserId();
                    //var roomid = /*"14859";//*/ VenueRtcCLI.VenueRTC.Instance.getRoomId();
                    //nickname = VenueRtcCLI.VenueRTC.Instance.getNickName();
                    //(App.Current as App)?.Dispatcher?.Invoke(() => { this.userid = userid;this.roomid = roomid;  });
                    //VenueIM.Instance.InitSDK();
                    //VenueIM.Instance.Login(userid, nickname);
                }
                catch (Exception ex)
                {
                    (App.Current as App)?.Dispatcher?.Invoke(() => App.LogError(ex));
                }
            });
            
            //VenueIM.Instance.onIMLogin += new OnIMLoginEvent(delegate (int errorCode)
            //{
            //    if (errorCode == 0)
            //    {
            //        Task.Run(() =>
            //        {
            //            try
            //            {
            //                VenueIM.Instance.JoinRoom(roomid);
            //            }
            //            catch (Exception ex)
            //            {
            //                (App.Current as App)?.Dispatcher?.Invoke(() => App.LogError(ex));
            //            }
            //        });
            //    }
            //});

            //VenueIM.Instance.onIMJoinRoom += new OnIMJoinRoomEvent(async delegate (bool success)
            //{
            //    (App.Current as App)?.Dispatcher?.Invoke(async() =>
            //    {
            //        if (success)
            //        {
            //            try
            //            {
            //                await getHistoryMessage();
            //                if (itemList.Count > 0)
            //                    onMessageReceive(itemList[itemList.Count - 1], true);

            //                SendJoinMessage();
            //            }
            //            catch (Exception ex)
            //            {
            //                App.LogError(ex);
            //            }
            //        }
            //    });
            //});

            //VenueIM.Instance.onReceiveMessage += new OnReceiveMessageEvent(delegate (string message)
            //{
            //    try
            //    {
            //        (App.Current as App).Dispatcher.InvokeAsync(() =>
            //        {
            //            MessageItem newItem = fromIMTextMessage(message);
            //            if (newItem.type == 2 && newItem.message_is_from_self)
            //                return;
            //            if (newItem != null)
            //            {
            //                /*type 为2的不显示*/
            //                if(newItem.type != 2) itemList.Add(newItem);
            //                onMessageReceive(newItem, false);
            //            }
            //        });
            //    }
            //    catch (Exception ex)
            //    {
            //        App.LogError(ex);
            //    }
            //});

            //VenueIM.Instance.onGetRoomMessages += new OnGetRoomMessagesEvent(delegate (List<string> msgList) {
            //    try
            //    {
            //        foreach (string textMessage in ((IEnumerable<string>)msgList).Reverse())
            //        {
            //            MessageItem newItem = fromIMTextMessage(textMessage);
            //            if (newItem != null)
            //                itemList.Insert(0, newItem);
            //        }

            //        getTextMessageCount = msgList.Count;
            //        historySignal.Release();
            //    }
            //    catch (Exception ex)
            //    {
            //        App.LogError(ex);
            //    }
            //});
        }

        public async Task<int> getHistoryMessage()
        {
            getTextMessageCount = 0;
            try
            {

                //VenueIM.Instance.GetRoomMessages(MessagePerPage);

                await historySignal.WaitAsync(10000); //10秒超时
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            return getTextMessageCount;
        }

        private void setItemRoleInfo(MessageItem newItem)
        {
            if (newItem == null)
                return;

            if (newItem.type == 2)
            {
                newItem.nameForground = normalFore;
                newItem.itemBackground = joinBack;
                newItem.roleTagVisible = Visibility.Collapsed;
                return;
            }
            else if (newItem.role == "anchor")
            {
                newItem.roleTagImage = anchorTag;
                newItem.nameForground = anchorFore;
            }
            else if (newItem.role == "assistant")
            {
                newItem.roleTagImage = assistTag;
                newItem.nameForground = assistantFore;
            }
            else if (newItem.role == "admin")
            {
                newItem.roleTagImage = assistTag;
                newItem.nameForground = adimFore;
            }
            else
            {
                newItem.roleTagVisible = Visibility.Collapsed;
                newItem.nameForground = normalFore;
            }

            //if(newItem.message_is_from_self)
            //    newItem.nameForground = selfFore;
        }

        private MessageItem fromIMTextMessage(string textMessage)
        {
            MessageItem newItem = null;
            //try
            //{
                newItem = JsonConvert.DeserializeObject<MessageItem>(textMessage);
                if (newItem.type != 2)
                    newItem.nickName += ":";
                setItemRoleInfo(newItem);
            //}
            //catch(Exception ex)
            //{
            //    App.LogError(ex);
            //}
            
            return newItem;
        }

        public MessageItem SendMessage(string text)
        {
            string nickname = string.Empty;
            string role = string.Empty;
            try
            {
                //nickname = VenueRtcCLI.VenueRTC.Instance.getNickName();
                //role = VenueRtcCLI.VenueRTC.Instance.getRole();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            MessageItem newItem = new MessageItem();
            newItem.type = 1;
            newItem.avatar = "";
            newItem.nickName = /*"123456";//*/nickname;
            newItem.content = text;
            newItem.role = /*"anchor";//*/role;
            newItem.message_is_from_self = true;

            setItemRoleInfo(newItem);

            string textContent = JsonConvert.SerializeObject(newItem);
            try
            {
                //VenueIM.Instance.SendTextMessage(textContent, false);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            newItem.nickName += ":";
            (App.Current as App)?.Dispatcher?.Invoke(() =>  itemList.Add(newItem));
            return newItem;
        }

        public void SendJoinMessage()
        {
            MessageItem newItem = new MessageItem();

            Task.Factory.StartNew(() =>
            {
                string key = nameof(SendJoinMessage);
                    App app = App.Current as App;
                    app.KTokenSource[key] = new CancellationTokenSource();
                    app.KTokenSourceCounter[key] = 0;
                try
                {
                    

                    Task.Run(() =>
                    {
                        App.StartWaiting(key);
                        if (!(App.Current as App).KTokenSource[key].IsCancellationRequested)
                        {
                            (App.Current as App).KTokenSource[key].Cancel();
                            (App.Current as App).KTokenSource.Remove(key);
                            (App.Current as App).Dispatcher.Invoke(() =>
                            {
                                App.LogTimeOut(null, key);
                            });
                            return;
                        }
                    });

                    //string nickname = VenueRtcCLI.VenueRTC.Instance.getNickName();
                    //string role = VenueRtcCLI.VenueRTC.Instance.getRole();
                    //newItem.type = 2;
                    //newItem.avatar = "";
                    //newItem.nickName = /*"123456";//*/nickname;
                    //newItem.content = "光临直播间";
                    //newItem.role = /*"anchor";//*/role;
                    //newItem.message_is_from_self = true;


                    
                    string textInput_Text = "";

                    setItemRoleInfo(newItem);

                    string textContent = JsonConvert.SerializeObject(newItem);
                    //if (!role.Equals("admin"))
                    {
                        //VenueIM.Instance.SendTextMessage(textContent, true);
                    }
                    (App.Current as App).KTokenSource[key].Cancel();
                }
                catch (Exception ex) { (App.Current as App).Dispatcher.InvokeAsync(() => App.LogError(ex,key)); }

                
            });

        }
    }
}
