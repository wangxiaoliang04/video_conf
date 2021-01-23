using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VenueRtcWpf.IM
{
    public delegate void OnMessageReceiveEvent(MessageItem newItem, bool forceScroll);

    public interface IMChatWrapper
    {
        event OnMessageReceiveEvent onMessageReceive;
        Task InitChatSDK();

        Task<int> getHistoryMessage();

        MessageItem SendMessage(string text);
    }

    public class MessageItem
    {
        //{"type": "text", "avatar": "", "nickName":"黑8", "content":"那里", "role":"anchor"}
        public int? type { get; set; }
        public string avatar { get; set; }
        public string content { get; set; }
        public string nickName { get; set; }
        public string role { get; set; }
        public bool message_is_from_self { get; set; }
        
        [JsonIgnore]
        public System.Windows.Media.Brush nameForground { get; set; }

        [JsonIgnore]
        public System.Windows.Media.Brush itemBackground { get; set; }

        [JsonIgnore]
        public ImageSource roleTagImage { get; set; }

        [JsonIgnore]
        public Visibility roleTagVisible { get; set; }
    }
}
