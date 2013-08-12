using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MAT.Web.Infrastructure.Tasks;

namespace MAT.Web.Infrastructure.IoC
{
    public class TaskInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<GiftEmailerTask>().LifeStyle.Transient);
        }
    }
}