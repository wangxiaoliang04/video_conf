using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using LeanCloud;
using LeanCloud.Realtime;
using System.Collections.ObjectModel;
using System.Threading;

namespace VenueRtcWpf.IM
{
    public class IMChatLeanWrapper : IMChatWrapper
    {
        public const int MessagePerPage = 10;

        ObservableCollection<MessageItem> itemList;
        AVIMClient vIMClient;
        AVIMConversation conversation;
        IAVIMMessage oldestMessage = null;
        public event OnMessageReceiveEvent onMessageReceive;

        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        public IMChatLeanWrapper(ObservableCollection<MessageItem> itemList)
        {
            this.itemList = itemList;
        }

        public async Task InitChatSDK()
        {
            string userid = string.Empty;
            string roomid = string.Empty;
            try
            {
                userid = VenueRtcCLI.VenueRTC.Instance.getUserId();
                roomid = VenueRtcCLI.VenueRTC.Instance.getRoomId();
            }
            catch(Exception ex)
            {
                App.LogError(ex);
            }
            AVClient.Initialize("Q2BQPmQMCARUy2LY6pqc8Tk3-gzGzoHsz", "5jl51fyVTMUhHt6ddghrXNTa");
            AVRealtime realtime = new AVRealtime("Q2BQPmQMCARUy2LY6pqc8Tk3-gzGzoHsz", "5jl51fyVTMUhHt6ddghrXNTa");

            Websockets.Net.WebsocketConnection.Link();
            vIMClient = await realtime.CreateClientAsync(userid);

            AVIMConversationQuery query = vIMClient.GetChatRoomQuery().WhereEqualTo("name", roomid);
            List<AVIMConversation> conversations = (List<AVIMConversation>)(await query.FindAsync());

            if (conversations == null || conversations.Count == 0)
                conversation = await vIMClient.CreateChatRoomAsync(roomid);
            else
                conversation = conversations[0];

            await vIMClient.JoinAsync(conversation);

            await getHistoryMessage();

            vIMClient.OnMessageReceived += VIMClient_OnMessageReceived;
        }

        public async Task<int> getHistoryMessage()
        {
            IEnumerable<IAVIMMessage> messages;
            if (oldestMessage == null)
                messages = (await conversation.QueryMessageAsync(limit: MessagePerPage));
            else
                messages = (await conversation.QueryMessageAsync(
                    beforeMessageId: oldestMessage.Id,
                    beforeTimeStamp: oldestMessage.ServerTimestamp,
                    limit: MessagePerPage));

            foreach (var message in messages.Reverse())
            {
                if (message is AVIMTextMessage)
                {
                    var textMessage = (AVIMTextMessage)message;
                    MessageItem newItem = fromIMTextMessage(textMessage);
                    itemList.Insert(0, newItem);
                }
            }

            List<IAVIMMessage> list = messages.ToList();
            if (list.Count > 0)
                oldestMessage = list[0];

            return list.Count;
        }

        private void VIMClient_OnMessageReceived(object sender, AVIMMessageEventArgs e)
        {
            if (e.Message is AVIMTextMessage)
            {
                if (_syncContext != SynchronizationContext.Current)
                {
                    _syncContext.Post(o =>
                    {
                        var textMessage = (AVIMTextMessage)e.Message;
                        MessageItem newItem = fromIMTextMessage(textMessage);
                        itemList.Add(newItem);
                        onMessageReceive(newItem, false);

                    }, null);
                    return;
                }
            }
        }

        private MessageItem fromIMTextMessage(AVIMTextMessage textMessage)
        {
            MessageItem newItem = JsonConvert.DeserializeObject<MessageItem>(textMessage.TextContent);
            
            return newItem;
        }

        public MessageItem SendMessage(string text)
        {
            string nickname = string.Empty;
            string role = string.Empty;
            try
            {
                nickname = VenueRtcCLI.VenueRTC.Instance.getNickName();
                role = VenueRtcCLI.VenueRTC.Instance.getRole();
            }
            catch(Exception ex)
            {
                App.LogError(ex);
            }
            

            MessageItem newItem = new MessageItem();
            newItem.type = 1;
            newItem.avatar = "";
            newItem.nickName = nickname;
            newItem.content = text;
            newItem.role = role;

            itemList.Add(newItem);

            string textContent = JsonConvert.SerializeObject(newItem);

            var textMessage = new AVIMTextMessage(textContent);
            conversation.SendMessageAsync(textMessage);

            return newItem;
        }
    }
}
