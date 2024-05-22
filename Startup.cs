using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SkillsAssessment.Startup))]
namespace SkillsAssessment
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
