using System;
using System.Runtime.InteropServices;
using ManageLiteAV;
using Newtonsoft.Json;
using TRTCCSharpDemo.Common;

namespace TRTCCSharpDemo
{
    /// <summary>
    /// SDK 的 Local 数据信息，使用 ini 本地存储
    /// </summary>
    class DataManager : IDisposable
    {
        public const string INI_ROOT_KEY                  = "TRTCDemo";
        // 用户
        public const string INI_KEY_USER_ID               = "INI_KEY_USER_ID";
        public const string INI_KEY_ROOM_ID               = "INI_KEY_ROOM_ID";
        public const string INI_KEY_ROLE_TYPE             = "INI_KEY_ROLE_TYPE";
        // 设备
        public const string INI_KEY_CHOOSE_CAMERA         = "INI_KEY_CHOOSE_CAMERA";
        public const string INI_KEY_CHOOSE_SPEAK          = "INI_KEY_CHOOSE_SPEAK";
        public const string INI_KEY_CHOOSE_MIC            = "INI_KEY_CHOOSE_MIC";
        // 视频
        public const string INI_KEY_VIDEO_BITRATE         = "INI_KEY_VIDEO_BITRATE";
        public const string INI_KEY_VIDEO_RESOLUTION      = "INI_KEY_VIDEO_RESOLUTION";
        public const string INI_KEY_VIDEO_RES_MODE        = "INI_KEY_VIDEO_RES_MODE";
        public const string INI_KEY_VIDEO_FPS             = "INI_KEY_VIDEO_FPS";
        public const string INI_KEY_VIDEO_QUALITY         = "INI_KEY_VIDEO_QUALITY";
        public const string INI_KEY_VIDEO_QUALITY_CONTROL = "INI_KEY_VIDEO_QUALITY_CONTROL";
        public const string INI_KEY_VIDEO_APP_SCENE       = "INI_KEY_VIDEO_APP_SCENE";
        public const string INI_KEY_VIDEO_FILL_MODE       = "INI_KEY_VIDEO_FILL_MODE";
        public const string INI_KEY_VIDEO_ROTATION        = "INI_KEY_VIDEO_ROTATION";
        // 音频
        public const string INI_KEY_AUDIO_MIC_VOLUME      = "INI_KEY_AUDIO_MIC_VOLUME";
        public const string INI_KEY_AUDIO_SPEAKER_VOLUME  = "INI_KEY_AUDIO_SPEAKER_VOLUME";
        public const string INI_KEY_AUDIO_SAMPLERATE      = "INI_KEY_AUDIO_SAMPLERATE";
        public const string INI_KEY_AUDIO_CHANNEL         = "INI_KEY_AUDIO_CHANNEL";
        public const string INI_KEY_AUDIO_QUALITY         = "INI_KEY_AUDIO_QUALITY";
        // 美颜
        public const string INI_KEY_BEAUTY_OPEN           = "INI_KEY_BEAUTY_OPEN";
        public const string INI_KEY_BEAUTY_STYLE          = "INI_KEY_BEAUTY_STYLE";
        public const string INI_KEY_BEAUTY_VALUE          = "INI_KEY_BEAUTY_VALUE";
        public const string INI_KEY_WHITE_VALUE           = "INI_KEY_WHITE_VALUE";
        public const string INI_KEY_RUDDINESS_VALUE       = "INI_KEY_RUDDINESS_VALUE";
        // 大小流
        public const string INI_KEY_SET_PUSH_SMALLVIDEO   = "INI_KEY_SET_PUSH_SMALLVIDEO";
        public const string INI_KEY_SET_PLAY_SMALLVIDEO   = "INI_KEY_SET_PLAY_SMALLVIDEO";
        // 测试
        public const string INI_KEY_SET_NETENV_STYLE      = "INI_KEY_SET_NETENV_STYLE";
        public const string INI_KEY_ROOMCALL_STYLE        = "INI_KEY_ROOMCALL_STYLE";
        // 镜像
        public const string INI_KEY_LOCAL_VIDEO_MIRROR    = "INI_KEY_LOCAL_VIDEO_MIRROR";
        public const string INI_KEY_REMOTE_VIDEO_MIRROR   = "INI_KEY_REMOTE_VIDEO_MIRROR";
        // 音量提示
        public const string INI_KEY_SHOW_AUDIO_VOLUME     = "INI_KEY_SHOW_AUDIO_VOLUME";
        // 混流
        public const string INI_KEY_CLOUD_MIX_TRANSCODING = "INI_KEY_CLOUD_MIX_TRANSCODING";

