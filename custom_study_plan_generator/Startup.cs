using Microsoft.Owin;
using Owin;

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
