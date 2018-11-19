using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalRWebDemo
{
    public class ServerHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        /// <summary>
        /// 1、广播类型
        ///     Clients.all 全体广播 
        ///     Clients.group 组播
        ///     广播排除
        ///     组播排除
        ///     指定用户触发
        /// </summary>
        /// <param name="msg"></param>
        public void SendMsg(string msg)
        {

            Clients.All.SendMessage(DateTime.Now.ToString(), msg);
        }

        public override Task OnConnected()
        {
            System.Diagnostics.Trace.WriteLine("客户端连接成功！");
            return base.OnConnected();
        }
    }
}