        private IniStorage storage;

        public ITRTCCloud trtcCloud;

        /**
        * @brief 会话类型
        */
        enum TIMConvType
        {
            kTIMConv_Invalid, // 无效会话
            kTIMConv_C2C,     // 个人会话
            kTIMConv_Group,   // 群组会话
            kTIMConv_System,  // 系统会话
        };



        /// <summary>
        /// IM SDK 初始化。
        /// </summary>
        /// <param name="sdk_app_id"></param>
        /// <param name="json_sdk_config"></param>
        /// <returns></returns>
        [DllImport(@"imsdk.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int TIMInit(long sdk_app_id, string json_sdk_config);
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user_id">用户id</param>
        /// <param name="user_sig">签名</param>
        /// <param name="cb"></param>
        /// <returns></returns>
        [DllImport(@"imsdk.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int TIMLogin(string user_id, string user_sig, TIMCommCallback cb);


        /// <summary>
        /// 创建群组
        /// </summary>
        /// <param name="json_group_create_param">用户id</param>
        /// <param name="cb"></param>
        /// <returns></returns>
        [DllImport(@"imsdk.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int TIMGroupCreate(string json_group_create_param, TIMCommCallback cb);

        /// <summary>
        /// 加入群组
        /// </summary>
        /// <param name="json_group_create_param">用户id</param>
        /// <param name="cb"></param>
        /// <returns></returns>
        [DllImport(@"imsdk.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int TIMGroupJoin(string group_id, string hello_msg, TIMCommCallback cb);

        /// <summary>
        /// 接收新消息回调
        /// </summary>
        /// <param name="cb"></param>
        [DllImport(@"imsdk.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private extern static void TIMAddRecvNewMsgCallback(delegateTIMRecvNewMsgCallback cb);

        

        /// <summary>
        /// 接收新消息回调
        /// </summary>
        /// <param name="cb"></param>
        [DllImport(@"imsdk.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private extern static int TIMMsgSendNewMsg(string conv_id, TIMConvType conv_type, string json_msg_param, TIMCommCallback cb);

        private delegate void delegateTIMRecvNewMsgCallback(string json_msg_array, IntPtr user_data);
        private void TIMRecvNewMsgCallback(string json_msg_array, IntPtr user_data)
        {
            var b = System.Text.Encoding.Default.GetBytes(json_msg_array);

            var jsonParams = JsonConvert.DeserializeObject<dynamic>(System.Text.Encoding.UTF8.GetString(b));
            //获取dynamic里边的数据
            string destId = jsonParams.message_client_time;
            string token = jsonParams.message_conv_id;

            //var ci = JsonConvert.DeserializeObject(System.Text.Encoding.UTF8.GetString(b));
            //string s = ci["txtContent"].ToString();

            //richTextBox1.AppendText(System.Text.Encoding.UTF8.GetString(b));
        }

        delegateTIMRecvNewMsgCallback mycall;
        TIMCommCallback mycall1;

        TIMCommCallback mycall2;

        TIMCommCallback mycall11;

        TIMCommCallback mycall3;

        public void ImInit()
        {
            int sdk_app_id = 1400473960;
            var data = new { sdk_config_log_file_path = AppDomain.CurrentDomain.BaseDirectory, sdk_config_config_file_path = AppDomain.CurrentDomain.BaseDirectory };
            TIMInit(sdk_app_id, JsonConvert.SerializeObject(data));

            mycall = new delegateTIMRecvNewMsgCallback(TIMRecvNewMsgCallback);
            TIMAddRecvNewMsgCallback(mycall);

            mycall1 = new TIMCommCallback(CommCallback);

            var tLSSig = new tencentyun.TLSSigAPIv2(sdk_app_id, "11ddbddae4444052e35f30d2b7db54c5a9d04811a3436411d01da7e57db64b01");
            TIMLogin(DataManager.GetInstance().userId, tLSSig.GenSig(DataManager.GetInstance().userId), mycall1);

            //CreateGroup();
        }

        public void Send_Message()
        {
            var data = new[] { new { elem_type = 0, text_elem_content = "aaaaaaaaaa" } };

            //Json::Value json_value_text;
            //json_value_text[kTIMElemType] = kTIMElem_Text;
            //json_value_text[kTIMTextElemContent] = message;

            
            var data1 = new { message_elem_array = data, message_sender = DataManager.GetInstance().userId, message_conv_id = DataManager.GetInstance().roomId.ToString(), message_conv_type = TIMConvType.kTIMConv_Group };

            /*Json::Value json_value_msg;
            json_value_msg[kTIMMsgElemArray].append(json_value_text);
            json_value_msg[kTIMMsgSender] = userid;
            json_value_msg[kTIMMsgClientTime] = time(NULL);
            json_value_msg[kTIMMsgServerTime] = time(NULL);
            //json_value_msg[kTIMMsgIsOnlineMsg] = online_message;
            json_value_msg[kTIMMsgConvId] = roomid;
            json_value_msg[kTIMMsgConvType] = kTIMConv_Group;

            std::string json_msg = json_value_msg.toStyledString();*/

            mycall2 = new TIMCommCallback(CommCallback222);

            int ret = TIMMsgSendNewMsg(DataManager.GetInstance().roomId.ToString(), TIMConvType.kTIMConv_Group, JsonConvert.SerializeObject(data1), mycall2);
            return;
        }

        public void CreateGroup()
        {
            TIMAddRecvNewMsgCallback(TIMRecvNewMsgCallback);
            //GC.KeepAlive(TIMRecvNewMsgCallback);
            //var data11 = new { create_group_param_group_name = "111222333", create_group_param_group_id = "5555", create_group_param_group_type = 4, create_group_param_add_option = 2 };
            //mycall11 = new TIMCommCallback(CommCallback111);
            //int ret = TIMGroupCreate(JsonConvert.SerializeObject(data11), mycall11);

            mycall3 = new TIMCommCallback(CommCallback333);

            TIMGroupJoin(DataManager.GetInstance().roomId.ToString(), "Want Join Group, Thank you", mycall3);
        }

        public delegate void TIMCommCallback(int code, string desc, string json_params, IntPtr user_data);

        private void CommCallback111(int code, string desc, string json_params, IntPtr user_data)
        {
            return;
        }

        private void CommCallback333(int code, string desc, string json_params, IntPtr user_data)
        {
            if (code != 0 && code != 10013)
            { // 失败
                if (code == 10015 || code == 10010)
                {
                    //ths->CreateRoom(ths->roomid);
                    var data11 = new { create_group_param_group_name = DataManager.GetInstance().roomId.ToString(), create_group_param_group_id = DataManager.GetInstance().roomId.ToString(), create_group_param_group_type = 4, create_group_param_add_option = 2 };
                    mycall11 = new TIMCommCallback(CommCallback111);
                    TIMGroupCreate(JsonConvert.SerializeObject(data11), mycall11);
                }
                else
                {
                    //ths->roomid = "";
                    //_imListener->onIMJoinRoom(false);
                }
            }

            Console.WriteLine("----------------------------------Hello CommCallback333");
            return;
        }

        private void CommCallback222(int code, string desc, string json_params, IntPtr user_data)
        {
            Console.WriteLine("----------------------------------Hello World");
            return;
        }

        private void CommCallback(int code, string desc, string json_params, IntPtr user_data)
        {
            return;
        }

        private DataManager()
        {
            trtcCloud = ITRTCCloud.getTRTCShareInstance();

            storage = new IniStorage(".\\TRTCDemo.ini");

            videoEncParams = new TRTCVideoEncParam();
            qosParams = new TRTCNetworkQosParam();
        }

        #region Disposed
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (trtcCloud != null)
            {
                ITRTCCloud.destroyTRTCShareInstance();
                trtcCloud.Dispose();
                trtcCloud = null;
            }
            disposed = true;
        }

        ~DataManager()
        {
            Dispose(false);
        }
        #endregion

        public void InitConfig()
        {
            // 用户信息配置
            string userId = storage.GetValue(INI_ROOT_KEY, INI_KEY_USER_ID);
            if (string.IsNullOrEmpty(userId))
                this.userId = Util.GetRandomString(8);
            else
                this.userId = userId;
            string roomId = storage.GetValue(INI_ROOT_KEY, INI_KEY_ROOM_ID);
            if (string.IsNullOrEmpty(roomId))
                this.roomId = uint.Parse(Util.GetRandomString(3));
            else
                this.roomId = uint.Parse(roomId);
            string role = storage.GetValue(INI_ROOT_KEY, INI_KEY_ROLE_TYPE);
            if (string.IsNullOrEmpty(role))
                this.roleType = TRTCRoleType.TRTCRoleAnchor;
            else 
                this.roleType = (TRTCRoleType)(int.Parse(role));

            // 视频参数配置
            string param;
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_VIDEO_BITRATE);
            if (string.IsNullOrEmpty(param))
                this.videoEncParams.videoBitrate = 550;
            else
                this.videoEncParams.videoBitrate = uint.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_VIDEO_FPS);
            if (string.IsNullOrEmpty(param))
                this.videoEncParams.videoFps = 15;
            else
                this.videoEncParams.videoFps = uint.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_VIDEO_RESOLUTION);
            if (string.IsNullOrEmpty(param))
                this.videoEncParams.videoResolution = TRTCVideoResolution.TRTCVideoResolution_640_360;
            else
                this.videoEncParams.videoResolution = (TRTCVideoResolution)int.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_VIDEO_RES_MODE);
            if (string.IsNullOrEmpty(param))
                this.videoEncParams.resMode = TRTCVideoResolutionMode.TRTCVideoResolutionModeLandscape;
            else
                this.videoEncParams.resMode = (TRTCVideoResolutionMode)int.Parse(param);

            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_VIDEO_QUALITY);
            if (string.IsNullOrEmpty(param))
                this.qosParams.preference = TRTCVideoQosPreference.TRTCVideoQosPreferenceClear;
            else 
                this.qosParams.preference = (TRTCVideoQosPreference)int.Parse(param);

            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_VIDEO_QUALITY_CONTROL);
            if (string.IsNullOrEmpty(param))
                this.qosParams.controlMode = TRTCQosControlMode.TRTCQosControlModeServer;
            else 
                this.qosParams.controlMode = (TRTCQosControlMode)int.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_VIDEO_APP_SCENE);
            if (string.IsNullOrEmpty(param))
                this.appScene = TRTCAppScene.TRTCAppSceneVideoCall;
            else 
                this.appScene = (TRTCAppScene)int.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_SET_PUSH_SMALLVIDEO);
            if (string.IsNullOrEmpty(param))
                this.pushSmallVideo = false;
            else
                this.pushSmallVideo = bool.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_SET_PLAY_SMALLVIDEO);
            if (string.IsNullOrEmpty(param))
                this.playSmallVideo = false;
            else
                this.playSmallVideo = bool.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_BEAUTY_OPEN);
            if (string.IsNullOrEmpty(param))
                this.isOpenBeauty = false;
            else
                this.isOpenBeauty = bool.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_BEAUTY_STYLE);
            if (string.IsNullOrEmpty(param))
                this.beautyStyle = TRTCBeautyStyle.TRTCBeautyStyleSmooth;
            else 
                this.beautyStyle = (TRTCBeautyStyle)int.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_BEAUTY_VALUE);
            if (string.IsNullOrEmpty(param))
                this.beauty = 0;
            else
                this.beauty = uint.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_WHITE_VALUE);
            if (string.IsNullOrEmpty(param))
                this.white = 0;
            else
                this.white = uint.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_RUDDINESS_VALUE);
            if (string.IsNullOrEmpty(param))
                this.ruddiness = 0;
            else
                this.ruddiness = uint.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_LOCAL_VIDEO_MIRROR);
            if (string.IsNullOrEmpty(param))
                this.isLocalVideoMirror = false;
            else
                this.isLocalVideoMirror = bool.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_REMOTE_VIDEO_MIRROR);
            if (string.IsNullOrEmpty(param))
                this.isRemoteVideoMirror = false;
            else
                this.isRemoteVideoMirror = bool.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_VIDEO_FILL_MODE);
            if (string.IsNullOrEmpty(param))
                this.videoFillMode = TRTCVideoFillMode.TRTCVideoFillMode_Fit;
            else
                this.videoFillMode = (TRTCVideoFillMode)int.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_VIDEO_ROTATION);
            if (string.IsNullOrEmpty(param))
                this.videoRotation = TRTCVideoRotation.TRTCVideoRotation0;
            else
                this.videoRotation = (TRTCVideoRotation)int.Parse(param);

            // 音频参数配置
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_AUDIO_MIC_VOLUME);
            if (string.IsNullOrEmpty(param))
                this.micVolume = 25;
            else
                this.micVolume = uint.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_AUDIO_SPEAKER_VOLUME);
            if (string.IsNullOrEmpty(param))
                this.speakerVolume = 25;
            else
                this.speakerVolume = uint.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_SHOW_AUDIO_VOLUME);
            if (string.IsNullOrEmpty(param))
                this.isShowVolume = false;
            else
                this.isShowVolume = bool.Parse(param);


            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_AUDIO_QUALITY);
            if (string.IsNullOrEmpty(param))
                this.AudioQuality = TRTCAudioQuality.TRTCAudioQualityDefault;
            else
                this.AudioQuality = (TRTCAudioQuality)int.Parse(param);


            // 测试参数配置
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_SET_NETENV_STYLE);
            if (string.IsNullOrEmpty(param))
                this.testEnv = 0;
            else
                this.testEnv = int.Parse(param);
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_ROOMCALL_STYLE);
            if (string.IsNullOrEmpty(param))
                this.pureAudioStyle = false;
            else
                this.pureAudioStyle = bool.Parse(param);

            // 混流配置
            param = storage.GetValue(INI_ROOT_KEY, INI_KEY_CLOUD_MIX_TRANSCODING);
            if (string.IsNullOrEmpty(param))
                this.isMixTranscoding = false;
            else
                this.isMixTranscoding = bool.Parse(param);

        }

        public void Uninit()
        {
            WriteConfig();
        }

        private void WriteConfig()
        {
            storage.SetValue(INI_ROOT_KEY, INI_KEY_USER_ID, userId);
            storage.SetValue(INI_ROOT_KEY, INI_KEY_ROOM_ID, roomId.ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_ROLE_TYPE, ((int)roleType).ToString());

            storage.SetValue(INI_ROOT_KEY, INI_KEY_VIDEO_BITRATE, videoEncParams.videoBitrate.ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_VIDEO_FPS, videoEncParams.videoFps.ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_VIDEO_RESOLUTION, ((int)videoEncParams.videoResolution).ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_VIDEO_RES_MODE, ((int)videoEncParams.resMode).ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_VIDEO_QUALITY, ((int)qosParams.preference).ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_VIDEO_QUALITY_CONTROL, ((int)qosParams.controlMode).ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_VIDEO_APP_SCENE, ((int)appScene).ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_SET_PUSH_SMALLVIDEO, pushSmallVideo.ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_SET_PLAY_SMALLVIDEO, playSmallVideo.ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_LOCAL_VIDEO_MIRROR, isLocalVideoMirror.ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_REMOTE_VIDEO_MIRROR, isRemoteVideoMirror.ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_BEAUTY_OPEN, isOpenBeauty.ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_BEAUTY_STYLE, ((int)beautyStyle).ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_BEAUTY_VALUE, beauty.ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_WHITE_VALUE, white.ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_RUDDINESS_VALUE, ruddiness.ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_VIDEO_FILL_MODE, ((int)videoFillMode).ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_VIDEO_ROTATION, ((int)videoRotation).ToString());

            storage.SetValue(INI_ROOT_KEY, INI_KEY_AUDIO_MIC_VOLUME, micVolume.ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_AUDIO_SPEAKER_VOLUME, speakerVolume.ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_SHOW_AUDIO_VOLUME, isShowVolume.ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_AUDIO_QUALITY, ((int)AudioQuality).ToString());

            storage.SetValue(INI_ROOT_KEY, INI_KEY_SET_NETENV_STYLE, testEnv.ToString());
            storage.SetValue(INI_ROOT_KEY, INI_KEY_ROOMCALL_STYLE, pureAudioStyle.ToString());

            storage.SetValue(INI_ROOT_KEY, INI_KEY_CLOUD_MIX_TRANSCODING, isMixTranscoding.ToString());
        }

        private static DataManager mInstance;
        //private TIMConvType kTIMConv_Group;
        private static readonly object padlock = new object();

        public static DataManager GetInstance()
        {
            if (mInstance == null)
            {
                lock (padlock)
                {
                    if (mInstance == null)
                    {
                        mInstance = new DataManager();
                    }
                }
            }
            return mInstance;
        }

        #region 用户相关
        public string userId { get; set; }

        public uint roomId { get; set; }

        public string strRoomId { get; set; }

        // 该字段只作用于直播模式
        public TRTCRoleType roleType { get; set; }

        public bool enterRoom { get; set; }
        #endregion

        #region 视频相关
        public TRTCVideoEncParam videoEncParams { get; set; }

        public TRTCNetworkQosParam qosParams { get; set; }

        public TRTCAppScene appScene { get; set; }

        public TRTCVideoFillMode videoFillMode { get; set; }
        public TRTCVideoRotation videoRotation { get; set; }
        public bool isLocalVideoMirror { get; set; }

        public bool pushSmallVideo { get; set; }
        public bool playSmallVideo { get; set; }


        public bool isRemoteVideoMirror { get; set; }

        public bool isMixTranscoding { get; set; }

        public TRTCRenderParams GetRenderParams()
        {
            TRTCRenderParams renderParams = new TRTCRenderParams
            {
                fillMode = videoFillMode,
                mirrorType = isLocalVideoMirror ?
                    TRTCVideoMirrorType.TRTCVideoMirrorType_Enable : TRTCVideoMirrorType.TRTCVideoMirrorType_Disable,
                rotation = videoRotation
            };
            return renderParams;
        }
        #endregion

        #region 音频相关
        public uint micVolume { get; set; }

        public uint speakerVolume { get; set; }

        public bool isShowVolume { get; set; }
        public TRTCAudioQuality AudioQuality { get; set; }
        #endregion

        #region 美颜相关
        public bool isOpenBeauty { get; set; }

        public TRTCBeautyStyle beautyStyle { get; set; }

        public uint beauty { get; set; }

        public uint white { get; set; }

        public uint ruddiness { get; set; }
        #endregion

        #region 测试
        public int testEnv { get; set; }

        public bool pureAudioStyle { get; set; }
        #endregion
        
    }

    class AVFrameBufferInfo
    {
        public byte[] data { get; set; }

        public int width { get; set; }

        public int height { get; set; }

        public bool newFrame { get; set; }

        public TRTCVideoRotation rotation { get; set; }

        public AVFrameBufferInfo()
        {
            rotation = TRTCVideoRotation.TRTCVideoRotation0;
            newFrame = false;
            width = 0;
            height = 0;
            data = null;
        }
    }

    class RemoteUserInfo
    {
        public string userId { get; set; }
        
        public int position { get; set; }
    }

    class PKUserInfo
    {
        public string userId { get; set; }

        public uint roomId { get; set; }

        public bool isEnterRoom { get; set; }
    }

    class UserVideoMeta
    {
        public string userId { get; set; }
        public uint roomId { get; set; }
        public TRTCVideoStreamType streamType { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public uint fps { get; set; }
        public bool pureAudio { get; set; }
        public bool mainStream { get; set; }

        public UserVideoMeta()
        {
            userId = "";
            streamType = TRTCVideoStreamType.TRTCVideoStreamTypeBig;
            width = 0;
            height = 0;
            fps = 0;
            pureAudio = false;
            mainStream = false;
        }
    }

    class IniStorage
    {
        // 声明INI文件的写操作函数 
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        // 声明INI文件的读操作函数 
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

        private string sPath = null;

        public IniStorage(string path)
        {
            this.sPath = path;
        }

        public void SetValue(string section, string key, string value)
        {
            // section=配置节，key=键名，value=键值，path=路径
            WritePrivateProfileString(section, key, value, sPath);
        }

        public string GetValue(string section, string key)
        {
            // 每次从ini中读取多少字节
            System.Text.StringBuilder temp = new System.Text.StringBuilder(255);
            // section=配置节，key=键名，temp=上面，path=路径
            GetPrivateProfileString(section, key, "", temp, 255, sPath);
            return temp.ToString();
        }
    }
}
