using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRWebDemo.Models
{
    /// <summary>
    /// 1、最简单的消息模型 标题与正文属性
    /// </summary>
    [Serializable]
    public class PushMessageModel
    {
        public int Id { get; set; }
        public String MSG_TITLE { get; set; }
        public String MSG_CONTENT { get; set; }
    }
}