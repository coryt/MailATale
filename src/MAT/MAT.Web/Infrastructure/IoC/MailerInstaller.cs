using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MAT.Core.Mailers;

namespace MAT.Web.Infrastructure.IoC
{
    public class MailerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                AllTypes
                    .FromAssemblyNamed("MAT.Core")
                    .BasedOn<IUserMailer>()
                    .LifestyleTransient()
                    .WithService.Base());
        }
    }
}