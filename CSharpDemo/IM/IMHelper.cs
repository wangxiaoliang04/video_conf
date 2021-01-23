using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VenueRtcWpf.IM
{
    public partial class IMHelper
    {
        
        #region Keys
        public const string code = "code";
        public const string data = "data";
        public const string token = "token";
        public const string message = "message";
        public const string success = "success";
        public const string username = "username";
        public const string password = "password";
        public const string status = "status";
        public const string error = "error";
        #endregion

        public const int SDKAPPID = 1400250557;
        public const int EXPIRETIME = 604800;
        public const string SECRETKEY = "e018d9ca89860b231e61f0944832d0d71658ab929db4295e8b9a52dfccb20d46";

        

        /// <summary>
        /// IM SDK 初始化
        /// </summary>
        /// <param name="sdk_app_id"></param>
        /// <param name="json_sdk_config"></param>
        /// <returns></returns>
        [DllImport(@"imsdk.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int TIMInit(long sdk_app_id, string json_sdk_config);
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="user_sig"></param>
        /// <param name="cb"></param>
        /// <returns></returns>
        [DllImport(@"imsdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int TIMLogin(string user_id, string user_sig, TIMCommCallback cb);

        /// <summary>
        /// 接收新消息回调
        /// </summary>
        /// <param name="cb"></param>
        [DllImport(@"imsdk.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void TIMAddRecvNewMsgCallback(TIMRecvNewMsgCallback cb);
        /// <summary>
        /// 获取好友添加请求未决信息列表
        /// </summary>
        /// <param name="json_get_pendency_list_param">获取未决列表接口参数的 JSON 字符串</param>
        /// <param name="cb"></param>
        /// <returns></returns>
        [DllImport(@"imsdk.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int TIMFriendshipGetPendencyList(string json_get_pendency_list_param, TIMCommCallback cb);
        /// <summary>
        /// 获取好友列表
        /// </summary>
        [DllImport(@"imsdk.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int TIMFriendshipGetFriendProfileList(TIMCommCallback cb);

    }
    //public partial class IMHelper
    //{
    //    public IMHelper()
    //    {
            
    //    }
    //    static IMHelper instance;
    //    public IMHelper Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //            {
    //                instance = new IMHelper();
    //            }
    //            return instance;
    //        }
    //    }
        
    //}
}
