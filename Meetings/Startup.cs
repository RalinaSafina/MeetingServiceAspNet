using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Meetings.Startup))]
namespace Meetings
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
