using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenueRtcWpf.IM
{
    /// <summary>
    /// 接口通用回调的定义
    /// </summary>
    /// <param name="code">值为 ERR_SUCC 表示成功，其他值表示失败。详情请参考 错误码</param>
    /// <param name="desc">错误描述字符串</param>
    /// <param name="json_params">JSON 字符串，不同的接口，JSON 字符串不一样</param>
    public delegate void TIMCommCallback(int code, string desc, string json_params, IntPtr user_data);
    /// <summary>
    /// 新消息回调
    /// </summary>
    /// <param name="json_msg_array">新消息数组</param>
    /// <param name="user_data"></param>
    public delegate void TIMRecvNewMsgCallback(string json_msg_array, IntPtr user_data);
    /// <summary>
    /// 消息已读回执回调
    /// </summary>
    /// <param name="json_msg_readed_receipt_array">消息已读回执数组</param>
    /// <param name="user_data"></param>
    public delegate void TIMMsgReadedReceiptCallback(string json_msg_readed_receipt_array, IntPtr user_data);
    /// <summary>
    /// 接收的消息被撤回回调
    /// </summary>
    /// <param name="json_msg_locator_array">消息定位符数组</param>
    /// <param name="user_data"></param>
    public delegate void TIMMsgRevokeCallback(string json_msg_locator_array, IntPtr user_data);
    /// <summary>
    /// 消息内元素相关文件上传进度回调
    /// </summary>
    /// <param name="json_msg">新消息</param>
    /// <param name="index">上传Elem元素在json_msg消息的下标</param>
    /// <param name="cur_size">上传当前大小</param>
    /// <param name="total_size">上传总大小</param>
    /// <param name="user_data"></param>
    public delegate void TIMMsgElemUploadProgressCallback(string json_msg,int index,int cur_size, int total_size, IntPtr user_data);

    /// <summary>
    /// 群事件回调
    /// </summary>
    /// <param name="json_group_tip_array">群提示列表</param>
    /// <param name="user_data"></param>
    public delegate void TIMGroupTipsEventCallback(string json_group_tip_array, IntPtr user_data);
    /// <summary>
    /// 会话事件回调
    /// </summary>
    /// <param name="conv_event">会话事件类型，请参考 TIMConvEvent</param>
    /// <param name="json_conv_array">会话信息列表</param>
    /// <param name="user_data"></param>
    public delegate void TIMConvEventCallback(TIMConvEvent conv_event, string json_conv_array, IntPtr user_data);

}
