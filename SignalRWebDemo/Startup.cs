using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SignalRWebDemo.Startup))]
namespace SignalRWebDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //配置集线器
            app.MapSignalR();
            //ConfigureAuth(app);
        }
    }
}
