using System.Web.Http.Controllers;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MAT.Web.Controllers;

namespace MAT.Web.Infrastructure.IoC
{
    internal class WebApiWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes
                                   .FromAssemblyContaining<SignUpController>()
                                   .BasedOn<IHttpController>()
                                   .LifestyleScoped());
        }
    }
}