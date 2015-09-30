using Microsoft.Owin;
using Owin;
using System.Net;
using System.Security.Cryptography.X509Certificates;

[assembly: OwinStartupAttribute(typeof(custom_study_plan_generator.Startup))]
namespace custom_study_plan_generator
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }

}
